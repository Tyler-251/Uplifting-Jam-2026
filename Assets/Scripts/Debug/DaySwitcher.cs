using UnityEngine;
using UnityEngine.SceneManagement;

public class DaySwitcher : MonoBehaviour
{
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
}
