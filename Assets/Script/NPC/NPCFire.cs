using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFire : MonoBehaviour {

    public GameObject Bullet;
    GameObject FirePos;

    public float cooldownTime = 0.3f;
    float countCooldown;

    public int maxAmmo = 30;
    int ammo;
    public float reloadCooldown = 1.0f;
    float reloadCountCooldown;

    //Degree
    public float maxDeviation = 2;
    float deviation = 0;
    public float deviationRate = 0.2f;
    public float deviationCooldown = 0.5f;
    float deviationCountCooldown;
    public bool bReloading;

    // Use this for initialization
    void Start() {
        FirePos = transform.GetChild(0).gameObject;
        countCooldown = cooldownTime;
        deviationCountCooldown = deviationCooldown;
        ammo = maxAmmo;
        reloadCountCooldown = reloadCooldown;
    }

    // Update is called once per frame
    void Update() {
        if (countCooldown > 0)
        {
            countCooldown -= Time.deltaTime;
        }

        if (deviationCountCooldown > 0)
        {
            deviationCountCooldown -= Time.deltaTime;
        }
        else
        {
            deviation = 0;
        }

        if (ammo == 0)
            Reload();
    }


    public void Fire()
    {
        if (countCooldown > 0 || ammo <= 0)
            return;

        deviationCountCooldown = deviationCooldown;

        if (deviation < maxDeviation)
            deviation += deviationRate;

        ammo--;

        countCooldown = cooldownTime;
        Quaternion rot = transform.rotation;
        rot = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, 90.0f + Random.Range(-deviation,deviation)));
        Instantiate(Bullet, FirePos.transform.position, rot);
    }

    public void Reload()
    {
        bReloading = true;
        if (reloadCountCooldown > 0)
        {
            reloadCountCooldown -= Time.deltaTime;
            return;
        }
        bReloading = false;
        reloadCountCooldown = reloadCooldown;
        ammo = maxAmmo;
    }
}
