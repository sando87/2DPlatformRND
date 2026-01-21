using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEditor.Experimental.GraphView;


namespace PahlBit
{
    public class NodeNav
    {
        public Vector2 MinPos { get => new Vector2(Position.x, Position.y); }
        public Vector2 MaxPos { get => new Vector2(Position.x + 1, Position.y + 1); }
        public Vector2 CenterPos { get => new Vector2(Position.x + 0.5f, Position.y + 0.5f); }

        public Vector2Int Position { get; private set; }
        public NodeNavGroup ParentGroup { get; set; }

        public NodeNav(Vector2Int position)
        {
            Position = position;
        }

    }
}