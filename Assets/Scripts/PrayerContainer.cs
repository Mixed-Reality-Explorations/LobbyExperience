using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrayerContainer : MonoBehaviour
{
    public ParticleSystem flame;
    private Material particleMaterial;
    private SphereCollider sphereCollider;
    public Prayer prayer;
    
    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        particleMaterial = flame.GetComponent<ParticleSystemRenderer>().material;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 screenPos = Input.mousePosition;
            Camera c = GameObject.FindObjectOfType<Camera>();
            Ray ray = c.ScreenPointToRay(screenPos);

            RaycastHit hit;
            if (Physics.Raycast(ray.origin, ray.direction, out hit))
            {
                if (hit.collider == sphereCollider)
                {
                    playAudio();
                }
            }
        }
       
    }

    public void playAudio()
    {
        sphereCollider.enabled = false;

        if (prayer.audio == null)
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
                        audioSource.clip = clip;
                        StartCoroutine(trackAudioPlayback());
                    });
                }
            });
        }
        else
        {
            StartCoroutine(trackAudioPlayback());
        }
    }

    IEnumerator trackAudioPlayback()
    {

        float initialAlpha = particleMaterial.GetColor("_TintColor").a;
        float fadeTimeSecs = .75f;
        Color color;

        AudioSource audioSource = GetComponent<AudioSource>();
        float clipLength = audioSource.clip.length;
        float elapsed = 0f;

        audioSource.Play();

        while (elapsed < clipLength)
        {
            elapsed += Time.deltaTime;

            if (elapsed < fadeTimeSecs)
            {
                color = particleMaterial.GetColor("_TintColor");
                color.a = Mathf.Lerp(initialAlpha, 0f, elapsed / fadeTimeSecs);
                particleMaterial.SetColor("_TintColor", color);
            }

            yield return new WaitForEndOfFrame();
        }

        elapsed = 0f;
        while (elapsed < fadeTimeSecs)
        {
            elapsed += Time.deltaTime;

            color = particleMaterial.GetColor("_TintColor");
            color.a = Mathf.Lerp(0f, initialAlpha, elapsed / fadeTimeSecs);
            particleMaterial.SetColor("_TintColor", color);

            yield return new WaitForEndOfFrame();
        }

        sphereCollider.enabled = true;

    }
}
