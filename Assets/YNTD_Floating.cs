using UnityEngine;
using System.Collections;

public class YNTD_Floating : MonoBehaviour {
    public double maxY = 0;
    public double minY = 0;
    public double mydY = .0005f;
    public bool goingUp = true;
	// Use this for initialization
	void Start () {
        transform.position = new Vector3(transform.position.x, (float)minY, transform.position.z);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        ///Debug.Log(gameObject.name + " : " + gameObject.transform.position.y);
        double dY = goingUp ? mydY : -mydY;
        transform.Translate(Vector3.up * (float)dY, Space.World);
        if(transform.position.y > maxY || transform.position.y < minY)
            goingUp = !goingUp;
        Debug.Log("mydy:" + mydY + "      miny:" + minY);
    }

    public void Sink()
    {
        goingUp = false;
        mydY = .015f;
        minY = -5;
    }

    public void Reset()
    {
        Debug.Log("Reset");
        transform.position = new Vector3(transform.position.x, (float)minY, transform.position.z);
        mydY = .0005f;
        minY = 1.7059999704361;
    }

}
