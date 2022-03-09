using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{

    public string newGameScene;
    public string boardSyncScene;
    public string titleScene;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void NewGame() {
        SceneManager.LoadScene(newGameScene);
    }

    public void SyncGame() {
        SceneManager.LoadScene(boardSyncScene);
    }

    public void title()
    {
        SceneManager.LoadScene(titleScene);
    }
}
