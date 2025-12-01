using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpeningSequence : MonoBehaviour
{
    public GameManager GM;
    public OpeningStep[] steps = new OpeningStep[5];
    public CanvasGroup masterCanvas;
    public Image img1, img2;
    public CanvasGroup img1Canvas, img2Canvas;
    public TextMeshProUGUI text1, text2;
    public CanvasGroup text1Canvas, text2Canvas;
    public FadeUtils fade;
    public int stepCount = 0;
    public int imgStepCount = 0;
    public bool stepAvailable = false;
    public float stepTimer;
    public float stepCooldown = 1.25f;

    public void OnEnable()
    {
        stepCount = 0;
        imgStepCount = 0;
        stepTimer = 0;
        fade.StopAllCoroutines();
        img1Canvas.alpha = img2Canvas.alpha = text1Canvas.alpha = text2Canvas.alpha = 0;
        masterCanvas.alpha = 1;
        GM.mainCam.inputLock = true;
        stepAvailable = true;
        NextStep();
    }

    public void Update()
    {
        if(stepAvailable == false) 
        {
            stepTimer += Time.deltaTime;
            if(stepTimer > stepCooldown ) 
            {
                stepAvailable = true;
                stepTimer = 0;
            }
        }
    }

    public void FinishOpening()
    {
        fade.StartFadeOut(masterCanvas);
        //GM.startgame
        GM.dialogue.QueueDialogue(0);
        GM.mainCam.inputLock = false;
        GM.angel.gameObject.SetActive(true);//disabled at start so she doesn't teleport away
        GM.angel.gameObject.transform.position = GM.angel.teleportLocations[0].transform.position;
        GM.angel.teleportTimer = 0;
    }


    public void NextStep()
    {
        if(stepAvailable == false) { return; }
        if(stepCount >= steps.Length) 
        {
            FinishOpening();
            return;
        }
        
        text1.gameObject.SetActive(true);
        text1.text = steps[stepCount].text;
        text1Canvas.alpha = 1;

        //if (stepCount % 2 == 0)
        //{
        //    text1.text = steps[stepCount].text;
        //    text1.gameObject.SetActive(true);
        //    text2.gameObject.SetActive(false);
        //    //fade.StartFadeOut(text2Canvas, 0, 2f, 0, text2Canvas.alpha);
        //    //fade.StartFadeIn(text1Canvas);
        //}
        //else
        //{
        //    text2.text = steps[stepCount].text;
        //    text1.gameObject.SetActive(false);
        //    text2.gameObject.SetActive(true);
        //    //fade.StartFadeOut(text1Canvas, 0, 2f, 0, text1Canvas.alpha);
        //    //fade.StartFadeIn(text2Canvas);
        //}

        if(steps[stepCount].img != img1.sprite && steps[stepCount].img != img2.sprite) 
        {
            if(imgStepCount % 2 == 0)
            {
                img1.sprite = steps[stepCount].img;
                img1.transform.SetAsLastSibling();
                fade.StartFadeIn(img1Canvas, 2f);
                fade.StartFadeOut(img2Canvas, 0, 1f, 0, img2Canvas.alpha);
            }
            else
            {
                img2.sprite = steps[stepCount].img;
                img2.transform.SetAsLastSibling();
                fade.StartFadeIn(img2Canvas, 2f);
                fade.StartFadeOut(img1Canvas, 0, 1f, 0, img1Canvas.alpha);
            }
            imgStepCount++;
        }

        stepCount++;
        stepAvailable = false;
    }
}
