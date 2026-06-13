using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class StartMenuPresenter : MonoBehaviour
{
    private readonly Dictionary<string, VisualElement> views = new Dictionary<string, VisualElement>();
    private readonly Stack<VisualElement> history = new Stack<VisualElement>();
    private VisualElement currentView;

    private void Start()
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
        if (element == null)
        {
            Debug.LogError($"{nameof(StartMenuPresenter)} could not find view '{name}'.", this);
            return;
        }

        views.Add(name, element);
        element.style.display = DisplayStyle.None;
    }

    private void SetupSubPresenters()
    {
        if (views.TryGetValue("MainMenu", out VisualElement mainMenuView))
        {
            var mainMenuPresenter = new MainMenuPresenter(mainMenuView);
            mainMenuPresenter.OpenSettings = () => ShowView("Settings");
            mainMenuPresenter.OpenLevelSelection = () => ShowView("LevelSelection");
            mainMenuPresenter.OpenStoryMode = () => ShowView("StoryMode");
        }

        if (views.TryGetValue("Settings", out VisualElement settingsView))
        {
            var settingsPresenter = new SettingsPresenter(settingsView);
            settingsPresenter.CloseSettings = GoBack;
        }

        if (views.TryGetValue("LevelSelection", out VisualElement levelSelectionView))
        {
            var levelSelectionPresenter = new LevelSelectionPresenter(levelSelectionView);
            levelSelectionPresenter.OpenMainMenu = GoBack;
            levelSelectionPresenter.SelectLevel = level =>
            {
                if (CastleSaveManager.IsLevelSelectableInFreeMode(level))
                {
                    CastleGameFlow.StartFreeModeLevel(level);
                }
            };
        }

        if (views.TryGetValue("StoryMode", out VisualElement storyModeView))
        {
            var storyModePresenter = new StoryModePresenter(storyModeView);
            storyModePresenter.OpenMainMenu = GoBack;
            storyModePresenter.StartNewGame = CastleGameFlow.StartStoryNewGame;
            storyModePresenter.ContinueGame = CastleGameFlow.ContinueStory;
        }

        if (views.TryGetValue("Multiplayer", out VisualElement multiplayerView))
        {
            var multiplayerPresenter = new MultiplayerPresenter(multiplayerView);
            multiplayerPresenter.OpenMainMenu = () => ShowView("MainMenu", clearHistory: true);
            multiplayerPresenter.OpenLocalGame = CastleGameFlow.StartLocalGame;
            multiplayerPresenter.OpenLanSettings = () => ShowView("LanSettings");
            multiplayerPresenter.GoBack = GoBack;
        }

        if (views.TryGetValue("LanSettings", out VisualElement lanSettingsView))
        {
            var lanSettingsPresenter = new LanSettingsPresenter(lanSettingsView);
            lanSettingsPresenter.HostGame = () => Debug.Log("LAN hosting is not implemented yet.");
            lanSettingsPresenter.JoinGame = () => Debug.Log("LAN joining is not implemented yet.");
            lanSettingsPresenter.GoBack = GoBack;
            lanSettingsPresenter.OpenMainMenu = () => ShowView("MainMenu", clearHistory: true);
        }
    }

    public void ShowView(string viewName, bool clearHistory = false)
    {
        if (!views.TryGetValue(viewName, out VisualElement nextView))
        {
            Debug.LogError($"{nameof(StartMenuPresenter)} missing view '{viewName}'.", this);
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

        currentView = nextView;
        currentView.style.display = DisplayStyle.Flex;
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
