using System.Collections;
using UnityEngine;

public class SFXPool : MonoBehaviour
{
    public AudioSource[] pool;
    public bool pitchRandom = false;
    public float pitchRange = 0.1f;
    public bool randomSound = false;
    public float originalVolume = 0.5f;
    float originalPitch = 1f;
    int sourceTracker = 0;
    public bool debugInputControl;

    private void Start()
    {
        originalVolume = pool[0].volume;
        originalPitch = pool[0].pitch;
        sourceTracker = 0;
    }

    public void SelectSound(float volumeRatio)
    {
        SelectSound(0, volumeRatio);
    }

    //public void Update()
    //{
    //    if (debugInputControl)
    //    {
    //        if (Keyboard.current.fKey.wasPressedThisFrame)
    //        {
    //            SelectSound(1);
    //        }
    //    }
    //}

    public void SelectSound(float pitch, float volumeRatio)
    {
        bool found = false;
        AudioSource final = null;
        if (randomSound == false)
        {
            if (pool.Length > 1)
            {
                for (int ii = 0; ii < pool.Length; ii += 1)
                {
                    if (pool[ii].isPlaying == false)
                    {
                        final = pool[ii];
                        ii = pool.Length;
                    }
                }
            }
            else
            {
                final = pool[0];
            }
        }
        else
        {
            if (pool.Length > 1)
            {
                int sampleNumber = Random.Range(0, pool.Length);
                if (pool[sampleNumber].isPlaying)//one retry to get a different one that isn't playing, but not using a while loop here for safety reasons
                {
                    sampleNumber = Random.Range(0, pool.Length);
                }

                final = pool[sampleNumber];
            }
            else
            {
                final = pool[0];
            }
        }

        if (final != null)
        {
            PlaySound(final, pitch, volumeRatio);
        }
    }

    public void PlaySound(AudioSource sound, float pitch, float volumeRatio)
    {
        if (pitchRandom)
        {
            //float tempPitch = 1 + Random.Range(-pitchRange, pitchRange);
            float tempPitch = originalPitch + Random.Range(-pitchRange, pitchRange);
            sound.pitch = tempPitch;
        }
        else if (pitch != 0)
        {
            sound.pitch = pitch;
        }
        else
        {
            //sound.pitch = 1;
        }

        sound.volume = originalVolume * volumeRatio;

        sound.Play();
    }

    public void SelectSoundScheduled(double dspTime, float pitch, float volumeRatio)
    {
        bool found = false;
        AudioSource final = null;
        if (pool.Length > 1)
        {
            final = pool[sourceTracker];
            IterateTracker();
        }
        else
        {
            final = pool[0];
        }

        if (final != null)
        {
            PlaySoundScheduled(final, dspTime, pitch, volumeRatio);
        }
    }

    public void PlaySoundScheduled(AudioSource sound, double dspTime, float pitch, float volumeRatio)
    {
        if (pitchRandom)
        {
            //float tempPitch = 1 + Random.Range(-pitchRange, pitchRange);
            float tempPitch = originalPitch + Random.Range(-pitchRange, pitchRange);
            sound.pitch = tempPitch;
        }
        else if (pitch != 0)
        {
            sound.pitch = pitch;
        }
        else
        {
            //sound.pitch = 1;
        }
        sound.volume = originalVolume * volumeRatio;

        sound.PlayScheduled(dspTime);
    }

    public void IterateTracker()
    {
        sourceTracker += 1;
        if (sourceTracker > pool.Length - 1)
        {
            sourceTracker = 0;
        }
    }

    public void StopSound()
    {
        foreach (AudioSource sound in pool)
        {
            sound.Stop();
        }
    }

    public void StartFadeOut(float duration)
    {
        StartCoroutine(FadeOut(duration));
    }

    public IEnumerator FadeOut(float duration)
    {
        float volStore = 0;
        float volTracker = 0;
        volStore = volTracker = pool[0].volume;

        while (volTracker > 0)
        {
            yield return new WaitForEndOfFrame();
            volTracker -= Time.unscaledDeltaTime / duration;
            foreach (AudioSource sound in pool)
            {
                sound.volume = volTracker;
            }
        }

        foreach (AudioSource sound in pool)
        {
            sound.Stop();
            sound.volume = volStore;
        }
    }

    public void StartFadeIn(float duration)
    {
        SelectSound(0);
        StartCoroutine(FadeIn(duration));
    }

    public IEnumerator FadeIn(float duration)
    {
        float volStore = 0;
        float volTracker = 0;
        volStore = volTracker = pool[0].volume;

        while (volTracker < originalVolume)
        {
            yield return new WaitForEndOfFrame();
            volTracker += Time.unscaledDeltaTime / duration;
            foreach (AudioSource sound in pool)
            {
                sound.volume = volTracker;
            }
        }
    }
}
