using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MultiplayerPresenter
{
    public Action OpenLanSettings { set => lanNetworkButton.clicked += value; }
    public Action OpenLocalGame { set => localButton.clicked += value; }
    public Action GoBack { set => goBackButton.clicked += value; }
    public Action OpenMainMenu { set => mainMenuButton.clicked += value; }

    private Button localButton;
    private Button lanNetworkButton;
    private Button goBackButton;
    private Button mainMenuButton;

    public MultiplayerPresenter(VisualElement root)
    {
        localButton = root.Q<Button>("LocalButton");
        lanNetworkButton = root.Q<Button>("LanNetworkButton");
        goBackButton = root.Q<Button>("GoBackButton");
        mainMenuButton = root.Q<Button>("MainMenuButton");
    }
}
