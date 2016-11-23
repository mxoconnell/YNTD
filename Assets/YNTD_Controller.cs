using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class YNTD_Controller : MonoBehaviour {
    [SerializeField] private UnityEngine.UI.Text title;
    [SerializeField] private UnityEngine.UI.Text credit;
    [SerializeField] private UnityEngine.UI.Text prompt;
    [SerializeField] private UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController FPSController;
    /*
     *  0 = before they begin game (main menu
     *  1 = After they begin the game 
     */
    int state = 0;
    bool isDisplayingInputPrompt = false;
    bool triggerOne = false; // Is the bottle by the pool being actively observed?

    // Use this for initialization
    void Start () {
        Assert.IsNotNull(title);
        Assert.IsNotNull(credit);
        Assert.IsNotNull(prompt);

        title.gameObject.SetActive(true);
        credit.gameObject.SetActive(true);
        prompt.gameObject.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {
        if(Input.anyKey)
        {
            // If they press a key on main menu remove the title cards
            if(state == 0)
            {
                title.gameObject.SetActive(false);
                credit.gameObject.SetActive(false);
                prompt.gameObject.SetActive(false);
                prompt.text = "Press  [E] to Drink";
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

            if(isDisplayingInputPrompt)
                prompt.gameObject.SetActive(true);
            else
                prompt.gameObject.SetActive(false);
        }
    }

   /* // Turns the displays off (no bottle is being observed)
    void Triggers_Off()
    {
        isDisplayingInputPrompt = false;
        triggerOne = false;
    }*/

    // The bottle by the pool is being observed
    void Trigger_One()
    {
        isDisplayingInputPrompt = true;
        triggerOne = true;
    }

    // The bottle by the pool is being observed
    void Trigger_One_Activated()
    {
        DrinkAlcohol();
        FlareUpLightInWater();
    }

    // Makes the light in the water's intensity increase then die back down
    void FlareUpLightInWater()
    {

    }

    // Blur camera a for a short moment to simulate drunkeness and make a sound of drinking
    void DrinkAlcohol()
    {

    }
}
