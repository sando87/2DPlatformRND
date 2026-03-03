using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpriteAnimation : MonoBehaviour
{
    [SerializeField] SpriteRenderer _Renderer = null;
    [SerializeField] Sprite[] _Sprites = null;
    [SerializeField] bool _Loop = false;
    [SerializeField] float _Interval = 0.1f;

    int mIndex = 0;

    void OnEnable()
    {
        StartAnimation();
    }

    public void StartAnimation()
    {
        mIndex = 0;
        int repeatCount = _Loop ? -1 : _Sprites.Length;
        StopAllCoroutines();
        this.ExRepeatCoroutine(_Interval, () => _Renderer.sprite = _Sprites[mIndex++ % _Sprites.Length], repeatCount);
    }
}
