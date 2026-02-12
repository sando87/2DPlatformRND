using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] int GoldDropPercent = 30;
    [SerializeField] Gold GoldPrefab = null;

    [SerializeField]
    [Dropdown(nameof(IDList))]
    string _ResourceID = "";
    List<string> IDList { get => EnemyResourceTable.Instance.GetAllInfo().Select(info => info.EnemyID).ToList(); }

    public SpecEnemy Spec { get; private set; } = null;

    BaseObject mBase = null;

    private void Awake()
    {
        mBase = GetComponentInParent<BaseObject>();

        Spec = mBase.GetComponentInChildren<SpecEnemy>();
        Spec.InitData(_ResourceID);
        Spec.LinkOption(mBase.Buffs.TotalBuffOption);
    }

    void Start()
    {
        mBase.Health.OnDied.AddListener(OnDeath);
    }

    public void OnDeath()
    {
        mBase.Body.LockBody = true;

        if (MyUtils.IsPercentHit((int)Spec.ItemDrop.PercentValue))
            DropItem();

        if (MyUtils.IsPercentHit(GoldDropPercent))
            DropGold();
    }

    void DropItem()
    {
        ItemObject.Create(mBase.Body.Center, Quaternion.identity);
    }
    void DropGold()
    {
        Gold itemObj = Instantiate(GoldPrefab, mBase.Body.Center, Quaternion.identity);
        itemObj.GoldAmount = Spec.GoldOnDeath;
    }
}
