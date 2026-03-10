using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// Progression Index Mappings
/// 1 : Unlock Xos Counter
/// 
public enum UpgradeCategory
{
    Trickiness,
    TurnSpeed,
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
    [TextArea]
    public string description;
    public int cost;
    public float value;
    public string progressionTags; // Comma-separated tags for progression requirements
    public int xosRequirement = 0;
    public List<string> ProgressionTags => new List<string>(progressionTags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(tag => tag.Trim()));
    public MessageSO optionalMessageTrigger = null;
    public List<UpgradeSO> requiredUpgrades = new List<UpgradeSO>();
    public List<int> requiredDays = new List<int>();
    public List<string> requiredProgressionTags = new List<string>();

}
