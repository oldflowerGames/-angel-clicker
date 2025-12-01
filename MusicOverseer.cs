using UnityEngine;

public class MusicOverseer : MonoBehaviour
{
    public GameManager GM;
    public int loopsBeforeFadeToB;
    public int loopsBeforeFadeToA;

    public int loopCount = 0;
    public bool playingB;

    public MusicManager managerA;
    public MusicManager managerB;
    public AudioSource activeAudioSource;

    public void AddLoop()
    {
        loopCount += 1;

        if(playingB == false && loopCount >= loopsBeforeFadeToB)
        {
            loopCount = 0;
            managerB.StartPlaying(true, true);
            managerB.StartFadeIn(10f, true, 0f);
            managerB.SetTime(managerA.GetCurrentTime());
            managerA.StartFadeOut(10f, true, 1f);
            GM.EnterNightMode();

            playingB = true;
        }
        else if(playingB && loopCount >= loopsBeforeFadeToA)
        {
            loopCount = 0;
            managerA.StartPlaying(true, true);
            managerA.StartFadeIn(10f, true, 3f);
            managerA.SetTime(managerB.GetCurrentTime());
            managerB.StartFadeOut(10f, false, 3f);
            GM.ExitNightMode();
            playingB = false;
        }

        if (playingB) { activeAudioSource = managerB.activeAudio; }
        else { activeAudioSource = managerA.activeAudio; }
    }

    public void FadeIn(float rate)
    {

    }
}
