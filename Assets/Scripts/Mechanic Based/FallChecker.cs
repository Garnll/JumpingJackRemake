using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallChecker : MonoBehaviour {

    [SerializeField]
    PlayerPawn player;

    private void OnEnable()
    {
        BoxCollider2D myCollider = GetComponent<BoxCollider2D>();

        myCollider.size = player.MySpriteRenderer.size * 0.25f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (player == null)
        {
            player = GetComponentInParent<PlayerPawn>();
        }

        if (!player.IsJumping)
        {
            player.StartFall();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!player.IsJumping)
        {
            player.StartFall();
        }
    }

}
