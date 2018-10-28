using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HighScoreManager {

    static int highScore = 0;

    public void LoadHighScore()
    {
        string dataPath = Path.Combine(Application.persistentDataPath, "hs.json");

        if (File.Exists(dataPath))
        {
            string dataAsJson = File.ReadAllText(dataPath);
            highScore = JsonUtility.FromJson<int>(dataAsJson);
        }
    }

    public void ChangeHighscore(int newHighscore)
    {
        highScore = newHighscore;
    }

    private void SaveHighScore()
    {
        string dataAsJson = JsonUtility.ToJson(highScore);
        string dataPath = Path.Combine(Application.persistentDataPath, "hs.json");
        
        File.WriteAllText(dataPath, dataAsJson);
    }

}
