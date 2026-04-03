using PahlBit;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGamePanel : MonoBehaviour
{
    [SerializeField] Image HPBarFill = null;
    [SerializeField] Image MPBarFill = null;
    [SerializeField] Image ExpBarFill = null;
    [SerializeField] TextMeshProUGUI GoldText = null;
    [SerializeField] BaseObject PlayerObject = null;
    [SerializeField] Transform[] SkillEquipSlots = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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

        float hpRate = PlayerObject.Health.HpRate;
        float barCurLength = hpRate * barMaxLength;
        HPBarFill.rectTransform.sizeDelta = new Vector2(barCurLength, HPBarFill.rectTransform.sizeDelta.y);

        float mpRate = PlayerObject.Health.ManaRate;
        float barCurLengthMP = mpRate * barMaxLength;
        MPBarFill.rectTransform.sizeDelta = new Vector2(barCurLengthMP, MPBarFill.rectTransform.sizeDelta.y);

        Experience experience = PlayerObject.GetComponentInChildren<Experience>();
        float expRate = experience.CurrentExpRate;
        float barCurLengthExp = expRate * barMaxLength;
        ExpBarFill.rectTransform.sizeDelta = new Vector2(barCurLengthExp, ExpBarFill.rectTransform.sizeDelta.y);

        ItemInventory inven = PlayerObject.GetComponentInChildren<ItemInventory>();
        GoldText.text = inven.CurrentGold.ToString();
    }

    void UpdateSkillEquipSlots()
    {
        SkillController skillController = PlayerObject.GetComponentInChildren<SkillController>();
        for (int i = 0; i < SkillEquipSlots.Length; i++)
        {
            Image skillIcon = SkillEquipSlots[i].GetChild(1).GetComponent<Image>();
            SkillBase equipSkill = skillController.GetEquipSkill(i);
            if (equipSkill != null)
            {
                skillIcon.sprite = equipSkill.Icon;
            }
            else
            {
                skillIcon.sprite = null;
            }
        }
    }
}
