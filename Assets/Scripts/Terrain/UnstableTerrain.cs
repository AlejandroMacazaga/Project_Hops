using System;
using Entities;
using Player;
using UnityEngine;

namespace Terrain
{
    public class UnstableTerrain : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            var controller = other.GetComponent<PlayerController>();
            if (!controller) return;
            controller.isOnUnstableGround = true;
        }

        private void OnTriggerExit(Collider other)
        {
            var controller = other.GetComponent<PlayerController>();
            if (!controller) return;
            controller.isOnUnstableGround = false;
        }
    }
}
