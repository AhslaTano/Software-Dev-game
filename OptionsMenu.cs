using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour{

    public AudioMixer audioMixer;

    public AudioSource audio_BackButton;

    public GameObject optionsMenu;
    public GameObject mainMenu;

    public Button BackButton;

    public void SetVolume(float volume){
        audioMixer.SetFloat("volume", Mathf.Log10(volume) * 20);
    }
    public void SetSoundVolume(float volume){
        audioMixer.SetFloat("soundVolume", Mathf.Log10(volume) * 20);
    }
    public void BackButton_Toggle(){

        audio_BackButton.Play();

        StartCoroutine(BackButtonCoroutine());
    }
    private IEnumerator BackButtonCoroutine(){

        yield return new WaitForSeconds(0.5f);

        if(audio_BackButton != null){

            optionsMenu.SetActive(false);
            mainMenu.SetActive(true);
        }
        Debug.Log("Returning to the main menu. Options menu deactivated.");
    }
}
