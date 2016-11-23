using UnityEngine;
using System.Collections;

public class YNTD_Floating : MonoBehaviour {
    public float maxY = 0;
    public float minY = 0;
    public bool goingUp = true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //if(Random.Range(0, 10) > 5)
        //    return;
        float dY = goingUp ? .0005f : -.0005f;
        transform.Translate(Vector3.up * dY, Space.World);
        if(transform.position.y > maxY || transform.position.y < minY)
            goingUp = !goingUp;

    }
}
