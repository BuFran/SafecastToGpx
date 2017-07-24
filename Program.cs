using Ltr.MultiProtocol.Nmea;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SafecastToGpx
{
    class Program
    {

        public static void SerializeXml<T>(T obj, string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.Serialize(ms, obj);
                File.WriteAllText(fileName, Encoding.UTF8.GetString(ms.ToArray()));
            }
        }

        static NMEAMessage ParseNmea(string s)
        {
            try
            {
                var msg = NMEAMessage.Parse(s);
                if (msg == null)
                    return null;

                switch (msg.GetStr(0))
                {
                    case "BNRDD":   // bGeigie nano
                    case "BMRDD":   // bGeigie Mini
                    case "BNXRDD":  // bGeigie NX
                        return new SafecastRow(msg);
                    default:
                        return null;
                }
            }
            catch
            {
                return null;
            }
        }

        static Files.Gpx11.Waypoint NmeaToGpx(NMEAMessage msg)
        {
            try
            {
                var r = msg as SafecastRow;
                if ((r == null))
                    return null;

                if (!r.GPSValidity.StartsWith("A"))
                    return null;

                // BUG in bGeigie nano firmware !!!!!
                int y = r.TimeStamp.Year;
                if ((y < 2016) || (y > 2018))
                    return null;

                return new Files.Gpx11.Waypoint(r.Latitude, r.Longitude, r.Altitude, r.TimeStamp);
            }
            catch
            {
                // for example datetime formatting error: skip the point
                return null;
            }
        }

        static IEnumerable<string> GetTrack(StreamReader sr)
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (line.StartsWith("# NEW LOG"))
                    yield break;
                yield return line;
            }
        }

        public static void Convert(string filename, string outfile)
        {
            if (File.Exists(outfile))
                return;

            Console.WriteLine($"Analysing file '{filename}'... ");

            List<Files.Gpx11.TrackSegment> Segments = new List<Files.Gpx11.TrackSegment>();
            using (var sr = new StreamReader(filename))
            {
                while (!sr.EndOfStream)
                {
                    var seg = new Files.Gpx11.TrackSegment(
                        GetTrack(sr)
                        .Where(x => !string.IsNullOrEmpty(x) && !x.StartsWith("#"))
                        .Select(ParseNmea)
                        .Select(NmeaToGpx)
                        .Where(x => x != null)
                        );

                    if (seg.Points.Length < 1)
                        continue;

                    // TODO split points where there is space longer than XX seconds

                    Segments.Add(seg);

                    Console.WriteLine($" - new Segment found ! Length={seg.Points.Length} ");
                }
            }

            Console.WriteLine($" - Total segments = {Segments.Count} ");

            var gpx = new Files.Gpx11.FileGpx11("Safecast To GPX Extractor", new Files.Gpx11.Track($"Track1", Segments));

            Console.WriteLine($"Writing file '{filename}'... ");

            SerializeXml(gpx, Path.ChangeExtension(filename, ".gpx"));
        }

        static void Main(string[] args)
        {
            Console.WriteLine(@"Safecast To Gpx converter version 1.0
Copyright (c) Frantisek Burian <BuFran _at_ seznam.cz>
License GPLv3+: GNU GPL version 3 or later < http://gnu.org/licenses/gpl.html>
This is free software: you are free to change and redistribute it.
There is NO WARRANTY, to the extent permitted by law.");

            // TODO: Add commandline parsing

            foreach (var log in Directory.EnumerateFiles("./", "*.log"))
                Convert(log, Path.ChangeExtension(log, ".gpx"));
            
        }
    }
}
