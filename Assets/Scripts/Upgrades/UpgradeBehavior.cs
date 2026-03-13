using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Overlays;

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
        image.sprite = upgrade.icon;
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
            if (upgradeData.optionalMessageTrigger != null)
            {
                TTTManager.instance.freezeBetweenMatches = true;
                DialogueManager.instance.PlayMessage(upgradeData.optionalMessageTrigger);
                DialogueManager.instance.onDialogueSequenceComplete.AddListener(() =>
                {
                    TTTManager.instance.freezeBetweenMatches = false;
                });
            }
            if (!TimelineManager.instance.saveData.unlockedUpgrades.Contains(upgradeData)) TimelineManager.instance.saveData.unlockedUpgrades.Add(upgradeData);
            PlayerSaveDataManager.instance.SavePlayerData(TimelineManager.instance.saveData);
        }
    }
}
