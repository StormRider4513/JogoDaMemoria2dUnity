using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class AudioSettingsUI : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_Text valueMusic;
    [SerializeField] private TMP_Text valueSFX;

    private void OnEnable()
    {
        SyncSliders();
    }

    private void SyncSliders()
    {
        float music = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 1f);

        musicSlider.SetValueWithoutNotify(music);
        valueMusic.text = UpdateText(music);
        sfxSlider.SetValueWithoutNotify(sfx);
        valueSFX.text = UpdateText(sfx);
    }

    public void OnMusicChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
        valueMusic.text = UpdateText(value);
    }

    public void OnSFXChanged(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
        valueSFX.text = UpdateText(value);
    }

    private string UpdateText(float value)
    {
        return Mathf.RoundToInt(value * 100f) + "%";
    }
}
