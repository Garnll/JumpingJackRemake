using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPawn : Pawn, IControllable {

    [SerializeField]
    float maxJumpSpeed = 0.5f;
    [Space(10)]
    [SerializeField]
    float maxStunTime = 4;
    [SerializeField]
    float fallStunTime = 1;
    [SerializeField]
    float enemyStunTime = 2;
    [SerializeField]
    float ceilingStunTime = 3;
    [Space(10)]
    [SerializeField]
    SpriteRenderer blinkScreen;
    [SerializeField]
    Color hitCeilingColor;
    [SerializeField]
    Color hazardDamageColor;


    Animator myAnimator;

    float jumpLength = -1;
    Vector2 originalSpawnPoint;

    bool isJumping;
    bool isFalling;
    bool gotDamaged;
    bool isStunned;
    float stunTime;
    float currentStunTime;

    bool hitCeiling;
    bool hitHole;

    HolePawn lastHoleJumped;
    private float blinkScreenTime;

    public delegate void Jumping();
    public static event Jumping OnPassThruHole;

    public bool IsJumping
    {
        get
        {
            return isJumping;
        }
    }

    public HolePawn LastHoleJumped
    {
        get
        {
            return lastHoleJumped;
        }
        set
        {
            lastHoleJumped = value;
        }
    }

    private void Start()
    {
        myAnimator = GetComponent<Animator>();

        blinkScreen.gameObject.SetActive(false);
        currentFloor = 0;
        currentStunTime = 0;
        isStunned = false;
        isJumping = false;
        isFalling = false;
        gotDamaged = false;
    }

    public void SetStartPosition(Vector2 start)
    {
        originalSpawnPoint = start;
        transform.position = originalSpawnPoint;
    }

    public void ReceiveInput(float horizontal)
    {
        if (GameController.Instance.endLevel)
        {
            return;
        }

        if (isStunned || isJumping || isFalling || gotDamaged)
        {
            return;
        }

        myAnimator.ResetTrigger("hitCeiling");
        myAnimator.ResetTrigger("hitFloor");
        myAnimator.SetInteger("horizontal", (int)horizontal);
        myDirection.x = horizontal;
        myDirection.y = 0;
        Move(myDirection);
    }

    public void ReceiveInput(bool jumpCommand)
    {
        if (GameController.Instance.endLevel)
        {
            return;
        }

        if (isStunned || !jumpCommand || isJumping || isFalling || gotDamaged)
        {
            return;
        }


        isJumping = true;
        CheckForHole();
    }

    protected override void Move(Vector2 direction)
    {
        if (direction.x != 0)
        {
            GameController.Instance.audioController.PlayWalk();
        }
        else
        {
            GameController.Instance.audioController.PlayIdle();
        }
        base.Move(direction);
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
            lastHoleJumped = hit.collider.GetComponent<HolePawn>();
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

    #region Fall Methods

    public void StartFall()
    {
        if (isFalling)
        {
            return;
        }

        isFalling = true;
        GameController.Instance.audioController.PlayFall();
        myAnimator.SetBool("falling", isFalling);
        StartCoroutine(Fall());
    }

    private IEnumerator Fall()
    {
        Vector2 fallDestiny = new Vector2(transform.position.x, originalSpawnPoint.y + jumpLength * (currentFloor - 1));

        Vector2 start = transform.position;
        float jumpCurrentTime = 0;

        while (Vector2.Distance(transform.position, fallDestiny) > 0.01f)
        {
            yield return new WaitForFixedUpdate();
            jumpCurrentTime += maxJumpSpeed * Time.fixedDeltaTime;

            transform.position = Vector2.Lerp(start, fallDestiny, jumpCurrentTime);
        }
        ChangeFloor(-1);

        isFalling = false;
        myAnimator.SetBool("falling", isFalling);

        transform.position = fallDestiny;

        myAnimator.SetTrigger("hitFloor");
        StunByFalling();
    }

    #endregion

    #region Jump Methods
    void StartJump()
    {
        GameController.Instance.audioController.PlayJump();
        myAnimator.SetBool("jumping", isJumping);
        StartCoroutine(Jump());
    }

    public IEnumerator Jump()
    {
        if (hitHole)
        {
            Vector2 jumpDestiny = new Vector2(transform.position.x, originalSpawnPoint.y + jumpLength * (currentFloor + 1));

            Vector2 start = transform.position;
            float jumpCurrentTime = 0;

            while (Vector2.Distance(transform.position, jumpDestiny) > 0.01f)
            {
                yield return new WaitForFixedUpdate();
                isJumping = true;
                jumpCurrentTime += maxJumpSpeed * Time.fixedDeltaTime;

                transform.position = Vector2.Lerp(start, jumpDestiny, jumpCurrentTime);
            }
            JumpThroughHole();

            transform.position = jumpDestiny;

            EndJump();
            hitHole = false;
            yield break;
        }

        else if (hitCeiling)
        {
            Vector2 start = transform.position;
            float jumpCurrentTime = 0;

            Vector2 jumpDestiny = new Vector2(transform.position.x, (transform.position.y + jumpLength) - mySpriteRenderer.bounds.size.y);

            if (hitHole)
            {
                yield break;
            }

            while (Vector2.Distance(transform.position, jumpDestiny) > 0.01f)
            {
                yield return new WaitForFixedUpdate();
                isJumping = true;
                jumpCurrentTime += maxJumpSpeed * Time.fixedDeltaTime;

                transform.position = Vector2.Lerp(start, jumpDestiny, jumpCurrentTime);
            }

            transform.position = jumpDestiny;
           
            StartStunByHittingCeiling();

            hitCeiling = false;
            yield break;
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
        isJumping = false;
        myAnimator.SetBool("jumping", isJumping);
    }

    #endregion

    #region Damage Methods

    public void Damage()
    {
        if (isJumping)
        {
            return;
        }

        gotDamaged = true;
        StunByDamagedByHazard();
    }


    #endregion

    #region Stun Methods

    private void StartStunByHittingCeiling()
    {
        GameController.Instance.audioController.PlayHitCeiling();

        blinkScreen.gameObject.SetActive(true);
        blinkScreen.color = hitCeilingColor;
        blinkScreenTime = 0.2f;

        myAnimator.SetTrigger("hitCeiling");

        ChangeFloor(0);
    }

    private void StunByHittingCeiling()
    {
        EndJump();

        transform.position = new Vector2(transform.position.x, (transform.position.y - jumpLength) + mySpriteRenderer.bounds.size.y);

        currentStunTime += ceilingStunTime;

        if (!isStunned)
        {
            StartCoroutine(Stun());
        }
    }

    private void StunByDamagedByHazard()
    {
        GameController.Instance.audioController.PlayHazard();

        blinkScreen.gameObject.SetActive(true);
        blinkScreen.color = hazardDamageColor;
        blinkScreenTime = 0.2f;

        if (currentStunTime < enemyStunTime)
        {
            currentStunTime = enemyStunTime;
        }

        if (!isStunned)
        {
            StartCoroutine(Stun());
        }

        gotDamaged = false;
    }

    private void StunByFalling()
    {
        if (currentFloor > 0)
        {
            if (currentStunTime < fallStunTime)
            {
                currentStunTime = fallStunTime;
            }
        }
        else
        {
            currentStunTime += ceilingStunTime;
        }

        if (!isStunned)
        {
            StartCoroutine(Stun());
        }
    }

    private IEnumerator Stun()
    {
        isStunned = true;

        myAnimator.SetBool("stunned", isStunned);
        while (currentStunTime > 0)
        {
            GameController.Instance.audioController.PlayStun();

            if (currentStunTime > maxStunTime)
            {
                currentStunTime = maxStunTime;
            }

            if (blinkScreenTime <= 0)
            {
                blinkScreen.gameObject.SetActive(false);
            }

            yield return new WaitForFixedUpdate();
            currentStunTime -= Time.fixedDeltaTime;
            if (blinkScreenTime > 0)
            {
                blinkScreenTime -= Time.fixedDeltaTime;
            }
        }

        EndStun();
    }

    private void EndStun()
    {
        GameController.Instance.audioController.StopPlaying();
        currentStunTime = 0;
        isStunned = false;

        myAnimator.SetBool("stunned", isStunned);
    }

    #endregion

    protected override void ChangeFloor(int floor)
    {
        currentFloor += floor;

        if (currentFloor == 0)
        {
            LoseALife();
        }

        if (currentFloor >= GameController.Instance.objectManager.MaxVisibleFloors)
        {
            GameController.Instance.Win();
        }

        if (currentFloor < 0)
        {
            Debug.LogWarning("Current floor negative. This shouldn't happen");
            currentFloor = 0;
        }

    }

    void LoseALife()
    {
        GameController.Instance.ChangeLifeCount(-1);
    }
}
