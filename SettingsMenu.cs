using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public GameManager GM;
    public Slider masterSlider, musicSlider, SFXSlider;
    public AudioMixer masterMixer, musicMixer, SFXMixer;
    public float masterVol, musicVol, SFXVol;
    bool playingSound = false;
    int soundTracker = 0;
    public Button exitButton, confirmExit;

    public void OnEnable()
    {
        confirmExit.gameObject.SetActive(false);
        if(Application.platform == RuntimePlatform.WebGLPlayer) { exitButton.gameObject.SetActive(false); }
    }

    public void ToggleMenu()
    {
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    public void VolumeInit()
    {
        musicMixer.SetFloat("Vol", VolumeEquation(-2));
        SFXMixer.SetFloat("Vol", VolumeEquation(0));

        musicSlider.SetValueWithoutNotify(-2f);
        SFXSlider.SetValueWithoutNotify(0);
    }


    //public void VolumeInit()
    //{
    //    //load the volume settings
    //    masterVol = GM.settingsSave.masterVol;
    //    musicVol = GM.settingsSave.musicVol;
    //    SFXVol = GM.settingsSave.SFXVol;
    //    voiceVol = GM.settingsSave.voiceVol;
    //    masterMixer.SetFloat("Vol", masterVol);
    //    musicMixer.SetFloat("Vol", musicVol);
    //    SFXMixer.SetFloat("Vol", SFXVol);
    //    voiceMixer.SetFloat("Vol", voiceVol);


    //    float param;
    //    masterMixer.GetFloat("Vol", out param);
    //    masterSlider.value = param;
    //    musicMixer.GetFloat("Vol", out param);
    //    musicSlider.value = param;
    //    SFXMixer.GetFloat("Vol", out param);
    //    SFXSlider.value = param;
    //    voiceMixer.GetFloat("Vol", out param);
    //    voiceSlider.value = param;
    //}

    public void SetMasterVolume()
    {
        masterVol = VolumeEquation(masterSlider.value);
        masterMixer.SetFloat("Vol", masterVol);
        //GM.settingsSave.masterVol = masterSlider.value;
    }

    public void SetMusicVolume()
    {
        musicVol = VolumeEquation(musicSlider.value);
        musicMixer.SetFloat("Vol", musicVol);
        //GM.settingsSave.musicVol = musicSlider.value;
    }

    public void SetSFXVolume()
    {
        SFXVol = VolumeEquation(SFXSlider.value);
        SFXMixer.SetFloat("Vol", SFXVol);
        //GM.settingsSave.SFXVol = SFXSlider.value;
        //if (playingSound == false && gameObject.activeInHierarchy)
        //{
        //    StartCoroutine(SFXSoundDelay());
        //}
    }

    //public IEnumerator SFXSoundDelay()
    //{
    //    playingSound = true;
    //    switch (soundTracker)
    //    {
    //        case 0:
    //            GM.sfx.PlaySound(GM.sfx.levelUpDing);
    //            break;
    //        case 1:
    //            GM.sfx.PlaySound(GM.sfx.playerWeaponHits);
    //            break;
    //        case 2:
    //            GM.sfx.PlaySound(GM.sfx.enemyNormalHits);
    //            break;
    //        case 3:
    //            GM.sfx.PlaySound(GM.sfx.playerMagicHits);
    //            break;
    //        case 4:
    //            GM.sfx.PlaySound(GM.sfx.mediumHeal);
    //            break;
    //    }
    //    soundTracker += 1;
    //    if (soundTracker > 4) { soundTracker = 0; }

    //    yield return new WaitForSeconds(0.8f);
    //    playingSound = false;
    //}

    public float VolumeEquation(float level)
    {
        //float factor = -Mathf.Abs(Mathf.Pow(level, 4.0f));
        float factor = -5f;
        switch (level)
        {
            case 0: factor = 0; break;
            case -1: factor = -2; break;
            case -2: factor = -4; break;
            case -3: factor = -7; break;
            case -4: factor = -10; break;
            case -5: factor = -15; break;
            case -6: factor = -21; break;
            case -7: factor = -28; break;
            case -8: factor = -35; break;
            case -9: factor = -45; break;
            case -10: factor = -80f; break;
        }
        return factor;
    }

    public void ExitGame()
    {
        if (confirmExit.gameObject.activeInHierarchy)
        {
            Application.Quit();
        }
        else
        {
            confirmExit.gameObject.SetActive(true);
        }
    }
}
