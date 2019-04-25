using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UAR;

public class AppController : MonoBehaviour
{

    public bool originDetected;
    public GameObject mainButton;
    public GameObject testPrefab;
    private GameObject testGO;

    // Start is called before the first frame update
    void Start()
    {

        UAR.UAR.IAnchorAdded += ImageAnchorAdded;
        UAR.UAR.IAnchorUpdated += ImageAnchorUpdated;

    }
    
    // Update is called once per frame
    void Update()
    {
        Debug.Log("Stop Me Here");
    }

    void ImageAnchorAdded(IAnchor anchor)
    {
        if (!originDetected && anchor.imgName == "StartImg")
        {
            AppStarted(anchor);
        }
    }

    void ImageAnchorUpdated(IAnchor anchor)
    {
        testGO.transform.SetPositionAndRotation(anchor.pose.position, anchor.pose.rotation);

    }

    void AppStarted(IAnchor anchor)
    {
        originDetected = true;
        transform.SetPositionAndRotation(anchor.pose.position, anchor.pose.rotation);
        testGO = Instantiate(testPrefab, Vector3.zero , Quaternion.identity, transform);
        mainButton.SetActive(true);

    }
}
