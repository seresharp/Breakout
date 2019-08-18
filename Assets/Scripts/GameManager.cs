using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject brickPrefab;
    public GameObject ballPrefab;

    private GameObject _paddle;

    private GameObject _brickContainer;
    private Vector2 _brickSize;

    private GameObject _ballContainer;
    private Vector2 _ballSize;

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    Debug.LogWarning("No GameManager found in scene, make sure one exists");
                }
            }

            return _instance;
        }
    }

    private void Start()
    {
        // Locate paddle
        _paddle = GameObject.FindWithTag("Paddle");

        // Get size of objects
        _brickSize = brickPrefab.GetComponent<BoxCollider2D>().size * brickPrefab.transform.localScale;

        float radius = ballPrefab.GetComponent<CircleCollider2D>().radius;
        _ballSize = new Vector2(radius * 2, radius * 2) * ballPrefab.transform.localScale;

        // Containers to hold every object in play
        _brickContainer = new GameObject("Bricks");
        _ballContainer = new GameObject("Balls");

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

        // Begin the game
        AddBall();
    }

    private void Update()
    {
        // Destroy balls that fall below the stage
        foreach (Transform ball in _ballContainer.transform)
        {
            if (ball.position.y < _paddle.transform.position.y - 0.25f)
            {
                Destroy(ball.gameObject);
            }
        }

        // Add a new ball if the player is out
        if (_ballContainer.transform.childCount == 0)
        {
            AddBall();
        }
    }

    public void AddBall()
    {
        Instantiate(ballPrefab, _paddle.transform.position + new Vector3(0, _ballSize.y / 2f), Quaternion.identity,
            _ballContainer.transform);
    }
}
