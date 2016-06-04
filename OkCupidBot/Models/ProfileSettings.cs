using OkCupidBot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OkCupidBot.Models
{
    public class ProfileSettings
    {
        [XmlElement("Setting")]
        public List<ProfileSetting> Settings { get; set; }

        public ProfileSettings()
        {
            this.Settings = new List<ProfileSetting>();
        }
    }

    public class ProfileSetting
    {
        public List<ProfileCondition> Conditions { get; set; }
        public List<Requirement> Requirements { get; set; }

        public ProfileSetting()
        {
            this.Conditions = new List<ProfileCondition>();
            this.Requirements = new List<Requirement>();
        }
    }



    public class Requirement
    {
        [XmlAttribute]
        public string Key { get; set; }

        [XmlAttribute]
        public Operator Operator { get; set; }

        [XmlAttribute]
        public string Value { get; set; }
    }

    [XmlType("Condition")]
    public class ProfileCondition
    {
        [XmlAttribute]
        public string Field { get; set; }

        [XmlAttribute]
        public int From { get; set; }

        [XmlAttribute]
        public int To { get; set; }
    }
}
