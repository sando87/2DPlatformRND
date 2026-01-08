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
        string _ID = "";
        List<string> IDList { get => EnemyResourceTable.Instance.GetAllInfo().Select(info => info.EnemyID).ToList(); }

        [field: SerializeField]
        public EnemyData Data { get; private set; } = null;

        void Awake()
        {
            Data = new EnemyData();
            Data.InitData(_ID);
        }
    }
}