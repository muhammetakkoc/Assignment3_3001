using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New UI Config", menuName = "UIConfig")]
public class PlayerUIConfig : ScriptableObject
{
    [SerializeField]
    public Color enemyColor = Color.red;

    [SerializeField]
    public Color neutralColor = Color.yellow;

    [SerializeField]
    public Color friendlyColor = Color.green;

    [SerializeField]
    public Color unknownColor = Color.white;

    [SerializeField]
    public Sprite selectedSprite;

    [SerializeField]
    public Sprite contactSprite;

    [SerializeField]
    public Sprite moveCommandSprite;

    [SerializeField]
    public Sprite canSelectSprite;

    [SerializeField]
    public AudioClip selectTargetSound;

    [SerializeField]
    public AudioClip targetAddedSound;

    [SerializeField]
    public AudioClip targetLostSound;

    [SerializeField]
    public AudioClip disallowedActionSound;

    [SerializeField]
    public AudioClip acknowledgeCommandSound;

    [SerializeField]
    public GameObject targetMarkerPrefab;
}
