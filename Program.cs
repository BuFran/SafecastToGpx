using Files.Gpx11;
using Ltr.MultiProtocol.Nmea;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SafecastToGpx
{
    class Program
    {
        static bool CfgSplitFiles = false;
        static bool CfgSplitTracks = false;
        static bool CfgSplitSegments = false;
        static bool CfgSplitNothing = false;

        static bool CfgSilent = false;
        static bool CfgMultithreaded = true;

        static bool CfgHelp = false;
        static bool CfgVersion = false;

        static string CfgFileMask = null;

        static TimeSpan CfgTimeShift = new TimeSpan(0, 0, 0);

        public static void ConvertFile(string filename)
        {
            if (!CfgSilent)
                Console.WriteLine($"Analysing file '{filename}'... ");

            DataBase db = new DataBase(filename);

            // todo run some analysis over the file

            var Segments = db.AllTracks.Select(
               x => new TrackSegment(x.Select(
                   z => new Waypoint(z.GpsLatitude, z.GpsLongitude, z.GpsAltitude, z.GpsTimeStamp + CfgTimeShift)))).ToArray();

            var Tracks = Segments.Select((x, i) => new Track($"Track{i}", new[] { x }));


            if (CfgSplitFiles || (!CfgSplitFiles && !CfgSplitNothing && !CfgSplitSegments && !CfgSplitTracks))
            {
                foreach (var seg in Tracks)
                    new FileGpx11("Safecast To GPX Extractor", seg)
                        .SerializeToXml(Path.ChangeExtension(filename, $".{seg.name}.gpx"));
            }

            string of = Path.ChangeExtension(filename, $".gpx");
            if (CfgSplitTracks)
            {
                new FileGpx11("Safecast To GPX Extractor", Tracks).SerializeToXml(of);
            }

            if (CfgSplitSegments)
            {
                new FileGpx11("Safecast To GPX Extractor", new Track($"Track1", Segments)).SerializeToXml(of);
            }

            if (CfgSplitNothing)
            {
                new FileGpx11("Safecast To GPX Extractor", new Track($"Track_cat", Segments.SelectMany(x=> x.Points).ToArray())).SerializeToXml(of);
            }

            if (!CfgSilent)
                Console.WriteLine($"File '{filename}' processed. Found {Segments.Count()} segments");
        }

        static string getArg(IEnumerator<string> e, string def, bool advance=false)
        {
            if (e.Current.Contains('='))
                return e.Current.Split(new[] { '=' }, 2)[1];

            if (advance && e.MoveNext())
                return e.Current;

            return def;
        }

        static bool getBoolArg(IEnumerator<string> e, bool def, bool advance = false)
        {
            return bool.Parse(getArg(e, def.ToString(), advance));
        }

        static Regex time = new Regex(@"([+-]?)(\d{1,2}):(\d{1,2})(?::(\d{1,2}))?", RegexOptions.Compiled);
        static Dictionary<string, TimeSpan> KnownShifts = new Dictionary<string, TimeSpan>
        {
            ["UTC"] = new TimeSpan(0, 0, 0),
            ["GMT"] = new TimeSpan(0, 0, 0),
            ["CET"] = new TimeSpan(1, 0, 0),
            ["CEST"] = new TimeSpan(2, 0, 0),
        };
        // TODO: Add important time zones from table https://en.wikipedia.org/wiki/List_of_time_zone_abbreviations
        static TimeSpan getTimeArg(IEnumerator<string> e, string def,bool advance = false)
        {
            string s = getArg(e, def, advance);

            if (KnownShifts.ContainsKey(s))
                return KnownShifts[s];

            var m = time.Match(s);

            if (!m.Success)
                throw new Exception($"This can't be treated as time: '{s}'\r\n try --help for usage");

            var sgn = m.Groups[1].Success ? m.Groups[1].Value : "+";
            var hh = m.Groups[2].Success ? int.Parse(m.Groups[2].Value) : 0;
            var mm = m.Groups[3].Success ? int.Parse(m.Groups[3].Value) : 0;
            var ss = m.Groups[4].Success ? int.Parse(m.Groups[4].Value) : 0;

            if (sgn == "+")
                return new TimeSpan(hh, mm, ss);
            else
                return TimeSpan.Zero - new TimeSpan(hh, mm, ss);
        }

        private static Dictionary<string, Action<IEnumerator<string>>> Commands = new Dictionary<string, Action<IEnumerator<string>>>()
        {
            ["--help"] = a => CfgHelp = true,
            ["--version"] = a => CfgVersion = true,
            ["--split-files"] = a => CfgSplitFiles = getBoolArg(a, true),
            ["--split-tracks"] = a => CfgSplitTracks = getBoolArg(a, true),
            ["--split-segments"] = a => CfgSplitSegments = getBoolArg(a, true),
            ["--split-nothing"] = a => CfgSplitNothing = getBoolArg(a, true),
            ["--silent"] = a => CfgSilent = true,
            ["--multithreaded"] = a => CfgMultithreaded = getBoolArg(a, true),
            ["--timeshift"] = a => CfgTimeShift = getTimeArg(a, "00:00"),
        };

        private static string SVersion = "1.0.deadbeef";
        private static string STitleLine = "Safecast To Gpx converter version ";
        private static string SCopy = "Copyright(c) Frantisek Burian <BuFran _at_ seznam.cz>";
        private static string SLicense = "License GPLv3+: GNU GPL version 3 or later <http://gnu.org/licenses/gpl.html>\r\nThis is free software: you are free to change and redistribute it.\r\nThere is NO WARRANTY, to the extent permitted by law.";
        private static string SUsage = @"
USAGE: 
  SafecastToGpx [options] [files]

OPTIONS:

  --split-files[=true]: (default) 
      generate separate file for each turn on/off of device

  --split-segments[=true]: 
      generate single file, with single track, separated into segments

  --split-tracks[=true]:
      generate single file, with multiple tracks, single segment each

  --split-nothing[=true]:
      generate single file, do not split data

  --silent:
      do not print any texts to stdout

  --multithreaded[=true]: (default)
      work with multiple threads in parallell

  --timeshift=[+-]HH:MM[:SS]
  --timeshift=CET
      shift UTC datetime to local time. Time zone abbreviations allowed. 
";

        static void Main(string[] args)
        {
            // parse command line
            var arg = Environment.GetCommandLineArgs().OfType<string>().GetEnumerator();
            arg.MoveNext();

            while (arg.MoveNext())
            {
                if (arg.Current.StartsWith("--"))
                {
                    Action<IEnumerator<string>> act;
                    if (!Commands.TryGetValue(arg.Current.Split(new[] { '=' }, 2)[0], out act))
                        throw new Exception($"Unknown command line parameter: '{arg.Current}'\r\n try --help for usage");

                    act(arg);
                }
                else
                    CfgFileMask = arg.Current;
            }

            if (CfgVersion)
            {
                Console.WriteLine(SVersion);
                return;
            }

            if (CfgHelp)
            {
                Console.WriteLine(STitleLine + SVersion);
                Console.WriteLine(SCopy);
                Console.WriteLine(SLicense);
                Console.WriteLine(SUsage);
                return;
            }

            
            if (!CfgSilent)
                Console.WriteLine(STitleLine + SVersion);

            if (string.IsNullOrEmpty(CfgFileMask))
                CfgFileMask = "*.log";

            var files = Directory.EnumerateFiles("./", CfgFileMask);

            if (CfgMultithreaded)
                Parallel.ForEach(files, ConvertFile);
            else
                foreach (var log in files)
                    ConvertFile(log);            
        }
    }
}
