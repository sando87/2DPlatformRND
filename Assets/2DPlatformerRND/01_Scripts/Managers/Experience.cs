
using UnityEngine;
using UnityEngine.Events;

namespace PahlBit
{
    public class Experience : MonoBehaviour
    {
        public CharacterRoot CharRoot => GetComponentInParent<CharacterRoot>();

        private double mFromExp = 0;
        private double mToExp = 0;
        private CharSaveData mCharacterSaveData = null;

        public int CurrentLevel { get; private set; } = 0;
        public int CurrentLevelIdx { get => CurrentLevel - 1; }
        public double RemainExp { get { return mToExp - CurrentExp; } }
        public double CurrentExpRate { get { return (CurrentExp - mFromExp) / (mToExp - mFromExp); } }
        public double CurrentExp { get; private set; } = 0;

        public UnityEvent OnLevelUp = new UnityEvent();

        void Awake()
        {
            Init();
        }

        void Start()
        {
            BattleDispatcher battleDispatcher = this.ExGetBase().GetComponentInChildren<BattleDispatcher>();
            if (battleDispatcher != null)
            {
                battleDispatcher.EventOnKillResult.AddListener((result) =>
                {
                    if (result.IsKilled)
                    {
                        float gainedExp = result.Target.ExGetBase().GetComponentInChildren<EnemyDataMono>().Data.Stats.ExpOnDeath;
                        AddExp(gainedExp);
                    }
                });
            }
        }

        public void Init()
        {
            UserSaveData userSaveData = SaveFileManager<UserSaveData>.Load();
            mCharacterSaveData = userSaveData.Characters[CharRoot.CharacterID].Stats;
            CurrentExp = mCharacterSaveData.CurrentExp;
            CurrentLevel = GameSystem.CurrentExpToLevel(mCharacterSaveData.CurrentExp);

            mFromExp = GameSystem.GetNextExpForLevelup(CurrentLevel - 1);
            mToExp = GameSystem.GetNextExpForLevelup(CurrentLevel);
        }

        public void AddExp(double exp)
        {
            CurrentExp += exp;
            mCharacterSaveData.CurrentExp = CurrentExp;

            while (CurrentExp >= mToExp)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            CurrentLevel += 1;
            mFromExp = mToExp;
            mToExp = GameSystem.GetNextExpForLevelup(CurrentLevel);

            OnLevelUp?.Invoke();

            GameSystem.DoSave_UserSaveData();
        }


    }

}