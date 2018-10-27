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

    public void SetTextHeight(float height)
    {
        textHeight = height;

        scoreText.rectTransform.sizeDelta = new Vector2(scoreText.rectTransform.sizeDelta.x, height);
    }

}
