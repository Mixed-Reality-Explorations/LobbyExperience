using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using Firebase;
//using Firebase.Database;
//using Firebase.Storage;
//using Firebase.Unity.Editor;
using System.Threading.Tasks;


public class DBController : MonoBehaviour
{

    public AudioSource audioSource;

    //private DatabaseReference reference;
    //private FirebaseStorage storage;
    //private StorageReference audio_ref;
    //private StorageReference storage_ref;

    private float[] audioData;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void setUpDatabase()
    {
        UAR.Logger.log(UAR.Logger.Type.Info, "DBController - Database Setting Up.");

        // Get a reference to the storage service, using the default Firebase App
        //FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://reflections-51bdd.firebaseio.com/");

        // Get the root reference location of the database.
        //reference = FirebaseDatabase.DefaultInstance.RootReference;

        //storage = FirebaseStorage.DefaultInstance;
        //storage_ref = storage.GetReferenceFromUrl("gs://reflections-51bdd.appspot.com");
        //audio_ref = storage_ref.Child("audioTest");
    }

    public void testDownload()
    {

        //UAR.Logger.log(UAR.Logger.Type.Info, "DBController - Downloading AudioTest.");
        //byte[] fileContents = { };
        //const long maxAllowedSize = 34008512;

        //audio_ref.GetBytesAsync(maxAllowedSize).ContinueWith((Task<byte[]> task) =>
        //{
        //    if (task.IsFaulted || task.IsCanceled)
        //    {
        //        Debug.Log("Downloading Failed");
        //        Debug.Log(task.Exception.ToString());
        //        // Uh-oh, an error occurred!
        //    }
        //    else
        //    {

        //        fileContents = task.Result;
        //        UAR.Logger.log(UAR.Logger.Type.Info, "DBController - Download Done.");
        //        audioData = new float[fileContents.Length / 4];

        //        System.Buffer.BlockCopy(fileContents, 0, audioData, 0, fileContents.Length);
        //        audioSource.Play();
        //    }
        //});
    }

}
