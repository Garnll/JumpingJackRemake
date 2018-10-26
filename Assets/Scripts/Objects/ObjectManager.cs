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
    HolePawn[] HolePawnsInPool;

    HolePawn[] HolePawnsInScene;
    int holePoolPosition = 0;

    EnemyObject[] EnemyObjectsInPool;
    EnemyObject[] EnemyObjectsInScene;

    float floorSpriteSize = -1;

    public int FloorQuantityInPool
    {
        get
        {
            return floors.Length;
        }
    }

    private void Awake()
    {
        foreach (HolePawn hole in HolePawnsInPool)
        {
            hole.gameObject.SetActive(false);
        }
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


    public void SpawnPlayerObject(float size, Vector2 position, float floorHeight)
    {
        playerPawn.transform.position = new Vector2(position.x, position.y + floorSpriteSize * 0.5f);
        //se le suma a la posición en y la mitad del tamaño del sprite del piso para que el personaje aparente estar "sobre" el piso.

        playerPawn.MySpriteRenderer.size = new Vector2(size, size);
    }

    /// <summary>
    /// Sets the first 2 holes to appear in the scene.
    /// </summary>
    public void SpawnFirstHoles()
    {
        holePoolPosition = 0;
        HolePawnsInScene = new HolePawn[maxHolesInScene];

        Vector2 spawnPosition = SpawnHolePosition();

        for (int i = 0; i < 2; i++)
        {
            SpawnNewHole(spawnPosition);
        }
    }

    public Vector2 SpawnHolePosition()
    {
        Vector2 newPosition = new Vector2();

        newPosition.x = Random.Range(LevelController.leftLevelBorder + floorSpriteSize * 0.1f, LevelController.rightLevelBorder - floorSpriteSize * 0.1f);
        newPosition.y = floors[Random.Range(1, 9)].transform.position.y;

        return newPosition;
    }

    public void SpawnNewHole(Vector2 position)
    {
        if (holePoolPosition >= maxHolesInScene)
        {
            return;
        }

        HolePawnsInScene[holePoolPosition] = HolePawnsInPool[holePoolPosition];
        HolePawnsInPool[holePoolPosition] = null;

        HolePawnsInScene[holePoolPosition].gameObject.SetActive(true);
        HolePawnsInScene[holePoolPosition].transform.position = position;

        GiveNewHoleDirection(HolePawnsInScene[holePoolPosition]);

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
}
