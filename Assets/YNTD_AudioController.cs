using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class YNTD_AudioController : MonoBehaviour {
    /// <summary>
    /// This is the sound machine!
    /// I have two source's for sound effects in case I want to play two sound effects concurrently.
    /// </summary>
    /// 
    public enum Sounds { SE_Gulp };

    [SerializeField] private AudioSource srcOmnipresentBG;
    [SerializeField] private AudioSource srcLocationBasedBG;
    [SerializeField] private AudioSource srcSoundEffects;
    [SerializeField] private AudioSource srcSoundEffects2;
    [SerializeField] private AudioClip SE_Gulp;
    [SerializeField] private AudioClip SE_Sigh;
    [SerializeField] private AudioClip BG_UnderWater;


    // Use this for initialization
    void Start () {
        Assert.IsNotNull(srcOmnipresentBG);
        Assert.IsNotNull(srcLocationBasedBG);
        Assert.IsNotNull(srcSoundEffects);
        Assert.IsNotNull(srcSoundEffects2);

        Assert.IsNotNull(SE_Gulp);
        Assert.IsNotNull(BG_UnderWater);
    }
	
    public void PlaySound(Sounds sound)
    {
        if(sound == Sounds.SE_Gulp)
        {
            srcSoundEffects.clip = SE_Gulp;
            srcSoundEffects.Play();

            srcSoundEffects2.clip = SE_Sigh;
            srcSoundEffects2.PlayDelayed(.5f);
        }

    }

}
