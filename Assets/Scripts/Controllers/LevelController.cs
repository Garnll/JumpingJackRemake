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
    int maxFloors = 9;

    ObjectManager myObjectManager;

    float offset;
    float playAreaHeight;
    float levelHeight;
    float floorHeight;
    float playerObjectSize;
    float levelWidth;

    public static float leftLevelBorder { get; private set; }
    public static float rightLevelBorder { get; private set; }

    Camera cam;

    void OnGUI()
    {
        Vector3 point = new Vector3();
        Event currentEvent = Event.current;
        Vector2 mousePos = new Vector2();

        // Get the mouse position from Event.
        // Note that the y position from Event is inverted.
        mousePos.x = currentEvent.mousePosition.x;
        mousePos.y = cam.pixelHeight - currentEvent.mousePosition.y;

        point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));

        GUILayout.BeginArea(new Rect(20, 20, 250, 120));
        GUILayout.Label("Screen pixels: " + cam.pixelWidth + ":" + cam.pixelHeight);
        GUILayout.Label("Mouse position: " + mousePos);
        GUILayout.Label("World position: " + point.ToString("F3"));
        GUILayout.EndArea();
    }

    void Awake () {
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

        levelWidth = cam.pixelWidth - offset; //Half the offset from Y



        if (myObjectManager.FloorQuantityInPool < maxFloors + 1) //Momentaneamente incluiré el piso 0
        {
            Debug.LogError("Not enough Floors on Pool");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Debug.Break();
#endif
            return;
        }

        SetBorders();
        SpawnFloors();
        SpawnPlayer();
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
        Debug.Log(levelWidth);
        leftLevelBorder = cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth - levelWidth + offset * 0.5f, 0)).x;
        rightLevelBorder = cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth - offset * 1.5f, 0)).x;
    }

    /// <summary>
    /// 
    /// </summary>
    private void SpawnPlayer()
    {
        myObjectManager.SpawnPlayerObject(
            playerObjectSize,
            cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth * 0.5f, offset)),
            floorHeight);
    }
}
