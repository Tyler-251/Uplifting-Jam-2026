using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimelineManager : MonoBehaviour
{
    public static TimelineManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public PlayerSaveData saveData;
    [SerializeField] private CurtainBehavior curtainBehavior;
    [SerializeField] private TTTManager tttManager;
    [SerializeField] private List<GameObject> stuffToHideDayOne;
    [SerializeField] private VertBarBehavior vertBarBehavior;
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
                StartDayTwo();
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
                UpdateDayOne();
                break;
            case 2:
                UpdateDayTwo();
                break;
            default:
                // Handle other days or unexpected values
                break;
        }
    }

#region DayOne
    [Header("Day One Dialogue")]
    [SerializeField] private MessageSO dayOneIntro;
    [SerializeField] private MessageSO postFirstMatchMessage;
    [SerializeField] private MessageSO postDrawMessage;
    [SerializeField] private MessageSO postBestOfThreeMessageWin;
    [SerializeField] private MessageSO postBestOfThreeMessageLose;
    private void StartDayOne()
    {
        PlayerSaveDataManager.instance.SavePlayerData(saveData);
        curtainBehavior.OpenCurtain("Day One", "5:00 PM", dayOneIntro);
        foreach (var obj in stuffToHideDayOne)
        {
            obj.SetActive(false);
        }
        vertBarBehavior.DisableChildren();
        DialogueManager.instance.onDialogueSequenceComplete.AddListener(() =>
        {
            tttManager.gameObject.SetActive(true);
            tttManager.StartGame();
            MusicAudioManager.instance.FadeInMainMusic();
        });
    }

    //
    // Day One Update Flags
    //
    private bool dayOneFirstMatchCompleted = false;
    private bool dayOneDrawCompleted = false;
    private bool dayOneBestOfThreeCompleted = false;

    private void UpdateDayOne()
    {
        // Post Match messages
        if (!dayOneDrawCompleted && vertBarBehavior.totalGames == 1 && vertBarBehavior.draws == 1 && DialogueManager.instance.currentMessage == null)
        {
            dayOneDrawCompleted = true;
            tttManager.freezeBetweenMatches = true;
            DialogueManager.instance.PlayMessage(postDrawMessage);
            DialogueManager.instance.onDialogueSequenceComplete.AddListener(() =>
            {
                tttManager.freezeBetweenMatches = false;
            });
        }
        else if (!dayOneFirstMatchCompleted && (vertBarBehavior.xWins >= 1 || vertBarBehavior.oWins >= 1) && DialogueManager.instance.currentMessage == null)
        {
            dayOneFirstMatchCompleted = true;
            tttManager.freezeBetweenMatches = true;
            DialogueManager.instance.PlayMessage(postFirstMatchMessage);
            DialogueManager.instance.onDialogueSequenceComplete.AddListener(() =>
            {
                vertBarBehavior.EnableChildren();
                vertBarBehavior.NextBestOf();
                tttManager.freezeBetweenMatches = false;
            });
        }
        else if (!dayOneBestOfThreeCompleted && (vertBarBehavior.xWins >= 2 || vertBarBehavior.oWins >= 2) && DialogueManager.instance.currentMessage == null)
        {
            dayOneBestOfThreeCompleted = true;
            tttManager.freezeBetweenMatches = true;
            if (vertBarBehavior.xWins > vertBarBehavior.oWins)
            {
                DialogueManager.instance.PlayMessage(postBestOfThreeMessageWin);
            }
            else
            {
                DialogueManager.instance.PlayMessage(postBestOfThreeMessageLose);
            }
            DialogueManager.instance.onDialogueSequenceComplete.AddListener(() =>
            {
                saveData.currentDay = 2; // Progress to day 2
                PlayerSaveDataManager.instance.SavePlayerData(saveData);
                SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload scene to progress timeline
            });
        }
    }
#endregion

#region DayTwo

    [Header("Day Two Dialogue")]
    [SerializeField] private MessageSO dayTwoIntro;
    [SerializeField] private MessageSO dayTwoFirstDrawMessage;
    [SerializeField] private MessageSO dayTwoFirstWinMessage;
    [SerializeField] private MessageSO dayTwoSecondFirstWinMessage;
    [SerializeField] private List<GameObject> stuffToHideDayTwo;
    private void StartDayTwo()
    {
        PlayerSaveDataManager.instance.SavePlayerData(saveData);
        curtainBehavior.OpenCurtain("Day Two", "5:00 PM", dayTwoIntro);
        foreach (var obj in stuffToHideDayTwo)
        {
            obj.SetActive(false);
        }
        DialogueManager.instance.onDialogueSequenceComplete.AddListener(() =>
        {
            MusicAudioManager.instance.FadeInMainMusic();
            tttManager.gameObject.SetActive(true);
            tttManager.StartGame();
        });

    }
    // Day Two Update Flags
    private bool dayTwoFirstWinCompleted = false;
    private bool dayTwoFirstDrawCompleted = false;
    private void UpdateDayTwo()
    {
        // Progress Best-Of
        if (vertBarBehavior.xWins == Mathf.CeilToInt(vertBarBehavior.bestOf/2f) || vertBarBehavior.oWins == Mathf.CeilToInt(vertBarBehavior.bestOf/2f))
        {
            vertBarBehavior.NextBestOf();
        }

        if (!dayTwoFirstDrawCompleted && vertBarBehavior.draws >= 1 && DialogueManager.instance.currentMessage == null)
        {
            dayTwoFirstDrawCompleted = true;
            tttManager.freezeBetweenMatches = true;
            DialogueManager.instance.PlayMessage(dayTwoFirstDrawMessage);
            DialogueManager.instance.onDialogueSequenceComplete.AddListener(() =>
            {
                tttManager.freezeBetweenMatches = false;
            });
        }
        else if (!dayTwoFirstWinCompleted && vertBarBehavior.xWins >= 1 && DialogueManager.instance.currentMessage == null)
        {
            dayTwoFirstWinCompleted = true;
            tttManager.freezeBetweenMatches = true;
            DialogueManager.instance.PlayMessage(dayTwoFirstWinMessage);
            DialogueManager.instance.onDialogueSequenceComplete.AddListener(() =>
            {
                tttManager.freezeBetweenMatches = false;
                // make shop visible
                ShopManager.instance.showShopPanel = true;
                ShopManager.instance.showXosCounter = true;
                ShopManager.instance.RenderShopEnvironment();

            });
        }

    }
#endregion
}
