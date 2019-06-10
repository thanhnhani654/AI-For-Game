using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    public GameObject ChonMap;
    public GameObject StartMenu;

    public Slider Team1;
    public Slider Team2;
    public Slider winKills;
    public NPCController.eNPCLevel level;
    public bool bFriendlyFire;
    public bool bPlayerTeam1;

    public Button PlayerTeam;
    public Button mapLevel;

    //Button Function
    public void NewGame()
    {
        ChonMap.SetActive(true);
        StartMenu.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        GlobalControl.Instance.SoPlayerTeam1 = Team1.value;
        GlobalControl.Instance.SoPlayerTeam2 = Team2.value;

        if (bPlayerTeam1)
            GlobalControl.Instance.SoPlayerTeam1 -= 1;
        else
            GlobalControl.Instance.SoPlayerTeam2 -= 1;

        GlobalControl.Instance.winKills = winKills.value;
        GlobalControl.Instance.bFriendlyFire = bFriendlyFire;
        //GlobalControl.Instance.bPlayerTeam1 = bPlayerTeam1;
        GlobalControl.Instance.level = level;

        Time.timeScale = 1;

        SceneManager.LoadScene("SampleScene");

    }

    public void PickTeam()
    {
        if (bPlayerTeam1)
            PlayerTeam.transform.GetChild(0).GetComponent<Text>().text = "Team 2";
        else
            PlayerTeam.transform.GetChild(0).GetComponent<Text>().text = "Team 1";
        bPlayerTeam1 = !bPlayerTeam1;
    }

    public void ChoseLevel()
    {
        switch (level)
        {
            case NPCController.eNPCLevel.Easy:
                mapLevel.transform.GetChild(0).GetComponent<Text>().text = "Normal";
                level = NPCController.eNPCLevel.Normal;
                break;
            case NPCController.eNPCLevel.Normal:
                mapLevel.transform.GetChild(0).GetComponent<Text>().text = "Hard";
                level = NPCController.eNPCLevel.Hard;
                break;
            case NPCController.eNPCLevel.Hard:
                mapLevel.transform.GetChild(0).GetComponent<Text>().text = "Easy";
                level = NPCController.eNPCLevel.Easy;
                break;
        }
    }

}