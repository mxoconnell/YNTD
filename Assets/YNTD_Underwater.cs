using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class YNTD_Underwater : MonoBehaviour {
    /// <summary>
    /// Okay so this level's triggers are all based on how high above the ground the player is. We trigger off they player's Y.    /// 
    /// </summary>
	// Use this for initialization
    public bool isUnderWater;
    UnityStandardAssets.ImageEffects.BlurOptimized blur;
    [SerializeField] private GameObject camera;
    void Start () {
        Assert.IsNotNull(camera);
        blur = camera.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized>();
        Assert.IsNotNull(blur);
    }

    public void SetEnabled(bool isEnabled)
    {
        blur.enabled = isEnabled;
    }


}
