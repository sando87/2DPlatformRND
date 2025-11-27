using DG.Tweening;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterRoot : MonoBehaviour
{
    [SerializeField] int _CharacterID = 1;
    public int CharacterID => _CharacterID;

    public Equipment Equip => GetComponentInChildren<Equipment>();
    public ItemInventory Inven => GetComponentInChildren<ItemInventory>();
    public CharObject Stats => GetComponentInChildren<CharObject>();
    public SkillController Skills => GetComponentInChildren<SkillController>();
    public BuffController Buffs => GetComponentInChildren<BuffController>();
    public Experience Exp => GetComponentInChildren<Experience>();

}
