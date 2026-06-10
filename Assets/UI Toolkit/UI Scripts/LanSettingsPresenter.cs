using System;
using UnityEngine;
using UnityEngine.UIElements;

public class LanSettingsPresenter
{
    public Action HostGame { set => hostButton.clicked += value; }
    public Action JoinGame { set => joinButton.clicked += value; }
    public Action GoBack { set => goBackButton.clicked += value; }
    public Action OpenMainMenu { set => mainMenuButton.clicked += value; }

    private Button hostButton;
    private Button joinButton;
    private Button goBackButton;
    private Button mainMenuButton;

    public LanSettingsPresenter(VisualElement root)
    {
        hostButton = root.Q<Button>("HostButton");
        joinButton = root.Q<Button>("JoinButton");
        goBackButton = root.Q<Button>("GoBackButton");
        mainMenuButton = root.Q<Button>("MainMenuButton");
    }
}
