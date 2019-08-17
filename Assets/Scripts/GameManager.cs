using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject brickPrefab;
    private GameObject _brickContainer;
    private Vector2 _brickSize;

    private void Start()
    {
        // Get size of brick
        _brickSize = brickPrefab.GetComponent<BoxCollider2D>().size * brickPrefab.transform.localScale;

        // Container to hold every brick in play
        _brickContainer = new GameObject("Bricks");

        // Create game stage TODO: Don't hard code this
        for (int i = 0; i < 11; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                GameObject brick = Instantiate(brickPrefab, _brickContainer.transform);
                brick.transform.position = new Vector2(i * _brickSize.x, j * _brickSize.y);

                if (j <= 1)
                {
                    brick.GetComponent<Brick>().SetColor(Color.green);
                }
                else if (j <= 3)
                {
                    brick.GetComponent<Brick>().SetColor(new Color(1, 165f / 255f, 0));
                }
                else
                {
                    brick.GetComponent<Brick>().SetColor(Color.red);
                }
            }
        }
    }
}
