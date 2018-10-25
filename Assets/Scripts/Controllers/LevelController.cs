using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectSpawner))]
public class LevelController : MonoBehaviour {

    [SerializeField]
    int myLevel = 0;
    [SerializeField] [Range(0, 1)]
    float offsetPercentage = 0.2f;
    [SerializeField]
    int maxFloors = 9;

    ObjectSpawner objectSpawner;

    float offset;
    float playAreaHeight;
    float levelHeight;
    float floorHeight;
    float playerObjectHeight;

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

        if (!objectSpawner)
        {
            objectSpawner = GetComponent<ObjectSpawner>();
        }

        playAreaHeight = cam.pixelHeight;
        offset = playAreaHeight * offsetPercentage;

        levelHeight = playAreaHeight - (2 * offset);
        floorHeight = levelHeight / maxFloors;

        playerObjectHeight = floorHeight*0.75f;

        Debug.Log("Posición piso 0: " + cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth * 0.5f, floorHeight * 0 + offset)));
        Debug.Log("Posición piso 1: " + cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth * 0.5f, floorHeight * 1 + offset)));
        Debug.Log("Posición piso 2: " + cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth * 0.5f, floorHeight * 2 + offset)));
        Debug.Log("Posición piso 3: " + cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth * 0.5f, floorHeight * 3 + offset)));
        Debug.Log("Posición piso 4: " + cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth * 0.5f, floorHeight * 4 + offset)));
        Debug.Log("Posición piso 5: " + cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth * 0.5f, floorHeight * 5 + offset)));
        Debug.Log("Posición piso 6: " + cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth * 0.5f, floorHeight * 6 + offset)));
        Debug.Log("Posición piso 7: " + cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth * 0.5f, floorHeight * 7 + offset)));
        Debug.Log("Posición piso 8: " + cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth * 0.5f, floorHeight * 8 + offset)));
    }
	
}
