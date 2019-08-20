using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    public GameObject brickPrefab;
    public GameObject ballPrefab;
    public Text scoreText;
    public Text livesText;
    public Text levelText;
    public GameObject playButton;
    public GameObject gameOverScreen;
    public GameObject nextLevelPrompt;

    private int _lives = 3;
    private int _score;
    private int _level = 1;

    private GameObject _paddle;
    private Vector2 _paddleStartPosition;

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

    public int Lives
    {
        get => _lives;
        set
        {
            _lives = value;
            livesText.text = "Lives: " + _lives;
        }
    }

    public int Score
    {
        get => _score;
        set
        {
            _score = value;
            scoreText.text = "Score: " + _score;
        }
    }

    public int Level
    {
        get => _level;
        set
        {
            _level = value;
            levelText.text = "Level " + _level;
        }
    }

    private void Start()
    {
        // Locate paddle
        _paddle = GameObject.FindWithTag("Paddle");
        _paddleStartPosition = _paddle.transform.position;

        // Get size of objects
        _brickSize = brickPrefab.GetComponent<BoxCollider2D>().size * brickPrefab.transform.localScale;

        float radius = ballPrefab.GetComponent<CircleCollider2D>().radius;
        _ballSize = new Vector2(radius * 2, radius * 2) * ballPrefab.transform.localScale;

        // Containers to hold every object in play
        _brickContainer = new GameObject("Bricks");
        _ballContainer = new GameObject("Balls");

        // Begin the game
        StartCoroutine(LoadStage(Level));
    }

    private void Update()
    {
        if (Time.timeScale < Mathf.Epsilon)
        {
            return;
        }

        if (Application.isEditor && Input.GetKeyDown(KeyCode.F1))
        {
            foreach (IDamageable damageable in FindObjectsOfType<Brick>())
            {
                damageable.TakeHit(1);
            }
        }

        if (Application.isEditor && Input.GetKeyDown(KeyCode.F2))
        {
            foreach (Transform ball in _ballContainer.transform)
            {
                Destroy(ball.gameObject);
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
            if (Lives <= 0)
            {
                // Destroy all bricks so the game over text isn't obscured
                foreach (Transform brick in _brickContainer.transform)
                {
                    Destroy(brick.gameObject);
                }

                // Display game over screen
                gameOverScreen.SetActive(true);
                Time.timeScale = 0f;
            }
            else
            {
                Lives--;
                AddBall();
            }
        }

        // Go to the next stage if there are no more bricks
        if (_brickContainer.transform.childCount == 0)
        {
            StartCoroutine(LoadStage(++Level));
        }
    }

    [UsedImplicitly]
    public void StartGame()
    {
        playButton.SetActive(false);
        Time.timeScale = 1f;
    }

    [UsedImplicitly]
    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    public void AddBall()
    {
        // Spawn a ball slightly over the paddle
        Instantiate(ballPrefab, _paddle.transform.position + new Vector3(0, _ballSize.y), Quaternion.identity,
            _ballContainer.transform);
    }

    private IEnumerator LoadStage(int stage)
    {
        ResetPaddle();

        // Destroy current balls before loading the next stage
        foreach (Transform ball in _ballContainer.transform)
        {
            Destroy(ball.gameObject);
        }

        // Pause the game for the pop in effect
        Time.timeScale = 0f;
        AddBall();

        // Display next level prompt
        if (stage > 1)
        {
            nextLevelPrompt.transform.Find("LevelText").GetComponent<Text>().text = "Starting Level " + stage;
            nextLevelPrompt.SetActive(true);
        }

        // Load level from resources, or create a random one if there's none left
        TextAsset t = Resources.Load<TextAsset>("Levels/" + stage);
        string[] lines = t == null ? CreateRandomStage() : t.text.Split('\n');

        // Setup values for random effect
        List<int> yVals = Enumerable.Range(0, lines.Length).ToList();

        Dictionary<int, List<int>> xVals = new Dictionary<int, List<int>>();
        for (int i = 0; i < lines.Length; i++)
        {
            xVals.Add(i, Enumerable.Range(0, lines[i].Length).ToList());
        }

        Random rnd = new Random();
        while (yVals.Count > 0)
        {
            // Choose random x and y to place brick at
            int i = rnd.Next(yVals.Count);
            int y = yVals[i];
            int x = xVals[y].PopRandom();

            // Remove full rows from the y list
            if (xVals[y].Count == 0)
            {
                yVals.RemoveAt(i);
            }

            // Ignore any invalid colors
            if (lines[y][x] != 'R' && lines[y][x] != 'O' && lines[y][x] != 'G')
            {
                continue;
            }

            // Create object from prefab
            GameObject brickObj = Instantiate(brickPrefab, _brickContainer.transform);
            brickObj.transform.position = new Vector2(x * _brickSize.x, 1.25f - y * _brickSize.y);

            Brick brick = brickObj.GetComponent<Brick>();

            // Set color and points
            switch (lines[y][x])
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
                default:
                    brick.SetColor(Color.green);
                    brick.SetPoints(1);
                    break;
            }

            yield return new WaitForSecondsRealtime(0.01f);
        }

        if (Level == 1)
        {
            // Wait for a button press for the first stage
            playButton.SetActive(true);
        }
        else
        {
            // Start without user input on other levels
            // Beginning immediately after the bricks pop in is too abrupt
            yield return new WaitForSecondsRealtime(1f);
            nextLevelPrompt.SetActive(false);
            Time.timeScale = 1f;
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

    private void ResetPaddle()
    {
        _paddle.transform.position = _paddleStartPosition;
        _paddle.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }
}
