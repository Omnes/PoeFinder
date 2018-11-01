using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Collections;

namespace PoeItemFinderServer
{
    public class ProcessedStash : IComparer
    {
        public string AccountName { get; set; }
        [Key]
        public string Id { get; set; }
        public string StashName { get; set; }
        public string StashType { get; set; }
        public virtual List<ProcessedItem> Items { get; set; }
        public bool IsPublic { get; set; }

        public ProcessedStash(RiverUpdate.Stash stash)
        {
            AccountName = stash.accountName;
            Id = stash.id;
            StashName = stash.stashName;
            StashType = stash.stashType;

            var itemList = new List<ProcessedItem>();
            foreach(var item in stash.items)
            {
                itemList.Add(new ProcessedItem(stash,item));
            }
            Items = itemList;
            IsPublic = stash.isPublic;
        }

        public ProcessedStash() { }

        public int Compare(object x, object y)
        {
            return ((ProcessedStash)x).Id.CompareTo(((ProcessedStash)y).Id);
        }
    }

    public class StashContext: DbContext
    {
        public DbSet<ProcessedItem> Items { get; set; }
        public DbSet<ProcessedStash> Stashes { get; set; }
    }
}
