using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PahlBit;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyAIFlying : EnemyAI
{
    protected override async UniTask<EnemyState> PatrolMode(CancellationToken ctx)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(100), cancellationToken: ctx);
            return EnemyState.Patrol;
        }
        finally
        {
        }
    }

}
