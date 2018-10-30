using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Floor : MonoBehaviour {

    SpriteRenderer mySpriteRenderer;

    public SpriteRenderer MySpriteRenderer
    {
        get
        {
            return mySpriteRenderer;
        }
    }

	void Awake () {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
	}
	
}
