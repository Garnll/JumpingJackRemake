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
        //Y Positions

        playAreaHeight = cam.pixelHeight;
        offset = playAreaHeight * offsetPercentage; 

        levelHeight = playAreaHeight - (2 * offset); //The play area is fixed  to it's slighty smaller than the window
        floorHeight = levelHeight / maxFloors; 

        //X Positions

        horizontalOffset = cam.pixelWidth * offsetPercentage;
        levelWidth = Vector2.Distance(cam.ScreenToWorldPoint(new Vector2(horizontalOffset, 0)), cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth, 0)));


        if (myObjectManager.FloorQuantityInPool < maxFloors + 1) //Floor 0 it's included
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
        SetUI();

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
    /// Tells the GameController to set up the UI
    /// </summary>
    private void SetUI()
    {
        GameController.Instance.SetUpUI(CheckLevelWidth(), levelHeight, floorHeight);
    }

    /// <summary>
    /// Checks the true measure in pixels of the level Width
    /// </summary>
    /// <returns></returns>
    private float CheckLevelWidth()
    {
        return cam.WorldToScreenPoint(new Vector2(rightLevelBorder, 0)).x - cam.WorldToScreenPoint(new Vector2(leftLevelBorder, 0)).x;
    }

    /// <summary>
    /// Sets the spawn point of the player.
    /// </summary>
    private void SpawnPlayer()
    {
        myObjectManager.SpawnPlayerObject(
            cam.ScreenToWorldPoint(new Vector2(cam.pixelWidth * 0.35f, offset)));

        playerObjectSize = myObjectManager.GetPlayerScale();
    }

    /// <summary>
    /// Makes the object manager spawn the first holes.
    /// </summary>
    private void SpawnHoles()
    {
        myObjectManager.SpawnFirstHoles(maxFloors);
    }

    /// <summary>
    /// Makes the object manager spawn enemies equal to level - 1.
    /// </summary>
    private void SpawnEnemies()
    {
        if (myLevel <= 0)
        {
            myLevel = 1;
        }

        myObjectManager.SpawnEnemyPawns(
            playerObjectSize,
            floorHeight,
            myLevel - 1);
    }

    /// <summary>
    /// Changes the size and position of the masks that go at either size of the play area.
    /// </summary>
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
