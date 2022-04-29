using System;
using System.Collections;
using System.Collections.Generic;
using ChessGame;
using ChessGame.PlayerInputInterface;
using StockfishHandlerNamespace;
using UnityEngine;
using Utils;

public class ChessEngineInput : BaseInputHandler
{
    private string output;
    private string fen;
    private StockfishHandler stock;

    private void Start()
    {
        stock = gameObject.AddComponent<StockfishHandler>();
        fen = NotationsHandler.DEFAULT_FEN;
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.LeftShift)) return;
        
        output = stock.GetMove(fen);
        
        if (!TurnActive || output == "" ) return;

        var move = (NotationsHandler.UCIToCoordinate("" + output[0] + output[1]),
            NotationsHandler.UCIToCoordinate("" + output[2] + output[3]));
        
        print(output + " -> " + move.Item1 + ", " + move.Item2);

        if (!SendMove(move.Item1, move.Item2)) return;

        output = "";
    }

    public override void ReceiveMove(Vector2Int from, Vector2Int to, MoveEventData moveData)
    {
        fen = moveData.Fen;
    }
}
