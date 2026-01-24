using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.IO;


namespace PahlBit
{
    public class PlatformerPathfinder
    {
        Dictionary<Vector2Int, NodeNav> mGroundNodes = new Dictionary<Vector2Int, NodeNav>();
        List<NodeNavGroup> mNodeGroups = new List<NodeNavGroup>();

        public void Init(Tilemap tilemap)
        {
            BoundsInt bounds = tilemap.cellBounds;
            NodeNavGroup groundNodeGroup = null;

            for (int x = bounds.min.x; x < bounds.max.x; x++)
            {
                for (int y = bounds.min.y; y < bounds.max.y; y++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    if (mGroundNodes.ContainsKey(pos))
                        continue;

                    while (IsGroundTile(tilemap, pos))
                    {
                        if (groundNodeGroup == null)
                        {
                            groundNodeGroup = new NodeNavGroup();
                        }

                        NodeNav newNode = new NodeNav(pos);
                        newNode.ParentGroup = groundNodeGroup;
                        newNode.IndexInGroup = groundNodeGroup.GroundNodes.Count;
                        groundNodeGroup.GroundNodes.Add(newNode);
                        mGroundNodes[pos] = newNode;
                        pos.x++;
                    }

                    if (groundNodeGroup != null)
                    {
                        LinkGroups(groundNodeGroup);
                        mNodeGroups.Add(groundNodeGroup);
                        groundNodeGroup = null;
                    }
                }
            }

            InitTransitions();
        }

        void InitTransitions()
        {
            foreach (var group in mNodeGroups)
            {
                group.InitTransitions();
            }
        }

        bool IsGroundTile(Tilemap tilemap, Vector2Int position)
        {
            Vector3Int pos = new Vector3Int(position.x, position.y, 0);
            return tilemap.HasTile(pos) && !tilemap.HasTile(pos + Vector3Int.up);
        }

        void LinkGroups(NodeNavGroup newGroup)
        {
            Rect newRect = newGroup.GetRect();
            newRect.min -= new Vector2(12, 8);
            newRect.max += new Vector2(12, 8);
            foreach (NodeNavGroup group in mNodeGroups)
            {
                Rect groupRect = group.GetRect();
                if (newRect.Overlaps(groupRect))
                {
                    newGroup.LinkedGroups.Add(group);
                    group.LinkedGroups.Add(newGroup);
                }
            }
        }

        public PathInfo GetNextPath(Vector2 worldPos, float moveSpeed)
        {
            NodeNav currentNode = GetCurrentGroundNode(worldPos);
            if (currentNode == null)
                return null;

            NodeNavGroup currentGroup = currentNode.ParentGroup;
            List<PathInfo> possiblePaths = new List<PathInfo>();
            foreach (var transition in currentGroup.Transitions)
            {
                bool isPossibleJump = JumpSimulationTable.IsPossibleJump(
                    startPos: transition.StartNode.Position,
                    destPos: transition.EndNode.Position,
                    horizontalMoveSpeed: moveSpeed,
                    out float requiredJumpForce
                );

                if (isPossibleJump)
                {
                    PathInfo pathInfo = new PathInfo();
                    pathInfo.Transition = transition;
                    pathInfo.jumpForce = requiredJumpForce;
                    possiblePaths.Add(pathInfo);
                }
            }

            PathInfo selectedPath = possiblePaths.Count > 0 ? possiblePaths[UnityEngine.Random.Range(0, possiblePaths.Count)] : null;
            return selectedPath;
        }

        public NodeNav GetCurrentGroundNode(Vector2 worldPos)
        {
            Vector2Int nodePos = new Vector2Int(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y) - 1);
            if (mGroundNodes.ContainsKey(nodePos))
                return mGroundNodes[nodePos];
            return null;
        }
    }

    public class PathInfo
    {
        public NodeTransition Transition { get; set; }
        public float jumpForce { get; set; }
    }
}