using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.IO;


namespace PahlBit
{
    public class PlayerDepthManager : MonoBehaviour
    {
        const int DepthWidthRange = 18;
        const int DepthHeightRange = 10;

        // 방향 벡터 (상하좌우)
        Vector2Int[] mDirections =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        [SerializeField] Tilemap _Tilemap = null;

        GameObject mPlayer = null;

        // BFS용 큐
        Queue<Vector2Int> mQueue = new Queue<Vector2Int>();

        // 방문 체크 (depth 기록 여부)
        Dictionary<Vector2Int, int> mVisited = new Dictionary<Vector2Int, int>();

        Dictionary<Vector2Int, PlayerDepthInfo> mPlayerDepthInfo = new Dictionary<Vector2Int, PlayerDepthInfo>();

        public void SetPlayer(GameObject player)
        {
            mPlayer = player;
        }

        void Update()
        {
            if (mPlayer != null)
            {
                UpdatePlayerDepth(mPlayer.GetComponent<BaseObject>().Body.Center);
            }
        }

        public void UpdatePlayerDepth(Vector2 playerPosition)
        {
            if (_Tilemap == null)
                return;

            // 월드 → 그리드 좌표
            Vector3Int playerCell3D = _Tilemap.WorldToCell(playerPosition);
            Vector2Int playerCell = new Vector2Int(playerCell3D.x, playerCell3D.y);

            mQueue.Clear();
            mVisited.Clear();

            // 시작점
            mQueue.Enqueue(playerCell);
            mVisited[playerCell] = 0;

            while (mQueue.Count > 0)
            {
                Vector2Int current = mQueue.Dequeue();
                int currentDepth = mVisited[current];

                // 범위 제한
                int dx = Mathf.Abs(current.x - playerCell.x);
                int dy = Mathf.Abs(current.y - playerCell.y);

                if (dx > DepthWidthRange || dy > DepthHeightRange)
                    continue;

                // 막힌 칸은 기록도, 확산도 안 함
                if (IsBlocked(current))
                    continue;

                // Depth 정보 업데이트
                if (!mPlayerDepthInfo.TryGetValue(current, out PlayerDepthInfo info))
                {
                    info = new PlayerDepthInfo(current);
                    mPlayerDepthInfo.Add(current, info);
                }
                info.UpdateDepth(currentDepth);

                // 이웃 탐색
                foreach (var dir in mDirections)
                {
                    Vector2Int next = current + dir;

                    if (mVisited.ContainsKey(next))
                        continue;

                    mVisited[next] = currentDepth + 1;
                    mQueue.Enqueue(next);
                }
            }
        }

        bool IsBlocked(Vector2Int pos)
        {
            Vector3Int pos3D = new Vector3Int(pos.x, pos.y, 0);
            return _Tilemap.HasTile(pos3D);
        }

        public PlayerDepthInfo GetPlayerDepthInfoAtPos(Vector2Int pos)
        {
            if (mPlayerDepthInfo.TryGetValue(pos, out PlayerDepthInfo info))
            {
                return info;
            }
            return null;
        }

#if UNITY_EDITOR
        void OnGUI()
        {
            // DebugDrawDepthInfo();
        }
#endif

        void DebugDrawDepthInfo()
        {
            Camera mainCam = Camera.main;
            foreach (var kvp in mPlayerDepthInfo)
            {
                Vector2Int pos = kvp.Key;
                PlayerDepthInfo info = kvp.Value;

                Vector2 centerPos = pos + new Vector2(0.5f, 0.5f);
                Vector3 screenPos = mainCam.WorldToScreenPoint(centerPos);
                if (screenPos.z > 0)
                {
                    GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 20, 20), info.GetDepth().ToString());
                }

                // Vector3 worldPos = _Tilemap.CellToWorld(new Vector3Int(pos.x, pos.y, 0)) + new Vector3(0.5f, 0.5f, 0f);
                // Debug.DrawLine(worldPos, worldPos + Vector3.up * 0.5f, Color.Lerp(Color.green, Color.red, info.GetDepth() / 30f), 0.1f);
            }
        }

    }

    public class PlayerDepthInfo
    {
        const int MaxDepth = 30;

        public Vector2Int Position { get; private set; }
        float mDirtyTime = 0f;
        int mDepth = 0;
        public bool IsOld { get => (Time.time - mDirtyTime) > 5f; }

        public PlayerDepthInfo(Vector2Int position)
        {
            Position = position;
        }

        public void UpdateDepth(int depth)
        {
            mDepth = depth;
            mDepth.ExSetMaximum(MaxDepth);
            mDirtyTime = Time.time;
        }
        public int GetDepth()
        {
            return IsOld ? MaxDepth : mDepth;
        }
    }
}