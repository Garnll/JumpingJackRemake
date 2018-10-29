using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallChecker : MonoBehaviour {

    [SerializeField]
    PlayerPawn player;

    private void OnEnable()
    {
        BoxCollider2D myCollider = GetComponent<BoxCollider2D>();

        myCollider.size = (player.MyCollider as BoxCollider2D).size;
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
        if (player.LastHoleJumped != null)
        {
            if (player.LastHoleJumped == collision.GetComponent<HolePawn>())
            {
                return;
            }
        }

        if (!player.IsJumping)
        {
            player.StartFall();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (player.LastHoleJumped != null)
        {
            player.LastHoleJumped = null;
        }
    }

}
