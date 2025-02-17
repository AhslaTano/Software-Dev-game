using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTile : MonoBehaviour
{
    private float speed = 5.0f;
    public bool free;
    public bool busy;
    //public Collider2D[] colliders;
    public Collider2D activeCollider, otherCollider;
    public Vector2 target;
    public bool targetAcquired;

    // Start is called before the first frame update
    void Start()
    {
        activeCollider = null;
        otherCollider = null;
        busy = false;
        targetAcquired = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(free && activeCollider != null && otherCollider != null)
        {
            float step = speed * Time.deltaTime;

            // move sprite towards the target location
            transform.position = Vector2.MoveTowards(activeCollider.transform.position, target, step) + new Vector2(transform.position.x - activeCollider.transform.position.x, transform.position.y - activeCollider.transform.position.y);
            if(new Vector2(activeCollider.transform.position.x, activeCollider.transform.position.y) == target)
            //if(transform.position + new Vector3(activeCollider.offset.x, activeCollider.offset.y, transform.position.z) == new Vector3(target.x, target.y, transform.position.z))
            {
                Debug.Log("I ARRIVED!");
                free = false;
                activeCollider = null;
                otherCollider = null;
            }
        }

    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.otherCollider.gameObject.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Dynamic && !busy)
        {
            if (free)
            {
                busy = true;
                Debug.Log(gameObject.name + " I am free and colliding");
                target = collision.transform.position;
                targetAcquired = true;
                otherCollider = collision.collider;
                collision.otherCollider.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                activeCollider = collision.otherCollider;
                collision.collider.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                busy = false;
            }
            /*
            else
            {
                busy = true;
                Debug.Log(gameObject.name + " I am stationary and colliding");
                // setup the activeCollider on the other tile
                TTile otherTile = collision.transform.parent.gameObject.GetComponent<TTile>();
                otherTile.activeCollider = collision.collider;
                collision.otherCollider.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                busy = false;
            }*/
        }
    }
}
