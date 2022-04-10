using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    public static SceneLoadManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        LoadScene(sceneName, false);
    }
    public void LoadSceneAdditive(string sceneName)
    {
        LoadScene(sceneName, true);
    }

    public void LoadScene(string sceneName, bool bLoadAdditive = false)
    {
        SceneManager.LoadScene(sceneName, bLoadAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
    }
}
