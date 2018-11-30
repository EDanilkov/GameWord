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

namespace Words
{
    class GameWords
    {
        public Player player1 = new Player();
        public Player player2 = new Player();
        private Services Services = new Services();
        private string firstWord;
        private Thread readThread;
        private System.Timers.Timer readTimer;
        private List<string> words = new List<string>();
        private static bool isRead = false;
        private static bool isTimeLeft;
        private static bool isFirstUser = true;

        public GameWords(GameWords g)
        {
            player1 = g.player1;
            player2 = g.player2;
        }

        public GameWords()
        {

        }

        public void Start()
        {
            Console.WriteLine("Имя первого игрока:");
            player1.Scan();
            Console.WriteLine("Имя второго игрока:");
            player2.Scan();
            string Choose;
            do
            {
                firstWord = null;
                Round();
                Console.WriteLine("Вы хотите сыграть ещё раз ? (Да/Нет)");

                while (!(String.Equals((Choose = Console.ReadLine()), "Нет") || String.Equals(Choose, "Да")))
                {
                    if(ConsoleCommand(Choose) == null)
                    {
                        Console.WriteLine("Вы хотите сыграть ещё раз ? (Да/Нет)");
                    }
                    else
                    {
                        Console.WriteLine("Введите Да/Нет.");
                    }                    
                }
            } while (String.Equals(Choose, "Да"));
            
            Services.Save(this);
        }

        public void ShowWords()
        {
            Console.WriteLine($"{player1.Name}\t\t|\t\t{player2.Name}");
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
            Services.Load();
            Console.WriteLine($"{player1.Name}\t\t|\t\t{player2.Name}");
            int win_1 = player1.Wins, win_2 = player1.Wins;
            foreach (GameWords gm in CollectionGameWords.gameWordsCollection)
            {
                if (gm.player1.Name == player1.Name && gm.player2.Name == player2.Name)
                {
                    win_1 += gm.player1.Wins;
                    win_2 += gm.player2.Wins;
                }
                if (gm.player1.Name == player2.Name && gm.player2.Name == player1.Name)
                {
                    win_1 += gm.player2.Wins;
                    win_2 += gm.player1.Wins;
                }
            }
            Console.WriteLine("{0}\t\t|\t\t{1}", win_1, win_2);

        }

        private void TotalScore()
        {
            Services.Load();
            foreach (GameWords gm in CollectionGameWords.gameWordsCollection)
            {
                Console.WriteLine($"{gm.player1.Name}: {gm.player1.Wins}\t|\t{gm.player2.Name}: {gm.player2.Wins}");
            }
        }

        public string ConsoleCommand(string input)
        {
            if (input.StartsWith("/"))
            {
                Console.Clear();
                readTimer.Stop();
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
                        Console.WriteLine("Неизвестная команда");
                        break;
                }
                Console.WriteLine("Нажмите Enter, чтобы продолжить игру");
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
            readThread.Abort();
            Console.Clear();
            Console.WriteLine("Время вышло.");
            Console.WriteLine();
            isTimeLeft = true;
        }

        private void ConsoleRead()
        {
            Console.Clear();
            Console.WriteLine("------------------------------------------------------------------------------------");
            Console.WriteLine("{0,50}", firstWord);
            Console.WriteLine("------------------------------------------------------------------------------------");
            Console.WriteLine();

            Console.WriteLine(isFirstUser ? $"{ player1.Name }, введите слово:" : $"{ player2.Name }, введите слово:");
            string text = "";
            while (CheckWord(text = Console.ReadLine()) == false)
            {
                if (ConsoleCommand(text) == null)
                {
                    ConsoleRead();
                    readTimer.Start();
                    return;
                }
                Console.WriteLine("Вы ввели неверное слово");
            }
            words.Add(text);
            isRead = true;
        }

        private int EnterWord()
        {
            isTimeLeft = false;
            isRead = false;

            Console.Clear();
            readTimer = new System.Timers.Timer();
            readTimer.Elapsed += new ElapsedEventHandler(TimeLeft);
            readTimer.Interval = 3300;

            readThread = new Thread(ConsoleRead);
            readThread.Priority = ThreadPriority.Highest;

            readTimer.Start();
            readThread.Start();

            while (isTimeLeft == false && isRead == false) ;
            readTimer.Stop();
            if (isRead == false)
            {
                return 0;
            }
            isFirstUser = isFirstUser ? false : true;
            readThread.Abort();
            isRead = false;
            return 1;
        }

        public bool CheckWord(string input)
        {
            int len = input.Length;
            if (len == 0) return false;
            string copyFirstWord = string.Copy(firstWord);
            for (int i = 0; i < len; i++)
            {
                if (copyFirstWord.IndexOf(input[i]) != -1)
                {
                    copyFirstWord.Remove(i, 1);
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
                Console.WriteLine("Слово не соответствует");
                return false;
            }
        }

        private void EnterFirstWord()
        {
            Console.WriteLine("Введите первое слово");
            while (!CheckFirstWord(firstWord = Console.ReadLine()))
            {
                Console.WriteLine("Пожалуйста, введите слово ещё раз");
            }
        }

        private void Round()
        {
            Console.Clear();
            EnterFirstWord();

            while (EnterWord() == 1) ;

            if (isFirstUser)
                player2.Wins++;
            else
                player1.Wins++;
            Console.WriteLine(isFirstUser ? $"{ player2.Name }, победил !" : $"{ player1.Name }, победил");
            Console.WriteLine();
        }

        public void StopProcess()
        {
            if (isRead == false && firstWord != null)
            {
                if (isFirstUser)
                    player2.Wins++;
                else
                    player1.Wins++;
                Services.Save(this);
            }
                
        }

        
    }
}
