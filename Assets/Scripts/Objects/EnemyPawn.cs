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
        if (transform.position.x <= LevelController.leftLevelBorder && myDirection == Vector2.left)
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
        myGhost.mySpriteRenderer.enabled = mySpriteRenderer.enabled;
        myGhost.isOutOfBounds = isOutOfBounds;

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
            mySpriteRenderer.enabled = false;
            isOutOfBounds = true;
        }

        if (floor > GameController.Instance.objectManager.MaxVisibleFloors)
        {
            currentFloor = 1;
            mySpriteRenderer.enabled = true;
            isOutOfBounds = false;
        }
        else if (floor < 1)
        {
            currentFloor = GameController.Instance.objectManager.MaxVisibleFloors;
        }
        else
        {
            currentFloor = floor;
        }
    }

}
