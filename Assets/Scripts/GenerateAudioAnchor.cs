using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class GenerateAudioAnchor : MonoBehaviour
{

    [SerializeField]
    private ARReferenceImage referenceImage;

    [SerializeField]
    private GameObject prefabsToGenerate;

    public GameObject testPrefabToGenerate;

    private GameObject imageAnchorGO;

    private GameObject mainCanvas;

    // Use this for initialization
    void Start() {
        UnityARSessionNativeInterface.ARImageAnchorAddedEvent += AddImageAnchor;
        UnityARSessionNativeInterface.ARImageAnchorUpdatedEvent += UpdateImageAnchor;
        UnityARSessionNativeInterface.ARImageAnchorRemovedEvent += RemoveImageAnchor;

    }

    void AddImageAnchor(ARImageAnchor arImageAnchor) {

        Debug.LogFormat("image anchor added[{0}] : tracked => {1}", arImageAnchor.identifier, arImageAnchor.isTracked);
        if (arImageAnchor.referenceImageName == referenceImage.imageName) {
            AddTestPrefab(arImageAnchor);
            AddUIButton();
            //AddAudioFireAnchors(arImageAnchor);
        }

    }

    void AddTestPrefab(ARImageAnchor arImageAnchor) {
        Vector3 position = UnityARMatrixOps.GetPosition(arImageAnchor.transform);
        Quaternion rotation = UnityARMatrixOps.GetRotation(arImageAnchor.transform);
        imageAnchorGO = Instantiate<GameObject>(testPrefabToGenerate, position, rotation);
        Debug.Log("TestPrefabAdded");

    }

    void AddUIButton() {
        mainCanvas = GameObject.FindWithTag("MainCanvas");
        Transform buttonTransform = mainCanvas.transform.Find("Button");
        buttonTransform.gameObject.SetActive(true);
        Debug.Log("AddedUIButton");
    }


    //void AddAudioFireAnchors(ARImageAnchor arImageAnchor) {

    //    Vector3 position = UnityARMatrixOps.GetPosition(arImageAnchor.transform);
    //    Quaternion rotation = UnityARMatrixOps.GetRotation(arImageAnchor.transform);

    //    // Will have to loop through DatabaseSnapshots and add one GO per snapshot
    //    imageAnchorGO = Instantiate<GameObject>(prefabsToGenerate, position, rotation);

    //}

    void UpdateImageAnchor(ARImageAnchor arImageAnchor) {
        Debug.LogFormat("image anchor updated[{0}] : tracked => {1}", arImageAnchor.identifier, arImageAnchor.isTracked);
        if (arImageAnchor.referenceImageName == referenceImage.imageName) {
            if (!imageAnchorGO.activeSelf) {
                imageAnchorGO.SetActive(true);
            }
            imageAnchorGO.transform.position = UnityARMatrixOps.GetPosition(arImageAnchor.transform);
            imageAnchorGO.transform.rotation = UnityARMatrixOps.GetRotation(arImageAnchor.transform);
        }

    }

    void RemoveImageAnchor(ARImageAnchor arImageAnchor) {
        Debug.LogFormat("image anchor removed[{0}] : tracked => {1}", arImageAnchor.identifier, arImageAnchor.isTracked);
        if (imageAnchorGO)
        {
            GameObject.Destroy(imageAnchorGO);
        }

    }

    void OnDestroy() {
        UnityARSessionNativeInterface.ARImageAnchorAddedEvent -= AddImageAnchor;
        UnityARSessionNativeInterface.ARImageAnchorUpdatedEvent -= UpdateImageAnchor;
        UnityARSessionNativeInterface.ARImageAnchorRemovedEvent -= RemoveImageAnchor;

    }

    //// Update is called once per frame
    //void Update() {

    //}
}
