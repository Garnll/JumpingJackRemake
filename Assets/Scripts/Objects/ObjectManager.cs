using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour {

    [SerializeField]
    Floor[] floors;
    [SerializeField]
    PlayerPawn playerPawn;

    HolePawn[] HolePawnsInPool;
    HolePawn[] HolePawnsInScene;
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
}
