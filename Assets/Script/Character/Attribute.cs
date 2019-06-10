using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Attribute : MonoBehaviour {

    public enum eTeam { Team1, Team2}

    public float maxHP = 100;
    public float hp;

    public float damage = 5;

    public bool bAlive = true;
    public float speed = 0.5f;

    public Text hpText;

    public eTeam Team;
    public int score;
    public string name;

    void Start()
    {
        hp = maxHP;

        if (hpText != null)
            hpText.text = "HP: " + hp + "/" + maxHP;
        score = 0;
    }

    public void DoDamage(float dmg)
    {
        hp -= dmg;

        if (this.GetComponent<Animation>() != null)
            this.GetComponent<Animation>().Play("RedFlash");

        if (hp <= 0)
        {
            hp = 0;
            bAlive = false;
            this.gameObject.SetActive(false);
        }

        if (hpText != null)
            hpText.text = "HP: " + hp + "/" + maxHP;
    }

    public void ReSpawn()
    {
        hp = maxHP;

        if (hpText != null)
            hpText.text = "HP: " + hp + "/" + maxHP;

        bAlive = true;
        this.gameObject.SetActive(true);
    }

}
