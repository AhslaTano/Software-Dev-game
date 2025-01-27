using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour{

    public AudioSource audioStart;
    public AudioSource audioOptions;
    public AudioSource audioQuit;

    public GameObject optionsMenu;
    public GameObject mainMenu;

    public void PlayGame(){

        audioStart.Play();

        StartCoroutine(StartGame());
    }
    public void Options(){

        audioOptions.Play();

        StartCoroutine(ToggleMenus());
    }
    public void QuitGame(){

        audioQuit.Play();

        StartCoroutine(QuitButton());
    }
    private IEnumerator StartGame(){

        // Optional: Add a small delay if needed for the sound effect to play before toggling
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        Debug.Log("Switching to Scene: In Game");
    }
    private IEnumerator ToggleMenus(){

        // Optional: Add a small delay if needed for the sound effect to play before toggling
        yield return new WaitForSeconds(0.5f);

        // Toggle menu visibility
        if(optionsMenu != null && mainMenu != null){

            optionsMenu.SetActive(true);
            mainMenu.SetActive(false);

            Debug.Log("Switching to Options Menu and disabling Main Menu.");
        }
    }
    private IEnumerator QuitButton(){

        // Optional: Add a small delay if needed for the sound effect to play before toggling
        yield return new WaitForSeconds(0.5f);

        Debug.Log("QUIR!");

        Application.Quit();
    }
}
