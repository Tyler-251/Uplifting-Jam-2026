using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UpgradeBehavior : MonoBehaviour
{
    public Image image;
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public UpgradeSO upgradeData;
    public Button purchaseButton;
    public TMP_Text costText;
    public int cost;

    public void Initialize(UpgradeSO upgrade)
    {
        upgradeData = upgrade;
        // image.sprite = upgrade.icon;
        titleText.text = upgrade.upgradeName;
        descriptionText.text = upgrade.description;
        cost = upgrade.cost;
        costText.text = cost.ToString();
        purchaseButton.onClick.AddListener(PurchaseUpgrade);
    }
    void PurchaseUpgrade()
    {
        if (ShopManager.instance.SpendXos(cost))
        {
            ShopManager.instance.acquiredUpgrades.Add(upgradeData);
            ShopManager.instance.RenderShopEnvironment();

            void SaveUpgradeProgressToTimeline()
            {
                if (TimelineManager.instance == null || TimelineManager.instance.saveData == null)
                {
                    return;
                }

                if (TimelineManager.instance.saveData.unlockedUpgrades == null)
                {
                    TimelineManager.instance.saveData.unlockedUpgrades = new System.Collections.Generic.List<UpgradeSO>();
                }

                if (TimelineManager.instance.saveData.progressionTags == null)
                {
                    TimelineManager.instance.saveData.progressionTags = new System.Collections.Generic.List<string>();
                }

                if (!TimelineManager.instance.saveData.unlockedUpgrades.Contains(upgradeData))
                {
                    TimelineManager.instance.saveData.unlockedUpgrades.Add(upgradeData);
                }

                foreach (var tag in upgradeData.ProgressionTags)
                {
                    if (!TimelineManager.instance.saveData.progressionTags.Contains(tag))
                    {
                        TimelineManager.instance.saveData.progressionTags.Add(tag);
                    }
                }

                if (upgradeData.progressDay)
                {
                    TimelineManager.instance.saveData.currentDay++;
                }

                PlayerSaveDataManager.instance.SavePlayerData(TimelineManager.instance.saveData);

                if (upgradeData.progressDay)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload scene to progress timeline
                }
            }

            if (upgradeData.optionalMessageTrigger != null)
            {
                DialogueManager.instance.PlayMessage(upgradeData.optionalMessageTrigger, () =>
                {
                    Debug.Log("test message trigger");
                    SaveUpgradeProgressToTimeline();
                });
            }
            else
            {
                SaveUpgradeProgressToTimeline();
            }
        }
    }
}
