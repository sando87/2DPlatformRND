using System.Collections;
using PahlBit;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SpriteAnimationEx : MonoBehaviour
{
    [SerializeField] SpriteRenderer _Renderer = null;
    [SerializeField] Sprite[] _SpritesStart = null;
    [SerializeField] Sprite[] _SpritesLoop = null;
    [SerializeField] Sprite[] _SpritesEnd = null;
    [SerializeField] float _Interval = 0.1f;
    [SerializeField] UnityEvent _OnStartLoop = null;

    int mIndex = 0;

    void OnEnable()
    {
        StartAnimation();
    }

    public void StartAnimation()
    {
        StopAllCoroutines();
        mIndex = 0;
        this.ExRepeatCoroutine(_Interval, () => _Renderer.sprite = _SpritesStart[mIndex++ % _SpritesStart.Length], _SpritesStart.Length);

        float delay = _Interval * _SpritesStart.Length;
        this.ExDelayedCoroutine(delay, OnStartLoopAnimation);
    }
    void OnStartLoopAnimation()
    {
        _OnStartLoop?.Invoke();
        mIndex = 0;
        this.ExRepeatCoroutine(_Interval, () => _Renderer.sprite = _SpritesLoop[mIndex++ % _SpritesLoop.Length], -1);
    }

    public void StopAnimation()
    {
        StopAllCoroutines();
        mIndex = 0;
        this.ExRepeatCoroutine(_Interval, () => _Renderer.sprite = _SpritesEnd[mIndex++ % _SpritesEnd.Length], _SpritesEnd.Length);

        float delay = _Interval * _SpritesEnd.Length;
        this.ExDelayedCoroutine(delay, OnEndAnimation);
    }

    void OnEndAnimation()
    {
        _Renderer.sprite = null;
    }
}
