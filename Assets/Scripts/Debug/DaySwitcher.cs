using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DaySwitcher : MonoBehaviour
{
    [SerializeField] List<UpgradeSO> dayTwoUpgrades; 

    //Clear PlayerPrefs only
    [ContextMenu("Clear PlayerPrefs")]
    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("PlayerPrefs cleared.");
    }
    //Switch to day 1
    [ContextMenu("Switch to Day One")]
    public void SwitchToDayOne()
    {
        PlayerSaveData saveData = PlayerSaveDataManager.instance.LoadPlayerData();
        saveData.currentDay = 1;
        PlayerSaveDataManager.instance.SavePlayerData(saveData);
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    //Switch to day 2
    [ContextMenu("Switch to Day Two")]
    public void SwitchToDayTwo()
    {
        PlayerSaveData saveData = PlayerSaveDataManager.instance.LoadPlayerData();
        saveData.currentDay = 2;
        PlayerSaveDataManager.instance.SavePlayerData(saveData);
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    // Switch to day 3 with all day 2 upgrades for testing
    [ContextMenu("Switch to Day Three")]
    public void SwitchToDayThree()
    {
        PlayerSaveData saveData = PlayerSaveDataManager.instance.LoadPlayerData();
        saveData.currentDay = 3;
        saveData.progressionTags = new List<string>();
        saveData.unlockedUpgrades = new List<UpgradeSO>(dayTwoUpgrades); // Simulate having all day two upgrades for testing day three
        PlayerSaveDataManager.instance.SavePlayerData(saveData);
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    [ContextMenu("Switch to Day Four")]
    public void SwitchToDayFour()
    {
        PlayerSaveData saveData = PlayerSaveDataManager.instance.LoadPlayerData();
        saveData.currentDay = 4;
        saveData.progressionTags = new List<string>();
        saveData.unlockedUpgrades = new List<UpgradeSO>(dayTwoUpgrades);
        PlayerSaveDataManager.instance.SavePlayerData(saveData);
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    [ContextMenu("Switch to Day Five")]
    public void SwitchToDayFive()
    {
        PlayerSaveData saveData = PlayerSaveDataManager.instance.LoadPlayerData();
        saveData.currentDay = 5;
        saveData.progressionTags = new List<string>();
        saveData.unlockedUpgrades = new List<UpgradeSO>(dayTwoUpgrades);
        PlayerSaveDataManager.instance.SavePlayerData(saveData);
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    [ContextMenu("Switch to Day 18255")]
    public void SwitchToDay18255()
    {
        PlayerSaveData saveData = PlayerSaveDataManager.instance.LoadPlayerData();
        saveData.currentDay = 18255;
        saveData.progressionTags = new List<string>();
        saveData.unlockedUpgrades = new List<UpgradeSO>(dayTwoUpgrades);
        PlayerSaveDataManager.instance.SavePlayerData(saveData);
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }
}
