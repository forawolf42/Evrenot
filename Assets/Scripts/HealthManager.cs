using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

[ScriptHelp(BackColor = EditorHeaderBackColor.Steel)]
public class HealthManager :Fusion.NetworkBehaviour
{
   [Networked] public int Health { get; set; }

   public void ShotShip()
   {
      if (Object.HasInputAuthority)
      {
         Rpc_ShootShip();
      }
   }

   [Rpc]
   public void Rpc_ShootShip()
   {
      Health--;
   }
}
