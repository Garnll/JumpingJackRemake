using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class GameOverScreenController : MonoBehaviour {

    [SerializeField]
    float textVelocity = 0.05f;
    [SerializeField]
    TextMeshProUGUI scoreText;

    [Space(10)]
    [SerializeField]
    Image newHighBox;
    [SerializeField]
    TextMeshProUGUI newHighText;
    [SerializeField]
    Color color1, color2;
    [SerializeField]
    float changeColorTime = 0.5f;

    [Space(10)]
    [SerializeField]
    TextMeshProUGUI pressText;

    bool writing;

    private void Awake()
    {
        Time.timeScale = 1;

        newHighBox.gameObject.SetActive(false);

        pressText.text = "";
        scoreText.text = "";
        writing = false;

        SetScore();
    }

    private void LateUpdate()
    {
        if (!writing)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Restart();
            }
        }
    }

    void SetScore()
    {
        if (GameController.Instance.CurrentLevel - 1 == 1)
        {
            StartCoroutine(AnimateScoreText(string.Format("FINAL SCORE {0}\nWITH {1} HAZARD",
                GameController.Instance.Score.ToString("00000"),
                (GameController.Instance.CurrentLevel - 1).ToString())));
        }
        else
        {
            StartCoroutine(AnimateScoreText(string.Format("FINAL SCORE {0}\nWITH {1} HAZARDS",
                GameController.Instance.Score.ToString("00000"),
                (GameController.Instance.CurrentLevel - 1).ToString())));
        }
    }

    private IEnumerator AnimateScoreText(string complete)
    {
        writing = true;

        StringBuilder str = new StringBuilder();
        int currentCharPosition = 0;

        while (currentCharPosition < complete.Length)
        {
            str.Append(complete[currentCharPosition++]);

            scoreText.text = str.ToString();
            yield return new WaitForSeconds(textVelocity);
        }

        CheckIfNewHigh();
    }

    void CheckIfNewHigh()
    {
        if (GameController.Instance.ShouldChangeHighScore())
        {
            newHighBox.gameObject.SetActive(true);
            StartCoroutine(AnimateHighBox());
        }
        StartCoroutine(AnimatePressText());
    }

    private IEnumerator AnimateHighBox()
    {
        Color buffer = color1;
        newHighBox.color = color1;
        newHighText.color = color2;


        while (true)
        {
            buffer = newHighBox.color;
            newHighBox.color = newHighText.color;
            newHighText.color = buffer;
            yield return new WaitForSeconds(changeColorTime);
        }
    }

    private IEnumerator AnimatePressText()
    {
        writing = true;

        string complete = "Press ENTER to replay";

        StringBuilder str = new StringBuilder();
        int currentCharPosition = 0;

        while (currentCharPosition < complete.Length)
        {
            str.Append(complete[currentCharPosition++]);

            pressText.text = str.ToString();
            yield return new WaitForSeconds(textVelocity);
        }

        writing = false;
    }

    private void Restart()
    {
        GameController.Instance.Restart();
    }
}
