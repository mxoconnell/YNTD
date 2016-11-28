using UnityEngine;
using System.Collections;


/// <summary>
/// USED for rotating, glowing bottles
/// </summary>
public class YNTD_FloatingAndRotate : MonoBehaviour {
    public double maxY = 0;
    public double minY = 0;
    public double mydY = .0005f;
    public bool goingUp = true;

    public bool preventRotX = false;
    public bool preventRotY = false;
    public bool preventTranslation = false;


    // Determines which way we are rotating (1 or -1)
    int isRotatingPositiveX = 1;
    int isRotatingPositiveY = 1;
    int isRotatingPositiveZ = 1;


    // Glow vars
    Material mat;
    float MAX_EMISSION = 4f;
    float curEmission = 0f;
    int isGlowingUp = 1; // 1 or -1
    Color baseColor = Color.yellow;
    [SerializeField] private float MIN_EMISSION = -2;

    // Use this for initialization
    void Start () {
        // Glow vars
        ColorUtility.TryParseHtmlString("#7F5920", out baseColor);
        curEmission = Random.Range(0, MAX_EMISSION);
        isGlowingUp = (Random.value >= 0.5) ? -1 : 1;
        Debug.Log(isGlowingUp);
        transform.position = new Vector3(transform.position.x, (float)minY, transform.position.z);
        transform.Rotate(new Vector3(Random.Range(-40, 40), Random.Range(-40, 40), Random.Range(-40,40)));
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        // UP AND DOWN
        double dY = goingUp ? mydY : -mydY;
        if(!preventTranslation)
            transform.position = new Vector3(transform.position.x, transform.position.y + (float)dY, transform.position.z);
        if(transform.position.y > maxY || transform.position.y < minY)
            goingUp = !goingUp;

        if(transform.rotation.eulerAngles.x < -40 || transform.rotation.eulerAngles.x > 40)
            isRotatingPositiveX *= -1;
        if(transform.rotation.eulerAngles.y < -40 || transform.rotation.eulerAngles.y > 40)
            isRotatingPositiveY *= -1;

        // ROUND AND ROUND (rotating to max/min of 40/-40)
        if(preventRotX)
            isRotatingPositiveX = 0;
        if(preventRotX)
            isRotatingPositiveY = 0;
        transform.Rotate(new Vector3(isRotatingPositiveX, isRotatingPositiveY, isRotatingPositiveZ) * Time.deltaTime * 20);

        // Determine if we need to change rotations
        if(Random.Range(0, 1000) < 5)
            isRotatingPositiveX *= -1;
        else if(Random.Range(0, 1000) < 5)
            isRotatingPositiveY *= -1;
        else if(Random.Range(0, 1000) < 5)
            isRotatingPositiveZ *= -1;


        // Make it Glow
        if(curEmission >= MAX_EMISSION)
            isGlowingUp = -1;
        else if(curEmission <= MIN_EMISSION)
            isGlowingUp = 1;

        curEmission += (isGlowingUp * .05f);
        Color finalColor = baseColor * Mathf.LinearToGammaSpace(curEmission<0 ? 0 : curEmission);
        Material mat = GetComponent<Renderer>().material;
        mat.SetColor("_EmissionColor", finalColor);

    }

    public void Sink()
    {
        goingUp = false;
        mydY = .009f;
        minY = -5;
    }

    public void Reset()
    {
        transform.position = new Vector3(transform.position.x, (float)minY, transform.position.z);
        mydY = .0005f;
        minY = 0;
    }

}
