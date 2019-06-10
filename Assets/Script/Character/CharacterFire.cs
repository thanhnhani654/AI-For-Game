using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;

public class CharacterFire : MonoBehaviour {

    CharacterInputManage _CI;
    public GameObject Bullet;
    GameObject FirePos;

    public Weapon.eWeaponType weaponType;
    public Weapon weapon;

    public bool bDisable = false;

    public Sprite[] sprites;

    void Start () {
        _CI = GetComponent<CharacterInputManage>();
        FirePos = transform.GetChild(0).gameObject;

        //GetWeapon
        weapon = this.transform.GetComponentInChildren<Weapon>();
        //weapon.SetWeaponWithType(weaponType);
        SetWeaponGun();
        weapon.Owner = this.transform.gameObject;
    }
	
	// Update is called once per frame
	void Update () {

        if (bDisable)
            return;

		if (Input.GetKey(_CI.Fire))
        {
            weapon.Fire();           
        }
	}

    public void SetWeaponGun()
    {
        weaponType = Weapon.eWeaponType.Gun;
        weapon.SetWeaponWithType(weaponType);
        if (this.GetComponent<Attribute>().Team == Attribute.eTeam.Team1)
            this.GetComponent<SpriteRenderer>().sprite = sprites[0];
        else
            this.GetComponent<SpriteRenderer>().sprite = sprites[3];
    }

    public void SetWeaponRifle()
    {
        weaponType = Weapon.eWeaponType.Rifle;
        weapon.SetWeaponWithType(weaponType);
        if (this.GetComponent<Attribute>().Team == Attribute.eTeam.Team1)
            this.GetComponent<SpriteRenderer>().sprite = sprites[1];
        else
            this.GetComponent<SpriteRenderer>().sprite = sprites[4];
    }

    public void SetWeaponShotGun()
    {
        weaponType = Weapon.eWeaponType.Shotgun;
        weapon.SetWeaponWithType(weaponType);
        if (this.GetComponent<Attribute>().Team == Attribute.eTeam.Team1)
            this.GetComponent<SpriteRenderer>().sprite = sprites[2];
        else
            this.GetComponent<SpriteRenderer>().sprite = sprites[5];
    }
}
