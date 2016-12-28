using UnityEngine;
using System.Collections;

public class YNTD_textController : MonoBehaviour {

    UnityEngine.UI.Text text;
    bool fadingIn = true; // If not fading in, we're fading out.
    static float MAX_BRIGHTNESS = 0.7f;
    // Use this for initialization
    void Start () {
        text = GetComponent<UnityEngine.UI.Text>();
        UnityEngine.Assertions.Assert.IsNotNull(text);

        // Start with the text invisible
        Color updatedColor = text.color;
        updatedColor.a = 0f; ;
        text.color = updatedColor;
    }

    // Update is called once per frame
    void Update () {
        Color updatedColor = text.color;
        if(fadingIn && updatedColor.a<MAX_BRIGHTNESS)
            updatedColor.a = updatedColor.a + 0.02f;
        else
            updatedColor.a = updatedColor.a - 0.04f;

        if(updatedColor.a > 1) updatedColor.a = 1;
        if(updatedColor.a < 0) updatedColor.a = 0;

        text.color = updatedColor;
    }

    public void SetFadingIn(bool isFadingIn)
    {
        fadingIn = isFadingIn;
    }

    public void SetText(string newText)
    {
        text.text = newText;
    }
}
