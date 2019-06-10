using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    [Range(0.01f,1000)]
    public float Speed = 0.01f;

    public float damage = 5;

    public LayerMask obstacleMask;

    float angle;
    Vector3 myPos;

    public float _TimetoDestroy = 2.0f;

    public GameObject owner;

    // Use this for initialization
    private void OnEnable()
    { 
        StartCoroutine(AutoDestroy(_TimetoDestroy));
	}
	
    void Start()
    {

    }

	// Update is called once per frame
	void Update () {
        myPos = transform.position;
        angle = transform.eulerAngles.magnitude * Mathf.Deg2Rad;
        myPos.x += (Mathf.Cos(angle) * Speed) * Time.deltaTime;
        myPos.y += (Mathf.Sin(angle) * Speed) * Time.deltaTime;

        transform.position = myPos;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Character")
        {
            col.gameObject.GetComponent<Attribute>().DoDamage(damage);
            if (!col.gameObject.GetComponent<Attribute>().bAlive)
            {
                //Debug.Log("AA");
                owner.GetComponent<Attribute>().score++;
            }
        }
        if (col.gameObject.tag != "Bullet")
            Destroy(this.gameObject);
    }

    IEnumerator AutoDestroy(float _time)
    {
        yield return new WaitForSeconds(_time);

        Destroy(gameObject);
    }

    public void SetDamage(float dmg)
    {
        damage = dmg;
    }
}
