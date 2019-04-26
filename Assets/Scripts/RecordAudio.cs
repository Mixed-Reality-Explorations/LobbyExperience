using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class RecordAudio : MonoBehaviour
{
    private bool micInitialized;
    private AudioSource audioSource;

    public float sensitivity;
    public bool blown;
    public int recordingTimeSecs;
    public Image progressIndicator;
    public Text recordingButtonText;

    void Awake() {
        audioSource = GetComponent<AudioSource>();
        progressIndicator.CrossFadeAlpha(1f, 0f, true);
    }

    public void Record() {
        Delete();

        if (Microphone.devices.Length > 0) {
            UAR.Logger.log(UAR.Logger.Type.Info, "Recording");
            StartCoroutine(trackRecordingProgress());
        }

    }

    IEnumerator trackRecordingProgress()
    {
        recordingButtonText.text = "STOP";
        progressIndicator.fillAmount = 0f;
        progressIndicator.CrossFadeAlpha(1f, .25f, true);
        audioSource.clip = Microphone.Start(Microphone.devices[0], false, recordingTimeSecs, 44100);
        float elpased = 0;

        while (Microphone.IsRecording(null))
        {
            elpased += .25f;
            progressIndicator.fillAmount = elpased / recordingTimeSecs;
            yield return new WaitForSeconds(.25f);
        }

        progressIndicator.CrossFadeAlpha(0f, .25f, true);
        recordingButtonText.text = "RECORD";
    }

    public void Stop()
    {
        if (Microphone.devices.Length > 0)
        {
            Microphone.End(null);
        }
    }

    public void Play() {
        if (Microphone.IsRecording(null))
        {
            Stop();
        }

        UAR.Logger.log(UAR.Logger.Type.Info, "Playing");
        if (audioSource.clip) {
            audioSource.Play();
        } else
        {
            UAR.Logger.log(UAR.Logger.Type.Info, "There is no audio to play.");
        }
    }

    public void Delete()
    {
        UAR.Logger.log(UAR.Logger.Type.Info, "Deleting");
        audioSource.Stop();
        Destroy(audioSource.clip);
    }

    public void Send()
    {
        UAR.Logger.log(UAR.Logger.Type.Info, "Sending");
        AppController.Instance.uploadPrayer(audioSource, transform.localPosition, Quaternion.identity);
    }
}
