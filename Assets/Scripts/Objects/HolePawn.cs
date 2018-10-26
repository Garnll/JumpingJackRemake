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

    protected override void CheckEndOfScreen()
    {

    }

    protected override void ChangeFloor()
    {

    }
}
