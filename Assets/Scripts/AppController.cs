using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.UI;
using UAR;

public class AppController : MonoBehaviour
{
    public static ConcurrentQueue<Action> mainQueue = new ConcurrentQueue<Action>();
    public static AppController Instance;

    public bool originDetected;
    public Dictionary<string, GameObject> categoryWorlds = new Dictionary<string, GameObject>();
    public string[] categories;
    private GameObject tarotPrefab;

    public GameObject mainButton;
    public GameObject scanButton;
    public GameObject testPrefab;

    public GameObject prayerContainerPrefab;

    private GameObject testGO;

    [SerializeField]
    private string activeCategory;
    private bool inWorld = false;
    private bool isScanning = true;

    void Awake()
    {
        
        foreach (var c in categories)
        {
            GameObject cGO = new GameObject(c);
            cGO.SetActive(false);
            cGO.transform.parent = transform;
            categoryWorlds.Add(c, cGO);
        }

        Instance = this;
    }

    void Start()
    {

        UAR.UAR.IAnchorAdded += ImageAnchorAdded;
        UAR.UAR.IAnchorUpdated += ImageAnchorUpdated;
        Prayer.InitDB();
    }

    private void Update()
    {
        Action action;
        if (mainQueue.TryDequeue(out action))
        {
            action();
        }
    }

    void ImageAnchorAdded(IAnchor anchor)
    {
        UAR.Logger.log(UAR.Logger.Type.Info, "Image Category Added: {0}", anchor.imgName);

        if (anchor.imgName == "StartImg")
        {
            if (!originDetected)
            {
                AppStarted(anchor);
            }
        }
        else if (!inWorld)
        {
            if (anchor.imgName == "theStoryteller" || anchor.imgName == "animalWhisperer" || anchor.imgName == "theGiver" || anchor.imgName == "theSnowFox")
            {
                enterTarotWorld(anchor);
            } else
            {
                enterCategoryWorld(anchor.imgName);

            }
        }
    }

    void ImageAnchorUpdated(IAnchor anchor)
    {
        UAR.Logger.log(UAR.Logger.Type.Info, "Image Category Updated: {0}", anchor.imgName);

        if (anchor.imgName == "StartImg")
        {
            transform.SetPositionAndRotation(anchor.pose.position, anchor.pose.rotation);
        }
        else if (!inWorld)
        {
            enterCategoryWorld(anchor.imgName);
        }
    }

    void AppStarted(IAnchor anchor)
    {
        originDetected = true;
        transform.SetPositionAndRotation(anchor.pose.position, anchor.pose.rotation);
        testGO = Instantiate(testPrefab, anchor.pose.position, anchor.pose.rotation, transform);

        mainButton.SetActive(true);
        scanButton.SetActive(true);

    }

    public void enterTarotWorld(IAnchor anchor)
    {
        switch (anchor.imgName)
        {
            case "theStoryteller":
                var tarotGO = Instantiate(Resources.Load(anchor.imgName), anchor.pose.position, anchor.pose.rotation);
                break;
            //write other cases and test
        }
    }

    public void enterCategoryWorld(string category)
    {
        activeCategory = category;
        inWorld = true;
        UAR.Logger.log(UAR.Logger.Type.Info, "enter world: {0}", activeCategory);
        categoryWorlds[activeCategory].SetActive(true);

        Prayer.registerForUpdates(activeCategory, (prayers, e) =>
        {
            UAR.Logger.log(UAR.Logger.Type.Info, "prayer exception? {0}", e);

            if (e == null)
            {
                var p = prayers[0];
                Debug.LogFormat("{0}", p);

                mainQueue.Enqueue(() => {
                    var world = categoryWorlds[activeCategory].transform;
                    GameObject pc = Instantiate(prayerContainerPrefab, world);
                    pc.transform.localPosition = p.position;
                    pc.transform.localRotation = p.orientation;

                    pc.GetComponent<PrayerContainer>().prayer = p;
                    
                    //pc.transform.parent = categoryWorlds[activeCategory].transform;
                });
            }
        });
    }

    public void exitCategoryWorld()
    {
        UAR.Logger.log(UAR.Logger.Type.Info, "exiting current world");
        categoryWorlds[activeCategory].SetActive(false);
        activeCategory = null;
        inWorld = false;
    }

    public void uploadPrayer(AudioSource recAudio, Vector3 position, Quaternion rotation)
    {

        UAR.Logger.log(UAR.Logger.Type.Info, "uploading to category {0}", activeCategory);

        //var dummy = Instantiate(testPrefab, transform);
        //dummy.transform.localPosition = position;
        //dummy.transform.localRotation = rotation;

        var p = new Prayer(activeCategory, position, rotation);
        p.audio = new float[recAudio.clip.samples * sizeof(float)];
        recAudio.clip.GetData(p.audio, 0);
        p.upload(e =>
        {
            UAR.Logger.log(UAR.Logger.Type.Info, "uploaded. ID: {0}", p.id);
            UAR.Logger.log(UAR.Logger.Type.Info, "upload exception? {0}", e);
        });
    }

    //public void toggleScanButton()
    //{
    //    if (!isScanning)
    //    {
    //        isScanning = true;
    //        scanButton.GetComponent<Image>().color = new Color(0, 0, 0, 177);
    //        UAR.UAR.tracking = true;
    //        UAR.Logger.log(UAR.Logger.Type.Info, "NOT tracking");

    //    }
    //    else
    //    {
    //        isScanning = false;
    //        scanButton.GetComponent<Image>().color = new Color(0, 0, 0, 137);
    //        UAR.UAR.tracking = false;
    //        UAR.Logger.log(UAR.Logger.Type.Info, "IS tracking");

    //    }
    //}


}
