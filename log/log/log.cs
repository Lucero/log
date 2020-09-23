using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace log
{
    //多线程 + 缓存 版本。
    //上层写到缓存中，这样几乎不会阻塞，最低程度降低对了测试时间的影响。由后台线程将缓存中数据写到磁盘中。
    //缺点：可能在大量数据未写完就退出程序，导致数据不完整。
    public static class log
    {
        public enum enDirection
        {
            Send,
            Receive
        }
        
        public static void SerialNumber(string sn)
        {
            WriteLine("[Product SN] " + sn);
        }

        public static void Delay(double t)
        {
            string str = "";
            if (t < 1.0)
            {
                str = "[Delay] " + Convert.ToInt64(t * 1000).ToString() + "ms";
            }
            else
            {
                str = "[Delay] " + t.ToString("0.0") + "s";
            }
            WriteLine(str);
        }

        public static void DisplayMessage(string s)
        {
            WriteLine(s);
        }

        #region Serial Port
        public static void SerialPort(SerialPort serial, string str, enDirection direct = enDirection.Send)
        {
            string serialConfig = "";
            string appendString = "";
            try
            {
                serialConfig = serial.PortName + "," + serial.BaudRate.ToString() + "," + serial.DataBits.ToString() + "," + serial.Parity.ToString() + "," + serial.StopBits.ToString();
                if (direct == enDirection.Send)
                {
                    appendString = "[" + serialConfig + " " + "S] " + str;
                }
                else if (direct == enDirection.Receive)
                {
                    appendString = "[" + serialConfig + " " + "R] " + str;
                }
                //appendString = clsUtils.Ascii2Reveal(appendString);
                WriteLine(appendString);
            }
            catch { }
        }
        #endregion

        #region NICard
        public static void NICard_WriteDigPort(string cardType, UInt32 line, UInt32 port, string portStatus)
        {
            WriteLine("[" + cardType + " set port] P" + line.ToString() + "_" + port.ToString() + " = " + portStatus);
        }
        public static void NICard_ReadDigPort(string cardType, UInt32 line, UInt32 port, string portStatus)
        {
            WriteLine("[" + cardType + " get port] P" + line.ToString() + "_" + port.ToString() + " = " + portStatus);
        }
        public static void NICardReadAI(string cardType, UInt32 ai, double value)
        {
            WriteLine("[" + cardType + " read AI] AI" + ai.ToString() + " = " + value.ToString("0.000"));
        }
        #endregion






        public static void Write(string msg)
        {
            Base.Write(msg);
        }
        public static void WriteLine(string msg)
        {
            Base.WriteLine(DateTime.Now.ToString("[HH:mm:ss:fff] ") + msg);
        }
        public static void Delete()
        {
            Base.Delete();
        }
        public static void Copy(string path)
        {
            Base.Copy(path);
        }

        #region Base
        private static class Base
        {
            #region Private
            private static string fdir = "./data";
            private static string fname = "DetailReport.txt";
            private static string fpath = fdir + "/" + fname;
            private static List<string> buffer = new List<string>();
            private static object locker = new object();
            private static void thread()
            {
                string tmpbuf = "";
                while (true)
                {
                    try
                    {
                        tmpbuf = "";
                        lock (locker)
                        {
                            if (buffer.Count != 0)
                            {
                                tmpbuf += buffer[0];
                                buffer.RemoveAt(0);
                            }
                        }
                        if (tmpbuf != "")
                        {
                            if (Directory.Exists(fdir) == false)
                            {
                                Directory.CreateDirectory(fdir);
                            }
                            if (File.Exists(fpath) == false)
                            {
                                using (File.Create(fpath)) { }
                            }
                            using (FileStream fs = new FileStream(fpath, FileMode.Open, FileAccess.Write))
                            {
                                using (StreamWriter sw = new StreamWriter(fs))
                                {
                                    sw.BaseStream.Seek(0, SeekOrigin.End);
                                    sw.Write(tmpbuf);
                                    sw.Flush();
                                }
                            }
                        }
                        else
                            Thread.Sleep(100);
                    }
                    catch { }
                }
            }
            #endregion
            static Base() //初始化
            {
                Thread th = new Thread(thread);
                th.IsBackground = true;
                th.Start();
            }
            public static void Write(string msg)
            {
                try
                {
                    lock (locker)
                    {
                        buffer.Add(msg);
                    }
                }
                catch { }
            }
            public static void WriteLine(string msg)
            {
                Write(msg + "\n");
            }
            public static void Delete()
            {
                try
                {
                    lock (locker)
                    {
                        if (Directory.Exists(fdir) == true)
                        {
                            if (File.Exists(fpath) == true)
                                File.Delete(fpath);
                        }
                    }
                }
                catch { }
            }
            public static void Copy(string path)
            {
                try
                {
                    lock (locker)
                    {
                        if (File.Exists(fpath) == true)
                            File.Copy(fpath, path);
                    }
                }
                catch { }
            }
        }
        #endregion
    }
}
