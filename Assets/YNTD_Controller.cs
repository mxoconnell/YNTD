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
        Assert.IsNotNull(FPSController);
        Assert.IsNotNull(txtTitle);
        Assert.IsNotNull(txtCredit);
        Assert.IsNotNull(txtPrompt);

    }
	
	// Update is called once per frame
	void Update () {

        // If we should display the input then...
        if(isDisplayingInputPrompt){
            // display it =)
            Debug.Log("Fading in");
            txtPrompt.SetFadingIn(true);
        }

        // If it's been greater than n seconds, lets start turning it off
        if(state != 0 && (DateTime.Now-timeOfLastPrompt).TotalSeconds > 1){
            // Let's make it fade away
            isDisplayingInputPrompt = false;
            txtPrompt.SetFadingIn(false);
            Debug.Log("Fading out");
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            // If they press a key on main menu remove the title cards
            if(state == 0)
            {
                txtTitle.SetFadingIn(false);
                txtCredit.SetFadingIn(false);

                isDisplayingInputPrompt = false;
                

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
    void Trigger_One()
    {
        txtPrompt.SetText("Press  [E] to Drink");
        isDisplayingInputPrompt = true;
        triggerOne = true; // TODO: when to turn this off?
        timeOfLastPrompt = DateTime.Now;
    }

    // The bottle by the pool is being observed
    void Trigger_One_Activated()
    {
        Debug.Log("YOU DRUNK IT!");
        DrinkAlcohol();
        FlareUpLightInWater();
    }

    // Makes the light in the water's intensity increase then die back down
    IEnumerator FlareUpLightInWater()
    {
        // If we are already lighting up let's make a hard restart
        if(isPoolLightingUp)
            StopAllCoroutines();

        //TODO make these real hooks
        float probeIntensity = 0;
        float pointLightIntensity = 0;
        

        // We are scaling two things to two different maximum intensities
        float LIGHT_PROBE_MAX_INTENSITY = 20;
        float POINT_LIGHT_MAX_INTENSITY = 2;
        float dIntensity = .02f;
        
        // Turn lights on
        while(probeIntensity < LIGHT_PROBE_MAX_INTENSITY || pointLightIntensity < POINT_LIGHT_MAX_INTENSITY)
        {
            probeIntensity += dIntensity*10;
            pointLightIntensity += dIntensity;
            yield return new WaitForSeconds(0.01f);
        }

        // Turn em off
        while(probeIntensity > 0 || pointLightIntensity > 0)
        {
            probeIntensity -= dIntensity*10;
            pointLightIntensity -= dIntensity;
            yield return new WaitForSeconds(0.01f);
        }

    }

    // Blur camera a for a short moment to simulate drunkeness and make a sound of drinking
    void DrinkAlcohol()
    {
        // TODO: Blur
        AudioController.PlaySound(YNTD_AudioController.Sounds.SE_Gulp);
    }
}
