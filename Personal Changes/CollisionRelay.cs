using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionRelay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(name + " - distance to center of parent -> " + Vector3.Distance(transform.position, transform.parent.position));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.collider.name + "<=>" + collision.otherCollider.name);
        Debug.Log(gameObject.name);
        Triomino dad = transform.parent.gameObject.GetComponent<Triomino>();
        dad.OnCollisionEnter2D(collision);
    }
}
