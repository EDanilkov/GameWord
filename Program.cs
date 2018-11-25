using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Words
{
    class Program
    {
        static GameWords game;

        static void Main(string[] args)
        {
            game = new GameWords();
            
        }
        ~Program()
        {
            game.Dispose();
        }

    }
}
