using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TrolaInfo.Classes
{
    public class Error
    {
        [JsonProperty("error")]
        public string error { get; set; }
    }
}
