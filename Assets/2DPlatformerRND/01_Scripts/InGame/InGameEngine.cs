using System.Collections;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class InGameEngine : MonoBehaviour
{
    [SerializeField] AudioClip _BGM = null;

    IEnumerator Start()
    {
        SoundPlayManager.Instance.Init();
        yield return null;
        SoundPlayManager.Instance.PlayBGM(_BGM);
    }

}
