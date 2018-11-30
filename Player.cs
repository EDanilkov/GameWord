using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Words
{
    class Player
    {
        public string Name { get; set; }
        public int Wins { get; set; }

        public Player()
        {
            Name = "";
            Wins = 0;
        }

        public Player(string _name, int _wins)
        {
            Name = _name;
            Wins = _wins;
        }

        public void Scan()
        {
            Name = Console.ReadLine();
            Wins = 0;
        }
    
    }
}
