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
}
