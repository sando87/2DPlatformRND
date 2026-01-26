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

        public void Init(Tilemap tilemap, Tilemap thinTilemap)
        {
            BoundsInt bounds = tilemap.cellBounds;
            BoundsInt thinBounds = thinTilemap.cellBounds;
            bounds.min = Vector3Int.Min(bounds.min, thinBounds.min);
            bounds.max = Vector3Int.Max(bounds.max, thinBounds.max);
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
                            groundNodeGroup.IsThinPlatform = false;
                        }

                        NodeNav newNode = new NodeNav(pos);
                        newNode.ParentGroup = groundNodeGroup;
                        newNode.IndexInGroup = groundNodeGroup.GroundNodes.Count;
                        newNode.IsThin = false;
                        groundNodeGroup.GroundNodes.Add(newNode);
                        mGroundNodes[pos] = newNode;
                        pos.x++;
                    }

                    while (IsThinTile(thinTilemap, pos))
                    {
                        if (groundNodeGroup == null)
                        {
                            groundNodeGroup = new NodeNavGroup();
                            groundNodeGroup.IsThinPlatform = true;
                        }

                        NodeNav newNode = new NodeNav(pos);
                        newNode.ParentGroup = groundNodeGroup;
                        newNode.IndexInGroup = groundNodeGroup.GroundNodes.Count;
                        newNode.IsThin = true;
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
        bool IsThinTile(Tilemap tilemap, Vector2Int position)
        {
            Vector3Int pos = new Vector3Int(position.x, position.y, 0);
            return tilemap.gameObject.layer == LayerID.ThinPlatform && tilemap.HasTile(pos);
        }

        void LinkGroups(NodeNavGroup newGroup)
        {
            Rect newRect = newGroup.GetRect();
            newRect.min -= new Vector2(14, 9);
            newRect.max += new Vector2(14, 9);
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

        public PathInfo FindPath(Vector2 worldPos, float moveSpeed)
        {
            NodeNav currentNode = GetCurrentGroundNode(worldPos);
            if (currentNode == null)
                return null;

            NodeNavGroup currentGroup = currentNode.ParentGroup;
            List<PathInfo> possiblePaths = new List<PathInfo>();
            foreach (var transition in currentGroup.Transitions)
            {
                if (transition.TransitionType == NodeTransitionType.JustJumpUp)
                {
                    bool isPossibleJump = JumpSimulationTable.IsPossibleJump(
                        startPos: transition.StartNode.Position,
                        destPos: transition.EndNode.Position,
                        horizontalMoveSpeed: moveSpeed,
                        out float requiredJumpForce
                    );

                    if (isPossibleJump)
                    {
                        NodeNav endNode = transition.EndNode.ParentGroup.GetNodeAtWorldPosX(currentNode.Position.x);
                        bool isNoNeedToMove = endNode != null;

                        PathInfo pathInfo = new PathInfo();
                        pathInfo.Transition = transition;
                        pathInfo.JumpForce = requiredJumpForce;
                        pathInfo.IsNoNeedToMove = isNoNeedToMove;
                        possiblePaths.Add(pathInfo);
                    }
                }
                else if (transition.TransitionType == NodeTransitionType.DropDown)
                {
                    NodeNav endNode = transition.EndNode.ParentGroup.GetNodeAtWorldPosX(currentNode.Position.x);
                    bool isNoNeedToMove = endNode != null;

                    PathInfo pathInfo = new PathInfo();
                    pathInfo.Transition = transition;
                    pathInfo.IsNoNeedToMove = isNoNeedToMove;
                    possiblePaths.Add(pathInfo);
                }
                else
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
                        pathInfo.JumpForce = requiredJumpForce;
                        possiblePaths.Add(pathInfo);
                    }
                }
            }

            PathInfo selectedPath = null;
            int minDepth = int.MaxValue;
            foreach (var path in possiblePaths)
            {
                Vector2Int startPos = path.Transition.StartNode.Position;
                Vector2Int endPos = path.Transition.EndNode.Position;
                PlayerDepthInfo depthInfo = PlayerDepthManager.Instance.GetPlayerDepthInfoAtPos(endPos + new Vector2Int(0, 1));
                if (depthInfo == null || depthInfo.IsOld)
                    continue;

                int curDepth = depthInfo.GetDepth();
                if (curDepth < minDepth)
                {
                    minDepth = curDepth;
                    selectedPath = path;
                }
            }
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
        public float JumpForce { get; set; }
        public bool IsNoNeedToMove { get; set; } = false;
    }
}