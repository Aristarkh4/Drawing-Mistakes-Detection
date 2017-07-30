using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drawing_Mistakes_Detection
{
    public class DrawingWithTag
    {
        [Newtonsoft.Json.JsonProperty("id")]
        public string Id { get; set; }

        [Newtonsoft.Json.JsonProperty("DateUtc")]
        public DateTime DateUtc { get; set; }

        [Newtonsoft.Json.JsonProperty("TagId")]
        public byte TagId { get; set; }
    }
}