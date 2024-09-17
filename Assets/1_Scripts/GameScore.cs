using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using static CardMatch.GameScore;
using System.IO;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;

namespace CardMatch
{

    [Serializable]
    public class NameTimeDictionary
    {
        public Dictionary<string, Dictionary<string, NameTime>> nameTimeByBoardType = new Dictionary<string, Dictionary<string, NameTime>>();
    }

    public class BinarySerializationUtils
    {
        public static void SerializeToBinary(NameTimeDictionary data, string filePath)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                formatter.Serialize(stream, data);
            }
        }

        public static NameTimeDictionary DeserializeFromBinary(string filePath)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                return (NameTimeDictionary)formatter.Deserialize(stream);
            }
        }
    }

    public class GameScore : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text title;

        [SerializeField]
        private TMP_Text gameType;

        [SerializeField]
        private TMP_Text noScores;

        [SerializeField]
        public List<TMP_Text> positions;

        [SerializeField]
        public List<TMP_Text> names;

        [SerializeField]
        public List<TMP_Text> times;

        [Serializable]
        public class NameTime
        {
            public string name;

            public string time;

            internal static bool isMinor(string time1, string time2)
            {
                if (time1 == null)
                {
                    return false;
                }

                time1 = time1.Replace(":", "");
                time2 = time2.Replace(":", "");
                int t1 = Int32.Parse(time1);
                int t2 = Int32.Parse(time2);
                return t1 < t2;
            }
        }

        private NameTimeDictionary nameTimeDictionary;

        public void LoadFromFile()
        {
            string filePath = Path.Combine(Application.persistentDataPath, "nameTimeData.dat");

            if (File.Exists(filePath))
            {
                nameTimeDictionary = BinarySerializationUtils.DeserializeFromBinary(filePath);
            }
        }

        public void SaveToFile()
        {
            string filePath = Path.Combine(Application.persistentDataPath, "nameTimeData.dat");
            BinarySerializationUtils.SerializeToBinary(nameTimeDictionary, filePath);
        }

        public void UpdateScore(string gameType, string name, string time)
        {
            title.text = "Top Scores(Game " + gameType + ")";

            time = time.Replace("Time:", "").Trim();

            Dictionary<string, NameTime> nameTypeMap = null;

            if (nameTimeDictionary == null)
            {
                nameTimeDictionary = new NameTimeDictionary();
            }

            if (!nameTimeDictionary.nameTimeByBoardType.ContainsKey(gameType))
            {
                nameTypeMap = new Dictionary<string, NameTime>();
                nameTimeDictionary.nameTimeByBoardType.Add(gameType, nameTypeMap);
            }
            else
            {
                nameTypeMap = nameTimeDictionary.nameTimeByBoardType[gameType];
            }

            NameTime nameType = null;
            if (nameTypeMap.ContainsKey(name))
            {
                nameType = nameTypeMap[name];
            }
            else
            {
                nameType = new NameTime();
                nameTypeMap.Add(name, nameType);
            }

            nameType.time = NameTime.isMinor(nameType.time, time) ? nameType.time : time;
            nameType.name = name;

            //
            // save to file
            //
            SaveToFile();

            UpdateUI();
        }

        public void UpdateUI()
        {
            if (!nameTimeDictionary.nameTimeByBoardType.ContainsKey(gameType.text))
            {
                ClearNameTimesPos();
                noScores.enabled = true;
            }
            else
            {
                noScores.enabled = false;

                Dictionary<string, NameTime> dic = nameTimeDictionary.nameTimeByBoardType[gameType.text];
                List<NameTime> list = dic.Values.ToList();
                list.Sort((x, y) => NameTime.isMinor(x.time, y.time) ? -1 : 1);

                for (int i = 0; i < 6; i++)
                {
                    positions[i].enabled = false;
                    names[i].enabled = false;
                    times[i].enabled = false;
                }

                for (int i = 0; i < 5 && i < list.Count; i++)
                {
                    if (i == 0)
                    {
                        positions[i].enabled = true;
                        names[i].enabled = true;
                        times[i].enabled = true;
                    }

                    NameTime nt = list[i];

                    positions[i + 1].text = (i + 1).ToString();
                    names[i + 1].text = nt.name;
                    times[i + 1].text = nt.time;
                    positions[i + 1].enabled = true;
                    names[i + 1].enabled = true;
                    times[i + 1].enabled = true;
                }
            }
        }

        void ClearNameTimesPos()
        {
            foreach (TMP_Text t in positions)
            {
                t.text = "";
            }

            foreach (TMP_Text t in names)
            {
                t.text = "";
            }

            foreach (TMP_Text t in times)
            {
                t.text = "";
            }
        }

        public void initialize()
        {
            if (nameTimeDictionary == null)
            {
                nameTimeDictionary = new NameTimeDictionary();
            }

            LoadFromFile();
            UpdateUI();
        }
    }
}