using DG.Tweening;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterRoot : MonoBehaviour
{
    [SerializeField] int _CharacterID = 1;
    public int CharacterID => _CharacterID;

    public Equipment Equip => GetComponentInChildren<Equipment>();
    public Inventory Inven => GetComponentInChildren<Inventory>();
    public CharacterStats Stats => GetComponentInChildren<CharacterStats>();
    public CharacterSkills Skills => GetComponentInChildren<CharacterSkills>();
    public BuffController Buffs => GetComponentInChildren<BuffController>();
    public Experience Exp => GetComponentInChildren<Experience>();

}
