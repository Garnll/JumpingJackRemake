using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPawn : Pawn, IControllable {

    [SerializeField]
    float maxJumpTime = 0.5f;
    [Space(10)]
    [SerializeField]
    float maxStunTime = 4;
    [SerializeField]
    float fallStunTime = 1;
    [SerializeField]
    float enemyStunTime = 2;
    [SerializeField]
    float ceilingStunTime = 3;

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


    public delegate void Jumping();
    public static event Jumping OnPassThruHole;

    public bool IsJumping
    {
        get
        {
            return isJumping;
        }
    }


    private void Start()
    {
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
        if (isStunned || isJumping || isFalling || gotDamaged)
        {
            return;
        }

        myDirection.x = horizontal;
        myDirection.y = 0;
        Move(myDirection);
    }

    public void ReceiveInput(bool jumpCommand)
    {
        if (isStunned || !jumpCommand || isJumping || isFalling || gotDamaged)
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

    #region Fall Methods

    public void StartFall()
    {
        if (isFalling)
        {
            return;
        }

        //animación de caida
        isFalling = true;
        StartCoroutine(Fall());
    }

    private IEnumerator Fall()
    {
        float fallTime = 1 / maxJumpTime * 0.8f;

        Vector2 fallDestiny = new Vector2(transform.position.x, originalSpawnPoint.y + jumpLength * (currentFloor - 1));


        while (Vector2.Distance(transform.position, fallDestiny) > 0.01f)
        {
            yield return new WaitForFixedUpdate();
            transform.position = Vector2.Lerp(transform.position, fallDestiny, fallTime * Time.fixedDeltaTime);
        }
        ChangeFloor(-1);

        isFalling = false;
        transform.position = fallDestiny;
        StunByFalling();
    }

    #endregion

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

            Vector2 jumpDestiny = new Vector2(transform.position.x, originalSpawnPoint.y + jumpLength * (currentFloor + 1));

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

    #region Damage Methods

    public void Damage()
    {
        if (isJumping)
        {
            return;
        }

        gotDamaged = true;
        //Iniciar animación de estar herido

        StunByDamagedByHazard();
    }


    #endregion

    #region Stun Methods

    private void StartStunByHittingCeiling()
    {

        //Animación de que se golpea

        ChangeFloor(0);
        StunByHittingCeiling();
    }

    private void StunByHittingCeiling()
    {
        transform.position = new Vector2(transform.position.x, (transform.position.y - jumpLength) + mySpriteRenderer.bounds.extents.y);

        currentStunTime += ceilingStunTime;

        if (!isStunned)
        {
            StartCoroutine(Stun());
        }
    }

    private void StunByDamagedByHazard()
    {
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

        Debug.Log("STUNNED");
        while (currentStunTime > 0)
        {
            if (currentStunTime > maxStunTime)
            {
                currentStunTime = maxStunTime;
            }
            yield return new WaitForFixedUpdate();
            currentStunTime -= Time.fixedDeltaTime;
        }
        //Animación de que está stunneado
        EndStun();
    }

    private void EndStun()
    {
        currentStunTime = 0;
        //De vuelta a Idle
        Debug.Log("No longer stunned");
        isStunned = false;
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
            Debug.Log("Win");
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
