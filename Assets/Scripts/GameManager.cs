using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject brickPrefab;
    public GameObject ballPrefab;
    public Text scoreText;
    public Text livesText;
    public Text levelText;

    private int _lives = 3;
    private int _score;
    private int _level = 1;

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
                GameObject brickObj = Instantiate(brickPrefab, _brickContainer.transform);
                brickObj.transform.position = new Vector2(i * _brickSize.x, j * _brickSize.y);

                Brick brick = brickObj.GetComponent<Brick>();

                if (j <= 1)
                {
                    brick.SetColor(Color.green);
                    brick.SetPoints(1);
                }
                else if (j <= 3)
                {
                    brick.SetColor(new Color(1, 165f / 255f, 0));
                    brick.SetPoints(3);
                }
                else
                {
                    brick.SetColor(Color.red);
                    brick.SetPoints(5);
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
            if (_lives <= 0)
            {
            }
            else
            {
                _lives--;
                livesText.text = "Lives: " + _lives;
                AddBall();
            }
        }
    }

    public void AddBall()
    {
        // Spawn a ball slightly over the paddle
        Instantiate(ballPrefab, _paddle.transform.position + new Vector3(0, _ballSize.y), Quaternion.identity,
            _ballContainer.transform);
    }

    public void GivePoints(int points)
    {
        _score += points;
        scoreText.text = "Score: " + _score;
    }
}
