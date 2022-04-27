using System;
using System.Collections.Generic;
using ChessGame;
using ChessGame.PlayerInputInterface;
using UI.BoardUI;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using Object = UnityEngine.Object;

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

    //Piece Movement
    private readonly Vector2Int m_DeselectValue = Constants.ErrorValue;
    private Vector2Int m_SelectedPiece = Constants.ErrorValue;

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

    [Header("Board Polling Settings")]
    [SerializeField] private float pollingDelay = 500f;
    private float _pollingDelay;

    [Header("Debug")] 
    [SerializeField] private bool verboseDebug;

    private void Start()
    {
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
            Debug.Log("White input is " + (whiteInput.name ?? "null") );
            Debug.Log("Black input is " + (whiteInput.name ?? "null") );
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

        m_BoardManager.pieceRemoved.AddListener(DestroyPieceObject);
        m_BoardManager.pieceMoved.AddListener(TransferPiece);
    }

    private bool Polling()
    {
        var finished = false;
        
        if (_pollingDelay <= 0f)
        { 
            _pollingDelay += pollingDelay / 1000f;
            print(_pollingDelay);
            finished = true;
        }

        _pollingDelay -= Time.deltaTime;
        
        return finished;
    }

    //Every frame
    private void Update()
    {
        ProcessSelectionRaycast();
        //CheckForInput();
    }

    public void CheckForInput()
    {
        if (IODriver.Instance == null || !Polling()) return;
        
        print("Checking for Input...");

        int[,] arr;
        
        try
        {
            arr = IODriver.Instance.BoardToArray();
        }
        catch
        {
            Debug.LogWarning("Not receiving input!");
            return;
        }
        
        UnhighlightAllTiles();
        HighlightTilesFromArray(arr);
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
            if ((int) m_BoardManager.GetPieceAt(hitPosition).pieceColor != (int) m_GameManager.playerTurn)
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
        else if (hitPosition == m_SelectedPiece)
        {
            UnhighlightLegalTiles(m_SelectedPiece);
            HighlightTile(hitPosition.x, hitPosition.y, false);
            m_SelectedPiece = m_DeselectValue;
        }
        else if (hitPosition != m_SelectedPiece && m_SelectedPiece != m_DeselectValue)
        {
            if (!MovePiece(m_SelectedPiece, hitPosition)) return;
            UnhighlightLegalTiles(m_SelectedPiece);
            HighlightTile(m_SelectedPiece.x, m_SelectedPiece.y, false);
            m_SelectedPiece = m_DeselectValue;
        }
    }

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
                m_ChessPieces[x,y] = CreateSprite((ChessPieceType)(int)Enum.Parse(typeof(ChessPieceType), char.ToString(char.ToLower(temp))), temp);
                m_ChessPieces[x,y].transform.position = new Vector3(x - TileOffsetX , 7 - y - TileOffsetY, 0);
                m_ChessPieces[x,y].row = y;
                m_ChessPieces[x,y].col = x;
            }
        }
    }

    // Creates a single piece sprite
    private ChessPiece2D CreateSprite(ChessPieceType type, char team)
    {
        var cp = Instantiate(prefabs[(int)type-1], transform).GetComponent<ChessPiece2D>();
        
        cp.type = type;

        cp.team = char.IsUpper(team) ? 0 : 1;

        cp.GetComponent<SpriteRenderer>().color = teamColors[cp.team];

        cp.transform.localScale = new Vector3(PieceSize, PieceSize, 1);
        
        return cp;
    }

    private Vector2Int GetTileIndex(Object mouseInfo)
    {
        for (var y = 0; y < tileCountY; y++)
            for (var x = 0; x < tileCountX; x++)
                if (m_Tiles[x, y] == mouseInfo)
                        return new Vector2Int(x, y);
        
        return -Vector2Int.one;
    }

    // private bool MovePiece(Vector2Int initialTile, Vector2Int finalTile,
    //     PieceColor color = PieceColor.Unassigned, string moveEventArgs = "")
    // {
    //     m_ColorToInputHandler.TryGetValue(m_GameManager.playerTurn, out var inputHandler);
    //
    //     if (color == PieceColor.Unassigned) color = m_BoardManager.GetPieceAt(initialTile).pieceColor;
    //     
    //     return MovePiece(initialTile, finalTile, new MoveEventData(inputHandler, color, moveEventArgs));
    // }

    private bool MovePiece(Vector2Int initialTile, Vector2Int finalTile)
    {
        m_ColorToInputHandler.TryGetValue(m_GameManager.playerTurn, out var inputHandler);
        
        if (inputHandler == null)
        {
            if (verboseDebug) 
                Debug.LogError("Board2D Error: MovePiece() called with null sender!");
            
            return false;
        }
        
        if (verboseDebug)
            Debug.Log("Board2D: From: " + initialTile + ", To: " + finalTile);

        if (inputHandler != null && inputHandler.SendMove(initialTile, finalTile)) return true;
        
        if (verboseDebug)
            Debug.Log("Board2D: Illegal move attempted from: " + initialTile + ", to: " + finalTile + '.');
        
        return false;
    }
    
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
    }

    public void HighlightTilesFromArray(int[,] arr)
    {
        for (var y = 0; y < tileCountY; y++)
            for (var x = 0; x < tileCountX; x++)
                if (arr[x, y] == 1)
                    HighlightTile(x, y);
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

    // Highlight pieces specified collection of Vector2Ints
    public void SetHighlightTiles(IEnumerable<Vector2Int> squares, bool bHighlight)
    {
        foreach (var coords in squares)
            if (bHighlight)
                HighlightTile(coords.x, coords.y);
            else
                UnhighlightTile(coords.x, coords.y);
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

        var squarePos = m_Tiles[square.x, square.y].transform.position;
        var diagonalDistance = Mathf.Sqrt(2f);
            
        foreach (var pos in legalMoves)
        {
            if (!baseChessPiece.CanMoveToPosition(pos)) continue;
            var tileObject = Instantiate(legalTilePrefab, new Vector3(pos.x + 0.5f, tileCountY - pos.y - 0.5f, 0), Quaternion.identity);
            tileObject.tag = "legalTile";

            var dotAnim = tileObject.GetComponent<DotAnimationController>();
            var dotPos = dotAnim.transform.position;
            var distanceDiagonal = Mathf.Abs((squarePos - dotPos).magnitude / diagonalDistance);
            var distanceLinear = Mathf.Abs((squarePos - dotPos).magnitude);

            print("diagonal: " + (squarePos - dotPos).magnitude / diagonalDistance);
            print("linear: " + (squarePos - dotPos).magnitude);
            print("diagonal rounded: " + distanceDiagonal);
            print("linear rounded: " + distanceLinear);

            dotAnim.DoDelayedGrowth(distanceDiagonal > 0.1f
                ? Mathf.RoundToInt(distanceLinear)
                : Mathf.RoundToInt(distanceDiagonal));
        }
    }

    private void UnhighlightLegalTiles(Vector2Int square)
    {
        var squarePos = m_Tiles[square.x, square.y].transform.position;
        var diagonalDistance = Mathf.Sqrt(2f);

        foreach (var tile in GameObject.FindGameObjectsWithTag("legalTile"))
        {
            var dotAnim = tile.GetComponent<DotAnimationController>();
            var dotPos = dotAnim.transform.position;
            var distanceDiagonal = Mathf.Abs((squarePos - dotPos).magnitude / diagonalDistance);
            var distanceLinear = (squarePos - dotPos).magnitude;

            print("diagonal: " + distanceDiagonal);
            print("linear: " + distanceLinear);


            dotAnim.DoDelayedShrink(distanceDiagonal > 0.1f
                ? Mathf.RoundToInt(distanceLinear)
                : Mathf.RoundToInt(distanceDiagonal));
        }
    }
    
    
    private void HighlightLegalTiles(Vector2Int square, bool highlight)
    {
        if (highlight)
            HighlightLegalTiles(square);
        else
            UnhighlightLegalTiles(square);
    }
}