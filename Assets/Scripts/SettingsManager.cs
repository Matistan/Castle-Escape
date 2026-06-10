using UnityEngine;

public static class SettingsManager
{
    private const int defaultMasterVolume = 5;
    private const int defaultMusicVolume = 10;

    public static int MasterVolume
    {
        get => PlayerPrefs.GetInt("MasterVolume", defaultMasterVolume);
        set
        {
            PlayerPrefs.SetInt("MasterVolume", value);
            ApplyMasterVolume(value);
        }
    }

    public static int MusicVolume
    {
        get => PlayerPrefs.GetInt("MusicVolume", defaultMusicVolume);
        set
        {
            PlayerPrefs.SetInt("MusicVolume", value);
            ApplyMusicVolume(value);
        }
    }

    public static int SoundVolume
    {
        get => PlayerPrefs.GetInt("SoundVolume", defaultMusicVolume);
        set
        {
            PlayerPrefs.SetInt("SoundVolume", value);
            ApplySoundVolume(value);
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitializeSettings()
    {
        ApplyMasterVolume(MasterVolume);
        ApplyMusicVolume(MusicVolume);
        ApplySoundVolume(SoundVolume);

        Debug.Log("Settings Manager Automatically Initialized!");
    }

    private static void ApplyMasterVolume(int volume)
    {
        // Implement logic to apply master volume to the audio system
        Debug.Log($"Master Volume set to: {volume}");
    }

    private static void ApplyMusicVolume(int volume)
    {
        // Implement logic to apply music volume to the audio system
        Debug.Log($"Music Volume set to: {volume}");
    }

    private static void ApplySoundVolume(int volume)
    {
        // Implement logic to apply sound volume to the audio system
        Debug.Log($"Sound Volume set to: {volume}");
    }

    public static void SaveToDisk()
    {
        PlayerPrefs.Save();
    }
}
