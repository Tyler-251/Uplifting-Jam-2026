using System;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeCategory
{
    Trickiness,
    TurnSpeed
}

public enum UpgradeType
{
    Additive,
    AdditiveMultiplier,
    Multiplier
}

[CreateAssetMenu(fileName = "UpgradeSO", menuName = "Scriptable Objects/UpgradeSO")]
public class UpgradeSO : ScriptableObject
{
    public UpgradeCategory category;
    public UpgradeType type;
    public Sprite icon;
    public string upgradeName;
    public string description;
    public int cost;
    public float value;
    public MessageSO optionalMessageTrigger = null;
    public List<UpgradeSO> requiredUpgrades = new List<UpgradeSO>();
    public List<int> requiredDays = new List<int>();
    public int xosRequirement = 0;

}
