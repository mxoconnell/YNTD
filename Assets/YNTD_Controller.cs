using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using System;

public class YNTD_Controller : MonoBehaviour {

    // External Controllers
    [SerializeField] private YNTD_AudioController AudioController;
    [SerializeField] private UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController FPSController;

    // UI Text
    [SerializeField] private YNTD_textController txtTitle;
    [SerializeField] private YNTD_textController txtCredit;
    [SerializeField] private YNTD_textController txtPrompt;

    // Trigger 1 Vars
    [SerializeField] private ReflectionProbe poolProbe;
    [SerializeField] private Light poolLight;
    [SerializeField] private GameObject poolBarrier;

    // Trigger 2 Vars
    [SerializeField] private ReflectionProbe cavePoolProbe;

    // Blur variables (due to drinking)
    // blurActivated ? use MAXvelocity : use MINvelocity
    // We are increasing the MIN as we progress through game
    UnityStandardAssets.ImageEffects.CameraMotionBlur blurScript;
    static int BLUR_MAX_VELOCITY = 1000000;
    int BLUR_MIN_VELOCITY = 1;
    int BLUR_TARGET_VELOCITY = 1;
    int dBlur = 10000;
    int curBlur;


    // Screen Shake
    Vector3 originPosition ;
    Quaternion originRotation;
    float shake_decay;
    float shake_intensity;

    // Misc
    [SerializeField] private YNTD_Floating poolCorpseController;

    /*
     *  0 = before they begin game (main menu
     *  1 = After they begin the game 
     */
    int state = 0;
    bool isDisplayingInputPrompt = true; // Start with prompt: "E to begin game"
    bool triggerOne = false;             // Is the bottle by the pool being actively observed?
    DateTime timeOfLastPrompt;           // This tracks the last time we were told to keep the prompt up. If it's been a second, we'll turn it off.
    bool isPoolLightingUp = false;

    // Use this for initialization
    void Start () {
        Assert.IsNotNull(poolProbe);
        Assert.IsNotNull(poolLight);

        Assert.IsNotNull(FPSController);
        Assert.IsNotNull(txtTitle);
        Assert.IsNotNull(txtCredit);
        Assert.IsNotNull(txtPrompt);
        blurScript = FPSController.GetComponentInChildren<UnityStandardAssets.ImageEffects.CameraMotionBlur>();
        Assert.IsNotNull(blurScript);
        curBlur = BLUR_TARGET_VELOCITY;
    }
	
	// Update is called once per frame
	void Update () {

        if(shake_intensity > 0)
        {
            transform.position = originPosition + UnityEngine.Random.insideUnitSphere * shake_intensity;
            transform.rotation = new Quaternion(
                            originRotation.x + UnityEngine.Random.Range(-shake_intensity, shake_intensity) * .2f,
                            originRotation.y + UnityEngine.Random.Range(-shake_intensity, shake_intensity) * .2f,
                            originRotation.z + UnityEngine.Random.Range(-shake_intensity, shake_intensity) * .2f,
                            originRotation.w + UnityEngine.Random.Range(-shake_intensity, shake_intensity) * .2f);
            shake_intensity -= shake_decay;
        }

        ///Debug.Log("curBlur:" + curBlur + "   |     BLUR_TARGET_VELOCITY:" + BLUR_TARGET_VELOCITY);
        // Determine which way to inlfuence the blur (due to alcohol)
        if(curBlur < BLUR_TARGET_VELOCITY-dBlur)
            curBlur += dBlur;
        else if(curBlur > BLUR_TARGET_VELOCITY+dBlur)
            curBlur -= dBlur;
        // For subtle changes
        if(Mathf.Abs(curBlur - BLUR_TARGET_VELOCITY) < dBlur)
            curBlur = BLUR_TARGET_VELOCITY;
        blurScript.velocityScale = curBlur;


        if(isDisplayingInputPrompt){
            txtPrompt.SetFadingIn(true);
        }

        // If it's been greater than n seconds, lets start turning it off
        if(state != 0 && (DateTime.Now-timeOfLastPrompt).TotalSeconds > 1){
            // Let's make it fade away
            isDisplayingInputPrompt = false;
            txtPrompt.SetFadingIn(false);
           ///Debug.Log("Fading out");
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            // If they press a key on main menu remove the title cards
            if(state == 0)
            {
                //UI
                txtTitle.SetFadingIn(false);
                txtCredit.SetFadingIn(false);
                isDisplayingInputPrompt = false;

                BLUR_TARGET_VELOCITY = 5;
                BLUR_MIN_VELOCITY = 5;

                // Remove the body in pool, if present
                poolCorpseController.Sink();

                // Enable the player to move around
                FPSController.gameHasNotBegun = false;
                state++;
            }
            else if (state == 1)
            {
                // We received a key press while waiting for input
                if(isDisplayingInputPrompt)
                {
                    if(triggerOne)
                        Trigger_One_Activated();
                }
            }
        }

    }

    // The bottle by the pool is being observed
    // This is called from the first person controller
    void Trigger_One()
    {
        txtPrompt.SetText("Press [E] to Drink");
        isDisplayingInputPrompt = true;
        triggerOne = true; // TODO: when to turn this off?
        timeOfLastPrompt = DateTime.Now;
    }

    // The bottle by the cave is being observed
    // This is called from the first person controller
    void Trigger_Two()
    {
        txtPrompt.SetText("Press [E] to Drink");
        isDisplayingInputPrompt = true;
        triggerOne = true; // TODO: when to turn this off?
        timeOfLastPrompt = DateTime.Now;
    }

    // The bottle by the pool is being observed
    void Trigger_One_Activated()
    {
        Debug.Log("YOU DRUNK IT!");
        BLUR_TARGET_VELOCITY = 10;
        BLUR_MIN_VELOCITY += 5;
        DrinkAlcohol();
        StartCoroutine( FlareUpLightInWater());
        poolBarrier.SetActive(false);

        //Debug:
        StartCoroutine(UndergroundWaterEffects());
    }

    //Makes the water underground glow very bright and raises the height
    IEnumerator UndergroundWaterEffects()
    {
        int maxIntensity = 70;
        // Turn lights on
        while(cavePoolProbe.intensity < maxIntensity)
        {
            cavePoolProbe.intensity += .2f;
            yield return new WaitForSeconds(0.01f);
        }
    }

    // Makes the light in the water's intensity increase then die back down
    IEnumerator FlareUpLightInWater()
    {
        // If we are already lighting up let's make a hard restart
        if(isPoolLightingUp)
            StopAllCoroutines();

        // We are scaling two things to two different maximum intensities
        float LIGHT_PROBE_MAX_INTENSITY = 20;
        float POINT_LIGHT_MAX_INTENSITY = 8;
        float dIntensity = .02f;
        
        // Turn lights on
        while(poolProbe.intensity < LIGHT_PROBE_MAX_INTENSITY || poolLight.intensity < POINT_LIGHT_MAX_INTENSITY)
        {
            poolProbe.intensity += dIntensity*10;
            poolLight.intensity += dIntensity;
            yield return new WaitForSeconds(0.01f);
        }

        // Let them stay on for a bit
        yield return new WaitForSeconds(2);

        // Turn em off
        while(poolProbe.intensity > 0 || poolLight.intensity > 0)
        {
            poolProbe.intensity -= dIntensity*10;
            poolLight.intensity -= dIntensity;
            yield return new WaitForSeconds(0.01f);
        }

    }

    // Blur camera a for a short moment to simulate drunkeness and make a sound of drinking
    void DrinkAlcohol()
    {
        Shake();
        StartCoroutine( BlurForSeconds(5));
        AudioController.PlaySound(YNTD_AudioController.Sounds.SE_Gulp);
    }

    // Turns the blur on for n seconds
    IEnumerator BlurForSeconds(int duration)
    {
        Debug.Log("BLURIIING!");
        BLUR_TARGET_VELOCITY = BLUR_MAX_VELOCITY;
        yield return new WaitForSeconds(duration);
        BLUR_TARGET_VELOCITY = BLUR_MIN_VELOCITY;
    }

    void Shake()
    {
        originPosition = transform.position;
        originRotation = transform.rotation;
        shake_intensity = 500f;
        shake_decay = 0.002f;
    }

    // Bring player back to the begining but add the body
    void RestartGame()
    {
        state = 0;
        poolBarrier.SetActive(true);
        poolCorpseController.gameObject.SetActive(true);
        poolCorpseController.Reset();

        //UI
        txtTitle.SetFadingIn(false);
        txtCredit.SetFadingIn(false);
        txtPrompt.SetText("Press [E] to Begin");
        isDisplayingInputPrompt = true;

        BLUR_TARGET_VELOCITY = 5;
        BLUR_MIN_VELOCITY = 5;
    }
}
