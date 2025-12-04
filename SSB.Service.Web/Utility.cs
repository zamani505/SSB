using System;
using System.IO;

namespace SSB.Service.Web
{
    public static class Utility
    {
        public static void CreateLog(string log)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\WsError.txt";
            using (StreamWriter outfile = new StreamWriter(path, true))
            {
                outfile.Write(log + Environment.NewLine);
            }

            //if (!File.Exists(path))
            //    OurStream = File.CreateText(path);
            //else
            //    OurStream = File.AppendText(path);
            //OurStream.WriteLine(log);
            //OurStream.Close();
        }
        public enum Operator
        {
            Armaghan = 1,
            PayamGostaran = 11
        }
         
    }
}