using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using System.Collections;

namespace PoeItemFinderServer
{
    public class ProcessedItem : IComparer
    {
        public string Name { get; set; }
        public string TypeLine { get; set; }
        public int FrameType { get; set; }
        public string Icon { get; set; }
        public string League { get; set; }
        public int Ilvl { get; set; }

        [Key]
        public string Id { get; set; }
        public string Old_id { get; set; }

        public bool Identified { get; set; }
        public bool Corrupted { get; set; }

        public string ImplicitMods { get; set; }
        public string ExplicitMods { get; set; }

        public string PriceString { get; set; }
        public float ChaosPrice { get; set; }
        public DateTime Date { get; set; }
        public int Links { get; set; }


        public ProcessedItem(RiverUpdate.Stash stash,RiverUpdate.Item item){
            Name = removeTags(item.name); //the name needs to be scrubbed of tags, also some items do not have a name only a typename (divination cards)
            TypeLine = removeTags(item.typeLine);
            FrameType = item.frameType;
            Icon = item.icon;
            League = item.league;
            Ilvl = item.ilvl;
            Id = item.id;
            Old_id = item.old_id;

            Identified = item.identified;
            Corrupted = item.corrupted;

            //TODO these should someday be reduced to a mod identifier and a magnitude (to save space and a shitton of other reasons)
            if (item.implicitMods != null)
            {
                foreach (var line in item.implicitMods)
                {
                    ImplicitMods += line + "\n";
                }
            }
            if (item.explicitMods != null)
            {
                foreach (var line in item.explicitMods)
                {
                    ExplicitMods += line + "\n";
                }
            }

            PriceString = PoeItemFinder.Currency.getPriceString(stash.stashName,item.note); //find out if its the note or the stashname that contains the price 
            ChaosPrice = PoeItemFinder.Currency.getItemPrice(PriceString);

            Date = DateTime.Now;

            Links = getLinks(item.sockets); //find the longest chain in item.sockets

        }

        public ProcessedItem() { }

        /// <summary>
        /// removes tags from names
        /// </summary>
        /// <param name="name">the item name ex: "<<set:MS>><<set:M>><<set:S>>Beast Whorl"</param>
        /// <returns>the name without the tags ex: "Beast Whorl"</returns>
        private string removeTags(string name)
        {
            return Regex.Replace(name, @"\<\<set:(\S+?)\>\>", "");
        }

        /// <summary>
        /// takes a list of sockets that are divided into groups, returns the count of the largest group
        /// </summary>
        /// <param name="sockets"></param>
        /// <returns>returns the longest link</returns>
        private int getLinks(List<RiverUpdate.Socket> sockets)
        {
            int[] groups = new int[6]; //items can max have 6 sockets
            foreach (var s in sockets)
            {
                groups[s.group] += 1;
            }
            //counting time!
            int max = 0;
            for (int i = 0; i < groups.Length; i++)
            {
                max = Math.Max(max, groups[i]);
            }
            return max;
        }

        public int Compare(object x, object y)
        {
            return ((ProcessedItem)x).Id.CompareTo(((ProcessedItem)y).Id); 
        }
    }
}
