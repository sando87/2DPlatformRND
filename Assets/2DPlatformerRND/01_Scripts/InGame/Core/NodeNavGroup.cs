using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;


namespace PahlBit
{
    public class NodeNavGroup
    {
        public bool IsThingPlatform { get; set; } = false;
        public List<NodeNav> GroundNodes = new List<NodeNav>();
        public List<NodeNavGroup> LinkedGroups = new List<NodeNavGroup>();
        public Dictionary<NodeNavGroup, List<NodeTransition>> Transitions = new Dictionary<NodeNavGroup, List<NodeTransition>>();

        public NodeNav MostLeftNode { get => GroundNodes[0]; }
        public NodeNav MostRightNode { get => GroundNodes[GroundNodes.Count - 1]; }

        public void InitTransitions()
        {
            Transitions.Clear();
            foreach (var linkedGroup in LinkedGroups)
            {
                List<NodeTransition> transitions = new List<NodeTransition>();
                bool hasTransitions = CreateTransitions(linkedGroup, transitions);
                if (!hasTransitions)
                    continue;

                Transitions[linkedGroup] = transitions;
            }
        }

        // 이게 핵심 알고리즘...
        // 주변 지형과의 위치나 높이에 따라 이동 가능 판단 및 이동 속성 부여
        // 주변 지형의 다양도에 따라 구현 난이도 상승..
        bool CreateTransitions(NodeNavGroup targetGroup, List<NodeTransition> transitions)
        {
            // 기본적으로 점프 높이는 3칸으로 고정하고 좌우 이동속도는 초당5칸으로 고정
            float jumpHeight = 3f;
            float moveSpeed = 5f;

            Rect targetRect = targetGroup.GetRect();
            Rect myRect = GetRect();

            // case 1: 타겟이 완전히 왼쪽으로 나가 있는 경우
            if (targetRect.xMax < myRect.xMin)
            {
                NodeNav targetMostRightNode = targetGroup.MostRightNode;
                NodeNav myMostLeftNode = MostLeftNode;
                return false;
            }
            // case 2: 타겟이 완전히 오른쪽으로 나가 있는 경우
            else if (myRect.xMax < targetRect.xMin)
            {
                return false;
            }
            // case 3: 타겟이 나의 왼쪽만 겹치는 경우
            else if (targetRect.xMin <= myRect.xMin && myRect.xMin <= targetRect.xMax && targetRect.xMax < myRect.xMax)
            {
                NodeTransition transition = new NodeTransition();
                transitions.Add(transition);
                return transitions.Count > 0;
            }
            // case 4: 타겟이 나의 오른쪽만 겹치는 경우
            else if (myRect.xMin < targetRect.xMin && targetRect.xMin <= myRect.xMax && myRect.xMax <= targetRect.xMax)
            {
                NodeTransition transition = new NodeTransition();
                transitions.Add(transition);
                return transitions.Count > 0;
            }
            // case 5: 타겟이 내 안에 포함된 있는 경우
            else if (myRect.xMin < targetRect.xMin && targetRect.xMax < myRect.xMax)
            {
            }
            // case 6: 타겟안에 내가 포함된 경우
            else if (targetRect.xMin <= myRect.xMin && myRect.xMax <= targetRect.xMax)
            {
                NodeTransition transition = new NodeTransition();
                transitions.Add(transition);
                return transitions.Count > 0;
            }
            return transitions.Count > 0;
        }

        public Rect GetRect()
        {
            Rect rect = new Rect();
            rect.min = GroundNodes[0].MinPos;
            rect.max = GroundNodes[GroundNodes.Count - 1].MaxPos;
            return rect;
        }

    }
}