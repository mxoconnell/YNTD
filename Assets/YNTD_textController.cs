﻿using UnityEngine;
using System.Collections;

public class YNTD_textController : MonoBehaviour {

    UnityEngine.UI.Text text;
    bool fadingIn = true; // If not fading in, we're fading out.
    static float MAX_BRIGHTNESS = 0.7f;
    // Use this for initialization
    void Start () {
        text = GetComponent<UnityEngine.UI.Text>();
        UnityEngine.Assertions.Assert.IsNotNull(text);
        Debug.Log(text.color.a);
    }

    // Update is called once per frame
    void Update () {
        Color updatedColor = text.color;
        if(fadingIn && updatedColor.a<MAX_BRIGHTNESS)
            updatedColor.a = updatedColor.a + 0.01f;
        else
            updatedColor.a = updatedColor.a - 0.01f;
        text.color = updatedColor;
    }

    public void SetFadingIn(bool isFadingIn)
    {
        fadingIn = isFadingIn;
    }
}
