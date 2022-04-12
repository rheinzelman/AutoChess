using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ray : MonoBehaviour
{
    // Draw Single Tile
    //private GameObject DrawSingleTile(float tileSize, int i, int j, bool colored)
    //{
    //    GameObject tileObject = new GameObject(string.Format("Y:{0}, X:{1}", i, j));
    //    tileObject.transform.parent = transform;

    //    Mesh mesh = new Mesh();
    //    tileObject.AddComponent<MeshFilter>().mesh = mesh;

    //    if (colored == true)
    //    {
    //        tileObject.AddComponent<MeshRenderer>().material = darkMat;
    //        tileObject.tag = "darkMat";
    //    }
    //    else
    //    {
    //        tileObject.AddComponent<MeshRenderer>().material = lightMat;
    //        tileObject.tag = "lightMat";
    //    }

    //    Vector3[] vertices = new Vector3[4];
    //    vertices[0] = new Vector3(j * tileSize, TILE_COUNT_X - i * tileSize); //topleft
    //    vertices[1] = new Vector3((j + 1) * tileSize, TILE_COUNT_X - i * tileSize); //topright
    //    vertices[2] = new Vector3(j * tileSize, TILE_COUNT_Y - (i + 1) * tileSize); //bottomleft
    //    vertices[3] = new Vector3((j + 1) * tileSize, TILE_COUNT_Y - (i + 1) * tileSize); //bottomright

    //    int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

    //    mesh.vertices = vertices;
    //    mesh.triangles = tris;

    //    tileObject.AddComponent<BoxCollider>();

    //    tileObject.layer = LayerMask.NameToLayer("Tile");


    //    return tileObject;
    //}

    // Convert the board state portion of the FEN to an 8x8 array
    //public char[,] getArray()
    //{

    //    //initialize our 8x8 array to return
    //    char[,] result = new char[8, 8];

    //    //this keeps track of where we will be placing piece characters in the 8x8 array
    //    int arrayIndex = 0;

    //    //iterate through the entire board_state_str
    //    for (int i = 0; i < board_state.Length; i++)
    //    {
    //        //convert our position in the board_state_str into an 8x8 array readable form
    //        int arrayRow = arrayIndex % 8;
    //        int arrayCol = (int)Math.Floor((double)(arrayIndex / 8));

    //        //if we encounter a letter, assign it to the appropriate spot in the array and increment the arrayIndex
    //        if (Char.IsLetter(board_state[i]))
    //        {
    //            result[arrayCol, arrayRow] = board_state[i];
    //            arrayIndex++;
    //        }
    //        //if we encounter a slash do nothing. Here for completeness sake
    //        else if (board_state[i] == '/') { }
    //        //if we encounter a number, iterate the arrayIndex by that many times
    //        else if (Char.IsNumber(board_state[i]))
    //        {
    //            //what a load of bullshit, python would never hurt me like this
    //            for (int k = 0; k < (int)board_state[i] - '0'; k++)
    //            {
    //                result[arrayCol, arrayRow] = '-';
    //                arrayIndex++;
    //                arrayRow = arrayIndex % 8;
    //                arrayCol = (int)Math.Floor((double)(arrayIndex / 8));

    //            }

    //        }

    //    }

    //    return result;

    //}
}
