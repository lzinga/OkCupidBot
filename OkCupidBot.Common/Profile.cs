using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidBot.Common
{
    public class Profile
    {
        public string Username { get; set; }
        public Uri ProfilePage { get; set; }
        public int Age { get; set; }
        public string Location { get; set; }
        public int MatchPercent { get; set; }
        public List<Orientation> Orientation { get; set; }
        public RelationshipStatus RelationshipStatus { get; set; }
        public BodyType BodyType { get; set; }
        public List<Ethnicity> Ethnicity { get; set; }
        public Height Height { get; set; }
        public Smoking Smoking { get; set; }
        public Sex Sex { get; set; }
        public bool IsOnline { get; set; }

        public string SelfSummary { get; set; }

        public Profile()
        {
            Orientation = new List<Orientation>();
            Ethnicity = new List<Ethnicity>();
        }

        public Profile(string username, Uri profile)
        {
            this.Username = username;
            this.ProfilePage = profile;

            Orientation = new List<Orientation>();
            Ethnicity = new List<Ethnicity>();
        }
    }
}
