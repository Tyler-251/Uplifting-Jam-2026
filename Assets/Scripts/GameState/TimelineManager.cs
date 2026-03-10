using Unity.VisualScripting;
using UnityEngine;

public class TimelineManager : MonoBehaviour
{
    public PlayerSaveData saveData;
    void Start()
    {
        saveData = PlayerSaveDataManager.instance.LoadPlayerData();
        switch (saveData.currentDay)
        {
            case 0:
                saveData.currentDay = 1; // Progress to day 1 on first run
                StartDayOne();
                break;
            case 1:
                StartDayOne();
                break;
            case 2:
                
                break;
            default:
                // Handle unexpected day value
                break;
        }
    }
    
    void Update()
    {
        switch (saveData.currentDay)
        {
            case 1:
                CheckDayOne();
                break;
            case 2:
                // Check conditions for day two progression
                break;
            default:
                // Handle other days or unexpected values
                break;
        }
    }

    [Header("Day One Dialogue")]
    [SerializeField] private MessageSO dayOneIntro;
    private void StartDayOne()
    {
        PlayerSaveDataManager.instance.SavePlayerData(saveData);
        DialogueManager.instance.PlayMessage(dayOneIntro);
        DialogueManager.instance.onDialogueSequenceComplete.AddListener(() =>
        {
            Debug.Log("Dialogue sequence complete for Day One.");
        });
    }

    private void CheckDayOne()
    {
        
    }

    [Header("Day Two Dialogue")]
    [SerializeField] private MessageSO dayTwoIntro;
    private void StartDayTwo()
    {
        PlayerSaveDataManager.instance.SavePlayerData(saveData);
        DialogueManager.instance.PlayMessage(dayTwoIntro);
    }
    private void CheckDayTwo()
    {
        
    }
}
