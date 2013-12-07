using UnityEngine;
using System.Collections;

public class CamControl : MonoBehaviour {

    public float camSpeed = 0.25f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += transform.right * Input.GetAxis( "Horizontal" ) * camSpeed;

    }
}
