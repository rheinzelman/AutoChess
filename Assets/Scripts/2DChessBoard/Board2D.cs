using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using IODriverNamespace;
using StockfishHandlerNamespace;
using AutoChess;
using AutoChess.Utility.FENHandler;
using AutoChess.PlayerInput;
using ChessGame;
using ChessGame.ChessPieces;
using ChessGame.PlayerInputInterface;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

public class Board2D : MonoBehaviour
{

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
    // bool boardConnected = true;
    // IODriver mainDriver;
    // private int[,] initial_bs;
    // private int[,] final_bs;

    //Board
    [HideInInspector] public int tileCountX = 8;
    [HideInInspector] public int tileCountY = 8;
    private const float TileOffsetX = -0.5f;
    private const float TileOffsetY = -0.525f;
    private const float CoordOffsetX = 5F;
    private const float CoordOffsetY = 5F;
    private const float PieceSize = 1.75f;
    private GameObject[,] m_Tiles;
    private ChessPiece2D[,] m_ChessPieces;

    //stockfish test
    //public StockfishHandler stockfishTest;

    //FEN test
    //public FENHandler fenTest;

    //Piece Movement
    //private ChessPiece2D selectedPiece = null;
    private readonly Vector2Int m_DeselectValue = Constants.ErrorValue;
    private Vector2Int m_SelectedPiece = Constants.ErrorValue;
    // private Vector2Int capturedPiece = Constants.ErrorValue;

    //Unity
    private Camera m_CurrentCamera;

    [Header("Board Settings")]

    //Managers
    private BoardManager m_BoardManager;
    private GameManager m_GameManager;

    // Input Handlers
    public BaseInputHandler whiteInput;
    public BaseInputHandler blackInput;

    // Determine if input is allowed
    private Dictionary<PlayerColor, bool> m_EnabledInputs;

    // Maps out the player color to the input handler responsible for handling its move
    private Dictionary<PlayerColor, BaseInputHandler> m_ColorToInputHandler;

    // Active color of the player taking the turn
    //private PlayerColor m_ActiveColor = PlayerColor.Unassigned;

    [Header("Debug")]
    [SerializeField] private bool verboseDebug;

    private void Start()
    {
        //IO Diver initialization, initial board state is recorded when game is initialized 
        // mainDriver = gameObject.AddComponent<IODriver>();
        // initial_bs = mainDriver.boardToArray();
        //
        // boardManager ??= GetComponent<BoardManager>();
        m_CurrentCamera ??= Camera.main;

        m_BoardManager = BoardManager.Instance;
        m_GameManager = GameManager.Instance;

        m_EnabledInputs = new Dictionary<PlayerColor, bool>()
        {
            {PlayerColor.White, whiteInput != null},
            {PlayerColor.Black, blackInput != null}
        };

        m_ColorToInputHandler = new Dictionary<PlayerColor, BaseInputHandler>()
        {
            {PlayerColor.White, whiteInput},
            {PlayerColor.Black, blackInput}
        };

        if (verboseDebug)
        {
            Debug.Log("White input evaluates to " + (whiteInput != null));
            Debug.Log("Black input evaluates to " + (blackInput != null));
            Debug.Log("White input is " + (whiteInput.name ?? "null"));
            Debug.Log("Black input is " + (whiteInput.name ?? "null"));
            Debug.Log("White input color is " + whiteInput.playerColor);
            Debug.Log("White input color is " + blackInput.playerColor);
            Debug.Log("White input is " + (m_EnabledInputs[PlayerColor.White] ? " enabled" : " disabled"));
            Debug.Log("Black input is " + (m_EnabledInputs[PlayerColor.Black] ? " enabled" : " disabled"));
        }

        verboseDebug = m_GameManager.verboseDebug;

        m_GameManager.onVerboseDebugChanged.AddListener(SetVerboseDebug);

        m_ChessPieces = new ChessPiece2D[tileCountY, tileCountX];
        m_Tiles = new GameObject[tileCountY, tileCountX];
        m_ChessPieces = new ChessPiece2D[tileCountX, tileCountY];

        SetupTiles();
        CreatePieceSprites();
        //DrawCoords();

        m_BoardManager.pieceRemoved.AddListener(DestroyPieceObject);
        m_BoardManager.pieceMoved.AddListener(TransferPiece);

        // stockfishTest = gameObject.AddComponent<StockfishHandler>();
        // fenTest = new FENHandler(FENHandler.DEFAULT_FEN);//gameObject.AddComponent<FENHandler>();

        //PlayGameFromFile(recordedGames[0]);

    }

    //Every frame
    private void Update()
    {
        //ProcessBoardInput();
        //ProcessStockfishInput();
        ProcessSelectionRaycast();
    }

    private void SetVerboseDebug(bool bEnabled)
    {
        verboseDebug = bEnabled;
    }

    private void ProcessSelectionRaycast()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        m_EnabledInputs.TryGetValue(GameManager.Instance.playerTurn, out var selectionAllowed);

        if (!selectionAllowed)
        {
            if (verboseDebug)
                Debug.Log("Board2D: Selection attempted on piece with disabled input.");

            return;
        }

        var ray = m_CurrentCamera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out var info, 100, LayerMask.GetMask("Tile"));

        // Get the coordinates of the tile i've hit
        var hitPosition = GetTileIndex(info.transform.gameObject);

        if (m_BoardManager.HasPieceAt(hitPosition) && m_SelectedPiece == m_DeselectValue)
        {
            if ((int)m_BoardManager.GetPieceAt(hitPosition).pieceColor != (int)m_GameManager.playerTurn)
            {
                if (verboseDebug)
                    Debug.Log("Board2D: Player has attempted to select an opponent's piece.");

                return;
            }

            m_SelectedPiece = hitPosition;
            HighlightLegalTiles(m_SelectedPiece, true);
            HighlightTile(hitPosition.x, hitPosition.y, true);
        }
        else if (!m_BoardManager.HasPieceAt(hitPosition) && m_SelectedPiece == m_DeselectValue)
        {
            if (verboseDebug)
                Debug.Log("Board2D: Player has attempted to select an empty square.");
        }
        // //if we click a tile that has a piece
        // if (chessPieces[hitPosition.x, hitPosition.y] && selectedPiece == deselectValue)
        // {
        //     selectedPiece = new Vector2Int(hitPosition.x, hitPosition.y);
        //     HighlightLegalTiles(selectedPiece, true);
        //     HighlightTile(hitPosition.x, hitPosition.y, true);
        // }

        // else if we select the same piece again, deselect
        else if (hitPosition == m_SelectedPiece)
        {
            UnhighlightLegalTiles();
            HighlightTile(hitPosition.x, hitPosition.y, false);
            m_SelectedPiece = m_DeselectValue;
        }

        else if (hitPosition != m_SelectedPiece && m_SelectedPiece != m_DeselectValue)
        {
            if (!MovePiece(m_SelectedPiece, hitPosition)) return;
            UnhighlightLegalTiles();
            HighlightTile(m_SelectedPiece.x, m_SelectedPiece.y, false);
            m_SelectedPiece = m_DeselectValue;
        }

        //If we have selected a piece and we are then selecting an empty tile 
        // else if (selectedPiece != deselectValue && chessPieces[hitPosition.x, hitPosition.y] == null)
        // {
        //     HighlightLegalTiles(selectedPiece, false);
        //     HighlightTile(selectedPiece.x, selectedPiece.y, false);
        //     string UCIMove = MovePiece(selectedPiece, hitPosition);
        //     Debug.Log(UCIMove);
        //     //mainDriver.performStandardMove(UCIMove.Substring(0,2), UCIMove.Substring(2,2));
        //     selectedPiece = deselectValue;
        // }
        //else if we select a piece with the opposite team, destroy opponent piece

        // else if (selectedPiece != deselectValue && chessPieces[hitPosition.x, hitPosition.y] != null && chessPieces[hitPosition.x, hitPosition.y].team != chessPieces[selectedPiece.x, selectedPiece.y].team)
        // {
        //     HighlightLegalTiles(selectedPiece, false);
        //     HighlightTile(selectedPiece.x, selectedPiece.y, false);
        //     MovePiece(selectedPiece, hitPosition);
        //     selectedPiece = deselectValue;
        // }

        // If no piece is selected, exit the function
        // if (selectedPiece == deselectValue) return;

        // RaycastHit info;
        // Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        // if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile")) && Input.GetMouseButtonDown(0))
        // {
        //
        //     // Get the indexes of the tile i've hit
        //     Vector2Int hitPosition = GetTileIndex(info.transform.gameObject);
        //
        //     //if we click a tile that has a piece
        //     if (chessPieces[hitPosition.x, hitPosition.y] && selectedPiece == deselectValue)
        //     {
        //         selectedPiece = new Vector2Int(hitPosition.x, hitPosition.y);
        //         HighlightLegalTiles(selectedPiece, true);
        //         HighlightTile(hitPosition.x, hitPosition.y, true);
        //     }
        //     //else if we select the same piece again, deselect
        //     else if (hitPosition == selectedPiece)
        //     {
        //         HighlightLegalTiles(selectedPiece, false);
        //         HighlightTile(hitPosition.x, hitPosition.y, false);
        //         selectedPiece = deselectValue;
        //     }
        //     //If we have selected a piece and we are then selecting an empty tile 
        //     else if (selectedPiece != deselectValue && chessPieces[hitPosition.x, hitPosition.y] == null)
        //     {
        //         HighlightLegalTiles(selectedPiece, false);
        //         HighlightTile(selectedPiece.x, selectedPiece.y, false);
        //         string UCIMove = MovePiece(selectedPiece, hitPosition);
        //         Debug.Log(UCIMove);
        //         //mainDriver.performStandardMove(UCIMove.Substring(0,2), UCIMove.Substring(2,2));
        //         selectedPiece = deselectValue;
        //     }
        //     //else if we select a piece with the opposite team, destroy opponent piece
        //
        //     else if (selectedPiece != deselectValue && chessPieces[hitPosition.x, hitPosition.y] != null && chessPieces[hitPosition.x, hitPosition.y].team != chessPieces[selectedPiece.x, selectedPiece.y].team)
        //     {
        //         HighlightLegalTiles(selectedPiece, false);
        //         HighlightTile(selectedPiece.x, selectedPiece.y, false);
        //         MovePiece(selectedPiece, hitPosition);
        //         selectedPiece = deselectValue;
        //     }
        //     // If no piece is selected, exit the function
        //     if (selectedPiece == deselectValue) return;
        // }
    }

    // private void ProcessStockfishInput()
    // {
    //     if (Input.GetKeyDown(KeyCode.A))
    //     {
    //         Debug.Log(stockfishTest.GetMove(boardManager.FenObject.getCurrentFEN(boardManager.BoardState)));
    //     }
    // }

    //     private void ProcessBoardInput()
    //     {
    //         if (boardConnected)
    //         {
    //             HighlightSquares();
    //             //when spacebar is pressed, attempt to grab physical board state changes and represent virtually
    //             if (Input.GetKeyDown(KeyCode.Space))
    //             {
    //
    //                 //grab the final board state
    //                 final_bs = mainDriver.boardToArray();
    //
    //                 for(int i = 0; i < 8; i++)
    //                 {
    //                     for( int j = 0; j < 8; j++)
    //                     {
    //                         print(i + ", " + j + ": " + final_bs[i,j]);
    //                     }
    //                 }
    //
    //                 for (int i = 0; i < 8; i++)
    //                 {
    //                     for (int j = 0; j < 8; j++)
    //                     {
    //                         print(i + ", " + j + ": " + initial_bs[i, j]);
    //                     }
    //                 }
    //
    //                 //compare initial and final board state
    //                 int[,] differenceArray = mainDriver.getDifferenceArray(initial_bs, final_bs);
    //
    //                 //if there was a piece moved to an empty space and a piece was previously moved to the capture square
    //                 //if (mainDriver.checkDifference(differenceArray) == 1 && capturedPiece != Vector2Int.zero)
    //                 if (capturedPiece != (Vector2Int.one * -1))
    //                 {
    //                     for (int i = 0; i < 8; i++)
    //                     {
    //                         for (int j = 0; j < 8; j++)
    //                         {
    //                             if (differenceArray[i, j] == -1)
    //                             {
    //                                 print("piece captured");
    //                                 //MovePiece(new Vector2Int(i, j), capturedPiece, false);
    //                                 capturedPiece = Vector2Int.one * -1;
    //                                 initial_bs = final_bs;
    //                             }
    //                         }
    //                     }
    //                 }
    //                 //if there was a piece moved to an empty space
    //                 else if (mainDriver.checkDifference(differenceArray) == 1)
    //                 {
    //                     print("piece moved to empty square");
    // // <<<<<<< HEAD
    // //                     List<Vector2Int> physical_move = mainDriver.getMoveFromDifferenceArray(differenceArray);
    // //                     MovePiece(physical_move[0], physical_move[1]);
    // //                 }
    // //                 //if a piece was moved to the capture square and the difference array notes that only one piece was moved
    // //                 else if (mainDriver.checkDifference(differenceArray) == 2 && mainDriver.capturedPiece() == true)
    // //                 {
    //
    // //                     for (int i = 0; i < 8; i++)
    // // =======
    //                     //List<Vector2Int> physical_move = mainDriver.getMoveFromDifferenceArray(difference_array);
    //                     //MovePiece(physical_move[0], physical_move[1], false);
    //                     initial_bs = final_bs;
    //                 }
    //                 //if a piece was moved to the capture square and the difference array notes that only one piece was moved
    //                 else if(mainDriver.capturedPiece() == true)
    //                 {
    //                     for(int i = 0; i < 8; i++)
    //                     {
    //                         for (int j = 0; j < 8; j++)
    //                         {
    //                             if (differenceArray[i, j] != 0)
    //                             {
    //                                 print("peice moved to capture square");
    //                                 capturedPiece = new Vector2Int(i, j);
    //                             }
    //                         }
    //                     }
    //
    //                 }
    //                 else if (mainDriver.checkDifference(differenceArray) == 0)
    //                 {
    //                     print("checkDifference error, move pieces back");
    //                 }
    //                 else
    //                 {
    //                     print("unknown board read error");
    //                 }
    //
    //             }
    //
    //             if (Input.GetKeyDown(KeyCode.H))
    //             {
    //                 mainDriver.homeCoreXY();
    //             }
    //
    //            
    //
    //         }
    //
    //         if (Input.GetKeyDown(KeyCode.A))
    //         {
    //             //Debug.Log(stockfishTest.GetMove(fenTest.getCurrentFEN(chessManager.board_state)));
    //         }
    //
    //         if (!currentCamera)
    //         {
    //             currentCamera = Camera.main;
    //             return;
    //         }
    //
    //         // RaycastHit info;
    //         // Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
    //         // if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile")) && Input.GetMouseButtonDown(0))
    //         // {
    //         //
    //         //     // Get the indexes of the tile i've hit
    //         //     Vector2Int hitPosition = GetTileIndex(info.transform.gameObject);
    //         //
    //         //     //if we click a tile that has a piece
    //         //     if (chessPieces[hitPosition.x, hitPosition.y] && selectedPiece == deselectValue)
    //         //     {
    //         //         selectedPiece = new Vector2Int(hitPosition.x, hitPosition.y);
    //         //         HighlightLegalTiles(selectedPiece, true);
    //         //         HighlightTile(hitPosition.x, hitPosition.y, true);
    //         //     }
    //         //     //else if we select the same piece again, deselect
    //         //     else if (hitPosition == selectedPiece)
    //         //     {
    //         //         HighlightLegalTiles(selectedPiece, false);
    //         //         HighlightTile(hitPosition.x, hitPosition.y, false);
    //         //         selectedPiece = deselectValue;
    //         //     }
    //         //     //If we have selected a piece and we are then selecting an empty tile 
    //         //     else if (selectedPiece != deselectValue && chessPieces[hitPosition.x, hitPosition.y] == null)
    //         //     {
    //         //         HighlightLegalTiles(selectedPiece, false);
    //         //         HighlightTile(selectedPiece.x, selectedPiece.y, false);
    //         //         //string UCIMove = MovePiece(selectedPiece, hitPosition, true);
    //         //         //Debug.Log(UCIMove);
    //         //         selectedPiece = deselectValue;
    //         //     }
    //         //     //else if we select a piece with the opposite team, destroy opponent piece
    //         //
    //         //     else if (selectedPiece != deselectValue && chessPieces[hitPosition.x, hitPosition.y] != null && chessPieces[hitPosition.x, hitPosition.y].team != chessPieces[selectedPiece.x, selectedPiece.y].team)
    //         //     {
    //         //         HighlightLegalTiles(selectedPiece, false);
    //         //         HighlightTile(selectedPiece.x, selectedPiece.y, false);
    //         //         //MovePiece(selectedPiece, hitPosition, true);
    //         //         selectedPiece = deselectValue;
    //         //     }
    //         //     // If no piece is selected, exit the function
    //         //     if (selectedPiece == deselectValue) return ;
    //         // }
    //     }

    // Set up the tiles 
    private void SetupTiles()
    {
        m_Tiles = new GameObject[tileCountX, tileCountY];

        var colored = false;

        for (var y = 0; y < tileCountY; y++)
        {
            for (var x = 0; x < tileCountX; x++)
            {
                m_Tiles[x, y] = CreateTile(x, y, colored);
                colored = !colored;
            }
            colored = !colored;
        }
    }

    // Draw single tile
    private GameObject CreateTile(int x, int y, bool colored)
    {
        var newPos = new Vector3(x + 0.5f, tileCountY - y - 0.5f, 0);

        var tileObject = Instantiate(colored ? darkTilePrefab : lightTilePrefab, newPos, Quaternion.identity);

        tileObject.transform.parent = transform;

        tileObject.name = "X: " + x + ", Y: " + y;

        return tileObject;
    }

    // Create all 2D piece sprites
    private void CreatePieceSprites()
    {
        for (var y = 0; y < tileCountY; y++)
        {
            for (var x = 0; x < tileCountX; x++)
            {
                if (m_BoardManager.BoardState[x, y] == '-') continue;
                var temp = m_BoardManager.BoardState[x, y];
                m_ChessPieces[x, y] = CreateSprite((ChessPieceType)(int)Enum.Parse(typeof(ChessPieceType), char.ToString(char.ToLower(temp))), temp);
                m_ChessPieces[x, y].transform.position = new Vector3(x - TileOffsetX, 7 - y - TileOffsetY, 0);
                m_ChessPieces[x, y].row = y;
                m_ChessPieces[x, y].col = x;
            }
        }
    }

    // Creates a single piece sprite
    private ChessPiece2D CreateSprite(ChessPieceType type, char team)
    {
        var cp = Instantiate(prefabs[(int)type - 1], transform).GetComponent<ChessPiece2D>();

        cp.type = type;

        cp.team = char.IsUpper(team) ? 0 : 1;

        cp.GetComponent<SpriteRenderer>().color = teamColors[cp.team];

        cp.transform.localScale = new Vector3(PieceSize, PieceSize, 1);

        return cp;
    }

    // private void DrawCoords()
    // {
    //     GameObject gameCanvas = GameObject.Find("UICanvas");
    //     for (int i = 0; i < 8; i++)
    //     {
    //         //Column Coordinates
    //         GameObject colTextGO = new GameObject("col coord " + i);
    //         colTextGO.transform.SetParent(gameCanvas.transform);
    //         Text colText = colTextGO.AddComponent<Text>();
    //         colText.text = (i+1).ToString();
    //         colText.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
    //         colText.fontSize = 8;
    //         if(i % 2 == 0)
    //         {
    //             colText.color = lightMat.color;
    //         }
    //         else
    //         {
    //             colText.color = darkMat.color;
    //         }
    //         colText.transform.position = new Vector3(TILE_OFFSET_X + 1.65F, i - TILE_OFFSET_Y - 0.7f, 0) ;
    //         colText.transform.localScale = new Vector3(1,1,1);
    //
    //         //Row Coordinates
    //         GameObject rowTextGO = new GameObject("row coord " + i);
    //         rowTextGO.transform.SetParent(gameCanvas.transform);
    //         Text rowText = rowTextGO.AddComponent<Text>();
    //         rowText.text = ((char)(i + 'a')).ToString();
    //         rowText.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
    //         rowText.fontSize = 8;
    //         if (i % 2 == 0)
    //         {
    //             rowText.color = lightMat.color;
    //         }
    //         else
    //         {
    //             rowText.color = darkMat.color;
    //         }
    //         rowText.transform.position = new Vector3(i + TILE_OFFSET_X + 2.45f, TILE_OFFSET_Y - .36f, 0);
    //         rowText.transform.localScale = new Vector3(1, 1, 1);
    //
    //     }
    // }

    private Vector2Int GetTileIndex(Object mouseInfo)
    {
        for (var y = 0; y < tileCountY; y++)
            for (var x = 0; x < tileCountX; x++)
                if (m_Tiles[x, y] == mouseInfo)
                    return new Vector2Int(x, y);

        return -Vector2Int.one;
    }

    // private string ConvertToUCI(string unconverted_string)
    // {
    //
    //     string returnValue = "";
    //     string letters = "abcdefgh";
    //
    //     for(int i = 0; i < unconverted_string.Length; i++)
    //     {
    //         if(i % 2 != 0)
    //         {
    //             returnValue += '8'-unconverted_string[i];
    //         }
    //         else
    //         {
    //             returnValue += letters[unconverted_string[i] - '0'];
    //         }
    //     }
    //
    //     return returnValue;
    // }

    // public string MovePiece(Vector2Int initial_tile, Vector2Int final_tile)
    // {
    //     string returnString = string.Format("{0}{1}{2}{3}", initial_tile.x, initial_tile.y, final_tile.x, final_tile.y);
    //
    //     print("From: " + initial_tile + ", To: " + final_tile);
    //
    //     if (!boardManager.MovePiece(initial_tile, final_tile)) return "Illegal move! - " + returnString;
    //
    //     return ConvertToUCI(returnString);
    // 

    private bool MovePiece(Vector2Int initialTile, Vector2Int finalTile,
        PieceColor color = PieceColor.Unassigned, string moveEventArgs = "")
    {
        m_ColorToInputHandler.TryGetValue(m_GameManager.playerTurn, out var inputHandler);

        if (color == PieceColor.Unassigned) color = m_BoardManager.GetPieceAt(initialTile).pieceColor;

        return MovePiece(initialTile, finalTile, new MoveEventData(inputHandler, color, moveEventArgs));
    }

    private bool MovePiece(Vector2Int initialTile, Vector2Int finalTile, MoveEventData moveData)
    {
        if (moveData.Sender == null)
        {
            if (verboseDebug)
                Debug.LogError("Board2D Error: MovePiece() called with null sender!");

            return false;
        }

        if (verboseDebug)
            Debug.Log("Board2D: From: " + initialTile + ", To: " + finalTile);

        if (moveData.Sender.SendMove(initialTile, finalTile, moveData)) return true;

        if (verboseDebug)
            Debug.Log("Board2D: Illegal move attempted from: " + initialTile + ", to: " + finalTile + '.');

        return false;
        //     public string MovePiece(Vector2Int initial_tile, Vector2Int final_tile, bool physcial_move)
        //     {
        //         string returnString = string.Format("{0}{1}{2}{3}", initial_tile.x, initial_tile.y, final_tile.x, final_tile.y);

        //         bool capture = false;

        //         if(boardManager.GetPieceAt(final_tile) != null)
        //         {
        //             capture = true;
        //         }

        //         if (!boardManager.MovePiece(initial_tile, final_tile)) return "Illegal move! - " + returnString;

        //         string UCIMove = ConvertToUCI(returnString);

        //         if (physcial_move && boardConnected)
        //         {

        //             if(capture == true)
        //             {
        //                 mainDriver.performCapture(UCIMove.Substring(2,2));
        //             }

        //             if (boardManager.GetPieceAt(final_tile) is Knight)
        //             {
        //                 mainDriver.performKnightMove(UCIMove.Substring(0, 2), UCIMove.Substring(2, 2));
        //             }
        //             // need to add a check to see if castling is still legal once holden's code is integrated
        //             else if (boardManager.GetPieceAt(final_tile) is King && (UCIMove == "e1g1" || UCIMove == "e1c1" || UCIMove == "e8g8" || UCIMove == "e8c8"))
        //             {
        //                 mainDriver.performCastling(UCIMove.Substring(0, 2), UCIMove.Substring(2, 2));
        //             }
        //             else
        //             {
        //                 mainDriver.performStandardMove(UCIMove.Substring(0, 2), UCIMove.Substring(2, 2));
        //             }
        //         }

        //         //DestroyPiece(final_tile);
        //         //TransferPiece(initial_tile, final_tile);

        //         return ConvertToUCI(returnString);
    }

    // private void UpdateBoardState(Vector2Int initialTile, Vector2Int finalTile)
    // {
    //     var temp = m_BoardManager.BoardState[initialTile.x, initialTile.y];
    //     m_BoardManager.BoardState[initialTile.x, initialTile.y] = '-';
    //     m_BoardManager.BoardState[finalTile.x, finalTile.y] = temp;
    // }

    // Destroys a piece sprite at the specified tile
    private void DestroyPieceObject(Vector2Int tile)
    {
        if (m_ChessPieces[tile.x, tile.y])
            m_ChessPieces[tile.x, tile.y].DestroyChessPiece();
    }

    // Transfers a piece sprite from initialTile to finalTile
    private void TransferPiece(Vector2Int initialTile, Vector2Int finalTile)
    {
        m_ChessPieces[initialTile.x, initialTile.y].transform.position = new Vector3(finalTile.x - TileOffsetX, 7 - finalTile.y - TileOffsetY, 0);
        m_ChessPieces[finalTile.x, finalTile.y] = m_ChessPieces[initialTile.x, initialTile.y];
        m_ChessPieces[initialTile.x, initialTile.y] = null;

        //UpdateBoardState(initialTile, finalTile);
    }

    // highlight a single tile
    public GameObject HighlightTile(int row, int col, bool color)
    {
        var selectedTile = m_Tiles[row, col];
        var selectedTileMeshRenderer = selectedTile.GetComponent<MeshRenderer>();

        if (color == true)
            selectedTileMeshRenderer.material = hoverMat;
        else
            selectedTileMeshRenderer.material = selectedTile.CompareTag("lightMat") ? lightMat : darkMat;

        return m_Tiles[row, col];
    }

    public GameObject UnhighlightTile(int row, int col)
    {
        var selectedTile = m_Tiles[row, col];
        var selectedTileMeshRenderer = selectedTile.GetComponent<MeshRenderer>();

        selectedTileMeshRenderer.material = selectedTile.CompareTag("lightMat") ? lightMat : darkMat;

        return m_Tiles[row, col];
    }

    public GameObject HighlightTile(int row, int col)
    {
        var selectedTile = m_Tiles[row, col];
        var selectedTileMeshRenderer = selectedTile.GetComponent<MeshRenderer>();

        selectedTileMeshRenderer.material = hoverMat;

        return m_Tiles[row, col];
    }

    // highlight squares that have a physical piece on them
    public void SetHighlightTiles(IEnumerable<Vector2Int> squares, bool bHighlight)
    {
        foreach (var coords in squares)
            if (bHighlight)
                HighlightTile(coords.x, coords.y);
            else
                UnhighlightTile(coords.x, coords.y);

        //grab the board state 
        //var physicalBoardState = mainDriver.boardToArray();

        //highlight squares that have pieces on them (will be removed when hardware is more sturdy)
        // for (var i = 0; i < 8; i++)
        //     for (var j = 0; j < 8; j++)
        //         HighlightTile(i, j, physicalBoardState[i, j] == 1);
    }

    public void HighlightTiles(IEnumerable<Vector2Int> squares)
    {
        foreach (var coords in squares)
            HighlightTile(coords.x, coords.y);
    }

    public void UnhighlightTiles(IEnumerable<Vector2Int> squares)
    {
        foreach (var coords in squares)
            UnhighlightTile(coords.x, coords.y);
    }

    private void UnhighlightAllTiles()
    {
        for (var y = 0; y < tileCountX; y++)
            for (var x = 0; x < tileCountX; x++)
                UnhighlightTile(x, y);
    }

    // add a dot highlight to squares with legal moves for the given piece
    private void HighlightLegalTiles(Vector2Int square)
    {
        var baseChessPiece = m_BoardManager.GetPieceAt(square);

        if (baseChessPiece == null) return;

        var legalMoves = new List<Vector2Int>();
        legalMoves.AddRange(baseChessPiece.legalPositions);
        legalMoves.AddRange(baseChessPiece.legalAttacks);

        foreach (var pos in legalMoves)
        {
            if (!baseChessPiece.CanMoveToPosition(pos)) continue;
            var tileObject = Instantiate(legalTilePrefab, new Vector3(pos.x + 0.5f, tileCountY - pos.y - 0.5f, 0), Quaternion.identity);
            tileObject.tag = "legalTile";
        }
    }

    private void UnhighlightLegalTiles()
    {
        foreach (var tile in GameObject.FindGameObjectsWithTag("legalTile"))
            GameObject.Destroy(tile);
    }


    private void HighlightLegalTiles(Vector2Int square, bool highlight)
    {
        if (highlight)
            HighlightLegalTiles(square);
        else
            UnhighlightLegalTiles();
    }

    // public void PlayGameFromFile(TextAsset gameFile) 
    // {
    //     string fileContents = gameFile.ToString();
    //
    //     string[] moves = fileContents.Split(' ');
    //
    //     foreach (var move in moves)
    //     {
    //         Vector2Int from = new Vector2Int((int)(move[0] - 96), (int)move[1] - 48);
    //         Vector2Int to = new Vector2Int((int)(move[2] - 96), (int)move[3] - 48);
    //         print(from + " " + to);
    //         //MovePiece(from, to);
    //     }
    //
    // }
}