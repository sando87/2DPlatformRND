using System;
using System.Collections.Generic;
using PahlBit;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGamePanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI LevelValue = null;
    [SerializeField] Image HPBarFill = null;
    [SerializeField] TextMeshProUGUI HPValue = null;
    [SerializeField] TextMeshProUGUI ShieldValue = null;
    [SerializeField] Image MPBarFill = null;
    [SerializeField] TextMeshProUGUI MPValue = null;
    [SerializeField] Image ExpBarFill = null;
    [SerializeField] TextMeshProUGUI ExpValue = null;
    [SerializeField] TextMeshProUGUI GoldText = null;
    [SerializeField] TextMeshProUGUI LifePotion = null;
    [SerializeField] TextMeshProUGUI ManaPotion = null;
    [SerializeField] BaseObject PlayerObject = null;
    [SerializeField] Transform[] SkillEquipSlots = null;

    [SerializeField] Transform _ActionsParent = null;
    [SerializeField] UIActionSelector _ActionSelector = null;

    Experience mExperience = null;
    ItemInventory mInven = null;
    SkillController mSkillController = null;

    int mLastLevel = 0;
    int mLastCurHP = 0;
    int mLastMaxHP = 0;
    int mLastShield = 0;
    int mLastCurMP = 0;
    int mLastMaxMP = 0;
    int mLastCurExp = 0;
    int mLastMaxExp = 0;
    int mLastGold = 0;
    int mLastLifePotion = 0;
    int mLastManaPotion = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mExperience = PlayerObject.GetComponentInChildren<Experience>();
        mInven = PlayerObject.GetComponentInChildren<ItemInventory>();
        mSkillController = PlayerObject.GetComponentInChildren<SkillController>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUIState();

        UpdateSkillEquipSlots();
    }

    public void OnClickStartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    public void OnClickQuitGame()
    {
        Application.Quit();
    }

    void UpdateUIState()
    {
        const float barMaxLength = 500;

        int curLevel = mExperience.CurrentLevel;
        if (mLastLevel != curLevel)
        {
            LevelValue.SetText("Lv. {0}", curLevel);
            mLastLevel = curLevel;
        }

        int curHP = (int)PlayerObject.Health.CurrentHP;
        int maxHP = (int)PlayerObject.Health.MaxHealth;
        if (mLastCurHP != curHP || mLastMaxHP != maxHP)
        {
            float hpRate = (float)curHP / maxHP;
            float barCurLength = hpRate * barMaxLength;
            HPBarFill.rectTransform.sizeDelta = new Vector2(barCurLength, HPBarFill.rectTransform.sizeDelta.y);
            HPValue.SetText("{0} / {1}", curHP, maxHP);

            mLastCurHP = curHP;
            mLastMaxHP = maxHP;
        }

        int curShield = (int)PlayerObject.Health.CurrentShield;
        if (mLastShield != curShield)
        {
            ShieldValue.SetText("({0})", curShield);

            mLastShield = curShield;
        }

        int curMP = (int)PlayerObject.Health.CurrentMana;
        int maxMP = (int)PlayerObject.Health.MaxMana;
        if (mLastCurMP != curMP || mLastMaxMP != maxMP)
        {
            float mpRate = (float)curMP / maxMP;
            float barCurLength = mpRate * barMaxLength;
            MPBarFill.rectTransform.sizeDelta = new Vector2(barCurLength, MPBarFill.rectTransform.sizeDelta.y);
            MPValue.SetText("{0} / {1}", curMP, maxMP);

            mLastCurMP = curMP;
            mLastMaxMP = maxMP;
        }

        int curExp = (int)(mExperience.CurrentExp - mExperience.ExpAtLevelStart);
        int maxExp = (int)(mExperience.ExpForNextLevel - mExperience.ExpAtLevelStart);
        if (mLastCurExp != curExp || mLastMaxExp != maxExp)
        {
            float expRate = (float)curExp / maxExp;
            float barCurLengthExp = expRate * barMaxLength;
            ExpBarFill.rectTransform.sizeDelta = new Vector2(barCurLengthExp, ExpBarFill.rectTransform.sizeDelta.y);
            ExpValue.SetText("{0} / {1}", curExp, maxExp);

            mLastCurExp = curExp;
            mLastMaxExp = maxExp;
        }

        int curGold = (int)mInven.CurrentGold;
        if (mLastGold != curGold)
        {
            GoldText.SetText("{0}", curGold);

            mLastGold = curGold;
        }
        int curlifePotion = (int)mInven.CurrentLifePotionCount;
        if (mLastLifePotion != curlifePotion)
        {
            LifePotion.SetText("{0}", curlifePotion);

            mLastLifePotion = curlifePotion;
        }
        int curManaPotion = (int)mInven.CurrentManaPotionCount;
        if (mLastManaPotion != curManaPotion)
        {
            ManaPotion.SetText("{0}", curManaPotion);

            mLastManaPotion = curManaPotion;
        }
    }

    void UpdateSkillEquipSlots()
    {
        for (int i = 0; i < SkillEquipSlots.Length; i++)
        {
            Image skillIcon = SkillEquipSlots[i].GetChild(1).GetComponent<Image>();
            Image skillCooltime = SkillEquipSlots[i].GetChild(2).GetComponent<Image>();
            SkillBase equipSkill = mSkillController.GetEquipSkill(i);
            if (equipSkill != null)
            {
                skillIcon.sprite = equipSkill.Icon;
                skillCooltime.fillAmount = equipSkill.CooltimeRate;
            }
            else
            {
                skillIcon.sprite = null;
                skillCooltime.fillAmount = 0;
            }
        }
    }

    void ShowActionSelector()
    {
        UIActionSelector.Show(_ActionSelector, _ActionsParent, new string[] { "testA", "testB", "testC" }, (result) =>
        {
            LOG.trace(result);
        });
    }
}
