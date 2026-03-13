using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    [System.Serializable]
    public enum Tab
    {
        Upgrades,
        Stats
    }
    [Header("UI References")]
    [SerializeField] private GameObject backPanel;
    [SerializeField] private GameObject xosCounter;
    [SerializeField] private TMP_Text xosCounterText;
    [SerializeField] private GameObject upgradesPanel;
    [SerializeField] private GameObject statsPanel;
    [Space(10)]
    [SerializeField] private GameObject ShopButton;
    [SerializeField] private GameObject StatsButton;
    [Space(10)]
    [SerializeField] private GameObject upgradeItemPrefab;

    // Progression Values
    [Header("Progression Values")]
    public bool showXosCounter = false;
    public bool showShopPanel = false;
    public bool showStatsPanel = false;
    [Header("Player Stats")]
    [SerializeField] public int xos = 0;
    [SerializeField] public int maxXos = 0;
    [Header("Upgrade Registry")]
    [SerializeField] private List<UpgradeSO> availableUpgrades;
    public List<UpgradeSO> acquiredUpgrades = new List<UpgradeSO>();

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

    private void Start()
    {
        UpdateMaxXos();
        RenderUpgrades(); // Initialize the upgrades based on the available upgrades
        RenderXOsCounter(); // Initialize the XOs counter display
        SwitchTab(currentTab); // Initialize the UI based on the default tab
        RenderShopEnvironment(); // Initialize the shop environment based on the current settings
    }

    public Tab currentTab = Tab.Upgrades;

    public void SwitchTab(Tab tab)
    {
        currentTab = tab;
        // Add logic to update the UI based on the selected tab
        switch (currentTab)
        {
            case Tab.Upgrades:
                upgradesPanel.SetActive(true);
                statsPanel.SetActive(false);
                break;
            case Tab.Stats:
                upgradesPanel.SetActive(false);
                statsPanel.SetActive(true);
                break;
        }
        RenderUpgrades(); // Refresh the upgrades display when switching tabs
        RenderShopEnvironment(); // Update the shop environment to reflect the current tab
    }
    public void SwitchToUpgradesTab() => SwitchTab(Tab.Upgrades);
    public void SwitchToStatsTab() => SwitchTab(Tab.Stats);

    public void RenderShopEnvironment()
    {
        // Rendering Panels
        backPanel.SetActive(showShopPanel);
        xosCounter.SetActive(showXosCounter);
        upgradesPanel.SetActive(showShopPanel && currentTab == Tab.Upgrades);
        statsPanel.SetActive(showShopPanel && currentTab == Tab.Stats);

        ShopButton.SetActive(showShopPanel);
        StatsButton.SetActive(showShopPanel && showStatsPanel);
        RenderUpgrades();
    }

    public void AddXos(int amount)
    {
        xos += amount;
        UpdateMaxXos();
        RenderXOsCounter();
        RenderUpgrades();
    }

    public bool SpendXos(int amount)
    {
        if (xos >= amount)
        {
            xos -= amount;
            UpdateMaxXos();
            RenderXOsCounter();
            RenderUpgrades();
            return true;
        }
        return false;
    }

    private void UpdateMaxXos()
    {
        if (xos > maxXos)
        {
            maxXos = xos;
        }
    }

    public void RenderXOsCounter()
    {
        xosCounterText.text = xos.ToString();
    }

    public void RenderUpgrades()
    {
        Debug.Log("Rendering Upgrades...");

        if (TimelineManager.instance != null && TimelineManager.instance.saveData != null) 
        foreach (UpgradeSO upgrade in TimelineManager.instance.saveData.unlockedUpgrades)
        {
            if (!acquiredUpgrades.Contains(upgrade))
            {
                acquiredUpgrades.Add(upgrade);
            }
        }


        SortUpgrades();
        if (upgradesPanel == null) { Debug.Log("mk1: upgradesPanel is null"); return;}
        Transform contentTransform = upgradesPanel.transform.GetChild(0).GetChild(0);
        if (contentTransform == null) { Debug.Log("mk2: Content transform not found"); return;}
        RectTransform upgradesContent;
        if (!contentTransform.TryGetComponent<RectTransform>(out upgradesContent)) { Debug.Log("mk3: RectTransform not found"); return;}
        
        foreach (Transform child in upgradesContent)
        {
            Destroy(child.gameObject);
        }

        foreach (var upgrade in availableUpgrades)
        {
            // REQUIREMENT CHECKS
            if (acquiredUpgrades.Contains(upgrade))
            {
                continue;
            }

            bool hasAllPrereqs = true;
            foreach (var prereq in upgrade.requiredUpgrades)
            {
                if (!acquiredUpgrades.Contains(prereq))
                {
                    hasAllPrereqs = false;
                    break;
                }
            }

            if (!hasAllPrereqs)
            {
                continue;
            }

            bool inCorrectDay = false;
            foreach (var reqStat in upgrade.requiredDays)
            {
                if (TimelineManager.instance != null && TimelineManager.instance.saveData != null && TimelineManager.instance.saveData.currentDay == reqStat)
                {
                    inCorrectDay = true;
                    break;
                }
            }
            if (!inCorrectDay)
            {
                continue;
            }
            if (upgrade.xosRequirement > maxXos)
            {
                continue;
            }
            if (upgrade.requiredProgressionTags.Count > 0)
            {
                bool hasRequiredTag = false;
                foreach (var tag in upgrade.requiredProgressionTags)
                {
                    if (TimelineManager.instance != null && TimelineManager.instance.saveData.progressionTags.Contains(tag))
                    {
                        hasRequiredTag = true;
                        break;
                    }
                }
                if (!hasRequiredTag)
                {
                    continue;
                }
            }
            // END REQUIREMENT CHECKS 
            
            GameObject upgradeItem = Instantiate(upgradeItemPrefab, upgradesContent);
            UpgradeBehavior upgradeBehavior = upgradeItem.GetComponent<UpgradeBehavior>();
            upgradeBehavior.Initialize(upgrade);
        }
    }

    public void SortUpgrades()
    {
        availableUpgrades.Sort((a, b) => a.index.CompareTo(b.index));
    }



    
}
