using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Files.Gpx11
{
    [Serializable()]
    [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
    [XmlRoot("gpx", Namespace = "http://www.topografix.com/GPX/1/1", IsNullable = false)]
    public class FileGpx11
    {
        public FileGpx11()
        {
            this.Version = "1.1";
        }

        
        public FileGpx11(string creator, IEnumerable<Track> tracks)
        {
            this.Version = "1.1";
            this.Creator = creator;
            Tracks = tracks.ToArray();
        }

        public FileGpx11(string creator, params Track[] tracks)
        {
            this.Version = "1.1";
            this.Creator = creator;
            Tracks = tracks;
        }

        public void SerializeToXml(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(this.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.Serialize(ms, this);
                File.WriteAllText(fileName, System.Text.Encoding.UTF8.GetString(ms.ToArray()));
            }
        }

        [XmlElement("metadata")]  public Metadata Metadata;
        [XmlElement("wpt")]       public Waypoint[] Waypoints;
        [XmlElement("rte")]       public Route[] Routes;
        [XmlElement("trk")]       public Track[] Tracks;
        [XmlElement("extensions")]public Extensions _Extensions;
        [XmlAttribute("version")] public string Version;
        [XmlAttribute("creator")] public string Creator;
    }

    [Serializable()]
    [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
    public class Metadata
    {
        public string name;
        public string desc;
        public Person author;
        public Copyright copyright;

        [XmlElement("link")]
        public Link[] Links;
        public DateTime time;

        [XmlIgnore()]
        public bool timeSpecified;
        public string keywords;
        public Bounds bounds;
        [XmlElement("extensions")]
        public Extensions _Extensions;
    }


    [Serializable()]
    [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
    public class Person
    {
        public string name;
        public Email email;
        public Link link;

        public override string ToString()
        {
            return name + " < " + email.ToString() + " > " + link.ToString();
        }
    }


    [Serializable()]
    [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
    public class Email
    {
        [XmlAttribute()] public string id;
        [XmlAttribute()] public string domain;

        public override string ToString()
        {
            return id + "@" + domain;
        }
    }


    [Serializable()]
    [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
    public class TrackSegment
    {
        [XmlElement("trkpt")]       public Waypoint[] Points;
        [XmlElement("extensions")]  public Extensions _Extensions;

        public TrackSegment()
        {

        }

        public TrackSegment(IEnumerable<Waypoint> pts)
        {
            Points = pts.ToArray();
        }
    }


    [Serializable()]
    [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
    public class Waypoint
    {
        public decimal ele;
        [XmlIgnore()]
        public bool eleSpecified;
        public DateTime time;
        [XmlIgnore()]
        public bool timeSpecified;
        public decimal magvar;
        [XmlIgnore()]
        public bool magvarSpecified;
        public decimal geoidheight;
        [XmlIgnore()]
        public bool geoidheightSpecified;
        public string name;
        public string cmt;
        public string desc;
        public string src;
        [XmlElement("link")]
        public Link[] Links;
        public string sym;
        public string type;
        public Fix fix;
        [XmlIgnore()]
        public bool fixSpecified;
        [XmlElement(DataType = "nonNegativeInteger")]
        public string sat;
        public decimal hdop;
        [XmlIgnore()]
        public bool hdopSpecified;
        public decimal vdop;
        [XmlIgnore()]
        public bool vdopSpecified;
        public decimal pdop;
        [XmlIgnore()]
        public bool pdopSpecified;
        public decimal ageofdgpsdata;
        [XmlIgnore()]
        public bool ageofdgpsdataSpecified;
        [XmlElement(DataType = "integer")]
        public string dgpsid;
        [XmlElement("extensions")]
        public Extensions _Extensions;
        public decimal speed;
        [XmlIgnore()]
        public bool speedSpecified;
        [XmlAttribute()]
        public decimal lat;
        [XmlAttribute()]
        public decimal lon;

        public override string ToString()
        {
            return lat.ToString("F7",CultureInfo.InvariantCulture) + "," + lon.ToString("F7",CultureInfo.InvariantCulture);
        }

        public Waypoint()
        {
        }

        public Waypoint(double Lat, double Lon, double Ele, DateTime Time)
        {
            lat = (decimal)Lat;
            lon = (decimal)Lon;
            ele = (decimal)Ele;
            time = Time;
            eleSpecified = timeSpecified = true;
        }
    }

    [Serializable()]
    [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
    public class Link
    {
        public string text;
        public string type;
        [XmlAttribute(DataType = "anyURI")]
        public string href;

        public override string ToString()
        {
            return text + " [ " + href + " ] ";
        }
    }


    [Serializable()]
    [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
    public enum Fix
    {
        none,
        [XmlEnum("2d")]
        Item2d,
        [XmlEnum("3d")]
        Item3d,
        dgps,
        pps,
    }


    [Serializable()]
    [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
    public partial class Extensions
    {
        [XmlAnyElement()] public XmlElement[] Any;
    }


    [Serializable()]
    [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
    public class Track
    {
        public string name;
        public string cmt;
        public string desc;
        public string src;
        [XmlElement("link")]
        public Link[] Links;
        [XmlElement(DataType = "nonNegativeInteger")]
        public string number;
        public string type;
        [XmlElement("extensions")]
        public Extensions _Extensions;
        [XmlElement("trkseg")]
        public TrackSegment[] Segments;

        public Track()
        {
        }

        public Track(string name, Waypoint[] points)
        {
            this.name = name;
            Segments = new TrackSegment[1];
            Segments[0] = new TrackSegment();
            Segments[0].Points = points;
        }

        public Track(string name, IEnumerable<TrackSegment> segments)
        {
            this.name = name;
            Segments = segments.ToArray();
        }
    }


    [Serializable()]
    [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
    public class Route
    {
        public string name;
        public string cmt;
        public string desc;
        public string src;
        [XmlElement("link")]
        public Link[] link;
        [XmlElement(DataType = "nonNegativeInteger")]
        public string number;
        public string type;
        [XmlElement("extensions")]
        public Extensions _Extensions;
        [XmlElement("rtept")]
        public Waypoint[] Points;
    }


    [Serializable()]
    [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
    public partial class Bounds
    {
        [XmlAttribute()] public decimal minlat;
        [XmlAttribute()] public decimal minlon;
        [XmlAttribute()] public decimal maxlat;
        [XmlAttribute()] public decimal maxlon;
    }


    [Serializable()]
    [XmlType(Namespace = "http://www.topografix.com/GPX/1/1")]
    public class Copyright
    {
        [XmlElement(DataType = "gYear")]
        public string year;
        [XmlElement(DataType = "anyURI")]
        public string license;
        [XmlAttribute()]
        public string author;
    }
}