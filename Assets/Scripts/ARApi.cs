using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARApi : MonoBehaviour
{

    public GameObject IOSPrefab;
    // public GameObject AndroidPrefab;
    private GameObject ARSession;

    void Start() {

        #if UNITY_EDITOR
        Debug.Log("Unity Editor");
        #endif
        
        #if UNITY_IOS
        Debug.Log("Iphone");
        ARSession = Instantiate(IOSPrefab);
        #endif  

        #if UNITY_ANDROID
        Debug.Log("Android");
        // ARSession = Instantiate(AndroidPrefab);
        #endif  	

    }

}
