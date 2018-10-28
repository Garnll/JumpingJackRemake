using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenController : MonoBehaviour {

    [SerializeField]
    float waitTime = 3;

	void Start () {
        //Inicia animación de titulo
        StartTimer();
	}
	
	private void StartTimer()
    {
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
