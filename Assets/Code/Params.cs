using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BGE
{
    public class Params
    {
        private static Dictionary<string, object> dictionary = new Dictionary<string, object>();

        public static bool showMessages = true;
        public static bool drawVectors = false;
        public static bool drawDebugLines = false;
        public static bool riftEnabled = false;
        public static float timeModifier;

        static Params()
        {
            Debug.Log("Loading default.txt");
            Load("default.txt");
        }

        private static void PrintException(string key, Exception e)
        {
            Console.WriteLine("Could not find property: " + key);
            Console.WriteLine("Did you remember to call Params.Load?");
            Console.WriteLine(e.StackTrace);
        }

        public static float GetFloat(string key)
        {
            try
            {
                return float.Parse("" + dictionary[key]);
            }
            catch (Exception e)
            {
                PrintException(key, e);                
            }
            return -1;
        }

        public static void Put(string key, object value)
        {
            dictionary[key] = value;
        }

        public static float GetWeight(string key)
        {
            try
            {
                return float.Parse("" + dictionary[key]) * GetFloat("steering_weight_tweaker");
            }
            catch (Exception e)
            {
                PrintException(key, e);
            }
            return -1;
        }

        public static object Get(string key)
        {
            try
            {
                return dictionary[key];
            }
            catch (Exception e)
            {
                PrintException(key, e);
            }
            return null;           
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
