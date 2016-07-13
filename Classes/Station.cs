using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrolaInfo.Classes
{
    public class Station
    {
        public string name { get; set; }
        public string number { get; set; }
        public List<Bus> buses { get; set; }
    }
}
