using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Globalization;

namespace Words
{
    class GameWords
    {

        public Player Player1 = new Player();
        public Player Player2 = new Player();
        private Services services = new Services();
        private string firstWord;
        private System.Timers.Timer readTimer = new System.Timers.Timer();
        private System.Timers.Timer showTimer = new System.Timers.Timer();
        private DateTime timer;
        private string[] items;
        private ConsoleMenu menu;
        private int timeGame = 10000;


        private int x, y, x1, y1;
        private List<string> words = new List<string>();
        private static bool isTimeLeft;
        private static bool isFirstUser = true;

        public delegate void MethodContainer();
        public event MethodContainer OnTime;

        delegate void method();

        public GameWords(GameWords g)
        {
            Player1 = g.Player1;
            Player2 = g.Player2;
        }

        public GameWords()
        {

        }
        

        public void WinByTime()
        {
            showTimer.Stop();
            Console.Clear();
            readTimer.Stop();
            isTimeLeft = true;
            Console.WriteLine(Localization.GetLocalizedString("TimeOut"));
            Console.WriteLine(Localization.GetLocalizedString("PressEnter"));
        }

        public void Menu()
        {
            showTimer.Elapsed += new ElapsedEventHandler(ShowTimer);
            readTimer.Elapsed += new ElapsedEventHandler(TimeLeft);
            method[] methods = new method[] { Start, CreatePlayers, SetTime, ChangeLanguage, Exit };
            items = new string[] { Localization.GetLocalizedString("Start"), Localization.GetLocalizedString("ChangePlayers"), Localization.GetLocalizedString("SetTime"), Localization.GetLocalizedString("ChangeLanguage"), Localization.GetLocalizedString("Exit") };
            menu = new ConsoleMenu(items);
            int menuResult;
            do
            {
                menuResult = menu.PrintMenu();
                methods[menuResult]();
                Console.WriteLine(Localization.GetLocalizedString("PressEnter"));
                Console.ReadKey();
            } while (menuResult != items.Length - 1);
        }

        public void SetTime()
        {
            Console.Clear();
            do
            {
                Console.WriteLine("Введите время игры: ");
            }
            while (!int.TryParse(Console.ReadLine(), out timeGame));
            timeGame *= 1000;
        }
        
        public void ChangeLanguage()
        {
            Console.Clear();
            string[] items = { "Русский", "English"};
            ConsoleMenuLanguage languageMenu = new ConsoleMenuLanguage(items);
            int menuResult = languageMenu.PrintMenu();
            switch (menuResult)
            {
                case 0:
                    Localization.SetLanguage("ru-RU");
                    break;
                case 1:
                    Localization.SetLanguage("en-GB");
                    break;
            }
            items = new string[] { Localization.GetLocalizedString("Start"), Localization.GetLocalizedString("ChangePlayers"), Localization.GetLocalizedString("ChangeLanguage"), Localization.GetLocalizedString("Exit") };
            menu = new ConsoleMenu(items);
        }

        public void Exit()
        {
            services.Save(this);
            Environment.Exit(0);
        }

        public void CreatePlayers()
        {
            if(!(firstWord == null))
            {
                services.Save(this);
            }
            Console.Clear();
            Console.WriteLine(Localization.GetLocalizedString("FirstPlayerName") + ":");
            Player1.Scan();
            Console.WriteLine(Localization.GetLocalizedString("SecondPlayerName") + ":");
            Player2.Scan();
        }

        public void Start()
        {
            words.Clear();
            firstWord = null;
            Round();
        }

        public void ShowWords()
        {
            Console.WriteLine($"{Player1.Name}\t\t|\t\t{Player2.Name}");
            int n = words.Count;
            for (int i = 0; i < n; i += 2)
            {
                try
                {
                    Console.WriteLine("{0}\t\t|\t\t{1}", words[i], words[i + 1]);
                }
                catch
                {
                    Console.WriteLine("{0}\t\t|\t\t{1}", words[i], "");
                }
            }
        }

        

        private void Score()
        {
            services.Load();
            Console.WriteLine($"{Player1.Name}\t\t|\t\t{Player2.Name}");
            int win_1 = Player1.Wins, win_2 = Player2.Wins;
            foreach (GameWords gm in CollectionGameWords.gameWordsCollection)
            {
                if (gm.Player1.Name == Player1.Name && gm.Player2.Name == Player2.Name)
                {
                    win_1 += gm.Player1.Wins;
                    win_2 += gm.Player2.Wins;
                }
                if (gm.Player1.Name == Player2.Name && gm.Player2.Name == Player1.Name)
                {
                    win_1 += gm.Player2.Wins;
                    win_2 += gm.Player1.Wins;
                }
            }
            Console.WriteLine("{0}\t\t|\t\t{1}", win_1, win_2);

        }

        private void TotalScore()
        {
            services.Load();
            int win_1, win_2;
            foreach (GameWords gm in CollectionGameWords.gameWordsCollection)
            {
                win_1 = gm.Player1.Wins;
                win_2 = gm.Player2.Wins;
                if (gm.Player1.Name == Player1.Name && gm.Player2.Name == Player2.Name)
                {
                    win_1 = Player1.Wins;
                    win_2 = Player2.Wins;
                    win_1 += gm.Player1.Wins;
                    win_2 += gm.Player2.Wins;
                }
                if (gm.Player1.Name == Player2.Name && gm.Player2.Name == Player1.Name)
                {
                    win_1 = Player1.Wins;
                    win_2 = Player2.Wins;
                    win_1 += gm.Player2.Wins;
                    win_2 += gm.Player1.Wins;
                }
                Console.WriteLine($"{gm.Player1.Name}: {win_1}\t|\t{gm.Player2.Name}: {win_2}");
            }
        }

        public string ConsoleCommand(string input)
        {
            if (input.StartsWith("/"))
            {
                Console.Clear();
                readTimer.Stop();
                showTimer.Stop();
                switch (input)
                {
                    case "/show-words":
                        ShowWords();
                        break;
                    case "/score":
                        Score();
                        break;
                    case "/total-score":
                        TotalScore();
                        break;
                    default:
                        Console.WriteLine(Localization.GetLocalizedString("UnknownCommand"));
                        break;
                }
                Console.WriteLine(Localization.GetLocalizedString("PressEnter"));
                Console.ReadKey();
                Console.Clear();
                return null;
            }
            return input;
        }
        
        static bool IsNumberContains(string input)
        {
            return input.All(c => char.IsLetter(c));
        }

        private void TimeLeft(object obj, EventArgs e)
        {
            OnTime();
        }

        private void ShowTimer(object obj, EventArgs e)
        {
            x1 = Console.CursorLeft;
            y1 = Console.CursorTop;
            Console.SetCursorPosition(x, y);
            Console.Write(timer.ToString("mm:ss"));
            timer = timer.AddSeconds(-1);
            Console.SetCursorPosition(x1, y1);
        }

        private bool EnterWord()
        {
            isTimeLeft = false;


            showTimer.Interval = 1000;
            readTimer.Interval = timeGame;
            timer = new DateTime(1970, 1, 1).Add(TimeSpan.FromTicks(timeGame * TimeSpan.TicksPerSecond / 1000)).ToLocalTime();
            showTimer.Start();
            readTimer.Start();

            Console.Clear();
            Console.WriteLine("------------------------------------------------------------------------------------");
            x = Console.CursorLeft;
            y = Console.CursorTop;
            Console.Write(timer.ToString("mm:ss"));
            Console.WriteLine("{0,50}", firstWord);
            Console.WriteLine("------------------------------------------------------------------------------------");
            Console.WriteLine();

            timer = timer.AddSeconds(-1);

            Console.WriteLine(isFirstUser ? $"{ Player1.Name }, " + Localization.GetLocalizedString("EnteredWord") + ":" : $"{ Player2.Name }, " + Localization.GetLocalizedString("EnteredWord") + ":");
            string text = "";
            while (CheckWord(text = Console.ReadLine()) == false)
            {
                if (isTimeLeft)
                {
                    return false;
                }
                if (ConsoleCommand(text) == null)
                {
                    if(EnterWord())
                    {
                        return true;
                    }
                    return false;
                }
                Console.WriteLine(Localization.GetLocalizedString("WrongWord"));
            }
            if (isTimeLeft)
            {
                return false;
            }
            readTimer.Stop();
            showTimer.Stop();
            words.Add(text);
            
            isFirstUser = isFirstUser ? false : true;
            return true;
        }

        public bool CheckWord(string input)
        {
            int len = input.Length;
            if (len == 0) return false;
            string copyFirstWord = string.Copy(firstWord);
            int clen = len;
            int ch;
            for (int i = 0; i < len; i++)
            {
                if ((ch = copyFirstWord.IndexOf(input[i])) != -1 && !words.Any(c => c.Equals(input)))
                {
                    copyFirstWord = copyFirstWord.Remove(ch, 1);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckFirstWord(string input)
        {
            if (input.Length > 7 && input.Length < 31 && IsNumberContains(input))
            {
                firstWord = input;
                return true;
            }
            else
            {
                Console.WriteLine(Localization.GetLocalizedString("WrongWord"));
                return false;
            }
        }

        private void EnterFirstWord()
        {
            Console.WriteLine(Localization.GetLocalizedString("FirstWord"));
            while (!CheckFirstWord(firstWord = Console.ReadLine()))
            {
                Console.WriteLine(Localization.GetLocalizedString("EnterAgain"));
            }
        }



        private void Round()
        {
            Console.Clear();
            EnterFirstWord();

            while (EnterWord());

            if (isFirstUser)
                Player2.Wins++;
            else
                Player1.Wins++;
            Console.WriteLine(isFirstUser ? $"{ Player2.Name }," + Localization.GetLocalizedString("Win") + " !" : $"{ Player1.Name }," + Localization.GetLocalizedString("Win") + " !");
            Console.WriteLine();
        }

        public void StopProcess()
        {
            if (isTimeLeft == false && firstWord != null)
            {
                if (isFirstUser)
                    Player2.Wins++;
                else
                    Player1.Wins++;
                services.Save(this);
            }
                
        }
    }
}
