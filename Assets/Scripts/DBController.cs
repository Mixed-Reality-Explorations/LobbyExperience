using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Database;
using Firebase.Storage;
using Firebase.Unity.Editor;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System;


public class Prayer
{
    public static string dbUrl = "https://reflections-51bdd.firebaseio.com/";
    private static FirebaseDatabase db;
    private static FirebaseStorage storage;

    public string id;

    public Vector3 position;

    public Quaternion orientation;

    public string category;

    public float[] audio;

    private PrayerSerializable prayerData;

    public static bool _DBInitialized = false;
    public static bool DBInitialized {
        get { return _DBInitialized; }
        private set { _DBInitialized = value; }
    }

    public Prayer(string category, Vector3 position, Quaternion orientation)
    {
        this.category = category;
        this.position = position;
        this.orientation = orientation;
        prayerData = new PrayerSerializable(position, orientation);

    }

    public Prayer(string id, string category, Vector3 position, Quaternion orientation) : this(category, position, orientation)
    {
        this.id = id;
    }


    [Serializable]
    public class PrayerSerializable
    {
        public Vector3 position;
        public Quaternion orientation;

        public PrayerSerializable(Vector3 position, Quaternion orientation)
        {
            this.position = position;
            this.orientation = orientation;
        }
    }

    public void attachAudio()
    {

    }

    public static void InitDB()
    {
        if (!DBInitialized) {
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(dbUrl);
            db = FirebaseDatabase.DefaultInstance;
            storage = FirebaseStorage.DefaultInstance;
            DBInitialized = true;
        }
    }

    public static void fetch(string category, Action<List<Prayer>, AggregateException> callback)
    {
        db.GetReference(category).GetValueAsync().ContinueWith(task => {
            if (task.Exception != null)
            {
                Debug.LogFormat("[ERROR]: {0}", task.Exception);
                callback(null, task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;
            var dbPrayers = snapshot.Children;
            var prayers = new List<Prayer>();

            try
            {
                foreach (DataSnapshot prayer in dbPrayers)
                {
                    var id = (string)prayer.Key;

                    var xyz = prayer.Child("position");
                    float x, y, z, w;

                    float.TryParse(xyz.Child("x").Value.ToString(), out x);
                    float.TryParse(xyz.Child("y").Value.ToString(), out y);
                    float.TryParse(xyz.Child("z").Value.ToString(), out z);
                    var position = new Vector3(x, y, z);

                    var xyzw = prayer.Child("orientation");
                    float.TryParse(xyzw.Child("x").Value.ToString(), out x);
                    float.TryParse(xyzw.Child("y").Value.ToString(), out y);
                    float.TryParse(xyzw.Child("z").Value.ToString(), out z);
                    float.TryParse(xyzw.Child("w").Value.ToString(), out w);
                    var orientation = new Quaternion(x, y, z, w);

                    prayers.Add(new Prayer(id, category, position, orientation));
                }

            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            callback(prayers, null);
        });
    }

    public void fetchAudio(Action<float[], AggregateException> callback)
    {
        byte[] buffer;
        const long bufferSize = 34008512;

        storage.GetReference(id).GetBytesAsync(bufferSize).ContinueWith(task => {
            if (task.Exception != null)
            {
                Debug.LogFormat("[ERROR]: {0}", task.Exception);
                callback(null, task.Exception);
                return;
            }

            buffer = task.Result;
            audio = new float[buffer.Length / sizeof(float)];
            System.Buffer.BlockCopy(buffer, 0, audio, 0, buffer.Length);
            callback(audio, null);
        });
    }

    public void upload(Action<AggregateException> callback)
    {
        var jsonstr = JsonUtility.ToJson(this.prayerData);
        id = db.RootReference.Child(category).Push().Key;

        db.RootReference.Child(category).Child(id).SetRawJsonValueAsync(jsonstr).ContinueWith(task=> {
            if (task.Exception != null)
            {
                Debug.LogFormat("[ERROR]: {0}", task.Exception);
                callback(task.Exception);
                return;
            }

            uploadAudio(id, callback);
        });
    }

    void uploadAudio(string id, Action<AggregateException> callback)
    {
        int bufferSize = audio.Length * sizeof(float);
        byte[] buffer = new byte[bufferSize];
        System.Buffer.BlockCopy(audio, 0, buffer, 0, bufferSize);

        storage.GetReferenceFromUrl("gs://reflections-51bdd.appspot.com").Child(id).PutBytesAsync(buffer).ContinueWith(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogFormat("[ERROR]: {0}", task.Exception);
                callback(task.Exception);
                return;
            }

            callback(null);
        });
    }
}

public class DBController : MonoBehaviour
{

    public AudioSource audioSource;
    public AudioClip clip;
    public Prayer p;

    private FirebaseDatabase db;
    private StorageReference storage;
    
    private float[] audioData;
    protected ConcurrentQueue<Action> mainQueue = new ConcurrentQueue<Action>();

    void Start()
    {
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

    public void upload(AudioSource recAudio)
    {

        var p = new Prayer("categoryOne", Vector3.zero, Quaternion.identity);
        p.audio = new float[recAudio.clip.samples * sizeof(float)];
        recAudio.clip.GetData(p.audio, 0);

        p.upload(e =>
        {
            Debug.LogFormat("exception? {0}", e);
        });

    }

    public void download()
    {
        Prayer.fetch("categoryOne", (prayers, e) =>
        {
            Debug.LogFormat("prayer exception? {0}", e);

            if (e == null)
            {
                p = prayers[0];
                Debug.LogFormat("{0}", p);

                p.fetchAudio((a, e2) =>
                {
                    Debug.LogFormat("audio exception? {0}", e2);

                    if (e2 == null)
                    {
                        mainQueue.Enqueue(()=>{
                            clip = AudioClip.Create(p.id, p.audio.Length, 1, 44100, false);
                            clip.SetData(p.audio, 0);

                            Debug.Log("About to play audio:");
                            AudioSource.PlayClipAtPoint(clip, Vector3.zero);
                        });                        
                    }
                });
            }
        });
    }

    public void record(AudioSource audio)
    {
        audioSource = audio;
        audioSource.clip = Microphone.Start("iPhone audio input", false, 5, 44100);

    }

}
