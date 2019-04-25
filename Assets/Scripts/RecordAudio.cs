using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordAudio : MonoBehaviour
{

    public AudioSource audioSource;
    public DBController databaseCtrl;

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

        databaseCtrl = GameObject.FindGameObjectWithTag("Database").GetComponent<DBController>();

    }

    public void Record() {
        Delete();

        if (Microphone.devices.Length > 0) {
            UAR.Logger.log(UAR.Logger.Type.Info, "Recording...");
            audioSource.clip = Microphone.Start(Microphone.devices[0], false, 5, 44100);
        }

    }

    public void Play() {
        UAR.Logger.log(UAR.Logger.Type.Info, "Playing...");
        if (audioSource.clip) {
            audioSource.Play();
        } else
        {
            UAR.Logger.log(UAR.Logger.Type.Info, "There is no audio to play.");
        }
    }

    public void Delete()
    {
        UAR.Logger.log(UAR.Logger.Type.Info, "Deleting...");
        audioSource.Stop();
        Destroy(audioSource.clip);
    }

    public void Send()
    {
        UAR.Logger.log(UAR.Logger.Type.Info, "Sending...");
        databaseCtrl.upload(audioSource, transform.localPosition, transform.localRotation);
    }
}
