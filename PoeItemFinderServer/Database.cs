using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Windows;

namespace PoeItemFinderServer
{
    class Database
    {
        private enum FrameType
        {
            Normal = 0,
            Magic,
            Rare,
            Unique,
            Gem,
            Currency,
            Divination
        }


        public Database() 
        {
        }

        public void ProcessRiverUpdate(RiverUpdate update)
        {
            try {
                using (var ctx = new StashContext())
                {
                    foreach (var stash in update.stashes)
                    {
                        var newStash = new ProcessedStash(stash);
                        var stashQuery = from s in ctx.Stashes where s.Id == newStash.Id select s;
                        var oldStash = stashQuery.FirstOrDefault();
                        if (oldStash != null)
                        {
                            //check which items are new
                            var newItems = newStash.Items.Except(oldStash.Items);
                            foreach (var item in newItems)
                            {
                                var itemQuery = from i in ctx.Items
                                                where ( //isMatchingItem
                                                    (i.FrameType == 6 && item.FrameType == 6 && i.TypeLine == item.TypeLine) || 
                                                    (i.FrameType != 6 && item.FrameType != 6 && i.Name == item.Name)
                                                )&&( //isMatchingLinks
                                                    (i.Links < 4 && item.Links < 4) ||
                                                    (i.Links >= 4 && i.Links == item.Links)
                                                )
                                                select i.ChaosPrice;
                                float minProfitMargin = 0.2f;
                                float minChaosProfit = 1f;
                                float averagePrice = itemQuery.Average();
                                if (item.ChaosPrice < averagePrice * 1.0 - minProfitMargin && item.ChaosPrice < averagePrice - minChaosProfit)
                                {
                                    //this item is potential deal!
                                    Application.Current.Dispatcher.Invoke(() =>
                ((MainWindow)Application.Current.MainWindow)).WriteToConsole(item.Name + " " + item.TypeLine + " Links:" + item.Links + " for only " + item.ChaosPrice + "c");
                                }
                            }

                            //check which items are no longer there
                            var removedItems = oldStash.Items.Except(newStash.Items);
                            foreach (var item in removedItems)
                            {
                                //ctx.SoldItems.Add(item);
                            }

                            //update the stash
                            ctx.Entry(oldStash).CurrentValues.SetValues(newStash);
                            ctx.Entry(oldStash).State = EntityState.Modified;

                        }
                        else
                        {
                            //its new
                            ctx.Stashes.Add(newStash);
                        }
                    }
                    ctx.SaveChanges();
                }
                Application.Current.Dispatcher.Invoke(() =>
                ((MainWindow)Application.Current.MainWindow)).WriteToConsole("Update done: " + update.next_change_id);

            } catch(Exception ex)
            {

            }
        }

        private bool isMatchingItem(ProcessedItem i1, ProcessedItem i2)
        {
            if (i1.FrameType != i2.FrameType) return false;

            if (i1.FrameType == (int)FrameType.Divination)
            {
                return i1.TypeLine == i2.TypeLine;
            }

            return i1.Name == i2.Name;
        }

        private bool isMatchingLinks(ProcessedItem i1, ProcessedItem i2)
        {
            if (i1.Links < 4 && i2.Links < 4)
            {
                return true;
            }
            return i1.Links == i2.Links;
        }

    }
}
