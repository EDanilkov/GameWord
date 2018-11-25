using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;
using System.IO;


namespace Words
{
    class GameWords : IDisposable
    {
        static List<GameWords> memberList = new List<GameWords>();
        public string UserName_1 { get; set; }
        public string UserName_2 { get; set; }
        public int WinUser_1 { get; set; }
        public int WinUser_2 { get; set; }
        private string FirstWord { get; set; }
        private Thread mythread;
        private System.Timers.Timer tm;
        private List<string> Words = new List<string>();
        private static bool isRead;
        private static bool isTimeLeft;
        private static bool isFirstUser = true;
        private bool isDisposed = false;



        public void ShowWords()
        {
            Console.WriteLine($"{UserName_1}\t\t|\t\t{UserName_2}");
            int n = Words.Count;
            for (int i = 0; i < n; i += 2)
            {
                try
                {
                    Console.WriteLine("{0}\t\t|\t\t{1}", Words[i], Words[i + 1]);
                }
                catch
                {
                    Console.WriteLine("{0}\t\t|\t\t{1}", Words[i], "");
                }
            }
        }

        public string ConsoleCommand(string input)
        {
            if (input[0] == '/')
            {
                Console.Clear();
                tm.Stop();
                switch (input)
                {
                    case "/show-words":
                        ShowWords();
                        break;
                    case "/score":

                        break;
                    case "/total-score":

                        break;
                    default:
                        Console.WriteLine("Неизвестная команда");
                        break;
                }
                Console.WriteLine("Нажмите Enter, чтобы продолжить игру");
                Console.ReadKey();
                tm.Start();
                return null;
            }
            return input;
        }


        static bool IsNumberContains(string input)
        {
            foreach (char c in input)
                if (Char.IsNumber(c))
                    return true;
            return false;
        }

        private void TimeLeft(object obj, EventArgs e)
        {
            mythread.Abort();
            Console.Clear();
            Console.WriteLine("Время вышло.");
            Console.WriteLine();
            isTimeLeft = true;
        }

        private void ConsoleRead()
        {
            Console.Clear();
            Console.WriteLine("------------------------------------------------------------------------------------");
            Console.WriteLine("{0,50}", FirstWord); 
            Console.WriteLine("------------------------------------------------------------------------------------");
            Console.WriteLine();

            Console.WriteLine(isFirstUser ? $"{ UserName_1 }, введите слово:" : $"{ UserName_2 }, введите слово:");
            string text = "";
            while (CheckWord(text = Console.ReadLine()) == false)
            {
                if (ConsoleCommand(text) == null)
                {
                    ConsoleRead();
                    return;
                }
                Console.WriteLine("Вы ввели неверное слово");
            }
            Words.Add(text);
            isRead = true;
        }

        private int EnterWord()
        {
            isTimeLeft = false;
            isRead = false;

            Console.Clear();
            tm = new System.Timers.Timer();
            tm.Elapsed += new ElapsedEventHandler(TimeLeft);
            tm.Interval = 10000;

            mythread = new Thread(ConsoleRead);
            mythread.Priority = ThreadPriority.Highest;

            tm.Start();
            mythread.Start();

            while (isTimeLeft == false && isRead == false) ;
            tm.Stop();
            if (isRead == false)
            {
                return 0;
            }
            isFirstUser = isFirstUser ? false : true;
            mythread.Abort();
            return 1;
        }

        public bool CheckWord(string input)
        {
            int len = input.Length;
            if (len == 0) return false;
            string copyFirstWord = string.Copy(FirstWord);
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
            if (input.Length > 7 && input.Length < 31 && !IsNumberContains(input))
            {
                FirstWord = input;
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
            while (!CheckFirstWord(FirstWord = Console.ReadLine()))
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
                WinUser_2++;
            else
                WinUser_1++;
            Console.WriteLine(isFirstUser ? $"{ UserName_2 }, победил !" : $"{ UserName_1 }, победил");
            Console.WriteLine();
        }

        /*static void Save()
        {
            String json = JsonConvert.SerializeObject(memberList);
            System.IO.File.WriteAllText("GameWords.json", json);
        }

        static void Load()
        {
            memberList = JsonConvert.DeserializeObject<List<GameWords>>(System.IO.File.ReadAllText("GameWords.json"));
            AddNew();
            Save();
        }*/

        public GameWords()
        {
            Console.WriteLine("Имя первого игрока:");
            UserName_1 = Console.ReadLine();
            Console.WriteLine("Имя второго игрока:");
            UserName_2 = Console.ReadLine();
            string Choose;
            do
            {
                FirstWord = null;
                Round();
                Console.WriteLine("Вы хотите сыграть ещё раз ? (Да/Нет)");

                while (!((Choose = Console.ReadLine()) == "Нет" || Choose == "Да"))
                {
                    Console.WriteLine("Введите Да/Нет.");
                }
            } while (Choose == "Да");


            File.WriteAllText("GameWords.json", JsonConvert.SerializeObject(this));
            Console.ReadKey();

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }
            if (disposing)
            {
                if (isRead == false && isTimeLeft == false && FirstWord != null)
                {
                    if (isFirstUser)
                        WinUser_2++;
                    else
                        WinUser_1++;
                }
                File.WriteAllText("1.txt", "1.txt");
                File.WriteAllText("GameWords.json", JsonConvert.SerializeObject(this));
            }
            isDisposed = true;
        }
        ~GameWords()
        {
            File.WriteAllText("1.txt", "1.txt");
        }
    }
}
