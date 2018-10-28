using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Text;

public class LoadingScreenController : MonoBehaviour {

    [SerializeField]
    TextMeshProUGUI nextLevelText, poemText;
    [SerializeField]
    GameObject nextLevelBox;
    [Space(10)]
    [SerializeField]
    float textVelocity = 0.1f;
    [SerializeField] [TextArea]
    string[] poemFragment;


    bool writing;

    private void Awake()
    {
        nextLevelBox.SetActive(true);
        writing = true;

        CheckNextLevel();
        CheckPoem();
    }

    private void LateUpdate()
    {
        if (!writing)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                GoToNextLevel();
            }
        }
    }

    void CheckNextLevel()
    {
        if (GameController.Instance.CurrentLevel > GameController.Instance.MaxLevels)
        {
            nextLevelBox.SetActive(false);
            return;
        }

        nextLevelText.text = string.Format("NEXT LEVEL - {0} HAZARD", (GameController.Instance.CurrentLevel - 1).ToString());
    }

    void CheckPoem()
    {
        StartCoroutine(AnimateText(poemFragment[GameController.Instance.CurrentLevel - 2]));
        //En el nivel 1 no hay poema, y el nivel 2 equivale a la posición 0 del array.
    }

    void GoToNextLevel()
    {
        SceneManager.LoadScene("Level" + GameController.Instance.CurrentLevel);
    }


    private IEnumerator AnimateText(string complete)
    {
        writing = true;

        StringBuilder str = new StringBuilder();
        int currentCharPosition = 0;

        while (currentCharPosition < complete.Length)
        {
            str.Append(complete[currentCharPosition++]);

            poemText.text = str.ToString();
            yield return new WaitForSeconds(textVelocity);
        }

        writing = false;
    }
}
