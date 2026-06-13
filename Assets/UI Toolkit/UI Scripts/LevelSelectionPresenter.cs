using System;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelSelectionPresenter
{
    public Action OpenMainMenu { set => mainMenuButton.clicked += value; }
    public Action<int> SelectLevel { set => BindLevelButtons(value); }

    private readonly Button level1Button;
    private readonly Button level2Button;
    private readonly Button level3Button;
    private readonly Button mainMenuButton;

    public LevelSelectionPresenter(VisualElement root)
    {
        level1Button = root.Q<Button>("Level1Button");
        level2Button = root.Q<Button>("Level2Button");
        level3Button = root.Q<Button>("Level3Button");
        mainMenuButton = root.Q<Button>("MainMenuButton");

        RefreshLevelButtons();
    }

    private void BindLevelButtons(Action<int> handler)
    {
        level1Button.clicked += () => handler?.Invoke(1);
        level2Button.clicked += () => handler?.Invoke(2);
        level3Button.clicked += () => handler?.Invoke(3);
    }

    private void RefreshLevelButtons()
    {
        ConfigureLevelButton(level1Button, 1);
        ConfigureLevelButton(level2Button, 2);
        ConfigureLevelButton(level3Button, 3);
    }

    private static void ConfigureLevelButton(Button button, int level)
    {
        if (button == null)
        {
            return;
        }

        bool selectable = CastleSaveManager.IsLevelSelectableInFreeMode(level);
        button.SetEnabled(selectable);

        if (!selectable)
        {
            button.text = $"Level {level}\nLocked";
            return;
        }

        float bestTime = CastleSaveManager.GetBestTime(level);
        int bestStars = CastleSaveManager.GetBestStars(level);
        button.text = $"Level {level}\nBest Time: {FormatTime(bestTime)}\nBest Score: {bestStars} Star(s)";
    }

    private static string FormatTime(float seconds)
    {
        if (seconds <= 0f)
        {
            return "00:00";
        }

        int minutes = Mathf.FloorToInt(seconds / 60f);
        int wholeSeconds = Mathf.FloorToInt(seconds % 60f);
        return $"{minutes:00}:{wholeSeconds:00}";
    }
}
