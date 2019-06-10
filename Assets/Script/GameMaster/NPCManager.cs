using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public static NPCManager instance = null;

    public List<GameObject> listNPCs;
    public GameObject dataGrid;
    public GameObject NPC;
    public GameObject team1Spawner;
    public GameObject team2Spawner;
    public float team1;
    public float team2;
    //public GameObject weaponData;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (this != instance)
            Destroy(gameObject);
    }

    public void CreateNPC(float team1Players, float team2Players, NPCController.eNPCLevel level = NPCController.eNPCLevel.Easy)
    {
        for (int i = 0; i < team1Players + team2Players; i++)
        {
            GameObject clone = Instantiate(NPC, this.transform.position, this.transform.rotation);
            clone.GetComponent<NPCController>().dataGrid = dataGrid;
            clone.GetComponent<NPCController>().directSpawn = true;           
            clone.GetComponent<Attribute>().name = "Bot " + (i + 1);
            clone.GetComponent<NPCController>().level = NPCController.eNPCLevel.Hard;
            clone.GetComponent<NPCController>().weaponType = Weapon.eWeaponType.Rifle;
            if (i < team1Players)
            {
                clone.GetComponent<Attribute>().Team = Attribute.eTeam.Team1;                
            }
            else
            {
                clone.GetComponent<Attribute>().Team = Attribute.eTeam.Team2;
            }
            listNPCs.Add(clone);
            clone.transform.SetParent(this.transform);
        }
        
        
    }

    private void Start()
    {
        if (GlobalControl.Instance != null)
        {
            //So luong NPC
            team1 = GlobalControl.Instance.SoPlayerTeam1;
            team2 = GlobalControl.Instance.SoPlayerTeam2;
            CreateNPC(team1, team2, GlobalControl.Instance.level);
        }
        else       
            CreateNPC(team1,team2);
    }
}
