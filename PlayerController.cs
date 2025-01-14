using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour{

    public GameObject InGameMenuCanvas;
    public GameObject InGameMenuPanel;

    private bool isMenuActive = false;

    private void Start(){
        
        // Ensure the in-game menu is initially inactive
        if(InGameMenuCanvas != null){

            InGameMenuCanvas.SetActive(false);
        }else{

            Debug.LogError("In-Game Menu Canvas is not assigned in the inspector.");
        }
        if (InGameMenuPanel != null){

            InGameMenuPanel.SetActive(false);
        }else{

            Debug.LogError("In-Game Menu Panel is not assigned in the inspector.");
        }
    }
    private void Update(){

        // Check if the "Tab" key is pressed
        if (Input.GetKeyDown(KeyCode.Tab)){

            ToggleInGameMenu();
        }
    }
    private void ToggleInGameMenu(){

        isMenuActive = !isMenuActive;

        if (isMenuActive){

            if(InGameMenuCanvas != null){

                InGameMenuCanvas.SetActive(true);
            }
            if (InGameMenuPanel != null){

                InGameMenuPanel.SetActive(true);
            }
            Debug.Log("In-Game Menu activated.");
        }else{

            if (InGameMenuCanvas != null){

                InGameMenuCanvas.SetActive(false);
            }
            if (InGameMenuPanel != null){

                InGameMenuPanel.SetActive(false);
            }
            Debug.Log("In-Game Menu activated.");
        }
    }
}
