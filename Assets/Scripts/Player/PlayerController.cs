using Assets.Scripts.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerController : EntityController
    {
        new PlayerData Data => (PlayerData)base.Data;
    }
}
