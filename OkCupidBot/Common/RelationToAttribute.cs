using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OkCupidBot.Common
{
    public class RelationToAttribute : Attribute
    {
        public string Name { get; set; }

        public RelationToAttribute(string name)
        {
            this.Name = name;
        }
    }
}
