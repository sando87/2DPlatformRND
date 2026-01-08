using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class EnemyDataMono : MonoBehaviour
    {
        [SerializeField]
        [Dropdown("IDList")]
        // [OnValueChanged(nameof(SelectEnemyID))]
        string _ID = "";
        List<string> IDList { get => EnemyResourceTable.Instance.GetAllInfo().Select(info => info.EnemyID).ToList(); }

        // [SerializeField]
        // [ReadOnly]
        // private EnemyData _EnemyData = null;
        // void SelectEnemyID() { if (_EnemyData == null) { _EnemyData = new EnemyData(); } _EnemyData.InitData(_ID); _EnemyData._StatsForDev = _EnemyData.Stats; }

        public EnemyData Data { get; private set; } = null;

        void Awake()
        {
            Data = new EnemyData();
            Data.InitData(_ID);
        }
    }
}