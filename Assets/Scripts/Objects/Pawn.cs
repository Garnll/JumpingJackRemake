using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public abstract class Pawn : MonoBehaviour {

    [SerializeField]
    float velocity;

    SpriteRenderer mySpriteRenderer;
    float currentFloor;
    Vector2 myDirection;

    public SpriteRenderer MySpriteRenderer
    {
        get
        {
            if (mySpriteRenderer == null)
            {
                mySpriteRenderer = GetComponent<SpriteRenderer>();
            }
            return mySpriteRenderer;
        }
    }

    protected void Awake()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected void Move(Vector2 direction)
    {
        transform.Translate(direction * Time.deltaTime * velocity);
    }

    protected abstract void CheckEndOfScreen();
    protected abstract void ChangeFloor();
}
