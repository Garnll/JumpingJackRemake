﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

public class LoadingScreenController : MonoBehaviour {

    [SerializeField]
    TextMeshProUGUI nextLevelText, poemText;
    [SerializeField]
    GameObject nextLevelBox;
    [SerializeField]
    GameObject pressBox;
    [Space(10)]
    [SerializeField]
    float textVelocity = 0.1f;
    [SerializeField] [TextArea]
    string[] poemFragment;


    bool writing;

    private void Awake()
    {
        Time.timeScale = 1;

        nextLevelBox.SetActive(true);
        pressBox.SetActive(false);
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

        if (GameController.Instance.CurrentLevel - 1 == 1)
        {
            nextLevelText.text = string.Format("NEXT LEVEL - {0} HAZARD", (GameController.Instance.CurrentLevel - 1).ToString());
        }
        else
        {
            nextLevelText.text = string.Format("NEXT LEVEL - {0} HAZARDS", (GameController.Instance.CurrentLevel - 1).ToString());
        }
    }

    void CheckPoem()
    {
        if (GameController.Instance.CurrentLevel > 1)
        {
            StartCoroutine(AnimateText(poemFragment[GameController.Instance.CurrentLevel - 2]));//En el nivel 1 no hay poema, y el nivel 2 equivale a la posición 0 del array.
        }
        else
        {
            pressBox.SetActive(true);
        }

    }

    void GoToNextLevel()
    {
        GameController.Instance.AdvanceToNextLevel();
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

        pressBox.SetActive(true);
        writing = false;
    }
}
