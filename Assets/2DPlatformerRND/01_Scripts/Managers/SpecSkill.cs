using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using NaughtyAttributes.Test;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public class SpecSkill : SpecBase
    {
        public float PhyAttack => mSpecPlayer.BaseAttack * (BaseStats.PhyAttack + mSpecPlayer.Option.PhyAttack);
        public float FireAttack => mSpecPlayer.BaseAttack * (BaseStats.FireAttack + mSpecPlayer.Option.FireAttack);
        public float IceAttack => mSpecPlayer.BaseAttack * (BaseStats.IceAttack + mSpecPlayer.Option.IceAttack);
        public float LightningAttack => mSpecPlayer.BaseAttack * (BaseStats.LightningAttack + mSpecPlayer.Option.LightningAttack);

        public float ManaUse => BaseStats.ManaUse;
        public float Cooltime => BaseStats.Cooltime * mSpecPlayer.Option.CooltimeDown;
        public float ProjectileCount => BaseStats.ProjectileCount + mSpecPlayer.Option.ProjectileCountUp;
        public float ProjectileSpeed => BaseStats.ProjectileSpeed * mSpecPlayer.Option.ProjectileSpeedUp;
        public float AttackRange => BaseStats.AttackRange * mSpecPlayer.Option.AttackRangeUp;
        public float SplashRange => BaseStats.SplashRange * mSpecPlayer.Option.SplashRangeUp;
        public float Duration => BaseStats.Duration * mSpecPlayer.Option.DurationUp;
        public float Interval => BaseStats.Interval;
        public float StartDelay => BaseStats.StartDelay;

        public SkillSaveData SaveData { get; private set; } = null;
        public SkillResourceData ResourceData { get; private set; } = null;
        public SkillStats BaseStats { get; private set; } = null;

        private SpecPlayer mSpecPlayer = null;

        public void Init(int characterID, string resourceID)
        {
            ResourceData = SkillResourceTable.Instance.GetInfo(resourceID);
            UserSaveData userSaveData = SaveFileManager<UserSaveData>.Load();
            SaveData = userSaveData.Characters[characterID].Skills[resourceID];

            mSpecPlayer = this.ExGetBase().PlayerObj.Spec;

            BaseStats = GetBasicStatByLevel(SaveData == null ? 0 : SaveData.LevelIndex);
        }

        public void UpdateBasicStat()
        {
            BaseStats = GetBasicStatByLevel(SaveData == null ? 0 : SaveData.LevelIndex);
        }

        SkillStats GetBasicStatByLevel(int currentLevelIndex)
        {
            SkillStats baseStats = new SkillStats();

            baseStats.ManaUse = ResourceData._ManaUse.GetValueByPoint(currentLevelIndex);
            baseStats.Cooltime = ResourceData._Cooltime.GetValueByPoint(currentLevelIndex);
            baseStats.ProjectileCount = ResourceData._ProjectileCount.GetValueByPoint(currentLevelIndex);
            baseStats.ProjectileSpeed = ResourceData._ProjectileSpeed.GetValueByPoint(currentLevelIndex);
            baseStats.AttackRange = ResourceData._AttackRange.GetValueByPoint(currentLevelIndex);
            baseStats.SplashRange = ResourceData._SplashRange.GetValueByPoint(currentLevelIndex);
            baseStats.Duration = ResourceData._Duration.GetValueByPoint(currentLevelIndex);
            baseStats.Interval = ResourceData._Interval.GetValueByPoint(currentLevelIndex);
            baseStats.StartDelay = ResourceData._StartDelay.GetValueByPoint(currentLevelIndex);
            baseStats.PhyAttack = (Percent)ResourceData._PhyAttack.GetValueByPoint(currentLevelIndex);
            baseStats.FireAttack = (Percent)ResourceData._FireAttack.GetValueByPoint(currentLevelIndex);
            baseStats.IceAttack = (Percent)ResourceData._IceAttack.GetValueByPoint(currentLevelIndex);
            baseStats.LightningAttack = (Percent)ResourceData._LightningAttack.GetValueByPoint(currentLevelIndex);

            return baseStats;
        }

        public DamageInfo CalcCurrentDamages()
        {
            DamageInfo damageInfo = new DamageInfo();
            damageInfo.PhyDamage = PhyAttack;
            damageInfo.FireDamage = FireAttack;
            damageInfo.IceDamage = IceAttack;
            damageInfo.LightningDamage = LightningAttack;
            damageInfo.IsCritical = MyUtils.IsPercentHit((int)mSpecPlayer.Option.CriticalRate);
            damageInfo.CriticalAttackUp = mSpecPlayer.Option.CriticalAttack;
            return damageInfo;
        }

        public void GetDisplayInfo(List<FieldData> fieldDatas)
        {
            fieldDatas.Clear();

            if (PhyAttack > 0) fieldDatas.Add(new FieldData() { Name = nameof(PhyAttack), Value = ((int)PhyAttack).ToString() });
            if (FireAttack > 0) fieldDatas.Add(new FieldData() { Name = nameof(FireAttack), Value = ((int)FireAttack).ToString() });
            if (IceAttack > 0) fieldDatas.Add(new FieldData() { Name = nameof(IceAttack), Value = ((int)IceAttack).ToString() });
            if (LightningAttack > 0) fieldDatas.Add(new FieldData() { Name = nameof(LightningAttack), Value = ((int)LightningAttack).ToString() });
            if (ManaUse > 0) fieldDatas.Add(new FieldData() { Name = nameof(ManaUse), Value = ((int)ManaUse).ToString() });
            if (Cooltime > 0) fieldDatas.Add(new FieldData() { Name = nameof(Cooltime), Value = $"{Cooltime:0.##}s" });
            if (ProjectileCount > 0) fieldDatas.Add(new FieldData() { Name = nameof(ProjectileCount), Value = ((int)ProjectileCount).ToString() });
            if (ProjectileSpeed > 0) fieldDatas.Add(new FieldData() { Name = nameof(ProjectileSpeed), Value = $"{ProjectileSpeed:0.##}" });
            if (AttackRange > 0) fieldDatas.Add(new FieldData() { Name = nameof(AttackRange), Value = $"{AttackRange:0.##}" });
            if (SplashRange > 0) fieldDatas.Add(new FieldData() { Name = nameof(SplashRange), Value = $"{SplashRange:0.##}" });
            if (Duration > 0) fieldDatas.Add(new FieldData() { Name = nameof(Duration), Value = $"{Duration:0.##}s" });
            if (Interval > 0) fieldDatas.Add(new FieldData() { Name = nameof(Interval), Value = $"{Interval:0.##}s" });
            if (StartDelay > 0) fieldDatas.Add(new FieldData() { Name = nameof(StartDelay), Value = $"{StartDelay:0.##}s" });
        }

        public void GetBasicStatInfo(List<FieldData> fieldDatas)
        {
            fieldDatas.Clear();

            SkillStats nextBaseStats = GetBasicStatByLevel(SaveData.LevelIndex + 1);

            if (BaseStats.PhyAttack.PercentValue > 0)
                fieldDatas.Add(new FieldData()
                {
                    Name = nameof(BaseStats.PhyAttack),
                    Value = BaseStats.PhyAttack.ToString() + " -> " + nextBaseStats.PhyAttack.ToString()
                });

            if (BaseStats.FireAttack.PercentValue > 0)
                fieldDatas.Add(new FieldData()
                {
                    Name = nameof(BaseStats.FireAttack),
                    Value = BaseStats.FireAttack.ToString() + " -> " + nextBaseStats.FireAttack.ToString()
                });

            if (BaseStats.IceAttack.PercentValue > 0)
                fieldDatas.Add(new FieldData()
                {
                    Name = nameof(BaseStats.IceAttack),
                    Value = BaseStats.IceAttack.ToString() + " -> " + nextBaseStats.IceAttack.ToString()
                });

            if (BaseStats.LightningAttack.PercentValue > 0)
                fieldDatas.Add(new FieldData()
                {
                    Name = nameof(BaseStats.LightningAttack),
                    Value = BaseStats.LightningAttack.ToString() + " -> " + nextBaseStats.LightningAttack.ToString()
                });

            if (BaseStats.ManaUse > 0)
                fieldDatas.Add(new FieldData()
                {
                    Name = nameof(BaseStats.ManaUse),
                    Value = BaseStats.ManaUse.ToString() + " -> " + nextBaseStats.ManaUse.ToString()
                });

            if (BaseStats.Cooltime > 0)
                fieldDatas.Add(new FieldData()
                {
                    Name = nameof(BaseStats.Cooltime),
                    Value = BaseStats.Cooltime.ToString("0.##") + "s -> " + nextBaseStats.Cooltime.ToString("0.##") + "s"
                });

            if (BaseStats.ProjectileCount > 0)
                fieldDatas.Add(new FieldData()
                {
                    Name = nameof(BaseStats.ProjectileCount),
                    Value = BaseStats.ProjectileCount.ToString() + " -> " + nextBaseStats.ProjectileCount.ToString()
                });

            if (BaseStats.ProjectileSpeed > 0)
                fieldDatas.Add(new FieldData()
                {
                    Name = nameof(BaseStats.ProjectileSpeed),
                    Value = BaseStats.ProjectileSpeed.ToString("0.##") + " -> " + nextBaseStats.ProjectileSpeed.ToString("0.##")
                });

            if (BaseStats.AttackRange > 0)
                fieldDatas.Add(new FieldData()
                {
                    Name = nameof(BaseStats.AttackRange),
                    Value = BaseStats.AttackRange.ToString("0.##") + " -> " + nextBaseStats.AttackRange.ToString("0.##")
                });

            if (BaseStats.SplashRange > 0)
                fieldDatas.Add(new FieldData()
                {
                    Name = nameof(BaseStats.SplashRange),
                    Value = BaseStats.SplashRange.ToString("0.##") + " -> " + nextBaseStats.SplashRange.ToString("0.##")
                });

            if (BaseStats.Duration > 0)
                fieldDatas.Add(new FieldData()
                {
                    Name = nameof(BaseStats.Duration),
                    Value = BaseStats.Duration.ToString("0.##") + "s -> " + nextBaseStats.Duration.ToString("0.##") + "s"
                });

            if (BaseStats.Interval > 0)
                fieldDatas.Add(new FieldData()
                {
                    Name = nameof(BaseStats.Interval),
                    Value = BaseStats.Interval.ToString("0.##") + "s -> " + nextBaseStats.Interval.ToString("0.##") + "s"
                });

            if (BaseStats.StartDelay > 0)
                fieldDatas.Add(new FieldData()
                {
                    Name = nameof(BaseStats.StartDelay),
                    Value = BaseStats.StartDelay.ToString("0.##") + "s -> " + nextBaseStats.StartDelay.ToString("0.##") + "s"
                });
        }
    }
}