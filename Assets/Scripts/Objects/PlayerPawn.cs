using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPawn : Pawn, IControllable {

    [SerializeField]
    float maxJumpTime = 0.5f;

    Animator myAnimator;
    float jumpLength = -1;

    bool isJumping;
    bool isStunned;
    float stunTime;
    float currentStunTime;

    bool hitCeiling;
    bool hitHole;


    public delegate void Jumping();
    public static event Jumping OnPassThruHole;

    private void Start()
    {
        currentFloor = 0;
        isStunned = false;
    }

    public void ReceiveInput(float horizontal)
    {
        if (isStunned || isJumping)
        {
            return;
        }

        myDirection.x = horizontal;
        myDirection.y = 0;
        Move(myDirection);
    }

    public void ReceiveInput(bool jumpCommand)
    {
        if (isStunned || !jumpCommand || isJumping)
        {
            return;
        }

        isJumping = true;
        CheckForHole();
    }

    private void CheckForHole()
    {
        if (jumpLength < 0)
        {
            jumpLength = GameController.Instance.objectManager.FloorHeight;
        }
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, jumpLength, 1 << LayerMask.NameToLayer("Hole"));

        if (hit.collider != null)
        {
            hitHole = true;
        }
        else
        {
            hitCeiling = true;
        }

        StartJump();
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
    #region Jump Methods
    void StartJump()
    {
        //Inicia Animación de Salto
        StartCoroutine(Jump());
    }

    public IEnumerator Jump()
    {
        if (hitHole)
        {
            float jumpTime = 1 / maxJumpTime;

            Vector2 jumpDestiny = new Vector2(transform.position.x, transform.position.y + jumpLength);

            //Animación de salto
            while (Vector2.Distance(transform.position, jumpDestiny) > 0.01f)
            {
                yield return new WaitForFixedUpdate();
                transform.position = Vector2.Lerp(transform.position, jumpDestiny, jumpTime * Time.fixedDeltaTime);
            }
            JumpThroughHole(); //Esto debería ser llamado a la mitad del salto

            transform.position = jumpDestiny;

            EndJump();
            hitHole = false;
        }

        else if (hitCeiling)
        {
            float jumpTime = (1 * 0.5f) / maxJumpTime;

            Vector2 jumpDestiny = new Vector2(transform.position.x, (transform.position.y + jumpLength) - mySpriteRenderer.bounds.extents.y);

            while (Vector2.Distance(transform.position, jumpDestiny) > 0.01f)
            {
                yield return new WaitForFixedUpdate();
                transform.position = Vector2.Lerp(transform.position, jumpDestiny, jumpTime * Time.fixedDeltaTime);
            }
            transform.position = jumpDestiny;

            StartStunByHittingCeiling();
            isJumping = false;
            hitCeiling = false;
        }
    }

    public void JumpThroughHole()
    {
        if (OnPassThruHole != null)
        {
            OnPassThruHole();
        }
        ChangeFloor(1);
    }

    private void EndJump()
    {
        //Animación de final de salto?
        isJumping = false;
    }

    #endregion

    #region Stun Methods

    private void StartStunByHittingCeiling()
    {
        isStunned = true;
        //Animación de que se golpea

        StunByHittingCeiling();
    }

    private void StunByHittingCeiling()
    {
        transform.position = new Vector2(transform.position.x, (transform.position.y - jumpLength) + mySpriteRenderer.bounds.extents.y);
        StartCoroutine(Stun(3));
    }

    private IEnumerator Stun(float stunnedTime)
    {
        stunTime = stunnedTime;

        Debug.Log("STUNNED");
        yield return new WaitForSeconds(stunnedTime);
        //Animación de que está stunneado
        EndStun();
    }

    private void EndStun()
    {
        //De vuelta a Idle
        Debug.Log("No longer stunned");
        isStunned = false;
    }

    #endregion

    protected override void ChangeFloor(int floor)
    {
        currentFloor += floor;

        if (currentFloor >= GameController.Instance.objectManager.MaxVisibleFloors)
        {
            Debug.Log("Win");
            //Ganó
        }

        if (currentFloor < 0)
        {
            Debug.LogWarning("Current floor negative. This shouldn't happen");
            currentFloor = 0;
        }
    }
}
