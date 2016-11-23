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
        //if(Random.Range(0, 10) > 5)
        //    return;
        double dY = goingUp ? mydY : -mydY;
        transform.Translate(Vector3.up * (float)dY, Space.World);
        if(transform.position.y > maxY || transform.position.y < minY)
            goingUp = !goingUp;

    }
}
