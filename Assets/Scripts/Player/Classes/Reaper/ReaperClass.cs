using System;
using KBCore.Refs;
using Player.States;
using UnityEngine;

namespace Player.Classes.Reaper
{
    [RequireComponent(typeof(CharacterMover))]
    public class ReaperClass : CharacterClass
    {
        public DashingState DashingState;
        
        public override void Awake()
        {
            base.Awake();
            
        }

        public void Start()
        {
            inputReader.EnablePlayerActions();
        }
    }
}