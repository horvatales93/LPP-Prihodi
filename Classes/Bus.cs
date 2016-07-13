using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrolaInfo.Classes
{
    public class Bus
    {
        public string direction { get; set; }
        public string number { get; set; }
        public List<string> arrivals { get; set; }
    }
}
