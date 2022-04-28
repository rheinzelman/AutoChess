using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChessGame;
using TMPro;
using UnityEngine;
using Utils;

public class MoveHistory : MonoBehaviour
{
    public TextMeshProUGUI tmp;
    
    // Start is called before the first frame update
    void Start()
    {
        BoardManager.Instance.pieceMoved.AddListener(OnMove);
        if (tmp == null) tmp = GetComponent<TextMeshProUGUI>();
    }

    private void OnMove(Vector2Int f, Vector2Int t)
    {
        tmp.text += "" + NotationsHandler.CoordinateToUCI(f) + NotationsHandler.CoordinateToUCI(t) + ", ";
    }
}
