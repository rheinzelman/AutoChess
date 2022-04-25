using UnityEngine;

public class ScrollingSquareManager : MonoBehaviour
{
    public GameObject ScrollingSquareSpawnerPrefab;
    public float moveSpeed = 1f;
    public float xPosDisplacement = 2.75f;
    public float yPosDisplacement = 1f;
    public int spawnerCount = 8;
    private bool bAlternate;

    // Start is called before the first frame update
    private void Start()
    {
        for (var i = 0; i < spawnerCount; i++)
        {
            var spawnerGO1 = Instantiate(ScrollingSquareSpawnerPrefab, transform);
            var newSpawner1 = spawnerGO1.GetComponent<ScrollingSquareSpawner>();

            newSpawner1.scrollDistance = xPosDisplacement * 2f;
            newSpawner1.scrollSpeed = moveSpeed;
            newSpawner1.moveRight = !bAlternate;
            newSpawner1.startWhite = bAlternate;

            if (bAlternate)
                spawnerGO1.transform.localPosition =
                    new Vector3(xPosDisplacement, i * yPosDisplacement + yPosDisplacement * 0.5f, 0f);
            else
                spawnerGO1.transform.localPosition = new Vector3(-1 * xPosDisplacement,
                    i * yPosDisplacement + yPosDisplacement * 0.5f, 0f);

            bAlternate = !bAlternate;
        }
    }
}