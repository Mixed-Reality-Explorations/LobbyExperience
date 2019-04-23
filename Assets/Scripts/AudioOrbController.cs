using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class AudioOrbController : MonoBehaviour
{

    public GameObject prefabToGenerate;
    private GameObject audioOrb;
    private string anchorId;
    private bool isOpen;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowAudioOrb()
    {
        if(!isOpen)
        {
            Vector3 orbPosition = Camera.main.transform.position + (2.0f * Camera.main.transform.forward);
            audioOrb = Instantiate(prefabToGenerate, orbPosition, Quaternion.identity);
            anchorId = audioOrb.GetComponent<UnityARUserAnchorComponent>().AnchorId;
            isOpen = true;
        } else
        {
            UnityARSessionNativeInterface.GetARSessionNativeInterface().RemoveUserAnchor(anchorId);
            Destroy(audioOrb);
            isOpen = false;
        }

    }
}
