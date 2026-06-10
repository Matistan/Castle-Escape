using System;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsPresenter
{
    public Action CloseSettings { set => goBackButton.clicked += value; }

    private SliderInt masterVolumeSlider;
    private SliderInt musicVolumeSlider;
    private SliderInt SoundVolumeSlider;
    private Button goBackButton;

    public SettingsPresenter(VisualElement root)
    {
        masterVolumeSlider = root.Q<SliderInt>("MasterVolumeSlider");
        musicVolumeSlider = root.Q<SliderInt>("MusicVolumeSlider");
        SoundVolumeSlider = root.Q<SliderInt>("SoundVolumeSlider");
        goBackButton = root.Q<Button>("GoBackButton");

        ConfigureSlider(masterVolumeSlider, SettingsManager.MasterVolume);
        ConfigureSlider(musicVolumeSlider, SettingsManager.MusicVolume);
        ConfigureSlider(SoundVolumeSlider, SettingsManager.SoundVolume);

        masterVolumeSlider.RegisterValueChangedCallback(evt => SettingsManager.MasterVolume = (int)evt.newValue);
        musicVolumeSlider.RegisterValueChangedCallback(evt => SettingsManager.MusicVolume = (int)evt.newValue);
        SoundVolumeSlider.RegisterValueChangedCallback(evt => SettingsManager.SoundVolume = (int)evt.newValue);

        goBackButton.clicked += () => SettingsManager.SaveToDisk();
    }

    private void ConfigureSlider(SliderInt slider, int initialValue)
    {
        slider.value = initialValue;
    }
}
