using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonBehavior : MonoBehaviour
{
    PlayerSaveData saveData;
    [SerializeField] private GameObject continueButton;
    void Start()
    {
        saveData = PlayerSaveDataManager.instance.LoadPlayerData();
        if (saveData != null && saveData.currentDay > 1 && saveData.currentDay < 18255 && continueButton != null)
        {
            Debug.Log("Continue button enabled. Current day: " + saveData.currentDay);
            continueButton.SetActive(true);
        }
        else
        {
            Debug.Log("Continue button disabled. No valid save data found or current day is not in the expected range.");
            continueButton.SetActive(false);
        }

    }
    public GameObject settingsPanel;


    public void OpenGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OpenNewGame()
    {
        PlayerSaveDataManager.instance.SavePlayerData(new PlayerSaveData()); // Clear save data by saving a new instance
        SceneManager.LoadScene("GameScene");
    }

    public void OpenSavedGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }
}
