using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerProfile 
{
   public string name;
   public int coins;

   public PlayerProfile()
   {
      coins = 0;
   }
}
