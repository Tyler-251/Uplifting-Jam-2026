using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;
    public enum Tab
    {
        Upgrades,
        Stats
    }
    [SerializeField] private GameObject upgradesPanel;
    [SerializeField] private GameObject statsPanel;

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
