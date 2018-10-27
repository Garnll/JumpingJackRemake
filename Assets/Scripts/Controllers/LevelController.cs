using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectManager))]
public class LevelController : MonoBehaviour {

    [SerializeField]
    int myLevel = 0;
    [SerializeField] [Range(0, 1)]
    float offsetPercentage = 0.2f;
    [SerializeField]
    int maxFloors = 8;
    [SerializeField]
    GameObject[] masks;

    ObjectManager myObjectManager;

    float offset;
    float horizontalOffset;
    float playAreaHeight;
    float levelHeight;
    float floorHeight;
    float playerObjectSize;
    float levelWidth;

    public static float leftLevelBorder { get; set; }
    public static float rightLevelBorder { get; set; }

    Camera cam;

    void Start () {
        cam = Camera.main;

        if (!myObjectManager)
        {
            myObjectManager = GetComponent<ObjectManager>();
            GameController.Instance.objectManager = myObjectManager;
        }
        //Posiciones en Y

        playAreaHeight = cam.pixelHeight;
        offset = playAreaHeight * offsetPercentage; 

        levelHeight = playAreaHeight - (2 * offset); //cuadro el area de juego para que cubra ligeramente menos que el total de la pantalla
        floorHeight = levelHeight / maxFloors; 

        playerObjectSize = floorHeight*0.75f/100; //El sprite del jugador es cuadrado

        //Posiciones en X

        horizontalOffset = cam.pixelWidth * offsetPercentage * 0.5f;
        levelWidth = cam.pixelWidth - horizontalOffset;



        if (myObjectManager.FloorQuantityInPool < maxFloors + 1) //se incluye el piso 0
        {
            Debug.LogError("Not enough Floors on Pool");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Debug.Break();
#endif
            return;
        }

        SpawnFloors();
        SetBorders();
        SpawnPlayer();
        SpawnHoles();
        SpawnEnemies();
        SpawnMasks();
    }

    /// <summary>
    /// Make the given number of floors appear in the scene.
    /// </summary>
    private void SpawnFloors()
    {
        for (int i = 0; i <= maxFloors; i++)
        {
            myObjectManager.SpawnFloor(i, cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth * 0.5f, (floorHeight * i) + offset)), levelWidth);
        }
    }

    /// <summary>
    /// Sets the static values of the play area borders.
    /// </summary>
    private void SetBorders()
    {
        leftLevelBorder = myObjectManager.CheckLeftLevelBorder();
        rightLevelBorder = myObjectManager.CheckRightLevelBorder();
    }

    /// <summary>
    /// Sets the spawn point of the player.
    /// </summary>
    private void SpawnPlayer()
    {
        myObjectManager.SpawnPlayerObject(
            playerObjectSize,
            cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth * 0.5f, offset)),
            floorHeight);
    }

    /// <summary>
    /// Makes the object manager spawn the first holes;
    /// </summary>
    private void SpawnHoles()
    {
        myObjectManager.SpawnFirstHoles(maxFloors);
    }

    private void SpawnEnemies()
    {
        myObjectManager.SpawnEnemyPawns(
            playerObjectSize,
            floorHeight,
            myLevel - 1);
    }

    private void SpawnMasks()
    {
        Vector2 positionLeft = new Vector2(
             (leftLevelBorder + cam.ScreenToWorldPoint(new Vector2(0, 0)).x) / 2,
             0);

        float size = masks[0].GetComponent<Renderer>().bounds.size.x;
        float newSize = leftLevelBorder - cam.ScreenToWorldPoint(new Vector2(0, 0)).x;

        Vector3 rescale = masks[0].transform.localScale;
        rescale.x = newSize * rescale.x / size;

        masks[0].transform.localScale = rescale;
        masks[0].transform.position = positionLeft;

        Vector2 positionRight = new Vector2(
            (rightLevelBorder + cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth, 0)).x) / 2,
            0);

        masks[1].transform.localScale = rescale;
        masks[1].transform.position = positionRight;
    }
}
