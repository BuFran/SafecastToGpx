using System;
using System.Xml;
using System.Xml.Serialization;

namespace Files.Gpx10 
{
    [Serializable()]
    [XmlType(AnonymousType=true)]
    [XmlRoot("gpx",IsNullable=false)]
    public partial class FileGpx10 {
        
        public FileGpx10() {
            this.version = "1.0";
        }
        
        public string name;
        public string desc;
        public string author;
        public string email;
        
        [XmlElement(DataType = "anyURI")]
        public string url;
        public string urlname;
        public DateTime time;
        [XmlIgnoreAttribute()]
        public bool timeSpecified;
        public string keywords;
        public boundsType bounds;

        [XmlElement("wpt")]        public gpxWpt[] Waypoints;
        [XmlElement("rte")]        public gpxRte[] Routes;
        [XmlElement("trk")]        public gpxTrk[] Tracks;
        [XmlAnyElementAttribute()] public XmlElement[] Any;
        
        [XmlAttribute()]           public string version;
        [XmlAttribute()]           public string creator;
    }
    
    
    
    [Serializable()]
    public partial class boundsType 
    {

        [XmlAttribute()]   public decimal minlat;
        [XmlAttribute()]   public decimal minlon;
        [XmlAttribute()]   public decimal maxlat;
        [XmlAttribute()]   public decimal maxlon;
    }
    
    
    
    [Serializable()]
    [XmlTypeAttribute(AnonymousType=true)]
    public partial class gpxWpt 
    {
        public decimal ele;
        [XmlIgnoreAttribute()]
        public bool eleSpecified;
        public DateTime time;
        [XmlIgnoreAttribute()]
        public bool timeSpecified;
        public decimal magvar;
        [XmlIgnoreAttribute()]
        public bool magvarSpecified;
        public decimal geoidheight;
        [XmlIgnoreAttribute()]
        public bool geoidheightSpecified;
        public string name;
        public string cmt;
        public string desc;
        public string src;
        [XmlElement(DataType = "anyURI")]
        public string url;
        public string urlname;
        public string sym;
        public string type;
        public fixType fix;
        [XmlIgnoreAttribute()]
        public bool fixSpecified;
        [XmlElement(DataType = "nonNegativeInteger")]
        public string sat;
        public decimal hdop;
        [XmlIgnoreAttribute()]
        public bool hdopSpecified;
        public decimal vdop;
        [XmlIgnoreAttribute()]
        public bool vdopSpecified;
        public decimal pdop;
        [XmlIgnoreAttribute()]
        public bool pdopSpecified;
        public decimal ageofdgpsdata;
        [XmlIgnoreAttribute()]
        public bool ageofdgpsdataSpecified;

        [XmlElement(DataType="integer")]    public string dgpsid;
        [XmlAnyElementAttribute()]  public XmlElement[] Any;
        [XmlAttribute()]            public decimal lat;
        [XmlAttribute()]            public decimal lon;
    }
    
    
    
    [Serializable()]
    public enum fixType 
    {
        none,
        [XmlEnum("2d")]   Item2d,
        [XmlEnum("3d")]   Item3d,
        dgps,
        pps,
    }
    
    [Serializable()]
    [XmlTypeAttribute(AnonymousType=true)]
    public partial class gpxRte 
    {
        public string name;
        public string cmt;
        public string desc;
        public string src;
        [XmlElement(DataType = "anyURI")]
        public string url;
        public string urlname;
        [XmlElement(DataType = "nonNegativeInteger")]
        public string number;
        [XmlAnyElementAttribute()]    public XmlElement[] Any;
        [XmlElement("rtept")]         public gpxRteRtept[] Points;
    }
    
    [Serializable()]
    [XmlTypeAttribute(AnonymousType=true)]
    public partial class gpxRteRtept 
    {
        public decimal ele;
        [XmlIgnoreAttribute()]
        public bool eleSpecified;
        public DateTime time;
        [XmlIgnoreAttribute()]
        public bool timeSpecified;
        public decimal magvar;
        [XmlIgnoreAttribute()]
        public bool magvarSpecified;
        public decimal geoidheight;
        [XmlIgnoreAttribute()]
        public bool geoidheightSpecified;
        public string name;
        public string cmt;
        public string desc;
        public string src;
        [XmlElement(DataType = "anyURI")]
        public string url;
        public string urlname;
        public string sym;
        public string type;
        public fixType fix;
        [XmlIgnoreAttribute()]
        public bool fixSpecified;
        [XmlElement(DataType = "nonNegativeInteger")]
        public string sat;
        public decimal hdop;
        [XmlIgnoreAttribute()]
        public bool hdopSpecified;
        public decimal vdop;
        [XmlIgnoreAttribute()]
        public bool vdopSpecified;
        public decimal pdop;
        [XmlIgnoreAttribute()]
        public bool pdopSpecified;
        public decimal ageofdgpsdata;
        [XmlIgnoreAttribute()]
        public bool ageofdgpsdataSpecified;
        [XmlElement(DataType = "integer")]  public string dgpsid;
        [XmlAnyElementAttribute()]          public XmlElement[] Any;
        [XmlAttribute()]                    public decimal lat;
        [XmlAttribute()]                    public decimal lon;
    }
    
    [Serializable()]
    [XmlTypeAttribute(AnonymousType=true)]
    public partial class gpxTrk 
    {
        public string name;
        public string cmt;
        public string desc;
        public string src;
        [XmlElement(DataType = "anyURI")]
        public string url;
        public string urlname;
        [XmlElement(DataType = "nonNegativeInteger")]  public string number;
        [XmlAnyElementAttribute()]                     public XmlElement[] Any;
        [XmlArrayItem("trkpt", typeof(gpxTrkTrksegTrkpt), IsNullable = false)]
        public gpxTrkTrksegTrkpt[] trkseg;
    }
    
    [Serializable()]
    [XmlTypeAttribute(AnonymousType=true)]
    public partial class gpxTrkTrksegTrkpt 
    {
        public decimal ele;
        [XmlIgnoreAttribute()]
        public bool eleSpecified;
        public DateTime time;
        [XmlIgnoreAttribute()]
        public bool timeSpecified;
        public decimal course;
        [XmlIgnoreAttribute()]
        public bool courseSpecified;
        public decimal speed;
        [XmlIgnoreAttribute()]
        public bool speedSpecified;
        public decimal magvar;
        [XmlIgnoreAttribute()]
        public bool magvarSpecified;
        public decimal geoidheight;
        [XmlIgnoreAttribute()]
        public bool geoidheightSpecified;
        public string name;
        public string cmt;
        public string desc;
        public string src;
        [XmlElement(DataType = "anyURI")]
        public string url;
        public string urlname;
        public string sym;
        public string type;
        public fixType fix;
        [XmlIgnoreAttribute()]
        public bool fixSpecified;
        [XmlElement(DataType = "nonNegativeInteger")]
        public string sat;
        public decimal hdop;
        [XmlIgnoreAttribute()]
        public bool hdopSpecified;
        public decimal vdop;
        [XmlIgnoreAttribute()]
        public bool vdopSpecified;
        public decimal pdop;
        [XmlIgnoreAttribute()]
        public bool pdopSpecified;
        public decimal ageofdgpsdata;
        [XmlIgnoreAttribute()]
        public bool ageofdgpsdataSpecified;
        [XmlElement(DataType = "integer")]    public string dgpsid;
        [XmlAnyElementAttribute()]            public XmlElement[] Any;
        [XmlAttribute()]                      public decimal lat;
        [XmlAttribute()]                      public decimal lon;
    }
}
