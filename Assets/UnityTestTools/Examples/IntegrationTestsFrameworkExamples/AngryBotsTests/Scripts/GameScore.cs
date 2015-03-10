using System;
using System.Collections.Generic;
using UnityEngine;

public class GameScore : MonoBehaviour
{
    static GameScore s_Instance;


    static GameScore Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = (GameScore)FindObjectOfType(typeof(GameScore));
            }

            return s_Instance;
        }
    }


    public void OnApplicationQuit()
    {
        s_Instance = null;
    }


    public string playerLayerName = "Player", enemyLayerName = "Enemies";


    int m_Deaths;
    readonly Dictionary<string, int> m_Kills = new Dictionary<string, int>();
    float m_StartTime;


    public static int Deaths
    {
        get
        {
            if (Instance == null)
            {
                return 0;
            }

            return Instance.m_Deaths;
        }
    }


    #if !UNITY_FLASH
    public static ICollection<string> KillTypes
    {
        get
        {
            if (Instance == null)
            {
                return new string[0];
            }

            return Instance.m_Kills.Keys;
        }
    }
    #endif  // if !UNITY_FLASH


    public static int GetKills(string type)
    {
        if (Instance == null || !Instance.m_Kills.ContainsKey(type))
        {
            return 0;
        }

        return Instance.m_Kills[type];
    }


    public static float GameTime
    {
        get
        {
            if (Instance == null)
            {
                return 0.0f;
            }

            return Time.time - Instance.m_StartTime;
        }
    }


    public static void RegisterDeath(GameObject deadObject)
    {
        if (Instance == null)
        {
            Debug.Log("Game score not loaded");
            return;
        }

        int
            playerLayer = LayerMask.NameToLayer(Instance.playerLayerName),
            enemyLayer = LayerMask.NameToLayer(Instance.enemyLayerName);

        if (deadObject.layer == playerLayer)
        {
            Instance.m_Deaths++;
        }
        else if (deadObject.layer == enemyLayer)
        {
            Instance.m_Kills[deadObject.name] = Instance.m_Kills.ContainsKey(deadObject.name) ? Instance.m_Kills[deadObject.name] + 1 : 1;
        }
    }


    public void OnLevelWasLoaded(int level)
    {
        if (m_StartTime == 0.0f)
        {
            m_StartTime = Time.time;
        }
    }
}
