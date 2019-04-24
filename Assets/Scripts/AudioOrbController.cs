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

            //Vector3 orbPosition = orbT.InverseTransformPoint(transform.position);
            //Quaternion orbQuaternion = Quaternion.identity * orbT.TransformDirection(transform.rotation * transform.forward);

            GameObject go = new GameObject();
            Transform orbT = go.transform;
            orbT.position = Camera.main.transform.position + (0.5f * Camera.main.transform.forward);
            orbT.LookAt(Camera.main.transform, Vector3.up);
            audioOrb = Instantiate(orbPrefab, orbT.position, orbT.rotation, transform);
            isOpen = true;

        } else
        {
            Destroy(audioOrb);
            isOpen = false;
        }

    }

}
