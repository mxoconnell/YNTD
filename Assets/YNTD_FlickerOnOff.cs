using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class YNTD_FlickerOnOff : MonoBehaviour {
    /// <summary>
    /// Flickers an item's enabled attribute from enabled to un-enabled, on and off again, within the hierarchy
    /// </summary>
    [SerializeField] GameObject itemToFlicker1;
    [SerializeField] GameObject itemToFlicker2;
    int numFlicks;
    int numFlicksNeeded;// Number of flicks until we turn the light on/off
    bool isFlickering;
    bool isOn; // If flickering then this is not observed
    static int NUM_FLICKS_UNTIL_OFF = 100;
    static int NUM_FLICKS_UNTIL_ON = 2500;

    // Use this for initialization
    void Start () {
        Assert.IsNotNull(itemToFlicker1);
        Assert.IsNotNull(itemToFlicker2);
        numFlicks = 0;
        numFlicksNeeded = 0;
        isFlickering = true;
        isOn = false;
    }
	
	// Update is called once per frame
	void Update () {
        InvokeRepeating("Flicker", 4.0f, 0.9f); // Starting [2nd parameter] seconds, repeating every [3rd parameter] seconds
    }

    void Flicker()
    {
        numFlicks++;
        if(isFlickering){
            itemToFlicker1.SetActive(!itemToFlicker1.activeInHierarchy);
            itemToFlicker2.SetActive(!itemToFlicker2.activeInHierarchy);
        }
        if(numFlicks > numFlicksNeeded+Random.Range(100, 500)){
            // if it was flickering it will now be on or off for a while
            if(isFlickering)
            {
                isOn = (Random.value >= 0.5);
                itemToFlicker1.SetActive(isOn);
                itemToFlicker2.SetActive(isOn);
                numFlicksNeeded = isOn ? NUM_FLICKS_UNTIL_OFF : NUM_FLICKS_UNTIL_ON;
            }
            isFlickering = !isFlickering;
            numFlicks = 0;
        }
    }

}
