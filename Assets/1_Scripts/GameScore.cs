using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class GameScore : MonoBehaviour
{
    [SerializeField]
    private TMP_Text gameType;

    [SerializeField]
    public List<TMP_Text> positions;

    [SerializeField]
    public List<TMP_Text> names;

    [SerializeField]
    public List<TMP_Text> times;

    public class NameTime
    {
        public string name;

        public string time;

        internal static bool isGreater(string time1, string time2)
        {
            if (time1 == null)
            {
                return false;
            }

            time1 = time1.Replace(":", "");
            time2 = time2.Replace(":", "");
            int t1 = Int32.Parse(time1);
            int t2 = Int32.Parse(time2);
            return t1 > t2;
        }
    }

    private Dictionary<string, Dictionary<string, NameTime>> nameTimeByBoardType = new Dictionary<string, Dictionary<string, NameTime>>();

    public void UpdateScore(string gameType, string name, string time)
    {
        time = time.Replace("Time:", "").Trim();

        Dictionary<string, NameTime> nameTypeMap = null;

        if (!nameTimeByBoardType.ContainsKey(gameType))
        {
            nameTypeMap = new Dictionary<string, NameTime>();
            nameTimeByBoardType.Add(gameType, nameTypeMap);
        }
        else
        {
            nameTypeMap = nameTimeByBoardType[gameType];
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

        nameType.time = NameTime.isGreater(nameType.time, time) ? nameType.time : time;

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (!nameTimeByBoardType.ContainsKey(gameType.text))
        {
            ClearNameTimesPos();
            positions[0].text = "No scores were recorded yet";
        }
        else
        {
            Dictionary<string, NameTime> dic = nameTimeByBoardType[gameType.text];
            List<NameTime> list = dic.Values.ToList();
            list.Sort((x, y) => x.time.CompareTo(y.time));

            for (int i = 0; i < 5; i++)
            {
                NameTime nt = list[i];

                positions[i].text = (i + 1).ToString();
                names[i].text = nt.name;
                times[i].text = nt.time;
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

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        
    }
}
