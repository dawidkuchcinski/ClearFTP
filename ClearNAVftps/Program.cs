using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace ClearNAVftps
{
    class Program
    {
        static void Main(string[] args)
        {
            string TodaysDate = DateTime.Now.ToString("yyyy_MM_dd");
            string Now = DateTime.Now.ToString("yyyy_MM_dd ");
            string input = File.ReadAllText(@"Path\to\file_with_creds.txt");
            string path = @"Path\to\log_file" + TodaysDate + "-log.txt";

            string[,] creditionals = new string[999,2];
            int i = 0;
            int j = 0;

            foreach(var row in input.Split('\n'))
            {
                j = 0;
                foreach(var col in row.Trim().Split(' '))
                {
                    creditionals[i, j] = col.Trim();
                    j++;
                }
                i++;
            }


            for (i = 0; i < 45; i++)
            {
                if(!String.IsNullOrEmpty(creditionals[i, 0]))
                {
                    File.AppendAllText(path, DateTime.Now + " " + creditionals[i, 0] + " " + creditionals[i, 1] + Environment.NewLine);
                    ConnectToFTP(creditionals[i, 0], creditionals[i, 1]);
                    ConnectToFTP(creditionals[i, 0], creditionals[i, 1]);
                }
            }

            void DeleteFileOnFtpServer(string fileName, string ftpUsername, string ftpPassword)
            {
                // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://address" + fileName);
                request.UsePassive = false;
                request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                request.Method = WebRequestMethods.Ftp.DeleteFile;
                request.Timeout = -1;

				File.AppendAllText(path, DateTime.Now + " " + "ftp://address" + fileName + ftpUsername + ftpPassword + Environment.NewLine);
                request.GetResponse();
                request = null;
            }

            void ConnectToFTP(string UserName, string pass)
            {
				FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create("ftp://address");
                List<string> files = new List<string>();

                string YesterdayDate = DateTime.Now.AddDays(-1).ToString("yyyy_MM_dd");

                ftpRequest.Credentials = new NetworkCredential(UserName, pass);
                try
                {
                    ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                    ftpRequest.Timeout = 300000;
                    File.AppendAllText(path, DateTime.Now + " " + UserName + Environment.NewLine);

                    using (FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse())
                    using (Stream responseStream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        File.AppendAllText(path, DateTime.Now + " " + "Pobrano listę plików" + Environment.NewLine);

                        while (!reader.EndOfStream)
                        {
                            files.Add(reader.ReadLine());
                        }
                        reader.Close();
                        response.Close();

                    }

                }
                catch(Exception ex)
                {
                    File.AppendAllText(path, DateTime.Now + " " + ex.StackTrace.ToString() + Environment.NewLine);
                }

                File.AppendAllText(path, DateTime.Now + " " + TodaysDate + Environment.NewLine);

                foreach (string Txt in files)
                {
                    if (!Txt.Trim().StartsWith(TodaysDate) && !Txt.Trim().StartsWith(YesterdayDate))
                    {
                        File.WriteAllText(path, DateTime.Now + " " + Txt + Environment.NewLine);
                        DeleteFileOnFtpServer(Txt, UserName, pass);
                    }
                }
                File.AppendAllText(path, DateTime.Now + " " + "Usunięto pliki" + Environment.NewLine);

                File.AppendAllText(path, DateTime.Now + " " + "koniec fukncji" + Environment.NewLine);
                File.AppendAllText(path, "-------------------------------------------------------------------" + Environment.NewLine);
                ftpRequest = null;
                files.Clear();
            }
            File.AppendAllText(path, DateTime.Now + " " + "koniec programu" + Environment.NewLine);
        }
    }
}
