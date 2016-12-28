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
    [SerializeField] private GameObject cavePoolWater;
    [SerializeField] private Fade cameraFade;

    // Blur variables (due to drinking)
    // blurActivated ? use MAXvelocity : use MINvelocity
    // We are increasing the MIN as we progress through game
    UnityStandardAssets.ImageEffects.CameraMotionBlur motionBlurScript;
    [SerializeField] private YNTD_Underwater waterBlurScript;
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
    [SerializeField] private GameObject focalPoint;
    Vector3 playerStartLocation;
    Quaternion playerStartRotation;
    Quaternion playerStartRotationCamera;

    /*
     *  0 = before they begin game (main menu
     *  1 = After they begin the game 
     */
    int state = 0;
    bool isDisplayingInputPrompt = true; // Start with prompt: "E to begin game"
    bool triggerOne = false;             // Is the bottle by the pool being actively observed?
    bool triggerTwo = false;
    DateTime timeOfLastPrompt;           // This tracks the last time we were told to keep the prompt up. If it's been a second, we'll turn it off.
    bool isPoolLightingUp = false;
    bool isDrowning = false; // are we under the rising cave water?

    // Use this for initialization
    void Start () {
        Assert.IsNotNull(poolProbe);
        Assert.IsNotNull(poolLight);

        Assert.IsNotNull(FPSController);
        Assert.IsNotNull(txtTitle);
        Assert.IsNotNull(txtCredit);
        Assert.IsNotNull(txtPrompt);
        motionBlurScript = FPSController.GetComponentInChildren<UnityStandardAssets.ImageEffects.CameraMotionBlur>();
        Assert.IsNotNull(motionBlurScript);
        playerStartRotation = FPSController.gameObject.transform.rotation;
        playerStartRotationCamera = FPSController.gameObject.GetComponentInChildren<Camera>().transform.rotation;
        playerStartLocation = FPSController.gameObject.transform.position;
        curBlur = BLUR_TARGET_VELOCITY;

        FPSController.gameObject.transform.LookAt(focalPoint.transform);
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
        motionBlurScript.velocityScale = curBlur;


        if(isDisplayingInputPrompt){
            txtPrompt.SetFadingIn(true);
        }

        // If it's been greater than n seconds, lets start turning it off
        if(state != 0 && (DateTime.Now-timeOfLastPrompt).TotalMilliseconds > 300){
            // Let's make it fade away
            isDisplayingInputPrompt = false;
            txtPrompt.SetFadingIn(false);
           ///Debug.Log("Fading out");
        }

        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q))
            Application.Quit();
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
                FPSController.gameObject.transform.LookAt(focalPoint.transform);
            }
            else if (state == 1)
            {
                // We received a key press while waiting for input
                if(isDisplayingInputPrompt)
                {
                    if(triggerOne)
                        Trigger_One_Activated();
                    if(triggerTwo)
                        Trigger_Two_Activated();

                }
            }
        }

        // Check what location-based ambient sound we should play based on player's y coord
        // Outside by pool, above ground
        if(FPSController.gameObject.transform.position.y > 2.45)
            AudioController.PlaySound(YNTD_AudioController.Sounds.BG_AboveGround);
        if(FPSController.gameObject.transform.position.y < 2.45 && FPSController.gameObject.transform.position.y > -.91)
            AudioController.PlaySound(YNTD_AudioController.Sounds.BG_InWater);
        if(FPSController.gameObject.transform.position.y < -.91)
            AudioController.PlaySound(YNTD_AudioController.Sounds.BG_InCave);

        // Determine if underwater blur
        waterBlurScript.SetEnabled(isDrowning || FPSController.gameObject.transform.position.y > -1 && FPSController.gameObject.transform.position.y <= 1.1);


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
        triggerTwo = true;
        timeOfLastPrompt = DateTime.Now;
    }

    // The bottle by the pool is being observed and drunk
    void Trigger_One_Activated()
    {
        Debug.Log("YOU DRUNK IT!");
        BLUR_TARGET_VELOCITY = 10;
        BLUR_MIN_VELOCITY += 5;
        DrinkAlcohol();
        StartCoroutine( FlareUpLightInWater());
        poolBarrier.SetActive(false);
    }

    // The bottle underground is being observed and drunk
    void Trigger_Two_Activated()
    {
        Debug.Log("YOU DRUNK IT X2!");
        BLUR_TARGET_VELOCITY = 30;
        BLUR_MIN_VELOCITY += 10;
        DrinkAlcohol();

        poolBarrier.SetActive(true);
        StartCoroutine(EndGameEffects());
        // Drowning sounds !

        
    }


    //Makes the water underground glow very bright and raises the height, this is for underground (used for trigger 2)
    IEnumerator EndGameEffects()
    {
        // Water effects
        int maxIntensity = 7;
        float maxY = -.97f;
        while(cavePoolWater.transform.position.y < 2)
        {
            cavePoolProbe.intensity += .2f;
            cavePoolWater.transform.position = new Vector3(cavePoolWater.transform.position.x, cavePoolWater.transform.position.y + .001f, cavePoolWater.transform.position.z);
            yield return new WaitForSeconds(0.01f);

            if(cavePoolWater.transform.position.y > maxY && !isDrowning)
            {
                Debug.Log("Drowning now!");
                // End Game
                isDrowning = true;
                AudioController.PlaySound(YNTD_AudioController.Sounds.SE_Drowned);
                cameraFade.FadeToImage();
                StartCoroutine(RestartGame());
            }
        }
        
    }

    // Makes the light in the water's intensity increase then die back down, this is the light on the surface of water (used for trigger 1)
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
    IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(19f);
        AudioController.KillSounds();
        state = 0;
        isDrowning = false;
        poolBarrier.SetActive(true);
        poolCorpseController.gameObject.SetActive(true);
        poolCorpseController.Reset();
        triggerOne = false;
        triggerTwo = false;

        cavePoolWater.transform.position = new Vector3(cavePoolWater.transform.position.x, -2.2f, cavePoolWater.transform.position.z); 

        cameraFade.FadeFromImage();

        FPSController.gameObject.transform.position = playerStartLocation;
        FPSController.gameObject.transform.rotation = playerStartRotation;
        FPSController.gameObject.GetComponentInChildren<Camera>().transform.rotation = playerStartRotationCamera;
        FPSController.gameObject.transform.LookAt(focalPoint.transform);

        FPSController.gameHasNotBegun = true;
        txtPrompt.SetFadingIn(false);
        poolProbe.intensity = 0;
        poolLight.intensity = 0;

        //UI
        yield return new WaitForSeconds(.5f);
        txtTitle.SetFadingIn(true);
        txtCredit.SetFadingIn(true);
        txtPrompt.SetFadingIn(true);
        txtPrompt.SetText("Press [E] to Begin");
        isDisplayingInputPrompt = true;


        BLUR_TARGET_VELOCITY = 5;
        BLUR_MIN_VELOCITY = 5;
        StopAllCoroutines();

    }
}
