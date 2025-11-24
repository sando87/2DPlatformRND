using UnityEngine;

namespace PahlBit
{
    public static class GameSystem
    {
        static public void DoSave_UserSaveData()
        {
            UserSaveData saveData = SaveFileManager<UserSaveData>.Load();
            SaveFileManager<UserSaveData>.Save(saveData);
        }
        static public int CurrentExpToLevel(double accumulatedExp)
        {
            return (int)(accumulatedExp / 100) + 1;
        }
        static public double GetNextExpForLevelup(int level)
        {
            return level * 100;
        }
    }
}