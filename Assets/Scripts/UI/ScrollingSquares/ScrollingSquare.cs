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

    private float distanceTraveled;
    [HideInInspector] public float startDistance;

    [Header("Debug")] [HideInInspector] public bool useSpecificPieceIndex;
    [HideInInspector] public int indexToUse;

    private void Start()
    {
        if (startWithPiece)
            pieceSprite.sprite = useSpecificPieceIndex
                ? possiblePieceSprites[indexToUse]
                : possiblePieceSprites[Random.Range(0, possiblePieceSprites.Count)];
        else
            pieceSprite.sprite = null;

        squareSprite.color = startWhite ? whiteColor : darkColor;

        if (startDistance > 0f)
            MoveSquare(startDistance);
    }

    // Update is called once per frame
    private void Update()
    {
        var deltaDistance = Time.deltaTime * travelSpeed;

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