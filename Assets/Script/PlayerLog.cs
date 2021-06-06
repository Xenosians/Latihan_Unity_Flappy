using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GDC.Innovation.Utility;
public class PlayerLog : MonoBehaviour
{
 
    public static int tapCount;
    public int score;
    public DateTime time;
    private static PlayerLog instance;
    public static PlayerLog GetInstance()
    {
        return instance;
    }
    private void Awake(){
        instance = this;
    }
   
    public static int GetTapCount(){
        tapCount = 0;
        return tapCount;
    }
}
