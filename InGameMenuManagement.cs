using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameMenuManagement : MonoBehaviour{

    public AudioSource audio_backButton;
    public AudioSource audio_quitButton;

    public GameObject GameMenuPanel;

    public void BackButton_Toggle(){

        audio_backButton.Play();

        StartCoroutine(BackButton());
    }
    public void QuitButton_Toggle(){

        audio_quitButton.Play();

        StartCoroutine(QuitButton());
    }
    private IEnumerator BackButton(){

        // Optional: Add a small delay if needed for the sound effect to play before toggling
        yield return new WaitForSeconds(0.5f);

        // Toggle menu visibility (In-Game menu & In-Game)
        GameMenuPanel.SetActive(false);
    }
    private IEnumerator QuitButton(){

        // Optional: Add a small delay if needed for the sound effect to play before toggling
        yield return new WaitForSeconds(0.5f);

        Debug.Log("Quit!");

        Application.Quit();
    }
}
