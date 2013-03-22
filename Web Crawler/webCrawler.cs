using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace mission_impossible_code_assassin
{
    class webCrawler
    {
        #region start,pause,stop
        //function to assgin threadpool function
        public static void start()
        {
            while (Data.nThreads < Data.Links_Queue.Count)
            {
                if (Data.nThreads < Data.user_defThreads)
                    if (Data.threads_parse[Data.nThreads] == null || Data.threads_parse[Data.nThreads].ThreadState != System.Threading.ThreadState.Suspended && Data.nThreads < 10)
                    {
                        try
                        {
                            Data.threads_parse[Data.nThreads] = new System.Threading.Thread(new System.Threading.ThreadStart(run_parser));
                            Data.threads_parse[Data.nThreads].IsBackground = true;
                            Data.threads_parse[Data.nThreads].Priority = System.Threading.ThreadPriority.Lowest;
                            Data.threads_parse[Data.nThreads].Start();
                            Data.nThreads++;
                            Data._AREvt.WaitOne(10, true);
                        }
                        catch (Exception e)
                        {
                            Data.Error = e.Message;
                        }
                    }
            }
        }
        public static void StopParsing()
        {
            Data.start_flag = false;
            try
            {
                for (int i = 0; i < Data.nThreads; i++)
                {
                    // Check if There is A Paused Threads
                    if (Data.threads_parse[i].ThreadState == System.Threading.ThreadState.Suspended)
                        Data.threads_parse[i].Resume();
                    Data.threads_parse[i].Abort();
                }
            }
            catch (Exception)
            {
            }

            for (int j = 0; j < Data.nThreads; j++)
            {
                try
                {
                    Thread thread = Data.threads_parse[j];
                    if (thread != null && thread.IsAlive)
                    {
                        // Check if There is A Paused Threads
                        if (thread.ThreadState == System.Threading.ThreadState.Suspended)
                            thread.Resume();
                        thread.Abort();
                    }
                }
                catch (Exception)
                {
                }
            }
                Data.Links_Queue.Clear();
                Data.Done_Links.Clear();
                Data.Downloaded_Links.Clear();
                Data.Download_Queue.Clear();
                Data.nThreads = 0;
                Data.crawled_links = 0;
                Data.Links_found = 0;
            
        }
        public static void PauseParsing()
        {
            Data.threadsrunning = false;
            if (Data.threads_parse[0] != null)
                if (Data.threads_parse[0].ThreadState == System.Threading.ThreadState.Running)
                    Data.threads_parse[0].Suspend();
            if (Data.Download_Thread != null)
                if (Data.Download_Thread.ThreadState == System.Threading.ThreadState.Running)
                    Data.Download_Thread.Suspend();

            for (int j = 1; j < Data.nThreads; j++)
            {
                if (Data.threads_parse[j] != null)
                    if (Data.threads_parse[j].ThreadState == System.Threading.ThreadState.Running)
                        Data.threads_parse[j].Suspend();
            }


        }
        public static void ContinueParsing()
        {
            if (Data.threadsrunning == null)
                return;
            if (Data.Download_Thread != null)
                if (Data.Download_Thread.ThreadState == System.Threading.ThreadState.Suspended)
                    Data.Download_Thread.Resume();

            for (int i = 0; i < Data.nThreads; i++)
            {
                if (Data.threads_parse[i] != null)
                    if (Data.threads_parse[i].ThreadState == System.Threading.ThreadState.Suspended)
                        Data.threads_parse[i].Resume();
            }

        }
        #endregion
        #region run methods
        public static void run_parser()
        {
            Data.threadsrunning = true;
            try
            {
                Uri link;
                lock (Data.Links_Queue)
                {
                    link = Data.Links_Queue.Dequeue();
                }
                if (link != null) 
                if (!Data.Done_Links.Contains(link))
                {
                    lock (Data.Done_Links)
                    {
                        Data.Done_Links.Add(link);
                    }
                    lock (Data.Download_Queue)
                    {
                        Data.Download_Queue.Enqueue(link);
                    }
                    List<Uri> page_links=new List<Uri>();
                    string pagehtml = parser.crawl_page(link, ref page_links);
                    page newpage = new page(pagehtml, page_links, link);
                    Data.pages.Add(newpage);
                    Data.crawled_links++;
                }
            }
            catch (Exception e)
            {
                Data.Error = e.Message;
            }
        }
        public static void start_download()
        {
            try
            {
                Uri link= null;
                lock (Data.Download_Queue)
                {
                    if (Data.Download_Queue.Count>0)
                        link = Data.Download_Queue.Dequeue();
                }
                if (link != null && !Data.downloading)
                {
                    Data.Working = link;
                    Data.Download_Thread = new Thread(()=>parser.get_file(link, Data.parent_path));
                    Data.Download_Thread.IsBackground = true;
                    Data.Download_Thread.Priority = ThreadPriority.Lowest;
                    Data.Download_Thread.Start();
                    Data.downloading = true;
                }
            }
            catch (Exception e)
            {
                Data.Error = e.Message;
            }
        }
        #endregion
    }
}
