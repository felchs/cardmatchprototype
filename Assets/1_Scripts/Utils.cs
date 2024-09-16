using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardMatch
{
    public class Utils
    {
        public static void Shuffle(ArrayList arrayList)
        {
            System.Random rng = new System.Random();
            int n = arrayList.Count;
            for (int i = n - 1; i > 0; i--)
            {
                // Randomly pick an index between 0 and i
                int j = rng.Next(0, i + 1);

                // Swap elements arrayList[i] and arrayList[j]
                object temp = arrayList[i];
                arrayList[i] = arrayList[j];
                arrayList[j] = temp;
            }
        }

        public static void Shuffle(Array array)
        {
            System.Random rng = new System.Random();
            int n = array.Length;
            for (int i = n - 1; i > 0; i--)
            {
                // Randomly pick an index between 0 and i
                int j = rng.Next(0, i + 1);

                // Swap elements array[i] and array[j]
                object temp = array.GetValue(i);
                array.SetValue(array.GetValue(j), i);
                array.SetValue(temp, j);
            }
        }

    }
}
