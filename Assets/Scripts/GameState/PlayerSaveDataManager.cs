using System.Collections.Generic;
using UnityEngine;

public class PlayerSaveData
{
    public int xos;
    public int currentDay = 0;
    public List<UpgradeSO> unlockedUpgrades;
    public List<string> progressionTags;
    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
    public static PlayerSaveData FromJson(string json)
    {
        return JsonUtility.FromJson<PlayerSaveData>(json);
    }
}
public class PlayerSaveDataManager : MonoBehaviour
{
    public static PlayerSaveDataManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private PlayerSaveData currentSaveData = null;

    public void SavePlayerData(PlayerSaveData data)
    {
        currentSaveData = data;
        PlayerPrefs.SetString("PlayerSaveData", data.ToJson());
        PlayerPrefs.Save();
    }

    public PlayerSaveData LoadPlayerData()
    {
        if (PlayerPrefs.HasKey("PlayerSaveData"))
        {
            string json = PlayerPrefs.GetString("PlayerSaveData");
            currentSaveData = PlayerSaveData.FromJson(json);
            return currentSaveData;
        }
        return PlayerSaveData.FromJson(PlayerPrefs.GetString("PlayerSaveData", new PlayerSaveData().ToJson()));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
