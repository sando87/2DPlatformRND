using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class SoundFXPlayer : MonoBehaviour
{
    public void PlaySFX(AudioClip clip)
    {
        SoundPlayManager.Instance.PlaySFXClip(clip);
    }
}
