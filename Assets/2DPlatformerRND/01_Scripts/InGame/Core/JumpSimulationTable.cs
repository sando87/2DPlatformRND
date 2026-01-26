using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Tilemaps;
using System.Collections.Generic;


namespace PahlBit
{
    public static class JumpSimulationTable
    {
        static Dictionary<int, float[]> mJumpTable = new Dictionary<int, float[]>();

        static void InitJumpTable()
        {
            // 점프 시작할때 받는 힘에 따라 도착 높이까지 떨어지는데 걸리는 시간 테이블(미리 시뮬레이션을 해서 구한 실험 데이터)
            // 환경조건 => Gravity: -9.81, Mass: 1, GravityScale: 5, LinearDamping: 1
            // 점프힘별로 (도착상대높이, 체공시간) 데이터 산출된 값들..

            // 점프힘 25일때 최대높이 4.7, 피크까지시간 0.41s
            // (4.00, 0.58),(3.00, 0.68),(2.00, 0.76),(1.00, 0.82),(0.00, 0.88),(-1.00, 0.93),(-2.00, 0.98),(-3.00, 1.02),(-4.00, 1.07),(-5.00, 1.11),(-6.00, 1.15)

            // 힘 22점프, 높이 3.73, 피크까지시간 0.37s
            // (3.00, 0.54),(2.00, 0.64),(1.00, 0.72),(0.00, 0.78),(-1.00, 0.84),(-2.00, 0.89),(-3.00, 0.94),(-4.00, 0.98),(-5.00, 1.03),(-6.00, 1.07)

            // 힘 18점프, 높이 2.59, 피크까지시간 0.31s
            // (2.00, 0.46),(1.00, 0.57),(0.00, 0.65),(-1.00, 0.71),(-2.00, 0.77),(-3.00, 0.82),(-4.00, 0.87),(-5.00, 0.92),(-6.00, 0.96)

            // 힘 14점프, 높이 1.62, 피크까지시간 0.25s
            // (1.00, 0.41),(0.00, 0.51),(-1.00, 0.59),(-2.00, 0.65),(-3.00, 0.71),(-4.00, 0.76),(-5.00, 0.81),(-6.00, 0.86)

            //                   점프힘      14    18     22     25
            mJumpTable[4] = new float[] { -1.0f, -1.0f, -1.0f, 0.58f };
            mJumpTable[3] = new float[] { -1.0f, -1.0f, 0.54f, 0.68f };
            mJumpTable[2] = new float[] { -1.0f, 0.46f, 0.64f, 0.76f };
            mJumpTable[1] = new float[] { 0.41f, 0.57f, 0.72f, 0.82f };
            mJumpTable[0] = new float[] { 0.51f, 0.65f, 0.78f, 0.88f };
            mJumpTable[-1] = new float[] { 0.59f, 0.71f, 0.84f, 0.93f };
            mJumpTable[-2] = new float[] { 0.65f, 0.77f, 0.89f, 0.98f };
            mJumpTable[-3] = new float[] { 0.71f, 0.82f, 0.94f, 1.02f };
            mJumpTable[-4] = new float[] { 0.76f, 0.87f, 0.98f, 1.07f };
            mJumpTable[-5] = new float[] { 0.81f, 0.92f, 1.03f, 1.11f };
            mJumpTable[-6] = new float[] { 0.86f, 0.96f, 1.07f, 1.15f };
            mJumpTable[-7] = new float[] { 0.90f, 1.00f, 1.11f, 1.18f };
            mJumpTable[-8] = new float[] { 0.94f, 1.04f, 1.14f, 1.21f };
            mJumpTable[-9] = new float[] { 0.97f, 1.08f, 1.17f, 1.24f };
            mJumpTable[-10] = new float[] { 1.00f, 1.11f, 1.20f, 1.26f };
        }

        static float GetJumpForce(int destOffsetY, float requiredTimeToReach)
        {
            if (mJumpTable.Count == 0)
            {
                InitJumpTable();
            }

            if (destOffsetY < -10 || destOffsetY > 4)
            {
                return -1f;
            }

            float[] timeTable = mJumpTable[destOffsetY];
            int idx = 0;
            for (idx = 0; idx < timeTable.Length; idx++)
            {
                float maxFloatingTime = timeTable[idx];
                if (maxFloatingTime < 0)
                    continue;

                if (requiredTimeToReach < maxFloatingTime)
                    return idx == 0 ? 14f : (idx == 1 ? 18f : (idx == 2 ? 22f : 25f));
            }

            return -1f;
        }

        // 현재 이동속도로 도착지점까지 점프로 이동 가능한지 여부 판단 및 가능하다면 필요한 점프값 반환
        public static bool IsPossibleJump(Vector2Int startPos, Vector2Int destPos, float horizontalMoveSpeed, out float requiredJumpForce)
        {
            int offsetY = destPos.y - startPos.y;
            float distanceX = Mathf.Abs(destPos.x - startPos.x);
            float timeToReach = distanceX / Mathf.Abs(horizontalMoveSpeed);
            float jumpForce = GetJumpForce(offsetY, timeToReach);
            requiredJumpForce = jumpForce;
            return jumpForce > 0f;
        }




        // 점프 궤적 데이터 수집을 위한 시뮬레이션 함수
        static List<Vector2> SimulateJumpTrajectory(
            Vector2 startPosition,
            float impulse,
            float mass,
            float gravityScale,
            float linearDamping,
            float velocityX,
            float totalTime)
        {
            List<Vector2> positions = new();

            float dt = Time.fixedDeltaTime;
            // float dt = 0.01f;

            // 초기 상태
            Vector2 pos = startPosition;
            Vector2 vel = Vector2.up * (impulse / mass);

            // int startY = (int)startPosition.y;
            // float peakY = 0;
            // int refY = 0;

            // 중력
            Vector2 gravity = new Vector2(0f, -9.81f) * gravityScale;

            float elapsed = 0f;
            // List<Vector2> points = new List<Vector2>();

            while (elapsed < totalTime)
            {
                // 중력 가속
                vel += gravity * dt;

                // Linear Damping (Rigidbody2D.drag)
                vel *= 1f / (1f + linearDamping * dt);

                vel.x = velocityX;

                // if(vel.y < 0)
                // {
                //     if(peakY == 0)
                //     {
                //         peakY = pos.y;
                //         refY = (int)peakY;
                //         LOG.trace("Peak Y at: " + (peakY - startPosition.y).ToString("F2") + "m");
                //         LOG.trace("Time to Peak: " + elapsed.ToString("F2") + "s");
                //     }
                // }

                // 위치 적분
                pos += vel * dt;

                // if(vel.y < 0)
                // {
                //     if(pos.y < refY)
                //     {
                //         int dy = refY - startY;
                //         // LOG.trace(dy + " : " + elapsed.ToString("F2") + "s");
                //         points.Add(new Vector2(dy, elapsed));
                //         refY--;
                //     }
                // }

                positions.Add(pos);

                elapsed += dt;
            }

            // string result = string.Join(",", points);
            // Debug.Log(result);

            return positions;
        }

        public static void DrawSimulationPoints(Vector2 startPosition, float jumpForce)
        {
            var trajectory = SimulateJumpTrajectory(
                startPosition: startPosition,
                impulse: jumpForce,
                mass: 1f,
                gravityScale: 5f,
                linearDamping: 1f,
                velocityX: 7f,
                totalTime: 1.5f
            );

            for (int i = 0; i < trajectory.Count - 1; i++)
            {
                bool isUp = trajectory[i + 1].y > trajectory[i].y;
                Color lineColor = isUp ? Color.red : Color.blue;
                Debug.DrawLine(trajectory[i], trajectory[i] + new Vector2(0.2f, 0), lineColor, 5f);
            }
        }

    }
}