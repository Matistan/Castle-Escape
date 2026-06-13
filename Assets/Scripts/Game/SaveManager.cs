using UnityEngine;

public static class SaveManager
{
    private const string StoryProgressKey = "StoryProgress";
    private const string StoryCompletedKey = "StoryCompleted";
    private const string FreeModeUnlockedKey = "FreeModeUnlocked";
    private const string BestTimePrefix = "BestTime_";
    private const string BestStarsPrefix = "BestStars_";
    private const string BestScorePrefix = "BestScore_";

    public static int StoryProgressLevel
    {
        get => PlayerPrefs.GetInt(StoryProgressKey, 1);
        set
        {
            PlayerPrefs.SetInt(StoryProgressKey, Mathf.Max(1, value));
            PlayerPrefs.Save();
        }
    }

    public static bool HasStoryProgress => StoryProgressLevel > 1 || IsLevelCompleted(1);

    public static bool IsStoryCompleted
    {
        get => PlayerPrefs.GetInt(StoryCompletedKey, 0) == 1;
        private set
        {
            PlayerPrefs.SetInt(StoryCompletedKey, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool IsFreeModeUnlocked
    {
        get => PlayerPrefs.GetInt(FreeModeUnlockedKey, 0) == 1;
        private set
        {
            PlayerPrefs.SetInt(FreeModeUnlockedKey, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static void ResetStoryProgress()
    {
        StoryProgressLevel = 1;
    }

    public static bool IsLevelCompleted(int level)
    {
        return GetBestStars(level) > 0;
    }

    public static bool IsLevelSelectableInFreeMode(int level)
    {
        if (!IsFreeModeUnlocked)
        {
            return false;
        }

        return IsFreeModeUnlocked && IsLevelCompleted(level);
    }

    public static float GetBestTime(int level)
    {
        return PlayerPrefs.GetFloat(BestTimePrefix + level, 0f);
    }

    public static int GetBestStars(int level)
    {
        return PlayerPrefs.GetInt(BestStarsPrefix + level, 0);
    }

    public static int GetBestScore(int level)
    {
        return PlayerPrefs.GetInt(BestScorePrefix + level, 0);
    }

    public static void RecordLevelCompletion(int level, float completionTime, int stars, int score)
    {
        if (GetBestStars(level) == 0 || completionTime < GetBestTime(level))
        {
            PlayerPrefs.SetFloat(BestTimePrefix + level, completionTime);
        }

        if (stars > GetBestStars(level))
        {
            PlayerPrefs.SetInt(BestStarsPrefix + level, stars);
        }

        if (score > GetBestScore(level))
        {
            PlayerPrefs.SetInt(BestScorePrefix + level, score);
        }

        if (GameFlow.CurrentMode == GameMode.Story && level >= StoryProgressLevel)
        {
            StoryProgressLevel = level + 1;
        }

        if (level >= GameFlow.TotalStoryLevels)
        {
            IsStoryCompleted = true;
            IsFreeModeUnlocked = true;
        }

        PlayerPrefs.Save();
    }
}
