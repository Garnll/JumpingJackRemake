using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour {

    [SerializeField]
    Floor[] floors;
    [SerializeField]
    PlayerPawn playerPawn;
    [SerializeField]
    int maxHolesInScene = 8;
    [SerializeField]
    HolePawn[] holePawnsInPool;
    [SerializeField]
    HolePawn[] holeGhostsPool;

    HolePawn[] holePawnsInScene;
    int holePoolPosition = 0;

    EnemyObject[] enemyObjectsInPool;
    EnemyObject[] eEnemyObjectsInScene;

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

        PlayerPawn.OnPassThruHole += SpawnNewHole;
    }


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

        floorSprite.size = new Vector2(width / 100, floorSprite.size.y);

        if (floorSpriteSize < 0)
        {
            floorSpriteSize = floorSprite.size.y; //se va a usar en calculos posteriores
        }

        floors[floorNumber].transform.position = new Vector2(floorPosition.x, floorPosition.y);
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


    public void SpawnPlayerObject(float size, Vector2 position, float floorHeight)
    {
        playerPawn.transform.position = new Vector2(position.x, position.y + floorSpriteSize * 0.5f);
        //se le suma a la posición en y la mitad del tamaño del sprite del piso para que el personaje aparente estar "sobre" el piso.

        playerPawn.MySpriteRenderer.size = new Vector2(size, size);
    }

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

        newPosition.x = Random.Range(LevelController.leftLevelBorder + floorSpriteSize * 0.1f, LevelController.rightLevelBorder - floorSpriteSize * 0.1f);

        int floorNumber = Random.Range(1, maxVisibleFloors + 1);
        newPosition.y = floors[floorNumber].transform.position.y;
        newPosition.z = floorNumber;

        return newPosition;
    }

    /// <summary>
    /// Spawns a Hole in a random position.
    /// </summary>
    private void SpawnNewHole()
    {
        SpawnNewHole(SpawnHolePosition());
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
        //Se hacen estos if para recrear la manera en que se creaban los huecos en el juego original
    }

    /// <summary>
    /// Gives a ghosts to a hole pawn, and gives this hole pawn as a ghost to the ghost.
    /// </summary>
    /// <param name="spawningHole"></param>
    private void GiveNewHoleAGhost(HolePawn spawningHole)
    {
        spawningHole.AddGhost(holeGhostsPool[holePoolPosition]);
        holeGhostsPool[holePoolPosition].AddGhost(spawningHole);
    }
}
