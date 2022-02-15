/* NOTES SECTION
 * 
 * need to figure out how to get dictionary to work instead of enum
 *
 * 
 * 
*/

using UnityEngine;
using System;
using FEN;

public class Board : MonoBehaviour {

    [Header("Rendering")]
    [SerializeField] private Material lightMat;
    [SerializeField] private Material darkMat;
    [SerializeField] private Material hoverMat;

    [Header("Prefabs and Mats")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Color[] teamColors;

    

    // Game Logic
    private ChessPiece[,] chessPieces;
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private const float TILE_OFFSET_X = -0.5f;
    private const float TILE_OFFSET_Y = -0.525f;
    private string DEFAULT_FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;


    // On Startup
    private void Awake() {
        DrawTiles(1, TILE_COUNT_X, TILE_COUNT_Y);
        DrawPieces();
    }

    //Every frame
    private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile")))
        {
            // Get the indexes of the tile i've hit
            Vector2Int hitPosition = GetTileIndex(info.transform.gameObject);

            // If we're hovering a tile after not hovering any tiles
            if (currentHover == -Vector2Int.one)
            {
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            // If we were already hovering a tile, change the previous one
            if (currentHover != hitPosition)
            {
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }
            else
            {
                if (currentHover != -Vector2Int.one)
                {
                    tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                    currentHover = -Vector2Int.one;
                }
            }
        }
    }

    // Draw Tiles
    private void DrawTiles(float tileSize, int tileCountX, int tileCountY) {
        tiles = new GameObject[tileCountX, tileCountY];
        bool colored = true;
        for (int i = 0; i < tileCountX; i++) {
            for (int j = 0; j < tileCountY; j++) {
                tiles[i, j] = DrawSingleTile(tileSize, i, j, colored);
                colored = !colored;
            }
            colored = !colored;
        }
    }

    // Draw Single Tile
    private GameObject DrawSingleTile(float tileSize, int i, int j, bool colored) {
        GameObject tileObject = new GameObject(string.Format("X:{0}, Y:{1}", i, j));
        tileObject.transform.parent = transform;

        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;

        if (colored == true)
        {
            tileObject.AddComponent<MeshRenderer>().material = darkMat;
        }
        else
        {
            tileObject.AddComponent<MeshRenderer>().material = lightMat;
        }

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(i * tileSize, j * tileSize); //topleft
        vertices[1] = new Vector3(i * tileSize, (j + 1) * tileSize); //topright
        vertices[2] = new Vector3((i + 1) * tileSize, j * tileSize); //bottomleft
        vertices[3] = new Vector3((i + 1) * tileSize, (j + 1) * tileSize); //bottomright

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2};

        mesh.vertices = vertices;
        mesh.triangles = tris;

        tileObject.AddComponent<BoxCollider>();

        tileObject.layer = LayerMask.NameToLayer("Tile");
        

        return tileObject;
    }

    //Draw All Pieces
    private void DrawPieces()
    {

        chessPieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];
        FENHandler FENObject = new FENHandler(DEFAULT_FEN);

        char[,] board_state = FENObject.getArray();

        int whiteTeam = 0;
        int blackTeam = 1;

        
        for (int i = 0; i < TILE_COUNT_X; i++)
        {
            for (int j = 0; j < TILE_COUNT_Y; j++)
            {
                if(board_state[i,j] != '-')
                {
                    char temp = board_state[i, j];
                    chessPieces[i,j] = DrawSinglePiece((ChessPieceType)(int)Enum.Parse(typeof(ChessPieceType), Char.ToString(Char.ToLower(temp))), temp);
                    chessPieces[i,j].transform.position = new Vector3(j - TILE_OFFSET_X, i - TILE_OFFSET_Y, 0);
                }
            }
        }

        //White team
        /*
        chessPieces[0, 0] = DrawSinglePiece(2, whiteTeam);
        chessPieces[0, 0].col = 0;
        chessPieces[0, 0].row = 0;
        chessPieces[0, 0].transform.position = new Vector3(5-TILE_OFFSET_X, 5-TILE_OFFSET_Y, 0);

        chessPieces[1, 0] = DrawSinglePiece(3, whiteTeam);
        chessPieces[1, 0].col = 1;
        chessPieces[1, 0].row = 0;
        chessPieces[1, 0].transform.position = new Vector3(chessPieces[1, 0].col - TILE_OFFSET_X, chessPieces[1, 0].row - TILE_OFFSET_Y, 0);
        */
    }

    //Draw Single Piece
    private ChessPiece DrawSinglePiece(ChessPieceType type, char team)
    {

        ChessPiece cp = Instantiate(prefabs[(int)type-1], transform).GetComponent<ChessPiece>();
        
        cp.type = type;
        if (Char.IsUpper(team))
        {
            cp.team = 0;
        }
        else
        {
            cp.team = 1;
        }
        cp.GetComponent<SpriteRenderer>().color = teamColors[cp.team];
        cp.transform.localScale = new Vector3(1.85f, 1.85f, 1);
        
        return cp;

    }

    private Vector2Int GetTileIndex(GameObject mouseInfo)
    {
        for (int i = 0; i < TILE_COUNT_X; i++)
        {
            for (int j = 0; j < TILE_COUNT_Y; j++)
            {
                if (tiles[i, j] == mouseInfo)
                    return new Vector2Int(i, j);
            }
        }
        return -Vector2Int.one;
    }

}