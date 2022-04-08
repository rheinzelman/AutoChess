using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingSquare : MonoBehaviour
{
    public SpriteRenderer pieceSprite, squareSprite;
    public List<Sprite> possiblePieceSprites;

    public Color darkColor;
    public Color whiteColor;

    public bool startWhite;

    public float distanceToTravel = 5.5f;
    public float travelSpeed = 1f;
    public bool travelRight = true;
    public bool startWithPiece = true;

    private float distanceTraveled = 0f;
    [HideInInspector] public float startDistance = 0f;

    [Header("Debug")]
    [HideInInspector] public bool useSpecificPieceIndex = false;
    [HideInInspector] public int indexToUse = 0;

    void Start()
    {
        if (startWithPiece)
            if (useSpecificPieceIndex)
                pieceSprite.sprite = possiblePieceSprites[indexToUse];
            else
                pieceSprite.sprite = possiblePieceSprites[Random.Range(0, possiblePieceSprites.Count)];
        else
            pieceSprite.sprite = null;

        if (startWhite)
            squareSprite.color = whiteColor;
        else
            squareSprite.color = darkColor;

        if (startDistance > 0f)
            MoveSquare(startDistance);
    }

    // Update is called once per frame
    void Update()
    {
        float deltaDistance = Time.deltaTime * travelSpeed;

        MoveSquare(deltaDistance);
    }

    private void MoveSquare(float deltaDistance)
    {
        if (travelRight)
            transform.position += Vector3.right * deltaDistance;
        else
            transform.position += Vector3.left * deltaDistance;

        distanceTraveled += deltaDistance;

        if (distanceTraveled >= distanceToTravel)
            Destroy(gameObject);
    }
}
