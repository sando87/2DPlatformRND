using System.Collections;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class InGameEngine : SingletonMono<InGameEngine>
{
    [SerializeField] AudioClip _BGM = null;
    [SerializeField] BaseObject _PlayerUnit = null;
    [SerializeField] InputSystemManager _InputManager = null;

    PopupStats mPopupStats;
    PopupInven mPopupInven;
    PopupSkill mPopupSkill;

    IEnumerator Start()
    {
        SoundPlayManager.Instance.Init();
        yield return null;
        SoundPlayManager.Instance.PlayBGM(_BGM);
        yield return newWaitForSeconds.Cache(0.1f);
        SetInputHandler(_PlayerUnit.Input);
    }

    void Update()
    {
        if (_InputManager.JustPressed(PlayerUnitInputType.ShowPopupStats))
        {
            mPopupStats = PopupManager.Instance.Toggle<PopupStats>();
            if (mPopupStats != null)
            {
                SetInputHandler(mPopupStats);
            }
            else
            {
                SetInputHandler(_PlayerUnit.Input);
            }
        }
        if (_InputManager.JustPressed(PlayerUnitInputType.ShowPopupInven))
        {
            mPopupInven = PopupManager.Instance.Toggle<PopupInven>();
            if (mPopupInven != null)
            {
                SetInputHandler(mPopupInven);
                mPopupInven.ItemInven = _PlayerUnit.GetComponentInChildren<ItemInventory>();
            }
            else
            {
                SetInputHandler(_PlayerUnit.Input);
            }
        }
        if (_InputManager.JustPressed(PlayerUnitInputType.ShowPopupSkill))
        {
            mPopupSkill = PopupManager.Instance.Toggle<PopupSkill>();
            if (mPopupSkill != null)
            {
                SetInputHandler(mPopupSkill);
            }
            else
            {
                SetInputHandler(_PlayerUnit.Input);
            }
        }
    }

    public void SetInputHandler(IInputHandler handler)
    {
        _InputManager.SetHandlerInput(handler);
    }

}
