using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class StartMenuPresenter : MonoBehaviour
{
    private Dictionary<string, VisualElement> views = new Dictionary<string, VisualElement>();
    private Stack<VisualElement> history = new Stack<VisualElement>();
    private VisualElement currentView;

    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        RegisterView("MainMenu", root.Q<VisualElement>("MainMenuUI"));
        RegisterView("Settings", root.Q<VisualElement>("SettingsUI"));
        RegisterView("StoryMode", root.Q<VisualElement>("StoryModeUI"));
        RegisterView("LevelSelection", root.Q<VisualElement>("LevelSelectionUI"));
        RegisterView("Multiplayer", root.Q<VisualElement>("MultiplayerUI"));
        RegisterView("LanSettings", root.Q<VisualElement>("LanSettingsUI"));

        SetupSubPresenters();

        ShowView("MainMenu", clearHistory: true);
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
        var mainMenuPresenter = new MainMenuPresenter(views["MainMenu"]);
        mainMenuPresenter.OpenSettings = () => ShowView("Settings");
        mainMenuPresenter.OpenLevelSelection = () => ShowView("LevelSelection");
        mainMenuPresenter.OpenStoryMode = () => ShowView("StoryMode");

        var settingsPresenter = new SettingsPresenter(views["Settings"]);
        settingsPresenter.CloseSettings = () => GoBack();

        var levelSelectionPresenter = new LevelSelectionPresenter(views["LevelSelection"]);
        levelSelectionPresenter.OpenMultiplayerMenu = () => ShowView("Multiplayer");
        levelSelectionPresenter.OpenMainMenu = () => GoBack();

        var storyModePresenter = new StoryModePresenter(views["StoryMode"]);
        storyModePresenter.OpenMainMenu = () => GoBack();
        storyModePresenter.OpenMultiplayerMenu = () => ShowView("Multiplayer");

        var multiplayerPresenter = new MultiplayerPresenter(views["Multiplayer"]);
        multiplayerPresenter.OpenMainMenu = () => ShowView("MainMenu", true);
        multiplayerPresenter.OpenLocalGame = () => Debug.Log("Start Local Game");
        multiplayerPresenter.OpenLanSettings = () => ShowView("LanSettings");
        multiplayerPresenter.GoBack = () => GoBack();

        var lanSettingsPresenter = new LanSettingsPresenter(views["LanSettings"]);
        lanSettingsPresenter.HostGame = () => Debug.Log("Host LAN Game");
        lanSettingsPresenter.JoinGame = () => Debug.Log("Join LAN Game");
        lanSettingsPresenter.GoBack = () => GoBack();
        lanSettingsPresenter.OpenMainMenu = () => ShowView("MainMenu", true);
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
