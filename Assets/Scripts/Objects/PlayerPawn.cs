using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPawn : Pawn, IControllable {

    Animator myAnimator;
    float jumpLength;

    bool isJumping;
    bool isStunned;
    float stunTime;
    float currentStunTime;

    public void ReceiveInput(float horizontal)
    {
        if (isStunned)
        {
            return;
        }

        myDirection.x = horizontal;
        myDirection.y = 0;
        Move(myDirection);
    }

    public void ReceiveInput(bool jump)
    {
        if (isStunned)
        {
            return;
        }
    }

    protected override void CheckEndOfScreen()
    {
        if (transform.position.x < LevelController.leftLevelBorder)
        {
            transform.position = new Vector2(LevelController.rightLevelBorder, transform.position.y);
        }

        if (transform.position.x > LevelController.rightLevelBorder)
        {
            transform.position = new Vector2(LevelController.leftLevelBorder, transform.position.y);
        }
    }

    protected override void ChangeFloor(int floor)
    {

    }
}
