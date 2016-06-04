using OkCupidBot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OkCupidBot.Models
{
    public class MessageSettings
    {
        [XmlElement("MessageGroup")]
        public List<MessageGroup> Groups { get; set; }

        public MessageSettings()
        {
            Groups = new List<MessageGroup>();
        }
    }

    public class MessageGroup
    {
        public List<MessageCondition> Conditions { get; set; }

        [XmlElement("Message")]
        public List<Message> Messages { get; set; }

        public MessageGroup()
        {
            Conditions = new List<MessageCondition>();
            Messages = new List<Message>();
        }
    }

    [XmlType("Condition")]
    public class MessageCondition
    {
        [XmlAttribute]
        public string Key { get; set; }

        [XmlAttribute]
        public Operator Operator { get; set; }

        [XmlAttribute]
        public string Value { get; set; }
    }

    public class Message
    {
        [XmlAttribute]
        public string Value { get; set; }
    }
}
