using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordAudio : MonoBehaviour
{
    private bool micInitialized;
    private AudioSource audioSource;

    public float sensitivity;
    public bool blown;

    void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    public void Record() {
        Delete();

        if (Microphone.devices.Length > 0) {
            UAR.Logger.log(UAR.Logger.Type.Info, "Recording");
            audioSource.clip = Microphone.Start(Microphone.devices[0], false, 5, 44100);
        }

    }

    public void Play() {

        UAR.Logger.log(UAR.Logger.Type.Info, "Playing");
        if (audioSource.clip) {
            audioSource.Play();
        } else
        {
            UAR.Logger.log(UAR.Logger.Type.Info, "There is no audio to play.");
        }
    }

    public void Delete()
    {
        UAR.Logger.log(UAR.Logger.Type.Info, "Deleting");
        audioSource.Stop();
        Destroy(audioSource.clip);
    }

    public void Send()
    {
        UAR.Logger.log(UAR.Logger.Type.Info, "Sending");
        AppController.Instance.uploadPrayer(audioSource, transform.localPosition, Quaternion.identity);
    }
}
