using DG.Tweening;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRoot : MonoBehaviour
{
    public Equipment Equip => GetComponentInChildren<Equipment>();
    public Inventory Inven => GetComponentInChildren<Inventory>();
    public PlayerStats Stats => GetComponentInChildren<PlayerStats>();
    public PlayerSkills Skills => GetComponentInChildren<PlayerSkills>();
    public BuffController Buffs => GetComponentInChildren<BuffController>();

}
