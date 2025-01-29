using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Triomino : MonoBehaviour
{
    public TriominoObject triominoData;  // Reference to the ScriptableObject

    private SpriteRenderer spriteRenderer;
    [SerializeField] private TextMeshPro middleText;
    [SerializeField] private TextMeshPro leftText;
    [SerializeField] private TextMeshPro rightText;
    private int middle, left, right;


    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        middle = triominoData.top;
        left = triominoData.left;
        right = triominoData.right;

        updatePips();
    }

    public void updatePips() // This exists only to set the visual pips to the ones it got from the triomino object.
    {
        middleText.text = middle.ToString();
        leftText.text = left.ToString();
        rightText.text = right.ToString();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) // J key for 120 degrees left
        {
            RotateTriomino(120); // Rotate left
        }
        else if (Input.GetKeyDown(KeyCode.L)) // L key for 120 degrees right
        {
            RotateTriomino(-120); // Rotate right
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
        Debug.Log("M= " + middle + " L= " + left + " R= " + right);
        //updatePips(); // This doesn't need to be here. Update pips only updates the values when loading data.
    }

    private void RotateRight() // Updates the variable values if rotating clockwise.
    {
        // When rotating clockwise, the middle moves to the right, the right moves to the bottom, and the left moves to the middle
        Debug.Log("Rotating clockwise");
        int temp = middle;
        middle = left;
        left = right;
        right = temp;
    }

    private void RotateLeft() // Updates the variable values if rotating counterclockwise.
    {
        // When rotating counterclockwise, the middle moves to the left, the left moves to the bottom, and the right moves to the middle
        Debug.Log("Rotating counterclockwise");
        int temp = middle;
        middle = right;
        right = left;
        left = temp;
    }
}


