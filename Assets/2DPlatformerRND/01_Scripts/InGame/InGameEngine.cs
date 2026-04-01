using System.Collections;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class InGameEngine : MonoBehaviour
{
    [SerializeField] AudioClip _BGM = null;
    [SerializeField] BaseObject _PlayerUnit = null;

    PopupStats mPopupStats;
    PopupInven mPopupInven;
    PopupSkill mPopupSkill;

    IEnumerator Start()
    {
        SoundPlayManager.Instance.Init();
        yield return null;
        SoundPlayManager.Instance.PlayBGM(_BGM);
    }

    void Update()
    {
        if (_PlayerUnit.Input.JustPressed(PlayerUnitInputType.ShowPopupStats))
        {
            mPopupStats = PopupManager.Instance.Toggle<PopupStats>();
            UpdatePlayerInputState();
        }
        if (_PlayerUnit.Input.JustPressed(PlayerUnitInputType.ShowPopupInven))
        {
            mPopupInven = PopupManager.Instance.Toggle<PopupInven>();
            if (mPopupInven != null)
            {
                mPopupInven.ItemInven = _PlayerUnit.GetComponentInChildren<ItemInventory>();
            }
            UpdatePlayerInputState();
        }
        if (_PlayerUnit.Input.JustPressed(PlayerUnitInputType.ShowPopupSkill))
        {
            mPopupSkill = PopupManager.Instance.Toggle<PopupSkill>();
            UpdatePlayerInputState();
        }
    }

    void UpdatePlayerInputState()
    {
        _PlayerUnit.Input.LockPlayerInput = true;
        if (mPopupStats != null)
            mPopupStats.PlayerInput = _PlayerUnit.Input;
        else if (mPopupInven != null)
            mPopupInven.PlayerInput = _PlayerUnit.Input;
        else if (mPopupSkill != null)
            mPopupSkill.PlayerInput = _PlayerUnit.Input;
        else
            _PlayerUnit.Input.LockPlayerInput = false;
    }

}
