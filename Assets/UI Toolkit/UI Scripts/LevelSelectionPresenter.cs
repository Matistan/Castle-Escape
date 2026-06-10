using System;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelSelectionPresenter
{
    public Action OpenMainMenu { set => mainMenuButton.clicked += value; }
    public Action OpenMultiplayerMenu
    {
        set
        {
            level1Button.clicked += value;
            level2Button.clicked += value;
            level3Button.clicked += value;
        }
    }

    private Button level1Button;
    private Button level2Button;
    private Button level3Button;
    private Button mainMenuButton;

    public LevelSelectionPresenter(VisualElement root)
    {
        level1Button = root.Q<Button>("Level1Button");
        level2Button = root.Q<Button>("Level2Button");
        level3Button = root.Q<Button>("Level3Button");
        mainMenuButton = root.Q<Button>("MainMenuButton");
    }
}
