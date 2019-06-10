using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance = null;

    List<GameObject> listTeam1;
    List<GameObject> listTeam2;
    List<GameObject> listTeam1ScoreDisplayer;
    List<GameObject> listTeam2ScoreDisplayer;

    public GameObject ScoreMenu;
    public GameObject NPChandler;
    public GameObject ScoreDisplayer;

    bool bDisable = true;
    bool bLoadlist = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    public void Start()
    {
        
    }

    public void LoadList()
    {
        for (int i = 0; i < NPChandler.GetComponent<NPCManager>().listNPCs.Count; i++)
        {           
            if (NPChandler.GetComponent<NPCManager>().listNPCs[i].GetComponent<Attribute>().Team == Attribute.eTeam.Team1)
            {
                listTeam1.Add(NPChandler.GetComponent<NPCManager>().listNPCs[i]);
                GameObject clone = Instantiate(ScoreDisplayer, this.transform.position + new Vector3(-161,90 - 30*listTeam1.Count,0), this.transform.rotation);
                clone.transform.SetParent(ScoreMenu.transform);
                //clone.transform.position = Camera.main.WorldToViewportPoint(new Vector3(-181, 60 + 0, 0));
                clone.GetComponent<Text>().text = NPChandler.GetComponent<NPCManager>().listNPCs[i].name;
                listTeam1ScoreDisplayer.Add(clone);
                
            }
            else
            {
                listTeam2.Add(NPChandler.GetComponent<NPCManager>().listNPCs[i]);
                GameObject clone = Instantiate(ScoreDisplayer, this.transform.position + new Vector3(161, 90 - 30 * listTeam2.Count, 0), this.transform.rotation);
                clone.transform.SetParent(ScoreMenu.transform);
                //clone.transform.position = new Vector3(92, 60 + 30 * listTeam2.Count, 0);
                clone.GetComponent<Text>().text = NPChandler.GetComponent<NPCManager>().listNPCs[i].name;
                listTeam2ScoreDisplayer.Add(clone);
            }
        }
    }

    private void Update()
    {
        if (!bDisable)
        {
            SortScore(listTeam1, listTeam1ScoreDisplayer);
            SortScore(listTeam2, listTeam2ScoreDisplayer);
        }
        //if (listTeam1.Count > 0)
        //    SapXepList(listTeam1);
        //if (listTeam2.Count > 0)
        //    SapXepList(listTeam2);
    }

    private void OnEnable()
    {
        bDisable = false;
        if (!bLoadlist)
        {           
            listTeam1 = new List<GameObject>();
            listTeam2 = new List<GameObject>();
            listTeam1ScoreDisplayer = new List<GameObject>();
            listTeam2ScoreDisplayer = new List<GameObject>();
            LoadList();
            bLoadlist = true;
        }
    }

    private void OnDisable()
    {
        bDisable = true;
    }


    void SortScore(List<GameObject> list, List<GameObject> scoreList)
    {
        List<GameObject> temp = new List<GameObject>();
        int j = 0;
        
        for (int i = 0; i < list.Count; i++)
        {
            for (j = 0; j < temp.Count; j++)
            {
                if (list[i].GetComponent<Attribute>().score < temp[j].GetComponent<Attribute>().score)
                    break;
            }
            temp.Insert(j, list[i]);
        }
        
        for (int i = 0; i < scoreList.Count; i++)
        {           
            scoreList[i].transform.GetChild(0).GetComponent<Text>().text = temp[temp.Count-i-1].GetComponent<Attribute>().score + " ";
            scoreList[i].GetComponent<Text>().text = temp[temp.Count - i-1].GetComponent<Attribute>().name;
        }
        
    }

    public void SapXepList(List<GameObject> list)
    {
        list.Sort(delegate (GameObject x, GameObject y)
        {
            if (x.GetComponent<Attribute>().score > y.GetComponent<Attribute>().score)
                return 1;
            else
                return -1;
        });
    }
}
