using System;
using UnityEngine;
using UnityEngine.UIElements;

public class StoryModePresenter
{
    public Action OpenMainMenu { set => mainMenuButton.clicked += value; }
    public Action OpenMultiplayerMenu { set
        {
            continueButton.clicked += value;
            newGameButton.clicked += value;
        }
    }

    private Button newGameButton;
    private Button continueButton;
    private Button mainMenuButton;

    public StoryModePresenter(VisualElement root)
    {
        newGameButton = root.Q<Button>("NewGameButton");
        continueButton = root.Q<Button>("ContinueButton");
        mainMenuButton = root.Q<Button>("MainMenuButton");
    }
}
