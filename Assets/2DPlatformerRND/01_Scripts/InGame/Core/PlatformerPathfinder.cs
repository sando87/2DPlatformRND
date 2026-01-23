using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Tilemaps;
using System.Collections.Generic;


namespace PahlBit
{
    public class PlatformerPathfinder
    {
        Dictionary<Vector2Int, NodeNav> mGroundNodes = new Dictionary<Vector2Int, NodeNav>();
        List<NodeNavGroup> mNodeGroups = new List<NodeNavGroup>();

        void Init(Tilemap tilemap)
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

        public NodeTransition GetNextTransition(Vector2 worldPos)
        {
            NodeNav currentNode = GetCurrentGroundNode(worldPos);
            if (currentNode == null)
                return null;

            NodeNavGroup currentGroup = currentNode.ParentGroup;
            NodeNavGroup nextGroup = currentGroup.LinkedGroups[UnityEngine.Random.Range(0, currentGroup.LinkedGroups.Count)];
            List<NodeTransition> transitions = currentGroup.GetTransitions(nextGroup);
            if (transitions != null && transitions.Count > 0)
            {
                return transitions[UnityEngine.Random.Range(0, transitions.Count)];
            }
            return null;
        }

        public NodeNav GetCurrentGroundNode(Vector2 worldPos)
        {
            Vector2Int nodePos = new Vector2Int(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y) - 1);
            if (mGroundNodes.ContainsKey(nodePos))
                return mGroundNodes[nodePos];
            return null;
        }
    }
}