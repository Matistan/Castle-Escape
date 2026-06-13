using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainGamePresenter : MonoBehaviour
{
    private Dictionary<string, VisualElement> views = new Dictionary<string, VisualElement>();
    private Stack<VisualElement> history = new Stack<VisualElement>();
    private VisualElement currentView;

    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        RegisterView("Game", root.Q<VisualElement>("GameUI"));
        RegisterView("PauseScreen", root.Q<VisualElement>("PauseScreenUI"));
        RegisterView("Settings", root.Q<VisualElement>("SettingsUI"));

        SetupSubPresenters();

        ShowView("Game");
    }

    private void RegisterView(string name, VisualElement element)
    {
        if (element != null)
        {
            views.Add(name, element);
            element.style.display = DisplayStyle.None;
        }
    }

    private void SetupSubPresenters()
    {
        var gamePresenter = new GamePresenter(views["Game"]);
        gamePresenter.OpenPause = () => ShowView("PauseScreen");

        var pauseMenuPresenter = new PauseMenuPresenter(views["PauseScreen"]);
        pauseMenuPresenter.ResumeGame = () => GoBack();
        pauseMenuPresenter.RetryLevel = () => GoBack();
        pauseMenuPresenter.OpenSettings = () => ShowView("Settings");
        pauseMenuPresenter.QuitToMainMenu = () => Debug.Log("Quit to Main Menu");

        var settingsPresenter = new SettingsPresenter(views["Settings"]);
        settingsPresenter.CloseSettings = () => GoBack();
    }

    public void ShowView(string viewName, bool clearHistory = false)
    {
        if (!views.ContainsKey(viewName)) return;

        if (clearHistory) history.Clear();
        else if (currentView != null) history.Push(currentView);

        if (currentView != null) currentView.style.display = DisplayStyle.None;

        currentView = views[viewName];
        currentView.style.display = DisplayStyle.Flex;
    }

    public void GoBack()
    {
        if (history.Count > 0)
        {
            if (currentView != null) currentView.style.display = DisplayStyle.None;

            currentView = history.Pop();
            currentView.style.display = DisplayStyle.Flex;
        }
    }
}
