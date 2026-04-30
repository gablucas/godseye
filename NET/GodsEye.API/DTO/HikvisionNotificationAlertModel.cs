using System.Xml.Serialization;

namespace GodsEye.API.DTO
{
    [XmlRoot("EventNotificationAlert", Namespace = "http://www.hikvision.com/ver20/XMLSchema"
)]
    public class HikvisionEventNotificationAlert
    {
        [XmlElement("ipAddress")]
        public string IpAddress { get; set; }

        [XmlElement("portNo")]
        public int PortNo { get; set; }

        [XmlElement("protocol")]
        public string Protocol { get; set; }

        [XmlElement("macAddress")]
        public string MacAddress { get; set; }

        [XmlElement("channelID")]
        public int ChannelID { get; set; }

        [XmlElement("dateTime")]
        public DateTime DateTime { get; set; }

        [XmlElement("activePostCount")]
        public int ActivePostCount { get; set; }

        [XmlElement("eventType")]
        public string EventType { get; set; }

        [XmlElement("eventState")]
        public string EventState { get; set; }

        [XmlElement("eventDescription")]
        public string EventDescription { get; set; }

        [XmlElement("inputIOPortID")]
        public int InputIOPortID { get; set; }

        [XmlElement("channelName")]
        public string ChannelName { get; set; }
    }
}
