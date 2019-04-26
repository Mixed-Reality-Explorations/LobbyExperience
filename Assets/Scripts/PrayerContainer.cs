using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrayerContainer : MonoBehaviour
{
    public Prayer prayer;


    public void playAudio()
    {
        prayer.fetchAudio((a, e2) =>
        {
            UAR.Logger.log(UAR.Logger.Type.Info, "audio exception? {0}", e2);

            if (e2 == null)
            {
                AppController.mainQueue.Enqueue(() =>
                {
                    AudioSource audioSource = GetComponent<AudioSource>();
                    var clip = AudioClip.Create(prayer.id, a.Length, 1, 44100, false);
                    clip.SetData(prayer.audio, 0);

                    UAR.Logger.log(UAR.Logger.Type.Info, "About to play audio");
                    audioSource.clip = clip;
                    audioSource.Play();
                });
            }
        });
    }
}
