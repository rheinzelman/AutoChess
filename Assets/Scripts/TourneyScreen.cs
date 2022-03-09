using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TourneyScreen : MonoBehaviour
{
    // Start is called before the first frame update
    public string titleScene;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Title()
    {
        SceneManager.LoadScene(titleScene);
    }
}
