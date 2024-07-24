using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ContactTargetingState
{
    Contact,
    Selected
}

[Serializable]
public class RadarMarkerUI : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI text;

    [SerializeField]
    public Image icon;

    [SerializeField]
    PlayerUIConfig playerUIConfig;

    private ContactTargetingState targetingState;
    public ContactTargetingState TargetingState
    {
        get { return targetingState; }
        set
        {
            targetingState = value;
            switch (targetingState)
            {
                case ContactTargetingState.Contact:
                    icon.sprite = playerUIConfig.contactSprite; break;
                case ContactTargetingState.Selected:
                    icon.sprite = playerUIConfig.selectedSprite; break;
            }
        }
    }

    public void UpdateColor(IFF_Tag iffResponse)
    {
        if (iffResponse == IFF_Tag.Neutral)
        {
            icon.color = playerUIConfig.neutralColor;
            text.color = playerUIConfig.neutralColor;
        }
        else if (iffResponse == IFF_Tag.Friendly)
        {
            icon.color = playerUIConfig.friendlyColor;
            text.color = playerUIConfig.friendlyColor;
        }
        else if (iffResponse == IFF_Tag.Frenemy || iffResponse == IFF_Tag.Enemy)
        {
            icon.color = playerUIConfig.enemyColor;
            text.color = playerUIConfig.enemyColor;
        }
        else
        {
            icon.color = playerUIConfig.unknownColor;
            text.color = playerUIConfig.unknownColor;
        }
    }
}