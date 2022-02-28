/*
 * 
 * Problems arising from GetMouseButton() I think. 
 * 
 */

using UnityEngine;
using System;
using FEN;
using IODriverNamespace;

public class Board : MonoBehaviour {

    [Header("Rendering")]
    [SerializeField] private Material lightMat;
    [SerializeField] private Material darkMat;
    [SerializeField] private Material hoverMat;

    [Header("Prefabs and Mats")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Color[] teamColors;

    [Header("Sounds")]

    //IO
    IODriver testDriver = new IODriver();
    

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

        int [,] test_bs = testDriver.boardToArray();

        
         for(int i = 0; i < 8; i++)
        {
            for(int j=0; j < 8; j++)
            {
                if(test_bs[i,j] == 1)
                {
                    Vector2Int input = new Vector2Int(i, j);
                    HighlightTile(input, true);
                }
                else
                {
                    Vector2Int input = new Vector2Int(i, j);
                    HighlightTile(input, false);
                }
            }
        }
 

        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile")) && Input.GetMouseButtonUp(0))
        {


            // Get the indexes of the tile i've hit
            Vector2Int hitPosition = GetTileIndex(info.transform.gameObject);

            //if we click a tile that has a piece
            if (chessPieces[hitPosition.x, hitPosition.y] && selectedPiece == null)
            {
                selectedPiece = chessPieces[hitPosition.x, hitPosition.y];
                
            } 
            //else if we select the same piece again, deselect
            else if (chessPieces[hitPosition.x, hitPosition.y] == selectedPiece)
            {
                selectedPiece = null;
            }
            //If we have selected a piece and we are then selecting an empty tile 
            if (selectedPiece != null && chessPieces[hitPosition.x, hitPosition.y] == null)
            {
                Debug.Log(MovePiece(selectedPiece, hitPosition));
                selectedPiece = null;

            } 
            //else if we select a piece with the opposite team, destroy opponent piece
            else if (chessPieces[hitPosition.x, hitPosition.y].team != selectedPiece.team)
            {
                chessPieces[hitPosition.x, hitPosition.y].DestroyChessPiece();
                MovePiece(selectedPiece, hitPosition);
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
    private ChessPiece  DrawSinglePiece(ChessPieceType type, char team)
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

    private string ConvertToUCI(string unconverted_string)
    {

        string returnValue = "";
        string letters = "abcdefg";

        for(int i = 0; i < unconverted_string.Length; i++)
        {
            if(i % 2 != 0)
            {
                returnValue += '8'-unconverted_string[i];
            }
            else
            {
                returnValue += letters[unconverted_string[i] - '0'];
            }
        }

        return returnValue;
    }

    private string MovePiece(ChessPiece piece, Vector2Int square)
    {

        string UCIReturnValue = string.Format("{0}{1}{2}{3}", piece.col, piece.row, square.y, square.x);

        chessPieces[piece.row, piece.col].transform.position = new Vector3(square.y - TILE_OFFSET_X, 7 - square.x - TILE_OFFSET_Y, 0);
        chessPieces[square.x, square.y] = chessPieces[piece.row, piece.col];
        char temp = board_state[piece.row,piece.col];
        board_state[piece.row,piece.col] = '-';
        board_state[square.x, square.y] = temp;

        return ConvertToUCI(UCIReturnValue); ;

    }

    private GameObject HighlightTile(Vector2Int tile, bool color)
    {
        // tileObject.AddComponent<MeshRenderer>().material = darkMat;
        if(color == true)
        {
            tiles[tile.x, tile.y].GetComponent<MeshRenderer>().material = hoverMat;
        }
        else
        {
            tiles[tile.x, tile.y].GetComponent<MeshRenderer>().material = lightMat;
        }
        return tiles[tile.x, tile.y];
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