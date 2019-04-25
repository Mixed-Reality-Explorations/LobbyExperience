using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordAudio : MonoBehaviour
{

    public AudioSource audioSource;
    private bool micInitialized;

    public float sensitivity;
    public bool blown;

    void Awake() {
        if (!audioSource) {
            audioSource = GetComponent<AudioSource>();
        } else
        {
            UAR.Logger.log(UAR.Logger.Type.Info, "You must attach an AudioSource.");
        }
    }

    public void Record() {

        Delete();

        if (Microphone.devices.Length > 0) {
            audioSource.clip = Microphone.Start(Microphone.devices[0], false, 5, 44100);
        }

    }

    public void Play() {
        if (audioSource.clip) {
            audioSource.Play();
        } else
        {
            UAR.Logger.log(UAR.Logger.Type.Info, "There is no audio to play.");
        }
    }

    public void Delete()
    {
        audioSource.Stop();
        Destroy(audioSource.clip);
    }
}
