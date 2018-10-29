using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    private static GameController instance = null;

    public static GameController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<GameController>();
            }
            return instance;
        }
    }

    [SerializeField]
    UIController uiController;
    [SerializeField]
    int initialLifes = 6;
    [SerializeField]
    int maxLevels = 21;
    [SerializeField]
    int[] levelsToGiveLife;

    int score;
    int highScore;
    int lifes;

    public ObjectManager objectManager { get; set; }
    HighScoreManager highScoreManager;

    int currentLevel;

    public int CurrentLevel
    {
        get
        {
            return currentLevel;
        }
    }

    public int MaxLevels
    {
        get
        {
            return maxLevels;
        }
    }

    public int Score
    {
        get
        {
            return score;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        highScoreManager = new HighScoreManager();

        currentLevel = 1;
        score = 0;
        lifes = initialLifes;
    }

    private void Update()
    {
        //DONE FOR TESTING
        if (Input.GetKeyDown(KeyCode.K))
        {
            Win();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void Win()
    {
        Time.timeScale = 0;

        //Suena música de ganar

        GoToLoadingScreen();
    }

    private void GoToGameOverScreen()
    {
        SceneManager.LoadScene("GameOverScreen");
    }

    private void GoToLoadingScreen()
    {
        currentLevel++;
        SceneManager.LoadScene("LoadingScreen");
    }

    public void AdvanceToNextLevel()
    {
        if (currentLevel <= maxLevels)
        {
            SceneManager.LoadScene("Level" + currentLevel);
        }
        else
        {
            GoToGameOverScreen();
        }
    }

    public void SetUpUI(float width, float height, float floorHeight)
    {
        if (uiController == null)
        {
            uiController = GameObject.FindObjectOfType<UIController>();
        }

        highScoreManager.LoadHighScore();
        highScore = highScoreManager.currentHighScore;

        uiController.SetTextAreaSize(floorHeight, width * 0.5f);
        uiController.SetGameArea(width, height);

        ChangeScore(0);
        ChangeLifeCount(0);
    }

    public bool ShouldGiveExtraLife()
    {
        for (int i = 0; i < levelsToGiveLife.Length; i++)
        {
            if (currentLevel == levelsToGiveLife[i])
            {
                ChangeLifeCount(+1);
                return true;
            }
        }

        return false;
    }

    public bool ShouldChangeHighScore()
    {
        if (score > highScore)
        {
            highScore = score;
            highScoreManager.ChangeHighscore(highScore);
            return true;
        }

        return false;
    }

    public void ChangeScore(int newScore)
    {
        score += newScore;
        uiController.SetScore(highScore, score);
    }

    public void ChangeLifeCount(int change)
    {
        lifes += change;
        if (uiController != null)
        {
            uiController.SetLifes(lifes);
        }

        if (lifes <= 0)
        {
            Lose();
        }
    }

    private void Lose()
    {
        Time.timeScale = 0;

        //Suena música de perdida

        GoToGameOverScreen();
    }

    public void Restart()
    {
        currentLevel = 1;
        score = 0;
        lifes = initialLifes;
        AdvanceToNextLevel();
    }

}
