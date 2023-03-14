using Newtonsoft.Json;
using System.Collections.Generic;

namespace net.fiveotwo.pyxelImporter
{
    public class Canvas
    {
        public int numLayers;
        public int width;
        public int tileWidth;
        public int height;
        public int tileHeight;
        public int currentLayerIndex;
        [JsonProperty("layers")]
        public IDictionary<int, Layer> layers { get; set; }
    }
}