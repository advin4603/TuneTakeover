using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using OsuParsers.Beatmaps;
using OsuParsers.Beatmaps.Objects;
using Unity.VisualScripting;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using System.IO.Compression;
using OsuParsers.Beatmaps.Objects.Taiko;
using OsuParsers.Decoders;
using OsuParsers.Enums.Beatmaps;
using UnityEngine.Events;
using UnityEngine.Serialization;


[CreateAssetMenu]
public class SongBeatmap : ScriptableObject
{
    [Serializable]
    public class SongHitObject
    {
        [Serializable]
        public enum SongHitObjectType
        {
            Attack,
            Defend
        }

        public SongHitObjectType hitObjectType;
        public float time;
        public List<string> hitObjectEvent;
    }

    [Serializable]
    public class SongTimingPoint
    {
        public bool Inherited;
        public double time;
        public double millisecondsPerBeat;
    }

    public double initialOffset;
    public string songTitle;
    public string songArtist;
    public List<SongHitObject> hitObjects;
    public List<SongTimingPoint> timingPoints;
    public float beforePlayDelay;
    public float approachRate;
    
    public float previewTimeSeconds = 0;
    
    public int lateForgivenessTimeMilliseconds = 100;
    public int earlyForgivenessTimeMilliseconds = 100;
    public int despawnDelayMilliseconds = 200;
    public float enemyHitPower = 1;
    public float playerHitPower = 1;

    public string levelSceneName;
    public Sprite thumbnail;

    public List<string> commonHitObjectEvent = new List<string>{"Enemy Beat Action"};

    public AudioClip audioClip;

    [CanBeNull] public Beatmap osuBeatmap;

    static double Clamp(double value, double min, double max)
    {
        if (value > max)
            return max;
        if (value < min)
            return min;

        return value;
    }

    public double SecondsPerBeatAt(int millisecondTime)
    {
        if (timingPoints.Count == 0)
            return 0;

        int timingPoint = 0;
        int samplePoint = 0;
        for (int i = 0; i < timingPoints.Count; i++)
        {
            if (timingPoints[i].time <= millisecondTime)
            {
                if (timingPoints[i].Inherited)
                    samplePoint = i;
                else
                    timingPoint = i;
            }
        }


        double multiplier = 1;
        if (samplePoint > timingPoint && timingPoints[samplePoint].millisecondsPerBeat < 0)
        {
            multiplier = Clamp(-timingPoints[samplePoint].millisecondsPerBeat, 10, 1000) / 100f;
        }

        return timingPoints[timingPoint].millisecondsPerBeat * multiplier / 1000f;
    }
#if UNITY_EDITOR
    public bool ImportOszFile(string osuFilePath, string parentFolderName)
    {
        osuBeatmap = BeatmapDecoder.Decode(osuFilePath);
        if (osuBeatmap.GeneralSection.ModeId != 1)
        {
            Debug.LogError("Cannot import a non-taiko beatmap");
            return false;
        }

        foreach (var hitObject in osuBeatmap.HitObjects)
        {
            if (hitObject is TaikoHit)
            {
                var taikoHitObject = hitObject as TaikoHit;
                SongHitObject newHitObject = new SongHitObject();
                newHitObject.hitObjectType = taikoHitObject.Color == TaikoColor.Blue
                    ? SongHitObject.SongHitObjectType.Defend
                    : SongHitObject.SongHitObjectType.Attack;
                newHitObject.time = taikoHitObject.StartTime;
                hitObjects.Add(newHitObject);
            }
        }

        foreach (var timingPoint in osuBeatmap.TimingPoints)
        {
            SongTimingPoint songTimingPoint = new SongTimingPoint();
            songTimingPoint.time = timingPoint.Offset;
            songTimingPoint.millisecondsPerBeat = timingPoint.BeatLength;
            songTimingPoint.Inherited = timingPoint.Inherited;
            timingPoints.Add(songTimingPoint);
        }

        initialOffset = (double)osuBeatmap.GeneralSection.AudioLeadIn / 1000f;
        audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(
            $"Assets/BeatmapData/{parentFolderName}/{osuBeatmap.GeneralSection.AudioFilename}");
        songTitle = osuBeatmap.MetadataSection.TitleUnicode;
        songArtist = osuBeatmap.MetadataSection.ArtistUnicode;
        approachRate = osuBeatmap.DifficultySection.ApproachRate;
        return true;
    }
    
#endif
}

#if UNITY_EDITOR

[CustomEditor(typeof(SongBeatmap))]
public class SongBeatmapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SongBeatmap songBeatmap = (SongBeatmap)target;

        // Display the existing properties
        DrawDefaultInspector();

        GUILayout.Space(10);

        // Add a button for importing .osz file
        if (GUILayout.Button("Import .osz File"))
        {
            string oszFilePath = EditorUtility.OpenFilePanel("Select .osz File to Import", "", "osz");
            if (string.IsNullOrEmpty(oszFilePath))
            {
                return;
            }

            if (!File.Exists(oszFilePath))
            {
                Debug.LogError($"File does not exist: {oszFilePath}");
                return;
            }

            string extractFolderPath =
                $"{Application.dataPath}/BeatmapData/{Path.GetFileNameWithoutExtension(oszFilePath)}";
            if (Directory.Exists(extractFolderPath))
            {
                Directory.Delete(extractFolderPath, true);
            }

            Directory.CreateDirectory(extractFolderPath);

            try
            {
                ZipFile.ExtractToDirectory(oszFilePath, extractFolderPath);
            }
            catch (Exception exception)
            {
                Debug.LogError($"Error while extracting: {exception}");
                return;
            }

            AssetDatabase.Refresh();
            string osuFilePath = EditorUtility.OpenFilePanel("Choose .osu file to Import", extractFolderPath, "osu");

            if (!File.Exists(osuFilePath))
            {
                Debug.LogError($"File does not exist: {osuFilePath}");
                return;
            }


            if (songBeatmap.ImportOszFile(osuFilePath, Path.GetFileNameWithoutExtension(oszFilePath)))
            {
                EditorUtility.SetDirty(songBeatmap);
            }
        }
    }
}
#endif