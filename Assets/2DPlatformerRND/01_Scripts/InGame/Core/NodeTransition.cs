using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEditor.Experimental.GraphView;


namespace PahlBit
{
    public class NodeTransition
    {
        public NodeTransitionType TransitionType { get; set; }
        public NodeNav StartNode { get; set; }
        public float JumpHeight { get; set; }
        public float MoveSpeed { get; set; }
        public int Dir { get; set; }

        // public NodeTransition()
        // {
        // }

    }

    public enum NodeTransitionType
    {
        None,
        Walk,
        Jump,
        DropDown,
        Dash,
    }
}