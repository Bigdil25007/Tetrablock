using System.IO;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public enum Mode
{
    ModeClassique,
    ModeRetro,
    ModeZen,
    ModeBonus
}
[System.Serializable]


public class SaveData : MonoBehaviour
{
    public class GameData
    {
        public Mode mode;
        public bool HideGhost;
        public Dictionary<string, KeyCode> DicKeyCode;
        public List<int> scoresList;

        public GameData(Mode mode, bool hideGhost, Dictionary<string, KeyCode> dicKeyCode, List<int> scoresList)
        {
            this.mode = mode;
            this.HideGhost = hideGhost;
            this.DicKeyCode = dicKeyCode;
            this.scoresList = scoresList;
        }

        public string ToSaveString()
        {
            string saveString = "";

            foreach (var kvp in DicKeyCode)
            {
                saveString += kvp.Key + "=" + kvp.Value.ToString() + "\n";
            }

            saveString += "HideGhost=" + HideGhost.ToString() + "\n";

            if (mode == Mode.ModeRetro)
            {
                saveString += "RetroScores=" + string.Join(",", scoresList.Select(score => score.ToString()).ToArray()) + "\n";
            }
            else if (mode == Mode.ModeClassique)
            {
                saveString += "ClassiqueScores=" + string.Join(",", scoresList.Select(score => score.ToString()).ToArray()) + "\n";
            }
            else if (mode == Mode.ModeBonus)
            {
                saveString += "BonusScores=" + string.Join(",", scoresList.Select(score => score.ToString()).ToArray()) + "\n";
            }

            return saveString;
        }

        public static GameData FromSaveString(string saveString)
        {
            GameData gameData = new GameData(Mode.ModeClassique, false, new Dictionary<string, KeyCode>(), new List<int>());

            string[] lines = saveString.Split('\n');

            foreach (string line in lines)
            {
                string[] parts = line.Split('=');

                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();

                    if (key.StartsWith("Mov") || key.StartsWith("Rot") || key.StartsWith("Desc") || key.StartsWith("Plac") || key.StartsWith("MovHaut"))
                    {
                        gameData.DicKeyCode[key] = SaveData.ToKeyCode(value);
                    }
                    else if (key == "HideGhost")
                    {
                        gameData.HideGhost = SaveData.ToBool(value);
                    }
                    else if (key == "RetroScores" && gameData.mode == Mode.ModeRetro)
                    {
                        gameData.scoresList = value.Split(',').Select(int.Parse).ToList();
                    }
                    else if (key == "ClassiqueScores" && gameData.mode == Mode.ModeClassique)
                    {
                        gameData.scoresList = value.Split(',').Select(int.Parse).ToList();
                    }
                    else if (key == "BonusScores" && gameData.mode == Mode.ModeBonus)
                    {
                        gameData.scoresList = value.Split(',').Select(int.Parse).ToList();
                    }
                }
            }

            return gameData;
        }
    }

    private static SaveData _instance;
    public Mode mode;
    public static SaveData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SaveData>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("SaveData");
                    _instance = singletonObject.AddComponent<SaveData>();
                    DontDestroyOnLoad(singletonObject);
                }
            }

            return _instance;
        }
    }

    public bool HideGhost = false;
    private string PathFile = Application.dataPath + "/data.txt";

    [SerializeField]
    public static KeyCode MovGauche { get; private set; } = KeyCode.Q;
    public static KeyCode MovDroite { get; private set; } = KeyCode.D;

    public static KeyCode MovBas { get; private set; } = KeyCode.S;
    public static KeyCode RotGauche { get; private set; } = KeyCode.A;
    public static KeyCode RotDroite { get; private set; } = KeyCode.E;
    public static KeyCode DescInstante { get; private set; } = KeyCode.Space;
    public static KeyCode PlacReserve { get; private set; } = KeyCode.C;
    public static KeyCode MovHaut { get; private set; } = KeyCode.Z;

    public Dictionary<string, KeyCode> DicKeyCode = new Dictionary<string, KeyCode>()
    {
        {"MovGauche",      MovGauche},
        {"MovDroite",      MovDroite},
        {"RotGauche",      RotGauche},
        {"RotDroite",      RotDroite},
        {"MovBas",         MovBas},
        {"DescInstante",   DescInstante},
        {"PlacReserve",    PlacReserve},
        {"MovHaut",        MovHaut}
    };

    public string[] keys;
    public KeyCode[] values;

    public Text scoreListText;
    private List<int> scoresList = new List<int>();

    string SaveStrRetro;
    string SaveStrBonus;  // Remplacé "SaveStrZen" par "SaveStrBonus"
    string SaveStrClassique;
    private GameObject[] objectsWithTag;

    private GameData gameData;

    void Start()
    {
        if (File.Exists(PathFile))
        {
            Load();
        }
        else
        {
            Save();
        }

        objectsWithTag = GameObject.FindGameObjectsWithTag("Ghost");
        ToggleGhostObjects();

        keys = gameData.DicKeyCode.Keys.ToArray();
        values = gameData.DicKeyCode.Values.ToArray();
    }


    public KeyCode ToKeyCode(string KeyStr)
    {
        try
        {
            KeyCode result = (KeyCode)Enum.Parse(typeof(KeyCode), KeyStr, true);
            return result;
        }
        catch (ArgumentException)
        {
            return KeyCode.Space;
        }
    }

    public void isClick()
    {
        HideGhost = !HideGhost;
        Save();
        ToggleGhostObjects();
    }

    void ToggleGhostObjects()
    {
        foreach (GameObject obj in objectsWithTag)
        {
            if (obj != null)
            {
                obj.SetActive(!HideGhost);
            }
        }
    }

    public void AddScore(int newScore)
    {
        scoresList.Add(newScore);
        scoresList = scoresList.OrderByDescending(score => score).ToList();
        UpdateScoreListText();
        scoreListText.gameObject.SetActive(true);
    }

    private void UpdateScoreListText()
    {
        string scoreText = "High Scores:\n\n";
        while (scoresList.Count < 10)
        {
            scoresList.Add(0);
        }

        for (int i = 0; i < 10; i++)
        {
            scoreText += (i + 1) + ". " + scoresList[i] + "\n";
        }

        scoreListText.text = scoreText;
    }

    public void HideScoreTable()
    {
        scoreListText.gameObject.SetActive(false);
    }

    public void Save()
    {
        gameData = new GameData(mode, HideGhost, DicKeyCode, scoresList);

        string saveString = gameData.ToSaveString();
        File.WriteAllText(PathFile, saveString);
    }

    public void Load()
    {
        if (File.Exists(PathFile))
        {
            string saveString = File.ReadAllText(PathFile);
            gameData = GameData.FromSaveString(saveString);

            // Mettre à jour les membres de SaveData avec les données chargées
            mode = gameData.mode;
            HideGhost = gameData.HideGhost;
            DicKeyCode = gameData.DicKeyCode;
            scoresList = gameData.scoresList;

            ToggleGhostObjects();
        }
    }


    public bool ToBool(string KeyStr)
    {
        if (KeyStr == "True")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
