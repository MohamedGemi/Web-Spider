using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mission_impossible_code_assassin
{
    class page
    {
        #region Attributes
        int numlinks;
        List<Uri> links_within;
        Uri father;
        string HTML;
        #endregion
        #region get info
        //get number of links
        public int get_nlinks()
        {
            return numlinks;
        }
        // get the father node
        public Uri get_father()
        {
            return father;
        }
        // get the HTML of the page
        public string get_page()
        {
            return HTML;
        }
        // get the Links within this page
        public List<Uri> get_linkswithin()
        {
            return links_within;
        }
        #endregion
        #region constructors
        public page(string pagegiven, List<Uri> linksgiven, Uri fathergiven)
        {
            HTML = pagegiven;
            links_within = linksgiven;
            father = fathergiven;
            numlinks = linksgiven.Count;
        }
        public page()
        {
            numlinks = 0;
            links_within = new List<Uri>();
            father = null;
            HTML = string.Empty;
        }
        #endregion
    }
}
