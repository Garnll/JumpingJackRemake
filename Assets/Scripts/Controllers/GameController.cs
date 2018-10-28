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

    float score;
    float highScore;
    int lifes;

    public ObjectManager objectManager { get; set; }

    int currentLevel;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        currentLevel = 0;
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

        uiController.SetScore(highScore, score);
    }

}
