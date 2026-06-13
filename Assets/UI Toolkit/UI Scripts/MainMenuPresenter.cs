using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuPresenter
{
    public Action OpenSettings { set => settingsButton.clicked += value; }
    public Action OpenLevelSelection { set => freeModeButton.clicked += value; }
    public Action OpenStoryMode { set => storyModeButton.clicked += value; }

    private readonly Button storyModeButton;
    private readonly Button freeModeButton;
    private readonly Button settingsButton;
    private readonly Button quitButton;

    public MainMenuPresenter(VisualElement root)
    {
        storyModeButton = root.Q<Button>("StoryModeButton");
        freeModeButton = root.Q<Button>("FreeModeButton");
        settingsButton = root.Q<Button>("SettingsButton");
        quitButton = root.Q<Button>("QuitButton");

        freeModeButton.SetEnabled(CastleSaveManager.IsFreeModeUnlocked);
        quitButton.clicked += Application.Quit;
    }
}
