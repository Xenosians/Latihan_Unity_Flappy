using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDC.Innovation.Utility;


public class SaveScript
{
    public string thisIsString = "Try";
    public float thisIsFloat = 5;
}


public class Main : MonoBehaviour
{   
    private void Start(){
        //PlayerLog.GetInstance().GetTapCount();
        SaveScript Save = new SaveScript();
        string json = GDCUtils.ConvertObjectToJsonString<SaveScript>(Save);

        GDCUtils.SaveFileToLocation(json, "C:/SERBASERBILAB/LAB_RPLGDC/SaveState/Save.json");
    }

}

