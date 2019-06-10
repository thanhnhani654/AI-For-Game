using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public GameObject Bullet;
    public GameObject Owner;
    public enum eWeaponType { Rifle, Gun, Shotgun, none}
    //GameObject FirePos;

    [System.Serializable]
    public struct sWeapon
    {
        //info
        [SerializeField]
        public string name;
        [SerializeField]
        public eWeaponType type;

        //On Editor
        public int maxAmmo;        
        public float cooldownTime;
        public float reloadCooldown;
        public float maxDeviation;        
        public float deviationRate;
        public float deviationCooldown;        
        public float shotgunBulletsInTime;
        public float damage;

        //Private
        [HideInInspector]
        public bool bReloading;
        [HideInInspector]
        public int ammo;
        [HideInInspector]
        public float countCooldown;
        [HideInInspector]
        public float deviation;
        [HideInInspector]
        public float deviationCountCooldown;
        [HideInInspector]
        public float reloadCountCooldown;
        
    }

    sWeapon weapon;
    
    public eWeaponType type = eWeaponType.Rifle;

    public List<sWeapon> ListWeapon;

    public void SetWeaponWithType(eWeaponType iType)
    {
        type = iType;
        for (int i = 0; i < ListWeapon.Count; i++)
        {
            if (ListWeapon[i].type == type)
            {
                weapon = ListWeapon[i];
                return;
            }
        }
    }

    public void Fire()
    {
        
        if (type == eWeaponType.Shotgun)
        {
            
            ShotgunFire();
        }
        else
        {
            
            RifleAndGunFire();
        }
    }

    void Update()
    {
        if (type == eWeaponType.Shotgun)
        {
            ShotgunUpdate();
        }
        else
        {
            RifleUpdate();
        }

        if (transform.parent != null)
            this.transform.rotation = transform.parent.rotation;
    }

    void RifleAndGunFire()
    {
        if (weapon.countCooldown > 0 || weapon.ammo <= 0)
            return;
       
        weapon.ammo--;

        weapon.countCooldown = weapon.cooldownTime;

        Quaternion rot = Deviation();
        GameObject clone = Instantiate(Bullet, this.transform.position, rot);
        clone.GetComponent<Bullet>().SetDamage(weapon.damage);
        clone.GetComponent<Bullet>().owner = Owner;
    }

    void ShotgunFire()
    {       
        if (weapon.countCooldown > 0 || weapon.ammo <= 0)
            return;
        
        weapon.ammo--;
        weapon.countCooldown = weapon.cooldownTime;
       
        for (int i = 0; i < weapon.shotgunBulletsInTime; i++)
        {
            Quaternion rot = Deviation();
            GameObject clone = Instantiate(Bullet, this.transform.position, rot);
            clone.GetComponent<Bullet>().SetDamage(weapon.damage);
            clone.GetComponent<Bullet>().owner = Owner;
        }
    }

    Quaternion Deviation()
    {
        weapon.deviationCountCooldown = weapon.deviationCooldown;
        if (weapon.deviation < weapon.maxDeviation)
            weapon.deviation += weapon.deviationRate;
        Quaternion rot = transform.rotation;
        rot = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, 90.0f + Random.Range(-weapon.deviation, weapon.deviation)));
        return rot;
    }

    void RifleUpdate()
    {
        if (weapon.countCooldown >= 0)
        {
            weapon.countCooldown -= Time.deltaTime;
        }

        if (weapon.deviationCountCooldown >= 0)
        {
            weapon.deviationCountCooldown -= Time.deltaTime;
        }
        else
        {
            weapon.deviation = 0;
        }

        if (weapon.ammo == 0)
            Reload();
    }

    void ShotgunUpdate()
    {
        if (weapon.countCooldown >= 0)
        {
            weapon.countCooldown -= Time.deltaTime;
        }

        if (weapon.deviationCountCooldown >= 0)
        {
            weapon.deviationCountCooldown -= Time.deltaTime;
        }
        else
        {
            weapon.deviation = 0;
        }

        if (weapon.ammo == 0)
            Reload();
    }

    public void Reload()
    {
        weapon.bReloading = true;
        if (weapon.reloadCountCooldown > 0)
        {
            weapon.reloadCountCooldown -= Time.deltaTime;
            return;
        }
        weapon.bReloading = false;
        weapon.reloadCountCooldown = weapon.reloadCooldown;
        weapon.ammo = weapon.maxAmmo;
    }



}
