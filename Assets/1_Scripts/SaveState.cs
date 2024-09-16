using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CardMatch
{
    public class SaveState : MonoBehaviour
    {
        public static void Save<T>(T objectToSave, string key)
        {
            string path = Application.persistentDataPath + "/saves/";
            Directory.CreateDirectory(path);
            string datString = JsonUtility.ToJson(objectToSave);
            File.WriteAllText(path + key + ".cardMatch", datString);
        }

        public static T Load<T>(string key)
        {
            T returnValue = default(T);
            string path = Application.persistentDataPath + "/saves/";
            string fileData = File.ReadAllText(path + key + ".cardMatch");

            returnValue = JsonUtility.FromJson<T>(fileData);

            return returnValue;
        }

        public static bool DoesSaveExist(string key)
        {
            string path = Application.persistentDataPath + "/saves/" + key + ".cardMatch";
            return File.Exists(path);
        }

        public static void DeleteSaveFile(string key)
        {
            string path = Application.persistentDataPath + "/saves/" + key + ".cardMatch";

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static void DeleteAllSaveFiles()
        {
            string path = Application.persistentDataPath + "/saves/";

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            Directory.CreateDirectory(path);
        }


        public void DoSaveState()
        {

        }

        public void LoadState()
        {

        }

        void Start()
        {
        }

        void Update()
        {
        }
    }
}
