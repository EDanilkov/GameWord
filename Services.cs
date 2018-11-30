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
                if (String.Equals(g.player1.Name, gm.player1.Name) && String.Equals(g.player2.Name, gm.player2.Name))
                {
                    gm.player1.Wins += g.player1.Wins;
                    gm.player2.Wins += g.player2.Wins;
                    isIt = true;
                    break;
                }
            }
            if (!isIt) CollectionGameWords.gameWordsCollection.Add(g);

            String json = JsonConvert.SerializeObject(CollectionGameWords.gameWordsCollection);
            File.WriteAllText("GameWords.json", json, Encoding.GetEncoding(1251));
        }


        public void Load()
        {
            string s = File.ReadAllText("GameWords.json");
            CollectionGameWords.gameWordsCollection = JsonConvert.DeserializeObject<List<GameWords>>(s) == null ? new List<GameWords>() : JsonConvert.DeserializeObject<List<GameWords>>(s);
        }
    }
}
