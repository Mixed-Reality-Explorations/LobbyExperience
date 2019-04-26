using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Database;
using Firebase.Storage;
using Firebase.Unity.Editor;
using System.Collections.Concurrent;
using System;


public class Prayer
{
    public static string dbUrl = "https://reflections-51bdd.firebaseio.com/";
    private static FirebaseDatabase db;
    private static FirebaseStorage storage;

    public static string currentCategory = "categoryOne"; //TODO: Remove this
    private static Dictionary<string, List<Prayer>> prayers = new Dictionary<string, List<Prayer>>();
    private static Dictionary<string, DatabaseReference> dbReferences = new Dictionary<string, DatabaseReference>();

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
        if (!DBInitialized)
        {
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(dbUrl);
            db = FirebaseDatabase.DefaultInstance;
            storage = FirebaseStorage.DefaultInstance;
            DBInitialized = true;
        }
    }

    public static void registerForUpdates(string category, Action<List<Prayer>, DatabaseError> callback)
    {
        if (dbReferences.ContainsKey(category))
        {
            return;
        }

        dbReferences[category] = db.GetReference(category);
        dbReferences[category].ChildAdded += (sender, args) => valuesChanged(category, args, callback);
    }

    private static void valuesChanged(string category, ChildChangedEventArgs args, Action<List<Prayer>, DatabaseError> callback)
    {
        if (args.DatabaseError != null)
        {
            UAR.Logger.log(UAR.Logger.Type.Error, "DB Error: {0}", args.DatabaseError.Message);
            callback(null, args.DatabaseError);
            return;
        }

        DataSnapshot snapshot = args.Snapshot;
        var prayer = snapshot;
        var prayersList = new List<Prayer>();

        try
        {
            //foreach (DataSnapshot prayer in dbPrayers)
            //{
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

            prayersList.Add(new Prayer(id, category, position, orientation));
            //}
        }
        catch (Exception e)
        {
            UAR.Logger.log(UAR.Logger.Type.Error, "Prayer Creation Error: {0}", e);
        }

        callback(prayersList, null);
    }

    public void fetchAudio(Action<float[], AggregateException> callback)
    {
        byte[] buffer;
        const long bufferSize = 34008512;

        storage.GetReference(id).GetBytesAsync(bufferSize).ContinueWith(task => {
            if (task.Exception != null)
            {
                UAR.Logger.log(UAR.Logger.Type.Error, "Fetch Audio ERROR: {0}", task.Exception);
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

        db.RootReference.Child(category).Child(id).SetRawJsonValueAsync(jsonstr).ContinueWith(task => {
            if (task.Exception != null)
            {
                UAR.Logger.log(UAR.Logger.Type.Error, "Upload ERROR: {0}", task.Exception);
                callback(task.Exception);
                return;
            }

            uploadAudio(callback);
        });
    }

    void uploadAudio(Action<AggregateException> callback)
    {
        int bufferSize = audio.Length * sizeof(float);
        byte[] buffer = new byte[bufferSize];
        System.Buffer.BlockCopy(audio, 0, buffer, 0, bufferSize);

        storage.GetReferenceFromUrl("gs://reflections-51bdd.appspot.com").Child(id).PutBytesAsync(buffer).ContinueWith(task =>
        {
            if (task.Exception != null)
            {
                UAR.Logger.log(UAR.Logger.Type.Error, "upload audio Error: {0}", task.Exception);
                callback(task.Exception);
                return;
            }

            if (task.IsCompleted)
            {
                UAR.Logger.log(UAR.Logger.Type.Info, "Audio Uploaded");
            }

            callback(null);
        });
    }
}