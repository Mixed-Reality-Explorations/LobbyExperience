using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class AudioOrb : MonoBehaviour
{

    private Transform cameraTransform;

    void Awake() {
        Debug.Log("STE ~~~~~~~~~ Audio Orb Position Set");
        cameraTransform = Camera.main.transform;
        transform.position = cameraTransform.position + cameraTransform.forward * 3f;
        transform.rotation = cameraTransform.rotation;
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void ToggleAudioOrb() {
        if (gameObject.activeSelf) {
            gameObject.SetActive(false);

        } else {
            gameObject.SetActive(true);

        }
    }
}
