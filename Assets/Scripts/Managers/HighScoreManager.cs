using System.IO;
using UnityEngine;

public class HighScoreManager {


    static int highScore = 0;

    public int currentHighScore
    {
        get
        {
            return highScore;
        }
    }

    public void LoadHighScore()
    {
        string dataPath = Path.Combine(Application.persistentDataPath, "hs.json");

        if (File.Exists(dataPath))
        {
            string dataAsJson = File.ReadAllText(dataPath);
            highScore = int.Parse(dataAsJson);
        }
    }

    public void ChangeHighscore(int newHighscore)
    {
        highScore = newHighscore;
        SaveHighScore();
    }

    private void SaveHighScore()
    {
        string dataAsJson = highScore.ToString();
        string dataPath = Path.Combine(Application.persistentDataPath, "hs.json");
        
        File.WriteAllText(dataPath, dataAsJson);
    }

}
