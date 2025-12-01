using System.Collections;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public AudioClip[] builderHello = new AudioClip[4];
    public AudioClip[] builderPickup = new AudioClip[4];
    public AudioClip[] builderScream = new AudioClip[4];
    public AudioClip[] builderExplosion = new AudioClip[4];
    public SFXPool[] allPools = new SFXPool[20];
    public AudioSource[] allSources = new AudioSource[20];

    AudioClip temp;
    public SFXPool selectBuilding, upgrade, collectTactile, constructionComplete, towerComplete, craftedCrystal, giantWave, badSurprise,
        activatePrestige, selectPrestige, endPrestige, martLevel, martTierIncrease, studyTotalLevel, craftedBigCrystal, endingWoosh;

    public AudioClip GetClip(AudioClip[] arrayPass)
    {
        temp = arrayPass[Random.Range(0, arrayPass.Length - 1)];
        return temp;
    }

    double nextEventTime;

    public void PlaySound(SFXPool pool)
    {
        PlaySound(pool, 1f);
    }
    public void PlaySound(SFXPool pool, float volumeRatio, bool cancelCoroutine = false)
    {
        if (cancelCoroutine)
        {
            pool.StopAllCoroutines();//cancels any fades that might be happening
        }
        //
        pool.SelectSound(volumeRatio);
        //
    }

    public void PlaySound(SFXPool pool, float volumeRatio, float delayTime)
    {
        StartCoroutine(SoundDelay(pool, volumeRatio, delayTime));
    }

    public void PlaySoundScheduled(SFXPool pool, float volumeRatio, float delayTime)
    {
        nextEventTime = AudioSettings.dspTime + delayTime;
        pool.SelectSoundScheduled(nextEventTime, 1, volumeRatio);
    }

    public IEnumerator SoundDelay(SFXPool pool, float volumeRatio, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        pool.SelectSound(volumeRatio);
    }

    public void PlaySoundPitch(SFXPool pool, float volumeRatio, float pitch)
    {
        pool.SelectSound(pitch, volumeRatio);
    }

    public void StopSound(SFXPool pool)
    {
        pool.StopSound();
    }

    public void FadeOutSound(SFXPool pool, float duration, bool coroutineOnPool = false)
    {
        if (coroutineOnPool)
        {
            pool.StartCoroutine(pool.FadeOut(duration));
        }
        else
        {
            StartCoroutine(pool.FadeOut(duration));
        }

    }

    public void FadeInSound(SFXPool pool, float duration)
    {
        pool.StartFadeIn(duration);
    }

}
