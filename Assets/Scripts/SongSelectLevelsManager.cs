using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SongSelectLevelsManager : MonoBehaviour
{
    [SerializeField] private List<SongBeatmap> levels;

    [SerializeField] private GameObject levelObjectPrefab;

    [SerializeField] private Image currentSelectedLevelImage;
    [SerializeField] private TextMeshProUGUI currentArtistNameText;
    [SerializeField] private TextMeshProUGUI currentSongNameText;
    [SerializeField] private TextMeshProUGUI currentSongScoreText;
    [SerializeField] private Button currentSongPlayButton;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var level in levels)
        {
            var newSongGO = Instantiate(levelObjectPrefab, transform);
            var songGOText = newSongGO.GetComponentInChildren<TextMeshProUGUI>();
            songGOText.text = level.songTitle;
            foreach (var image in newSongGO.GetComponentsInChildren<Image>())
            {
                if (image.gameObject == newSongGO)
                    continue;

                image.sprite = level.thumbnail;
                break;
            }

            var songButton = newSongGO.GetComponent<Button>();
            songButton.onClick.AddListener(() =>
            {
                ChangeLevel(level); // TODO might cause closure issues, check
            });
        }

        if (levels.Count > 0)
        {
            ChangeLevel(levels[0]);
        }
    }

    void ChangeLevel(SongBeatmap songBeatmap)
    {
        currentSelectedLevelImage.sprite = songBeatmap.thumbnail;
        currentSongNameText.text = songBeatmap.songTitle;
        currentArtistNameText.text = songBeatmap.songArtist;
        currentSongScoreText.text = "No Scores";
        currentSongPlayButton.onClick.RemoveAllListeners();
        currentSongPlayButton.onClick.AddListener(() =>
        {
            // TODO make level switching logic
            SceneManager.LoadScene(songBeatmap.levelSceneName);

        });
        BeatmapManager.Instance.currentPlayingBeatmap = songBeatmap;
        Conductor.Instance.hasBeforePlayDelay = false;
        Conductor.Instance.songSource.Stop();
        Conductor.Instance.PlayLoopPreview();
    }
}