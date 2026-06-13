using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainGamePresenter : MonoBehaviour
{
    private readonly Dictionary<string, VisualElement> views = new Dictionary<string, VisualElement>();
    private readonly Stack<VisualElement> history = new Stack<VisualElement>();

    private VisualElement currentView;
    private GamePresenter gamePresenter;
    private FinishLevelPresenter finishLevelPresenter;
    private bool isPaused;

    private void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        RegisterView("Game", root.Q<VisualElement>("GameUI"));
        RegisterView("PauseScreen", root.Q<VisualElement>("PauseScreenUI"));
        RegisterView("Settings", root.Q<VisualElement>("SettingsUI"));
        RegisterView("FinishLevel", root.Q<VisualElement>("FinishLevelUI"));

        SetupSubPresenters();
        ShowView("Game", clearHistory: true);
        StartCoroutine(BindLevelManagerWhenReady());
    }

    private IEnumerator BindLevelManagerWhenReady()
    {
        while (LevelManager.Instance == null)
        {
            yield return null;
        }

        LevelManager.Instance.LevelCompleted += HandleLevelCompleted;
    }

    private void OnDestroy()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.LevelCompleted -= HandleLevelCompleted;
        }

        Time.timeScale = 1f;
    }

    private void Update()
    {
        UpdateHud();
        HandlePauseInput();
    }

    private void RegisterView(string name, VisualElement element)
    {
        if (element == null)
        {
            return;
        }

        views.Add(name, element);
        element.style.display = DisplayStyle.None;
    }

    private void SetupSubPresenters()
    {
        if (views.TryGetValue("Game", out VisualElement gameView))
        {
            gamePresenter = new GamePresenter(gameView);
            gamePresenter.OpenPause = TogglePause;
        }

        if (views.TryGetValue("PauseScreen", out VisualElement pauseView))
        {
            var pauseMenuPresenter = new PauseMenuPresenter(pauseView);
            pauseMenuPresenter.ResumeGame = ResumeGame;
            pauseMenuPresenter.RetryLevel = () => GameFlow.ReloadCurrentLevel();
            pauseMenuPresenter.OpenSettings = () => ShowView("Settings");
            pauseMenuPresenter.QuitToMainMenu = () => GameFlow.LoadMainMenu();
        }

        if (views.TryGetValue("Settings", out VisualElement settingsView))
        {
            var settingsPresenter = new SettingsPresenter(settingsView);
            settingsPresenter.CloseSettings = GoBack;
        }

        if (views.TryGetValue("FinishLevel", out VisualElement finishView))
        {
            finishLevelPresenter = new FinishLevelPresenter(finishView);
            finishLevelPresenter.NextLevel = HandleNextLevel;
            finishLevelPresenter.RetryLevel = () => GameFlow.ReloadCurrentLevel();
            finishLevelPresenter.ReturnToMainMenu = () => GameFlow.LoadMainMenu();
            finishLevelPresenter.Hide();
        }
    }

    private void UpdateHud()
    {
        if (gamePresenter == null || LevelManager.Instance == null || isPaused)
        {
            return;
        }

        gamePresenter.UpdateHud(
            LevelManager.Instance.ElapsedTime,
            LevelManager.Instance.CollectedCount,
            LevelManager.Instance.RegisteredCollectibles);
    }

    private void HandlePauseInput()
    {
        if (InputManager.Instance == null || LevelManager.Instance == null || LevelManager.Instance.IsCompleted)
        {
            return;
        }

        if (InputManager.Instance.PlayerOne.PausePressed || InputManager.Instance.PlayerTwo.PausePressed)
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        if (LevelManager.Instance != null && LevelManager.Instance.IsCompleted)
        {
            return;
        }

        if (isPaused && views.TryGetValue("PauseScreen", out VisualElement pauseView) && currentView == pauseView)
        {
            ResumeGame();
            return;
        }

        isPaused = true;
        LevelManager.Instance?.SetPaused(true);
        ShowView("PauseScreen");
    }

    private void ResumeGame()
    {
        isPaused = false;
        LevelManager.Instance?.SetPaused(false);
        GoBack();
    }

    private void HandleLevelCompleted(LevelResults results)
    {
        isPaused = true;
        finishLevelPresenter?.ShowResults(results);
        ShowView("FinishLevel", clearHistory: true);
    }

    public void ShowView(string viewName, bool clearHistory = false)
    {
        if (!views.ContainsKey(viewName))
        {
            return;
        }

        if (clearHistory)
        {
            history.Clear();
        }
        else if (currentView != null)
        {
            history.Push(currentView);
        }

        if (currentView != null)
        {
            currentView.style.display = DisplayStyle.None;
        }

        currentView = views[viewName];
        currentView.style.display = DisplayStyle.Flex;
    }

    private void HandleNextLevel()
    {
        int nextLevel = GameFlow.SelectedLevel + 1;
        if (nextLevel > GameFlow.TotalStoryLevels)
        {
            return;
        }

        GameFlow.SelectedLevel = nextLevel;
        GameFlow.LoadSelectedLevel();
    }

    public void GoBack()
    {
        if (history.Count == 0)
        {
            return;
        }

        if (currentView != null)
        {
            currentView.style.display = DisplayStyle.None;
        }

        currentView = history.Pop();
        currentView.style.display = DisplayStyle.Flex;
    }
}
