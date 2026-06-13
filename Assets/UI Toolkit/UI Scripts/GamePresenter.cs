using System;
using UnityEngine;
using UnityEngine.UIElements;

public class GamePresenter
{
    public Action OpenPause { set => pauseButton.clicked += value; }

    private readonly Button pauseButton;
    private readonly Label scoreLabel;

    public GamePresenter(VisualElement root)
    {
        pauseButton = root.Q<Button>("PauseButton");
        scoreLabel = root.Q<Label>("ScoreLabel");
    }

    public void UpdateHud(float elapsedTime, int collectedCount, int totalCollectibles)
    {
        if (scoreLabel == null)
        {
            return;
        }

        scoreLabel.text = $"Time: {FormatTime(elapsedTime)}\nStars collected: {collectedCount}/{Mathf.Max(totalCollectibles, collectedCount)}";
    }

    private static string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int wholeSeconds = Mathf.FloorToInt(seconds % 60f);
        int milliseconds = Mathf.FloorToInt((seconds * 1000f) % 1000f);
        return $"{minutes:00}:{wholeSeconds:00}:{milliseconds:000}";
    }
}
