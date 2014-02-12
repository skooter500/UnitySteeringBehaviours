using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BGE
{
    class Params
    {
        private static Dictionary<string, object> dictionary = new Dictionary<string, object>();

        public static bool showMessages = true;
        public static bool drawVectors = false;
        public static bool drawDebugLines = false;
        public static float timeModifier;

        public static float GetFloat(string key)
        {
            return float.Parse("" + dictionary[key]);
        }

        public static void Put(string key, object value)
        {
            dictionary[key] = value;
        }

        public static float GetWeight(string key)
        {
            return float.Parse("" + dictionary[key]) * GetFloat("steering_weight_tweaker");
        }

        public static object Get(string key)
        {
            return dictionary[key];
        }

        public static void Load(string filename)
        {
            dictionary.Clear();
            StreamReader sw = new StreamReader(Application.dataPath + System.IO.Path.DirectorySeparatorChar + filename);
            char[] delims = { '=' };
            while (!sw.EndOfStream)
            {
                string line = sw.ReadLine();
                if (line.Length != 0)
                {
                    string[] elements = line.Split(delims);
                    string key = elements[0].Trim();
                    string value = elements[1].Trim();
                    dictionary[key] = value;
                }
            }

            timeModifier = 0.0f;
        }
    }
}
