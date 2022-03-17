using UnityEngine;
using System;
using System.Threading;
using FEN;
using IODriverNamespace;

public class Board2D : MonoBehaviour {

    // Prefabs
    [Header("Tile Prefabs")]
    [SerializeField] private GameObject lightTilePrefab;
    [SerializeField] private GameObject darkTilePrefab;

    [Header("Piece Prefabs")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Color[] teamColors;

    [Header("Materials")]
    [SerializeField] private Material lightMat;
    [SerializeField] private Material darkMat;
    [SerializeField] private Material hoverMat;

    //[Header("Sounds")]

    //IO
    IODriver mainDriver;// = new IODriver();
    private int[,] initial_bs;
    private int[,] final_bs;

    //Board
    [HideInInspector] public int TILE_COUNT_X = 8;
    [HideInInspector] public int TILE_COUNT_Y = 8;
    private const float TILE_OFFSET_X = -0.5f;
    private const float TILE_OFFSET_Y = -0.525f;
    private GameObject[,] tiles;
    private ChessPiece2D[,] chessPieces;

    //Piece Movement
    //private ChessPiece2D selectedPiece = null;
    private Vector2Int deselectValue = Vector2Int.one * -1;
    private Vector2Int selectedPiece = Vector2Int.one * -1;

    //Unity
    private Camera currentCamera;

    [Header("Board Settings")]
    //ChessManager
    public ChessManager chessManager;
    public BoardManager boardManager;


    // On Startup
    private void Awake() {
    }

    private void Start()
    {
        mainDriver = gameObject.AddComponent<IODriver>();

        chessPieces = new ChessPiece2D[TILE_COUNT_Y, TILE_COUNT_X];
        tiles = new GameObject[TILE_COUNT_Y, TILE_COUNT_X];

        chessPieces = new ChessPiece2D[TILE_COUNT_X, TILE_COUNT_Y];

        SetupTiles();
        DrawPieces();
    }

    //Every frame
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(initial_bs == null)
            {
                Debug.Log("1");
                initial_bs = mainDriver.boardToArray("1111111100000000000000001111111111111111000000000000000011111111");
            } else if(initial_bs != null && final_bs == null)
            {
                Debug.Log("2");
                final_bs = mainDriver.boardToArray("1111111100000000000000001111111111111110100000000000000011111111");
            } else if(initial_bs != null && final_bs != null)
            {
                Debug.Log("3");
                int[] test_move = mainDriver.getDifference(initial_bs, final_bs);
                Vector2Int initial_tile = new Vector2Int(test_move[0], test_move[1]);
                Vector2Int final_tile = new Vector2Int(test_move[2], test_move[3]);
                //MovePieceByV2I(initial_tile, final_tile);
                MovePiece(initial_tile, final_tile);
                initial_bs = null;
                final_bs = null;
            }
        }
 

        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile")) && Input.GetMouseButtonDown(0))
        {
            print(info.transform.gameObject);

            // Get the indexes of the tile i've hit
            Vector2Int hitPosition = GetTileIndex(info.transform.gameObject);
            print("Hit: " + hitPosition);
            if (chessPieces[hitPosition.x, hitPosition.y]) print("Piece: " + chessPieces[hitPosition.x, hitPosition.y].name);


            //if we click a tile that has a piece
            if (chessPieces[hitPosition.x, hitPosition.y] && selectedPiece == deselectValue)
            {
                selectedPiece = new Vector2Int(hitPosition.x, hitPosition.y);
                print("Selected: " + selectedPiece);
            }
            //else if we select the same piece again, deselect
            else if (hitPosition == selectedPiece)
            {
                print("Delected: " + selectedPiece);
                selectedPiece = deselectValue;
            }

            // If no piece is selected, exit the function
            if (selectedPiece == deselectValue) return;

            //If we have selected a piece and we are then selecting an empty tile 
            if (selectedPiece != deselectValue && chessPieces[hitPosition.x, hitPosition.y] == null)
            {
                print("Attempting move: " + selectedPiece + " -> " + hitPosition);
                Debug.Log(MovePiece(selectedPiece, hitPosition));
                selectedPiece = deselectValue;

            }
            //else if we select a piece with the opposite team, destroy opponent piece
            else if (chessPieces[hitPosition.x, hitPosition.y].team != chessPieces[selectedPiece.x, selectedPiece.y].team)
            {
                //chessPieces[hitPosition.x, hitPosition.y].DestroyChessPiece();
                print("Attempting take: " + selectedPiece + " -> " + hitPosition);
                MovePiece(selectedPiece, hitPosition);
                selectedPiece = deselectValue;
            }
        }



    }

    // Set up the tiles 
    private void SetupTiles()
    {
        tiles = new GameObject[TILE_COUNT_X, TILE_COUNT_Y];

        bool colored = false;

        for (int y = 0; y < TILE_COUNT_Y; y++)
        {
            for (int x = 0; x < TILE_COUNT_X; x++)
            {
                tiles[x, y] = DrawSingleTile(x, y, colored);
                colored = !colored;
            }
            colored = !colored;
        }
    }

    // Draw single tile
    private GameObject DrawSingleTile(int x, int y, bool colored)
    {
        Vector3 newPos = new Vector3(x + 0.5f, TILE_COUNT_Y - y - 0.5f, 0);

        GameObject tileObject = Instantiate(colored ? darkTilePrefab : lightTilePrefab, newPos, Quaternion.identity);

        tileObject.transform.parent = transform;

        tileObject.name = "X: " + x + ", Y: " + y;

        return tileObject;
    }

    // Set up all pieces
    private void DrawPieces()
    {   
        for (int y = 0; y < TILE_COUNT_Y; y++)
        {
            for (int x = 0; x < TILE_COUNT_X; x++)
            {
                if(chessManager.board_state[x,y] != '-')
                {
                    char temp = chessManager.board_state[x, y];
                    chessPieces[x,y] = DrawSinglePiece((ChessPieceType)(int)Enum.Parse(typeof(ChessPieceType), Char.ToString(Char.ToLower(temp))), temp);
                    chessPieces[x,y].transform.position = new Vector3(x - TILE_OFFSET_X , 7 - y - TILE_OFFSET_Y, 0);
                    chessPieces[x,y].row = y;
                    chessPieces[x,y].col = x;
                } 
            }
        }
    }

    //Draw single piece
    private ChessPiece2D DrawSinglePiece(ChessPieceType type, char team)
    {
        ChessPiece2D cp = Instantiate(prefabs[(int)type-1], transform).GetComponent<ChessPiece2D>();
        
        cp.type = type;

        if (Char.IsUpper(team))
            cp.team = 0;
        else
            cp.team = 1;

        cp.GetComponent<SpriteRenderer>().color = teamColors[cp.team];

        cp.transform.localScale = new Vector3(1.85f, 1.85f, 1);
        
        return cp;
    }

    private Vector2Int GetTileIndex(GameObject mouseInfo)
    {
        for (int y = 0; y < TILE_COUNT_Y; y++)
        {
            for (int x = 0; x < TILE_COUNT_X; x++)
            {
                if (tiles[x, y] == mouseInfo)
                    return new Vector2Int(x, y);
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

    //private string MovePiece(ChessPiece2D piece, Vector2Int square)
    //{

    //    string UCIReturnValue = string.Format("{0}{1}{2}{3}", piece.col, piece.row, square.y, square.x);

    //    chessPieces[piece.row, piece.col].transform.position = new Vector3(square.y - TILE_OFFSET_X, 7 - square.x - TILE_OFFSET_Y, 0);
    //    chessPieces[square.x, square.y] = chessPieces[piece.row, piece.col];
    //    char temp = chessManager.board_state[piece.row,piece.col];
    //    chessManager.board_state[piece.row,piece.col] = '-';
    //    chessManager.board_state[square.x, square.y] = temp;

    //    return ConvertToUCI(UCIReturnValue); ;

    //}

    //private string MovePieceByV2I(Vector2Int initial_tile, Vector2Int final_tile) 
    //{

    //    string returnString = string.Format("{0}{1}{2}{3}", initial_tile.x, initial_tile.y, final_tile.x, final_tile.y);

    //    chessPieces[initial_tile.x, initial_tile.y].transform.position = new Vector3(final_tile.y - TILE_OFFSET_X, 7 - final_tile.x - TILE_OFFSET_Y, 0);
    //    chessPieces[final_tile.x, final_tile.y] = chessPieces[initial_tile.x, initial_tile.y];
    //    char temp = chessManager.board_state[initial_tile.x, initial_tile.y];
    //    chessManager.board_state[initial_tile.x, initial_tile.y] = '-';
    //    chessManager.board_state[final_tile.x, final_tile.y] = temp;

    //    return returnString;

    //}

    public string MovePiece(Vector2Int initial_tile, Vector2Int final_tile)
    {
        string returnString = string.Format("From: X = {0}, Y = {1} -- To: X = {2}, Y = {3}", initial_tile.x, initial_tile.y, final_tile.x, final_tile.y);

        if (!boardManager.MovePiece(initial_tile, final_tile)) return "Illegal move! - " + returnString;
            
        if (chessPieces[final_tile.x, final_tile.y]) 
            chessPieces[final_tile.x, final_tile.y].DestroyChessPiece();

        chessPieces[initial_tile.x, initial_tile.y].transform.position = new Vector3(final_tile.x - TILE_OFFSET_X, 7 - final_tile.y - TILE_OFFSET_Y, 0);
        chessPieces[final_tile.x, final_tile.y] = chessPieces[initial_tile.x, initial_tile.y];
        chessPieces[initial_tile.x, initial_tile.y] = null;
        char temp = chessManager.board_state[initial_tile.x, initial_tile.y];
        chessManager.board_state[initial_tile.x, initial_tile.y] = '-';
        chessManager.board_state[final_tile.x, final_tile.y] = temp;

        return returnString;

    }

    private GameObject HighlightTile(Vector2Int tile, bool color)
    {

        GameObject selectedTile = tiles[tile.x, tile.y];

        // tileObject.AddComponent<MeshRenderer>().material = darkMat;
        if(color == true)
        {
            selectedTile.GetComponent<MeshRenderer>().material = hoverMat;
        }
        else
        {
            if(selectedTile.tag == "lightMat")
            {
                selectedTile.GetComponent<MeshRenderer>().material = lightMat;
            }
            else
            {
                selectedTile.GetComponent<MeshRenderer>().material = darkMat;
            }
        }
        return tiles[tile.x, tile.y];
    }

}