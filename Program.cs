using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Words
{

    class Program
    {
        private static bool isclosing = false;
        static GameWords game; 
        static void Main(string[] args)
        {
            SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);

            game = new GameWords();
            game.OnTime += game.WinByTime;
            game.ChangeLanguage();
            game.CreatePlayers();
            game.Menu();

            Console.WriteLine("CTRL+C,CTRL+BREAK or suppress the application to exit");

            while (!isclosing);
        }

        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
             switch (ctrlType)
             {
                 case CtrlTypes.CTRL_C_EVENT:
                     Console.WriteLine("CTRL+C received!");
                     isclosing = true;
                     break;

                 case CtrlTypes.CTRL_BREAK_EVENT:
                     Console.WriteLine("CTRL+BREAK received!");
                     isclosing = true;
                     break;

                 case CtrlTypes.CTRL_CLOSE_EVENT:
                     Console.WriteLine("Program being closed!");
                     game.StopProcess();
                     isclosing = true;
                     break;

                 case CtrlTypes.CTRL_LOGOFF_EVENT:
                 case CtrlTypes.CTRL_SHUTDOWN_EVENT:
                     Console.WriteLine("User is logging off!");
                     game.StopProcess();
                     isclosing = true;
                     break;

             }
             
            return true;

        }
        
        #region unmanaged
        
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT

        }
        #endregion
        
    }
}
