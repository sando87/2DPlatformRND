using DG.Tweening;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterRoot : MonoBehaviour
{
    [SerializeField] int _CharacterID = 1;

    private int mCharacterID = -1;

    public int CharacterID => GetCharacterData();

    void Awake()
    {
    }

    int GetCharacterData()
    {
        if (mCharacterID >= 0)
        {
            return mCharacterID;
        }

        UserSaveData userData = SaveFileManager<UserSaveData>.Load();
        if (userData.Characters.ContainsKey(_CharacterID))
        {
            mCharacterID = _CharacterID;
        }
        else
        {
            userData.Characters[_CharacterID] = new CharacterSaveData();
            mCharacterID = _CharacterID;
            SaveFileManager<UserSaveData>.Save(userData);
        }

        return mCharacterID;
    }

    public ItemInventory Inven => GetComponentInChildren<ItemInventory>();
    public CharObject Stats => GetComponentInChildren<CharObject>();
    public SkillController Skills => GetComponentInChildren<SkillController>();
    public BuffController Buffs => GetComponentInChildren<BuffController>();
    public Experience Exp => GetComponentInChildren<Experience>();

}
