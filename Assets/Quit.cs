using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Quit : MonoBehaviour
{
    public void QuitToMenu()
    {
        SceneManager.LoadScene("MobileMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
