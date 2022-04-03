using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;
using UnityEngine;
using FENNamespace;

namespace StockfishHandlerNamespace
{

    public class StockfishHandler : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public string GetMove(string FENString)
        {

            
            var stockfish = new System.Diagnostics.Process();
            stockfish.StartInfo.FileName = "C:/Users/Ray/AutoChess/Assets/stockfish/stockfish.exe";
            stockfish.StartInfo.UseShellExecute = false;
            stockfish.StartInfo.CreateNoWindow = true;
            stockfish.StartInfo.RedirectStandardInput = true;
            stockfish.StartInfo.RedirectStandardOutput = true;
            stockfish.Start();

            string setupString = "position fen " + FENString;
            stockfish.StandardInput.WriteLine(setupString);

            string processString = "go movetime 500";

            string intro = stockfish.StandardOutput.ReadLine();

            stockfish.StandardInput.WriteLine(processString);

            string bestMove = stockfish.StandardOutput.ReadLine();

            while(bestMove.Contains("bestmove") != true)
            {
                bestMove = stockfish.StandardOutput.ReadLine();
            }

            stockfish.Close();

            string output = bestMove.Substring(9,4);

            return output;

        }

    }


}
