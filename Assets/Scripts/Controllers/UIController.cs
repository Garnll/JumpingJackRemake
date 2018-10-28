using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour {

    [SerializeField]
    Image gameAreaUI;
    [SerializeField]
    TextMeshProUGUI scoreText;
    [Space(10)]
    [SerializeField]
    Image lifeImage;

    List<Image> lifes = new List<Image>();

	public void SetGameArea(float width, float height)
    {
        gameAreaUI.rectTransform.sizeDelta = new Vector2(width, height);

        SetFontSize();
    }

    public void SetTextAreaSize(float height, float width)
    {
        Vector2 newSize = new Vector2(width, height);

        scoreText.rectTransform.sizeDelta = newSize;
    }

    private void SetFontSize()
    {
        float newFontSize = ((gameAreaUI.rectTransform.sizeDelta.x) * scoreText.fontSize) / (570);

        scoreText.fontSize = newFontSize;
    }

    public void SetScore(int hiScore, int score)
    {
        scoreText.text = string.Format("HI" + hiScore.ToString("00000") + "  " + "SC" + score.ToString("00000"));
    }

    public void SetLifes(int number)
    {
        if (lifes.Count < number)
        {
            for (int i = 0; i < number; i++)
            {
                if (lifes.Count < i + 1)
                {
                    lifes.Add(Instantiate(lifeImage, gameAreaUI.rectTransform));

                    float newImageSize = scoreText.rectTransform.sizeDelta.y * 0.4f;

                    lifes[i].rectTransform.sizeDelta = new Vector2(newImageSize, newImageSize);

                    lifes[i].rectTransform.anchoredPosition = new Vector2(0 + newImageSize * 0.75f * i, scoreText.rectTransform.sizeDelta.y * 0.5f);
                }
                else
                {
                    lifes[i].gameObject.SetActive(true);
                }
            }
        }
        else if (lifes.Count > number)
        {
            int lifesLeft = lifes.Count;
            while (lifesLeft > number)
            {
                lifes[lifesLeft - 1].gameObject.SetActive(false);
                lifesLeft--;
            }
        }
    }

}
