using System.Collections;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class InGameEngine : MonoBehaviour
{
    [SerializeField] AudioClip _BGM = null;
    [SerializeField] PlayerUnitInput _PlayerInput = null;

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
        if (_PlayerInput.JustPressed(PlayerUnitInputType.ShowPopupStats))
        {
            mPopupStats = PopupManager.Instance.Toggle<PopupStats>();
            UpdatePlayerInputState();
        }
        if (_PlayerInput.JustPressed(PlayerUnitInputType.ShowPopupInven))
        {
            mPopupInven = PopupManager.Instance.Toggle<PopupInven>();
            UpdatePlayerInputState();
        }
        if (_PlayerInput.JustPressed(PlayerUnitInputType.ShowPopupSkill))
        {
            mPopupSkill = PopupManager.Instance.Toggle<PopupSkill>();
            UpdatePlayerInputState();
        }
    }

    void UpdatePlayerInputState()
    {
        _PlayerInput.LockPlayerInput = true;
        if (mPopupStats != null)
            mPopupStats.PlayerInput = _PlayerInput;
        else if (mPopupInven != null)
            mPopupInven.PlayerInput = _PlayerInput;
        else if (mPopupSkill != null)
            mPopupSkill.PlayerInput = _PlayerInput;
        else
            _PlayerInput.LockPlayerInput = false;
    }

}
