﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Configuration;
using System.IO;
using System.Net;


namespace CmsCheckin
{
    public class Print
    {
        public static void MemberList(string orgid)
        {
            Uri url;
            string str;
            XDocument x;
            url = new Uri(new Uri(Util.ServiceUrl()),
                string.Format("Checkin/Class/{0}", Util.GetDigits(orgid)));
            var wc = new WebClient();
            str = wc.DownloadString(url + Program.QueryString);
            if (string.IsNullOrEmpty(str))
                return;
            x = XDocument.Parse(str);
            var list = x.Root.Descendants("Name").Select(m => m.Value).ToList();
            var n = list.Count / 5;
            if (n % 5 > 0)
                n++;
            n = n * 5 - 1;
            for (; n > 0; n -= 5)
                PrintLabel5(list, n);

            list = new List<string>();
            list.Add(x.Root.Attribute("Name").Value);
            list.Add(x.Root.Attribute("Teacher").Value);
            list.Add(x.Root.Attribute("Date").Value);
            list.Add(x.Root.Attribute("Time").Value);
            list.Add("Count: " + x.Root.Attribute("Count").Value);
            PrintLabel5(list, 4);

            BlankLabel();
        }
        private static void PrintLabel5(List<string> list, int n)
        {
            var memStrm = new MemoryStream();
            var sw = new StreamWriter(memStrm);
            if (Program.Printer.StartsWith("Datamax"))
            {
                sw.WriteLine("\x02n");
                sw.WriteLine("\x02M0500");
                sw.WriteLine("\x02O0220");
                sw.WriteLine("\x02V0");
                sw.WriteLine("\x02SG");
                sw.WriteLine("\x02d");
                sw.WriteLine("\x01D");
                sw.WriteLine("\x02L");
                sw.WriteLine("D11");
                sw.WriteLine("PG");
                sw.WriteLine("pC");
                sw.WriteLine("SG");
                sw.WriteLine("ySPM");
                sw.WriteLine("A2");
                if (list.Count > n)
                    sw.WriteLine("1911A1000040010" + list[n]);
                n--;
                if (list.Count > n)
                    sw.WriteLine("1911A1000210010" + list[n]);
                n--;
                if (list.Count > n)
                    sw.WriteLine("1911A1000370010" + list[n]);
                n--;
                if (list.Count > n)
                    sw.WriteLine("1911A1000540010" + list[n]);
                n--;
                if (list.Count > n)
                    sw.WriteLine("1911A1000700010" + list[n]);
                n--;
                sw.WriteLine("Q0001");
                sw.WriteLine("E");
            }
            if (Program.Printer.StartsWith("ZDesigner"))
            {
                sw.WriteLine(@"^XA~TA000~JSN^LT0^MNW^MTD^PON^PMN^LH0,0^JMA^PR2,2~SD15^JUS^LRN^CI0^XZ");
                sw.WriteLine(@"^XA");
                sw.WriteLine(@"^MMT");
                sw.WriteLine(@"^PW609");
                sw.WriteLine(@"^LL0406");
                sw.WriteLine(@"^LS0");
                if (list.Count > n)
                    sw.WriteLine(string.Format(@"^FT24,179^A0N,28,28^FH\^FD{0}^FS", list[n]));
                n--;
                if (list.Count > n)
                    sw.WriteLine(string.Format(@"^FT24,144^A0N,28,28^FH\^FD{0}^FS", list[n]));
                n--;
                if (list.Count > n)
                    sw.WriteLine(string.Format(@"^FT24,110^A0N,28,28^FH\^FD{0}^FS", list[n]));
                n--;
                if (list.Count > n)
                    sw.WriteLine(string.Format(@"^FT24,76^A0N,28,28^FH\^FD{0}^FS", list[n]));
                n--;
                if (list.Count > n)
                    sw.WriteLine(string.Format(@"^FT24,41^A0N,28,28^FH\^FD{0}^FS", list[n]));
                n--;
                sw.WriteLine(string.Format(@"^PQ1,0,1,Y^XZ"));
            }
            sw.Flush();

            memStrm.Position = 0;
            PrintRawHelper.SendDocToPrinter(Program.Printer, memStrm);
            sw.Close();
        }
        public const string datamax1 = "Datamax E-4203";
        public const string datamax2 = "Datamax E-4204";
        public const string zebra = "ZDesigner LP 2844-Z";
        public const string zebra2 = "ZDesigner LP 2844";

        public static void BlankLabel()
        {
            if (Program.Printer.StartsWith("Datamax"))
            {
                var ms = new MemoryStream();
                var st = new StreamWriter(ms);
                st.WriteLine("\x02L");
                st.WriteLine("H07");
                st.WriteLine("D11");
                st.WriteLine("E");
                st.Flush();
                ms.Position = 0;
                PrintRawHelper.SendDocToPrinter(Program.Printer, ms);
                st.Close();
            }
        }
        public static void Label(LabelInfo li, DateTime time)
        {
            if (li.n == 0)
                return;
            var n = li.n;
            if (n > Program.MaxLabels)
                n = Program.MaxLabels;
            var memStrm = new MemoryStream();
            var sw = new StreamWriter(memStrm);
            if (Program.Printer.StartsWith("Datamax"))
            {
                sw.WriteLine("\x02n");
                sw.WriteLine("\x02M0500");
                sw.WriteLine("\x02O0220");
                sw.WriteLine("\x02V0");
                sw.WriteLine("\x02SG");
                sw.WriteLine("\x02d");
                sw.WriteLine("\x01D");
                sw.WriteLine("\x02L");
                sw.WriteLine("D11");
                sw.WriteLine("PG");
                sw.WriteLine("pC");
                sw.WriteLine("SG");
                sw.WriteLine("ySPM");
                sw.WriteLine("A2");
                sw.WriteLine("1911A3000450009" + li.first);
                sw.WriteLine("1911A1000300011" + li.last);
                sw.WriteLine("1911A1000060008" + " (" + li.pid + " " + li.mv + ")" + time.ToString("  M/d/yy"));
                sw.WriteLine("1911A2400040179" + time.ToString("HHmmss"));
                sw.WriteLine("Q" + n.ToString("0000"));
                sw.WriteLine("E");
            }
            if (Program.Printer.StartsWith("ZDesigner"))
            {
                //sw.WriteLine("CT~~CD,~CC^~CT~");
                sw.WriteLine("^XA~TA000~JSN^LT0^MNW^MTD^PON^PMN^LH0,0^JMA^PR2,2~SD15^JUS^LRN^CI0^XZ");
                sw.WriteLine("^XA");
                sw.WriteLine("^MMT");
                sw.WriteLine("^PW609");
                sw.WriteLine("^LL0203");
                sw.WriteLine("^LS0");
                sw.WriteLine(string.Format(@"^FT592,122^A0I,79,79^FH\^FD{0}^FS", li.first));
                sw.WriteLine(string.Format(@"^FT583,69^A0I,34,33^FH\^FD{0}^FS", li.last));
                sw.WriteLine(string.Format(@"^FT583,25^A0I,34,33^FH\^FD{0} {1}   {2:M/d/yy}^FS", li.pid, li.mv, time));
                sw.WriteLine(string.Format(@"^FT203,26^A0I,68,67^FH\^FD{0:HHmmss}^FS", time));
                sw.WriteLine(string.Format("^PQ{0},0,1,Y^XZ", n));
            }
            sw.Flush();

            memStrm.Position = 0;
            PrintRawHelper.SendDocToPrinter(Program.Printer, memStrm);
            sw.Close();
        }

    }
}
