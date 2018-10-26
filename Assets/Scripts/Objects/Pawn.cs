using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public abstract class Pawn : MonoBehaviour {

    [SerializeField]
    protected float velocity;

    protected SpriteRenderer mySpriteRenderer;
    protected int currentFloor;
    protected Vector2 myDirection;

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
        CheckEndOfScreen();
    }

    protected abstract void CheckEndOfScreen();
    protected abstract void ChangeFloor(int floor);
}
