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

        uiController.SetGameArea(width, height);
        uiController.SetTextHeight(floorHeight);
    }

}
