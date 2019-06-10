using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    CharacterInputManage _CI;
    CharacterMovement characterMovement;
    CharacterFire characterFire;

    public GameObject gunMenu;
    public GameObject ThongKeMenu;
    public GameObject PasueMenu;
    public GameObject EndGame;

    bool bToogleGunMenu = false;
    bool bGameWon = false;

    void Start()
    {
        _CI = GetComponent<CharacterInputManage>();
        characterMovement = GetComponent<CharacterMovement>();
        characterFire = GetComponent<CharacterFire>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(_CI.GetWeapon))
        {
            ToogleGunMenu();
        }

        if (Input.GetKeyDown(_CI.ThongKe) && !bGameWon)
        {
            if (ThongKeMenu.active)
                ThongKeMenu.SetActive(false);
            else
                ThongKeMenu.SetActive(true);
        }
        
        if (Input.GetKeyDown(_CI.PauseGame))
        {
            if (Time.timeScale == 1)
            {
                PasueMenu.SetActive(true);
                Time.timeScale = 0;               
                DisableController();
            }
            else
            {
                Time.timeScale = 1;
                PasueMenu.SetActive(false);
                ActiveController();
            }
        }
    }

    void DisableController()
    {
        characterFire.bDisable = true;
        characterMovement.bDisable = true;
    }

    void ActiveController()
    {
        characterFire.bDisable = false;
        characterMovement.bDisable = false;
    }

    void TurnOnGunMenu()
    {
        bToogleGunMenu = true;
        DisableController();
        gunMenu.SetActive(true);
    }

    void TurnOffGunMenu()
    {
        bToogleGunMenu = false;
        ActiveController();
        gunMenu.SetActive(false);
    }

    public void ToogleGunMenu()
    {
        if (bToogleGunMenu)
            TurnOffGunMenu();
        else
            TurnOnGunMenu();
    }

    public void Resume()
    {
        Time.timeScale = 1;
        PasueMenu.SetActive(false);
        ActiveController();

    }

    public void Exit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MenuScene");
    }

    public void GameWon(bool bTeam1Won)
    {
        if (bTeam1Won)
        {
            ThongKeMenu.transform.GetChild(0).GetComponent<Text>().text = "Team 1 Won";
        }
        else
        {
            ThongKeMenu.transform.GetChild(0).GetComponent<Text>().text = "Team 2 Won";
        }

        Time.timeScale = 0;
        DisableController();
        bGameWon = true;
        ThongKeMenu.SetActive(true);
        EndGame.SetActive(true);

    }

}
