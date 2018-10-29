using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPawn : Pawn {

    EnemyPawn myGhost;
    bool isExitingFloor;
    bool isOutOfBounds;
    public float floorOffset { get; set; }

    public EnemyPawn MyGhost
    {
        get
        {
            return myGhost;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isOutOfBounds)
        {
            return;
        }

        Attack(collision.GetComponent<PlayerPawn>());
    }

    private void Attack(PlayerPawn player)
    {
        //Inicia animación de ataque?

        player.Damage();
    }

    public void SetStartPosition(Vector3 position)
    {
        transform.position = (Vector2)position;
        currentFloor = (int)position.z;
    }

    public void StartMoving()
    {
        myDirection = Vector2.left;
        StopCoroutine(UpdateMovement());
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


    public void AddGhost(EnemyPawn newGhost)
    {
        myGhost = newGhost;
    }

    protected override void CheckEndOfScreen()
    {
        if (transform.position.x <= LevelController.leftLevelBorder + (mySpriteRenderer.bounds.extents.x*1.25f) && myDirection == Vector2.left)
        {
            if (!isExitingFloor)
            {
                isExitingFloor = true;
                ChangeFloor(currentFloor + 1);
                ActivateGhost();
            }

            if (transform.position.x <= LevelController.leftLevelBorder - mySpriteRenderer.bounds.extents.x && isExitingFloor)
            {
                DeactivateMyself();
            }
        }
    }

    private void ActivateGhost()
    {
        myGhost.transform.position = new Vector2(LevelController.rightLevelBorder + mySpriteRenderer.bounds.extents.x,
            GameController.Instance.objectManager.FloorPosition(currentFloor) + floorOffset);

        myGhost.currentFloor = currentFloor;
        myGhost.floorOffset = floorOffset;

        myGhost.gameObject.SetActive(true);

        myGhost.StartMoving();
    }

    private void DeactivateMyself()
    {
        isExitingFloor = false;
        StopCoroutine(UpdateMovement());
        gameObject.SetActive(false);
    }

    protected override void ChangeFloor(int floor)
    {
        if (floor == GameController.Instance.objectManager.MaxVisibleFloors)
        {
            DissapearOnLastFloor();
        }

        if (floor > GameController.Instance.objectManager.MaxVisibleFloors)
        {
            currentFloor = 1;
        }
        else if (floor < 1)
        {
            currentFloor = GameController.Instance.objectManager.MaxVisibleFloors;
        }
        else
        {
            currentFloor = floor;
        }

        if (currentFloor < GameController.Instance.objectManager.MaxVisibleFloors)
        {
            AppearIfNotOnLastFloor();
        }
    }

    void DissapearOnLastFloor()
    {
        myGhost.mySpriteRenderer.enabled = false;
        myGhost.isOutOfBounds = true;
    }

    void AppearIfNotOnLastFloor()
    {
        myGhost.mySpriteRenderer.enabled = true;
        myGhost.isOutOfBounds = false;
    }

}
