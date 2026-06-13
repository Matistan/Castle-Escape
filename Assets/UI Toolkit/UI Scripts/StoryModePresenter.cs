using System;
using UnityEngine;
using UnityEngine.UIElements;

public class StoryModePresenter
{
    public Action OpenMainMenu { set => mainMenuButton.clicked += value; }
    public Action StartNewGame { set => newGameButton.clicked += value; }
    public Action ContinueGame { set => continueButton.clicked += value; }

    private readonly Button newGameButton;
    private readonly Button continueButton;
    private readonly Button mainMenuButton;

    public StoryModePresenter(VisualElement root)
    {
        newGameButton = root.Q<Button>("NewGameButton");
        continueButton = root.Q<Button>("ContinueButton");
        mainMenuButton = root.Q<Button>("MainMenuButton");

        continueButton.SetEnabled(CastleSaveManager.HasStoryProgress);
    }
}
