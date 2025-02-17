using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Triomino : MonoBehaviour
{
    public TriominoObject data;
    private SpriteRenderer sprite;
    public TMP_Text topText;
    public TMP_Text leftText;
    public TMP_Text rightText;
    private Vector3 offset; // To track the mouse offset while dragging
    public bool isDragging;
    public bool inDeck;

    //Collision
    private float speed = 10.0f;
    public bool free;// not collided
    public Collider2D[] colliders; // Three colliders, one for each face
    public Collider2D activeCollider;
    public Collider2D otherCollider;
    public List<Collider2D> otherColliders = new List<Collider2D>(); // Supports multiple colliders during snapping
    public Vector2 target;
    public bool busy;
    public bool targetAcquired;
    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        topText.text = data.top.ToString();
        leftText.text = data.left.ToString();
        rightText.text = data.right.ToString();
        isDragging = false;
        activeCollider = null;
        otherCollider = null;
        busy = false;
        targetAcquired = false;
        inDeck = true;
    }
    private void Update()
    {
        /*if (isDragging)
        {
            OnMouseDown();
            OnMouseDrag();

            if (Input.GetKeyDown(KeyCode.J)) // J key for 120 degrees left
            {
                RotateTriomino(120); // Rotate left
            }
            else if (Input.GetKeyDown(KeyCode.L)) // L key for 120 degrees right
            {
                RotateTriomino(-120); // Rotate right
            }
        }

        if (Input.GetMouseButtonDown(1) && isDragging)
        {
            transform.Rotate(0, 0, 180);
            //rotate tile 180
        }
        if (free && activeCollider != null && otherColliders.Count > 0)
        {
            float step = speed * Time.deltaTime;

            // Move sprite towards the target location
            Vector2 combinedOffset = Vector2.zero;
            foreach (Collider2D col in otherColliders)
            {
                combinedOffset += col.offset;
            }
            combinedOffset /= otherColliders.Count; // Average the offset if multiple colliders

            transform.position = Vector2.MoveTowards(
                (Vector2)transform.position + activeCollider.offset,
                target,
                step
            ) + combinedOffset;

            if (Vector2.Distance((Vector2)transform.position + activeCollider.offset, target) < 0.01f)
            {
                Debug.Log("I arrived");
                free = false;
                activeCollider = null;
                otherColliders.Clear();
            }
        }*/
        if (!inDeck && free && activeCollider != null && otherCollider != null)
        {
            float step = speed * Time.deltaTime;

            // move sprite towards the target location
            transform.position = Vector2.MoveTowards(activeCollider.transform.position, target, step) + new Vector2(transform.position.x - activeCollider.transform.position.x, transform.position.y - activeCollider.transform.position.y);
            if (new Vector2(activeCollider.transform.position.x, activeCollider.transform.position.y) == target)
            //if(transform.position + new Vector3(activeCollider.offset.x, activeCollider.offset.y, transform.position.z) == new Vector3(target.x, target.y, transform.position.z))
            {
                Debug.Log("I ARRIVED!");
                free = false;
                activeCollider = null;
                otherCollider = null;
            }
        }
    }
    
    // Function to rotate the Triomino by a specified amount (in degrees)
    private void RotateTriomino(float angle)
    {
        transform.Rotate(0, 0, angle);  // Rotate on the Z-axis by the specified angle
        if (angle > 0)
        {
            RotateLeft();
        }
        else
        {
            RotateRight();
        }
        Debug.Log("M= " + data.top + " L= " + data.left + " R= " + data.right);
        //updatePips(); // This doesn't need to be here. Update pips only updates the values when loading data.
    }

    private void RotateRight() // Updates the variable values if rotating clockwise.
    {
        // When rotating clockwise, the middle moves to the right, the right moves to the bottom, and the left moves to the middle
        Debug.Log("Rotating clockwise");
        int temp = data.top;
        data.top = data.left;
        data.left = data.right;
        data.right = temp;
    }

    private void RotateLeft() // Updates the variable values if rotating counterclockwise.
    {
        // When rotating counterclockwise, the middle moves to the left, the left moves to the bottom, and the right moves to the middle
        Debug.Log("Rotating counterclockwise");
        int temp = data.top;
        data.top = data.right;
        data.right = data.left;
        data.left = temp;
    }
    private void OnMouseDown()
    {
        // Capture the offset between the mouse and the tile's position
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        offset = transform.position - worldPosition;

        isDragging = true;
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            // Update the tile's position to follow the mouse, maintaining the offset
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            transform.position = worldPosition + offset;
        }
    }

    private void OnMouseUp()
    {
        if (isDragging)
        {
            // Snap the tile to the nearest grid position or board
            Vector3 snappedPosition = new Vector3(
                Mathf.Round(transform.position.x),
                Mathf.Round(transform.position.y),
                transform.position.z);

            transform.position = snappedPosition;
            isDragging = false;
            inDeck = false;

            Debug.Log($"Tile placed at: {transform.position}");
        }
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        /*if (IsValidMove(selectedTile))
        {
            ValidFlickerEffect();
        }
        else
        {
            InvalidFlickerEffect();
            ReturnTileToHand(selectedTile);
        }*/
        if (collision.otherCollider.gameObject.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Dynamic && !busy && !inDeck)
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

    public bool CompareSides(Collider2D thisCollider, Collider2D otherCollider)
    {
        // Determine which sides are being compared based on the colliders
        int thisSideIndex = GetSideIndexForCollider(thisCollider);
        int otherSideIndex = GetSideIndexForCollider(otherCollider);

        // Get the pip pairs for both sides
        var thisSidePips = GetSidePips(thisSideIndex);
        var otherSidePips = otherCollider.GetComponent<Triomino>().GetSidePips(otherSideIndex); // Access the Triomino on the other piece

        // Compare the two sides' pip pairs.
        return (thisSidePips.Item1 == otherSidePips.Item1 && thisSidePips.Item2 == otherSidePips.Item2);
    }

    // Helper function to get the correct side based on the collider
    private int GetSideIndexForCollider(Collider2D collider)
    {
        // You can map each collider to a side (top, left, right)
        if (collider == colliders[0]) return 0; // Left side
        if (collider == colliders[1]) return 1; // Right side
        if (collider == colliders[2]) return 2; // Bottom/Middle side

        return -1; // Error value
    }

    // Get the pip pair for a given side index
    private (int, int) GetSidePips(int sideIndex)
    {
        // Return the pip pair for the specified side based on current rotation
        if (sideIndex == 0) return (data.top, data.left);   // Top-left side
        if (sideIndex == 1) return (data.top, data.right); // Top-right side
        if (sideIndex == 2) return (data.left, data.right);  // Right-left side

        return (-1, -1); // Default error value
    }
}
