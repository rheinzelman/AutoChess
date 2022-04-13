using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using IODriverNamespace;
using StockfishHandlerNamespace;
using AutoChess;

namespace RecordedGameNamespace
{
    public class RecordedGame : MonoBehaviour
    {
        [Header("Game File")]
        [SerializeField] private TextAsset gameFile;

        Stack<Vector2Int[]> futureMoves = new Stack<Vector2Int[]>();
        Stack<Vector2Int[]> pastMoves = new Stack<Vector2Int[]>();

        private void Start()
        {
            // create a string containing the whole gamefile contents
            string fileContents = gameFile.ToString();
            // split the gamefile contents by space
            string[] movesStrings = fileContents.Split(' ');
            // in reverse, put the moves onto the futuremoves stack 
            for(int i = movesStrings.Length - 1; i >= 0; i--)
            {
                string convertedMove = toVector2IntString(movesStrings[i]);
                Vector2Int[] moveVector = new Vector2Int[2];
                moveVector[0] = new Vector2Int(convertedMove[0] - '0', convertedMove[1] - '0');
                moveVector[1] = new Vector2Int(convertedMove[2] - '0', convertedMove[3] - '0');
                futureMoves.Push(moveVector);
            }   

        }

        private string toVector2IntString(string input)
        {
            string initial_square = input.Substring(0, 2);
            string final_square = input.Substring(2,2);

            string output = "";

            output += (input[0] - 'a');
            output += 8 - (input[1] - '0');
            output += (input[2] - 'a');
            output +=  8- (input[3] - '0');

            return output;

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if(futureMoves.Count > 0) NextMove();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if(pastMoves.Count > 0) PreviousMove();
            }
        }

        //send the move, and push a pop of futureMoves onto pastMoves
        public void NextMove()
        {
            Vector2Int[] move = futureMoves.Peek();
            //this.GetComponent<Board2D>().MovePiece(move[0], move[1], true);
            pastMoves.Push(futureMoves.Pop());
        }

        //send the move, and push a pop of pastMoves onto futureMoves
        public void PreviousMove()
        {
            Vector2Int[] move = pastMoves.Peek();
            //this.GetComponent<Board2D>().MovePiece(move[0], move[1], true);
            futureMoves.Push(pastMoves.Pop());  
        }

    }

}

