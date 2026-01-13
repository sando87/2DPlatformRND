using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] int GoldDropPercent = 30;
    [SerializeField] Gold GoldPrefab = null;

    BaseObject mBase = null;
    EnemyDataMono mEnemyData = null;

    private void Awake()
    {
        mBase = GetComponentInParent<BaseObject>();
        mEnemyData = mBase.GetComponentInChildren<EnemyDataMono>();
    }

    void Start()
    {

        mBase.Health.InitHealth(mEnemyData.Data.Stats.Health, 0, 0);
        mBase.Health.OnDied.AddListener(OnDeath);

    }

    public void OnDeath()
    {
        if (MyUtils.IsPercentHit((int)mEnemyData.Data.Stats.ItemDrop.PercentValue))
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
        itemObj.GoldAmount = mEnemyData.Data.Stats.GoldOnDeath;
    }
}
