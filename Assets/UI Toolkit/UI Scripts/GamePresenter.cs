using System;
using UnityEngine;
using UnityEngine.UIElements;

public class GamePresenter
{
    public Action OpenPause { set => pauseButton.clicked += value; }

    private Button pauseButton;
    private Label scoreLabel;

    public GamePresenter(VisualElement root)
    {
        pauseButton = root.Q<Button>("PauseButton");
        scoreLabel = root.Q<Label>("ScoreLabel");
    }
}
