using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalControl : MonoBehaviour
{
    public static GlobalControl Instance;
    public float SoPlayerTeam1;
    public float SoPlayerTeam2;

    public bool bFriendlyFire;

    public bool bPlayerTeam1;
    public NPCController.eNPCLevel level;
    public float winKills;

    public bool team1Won;

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
}
