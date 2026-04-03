using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace PahlBit
{
    public class Consts
    {
        public const int PointByLevelup = 5;
    }


    public partial struct AnimStateNameHash
    {
        public static readonly int ExitDummy = Animator.StringToHash("ExitDummy");

        public static readonly int Idle = Animator.StringToHash("Idle");
        public static readonly int Run = Animator.StringToHash("Run");
        public static readonly int Jump = Animator.StringToHash("Jump");
        public static readonly int Attack = Animator.StringToHash("Attack");
        public static readonly int Hit = Animator.StringToHash("Hit");
        public static readonly int Hert = Animator.StringToHash("Hert");
        public static readonly int WakeUp = Animator.StringToHash("WakeUp");
        public static readonly int Damaged = Animator.StringToHash("Damaged");
        public static readonly int Death = Animator.StringToHash("Death");
        public static readonly int Respawn = Animator.StringToHash("Respawn");

        public static readonly int Skill = Animator.StringToHash("Skill");
        public static readonly int Skill1 = Animator.StringToHash("Skill1");
        public static readonly int Skill2 = Animator.StringToHash("Skill2");
        public static readonly int Dash = Animator.StringToHash("Dash");
        public static readonly int HitFlying = Animator.StringToHash("HitFlying");
        public static readonly int HitStrong = Animator.StringToHash("HitStrong");

        public static readonly int MeleeA = Animator.StringToHash("MeleeA");
        public static readonly int MeleeB = Animator.StringToHash("MeleeB");
        public static readonly int MeleeC = Animator.StringToHash("MeleeC");
        public static readonly int MeleeD = Animator.StringToHash("MeleeD");

        public static readonly int UpperIdle = Animator.StringToHash("UpperIdle");
        public static readonly int UpperAttack = Animator.StringToHash("UpperAttack");

        // Anims For Enemy
        public static readonly int Sleep = Animator.StringToHash("Sleep");
        public static readonly int Fly = Animator.StringToHash("Fly");

    }

    public static class AnimatorParams
    {
        public static readonly int MotionSpeed = Animator.StringToHash("MotionSpeed");
        public static readonly int DoNextCombo = Animator.StringToHash("DoNextCombo");
        public static readonly int StopLoop = Animator.StringToHash("StopLoop");
        public static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
        // public static readonly int IsGround = Animator.StringToHash("IsGround");
        // public static readonly int IsMoving = Animator.StringToHash("IsMoving");
    }

    public class LayerID
    {
        public static readonly int Terrain = LayerMask.NameToLayer("Terrain");
        public static readonly int Player = LayerMask.NameToLayer("Player");
        public static readonly int Enemy = LayerMask.NameToLayer("Enemy");
        public static readonly int Neutral = LayerMask.NameToLayer("Neutral");
        public static readonly int PlatformPlayer = LayerMask.NameToLayer("PlatformPlayer");
        public static readonly int PlayerObject = LayerMask.NameToLayer("PlayerObject");
        public static readonly int PlatformEnemy = LayerMask.NameToLayer("PlatformEnemy");
        public static readonly int Platform = LayerMask.NameToLayer("Platform");
        public static readonly int ThinPlatform = LayerMask.NameToLayer("ThinPlatform");
        public static readonly int StandableOnThin = LayerMask.NameToLayer("StandableOnThin");
    }

    public class MyLayerMask
    {
        public static readonly int Ground = 1 << LayerID.Terrain | 1 << LayerID.ThinPlatform;
    }
    public class StringHashes
    {
        public static readonly int ColorBurn = "ColorBurn".GetHashCode();
        public static readonly int ColorFreez = "ColorFreez".GetHashCode();
    }

    public interface IReactableFire
    {
        void OnReactFire(ElementFireAffector affector);
    }
    public interface IReactableLaser
    {
        Vector2 ReflectPos { get; }
        Vector2 ReflectDir { get; }

        void OnReactLaserReflection(ProjectileBase affectorLaser);
    }

}