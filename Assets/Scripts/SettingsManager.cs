using UnityEngine;

public static class SettingsManager
{
    private const int defaultMasterVolume = 5;
    private const int defaultMusicVolume = 10;
    private const int defaultSoundVolume = 10;

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
        get => PlayerPrefs.GetInt("SoundVolume", defaultSoundVolume);
        set
        {
            PlayerPrefs.SetInt("SoundVolume", value);
            ApplySoundVolume(value);
        }
    }

    public static float SfxVolumeMultiplier => NormalizeVolume(SoundVolume);

    public static float MusicVolumeMultiplier => NormalizeVolume(MusicVolume);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitializeSettings()
    {
        ApplyMasterVolume(MasterVolume);
        ApplyMusicVolume(MusicVolume);
        ApplySoundVolume(SoundVolume);
    }

    public static void PlaySfx(AudioClip clip, Vector3 position, float volumeScale = 1f)
    {
        if (clip == null)
        {
            return;
        }

        AudioSource.PlayClipAtPoint(clip, position, volumeScale * SfxVolumeMultiplier);
    }

    public static void SaveToDisk()
    {
        PlayerPrefs.Save();
    }

    private static void ApplyMasterVolume(int volume)
    {
        AudioListener.volume = NormalizeVolume(volume);
    }

    private static void ApplyMusicVolume(int volume)
    {
        // Reserved for music playback when BGM is added.
    }

    private static void ApplySoundVolume(int volume)
    {
        // SFX use SfxVolumeMultiplier at play time.
    }

    private static float NormalizeVolume(int sliderValue)
    {
        return Mathf.Clamp01(sliderValue / 10f);
    }
}
