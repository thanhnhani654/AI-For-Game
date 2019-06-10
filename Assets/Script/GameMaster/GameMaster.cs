using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour {

	public enum Mode { DeathMath};
    Mode gameMode = Mode.DeathMath;

    [System.Serializable]
    public struct sDeathMatch
    {
        public int team1Score;
        public int team2Score;
        public int winScore;
        public Text team1Text;
        public Text team2Text;
        public Text winText;
    }

    public GameObject NPCHandler;
    public GameObject character;

    public List<GameObject> characterSpawn;

    public sDeathMatch modeDM;

    public bool bFirstSpawnChar = true;

    void Start()
    {
        //modeDM = new sDeathMatch();
        if (GlobalControl.Instance != null)
        {
            modeDM.winScore = (int)GlobalControl.Instance.winKills;
        }

        if (gameMode == Mode.DeathMath)
        {
            modeDM.winText.text = "Win Score: " + modeDM.winScore;
            modeDM.team1Text.text = "Team 1: " + modeDM.team1Score;
            modeDM.team2Text.text = "Team 2: " + modeDM.team2Score;

        }
    }

    void Update()
    {       
        if (gameMode == Mode.DeathMath)
        {
            UpdateDeathMatch();
        }
    }

    void UpdateDeathMatch()
    {
        List<GameObject> listNPCs = NPCHandler.GetComponent<NPCManager>().listNPCs;

        if (modeDM.team1Score < modeDM.winScore && modeDM.team2Score < modeDM.winScore)
        {
            for (int i = 0; i < characterSpawn.Count; i++)
            {
                for (int j = 0; j < listNPCs.Count; j++)
                {
                    if (listNPCs[j].GetComponent<Attribute>().Team != characterSpawn[i].GetComponent<Spawner>().team)
                        continue;

                    if (listNPCs[j].GetComponent<NPCController>().directSpawn)
                    {
                        if (!characterSpawn[i].GetComponent<Spawner>().DirectSpawn(listNPCs[j]))
                            continue;
                        listNPCs[j].GetComponent<NPCController>().directSpawn = false;
                        continue;
                    }

                     if (listNPCs[j].GetComponent<Attribute>().bAlive)
                        continue;

                    if (!characterSpawn[i].GetComponent<Spawner>().spawning)
                    {
                        characterSpawn[i].GetComponent<Spawner>().SetTarget(listNPCs[j]);
                        characterSpawn[i].GetComponent<Spawner>().Spawn();
                        if (listNPCs[j].GetComponent<Attribute>().Team == Attribute.eTeam.Team1)
                            modeDM.team2Score++;
                        else
                            modeDM.team1Score++;
                    }
                }

                if (character.GetComponent<Attribute>().Team != characterSpawn[i].GetComponent<Spawner>().team)
                    continue;

                if (bFirstSpawnChar)
                {
                    characterSpawn[i].GetComponent<Spawner>().SetTarget(character);
                    characterSpawn[i].GetComponent<Spawner>().DirectSpawnCharacter();
                    bFirstSpawnChar = false;
                    continue;
                }

                if (!character.GetComponent<Attribute>().bAlive)
                {
                    if (!characterSpawn[i].GetComponent<Spawner>().spawning)
                    {
                        characterSpawn[i].GetComponent<Spawner>().SetTarget(character);
                        characterSpawn[i].GetComponent<Spawner>().CharacterSpawn();
                        if (character.GetComponent<Attribute>().Team == Attribute.eTeam.Team1)
                            modeDM.team2Score++;
                        else
                            modeDM.team1Score++;
                    }
                }
            }     
        }

       
        modeDM.team1Text.text = "Team 1: " + modeDM.team1Score;
        modeDM.team2Text.text = "Team 2: " + modeDM.team2Score;

        if (modeDM.team1Score == modeDM.winScore)
        {
            character.GetComponent<GameHandler>().GameWon(true);
            NPCHandler.SetActive(false);
        }
        else if (modeDM.team2Score == modeDM.winScore)
        {
            character.GetComponent<GameHandler>().GameWon(false);
            NPCHandler.SetActive(false);
        }
    }


}
