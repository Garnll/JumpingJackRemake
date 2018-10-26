using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolePawn : Pawn {


    private void Start()
    {
        transform.localScale = new Vector2(
            ((25 * Screen.width) / 800),
            ((2.5f * Screen.height) / 600));
    }

    public void GiveDirection(Vector2 direction)
    {
        myDirection = direction;
        StartCoroutine(UpdateMovement());
    }

    private IEnumerator UpdateMovement()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            Move(myDirection);
        }
    }

    protected override void CheckEndOfScreen()
    {

    }

    protected override void ChangeFloor()
    {

    }
}
