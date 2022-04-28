using System;
using System.Collections;
using System.Collections.Generic;
using ChessGame;
using DG.Tweening;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReceiveEndGame : MonoBehaviour
{
    public GameObject winScreenWhite;
    public GameObject winScreenBlack;
    public GameObject drawScreen;
    
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.onGameOver.AddListener(ReceiveEnd);
    }

    private void ReceiveEnd(EndState end, PlayerColor color)
    {
        GetComponent<Image>().enabled = true;
        
        switch (color)
        {
            case PlayerColor.Unassigned:
                drawScreen.SetActive(true);
                break;
            case PlayerColor.White:
                winScreenWhite.SetActive(true);
                break;
            case PlayerColor.Black:
                winScreenBlack.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(color), color, null);
            
        }

        DOVirtual.DelayedCall(5f, () => SceneManager.LoadScene("MobileMenu"));
    }
}
