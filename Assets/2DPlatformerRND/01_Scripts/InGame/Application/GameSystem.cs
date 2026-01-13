using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

namespace PahlBit
{
    public static class GameSystem
    {
        static float SaveRequestedTime = 0;

        static public void DoSave_UserSaveData()
        {
            UserSaveData saveData = SaveFileManager<UserSaveData>.Load();
            SaveFileManager<UserSaveData>.Save(saveData);
        }

        // 바로 세이브 하지 않고 일정 시간 동안 다시 RequestSave() 호출이 없으면 그때 세이브 하는 방식
        // 골드나 경험치 같이 자주 변경되는 데이터를 위한 세이브 방식
        static public void RequestSave()
        {
            bool isAlreadyRequested = SaveRequestedTime != 0;
            SaveRequestedTime = Time.time;
            if (!isAlreadyRequested)
            {
                WaitSaveUntilBreak().Forget();
            }
        }
        static async UniTask WaitSaveUntilBreak()
        {
            while (Time.time - SaveRequestedTime < 3.0f)
            {
                await UniTask.DelayFrame(3);
            }
            SaveRequestedTime = 0;
            DoSave_UserSaveData();
        }
        static public int CurrentExpToLevel(float accumulatedExp)
        {
            return (int)(accumulatedExp / 100) + 1;
        }
        static public float GetNextExpForLevelup(int level)
        {
            return level * 100;
        }
    }
}