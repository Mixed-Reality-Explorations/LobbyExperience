using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordAudio : MonoBehaviour
{
    
    public AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        foreach (var device in Microphone.devices) {
            Debug.Log("====== Name: " + device);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Record() {
        AudioSource audioSource = GetComponent<AudioSource>();
        Debug.Log("Start Recording");
        audioSource.clip = Microphone.Start("iPhone audio input", false, 5, 44100);
        Debug.Log("Done Recording");
    }

    public void Play() {
        audioSource.Play();
    }
}
