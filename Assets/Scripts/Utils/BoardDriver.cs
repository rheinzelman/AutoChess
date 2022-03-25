using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoardDriverNamespace
{

    public class BoardDriver : MonoBehaviour
    {

        int step;
        Vector2[,] positionArray;

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

        }

        

    }


}

