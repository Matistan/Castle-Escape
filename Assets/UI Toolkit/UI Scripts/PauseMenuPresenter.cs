using System;
using UnityEngine;
using UnityEngine.UIElements;

public class PauseMenuPresenter
{
    public Action ResumeGame { set => resumeButton.clicked += value; }
    public Action RetryLevel { set => retryButton.clicked += value; }
    public Action OpenSettings { set => settingsButton.clicked += value; }
    public Action QuitToMainMenu { set => mainMenuButton.clicked += value; }
    
    private Button resumeButton;
    private Button retryButton;
    private Button settingsButton;
    private Button mainMenuButton;
    public PauseMenuPresenter(UnityEngine.UIElements.VisualElement root)
    {
        resumeButton = root.Q<UnityEngine.UIElements.Button>("ResumeButton");
        retryButton = root.Q<UnityEngine.UIElements.Button>("RetryButton");
        settingsButton = root.Q<UnityEngine.UIElements.Button>("SettingsButton");
        mainMenuButton = root.Q<UnityEngine.UIElements.Button>("MainMenuButton");
    }
}
