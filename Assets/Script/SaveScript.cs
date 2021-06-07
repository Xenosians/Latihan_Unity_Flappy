using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDC.Innovation.Utility;


public class SaveScript
{
    public System.TimeSpan time;
    public int thisIsInt = 0;
    public float thisIsFloat = 5;
}


public class Main : MonoBehaviour
{   
    private System.TimeSpan GetTime(){
        return Level.GetInstance().GetTime();
    }
    private int GetTapCountSave(){
        return PlayerLog.GetTapCount();
    }

    private int GetHighscore(){
        return Score.GetHighscore();
    }
    private void Start(){
  
        SaveScript save = new SaveScript();
        string json = GDCUtils.ConvertObjectToJsonString<SaveScript>(save);
        save.thisIsFloat = GetTapCountSave();
        save.time = GetTime();
        save.thisIsInt = GetHighscore();

        GDCUtils.SaveFileToLocation(json, "C:/SERBASERBILAB/LAB_RPLGDC/SaveState/Save.json");
    }

}

