using System.Collections;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class InGameEngine : MonoBehaviour
{
    [SerializeField] AudioClip _BGM = null;
    [SerializeField] PlayerUnitInput _PlayerInput = null;

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
            PopupManager.Instance.Toggle<PopupStats>();
        }
        if (_PlayerInput.JustPressed(PlayerUnitInputType.ShowPopupInven))
        {
            PopupManager.Instance.Toggle<PopupInven>();
        }
        if (_PlayerInput.JustPressed(PlayerUnitInputType.ShowPopupSkill))
        {
            PopupManager.Instance.Toggle<PopupSkill>();
        }
    }

}
