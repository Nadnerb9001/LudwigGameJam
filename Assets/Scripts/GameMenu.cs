using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    private PlayerController controller;

    private void Start()
    {
        controller = GameObject.FindObjectOfType<PlayerController>().GetComponent<PlayerController>();
    }

    public void ContinueButton()
    {
        controller.HideGameMenu();
    }

    public void MenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitButton()
    {
        controller.SavePosition();
        Application.Quit();
    }
}
