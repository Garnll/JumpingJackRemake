using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolePawn : Pawn {

    [SerializeField]
    float expectedSizePercentage = 0.1f;

    HolePawn myGhost;
    bool isExitingFloor;

    private void Start()
    {
        isExitingFloor = false;
    }

    public void GiveSize(Vector2 floorSize)
    {
        Vector2 newSize = new Vector2(floorSize.x * expectedSizePercentage, floorSize.y);

        Vector2 size = GetComponent<SpriteRenderer>().bounds.size;

        Vector3 rescale = transform.localScale;
        rescale.x = (newSize.x * rescale.x) / size.x;
        rescale.y = (newSize.y * rescale.y) / size.y;

        transform.localScale = rescale;
    }

    public HolePawn MyGhost
    {
        get
        {
            return myGhost;
        }
    }

    public void GiveSpawningFloor(Vector2 position, int floorNumber)
    {
        transform.position = position;
        currentFloor = floorNumber;
    }

    public void GiveDirection(Vector2 direction)
    {
        myDirection = direction;

        StopCoroutine(UpdateMovement());
        StartCoroutine(UpdateMovement());
    }

    public void AddGhost(HolePawn newGhost)
    {
        myGhost = newGhost;
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
        if (transform.position.x <= LevelController.leftLevelBorder && myDirection == Vector2.left)
        {
            if (!isExitingFloor)
            {
                isExitingFloor = true;
                ChangeFloor(currentFloor + 1);
                ActivateGhostUp();
            }

            if (transform.position.x <= LevelController.leftLevelBorder - mySpriteRenderer.bounds.extents.x && isExitingFloor)
            {
                DeactivateMyself();
            }
        }

        if (transform.position.x >= LevelController.rightLevelBorder && myDirection == Vector2.right)
        {
            if (!isExitingFloor)
            {
                isExitingFloor = true;
                ChangeFloor(currentFloor - 1);
                ActivateGhostDown();
            }

            if (transform.position.x >= LevelController.rightLevelBorder + mySpriteRenderer.bounds.extents.x && isExitingFloor)
            {
                DeactivateMyself();
            }
        }
    }

    /// <summary>
    /// Activates a ghost that is one floor up.
    /// </summary>
    private void ActivateGhostUp()
    {
        myGhost.transform.position = new Vector2(LevelController.rightLevelBorder + mySpriteRenderer.bounds.extents.x, 
            GameController.Instance.objectManager.FloorPosition(currentFloor));
        ActivateGhost();
    }

    /// <summary>
    /// Activates a ghost that is one floor down.
    /// </summary>
    private void ActivateGhostDown()
    {
        myGhost.transform.position = new Vector2(LevelController.leftLevelBorder - mySpriteRenderer.bounds.extents.x, 
            GameController.Instance.objectManager.FloorPosition(currentFloor));
        ActivateGhost();
    }

    /// <summary>
    /// Sets active the ghost.
    /// </summary>
    private void ActivateGhost()
    {
        myGhost.currentFloor = currentFloor;

        myGhost.gameObject.SetActive(true);

        myGhost.GiveDirection(myDirection);
    }

    private void DeactivateMyself()
    {
        isExitingFloor = false;
        StopCoroutine(UpdateMovement());
        gameObject.SetActive(false);
    }

    protected override void ChangeFloor(int floor)
    {
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
    }
}
