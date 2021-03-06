﻿using UnityEngine;

public class ObjectManager : MonoBehaviour {

    [SerializeField]
    Floor[] floors;
    [SerializeField]
    PlayerPawn playerPawn;
    [SerializeField]
    int maxHolesInScene = 8;

    [Space(10)]

    [SerializeField]
    int pointsForPassing = 5;
    [SerializeField]
    HolePawn[] holePawnsInPool;
    [SerializeField]
    HolePawn[] holeGhostsPool;

    [Space(10)]

    [SerializeField]
    EnemyPawn[] enemyPawnsInPool;
    [SerializeField]
    EnemyPawn[] enemyGhostsPool;

    HolePawn[] holePawnsInScene;
    int holePoolPosition = 0;


    EnemyPawn[] enemyPawnsInScene;

    float floorSpriteSize = -1;
    int maxVisibleFloors = 8;

    public int MaxVisibleFloors
    {
        get
        {
            return maxVisibleFloors;
        }
    }

    public int FloorQuantityInPool
    {
        get
        {
            return floors.Length;
        }
    }

    public float FloorHeight
    {
        get
        {
            return floors[floors.Length - 1].transform.position.y - floors[floors.Length - 2].transform.position.y;
        }
    }

    private void Awake()
    {
        if (holePawnsInPool.Length == 0 || holeGhostsPool.Length == 0)
        {
            Debug.LogError("An Hole Pool it's empty");
        }

        foreach (HolePawn hole in holePawnsInPool)
        {
            hole.gameObject.SetActive(false);
        }
        foreach (HolePawn hole in holeGhostsPool)
        {
            hole.gameObject.SetActive(false);
        }

        foreach (EnemyPawn enemy in enemyPawnsInPool)
        {
            enemy.gameObject.SetActive(false);
        }

        foreach (EnemyPawn enemy in enemyGhostsPool)
        {
            enemy.gameObject.SetActive(false);
        }

        PlayerPawn.OnPassThruHole += PlayerPassedHole;
    }

    private void OnDestroy()
    {
        PlayerPawn.OnPassThruHole -= PlayerPassedHole;
    }

    #region Floor Methods

    /// <summary>
    /// Gives the position of the specified floor.
    /// </summary>
    /// <param name="floorNumber"></param>
    /// <returns></returns>
    public float FloorPosition(int floorNumber)
    {
        return floors[floorNumber].transform.position.y;
    }

    /// <summary>
    /// Translates the floors to a given Position, sets their width and makes them appear, if not active.
    /// </summary>
    /// <param name="floorNumber"></param>
    /// <param name="floorPosition"></param>
    /// <param name="width"></param>
    public void SpawnFloor(int floorNumber, Vector2 floorPosition, float width)
    {
        if (!floors[floorNumber].gameObject.activeInHierarchy)
        {
            floors[floorNumber].gameObject.SetActive(true);
        }

        SpriteRenderer floorSprite = floors[floorNumber].GetComponent<SpriteRenderer>();

        float size = floorSprite.bounds.size.x;


        Vector3 rescale = floors[floorNumber].transform.localScale;
        rescale.x = (width * rescale.x) / size;

        floors[floorNumber].transform.localScale = rescale;

        if (floorSpriteSize < 0)
        {
            floorSpriteSize = floorSprite.size.y; //se va a usar en calculos posteriores
        }

        floors[floorNumber].transform.position = new Vector2(floorPosition.x, floorPosition.y);

        if (floorNumber == 0)
        {
            floors[floorNumber].gameObject.SetActive(false);
        }
    }

    public float CheckLeftLevelBorder()
    {
        SpriteRenderer floorSprite = floors[0].GetComponent<SpriteRenderer>();
        return floorSprite.bounds.center.x - floorSprite.bounds.extents.x;
    }

    public float CheckRightLevelBorder()
    {
        SpriteRenderer floorSprite = floors[0].GetComponent<SpriteRenderer>();
        return floorSprite.bounds.center.x + floorSprite.bounds.extents.x;
    }

    #endregion

    #region Player Methods

    public void SpawnPlayerObject(Vector2 position)
    {

        playerPawn.SetStartPosition(new Vector2(position.x, position.y + floorSpriteSize * 0.5f));
        //se le suma a la posición en y la mitad del tamaño del sprite del piso para que el personaje aparente estar "sobre" el piso.

        float newSize = Vector2.Distance(
            new Vector2 (0, floors[1].transform.position.y), 
            new Vector2 (0, floors[0].transform.position.y)) * 0.75f;

        float size = playerPawn.MySpriteRenderer.bounds.size.x;

        Vector3 rescale = playerPawn.transform.localScale;
        rescale.x = (newSize * rescale.x) / size;
        rescale.y = rescale.x;

        playerPawn.transform.localScale = rescale;

        playerPawn.gameObject.SetActive(true);
    }

    public float GetPlayerScale()
    {
        return playerPawn.transform.localScale.x;
    }

    #endregion

    #region Hole Methods

    /// <summary>
    /// Sets the first 2 holes to appear in the scene.
    /// </summary>
    public void SpawnFirstHoles(int maxFloors)
    {
        holePoolPosition = 0;
        holePawnsInScene = new HolePawn[maxHolesInScene];
        maxVisibleFloors = maxFloors;

        Vector3 spawnPosition = SpawnHolePosition();

        for (int i = 0; i < 2; i++)
        {
            SpawnNewHole(spawnPosition);
        }
    }

    /// <summary>
    /// Returns a random position from where to spawn a hole.
    /// </summary>
    /// <returns></returns>
    public Vector3 SpawnHolePosition()
    {
        Vector3 newPosition = new Vector2();

        newPosition.x = Random.Range(LevelController.leftLevelBorder + floors[0].MySpriteRenderer.bounds.size.x * 0.1f, 
            LevelController.rightLevelBorder - floors[0].MySpriteRenderer.bounds.size.x * 0.1f);

        int floorNumber = Random.Range(1, maxVisibleFloors + 1);
        newPosition.y = floors[floorNumber].transform.position.y;

        if (holePoolPosition >= 2)
        {
            CheckHolePositionAvailability(newPosition, floorNumber);
        }

        newPosition.z = floorNumber;

        return newPosition;
    }

    /// <summary>
    /// Checks wheter the position to spawn a new hole is available.
    /// </summary>
    private void CheckHolePositionAvailability(Vector3 newPosition, int floorNumber)
    {
        for (int i = 0; i < holePawnsInScene.Length; i++)
        {
            if (holePawnsInScene[i] != null)
            {
                if (holePawnsInScene[i].gameObject.activeInHierarchy)
                {
                    if (newPosition.x >= holePawnsInScene[i].transform.position.x - holePawnsInScene[i].MySpriteRenderer.bounds.extents.x &&
                        newPosition.x <= holePawnsInScene[i].transform.position.x + holePawnsInScene[i].MySpriteRenderer.bounds.extents.x &&
                        newPosition.y == holePawnsInScene[i].transform.position.y)
                    {
                        if (floorNumber == 1)
                        {
                            floorNumber = Random.Range(2, floorNumber);
                        }
                        else if (floorNumber == maxVisibleFloors + 1)
                        {
                            floorNumber = Random.Range(1, floorNumber);
                        }
                        else
                        {
                            int select = Random.Range(0, 1);
                            if (select == 0)
                            {
                                floorNumber = Random.Range(1, floorNumber);
                            }
                            else
                            {
                                floorNumber = Random.Range(floorNumber, maxVisibleFloors + 1);
                            }
                        }

                        newPosition.y = floors[floorNumber].transform.position.y;
                    }
                }

                if (holePawnsInScene[i].MyGhost.gameObject.activeInHierarchy)
                {
                    if (newPosition.x >= holePawnsInScene[i].MyGhost.transform.position.x - holePawnsInScene[i].MyGhost.MySpriteRenderer.bounds.extents.x &&
                        newPosition.x <= holePawnsInScene[i].MyGhost.transform.position.x + holePawnsInScene[i].MyGhost.MySpriteRenderer.bounds.extents.x &&
                        newPosition.y == holePawnsInScene[i].MyGhost.transform.position.y)
                    {
                        if (floorNumber == 1)
                        {
                            floorNumber = Random.Range(2, floorNumber);
                        }
                        else if (floorNumber == maxVisibleFloors + 1)
                        {
                            floorNumber = Random.Range(1, floorNumber);
                        }
                        else
                        {
                            int select = Random.Range(0, 1);
                            if (select == 0)
                            {
                                floorNumber = Random.Range(1, floorNumber);
                            }
                            else
                            {
                                floorNumber = Random.Range(floorNumber, maxVisibleFloors + 1);
                            }
                        }

                        newPosition.y = floors[floorNumber].transform.position.y;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Calls an hole to spawn, and gives points to the player.
    /// </summary>
    private void PlayerPassedHole()
    {
        SpawnNewHole(SpawnHolePosition());
        GivePointsToPlayer();
    }

    /// <summary>
    /// Gives points to the player (GameController, to be precise) for passing a hole. 
    /// </summary>
    private void GivePointsToPlayer()
    {
        GameController.Instance.ChangeScore(pointsForPassing * GameController.Instance.CurrentLevel);
    }

    /// <summary>
    /// Spawns a Hole in a given position.
    /// </summary>
    /// <param name="position"></param>
    private void SpawnNewHole(Vector3 position)
    {
        if (holePoolPosition >= maxHolesInScene)
        {
            return;
        }

        holePawnsInScene[holePoolPosition] = holePawnsInPool[holePoolPosition];
        holePawnsInPool[holePoolPosition] = null;

        holePawnsInScene[holePoolPosition].gameObject.SetActive(true);
        holePawnsInScene[holePoolPosition].GiveSpawningFloor(position, (int)position.z);
        holePawnsInScene[holePoolPosition].GiveSize(floors[0].MySpriteRenderer.bounds.size);

        GiveNewHoleDirection(holePawnsInScene[holePoolPosition]);
        GiveNewHoleAGhost(holePawnsInScene[holePoolPosition]);

        holePoolPosition++;
    }

    private void GiveNewHoleDirection(HolePawn spawningHole)
    {
        if (holePoolPosition >= (6-1))
        {
            spawningHole.GiveDirection(Vector2.left);
        }
        else if (holePoolPosition >= (3-1))
        {
            spawningHole.GiveDirection(Vector2.right);
        }
        else if (holePoolPosition == (2-1))
        {
            spawningHole.GiveDirection(Vector2.left);
        }
        else
        {
            spawningHole.GiveDirection(Vector2.right);
        }
    }

    /// <summary>
    /// Gives a ghosts to a hole pawn, and gives this hole pawn as a ghost to the ghost.
    /// </summary>
    /// <param name="spawningHole"></param>
    private void GiveNewHoleAGhost(HolePawn spawningHole)
    {
        spawningHole.AddGhost(holeGhostsPool[holePoolPosition]);
        holeGhostsPool[holePoolPosition].AddGhost(spawningHole);

        holeGhostsPool[holePoolPosition].GiveSize(floors[0].MySpriteRenderer.bounds.size);
    }

    #endregion

    #region Enemy Methods

    public Vector3 SetEnemyPosition()
    {
        Vector3 newPosition = new Vector2();

        newPosition.x = Random.Range(LevelController.leftLevelBorder + floors[0].MySpriteRenderer.bounds.size.x * 0.1f, 
            LevelController.rightLevelBorder - floors[0].MySpriteRenderer.bounds.size.x * 0.1f);

        int floorNumber = Random.Range(1, maxVisibleFloors);
        
        newPosition.y = floors[floorNumber].transform.position.y + floors[0].MySpriteRenderer.bounds.extents.y;

        if (enemyPawnsInScene != null)
        {
            CheckEnemyPositionAvailability(newPosition, floorNumber);
        }

        newPosition.z = floorNumber;

        return newPosition;
    }

    /// <summary>
    /// Checks if the position to spawn a new enemy is available.
    /// </summary>
    private void CheckEnemyPositionAvailability(Vector3 newPosition, int floorNumber)
    {
        for (int i = 0; i < enemyPawnsInScene.Length; i++)
        {
            if (enemyPawnsInScene[i] != null)
            {
                if (enemyPawnsInScene[i].gameObject.activeInHierarchy)
                {
                    while (newPosition.x >= enemyPawnsInScene[i].transform.position.x - enemyPawnsInScene[i].MySpriteRenderer.bounds.extents.x &&
                        newPosition.x <= enemyPawnsInScene[i].transform.position.x + enemyPawnsInScene[i].MySpriteRenderer.bounds.extents.x &&
                        newPosition.y == enemyPawnsInScene[i].transform.position.y)
                    {
                        newPosition.x = newPosition.x + enemyPawnsInScene[i].MySpriteRenderer.bounds.size.x * 2f;
                    }
                }

                if (enemyPawnsInScene[i].MyGhost.gameObject.activeInHierarchy)
                {
                    while (newPosition.x >= enemyPawnsInScene[i].MyGhost.transform.position.x - enemyPawnsInScene[i].MyGhost.MySpriteRenderer.bounds.extents.x &&
                        newPosition.x <= enemyPawnsInScene[i].MyGhost.transform.position.x + enemyPawnsInScene[i].MyGhost.MySpriteRenderer.bounds.extents.x &&
                        newPosition.y == enemyPawnsInScene[i].MyGhost.transform.position.y)
                    {
                        newPosition.x = newPosition.x + enemyPawnsInScene[i].MySpriteRenderer.bounds.size.x * 2f;
                    }
                }
            }
        }
    }

    public void SpawnEnemyPawns(float size,  float floorHeight, int enemyCount)
    {
        enemyPawnsInScene = new EnemyPawn[enemyCount];

        for (int i = 0; i < enemyPawnsInScene.Length; i++)
        {
            enemyPawnsInScene[i] = enemyPawnsInPool[i];
            enemyPawnsInPool[i] = null;

            enemyPawnsInScene[i].transform.localScale = new Vector2(size, size);

            enemyPawnsInScene[i].gameObject.SetActive(true);
            GiveEnemyGhost(enemyPawnsInScene[i], i);

            enemyPawnsInScene[i].floorOffset = floors[0].MySpriteRenderer.bounds.extents.y;
            enemyPawnsInScene[i].SetStartPosition(SetEnemyPosition());

            enemyPawnsInScene[i].StartMoving();
        }
    }

    private void GiveEnemyGhost(EnemyPawn enemy, int position)
    {
        enemy.AddGhost(enemyGhostsPool[position]);
        enemyGhostsPool[position].AddGhost(enemy);

        enemy.MyGhost.transform.localScale = enemy.transform.localScale;
    }

    #endregion
}
