using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;
using System.Threading.Tasks;
using UnityEditor;
using System.IO;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }

    // Desktop
    private string saveFolderPath = "C:\\Users\\ParkSungJin\\Desktop\\SaveFiles";
    
    // Laptop
    //private string saveFolderPath = "C:\\Users\\PSJ\\Desktop\\SaveFile";



    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        //saveFolderPath = Application.persistentDataPath;
        //LoadSaveFiles();
    }

    public void SaveGame()
    {
        SaveData saveData = new SaveData
        {
            player = new PlayerData(GameManager.Instance.Player),
            inventory = ItemManager.Instance.Inventory.ToDictionary(i => i.Key, i => i.Value.Count),
            dungeonProgress = new ProgressGameData(GameManager.Instance.CurCount, GameManager.Instance.Boss.IsDead)
        };

        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);

        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string saveFilePath = Path.Combine(saveFolderPath, $"Save_{timestamp}.json");
        File.WriteAllText(saveFilePath, json);

        Debug.Log("게임 저장 완료: " + saveFolderPath);
    }

    public void LoadSaveFiles(Button[] buttons)
    {
        // 저장된 JSON 파일 목록 가져오기
        string[] files = Directory.GetFiles(saveFolderPath, "*.json");
        int count = 0;
        foreach (string file in files)
        {
            FileInfo fileInfo = new FileInfo(file);
            string fileName = Path.GetFileNameWithoutExtension(fileInfo.Name); // 확장자 제거
            //string lastModified = fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"); // 저장 시간 포맷

            buttons[count].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = fileName;
            buttons[count].onClick.AddListener(() => { LoadDataFromJson(fileInfo.FullName); });
            count++;
        }
        Canvas.ForceUpdateCanvases();
    }

    public void LoadDefaultDataFromJson()
    {
        Debug.Log("🔄 기본 데이터 로드 시작...");

        SettingData setting = new SettingData();

        // 파일 로드 확인
        TextAsset textAsset = Resources.Load<TextAsset>("Json/items");

        string json = textAsset.text;

        try
        {
            setting.itemInfo.data = JsonConvert.DeserializeObject<Dictionary<string, ItemData>>(json);
            if (setting.itemInfo.data == null)
            {
                Debug.LogError("⚠️ JSON 변환 실패: 변환된 데이터가 NULL입니다!");
                return;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"⚠️ JSON 변환 중 오류 발생: {e.Message}");
            return;
        }

        // 초기화 방지
        setting.itemInfo.data = setting.itemInfo.data ?? new Dictionary<string, ItemData>();

        // 아이템 정보 로드
        ItemManager.Instance.LoadItemsInfoFromJson(setting.itemInfo);
    }


    void LoadDataFromJson(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("저장된 데이터가 없습니다.");
            return;
        }
        // 플레이어 정보 로드
        string json = File.ReadAllText(filePath);
        SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json);

        GameManager.Instance.Player.LoadPlayerData(saveData.player);
        ItemManager.Instance.LoadInventoryData(saveData.inventory);
        GameManager.Instance.LoadDungeonData(saveData.dungeonProgress);

        GameManager.Instance.MoveTown();
        GameManager.Instance.IsLoadData = true;
    }
}

[System.Serializable]
public class SettingData
{
    public SettingData()
    {
        itemInfo = new ItemInfo();
    }
    public ItemInfo itemInfo;
}


[System.Serializable]
public class SaveData
{
    public PlayerData player;
    public Dictionary<string, int> inventory;
    public ProgressGameData dungeonProgress;
}

[System.Serializable]
public class PlayerData
{
    public string name;
    public int level;
    public int exp;
    public int maxExp;
    public int hp;
    public int maxHp;
    public int attackPower;

    public PlayerData() { }

    public PlayerData(Player player)
    {
        name = player.Name;
        level = player.Level;
        exp = player.Exp;
        maxExp = player.MaxExp;
        hp = player.Hp;
        maxHp = player.MaxHp;
        attackPower = player.AttackPower;
    }
}

[System.Serializable]
public class ProgressGameData
{
    public int currentFloor; 
    public bool bossDefeated;

    public ProgressGameData(int floor, bool b) 
    {
        currentFloor = floor;
        bossDefeated = b;
    }
}

[System.Serializable]
public class ItemInfo
{
    public Dictionary<string, ItemData> data;
}


