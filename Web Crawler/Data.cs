using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace mission_impossible_code_assassin
{
    class Data
    {
        #region containers
        public static Queue<Uri> Links_Queue = new Queue<Uri>();
        public static HashSet<Uri> Done_Links = new HashSet<Uri>();
        public static Queue<Uri> Download_Queue = new Queue<Uri>();
        public static HashSet<Uri> Downloaded_Links = new HashSet<Uri>();
        public static HashSet<page> pages = new HashSet<page>();
        public static List<Uri> small_links = new List<Uri>();
        public static string Error;
        public static string parent_path;
        #endregion
        #region counters
        public static int crawled_links = 0;
        public static int downloaded_links = 0;
        public static int Links_found = 0;
        public static Uri Working;
        public static int user_defThreads = 0;
        public static int fnam = 0;
        public static string available_ram;
        public static string cpuUsage;
        public static AutoResetEvent _AREvt = new AutoResetEvent(true);
        #endregion
        #region threads
        public static Thread[] threads_parse = new Thread[10];
        public static Thread Download_Thread;
        public static Thread get_file;
        public static Thread webcrawl_thread;
        public static Thread performance_thread;
        public static Thread loop_thread;
        public static Thread update_thread;
        public static Thread CpuRamStatus;
        #endregion
        #region flags
        public static bool start_flag = false;
        public static int nThreads =0;
        public static bool downloading = false;
        public static bool is_there_internet = false;
        public static bool threadsrunning = false;
        #endregion
    }
}
