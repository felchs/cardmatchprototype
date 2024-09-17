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
    private TMP_Text noScores;

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

        nameType.time = NameTime.isMinor(nameType.time, time) ? nameType.time : time;
        nameType.name = name;

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (!nameTimeByBoardType.ContainsKey(gameType.text))
        {
            ClearNameTimesPos();
            noScores.enabled = true;
        }
        else
        {
            noScores.enabled = false;

            Dictionary<string, NameTime> dic = nameTimeByBoardType[gameType.text];
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

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        
    }
}
