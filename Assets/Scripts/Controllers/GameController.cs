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

    [SerializeField]
    public AudioController audioController { get; private set; }

    int score;
    int highScore;
    int lifes;

    public bool endLevel { get; private set; }

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

    public int Lifes
    {
        get
        {
            return lifes;
        }
    }

    private void Awake()
    {
        Cursor.visible = false;
        DontDestroyOnLoad(this);
        highScoreManager = new HighScoreManager();
        endLevel = false;

        if (audioController == null)
        {
            audioController = GetComponent<AudioController>();  
        }

        currentLevel = 1;
        score = 0;
        lifes = initialLifes;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    /// <summary>
    /// Stops the game and plays the win sound.
    /// </summary>
    public void Win()
    {
        Time.timeScale = 0;
        endLevel = true;

        audioController.PlayWin();
    }

    public void GoToGameOverScreen()
    {
        SceneManager.LoadScene("GameOverScreen");
    }

    public void GoToLoadingScreen()
    {
        currentLevel++;
        SceneManager.LoadScene("LoadingScreen");
    }

    public void AdvanceToNextLevel()
    {
        if (currentLevel <= maxLevels)
        {
            endLevel = false;
            SceneManager.LoadScene("Level" + currentLevel);
        }
        else
        {
            GoToGameOverScreen();
        }
    }

    /// <summary>
    /// Sets the UI size, and it's first values.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="floorHeight"></param>
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

    /// <summary>
    /// Checks if the next level gives an extra life.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Checks if the current score is higher that the last Highscore.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Adds the score given to the current score.
    /// </summary>
    /// <param name="newScore"></param>
    public void ChangeScore(int newScore)
    {
        score += newScore;
        uiController.SetScore(highScore, score);
    }

    /// <summary>
    /// Adds the lifes given to the current lifes.
    /// </summary>
    /// <param name="change"></param>
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

    /// <summary>
    /// Stops the game and plays the lose sound.
    /// </summary>
    private void Lose()
    {
        endLevel = true;
        Time.timeScale = 0;

        audioController.StopPlaying();
        audioController.PlayLose();
    }

    /// <summary>
    /// Restarts the values of the Gamecontroller, returning it to the first level.
    /// </summary>
    public void Restart()
    {
        endLevel = false;
        currentLevel = 1;
        score = 0;
        lifes = initialLifes;
        AdvanceToNextLevel();
    }

}
