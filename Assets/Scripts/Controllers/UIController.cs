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

    float textHeight;

	public void SetGameArea(float width, float height)
    {
        gameAreaUI.rectTransform.sizeDelta = new Vector2(width, height);
    }

    public void SetTextAreaSize(float height, float width)
    {
        textHeight = height;

        Vector2 newSize = new Vector2(width, height);

        scoreText.rectTransform.sizeDelta = newSize;

        SetFontSize();
    }

    private void SetFontSize()
    {
        float newFontSize = ((gameAreaUI.rectTransform.sizeDelta.x * gameAreaUI.rectTransform.sizeDelta.y) * scoreText.fontSize) / (570 * 480);

        scoreText.fontSize = newFontSize;
    }

    public void SetScore(float hiScore, float score)
    {
        scoreText.text = string.Format("HI" + hiScore.ToString("00000") + "  " + "SC" + score.ToString("00000"));
    }

}
