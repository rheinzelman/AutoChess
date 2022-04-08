using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingSquareSpawner : MonoBehaviour
{
    [Header("Settings")]
    public GameObject scrollingSquarePrefab;
    public float scrollSpeed = 1f;
    public float scrollDistance = 5.5f;
    public bool moveRight = true;
    public bool startWhite = true;
    public bool startWithPiece = true;
    public bool alternateEmptySquares = true;
    
    private float spawnRate = 1f;
    private float spawnTimer = 0f;
    private float pieceScale = 1f;
    private bool spawnWhite;

    [Header("Debug")]
    [SerializeField] private bool useSpecificSpriteIndex = false;
    [SerializeField] private int indexToUse = 0;

    // Start is called before the first frame update
    void Start()
    {
        pieceScale = scrollingSquarePrefab.transform.localScale.x;
        spawnRate = pieceScale / scrollSpeed;
        spawnWhite = startWhite;

        InitializeSquares();
    }

    private void InitializeSquares()
    {
        int spawnCount = Mathf.CeilToInt(scrollDistance / pieceScale);
        spawnTimer = spawnRate * spawnCount;

        for (int i = 0; i < spawnCount; i++)
            CheckSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer += Time.deltaTime;

        CheckSpawn();
    }

    private void CheckSpawn()
    {
        if (spawnTimer < spawnRate) return;

        spawnTimer -= spawnRate;

        GameObject newSquareGO = Instantiate(scrollingSquarePrefab);
        ScrollingSquare newSquare = newSquareGO.GetComponent<ScrollingSquare>();

        if (alternateEmptySquares)
        {
            newSquare.startWithPiece = startWithPiece;
            startWithPiece = !startWithPiece;
        }

        newSquareGO.transform.position = transform.position;

        newSquare.startDistance = scrollSpeed * spawnTimer;
        newSquare.startWhite = spawnWhite;
        newSquare.distanceToTravel = scrollDistance;
        newSquare.travelSpeed = scrollSpeed;
        newSquare.travelRight = moveRight;

        if (useSpecificSpriteIndex)
        {
            newSquare.useSpecificPieceIndex = true;
            newSquare.indexToUse = indexToUse;
        }

        spawnWhite = !spawnWhite;
    }
}
