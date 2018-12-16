using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Words
{
    class Services
    {
        public Services()
        {

        }

        public void Save(GameWords g)
        {
            Load();
            bool isIt = false;
            foreach (GameWords gm in CollectionGameWords.gameWordsCollection)
            {
                if (String.Equals(g.Player1.Name, gm.Player1.Name) && String.Equals(g.Player2.Name, gm.Player2.Name))
                {
                    gm.Player1.Wins += g.Player1.Wins;
                    gm.Player2.Wins += g.Player2.Wins;
                    isIt = true;
                    break;
                }
            }
            if (!isIt) CollectionGameWords.gameWordsCollection.Add(g);

            String json = JsonConvert.SerializeObject(CollectionGameWords.gameWordsCollection);
            File.WriteAllText("GameWords.json", json, Encoding.UTF8);
        }


        public void Load()
        {
            try
            {
                string s = File.ReadAllText("GameWords.json");
                CollectionGameWords.gameWordsCollection = JsonConvert.DeserializeObject<List<GameWords>>(s) == null ? new List<GameWords>() : JsonConvert.DeserializeObject<List<GameWords>>(s);
            }
            catch
            {
                Console.Clear();
                Console.WriteLine("Ошибка открытия файла для записи результатов.");
                Console.ReadKey();
                Environment.Exit(1);
            }
        }
    }
}
