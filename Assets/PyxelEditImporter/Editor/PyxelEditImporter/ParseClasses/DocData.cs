using Newtonsoft.Json;
using System.Collections.Generic;

namespace net.fiveotwo.pyxelImporter
{
    public class DocData
    {
        public string version;
        public string name;
        public Canvas canvas;
        [JsonProperty("animations")]
        public IDictionary<int, Animation> animations { get; set; }
    }
}