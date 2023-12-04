using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using CI.QuickSave;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UpdateScore : MonoBehaviour
{
    private TextMeshProUGUI text;

    void PerformUpdate()
    {
        List<(float, DateTime)> data;
        try
        {
            var reader = QuickSaveReader.Create("Scores");
            data = reader.Read<List<(float, System.DateTime)>>(BeatmapManager.Instance.currentPlayingBeatmap
                .levelSceneName) ?? new List<(float, DateTime)>();
        }
        catch (QuickSaveException e)
        {
            // TODO catch only doesnt exist errors
            data = new List<(float, DateTime)>();
        }

        data.Add((ScoreManager.Instance.score, System.DateTime.Now));
        var writer = QuickSaveWriter.Create("Scores");
        writer.Write(BeatmapManager.Instance.currentPlayingBeatmap.levelSceneName, data);
        writer.Commit();
        text.text = $"Score: {ScoreManager.Instance.score:F2}";
    }

    private void OnEnable()
    {
        text = GetComponent<TextMeshProUGUI>();
        BeatmapManager.Instance.OnFinish += PerformUpdate;
    }

    private void OnDisable()
    {
        BeatmapManager.Instance.OnFinish -= PerformUpdate;
    }
}