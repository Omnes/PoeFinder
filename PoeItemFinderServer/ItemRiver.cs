using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Windows;

//TODO rensa upp och skriv om allt som har med next change id att göra

namespace PoeItemFinderServer
{
    class ItemRiver
    {
        private const string changeIDPath = "nextChangeID.txt";
        public string nextChangeId = "";

        private Queue<RiverUpdate> updateQueue = new Queue<RiverUpdate>();
        private object updateQueueLock = new object();
        //private readonly int maxUpdateQueueLenght = 5;

        //private EventHandler onUpdateReady;
        private Database database;

        public ItemRiver()
        {
            try
            {
                StreamReader objReader = new StreamReader(changeIDPath);
                nextChangeId = objReader.ReadLine();
                objReader.Close();
            }
            catch (FileNotFoundException ex)
            {
                //Application.Current.Dispatcher.Invoke(() =>
                //((MainWindow)Application.Current.MainWindow).WriteToConsole(ex.Message));

                //TODO it would be a good idea to create the missing file here
                nextChangeId = "";
            }
            database = new Database();
        }

        public bool isUpdateAvailaible()
        {
            lock (updateQueueLock)
            {
                return updateQueue.Count>0;
            }
        }

        /// <summary>
        /// returns a queued update, if none exist pull one and return that
        /// </summary>
        /// <returns></returns>
        public async Task<RiverUpdate> getUpdate()
        {
            //another lock is needed here to prevent 2 treads trying to get the same update
            if (!isUpdateAvailaible())
            {
                
                await buzyWaitUntilUpdateIsReady();
            }

            lock (updateQueueLock)
            {
                var update = updateQueue.Dequeue();
                try
                {
                    //we only want to update the file with the change ids that are consumed (to prevent that updates gets lost on server restarts)
                    File.WriteAllText(changeIDPath, update.next_change_id);
                }
                catch(Exception e)
                {
                    //TODO do stuff here
                }
                return update;
            }
        }

        /// <summary>
        /// retrives a update from the api
        /// </summary>
        private async Task pullUpdate()
        {

            string url = "";
            if (nextChangeId == "")
            {
                url = "http://www.pathofexile.com/api/public-stash-tabs";
            }
            else
            {
                url = "http://www.pathofexile.com/api/public-stash-tabs?id=" + nextChangeId + "";
            }

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            WebResponse webResponse = webRequest.GetResponse();
            var responseStream = webResponse.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream);

            string response = streamReader.ReadToEnd();
            webResponse.Close();
            streamReader.Close();

            lock (updateQueueLock)
            {
                var update = new RiverUpdate();
                try {
                    update = RiverUpdate.CreateNew(response);
                }catch(Newtonsoft.Json.JsonException e)
                {
                    //TODO! this needs to be handled
                    throw e;
                }
                setNextChangeId(update.next_change_id);
                //updateQueue.Enqueue(update);
                database.ProcessRiverUpdate(update);
            }

        }
        /// <summary>
        /// continiously fetches updates from the api
        /// TODO: limit how many it can queue up
        /// </summary>
        /// <returns></returns>
        public async Task pullTask()
        {
            while (true)
            {
                await pullUpdate();
                Application.Current.Dispatcher.Invoke(() =>
                ((MainWindow)Application.Current.MainWindow).WriteToConsole("Pulled!"));
            }
        }

        //TODO find a better solution
        public async Task buzyWaitUntilUpdateIsReady()
        {
            bool wait = true;
            while (wait)
            {
                wait = !isUpdateAvailaible();
                int pollInterval = 200;
                await Task.Delay(pollInterval);
            }
        }

        public void setNextChangeId(string id)
        {
            nextChangeId = id;
        }
    }
}


