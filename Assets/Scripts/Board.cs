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

    [Header("Sounds")]

    //Board
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private const float TILE_OFFSET_X = -0.5f;
    private const float TILE_OFFSET_Y = -0.525f;
    private GameObject[,] tiles = new GameObject[TILE_COUNT_Y, TILE_COUNT_X];
    private ChessPiece[,] chessPieces = new ChessPiece[TILE_COUNT_Y, TILE_COUNT_X];
    private string DEFAULT_FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    private char[,] board_state;

    //Piece Movement
    private ChessPiece selectedPiece = null;

    //Unity
    private Camera currentCamera;


    // On Startup
    private void Awake() {

        chessPieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];
        FENHandler FENObject = new FENHandler(DEFAULT_FEN);

        board_state = FENObject.getArray();

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
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile")) && Input.GetMouseButtonDown(0))
        {
            // Get the indexes of the tile i've hit
            Vector2Int hitPosition = GetTileIndex(info.transform.gameObject);

            //if we click a tile that has a piece
            if (chessPieces[hitPosition.x, hitPosition.y])
            {
                selectedPiece = chessPieces[hitPosition.x, hitPosition.y];
                
            }
            //If we have selected a piece and we are then selecting an empty tile 
            if (chessPieces[hitPosition.x, hitPosition.y] == null && selectedPiece != null)
            {
                Debug.Log("Row: " + selectedPiece.row + " Col: " + selectedPiece.col);
                MovePiece(selectedPiece, hitPosition);
                DrawPieces();
                selectedPiece = null;
            }

        }
        
    }

    // Draw Tiles
    private void DrawTiles(float tileSize, int tileCountX, int tileCountY) {
        tiles = new GameObject[tileCountX, tileCountY];
        bool colored = false;
        for (int i = 0; i < tileCountY; i++) {
            for (int j = 0; j < tileCountX; j++) {
                tiles[i, j] = DrawSingleTile(tileSize, i, j, colored);
                colored = !colored;
            }
            colored = !colored;
        }
    }

    // Draw Single Tile
    private GameObject DrawSingleTile(float tileSize, int i, int j, bool colored) {
        GameObject tileObject = new GameObject(string.Format("Y:{0}, X:{1}", i, j));
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
        vertices[0] = new Vector3(j * tileSize,TILE_COUNT_X - i * tileSize); //topleft
        vertices[1] = new Vector3((j + 1) * tileSize, TILE_COUNT_X - i * tileSize); //topright
        vertices[2] = new Vector3(j * tileSize, TILE_COUNT_Y - (i + 1) * tileSize); //bottomleft
        vertices[3] = new Vector3((j + 1) * tileSize, TILE_COUNT_Y - (i + 1) * tileSize); //bottomright

        int[] tris = new int[] { 0 , 1, 2, 1, 3, 2 };

        mesh.vertices = vertices;
        mesh.triangles = tris;

        tileObject.AddComponent<BoxCollider>();

        tileObject.layer = LayerMask.NameToLayer("Tile");
        

        return tileObject;
    }

    //Draw All Pieces
    private void DrawPieces()
    {   
        for (int i = 0; i < TILE_COUNT_X; i++)
        {
            for (int j = 0; j < TILE_COUNT_Y; j++)
            {
                if(board_state[i,j] != '-')
                {
                    char temp = board_state[i, j];
                    chessPieces[i,j] = DrawSinglePiece((ChessPieceType)(int)Enum.Parse(typeof(ChessPieceType), Char.ToString(Char.ToLower(temp))), temp);
                    chessPieces[i,j].transform.position = new Vector3(j - TILE_OFFSET_X ,7-i-TILE_OFFSET_Y, 0);
                    chessPieces[i,j].row = i;
                    chessPieces[i,j].col = j;
                }
            }
        }
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

    private void MovePiece(ChessPiece piece, Vector2Int square)
    {
        //Debug.Log("Row: " + piece.row + "Col: " + piece.col);
        //Debug.Log("Row: " + square.x + "Col: " + square.y);

        char temp = board_state[piece.row,piece.col];
        board_state[piece.row,piece.col] = '-';
        board_state[square.x, square.y] = temp;
    }

}

/*
 for(int i = 0; i < 8; i++)
        {
            for(int j=0; j < 8; j++)
            {
                Debug.Log(string.Format("{0},{1}", i,j) + board_state[i,j]);
            }
        }
 */