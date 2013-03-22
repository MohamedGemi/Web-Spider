using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace mission_impossible_code_assassin
{
    class parser
    {
        #region parser
        public static string crawl_page(Uri link,ref List<Uri> Links)
        {
            try
            {
                string Rstring; // variable where page will be stored
                WebRequest myWebRequest; // web request object which will send request to the server
                WebResponse myWebResponse; // web response
                myWebRequest = WebRequest.Create(link); // request link
                myWebResponse = myWebRequest.GetResponse(); // receive response 
                // reading response which is the html file
                Stream streamResponse = myWebResponse.GetResponseStream();
                StreamReader sreader = new StreamReader(streamResponse);
                Rstring = sreader.ReadToEnd();
                Links = GetContent(Rstring, link);
                lock (Data.small_links)
                {
                    Data.small_links = Links;
                }
                lock (Data.Links_Queue)
                {
                    foreach (Uri linkwithin in Links)
                    {
                        Data.Links_Queue.Enqueue(linkwithin);
                        Data.Links_found++;
                    }
                }
                return Rstring;
            }
            catch (Exception e)
            {
                Data.Error = e.Message;
                return null;
            }
        }

        // Method To Get The Content Of The Pages
        private static List<Uri> GetContent(String Rstring, Uri baseurl)
        {
            //get the page from the string
            List<Uri> mo = new List<Uri>();
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(Rstring);
            HtmlNodeCollection linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");
            HtmlNodeCollection imgnodes = doc.DocumentNode.SelectNodes("//img[@src]");
            // check if null , and stores the found page links or images
            if (linkNodes != null)
            {
                foreach (HtmlNode linkNode in linkNodes)
                {
                    HtmlAttribute att = linkNode.Attributes["href"];
                    Uri newfm = new Uri(baseurl, att.Value);
                    mo.Add(newfm);
                }
            }
            if (imgnodes != null)
            {
                foreach (HtmlNode imno in imgnodes)
                {
                    HtmlAttribute atttwo = imno.Attributes["src"];
                    Uri newfmtwo = new Uri(baseurl, atttwo.Value);
                    mo.Add(newfmtwo);
                }
            }
            return mo;

        }
        #endregion
        #region download
        public static string create_folder(string path, string folder_name)
        {
            string actpath = path; // Take The PAth
            actpath = System.IO.Path.Combine(actpath, folder_name); // Combine The Accepted Path With Folder Name
            System.IO.Directory.CreateDirectory(actpath); // Make The Folder
            return actpath; // Return Current Path
        }
        public static void get_file(Uri link, string path)
        {
            string filename = link.Host.ToString(); // Take The Link host
            filename += Data.fnam.ToString(); // Enum The Files

            string LinkName = link.ToString();

            if (LinkName.Contains(".gif"))
            {
                filename += ".gif";
            }
            else if (LinkName.Contains(".jpg"))
            {
                filename += ".jpg";
            }
            else if (LinkName.Contains(".png"))
            {
                filename += ".png";
            }
            else
            {
                filename += ".html";
            }

            Data.fnam++;

            try
            {

                char[] MyChar = { '@', '\\', '/', '?', ',', '!', ' ', '*', '&', '^', '%' };

                path = create_folder(path, link.Host);

                path = System.IO.Path.Combine(path, filename);
                Data.get_file = new Thread(() => DownloadFile(link.ToString(), path));
                Data.get_file.IsBackground = true;
                Data.get_file.Start();
                Data.downloaded_links++;
            }

            catch (Exception e)
            {
                Data.Error = e.Message;
            }
        }

        // Method To download The Files
        public static int DownloadFile(String remoteFilename, String localFilename)
        {
            // Function will return the number of bytes processed
            // to the caller. Initialize to 0 here.
            int bytesProcessed = 0;

            // Assign values to these objects here so that they can
            // be referenced in the finally block
            Stream remoteStream = null;
            Stream localStream = null;
            WebResponse response = null;

            // Use a try/catch/finally block as both the WebRequest and Stream
            // classes throw exceptions upon error
            try
            {
                // Create a request for the specified remote file name
                WebRequest request = WebRequest.Create(remoteFilename);
                if (request != null)
                {
                    // Send the request to the server and retrieve the
                    // WebResponse object
                    response = request.GetResponse();
                    if (response != null)
                    {
                        // Once the WebResponse object has been retrieved,
                        // get the stream object associated with the response's data
                        remoteStream = response.GetResponseStream();

                        // Create the local file
                        localStream = File.Create(localFilename);

                        // Allocate a 1k buffer
                        byte[] buffer = new byte[1024];
                        int bytesRead;

                        // Simple do/while loop to read from stream until
                        // no bytes are returned

                        do
                        {
                            // Read data (up to 1k) from the stream
                            bytesRead = remoteStream.Read(buffer, 0, buffer.Length);

                            // Write the data to the local file
                            localStream.Write(buffer, 0, bytesRead);

                            // Increment total bytes processed
                            bytesProcessed += bytesRead;
                        } while (bytesRead > 0);
                    }
                }
            }

            catch (Exception e)
            {
                Data.Error = e.Message;
                Data.Download_Thread.Abort();
                Data.downloading = false;
            }

            finally
            {
                // Close the response and streams objects here
                // to make sure they're closed even if an exception
                // is thrown at some point
                if (response != null) response.Close();
                if (remoteStream != null) remoteStream.Close();
                if (localStream != null) localStream.Close();
            }
            lock (Data.Downloaded_Links)
            {
                Data.Downloaded_Links.Add(Data.Working);
                //Data.downloaded_links++;
            }
            // Return total bytes processed to caller.
            return bytesProcessed;
        }
        #endregion
    }
}
