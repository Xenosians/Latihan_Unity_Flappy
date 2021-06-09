using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;
using System;

using GDC.Innovation.Utility;

public class GameHandler : MonoBehaviour
{

    private static GameHandler instance;
    public static GameHandler GetInstance()
    {
        return instance;
    }

    [SerializeField]private PlayerLog playerLog;

    private void Start(){
        Debug.Log("GameHandler.start");

        instance = this;

        Score.Start();
    }

    public void IncreaseTapCount() 
    {
        playerLog.tapCount++;
    }

    public void IncreaseScore()
    {
        playerLog.score++;
    }

    public void SetTime(TimeSpan time)
    {
        playerLog.time = time;
    }

    public void SaveLog() 
    {
        string json = GDCUtils.ConvertObjectToJsonString<PlayerLog>(playerLog);

        GDCUtils.SaveFileToLocation(DateTime.Now.Ticks.ToString()+".json" , json);
    }

}
