using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class YNTD_AudioController : MonoBehaviour {
    /// <summary>
    /// This is the sound machine!
    /// I have two source's for sound effects in case I want to play two sound effects concurrently.
    /// </summary>
    /// 
    public enum Sounds { SE_Gulp, BG_InWater, BG_AboveGround, BG_InCave, SE_Drowned };

    [SerializeField] private AudioSource srcOmnipresentBG;
    [SerializeField] private AudioSource srcLocationBasedBG;
    [SerializeField] private AudioSource srcSoundEffects;
    [SerializeField] private AudioSource srcSoundEffects2;
    [SerializeField] private AudioClip SE_Gulp;
    [SerializeField] private AudioClip SE_Sigh;
    [SerializeField] private AudioClip BG_AboveGround;
    [SerializeField] private AudioClip BG_InWater;
    [SerializeField] private AudioClip BG_InCave;
    [SerializeField] private AudioClip SE_Drowned;


    // Use this for initialization
    void Start () {
        Assert.IsNotNull(srcOmnipresentBG);
        Assert.IsNotNull(srcLocationBasedBG);
        Assert.IsNotNull(srcSoundEffects);
        Assert.IsNotNull(srcSoundEffects2);

        Assert.IsNotNull(SE_Gulp);
        Assert.IsNotNull(BG_AboveGround);
        Assert.IsNotNull(BG_InWater);
        Assert.IsNotNull(BG_InCave);
        Assert.IsNotNull(SE_Drowned);
    }

    public void KillSounds()
    {
        srcSoundEffects.Stop();
        srcSoundEffects2.Stop();
    }


    public void PlaySound(Sounds sound)
    {
        // SE's
        if(sound == Sounds.SE_Gulp)
        {
            srcSoundEffects.clip = SE_Gulp;
            srcSoundEffects.Play();
            // Note: You could do this with one source
            srcSoundEffects2.clip = SE_Sigh;
            srcSoundEffects2.PlayDelayed(.5f); 
        }
        // SE's
        else if(sound == Sounds.SE_Drowned)
        {
            srcSoundEffects.clip = SE_Drowned;
            srcSoundEffects.Play();
            srcSoundEffects2.clip = BG_InWater;
            srcSoundEffects2.Play();
        }
        // BG's
        else if(sound == Sounds.BG_AboveGround && srcLocationBasedBG.clip != BG_AboveGround)
        {
            srcLocationBasedBG.clip = BG_AboveGround;
            srcLocationBasedBG.Play();
        }
        else if(sound == Sounds.BG_InWater && srcLocationBasedBG.clip != BG_InWater)
        {
            srcLocationBasedBG.clip = BG_InWater;
            srcLocationBasedBG.Play();
        }
        else if(sound == Sounds.BG_InCave && srcLocationBasedBG.clip != BG_InCave)
        {
            srcLocationBasedBG.clip = BG_InCave;
            srcLocationBasedBG.Play();

        }

    }

}
