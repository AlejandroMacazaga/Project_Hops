using System;
using Entities;
using Player.Events;
using UnityEngine;
using Utils.EventBus;

namespace Player.Classes.Reaper
{
    public class BloodResourceComponent: MonoBehaviour, IVisitable
    {
        [SerializeField] public int currentBloodPoints;

        [SerializeField] public int maxBloodPoints;
        
        public void SetValues(BloodResourceData data)
        {
            currentBloodPoints = data.currentBloodPoints;
            maxBloodPoints = data.maxBloodPoints;
        }
        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public int Use()
        {
            var toReturn = currentBloodPoints;
            currentBloodPoints = 0;
            RaiseEvent();
            return toReturn;
        }

        public void Add(int blood)
        {
            currentBloodPoints += blood;
            RaiseEvent();
        }
        
        private void RaiseEvent()
        {
            EventBus<ReaperBloodChange>.Raise(new ReaperBloodChange()
            {
                Current = currentBloodPoints
            });
        }
    }
    
    [Serializable]
    public struct BloodResourceData
    {
        public int currentBloodPoints;
        public int maxBloodPoints;
    }
}