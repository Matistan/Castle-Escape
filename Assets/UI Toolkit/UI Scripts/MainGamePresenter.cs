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
        while (CastleLevelManager.Instance == null)
        {
            yield return null;
        }

        CastleLevelManager.Instance.LevelCompleted += HandleLevelCompleted;
    }

    private void OnDestroy()
    {
        if (CastleLevelManager.Instance != null)
        {
            CastleLevelManager.Instance.LevelCompleted -= HandleLevelCompleted;
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
            pauseMenuPresenter.RetryLevel = () => CastleGameFlow.ReloadCurrentLevel();
            pauseMenuPresenter.OpenSettings = () => ShowView("Settings");
            pauseMenuPresenter.QuitToMainMenu = () => CastleGameFlow.LoadMainMenu();
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
            finishLevelPresenter.RetryLevel = () => CastleGameFlow.ReloadCurrentLevel();
            finishLevelPresenter.ReturnToMainMenu = () => CastleGameFlow.LoadMainMenu();
            finishLevelPresenter.Hide();
        }
    }

    private void UpdateHud()
    {
        if (gamePresenter == null || CastleLevelManager.Instance == null || isPaused)
        {
            return;
        }

        gamePresenter.UpdateHud(
            CastleLevelManager.Instance.ElapsedTime,
            CastleLevelManager.Instance.CollectedCount,
            CastleLevelManager.Instance.RegisteredCollectibles);
    }

    private void HandlePauseInput()
    {
        if (CastleInputManager.Instance == null || CastleLevelManager.Instance == null || CastleLevelManager.Instance.IsCompleted)
        {
            return;
        }

        if (CastleInputManager.Instance.PlayerOne.PausePressed || CastleInputManager.Instance.PlayerTwo.PausePressed)
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        if (CastleLevelManager.Instance != null && CastleLevelManager.Instance.IsCompleted)
        {
            return;
        }

        if (isPaused && views.TryGetValue("PauseScreen", out VisualElement pauseView) && currentView == pauseView)
        {
            ResumeGame();
            return;
        }

        isPaused = true;
        CastleLevelManager.Instance?.SetPaused(true);
        ShowView("PauseScreen");
    }

    private void ResumeGame()
    {
        isPaused = false;
        CastleLevelManager.Instance?.SetPaused(false);
        GoBack();
    }

    private void HandleLevelCompleted(CastleLevelResults results)
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
        int nextLevel = CastleGameFlow.SelectedLevel + 1;
        if (nextLevel > CastleGameFlow.TotalStoryLevels)
        {
            return;
        }

        CastleGameFlow.SelectedLevel = nextLevel;
        CastleGameFlow.LoadSelectedLevel();
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
