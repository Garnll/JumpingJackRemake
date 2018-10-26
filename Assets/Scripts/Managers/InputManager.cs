using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    [SerializeField]
    PlayerPawn playerControllable;

	void Update ()
    {
        playerControllable.ReceiveInput(Input.GetAxisRaw("Horizontal"));

        playerControllable.ReceiveInput(Input.GetButton("Jump") || Input.GetKey(KeyCode.UpArrow));
	}
}
