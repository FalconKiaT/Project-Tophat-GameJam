using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    [Header("Levels to Load")]
    [SerializeField] public string loadScene;

    [Header("Volume Setting")]
    [SerializeField] private UnityEngine.UI.Slider volumeSlider = null;
    [SerializeField] private TMP_Text volumeText = null;
    [SerializeField] private float defaultVolume = 1.0f;

    [Header("Gameplay Settings")]
    [SerializeField] private TMP_Text gameplayText = null;
    [SerializeField] private UnityEngine.UI.Slider gameplaySlider = null;
    [SerializeField] private int defaultGameplay = 4;
    public int mainGameplay = 4;

    [Header("Graphics Settings")]
    [SerializeField] private TMP_Text graphicsText = null;
    [SerializeField] private UnityEngine.UI.Slider graphicsSlider = null;
    [SerializeField] private int defaultGraphics = 1;

    [Header("Toggle Settings")]
    [SerializeField] private UnityEngine.UI.Toggle invertYToggle = null;
    [SerializeField] private UnityEngine.UI.Toggle fullscreenToggle = null;

    [Header("Confirmation")]
    [SerializeField] private GameObject confirmationPrompt = null;

    private float _brightnesslevel;

    public void Exit()
    {
        Application.Quit();
    }

    public void SetSensitivity()
    {
        mainGameplay = Mathf.RoundToInt(gameplaySlider.value);
        gameplayText.text = gameplaySlider.value.ToString();
        ApplyGameplay();
    }

    public void ApplyGameplay() {
        if (invertYToggle.isOn)
        {
            PlayerPrefs.SetInt("masterInvertY", 1);
        }
        else
        {
            PlayerPrefs.SetInt("masterInvertY", 0);
        }

        PlayerPrefs.SetFloat("masterSen", mainGameplay);
        StartCoroutine(ConfirmationBox());
    }

    public void SetGraphics()
    {
        _brightnesslevel = graphicsSlider.value;
        graphicsText.text = System.Math.Round(_brightnesslevel, 2).ToString();
        ApplyGraphics();
    }

    public void ApplyGraphics()
    {
        if (fullscreenToggle.isOn)
        {
            PlayerPrefs.SetInt("masterFullscreen", 1);
        }
        else
        {
            PlayerPrefs.SetInt("masterFullscreen", 0);
        }
        Screen.fullScreen = fullscreenToggle.isOn;
        PlayerPrefs.SetFloat("masterBrightness", _brightnesslevel);
        StartCoroutine(ConfirmationBox());
    }
    public void SetVolume()
    {
        AudioListener.volume = volumeSlider.value;
        volumeText.text = System.Math.Round(volumeSlider.value, 2).ToString();
        VolumeApply();
    }

    public void ResetButton(string MenuType)
    {
        if(MenuType == "Sound")
        {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeText.text = defaultVolume.ToString();
            VolumeApply();
        }

        if (MenuType == "Gameplay")
        {
            gameplayText.text = defaultGameplay.ToString();
            gameplaySlider.value = defaultGameplay;
            mainGameplay = defaultGameplay;
            invertYToggle.isOn = false;
            ApplyGameplay();
        }

        if (MenuType == "Graphics")
        {
            graphicsText.text = defaultGraphics.ToString();
            graphicsSlider.value = defaultGraphics;
            fullscreenToggle.isOn = false;
            ApplyGameplay();
        }
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(ConfirmationBox());
    }

    public IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        confirmationPrompt.SetActive(false);
    }
}

