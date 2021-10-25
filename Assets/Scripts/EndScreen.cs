using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    [SerializeField] Text pointsText;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnEnd(float time)
    {
        TimeSpan ts = TimeSpan.FromSeconds(time);
        pointsText.text = $"In only: {ts.ToString(@"hh\:mm\:ss")}";
    }

    public void MenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
