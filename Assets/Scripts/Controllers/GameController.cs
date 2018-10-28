using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    int score;
    int highScore;
    int lifes;

    public ObjectManager objectManager { get; set; }

    int currentLevel;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        currentLevel = 0;
        score = 0;
        highScore = 0; //Temporal, falta hacer la puntuación alta.
        lifes = initialLifes;
    }

    public void Win()
    {
        Time.timeScale = 0;
    }

    public void AdvanceToNextLevel()
    {
        currentLevel++;
    }

    public void SetUpUI(float width, float height, float floorHeight)
    {
        if (uiController == null)
        {
            uiController = GameObject.FindObjectOfType<UIController>();
        }

        uiController.SetTextAreaSize(floorHeight, width * 0.5f);
        uiController.SetGameArea(width, height);

        ChangeScore(score);
        ChangeLifeCount(0);
    }

    public void ChangeHighScore(int newHighScore)
    {
        highScore = newHighScore;
    }

    public void ChangeScore(int score)
    {
        score += score;
        uiController.SetScore(highScore, score);
    }

    public void ChangeLifeCount(int change)
    {
        lifes += change;
        uiController.SetLifes(lifes);
    }

}
