using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public GameManager GM;
    public MusicOverseer overseer;
    public bool intro = false;
    public bool autoStart = false;
    bool introComplete = false;
    bool loop1Active = false;
    bool loop2Active = false;
    public float introTime;
    public float loopTime;
    public float debugTime;
    public float originalVolume, originalIntroVolume;

    public AudioSource introAudio;
    public AudioSource loopAudio1;
    public AudioSource loopAudio2;

    public AudioSource activeAudio;

    public List<float> speedUpCues = new List<float>();
    public float phraseLength = 0;
    public float phraseOffset = 1.16f;

    public int songIndex = 0;

    private void Start()
    {
        if (autoStart == true)
        {
            StartPlaying();
        }
        originalVolume = loopAudio1.volume;
        if (introAudio != null)
        {
            originalIntroVolume = introAudio.volume;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (intro == true)
        {
            if (introComplete == false)
            {
                if (introAudio.clip != null)
                {
                    if (introAudio.time > introTime)
                    {
                        loopAudio1.Play();
                        activeAudio = loopAudio1;
                        overseer.activeAudioSource = loopAudio1;
                        introComplete = true;
                        loop1Active = true;
                    }
                }
            }
        }
        if (loop1Active == true)
        {
            if (loopAudio1.clip != null)
            {
                if (loopAudio1.time > loopTime)
                {
                    //if (GM.debugMode) { PausePlaying(); }//debug while testing, won't do this in final version
                    loopAudio2.time = 0;//this shouldn't cause any issues but use caution
                    loopAudio2.Play();
                    activeAudio = loopAudio2;
                    loop1Active = false;
                    loop2Active = true;
                    overseer.AddLoop();
                }
            }
        }
        if (loop2Active == true)
        {
            if (loopAudio2.clip != null)
            {
                if (loopAudio2.time > loopTime)
                {
                    //if (MA.debugMode) { PausePlaying(); }//debug while testing, won't do this in final version
                    loopAudio1.time = 0;
                    loopAudio1.Play();
                    activeAudio = loopAudio1;
                    loop2Active = false;
                    loop1Active = true;
                    overseer.AddLoop();
                }
            }
        }
    }

    public void SetVolume(float ratio)
    {
        if (introAudio != null)
        {
            introAudio.volume = originalIntroVolume * ratio;
        }
        loopAudio1.volume = loopAudio2.volume = originalVolume * ratio;
    }

    public void SetTime(float time)
    {
        if (introAudio != null)
        {
            introAudio.time = time;
        }
        if (loop1Active)
        {
            loopAudio1.time = time;
            loopAudio2.time = 0;
        }
        else
        {
            loopAudio2.time = time;
            loopAudio1.time = 0;
        }
    }

    public float GetCurrentTime()
    {
        float currentTime = 0;
        if (loop1Active) { currentTime = loopAudio1.time; }
        else { currentTime = loopAudio2.time; }

        return currentTime;
    }

    public void StartPlaying(bool silent = false, bool skipIntro = false)
    {
        if (silent)
        {
            SetVolume(0);
        }

        if (intro == false || skipIntro == true)
        {
            loopAudio1.Play();
            activeAudio = loopAudio1;
            loop1Active = true;
            if (debugTime != 0)
            {
                loopAudio1.time = debugTime;
            }
        }
        else
        {
            introAudio.Play();
            activeAudio = introAudio;
            overseer.activeAudioSource = introAudio;
            if (debugTime != 0)
            {
                introAudio.time = debugTime;
            }
        }
    }

    public void ResumePlaying()
    {
        if (loop1Active)
        {
            loopAudio1.UnPause();
            //loopAudio1.Play();
        }
        else if (loop2Active)
        {
            loopAudio2.UnPause();
        }
        else if (intro && introComplete == false)
        {
            introAudio.UnPause();
        }
    }

    public void PausePlaying()
    {
        if (loop1Active)
        {
            loopAudio1.Pause();
        }
        else if (loop2Active)
        {
            loopAudio2.Pause();
        }
        else if (introAudio != null)
        {
            introAudio.Pause();
        }
    }

    public void ResetSong()
    {
        loopAudio1.volume = loopAudio2.volume = originalVolume;
        loopAudio1.Stop(); loopAudio2.Stop();
        if (introAudio != null) { introAudio.volume = originalIntroVolume; introAudio.Stop(); }
        introComplete = false;
    }

    public void StartFadeOut(float duration, bool startFromOrigin, float startDelay)
    {
        StartCoroutine(FadeOut(duration, startFromOrigin, startDelay));
    }

    public IEnumerator FadeOut(float duration, bool startFromOriginalVolume, float startDelay)
    {
        if(startDelay > 0)
        {
            yield return new WaitForSeconds(startDelay);
        }
        float period = 0;
        float newVol;
        float startVol = loopAudio1.volume;
        if (loop2Active)
        {
            startVol = loopAudio2.volume;
        }
        while (period < duration)
        {
            yield return new WaitForEndOfFrame();
            //period += Time.unscaledDeltaTime;//almost certainly want this to be unscaled but it seems to cause issues, maybe just in the editor? test in a build later
            period += Time.deltaTime;

            //if (startFromOriginalVolume) 
            //{ 
            //    newVol = Mathf.Lerp(originalVolume, 0, period / duration);
            //}
            //else 
            //{ 
            newVol = Mathf.Lerp(startVol, 0, period / duration);
            //}

            if (introAudio != null) { introAudio.volume = newVol; }
            loopAudio1.volume = newVol;
            loopAudio2.volume = newVol;
        }

        loopAudio1.volume = 0;
        loopAudio1.Pause();

        loopAudio2.volume = 0;
        loopAudio2.Pause();

        if (introAudio != null)
        {
            introAudio.volume = 0;
            introAudio.Pause();
        }
    }

    public void StartFadeIn(float duration, bool resume, float startDelay)
    {
        StartCoroutine(FadeIn(duration, resume, startDelay));
    }

    public IEnumerator FadeIn(float duration, bool resume, float startDelay)
    {
        if (startDelay > 0)
        {
            yield return new WaitForSeconds(startDelay);
        }
        loopAudio1.volume = loopAudio2.volume = 0;
        if (resume) { ResumePlaying(); }
        float period = 0;
        float newVol;
        while (period < duration)
        {
            yield return new WaitForEndOfFrame();
            period += Time.deltaTime;
            newVol = Mathf.Lerp(0, originalVolume, period / duration);
            loopAudio1.volume = newVol;
            loopAudio2.volume = newVol;
            if (intro) { introAudio.volume = newVol; }
        }
    }

    public IEnumerator FadeInAccurate(float duration, bool resume, float goalVal)
    {
        float period = 0;
        float newVol;
        if (resume) { ResumePlaying(); }
        while (period < duration)
        {
            yield return new WaitForEndOfFrame();
            period += Time.deltaTime;
            newVol = Mathf.Lerp(0, goalVal, period / duration);
            loopAudio1.volume = newVol;
            loopAudio2.volume = newVol;
        }
    }

    public IEnumerator MusicDucking(float bottomValue, float bottomHoldTime, float fadeRate, bool instant, float fadeReturnRate = 1)
    {
        float originalVol;
        originalVol = loopAudio1.volume;

        if (instant == false)
        {
            while (loopAudio1.volume > bottomValue)//if not instant, do a quick fade out
            {
                loopAudio1.volume -= Time.deltaTime * fadeRate * 10;
                loopAudio2.volume = loopAudio1.volume;
                if (intro) { introAudio.volume = loopAudio1.volume; }
                yield return new WaitForEndOfFrame();
            }
        }

        loopAudio1.volume = loopAudio2.volume = bottomValue;//otherwise just instantly set it to the lower volume
        if (intro) { introAudio.volume = bottomValue; }

        yield return new WaitForSeconds(bottomHoldTime);

        while (loopAudio1.volume < originalVol)
        {
            loopAudio1.volume += Time.deltaTime * fadeRate * fadeReturnRate;
            loopAudio2.volume = loopAudio1.volume;
            if (intro) { introAudio.volume = loopAudio1.volume; }
            yield return new WaitForEndOfFrame();
        }

        loopAudio1.volume = loopAudio2.volume = originalVol;
        if (intro) { introAudio.volume = originalVol; }
    }

    public IEnumerator MusicDuckingHold(float bottomValue, float fadeRate)
    {
        while (loopAudio1.volume > bottomValue)//if not instant, do a quick fade out
        {
            loopAudio1.volume -= Time.deltaTime * fadeRate;
            loopAudio2.volume = loopAudio1.volume;
            if (intro) { introAudio.volume = loopAudio1.volume; }
            yield return new WaitForEndOfFrame();
        }

        loopAudio1.volume = loopAudio2.volume = bottomValue;//otherwise just instantly set it to the lower volume
        if (intro) { introAudio.volume = bottomValue; }
    }

    public IEnumerator MusicDuckingReturn(float fadeRate)
    {
        while (loopAudio1.volume < originalVolume)
        {
            loopAudio1.volume += Time.deltaTime * fadeRate;
            loopAudio2.volume = loopAudio1.volume;
            if (intro) { introAudio.volume = loopAudio1.volume; }
            yield return new WaitForEndOfFrame();
        }
        loopAudio1.volume = loopAudio2.volume = originalVolume;
        if (intro) { introAudio.volume = originalIntroVolume; }
    }
}
