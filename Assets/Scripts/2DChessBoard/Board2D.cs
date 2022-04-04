using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Threading;
using FENNamespace;
using IODriverNamespace;
using StockfishHandlerNamespace;

public class Board2D : MonoBehaviour {

    // Prefabs
    [Header("Tile Prefabs")]
    [SerializeField] private GameObject lightTilePrefab;
    [SerializeField] private GameObject darkTilePrefab;
    [SerializeField] private GameObject legalTilePrefab;

    [Header("Piece Prefabs")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Color[] teamColors;

    [Header("Materials")]
    [SerializeField] private Material lightMat;
    [SerializeField] private Material darkMat;
    [SerializeField] private Material hoverMat;

    //[Header("Sounds")]

    //IO
    bool boardConnected = true;
    IODriver mainDriver;
    private int[,] initial_bs;
    private int[,] final_bs;

    //Board
    [HideInInspector] public int TILE_COUNT_X = 8;
    [HideInInspector] public int TILE_COUNT_Y = 8;
    private const float TILE_OFFSET_X = -0.5f;
    private const float TILE_OFFSET_Y = -0.525f;
    private const float COORD_OFFSET_X = 5F;
    private const float COORD_OFFSET_Y = 5F;
    private const float PIECE_SIZE = 1.75f;
    private GameObject[,] tiles;
    private ChessPiece2D[,] chessPieces;

    //stockfish test
    public StockfishHandler stockfishTest;

    //FEN test
    public FENHandler fenTest;

    //Piece Movement
    //private ChessPiece2D selectedPiece = null;
    private Vector2Int deselectValue = Vector2Int.one * -1;
    private Vector2Int selectedPiece = Vector2Int.one * -1;

    //Unity
    private Camera currentCamera;

    [Header("Board Settings")]

    //ager
    public ChessManager chessManager;
    public BoardManager boardManager;
    public GameManager gameManager;


    // On Startup
    private void Awake() {

    }

    private void Start()
    {
        //IO Diver initialization, initial board state is recorded when game is initialized 
        mainDriver = gameObject.AddComponent<IODriver>();
        //initial_bs = mainDriver.boardToArray();

        chessPieces = new ChessPiece2D[TILE_COUNT_Y, TILE_COUNT_X];
        tiles = new GameObject[TILE_COUNT_Y, TILE_COUNT_X];
        chessPieces = new ChessPiece2D[TILE_COUNT_X, TILE_COUNT_Y];

        SetupTiles();
        DrawPieces();
        DrawCoords();

        boardManager.pieceRemoved.AddListener(DestroyPieceObject);
        boardManager.pieceMoved.AddListener(TransferPiece);

        stockfishTest = gameObject.AddComponent<StockfishHandler>();
        fenTest = gameObject.AddComponent<FENHandler>();    

    }

    //Every frame
    private void Update()
    {
        /*
        if (boardConnected)
        {
            HighlightSquares();

            //when spacebar is pressed, attempt to grab physical board state and represent virtually
            if (Input.GetKeyDown(KeyCode.Space))
            {

                //grab the final board state
                final_bs = mainDriver.boardToArray();

                //compare initial and final board state
                int [,] difference_array = mainDriver.getDifferenceArray(initial_bs, final_bs);

                //if there was a piece moved to an empty space
                if(mainDriver.checkDifference(difference_array) == 1)
                {
                    List<Vector2Int> physical_move = mainDriver.getMoveFromDifferenceArray(difference_array);
                    MovePiece(physical_move[0], physical_move[1]);
                } 
                //if a piece was moved to the capture square
                else if(mainDriver.checkDifference(difference_array) == 2 && mainDriver.capturedPiece() == true)
                {

                    //to piece = -1 spot



                } else if(mainDriver.checkDifference(difference_array) == 0)
                {
                    print("checkDifference error, move pieces back");
                }
                else
                {
                    print("unknown board read error");
                }
                   
            }
        }*/

        

        if (Input.GetKeyDown(KeyCode.Space))
        {

            //Debug.Log(fenTest.getCurrentFEN(chessManager.board_state));
            mainDriver.test();

        }

        if (Input.GetKeyDown(KeyCode.A))
        {
           
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

            // Get the indexes of the tile i've hit
            Vector2Int hitPosition = GetTileIndex(info.transform.gameObject);

            //if we click a tile that has a piece
            if (chessPieces[hitPosition.x, hitPosition.y] && selectedPiece == deselectValue)
            {
                selectedPiece = new Vector2Int(hitPosition.x, hitPosition.y);
                HighlightLegalTiles(selectedPiece, true);
                HighlightTile(hitPosition.x, hitPosition.y, true);
            }
            //else if we select the same piece again, deselect
            else if (hitPosition == selectedPiece)
            {
                HighlightLegalTiles(selectedPiece, false);
                HighlightTile(hitPosition.x, hitPosition.y, false);
                selectedPiece = deselectValue;
            }
            //If we have selected a piece and we are then selecting an empty tile 
            else if (selectedPiece != deselectValue && chessPieces[hitPosition.x, hitPosition.y] == null)
            {
                HighlightLegalTiles(selectedPiece, false);
                HighlightTile(selectedPiece.x, selectedPiece.y, false);
                string UCIMove = MovePiece(selectedPiece, hitPosition);
                Debug.Log(UCIMove);
                mainDriver.performStandardMove(UCIMove.Substring(0,2), UCIMove.Substring(2,2));
                selectedPiece = deselectValue;
            }
            //else if we select a piece with the opposite team, destroy opponent piece

            else if (selectedPiece != deselectValue && chessPieces[hitPosition.x, hitPosition.y] != null && chessPieces[hitPosition.x, hitPosition.y].team != chessPieces[selectedPiece.x, selectedPiece.y].team)
            {
                HighlightLegalTiles(selectedPiece, false);
                HighlightTile(selectedPiece.x, selectedPiece.y, false);
                MovePiece(selectedPiece, hitPosition);
                selectedPiece = deselectValue;
            }
            // If no piece is selected, exit the function
            if (selectedPiece == deselectValue) return ;


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

        cp.transform.localScale = new Vector3(PIECE_SIZE, PIECE_SIZE, 1);
        
        return cp;
    }

    private void DrawCoords()
    {
        GameObject gameCanvas = GameObject.Find("UICanvas");
        for (int i = 0; i < 8; i++)
        {
            //Column Coordinates
            GameObject colTextGO = new GameObject("col coord " + i);
            colTextGO.transform.SetParent(gameCanvas.transform);
            Text colText = colTextGO.AddComponent<Text>();
            colText.text = (i+1).ToString();
            colText.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            colText.fontSize = 8;
            if(i % 2 == 0)
            {
                colText.color = lightMat.color;
            }
            else
            {
                colText.color = darkMat.color;
            }
            colText.transform.position = new Vector3(TILE_OFFSET_X + 1.65F, i - TILE_OFFSET_Y - 0.7f, 0) ;
            colText.transform.localScale = new Vector3(1,1,1);

            //Row Coordinates
            GameObject rowTextGO = new GameObject("row coord " + i);
            rowTextGO.transform.SetParent(gameCanvas.transform);
            Text rowText = rowTextGO.AddComponent<Text>();
            rowText.text = ((char)(i + 'a')).ToString();
            rowText.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            rowText.fontSize = 8;
            if (i % 2 == 0)
            {
                rowText.color = lightMat.color;
            }
            else
            {
                rowText.color = darkMat.color;
            }
            rowText.transform.position = new Vector3(i + TILE_OFFSET_X + 2.45f, TILE_OFFSET_Y - .36f, 0);
            rowText.transform.localScale = new Vector3(1, 1, 1);

        }
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


    public string MovePiece(Vector2Int initial_tile, Vector2Int final_tile)
    {
        string returnString = string.Format("{0}{1}{2}{3}", initial_tile.x, initial_tile.y, final_tile.x, final_tile.y);

        if (!boardManager.MovePiece(initial_tile, final_tile)) return "Illegal move! - " + returnString;

        //DestroyPiece(final_tile);
        //TransferPiece(initial_tile, final_tile);

        return ConvertToUCI(returnString);
    }

    public void UpdateBoardState(Vector2Int initial_tile, Vector2Int final_tile)
    {
        char temp = chessManager.board_state[initial_tile.x, initial_tile.y];
        chessManager.board_state[initial_tile.x, initial_tile.y] = '-';
        chessManager.board_state[final_tile.x, final_tile.y] = temp;
    }

    public void DestroyPieceObject(Vector2Int tile)
    {
        if (chessPieces[tile.x, tile.y])
            chessPieces[tile.x, tile.y].DestroyChessPiece();
    }

    public void TransferPiece(Vector2Int initial_tile, Vector2Int final_tile)
    {
        chessPieces[initial_tile.x, initial_tile.y].transform.position = new Vector3(final_tile.x - TILE_OFFSET_X, 7 - final_tile.y - TILE_OFFSET_Y, 0);
        chessPieces[final_tile.x, final_tile.y] = chessPieces[initial_tile.x, initial_tile.y];
        chessPieces[initial_tile.x, initial_tile.y] = null;

        UpdateBoardState(initial_tile, final_tile);
    }

    private GameObject HighlightTile(int row, int col, bool color)
    {

        GameObject selectedTile = tiles[row, col];

        if (color == true)
        {
            selectedTile.GetComponent<MeshRenderer>().material = hoverMat;
        }
        else
        {
            if (selectedTile.tag == "lightMat")
            {
                selectedTile.GetComponent<MeshRenderer>().material = lightMat;
            }
            else
            {
                selectedTile.GetComponent<MeshRenderer>().material = darkMat;
            }
        }
        return tiles[row, col];
    }

    private void HighlightSquares()
    {
        //grab the board state 
        int[,] physical_board_state = mainDriver.boardToArray();

        //highlight squares that have pieces on them (will be removed when hardware is more sturdy)
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (physical_board_state[i, j] == 1)
                {
                    HighlightTile(i, j, true);
                }
                else
                {
                    HighlightTile(i, j, false);
                }
            }
        }
    }

    private void HighlightLegalTiles(Vector2Int square, bool highlight)
    {
        ChessPiece chessPiece = boardManager.GetPieceAt(square);

        chessPiece.FindLegalPositions();
        
        if (highlight)
        {
            for (int i = 0; i < chessPiece.LegalPositions.Count; i++)
            {
                GameObject tileObject = Instantiate(legalTilePrefab, new Vector3(chessPiece.LegalPositions[i].x + 0.5f, TILE_COUNT_Y - chessPiece.LegalPositions[i].y - 0.5f, 0), Quaternion.identity);
                tileObject.tag = "legalTile";
            }
        }
        else
        {
            GameObject[] legalTiles = GameObject.FindGameObjectsWithTag("legalTile");
            foreach(GameObject tile in legalTiles)
            {
                GameObject.Destroy(tile);
            }
        }
        

    }

}