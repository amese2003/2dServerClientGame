using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class Player : GameObject
    {       
        public ClientSession Session { get; set; }        

        public Player()
        {
            ObjectType = GameObjectType.Player;
            Speed = 10.0f;
        }

        public override void OnDammaged(GameObject attacker, int damage)
        {
            Console.WriteLine($"TODO : Damage {damage}");
        }
    }
}
