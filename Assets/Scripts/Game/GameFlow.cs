using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameMode
{
    Story,
    FreeMode
}

public static class GameFlow
{
    public const string MainMenuScene = "Main Menu";
    public const string Level1Scene = "Level 1";
    public const string Level2Scene = "Level 2";
    public const string Level3Scene = "Level 3";
    public const int TotalStoryLevels = 3;

    public static GameMode CurrentMode { get; set; } = GameMode.Story;
    public static int SelectedLevel { get; set; } = 1;

    public static string GetLevelSceneName(int level)
    {
        return level switch
        {
            1 => Level1Scene,
            2 => Level2Scene,
            3 => Level3Scene,
            _ => Level1Scene
        };
    }

    public static void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(MainMenuScene);
    }

    public static void LoadSelectedLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(GetLevelSceneName(SelectedLevel));
    }

    public static void ReloadCurrentLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static void StartStoryNewGame()
    {
        CurrentMode = GameMode.Story;
        SelectedLevel = 1;
        SaveManager.ResetStoryProgress();
        LoadSelectedLevel();
    }

    public static void ContinueStory()
    {
        CurrentMode = GameMode.Story;
        SelectedLevel = Mathf.Clamp(SaveManager.StoryProgressLevel, 1, TotalStoryLevels);
        LoadSelectedLevel();
    }

    public static void StartFreeModeLevel(int level)
    {
        CurrentMode = GameMode.FreeMode;
        SelectedLevel = level;
        LoadSelectedLevel();
    }

    public static void StartLocalGame()
    {
        CurrentMode = GameMode.Story;
        SelectedLevel = 1;
        LoadSelectedLevel();
    }
}
