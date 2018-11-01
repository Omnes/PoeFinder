using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace PoeItemFinderServer
{
    public class RiverUpdate
    {
        public string next_change_id;
        public List<Stash> stashes = new List<Stash>();

        public static RiverUpdate CreateNew(string jsonString)
        {

            ///////////////// only for debugging
            //File.WriteAllText("StashData.txt", JsonConvert.SerializeObject(JsonConvert.DeserializeObject<JObject>(jsonString),Formatting.Indented));

            var riverUpdate = JsonConvert.DeserializeObject<RiverUpdate>(jsonString);
            return riverUpdate;
        }


        public class Stash
        {
            public string accountName;
            public string lastCharacterName;
            public string id;
            [JsonProperty("stash")]
            public string stashName;
            public string stashType;
            public List<Item> items;
            [JsonProperty("public")]
            public bool isPublic;
        }

        public class Item
        {
            public bool vertified;
            public int w;
            public int h;
            public int ilvl;
            public string icon;
            public bool support;
            public string league;
            public string id;
            public string old_id;
            public List<Socket> sockets;
            public string name;
            public string typeLine;
            public bool identified;
            public bool corrupted;
            public bool lockedToCharacter;
            public string note;
            public List<Property> properties;
            public List<Requirement> requirements;
            public List<string> explicitMods;
            public List<string> implicitMods;
            public List<string> flavourText;
            public int frameType;
            public string artFilename;
            public int x;
            public int y;
            public string inventoryId;
            public List<Item> socketedItems;

        }

        public class Socket
        {
            public int group;
            public string attr;
        }

        public class Property
        {
            public string name;
            public List<List<string>> values; //TODO look into a better solution. The original json for this ("values":[["23",0]]) contains a array with both strings and ints
            public int displayMode;
        }

        public class Requirement
        {
            public string name;
            public List<List<string>> values;  
            public int displayMode;
        }
    }
}
