using System.Collections;
using UnityEngine;
using UnityEngine.UI;
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

    [Space(10)]
    [SerializeField]
    Image extraLifeBox;
    [SerializeField]
    TextMeshProUGUI extraLifeText;
    [SerializeField]
    Color color1, color2;
    [SerializeField]
    float changeColorTime = 0.5f;


    bool writing;

    private void Awake()
    {
        Time.timeScale = 1;

        extraLifeBox.gameObject.SetActive(false);
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

    private void CheckNextLevel()
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

    private void CheckPoem()
    {
        if (GameController.Instance.CurrentLevel > 1)
        {
            StartCoroutine(AnimateText(poemFragment[GameController.Instance.CurrentLevel - 2]));
            //On the first level there's no poem, so the first part of the poem it's in the second level, meaning position 0 = level - 2.
        }
        else
        {
            pressBox.SetActive(true);
        }

    }

    private void GoToNextLevel()
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

        if (GameController.Instance.ShouldGiveExtraLife())
        {
            extraLifeBox.gameObject.SetActive(true);
            StartCoroutine(AnimateExtraLife());
        }

        SetPressBoxActive();
        
    }

    private void SetPressBoxActive()
    {
        pressBox.SetActive(true);
        writing = false;
    }

    private IEnumerator AnimateExtraLife()
    {
        Color buffer = color1;
        extraLifeBox.color = color1;
        extraLifeText.color = color2;

        while (true)
        {
            buffer = extraLifeBox.color;
            extraLifeBox.color = extraLifeText.color;
            extraLifeText.color = buffer;
            yield return new WaitForSeconds(changeColorTime);
        }
    }
}
