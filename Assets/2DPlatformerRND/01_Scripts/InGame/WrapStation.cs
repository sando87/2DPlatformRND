using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class WrapStation : MonoBehaviour
    {
        [SerializeField] GameObject _Indicator = null;
        [SerializeField] int _ThisWrapID = 0;
        [SerializeField] SceneType _DestScene = SceneType.LevelDemo;
        [SerializeField] int _DestWarpID = 0;

        public int ThisWrapID => _ThisWrapID;
        public SceneType DestScene => _DestScene;
        public int DestWarpID => _DestWarpID;

        public void ShowIndicator(Collider2D col)
        {
            _Indicator.SetActive(true);
        }

        public void HideIndicator(Collider2D col)
        {
            _Indicator.SetActive(false);
        }
    }
}
