using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace CalendarHabitsApp.Helpers
{
    public class Common
    {
        public static bool SaveJson<T>(T theobject, string filePath)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        //sw.WriteLine("#" + Common.GetVersion());

                        using (MemoryStream ms = new MemoryStream())
                        {
                            DataContractJsonSerializer serializer = new DataContractJsonSerializer
                            (typeof(T));
                            serializer.WriteObject(ms, theobject);
                            Encoding enc = Encoding.UTF8;
                            sw.Write(enc.GetString(ms.ToArray()));
                        }
                    }

                    fs.Flush();
                    fs.Close();
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static T LoadJson<T>(string filePath)
        {
            T result;
            if (!System.IO.File.Exists(filePath))
            {
                return default(T);
            }

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    string filetext = sr.ReadToEnd();

                    if (filetext.StartsWith("#"))
                    {
                        int firstlineindex = filetext.IndexOf(System.Environment.NewLine);
                        filetext = filetext.Substring(firstlineindex + System.Environment.NewLine.Length);
                    }

                    using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(filetext)))
                    {
                        DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings();
                        var deserializer = new DataContractJsonSerializer(typeof(T), settings);
                        result = (T)deserializer.ReadObject(ms);
                    }
                }

                fs.Close();
            }

            return result;
        }

        public static String GetMonthNameFromNumber(int monthNumber)
        {
            switch (monthNumber)
            {
                case (1):
                    return "January";
                case (2):
                    return "February";
                case (3):
                    return "March";
                case (4):
                    return "April";
                case (5):
                    return "May";
                case (6):
                    return "June";
                case (7):
                    return "July";
                case (8):
                    return "August";
                case (9):
                    return "September";
                case (10):
                    return "October";
                case (11):
                    return "November";
                case (12):
                    return "December";
                default:
                    return "";
            }
        }
    }
}
