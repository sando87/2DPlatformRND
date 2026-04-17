using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

namespace PahlBit
{
    public class EnemySummoner : MonoBehaviour
    {
        [SerializeField] BaseObject _BaseObjPrefab = null;
        [SerializeField] SpriteAnimation _SummonAnim = null;

        private bool mIsSummoning = false;

        void Start()
        {
            this.ExGetBase().Interactor.OnInteractSignal.AddListener(StartSummoning);
            _SummonAnim.EventEndAnim.AddListener(DoSummonEnemy);
            _SummonAnim.gameObject.SetActive(false);
        }

        void StartSummoning(BaseObject sender, InteractMask mask)
        {
            if (mIsSummoning)
                return;

            mIsSummoning = true;
            float delay = MyUtils.RandomFloat(0, 1.0f);
            this.ExDelayedCoroutine(delay, () => _SummonAnim.gameObject.SetActive(true));
        }

        public void DoSummonEnemy()
        {
            Vector3 spawnPos = transform.position + new Vector3(0, 0.1f, 0);
            BaseObject obj = Instantiate(_BaseObjPrefab, spawnPos, Quaternion.identity);
            EnemyBase enemy = obj.EnemyObj;
            enemy.SetTarget(InGameEngine.Inst.Player);
            
            mIsSummoning = false;
        }
    }
}