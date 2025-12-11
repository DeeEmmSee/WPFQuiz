using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFApp.Utilities
{
    public static class GeneralUtilities
    {
        public static void Shuffle<T>(ref List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                int k = Random.Shared.Next(0, n--);
                //int k = rng.Next(n--);
                T temp = list[n];
                list[n] = list[k];
                list[k] = temp;
            }
        }

    }
}
