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
    [SerializeField] private GameObject upgradesPanel;
    [SerializeField] private GameObject statsPanel;
    [Space(10)]
    [SerializeField] private GameObject ShopButton;
    [SerializeField] private GameObject StatsButton;

    // Progression Values
    [Header("Progression Values")]
    public bool showXosCounter = false;
    public bool showShopPanel = false;
    public bool showStatsPanel = false;
    [Header("Player Stats")]
    [SerializeField] private int xos = 0;

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
    }
    public void SwitchToUpgradesTab() => SwitchTab(Tab.Upgrades);
    public void SwitchToStatsTab() => SwitchTab(Tab.Stats);

    public void RenderShopEnvironment()
    {
        backPanel.SetActive(showShopPanel);
        xosCounter.SetActive(showXosCounter);
        upgradesPanel.SetActive(showShopPanel && currentTab == Tab.Upgrades);
        statsPanel.SetActive(showShopPanel && currentTab == Tab.Stats);

        ShopButton.SetActive(showShopPanel);
        StatsButton.SetActive(showShopPanel && showStatsPanel);
    }

    public void RenderUpgrades()
    {
        RectTransform upgradesContent = upgradesPanel.transform.Find("Content").GetComponent<RectTransform>();
        // Clear existing upgrade items
        foreach (Transform child in upgradesContent)
        {
            Destroy(child.gameObject);
        }
        // TODO: render upgrades
        
    }



    
}
