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
            case 3:
                StartDayThree();
                break;
            case 4:
                StartDayFour();
                break;
            case 5:
                StartDayFive();
                break;
            case 18255:
                StartDay18255();
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
            case 3:
                UpdateDayThree();
                break;
            case 4:
                UpdateDayFour();
                break;
            case 5:
                UpdateDayFive();
                break;
            case 18255:
                UpdateDay18255();
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
            foreach (var obj in stuffToHideDayTwo)
            {
                if (obj.GetComponent<UltManager>() != null) continue;
                obj.SetActive(true);
            }
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

#region DayThree

    [Header("Day Three Dialogue")]
    [SerializeField] private MessageSO dayThreeIntro;
    private bool dayThreeUltUnlocked = false;
    private void StartDayThree()
    {
        PlayerSaveDataManager.instance.SavePlayerData(saveData);
        curtainBehavior.OpenCurtain("Day Three", "5:00 PM", dayThreeIntro);
        foreach (var obj in stuffToHideDayTwo)
        {
            obj.SetActive(false);
        }
        DialogueManager.instance.onDialogueSequenceComplete.AddListener(() =>
        {
            MusicAudioManager.instance.FadeInMainMusic();
            foreach (var obj in stuffToHideDayTwo)
            {
                if (obj.GetComponent<UltManager>() != null) continue;
                obj.SetActive(true);
            }
            tttManager.StartGame();
            if (ShopManager.instance != null)
            {
                ShopManager.instance.showShopPanel = true;
                ShopManager.instance.showXosCounter = true;
                ShopManager.instance.RenderShopEnvironment();
            }
        });
    }

    private void UpdateDayThree()
    {
        // Progress Best-Of
        if (vertBarBehavior.xWins == Mathf.CeilToInt(vertBarBehavior.bestOf/2f) || vertBarBehavior.oWins == Mathf.CeilToInt(vertBarBehavior.bestOf/2f))
        {
            vertBarBehavior.NextBestOf();
        }

        if (dayThreeUltUnlocked)
        {
            return;
        }

        if (saveData == null || saveData.progressionTags == null || DialogueManager.instance == null)
        {
            return;
        }

        if (saveData.progressionTags.Contains("ultimate") && DialogueManager.instance.currentMessage == null)
        {
            Debug.Log("Unlocking Ult!");
            dayThreeUltUnlocked = true;
            foreach (var obj in stuffToHideDayTwo)
            {
                if (obj == null)
                {
                    continue;
                }

                var ultManager = obj.GetComponent<UltManager>();
                if (ultManager != null)
                {
                    obj.SetActive(true);
                    ultManager.UseUlt();
                }
            }
        }
    }

#endregion
#region DayFour

    [Header("Day Four Dialogue")]
    [SerializeField] private MessageSO dayFourIntro;

    private void StartDayFour()
    {
        PlayerSaveDataManager.instance.SavePlayerData(saveData);
        curtainBehavior.OpenCurtain("Day Four", "5:00 PM", dayFourIntro);

        foreach (var obj in stuffToHideDayTwo)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
        DialogueManager.instance.onDialogueSequenceComplete.AddListener(() =>
        {
            //next day
            saveData.currentDay = 5; // Progress to day 5 (or end of current content)
            PlayerSaveDataManager.instance.SavePlayerData(saveData);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload scene to progress timeline
        });
    }

    private void UpdateDayFour()
    {
        // filler
    }

#endregion

#region DayFive

    [Header("Day Five Dialogue")]
    [SerializeField] private MessageSO dayFiveIntro;

    private void StartDayFive()
    {
        PlayerSaveDataManager.instance.SavePlayerData(saveData);
        curtainBehavior.OpenCurtain("Day Five", "5:00 PM", dayFiveIntro);

        foreach (var obj in stuffToHideDayTwo)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
        DialogueManager.instance.onDialogueSequenceComplete.AddListener(() =>
        {
            //next day
            saveData.currentDay = 18255; // Progress to day 18255 (or end of current content)
            PlayerSaveDataManager.instance.SavePlayerData(saveData);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload scene to progress timeline
        });
    }

    private void UpdateDayFive()
    {
        // filler
    }

#endregion

#region Day18255

    [Header("Day 18255 Dialogue")]
    [SerializeField] private MessageSO day18255Intro;

    private void StartDay18255()
    {
        PlayerSaveDataManager.instance.SavePlayerData(saveData);
        curtainBehavior.OpenCurtain("Day 18255", "50 Years Later...", day18255Intro);

        foreach (var obj in stuffToHideDayTwo)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
        DialogueManager.instance.onDialogueSequenceComplete.AddListener(() =>
        {
            curtainBehavior.CloseCurtain(() =>
            {
                // Reset timeline for new game plus loop
                PlayerPrefs.DeleteAll();
                SceneManager.LoadScene("TitleScene"); // Reload scene to progress timeline
            });
        });
    }

    private void UpdateDay18255()
    {
        // filler
    }

#endregion
}
