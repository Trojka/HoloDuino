using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadItLib
{
    public static class ReadIt
    {
        //public static string GetValue()
        //{
        //    return "The value";
        //}

        private static bool _stopProcessing;
        private static int _maxRead = 20;

        public static async Task<int> StartProcessing(Action<string> action)
        {
            _stopProcessing = false;

            int i = 0;

            Random rnd = new Random();

            while (!_stopProcessing && i < _maxRead)
            {
                int temp = rnd.Next(100);
                action(temp.ToString());
                i++;

                await Task.Delay(1000);
            }

            return 0;
        }

        public static void StopProcessing()
        {
            _stopProcessing = true;
        }
    }
}
