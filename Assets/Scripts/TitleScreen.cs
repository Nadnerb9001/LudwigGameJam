using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] GameObject aboutScreen;
    [SerializeField] GameObject settingScreen;
    [SerializeField] private AudioMixer mixer;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        float storedValue = PlayerPrefs.GetFloat("MasterVolume");
        mixer.SetFloat("MasterVolume", storedValue);
        storedValue = PlayerPrefs.GetFloat("BGMVolume");
        mixer.SetFloat("BGMVolume", storedValue);
        storedValue = PlayerPrefs.GetFloat("EffectsVolume");
        mixer.SetFloat("EffectsVolume", storedValue);
    }

    public void StartButton()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void AboutButton()
    {
        aboutScreen.SetActive(true);
    }

    public void SettingButton()
    {
        settingScreen.SetActive(true);
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
