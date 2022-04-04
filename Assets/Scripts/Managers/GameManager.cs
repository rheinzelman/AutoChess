using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public bool playerTurn;
    public bool gameOver;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        playerTurn = true;
        gameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
