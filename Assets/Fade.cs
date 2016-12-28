using UnityEngine;
using System.Collections;

public class Fade : MonoBehaviour {
    [SerializeField]
    private Texture2D fadeTexture;
    float alpha = 0.0f;
    int fadeDir = -1;
    bool isFading = false;
    float fadeSpeed = 0.009f;
    int drawDepth = -1000;

    void OnGUI () {
        if(isFading)
        {
            alpha -= fadeDir * fadeSpeed * Time.deltaTime;
            alpha = Mathf.Clamp01(alpha);

            Color thisAlpha = GUI.color;
            thisAlpha.a = alpha;
            GUI.color = thisAlpha;

            GUI.depth = drawDepth;

            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);

            // Stop fading
            if(isFading && (alpha < 0 || alpha > 1))
                isFading = false;
        }
    }

    public void FadeToImage()
    {
        isFading = true;
        fadeSpeed = 0.009f;
        fadeDir = -1;
    }
    public void FadeFromImage()
    {
        isFading = true;
        fadeSpeed = 0.1f;
        fadeDir = 1;
    }

}
