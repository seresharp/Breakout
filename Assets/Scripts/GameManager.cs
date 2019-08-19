using System.Text;
using Interfaces;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

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

        // Begin the game
        LoadStage(_level);
        AddBall();
    }

    private void Update()
    {
        if (Application.isEditor && Input.GetKeyDown(KeyCode.F1))
        {
            foreach (IDamageable damageable in FindObjectsOfType<Brick>())
            {
                damageable.TakeHit(1);
            }
        }

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

        // Go to the next stage if there are no more bricks
        if (_brickContainer.transform.childCount == 0)
        {
            _level++;
            levelText.text = "Level " + _level;

            // Destroy current balls, load the next stage
            foreach (Transform ball in _ballContainer.transform)
            {
                Destroy(ball.gameObject);
            }

            LoadStage(_level);
            AddBall();
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

    private void LoadStage(int stage)
    {
        TextAsset t = Resources.Load<TextAsset>("Levels/" + stage);
        string[] lines = t == null ? CreateRandomStage() : t.text.Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            Debug.Log(lines[i]);
            for (int j = 0; j < lines[i].Length; j++)
            {
                // Ignore any invalid colors
                if (lines[i][j] != 'R' && lines[i][j] != 'O' && lines[i][j] != 'G')
                {
                    continue;
                }

                // Create object from prefab
                GameObject brickObj = Instantiate(brickPrefab, _brickContainer.transform);
                brickObj.transform.position = new Vector2(j * _brickSize.x, 1.25f - i * _brickSize.y);

                Brick brick = brickObj.GetComponent<Brick>();

                // Set color and points
                switch (lines[i][j])
                {
                    case 'R':
                        brick.SetColor(Color.red);
                        brick.SetPoints(5);
                        break;
                    case 'O':
                        brick.SetColor(new Color(1, 165f / 255f, 0));
                        brick.SetPoints(3);
                        break;
                    case 'G':
                        brick.SetColor(Color.green);
                        brick.SetPoints(1);
                        break;
                }
            }
        }
    }

    private string[] CreateRandomStage()
    {
        char[] colors = {'R', 'O', 'G', ' '};
        Random rnd = new Random();

        string[] lines = new string[rnd.Next(5) + 5];
        for (int i = 0; i < lines.Length; i++)
        {
            StringBuilder line = new StringBuilder();
            for (int j = 0; j < 11; j++)
            {
                line.Append(colors[rnd.Next(colors.Length)]);
            }

            lines[i] = line.ToString();
        }

        return lines;
    }
}
