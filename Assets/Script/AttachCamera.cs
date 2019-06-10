using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachCamera : MonoBehaviour {

    public bool bAttach = true;
    public GameObject AttachObject;
   
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 position = transform.position;
        position.x = AttachObject.transform.position.x;
        position.y = AttachObject.transform.position.y;

        transform.position = position;
    }
}
