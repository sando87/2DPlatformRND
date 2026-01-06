using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;



#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;

#endif

namespace PahlBit
{
    public static partial class MyExtensions
    {

        public static UniTask ExDelayedTask(
            this MonoBehaviour mono,
            float delaySeconds,
            Action action,
            DelayType delayType = DelayType.DeltaTime,
            CancellationToken? externalToken = null)
        {
            var destroyToken = mono.GetCancellationTokenOnDestroy();

            CancellationToken ct = externalToken.HasValue
                ? CancellationTokenSource.CreateLinkedTokenSource(destroyToken, externalToken.Value).Token
                : destroyToken;

            return ExDelayTaskInternal(action, delaySeconds, delayType, ct);
        }

        static async UniTask ExDelayTaskInternal(
            Action action,
            float delaySeconds,
            DelayType delayType,
            CancellationToken ct)
        {
            try
            {
                await UniTask.Delay(
                    TimeSpan.FromSeconds(delaySeconds),
                    delayType,
                    cancellationToken: ct
                );
                action?.Invoke();
            }
            catch (OperationCanceledException)
            {
                // 정상 취소
            }
        }


    }
}