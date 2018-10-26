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
        if (transform.position.x <= LevelController.leftLevelBorder + mySpriteRenderer.size.x * 0.25f)
        {
            transform.position = new Vector2(LevelController.rightLevelBorder - mySpriteRenderer.size.x * 0.25f, transform.position.y);
        }

        if (transform.position.x >= LevelController.rightLevelBorder - mySpriteRenderer.size.x * 0.25f)
        {
            transform.position = new Vector2(LevelController.leftLevelBorder + mySpriteRenderer.size.x * 0.25f, transform.position.y);
        }
    }

    protected override void ChangeFloor()
    {

    }
}
