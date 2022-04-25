using UnityEngine;

public class ScrollingSquareSpawner : MonoBehaviour
{
    [Header("Settings")] public GameObject scrollingSquarePrefab;
    public float scrollSpeed = 1f;
    public float scrollDistance = 5.5f;
    public bool moveRight = true;
    public bool startWhite = true;
    public bool startWithPiece = true;
    public bool alternateEmptySquares = true;

    private float m_SpawnRate = 1f;
    private float m_SpawnTimer;
    private float m_PieceScale = 1f;
    private bool m_SpawnWhite;

    [Header("Debug")] [SerializeField] private bool useSpecificSpriteIndex;
    [SerializeField] private int indexToUse;

    // Start is called before the first frame update
    private void Start()
    {
        m_PieceScale = scrollingSquarePrefab.transform.localScale.x;
        m_SpawnRate = m_PieceScale / scrollSpeed;
        m_SpawnWhite = startWhite;

        InitializeSquares();
    }

    private void InitializeSquares()
    {
        var spawnCount = Mathf.CeilToInt(scrollDistance / m_PieceScale);
        m_SpawnTimer = m_SpawnRate * spawnCount;

        for (var i = 0; i < spawnCount; i++)
            CheckSpawn();
    }

    // Update is called once per frame
    private void Update()
    {
        m_SpawnTimer += Time.deltaTime;

        CheckSpawn();
    }

    private void CheckSpawn()
    {
        if (m_SpawnTimer < m_SpawnRate) return;

        m_SpawnTimer -= m_SpawnRate;

        var newSquareGO = Instantiate(scrollingSquarePrefab);
        var newSquare = newSquareGO.GetComponent<ScrollingSquare>();

        if (alternateEmptySquares)
        {
            newSquare.startWithPiece = startWithPiece;
            startWithPiece = !startWithPiece;
        }

        newSquareGO.transform.position = transform.position;

        newSquare.startDistance = scrollSpeed * m_SpawnTimer;
        newSquare.startWhite = m_SpawnWhite;
        newSquare.distanceToTravel = scrollDistance;
        newSquare.travelSpeed = scrollSpeed;
        newSquare.travelRight = moveRight;

        if (useSpecificSpriteIndex)
        {
            newSquare.useSpecificPieceIndex = true;
            newSquare.indexToUse = indexToUse;
        }

        m_SpawnWhite = !m_SpawnWhite;
    }
}