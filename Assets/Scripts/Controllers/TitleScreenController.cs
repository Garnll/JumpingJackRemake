using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenController : MonoBehaviour {

    [SerializeField]
    float waitTime = 3;
    [SerializeField]
    GameObject copyrightText;

    public void StartTimer()
    {
        copyrightText.SetActive(true);
        StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        while (waitTime > 0)
        {
            yield return new WaitForFixedUpdate();
            waitTime -= Time.fixedDeltaTime;
        }

        GameController.Instance.AdvanceToNextLevel();
    }
}
