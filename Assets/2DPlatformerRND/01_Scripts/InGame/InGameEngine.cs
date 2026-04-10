using System.Collections;
using System.Collections.Generic;
using PahlBit;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class InGameEngine : MonoBehaviour
    {
        public static InGameEngine Inst => InGameManager.Instance.Engine;

        [SerializeField] AudioClip _BGM = null;
        [SerializeField] BaseObject _PlayerPrefab = null;
        [SerializeField] InputSystemManager _InputManager = null;
        [SerializeField] InGamePanel _InGamePanel = null;
        [SerializeField] ItemSelector _ItemSelector = null;
        [SerializeField] CinemachineCamera _CinemachineCamera = null;
        [SerializeField] PlayerDepthManager _PlayerDepthManager = null;
        [SerializeField] PlatformerPathfinder _Pathfinder = null;

        public BaseObject Player { get; private set; } = null;
        public PlayerDepthManager DepthManager => _PlayerDepthManager;
        public PlatformerPathfinder Pathfinder => _Pathfinder;

        PopupStats mPopupStats;
        PopupInven mPopupInven;
        PopupSkill mPopupSkill;

        IEnumerator Start()
        {
            SoundPlayManager.Instance.Init();
            yield return null;
            SoundPlayManager.Instance.PlayBGM(_BGM);
            yield return newWaitForSeconds.Cache(0.1f);

            Vector2 destWarp = FindRespawnPosition(InGameManager.Instance.DestWarpID);
            RespawnPlayer(destWarp);
            _InGamePanel.DoActivatePanel(Player);
            _CinemachineCamera.Follow = Player.transform;
            _PlayerDepthManager.SetPlayer(Player.gameObject);
            yield return newWaitForSeconds.Cache(0.2f);
            SetInputHandler(Player.Input);
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
                    SetInputHandler(Player.Input);
                }
            }
            if (_InputManager.JustPressed(PlayerUnitInputType.ShowPopupInven))
            {
                mPopupInven = PopupManager.Instance.Toggle<PopupInven>();
                if (mPopupInven != null)
                {
                    SetInputHandler(mPopupInven.InputHandler);
                    mPopupInven.ItemInven = Player.GetComponentInChildren<ItemInventory>();
                }
                else
                {
                    SetInputHandler(Player.Input);
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
                    SetInputHandler(Player.Input);
                }
            }
        }

        public void RespawnPlayer(Vector3 pos)
        {
            Player = Instantiate(_PlayerPrefab, pos, Quaternion.identity);
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
            Canvas canvas = _ItemSelector.GetComponentInParent<Canvas>();
            canvas.transform.position = Player.transform.position;
            _ItemSelector.transform.position = worldPos;
            _ItemSelector.ShowItemSelector(items);
        }
        public void HideItemSelector()
        {
            _ItemSelector.HideItemSelector();
        }
        public void MoveItemSelector(bool isUp)
        {
            if (isUp)
            {
                _ItemSelector.MoveUp();
            }
            else
            {
                _ItemSelector.MoveDown();
            }
        }

        public void DoWarpStation(SceneType destScene, int destWarpID)
        {
            _InputManager.ClearHandlerInput();
            InGameManager.Instance.StartScene(destScene, destWarpID);
        }

        Vector2 FindRespawnPosition(int destWarpID)
        {
            WrapStation[] stations = FindObjectsByType<WrapStation>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            if (stations == null || stations.Length == 0)
                return Vector2.zero;

            foreach (WrapStation station in stations)
            {
                if (station.ThisWrapID == destWarpID)
                {
                    return station.transform.position;
                }
            }

            return stations[0].transform.position;
        }
    }
}
