using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SaveData : MonoBehaviour{

    public bool SaveButton = false;

    //public List<KeyCode> ListKeyCode= new List<KeyCode>();
    public static KeyCode G = KeyCode.G;
    public static KeyCode D = KeyCode.D;
    public static KeyCode S = KeyCode.S;
    public static KeyCode A = KeyCode.A;
    public static KeyCode E = KeyCode.E;
    public static KeyCode C = KeyCode.C;
    public static KeyCode Space = KeyCode.Space;
    private KeyCode[] TabKeyCode = { G, D, S, A, E, C, Space };
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M)){
            Save();
            
        }
        //if(Input.GetKeyDown(KeyCode.L)){
        //    Load();
        //}

        
    }
    void Start()
    {
        Load();
    }
    public KeyCode ToKeyCode(string KeyStr)
    {
        return (KeyCode)System.Enum.Parse(typeof(KeyCode), KeyStr, true);
    }

    void Save(){
        //string strTouches= new string;
        
        string[] touchesStr = new string[TabKeyCode.Length];
        for (int i = 0; i < TabKeyCode.Length; i++)
        {
            touchesStr[i] = TabKeyCode[i].ToString();
        }
        string SaveString = string.Join(", ",touchesStr);
        Debug.Log(SaveString);
        File.WriteAllText(Application.dataPath + "/data.txt",SaveString);
        Debug.Log("Sauvegarde effectuer");
    }
    void Load(){
        string SaveStringG = File.ReadAllText(Application.dataPath +"/data.txt");
        string[] content = SaveStringG.Split(new[] { ", " }, System.StringSplitOptions.None);
        for(int i = 0; i < content.Length; i++)
        {
            TabKeyCode[i] = ToKeyCode(content[i]);
        }
        Debug.Log("Chargement effectuer");
    }
}