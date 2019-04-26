using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class AudioOrbController : MonoBehaviour
{

    public GameObject orbPrefab;
    private GameObject audioOrb;
    private string anchorId;
    private bool isOpen;

    public void ShowAudioOrb()
    {
        UAR.Logger.log(UAR.Logger.Type.Info, "Show Audio Orb: {0}", !isOpen);

        if (!isOpen)
        {

            GameObject go = new GameObject();
            Transform orbT = go.transform;
            orbT.position = Camera.main.transform.position + (0.5f * Camera.main.transform.forward);
            orbT.rotation = Quaternion.LookRotation(orbT.position - Camera.main.transform.position);


            audioOrb = Instantiate(orbPrefab, orbT.position, orbT.rotation, transform);
            isOpen = true;

        } else
        {
            Destroy(audioOrb);
            isOpen = false;
        }

    }

}
