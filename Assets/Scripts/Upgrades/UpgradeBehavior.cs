using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        ShopManager.instance.SpendXos(cost);
        ShopManager.instance.acquiredUpgrades.Add(upgradeData);
        ShopManager.instance.RenderShopEnvironment();
    }
}
