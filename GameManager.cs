using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour{

    private int playerOneScore = 0;
    private int playerTwoScore = 0;

    [SerializeField] private Text playerOneScoreText;
    [SerializeField] private Text playerTwoScoreText;

    // Reference for player turn
    private int currentPlayer = 1;

    // Singleton instance (global access to GameManager if desired)
    public static GameManager Instance;

    private void Awake(){
        
        if(Instance == null){

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }else{

            Destroy(gameObject);
        }
    }
    void Start(){

        UpdateScoreUI();
    }
    // Adds points to a player's score (both players)
    public void UpdateScoreUI(){

        if(playerOneScoreText != null){

            playerOneScoreText.text = "Player 1: " + playerOneScore.ToString();
        }
        if(playerTwoScoreText != null){

            playerTwoScoreText.text = "Player 2: " + playerTwoScore.ToString();
        }
    }
    public void SwitchTurn(){

        currentPlayer = (currentPlayer == 1) ? 2 : 1;
    }
    // Getter for the current player's turn
    public int GetCurrentPlayer(){

        return currentPlayer;
    }
    // Resets scores (optional for replay purposes)
    public void ResetScore(){

        playerOneScore = 0;
        playerTwoScore = 0;

        UpdateScoreUI();
    }
}
