using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoardDriverNamespace
{

    public class BoardDriver : MonoBehaviour
    {

        int step;
        Vector2[,] positionArray;

        private int[,] initial_bs;
        private int[,] final_bs;

        // Start is called before the first frame update
        void Start()
        {

            positionArray = new Vector2[8, 8]
            {
                {new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0)},
                {new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0)},
                {new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0)},
                {new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0)},
                {new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0)},
                {new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0)},
                {new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0)},
                {new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0), new Vector2(0,0)}
            };

            initial_bs = new int[8, 8]
            {
                { 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1 }
            };

        }

        

    }


}

