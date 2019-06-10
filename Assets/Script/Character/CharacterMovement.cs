using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour {
    [Range(0, 1)]
    public float Speed = 0.04f;
    CharacterInputManage _CI;

    public bool bDisable = false;

    // Use this for initialization
    void Start () {
        _CI = GetComponent<CharacterInputManage>();
	}
	
	// Update is called once per frame
	void Update () {

        if (bDisable)
            return;

        CharacterMovementUpdate(_CI);

        Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
    }

    void CharacterMovementUpdate(CharacterInputManage _CI)
    {
        if (Input.GetKey(_CI.MoveUp))
        {
            Vector3 position = this.transform.position;
            position.y += Speed;
            this.transform.position = position;
        }

        if (Input.GetKey(_CI.MoveDown))
        {
            Vector3 position = this.transform.position;
            position.y -= Speed;
            this.transform.position = position;
        }

        if (Input.GetKey(_CI.MoveRight))
        {
            Vector3 position = this.transform.position;
            position.x += Speed;
            this.transform.position = position;
        }

        if (Input.GetKey(_CI.MoveLeft))
        {
            Vector3 position = this.transform.position;
            position.x -= Speed;
            this.transform.position = position;
        }
    }

    public void Disable()
    {
        
    }
}
