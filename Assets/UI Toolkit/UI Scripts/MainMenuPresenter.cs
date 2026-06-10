using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuPresenter
{
    public Action OpenSettings { set => settingsButton.clicked += value; }
    public Action OpenLevelSelection { set => freeModeButton.clicked += value; }
    public Action OpenStoryMode { set => storyModeButton.clicked += value; }

    private Button storyModeButton;
    private Button freeModeButton;
    private Button settingsButton;
    private Button quitButton;

    public MainMenuPresenter(VisualElement root)
    {
        storyModeButton = root.Q<Button>("StoryModeButton");
        freeModeButton = root.Q<Button>("FreeModeButton");
        settingsButton = root.Q<Button>("SettingsButton");
        quitButton = root.Q<Button>("QuitButton");

        quitButton.clicked += QuitGame;
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
