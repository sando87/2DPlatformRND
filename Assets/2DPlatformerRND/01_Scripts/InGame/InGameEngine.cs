using System.Collections;
using System.Collections.Generic;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class InGameEngine : SingletonMono<InGameEngine>
{
    [SerializeField] AudioClip _BGM = null;
    [SerializeField] BaseObject _PlayerUnit = null;
    [SerializeField] InputSystemManager _InputManager = null;
    [SerializeField] InGamePanel _InGamePanel = null;

    public BaseObject Player => _PlayerUnit;

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
                SetInputHandler(mPopupStats.InputHandler);
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
                SetInputHandler(mPopupInven.InputHandler);
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
                SetInputHandler(mPopupSkill.InputHandler);
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
    public IInputHandler GetInputHandler()
    {
        return _InputManager.GetHandlerInput();
    }

    public void ShowItemSelector(Vector2 worldPos, List<ItemObject> items)
    {
        ItemSelector itemSelector = _InGamePanel.GetComponentInChildren<ItemSelector>();
        if (itemSelector != null)
        {
            itemSelector.transform.position = worldPos;
            itemSelector.ShowItemSelector(items);
        }
    }
    public void HideItemSelector()
    {
        ItemSelector itemSelector = _InGamePanel.GetComponentInChildren<ItemSelector>();
        if (itemSelector != null)
        {
            itemSelector.HideItemSelector();
        }
    }
    public void MoveItemSelector(bool isUp)
    {
        ItemSelector itemSelector = _InGamePanel.GetComponentInChildren<ItemSelector>();
        if (itemSelector != null)
        {
            if (isUp)
            {
                itemSelector.MoveUp();
            }
            else
            {
                itemSelector.MoveDown();
            }
        }
    }


}
