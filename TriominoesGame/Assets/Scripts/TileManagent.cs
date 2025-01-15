using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Tiles : MonoBehaviour
{
    public static Tiles Instance;
    public class Triomino
    {
        public string Top { get; set; }
        public string Left { get; set; }
        public string Right { get; set; }
        public GameObject TileObject { get; set; } // Reference to the Unity GameObject
    }
    private List<Triomino> tiles = new List<Triomino>(); // list of all 56 triominos
    private List<Triomino> player1_hand = new List<Triomino>(); // player 1 hand
    private List<Triomino> player2_hand = new List<Triomino>(); // computer/player 2 hand
    private List<Triomino> board = new List<Triomino>(); // placed tiles on the board
    private int maxDrawAttempts = 3; // maximum draws allowed from the "well"
    private Triomino selectedTile; // Currently selected tile
    private bool isDragging = false; // Moving Selected tile
    public Camera mainCamera; // Reference to the main camera for raycasting
    public Transform boardTransform; // Reference to the board's transform for snapping tiles
    internal bool IsDragging; // Bool check for dragging tile
    [Header("Flicker Effect")]
    [SerializeField] private Color validColor = Color.white; // Color to flicker to valid color
    [SerializeField] private Color invalidColor = Color.white; // Color to flicker to invalid color
    [SerializeField] private float flickerDuration = 0.1f; // Duration of each flicker
    [SerializeField] private int flickerCount = 3; // Number of flickers
    private MeshRenderer meshRenderer; // Reference to the enemy's mesh renderer
    private Material material; // Material of the MeshRenderer
    private Color originalColor; // Store the original color of the material

    public object SelectedTile { get; internal set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        // Get the MeshRenderer and its material
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            material = meshRenderer.material;
            originalColor = material.color; // Store the original color
        }
        else
        {
            Debug.LogWarning("MeshRenderer not found on " + gameObject.name);
        }
    }

    void Start()
    {
        InitializeTiles();

        DealHand(player1_hand);
        DealHand(player2_hand);

        Debug.Log($"Player 1 Hand: {HandToString(player1_hand)}");
        Debug.Log($"Player 2 Hand: {HandToString(player2_hand)}");
    }

    private IEnumerator ValidFlickerEffect()
    {
        for (int i = 0; i < flickerCount; i++)
        {
            // Change to damage color
            material.color = validColor;
            yield return new WaitForSeconds(flickerDuration);

            // Reset to original color
            material.color = originalColor;
            yield return new WaitForSeconds(flickerDuration);
        }
    }

    private IEnumerator InvalidFlickerEffect()
    {
        for (int i = 0; i < flickerCount; i++)
        {
            // Change to damage color
            material.color = invalidColor;
            yield return new WaitForSeconds(flickerDuration);

            // Reset to original color
            material.color = originalColor;
            yield return new WaitForSeconds(flickerDuration);
        }
    }

    public void DealHand(List<Triomino> playerHand)
    {
        for (int i = 0; i < 8; i++) // 8 for a full hand
        {
            if (tiles.Count == 0)
            {
                Debug.LogWarning("No more tiles to deal!");
                break;
            }

            int randomIndex = UnityEngine.Random.Range(0, tiles.Count);
            playerHand.Add(tiles[randomIndex]);
            tiles.RemoveAt(randomIndex);
        }
    }

    public string HandToString(List<Triomino> hand)
    {
        string result = "";
        foreach (var tile in hand)
        {
            result += $"[{tile.Left}, {tile.Top}, {tile.Right}] ";
        }
        return result;
    }
    public void SelectTile()
    {
        // Perform a raycast to detect the tile under the mouse pointer
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Check if the hit object is a tile
            Triomino tile = hit.collider.GetComponent<Triomino>();
            if (tile != null)
            {
                selectedTile = tile;
                isDragging = true;
            }
        }
    }

    public void DragTile()
    {
        // Update the tile's position to follow the mouse
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = mainCamera.WorldToScreenPoint(selectedTile.TileObject.transform.position).z; // Maintain z-depth
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        selectedTile.TileObject.transform.position = new Vector3(worldPosition.x, worldPosition.y, selectedTile.TileObject.transform.position.z);
    }

    public void RotateTile()
    {
        // Rotate the tile 120 degrees clockwise around its Z-axis
        selectedTile.TileObject.transform.Rotate(0, 0, 120);
    }

    public void PlaceTile()
    {
        isDragging = false;

        if (selectedTile == null) return;

        if (IsValidMove(selectedTile))
        {
            ValidFlickerEffect();
            SnapToBoard(selectedTile);
            board.Add(selectedTile);
            player1_hand.Remove(selectedTile); // Adjust for player2_hand if necessary
        }
        else
        {
            InvalidFlickerEffect();
            ReturnTileToHand(selectedTile);
        }

        selectedTile = null;
    }

    public void SnapToBoard(Triomino tile)
    {
        // Snap the tile's position to a grid or designated spot on the board
        Vector3 boardPosition = boardTransform.position; // Example: Snap to board center
        tile.TileObject.transform.position = new Vector3(Mathf.Round(boardPosition.x), Mathf.Round(boardPosition.y), boardPosition.z);
    }

    public void ReturnTileToHand(Triomino tile)
    {
        // Reset the tile's position to the player's hand area
        // Example: Reset to a predefined hand position
        tile.TileObject.transform.position = new Vector3(0, 0, 0); // Placeholder
    }

    public void DrawFromWell(List<Triomino> playerHand)
    {
        int drawCount = 0;

        while (drawCount < maxDrawAttempts)
        {
            if (tiles.Count == 0)
            {
                Debug.LogWarning("The well is empty! No tiles to draw.");
                break;
            }

            // Draw a tile from the well
            int randomIndex = UnityEngine.Random.Range(0, tiles.Count);
            Triomino drawnTile = tiles[randomIndex];
            tiles.RemoveAt(randomIndex);
            playerHand.Add(drawnTile);
            drawCount++;

            Debug.Log($"Drew tile: [{drawnTile.Left}, {drawnTile.Top}, {drawnTile.Right}]");

            // Check if the new tile creates a valid move
            if (IsValidMove(drawnTile))
            {
                Debug.Log("Valid move found after drawing.");
                return; // End the drawing process
            }
        }

        Debug.Log("No valid moves available after drawing. Ending turn.");
    }

    public bool IsValidMove(Triomino tile)
    {
        // Example logic: Check if the tile matches any adjacent tiles on the board
        // You can add more complex validation rules based on your game's rules
        if (board.Count == 0)
        {
            // First move: Any tile is valid
            return true;
        }

        foreach (Triomino placedTile in board)
        {
            // Simplified check for a match
            if (tile.Left == placedTile.Right || tile.Top == placedTile.Top || tile.Right == placedTile.Left)
            {
                return true;
            }
        }

        return false;
    }

    public void InitializeTiles()
    {
        // Triples
        tiles[56] = new Triomino { Left = "5", Top = "5", Right = "5" };
        tiles[55] = new Triomino { Left = "4", Top = "4", Right = "4" };
        tiles[54] = new Triomino { Left = "3", Top = "3", Right = "3" };
        tiles[53] = new Triomino { Left = "2", Top = "2", Right = "2" };
        tiles[52] = new Triomino { Left = "1", Top = "1", Right = "1" };
        tiles[51] = new Triomino { Left = "0", Top = "0", Right = "0" };
        // Doubles
        tiles[1] = new Triomino { Left = "4", Top = "5", Right = "5" };
        tiles[2] = new Triomino { Left = "3", Top = "5", Right = "5" };
        tiles[3] = new Triomino { Left = "2", Top = "5", Right = "5" };
        tiles[4] = new Triomino { Left = "1", Top = "5", Right = "5" };
        tiles[5] = new Triomino { Left = "0", Top = "5", Right = "5" };
        tiles[6] = new Triomino { Left = "4", Top = "4", Right = "5" };
        tiles[7] = new Triomino { Left = "3", Top = "4", Right = "4" };
        tiles[8] = new Triomino { Left = "2", Top = "4", Right = "4" };
        tiles[9] = new Triomino { Left = "1", Top = "4", Right = "4" };
        tiles[10] = new Triomino { Left = "0", Top = "4", Right = "4" };
        tiles[11] = new Triomino { Left = "3", Top = "3", Right = "5" };
        tiles[12] = new Triomino { Left = "3", Top = "3", Right = "4" };
        tiles[13] = new Triomino { Left = "2", Top = "3", Right = "3" };
        tiles[14] = new Triomino { Left = "1", Top = "3", Right = "3" };
        tiles[15] = new Triomino { Left = "0", Top = "3", Right = "3" };
        tiles[16] = new Triomino { Left = "2", Top = "2", Right = "5" };
        tiles[17] = new Triomino { Left = "2", Top = "2", Right = "4" };
        tiles[18] = new Triomino { Left = "2", Top = "2", Right = "3" };
        tiles[19] = new Triomino { Left = "1", Top = "2", Right = "2" };
        tiles[20] = new Triomino { Left = "0", Top = "2", Right = "2" };
        tiles[21] = new Triomino { Left = "1", Top = "1", Right = "5" };
        tiles[22] = new Triomino { Left = "1", Top = "1", Right = "4" };
        tiles[23] = new Triomino { Left = "1", Top = "1", Right = "3" };
        tiles[24] = new Triomino { Left = "1", Top = "1", Right = "2" };
        tiles[25] = new Triomino { Left = "0", Top = "1", Right = "1" };
        tiles[26] = new Triomino { Left = "0", Top = "0", Right = "5" };
        tiles[27] = new Triomino { Left = "0", Top = "0", Right = "4" };
        tiles[28] = new Triomino { Left = "0", Top = "0", Right = "3" };
        tiles[29] = new Triomino { Left = "0", Top = "0", Right = "2" };
        tiles[30] = new Triomino { Left = "0", Top = "0", Right = "1" };
        // Singles
        tiles[31] = new Triomino { Left = "3", Top = "4", Right = "5" };
        tiles[32] = new Triomino { Left = "2", Top = "4", Right = "5" };
        tiles[33] = new Triomino { Left = "1", Top = "4", Right = "5" };
        tiles[34] = new Triomino { Left = "0", Top = "4", Right = "5" };
        tiles[35] = new Triomino { Left = "2", Top = "3", Right = "5" };
        tiles[36] = new Triomino { Left = "1", Top = "3", Right = "5" };
        tiles[37] = new Triomino { Left = "0", Top = "3", Right = "5" };
        tiles[38] = new Triomino { Left = "2", Top = "3", Right = "4" };
        tiles[39] = new Triomino { Left = "1", Top = "3", Right = "4" };
        tiles[40] = new Triomino { Left = "0", Top = "3", Right = "4" };
        tiles[41] = new Triomino { Left = "1", Top = "2", Right = "5" };
        tiles[42] = new Triomino { Left = "0", Top = "2", Right = "5" };
        tiles[43] = new Triomino { Left = "0", Top = "1", Right = "5" };
        tiles[44] = new Triomino { Left = "1", Top = "2", Right = "4" };
        tiles[45] = new Triomino { Left = "0", Top = "2", Right = "4" };
        tiles[46] = new Triomino { Left = "0", Top = "1", Right = "4" };
        tiles[47] = new Triomino { Left = "1", Top = "2", Right = "3" };
        tiles[48] = new Triomino { Left = "0", Top = "2", Right = "3" };
        tiles[49] = new Triomino { Left = "0", Top = "1", Right = "3" };
        tiles[50] = new Triomino { Left = "0", Top = "1", Right = "2" };
    }
}

