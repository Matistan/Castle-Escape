using System;
using UnityEngine;
using UnityEngine.UIElements;

public class FinishLevelPresenter
{
    public Action NextLevel { set => nextLevelButton.clicked += value; }
    public Action RetryLevel { set => retryButton.clicked += value; }
    public Action ReturnToMainMenu { set => mainMenuButton.clicked += value; }

    private readonly VisualElement root;
    private readonly Label headerLabel;
    private readonly Label summaryLabel;
    private readonly Button nextLevelButton;
    private readonly Button retryButton;
    private readonly Button mainMenuButton;

    public FinishLevelPresenter(VisualElement rootElement)
    {
        root = rootElement;
        headerLabel = root.Q<Label>("Header");
        summaryLabel = root.Q<Label>("SummaryLabel");
        nextLevelButton = root.Q<Button>("NextLevelButton");
        retryButton = root.Q<Button>("RetryButton");
        mainMenuButton = root.Q<Button>("MainMenuButton");
    }

    public void ShowResults(CastleLevelResults results)
    {
        headerLabel.text = "Level Complete!";
        summaryLabel.text =
            $"Time: {FormatTime(results.ElapsedTime)}\n" +
            $"Collectibles: {results.CollectedCount}/{Mathf.Max(results.TotalCollectibles, results.CollectedCount)}\n" +
            $"Score: {results.Score}\n" +
            $"Stars: {results.Stars}/3";

        bool hasNextLevel = results.LevelIndex < CastleGameFlow.TotalStoryLevels;
        nextLevelButton.style.display = hasNextLevel ? DisplayStyle.Flex : DisplayStyle.None;
        root.style.display = DisplayStyle.Flex;
    }

    public void Hide()
    {
        root.style.display = DisplayStyle.None;
    }

    private static string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int wholeSeconds = Mathf.FloorToInt(seconds % 60f);
        int milliseconds = Mathf.FloorToInt((seconds * 1000f) % 1000f);
        return $"{minutes:00}:{wholeSeconds:00}:{milliseconds:000}";
    }
}
