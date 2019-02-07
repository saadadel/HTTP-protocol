using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            string path = @"E:\College\Network\HTTP\HTTP\project\Template[2018-2019]/log.txt";
            //StreamWriter file = new StreamWriter(@"E:\College\Network\HTTP\HTTP\project\Template[2018-2019]/log.txt");
            if (!File.Exists(path))
            {
                using (StreamWriter logFile = new StreamWriter(path))
                {
                    string line = ex.ToString() + "   " + DateTime.Now + "\r\n";
                    logFile.WriteLine(line);
                }

            }
            else
            {
                using (StreamWriter logFile = new StreamWriter(path, true))
                {
                    string line = ex.ToString() + "   " + DateTime.Now + "\r\n";
                    logFile.WriteLine(line);
                }
            }
            // for each exception write its details associated with datetime 
        }
    }
}
