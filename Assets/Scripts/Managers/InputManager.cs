using UnityEngine;

public class InputManager : MonoBehaviour {

    [SerializeField]
    PlayerPawn playerControllable;

    private void Update ()
    {
        playerControllable.ReceiveInput(Input.GetAxisRaw("Horizontal"));

        playerControllable.ReceiveInput(Input.GetButton("Jump") || Input.GetKey(KeyCode.UpArrow));
	}
}
