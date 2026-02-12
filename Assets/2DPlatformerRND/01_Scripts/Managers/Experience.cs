
using UnityEngine;
using UnityEngine.Events;

namespace PahlBit
{
    public class Experience : MonoBehaviour
    {
        private float mFromExp = 0;
        private float mToExp = 0;
        private CharSaveData mCharacterSaveData = null;

        public int CurrentLevel { get; private set; } = 0;
        public int CurrentLevelIdx { get => CurrentLevel - 1; }
        public float RemainExp { get { return mToExp - CurrentExp; } }
        public float CurrentExpRate { get { return (CurrentExp - mFromExp) / (mToExp - mFromExp); } }
        public float CurrentExp { get; private set; } = 0;

        public UnityEvent OnLevelUp = new UnityEvent();

        void Start()
        {
            BattleDispatcher battleDispatcher = this.ExGetBase().GetComponentInChildren<BattleDispatcher>();
            if (battleDispatcher != null)
            {
                battleDispatcher.EventOnKillResult.AddListener((result) =>
                {
                    if (result.IsKilled)
                    {
                        EnemyBase enemy = result.Target.ExGetBase().EnemyObj;
                        if (enemy != null)
                        {
                            float gainedExp = enemy.Spec.ExpOnDeath;
                            AddExp(gainedExp);
                        }
                    }
                });
            }
        }

        public void Init(int characterID)
        {
            UserSaveData userSaveData = SaveFileManager<UserSaveData>.Load();
            mCharacterSaveData = userSaveData.Characters[characterID].Stats;
            CurrentExp = mCharacterSaveData.CurrentExp;
            CurrentLevel = GameSystem.CurrentExpToLevel(mCharacterSaveData.CurrentExp);

            mFromExp = GameSystem.GetNextExpForLevelup(CurrentLevel - 1);
            mToExp = GameSystem.GetNextExpForLevelup(CurrentLevel);
        }

        public void AddExp(float exp)
        {
            CurrentExp += exp;
            mCharacterSaveData.CurrentExp = CurrentExp;
            GameSystem.RequestSave();

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