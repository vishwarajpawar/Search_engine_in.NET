using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;

namespace Searcher
{
    delegate void FileFound(string path);
    delegate void DirFound(string[] paths);
    class CSearcher
    {
        public event FileFound OnFileFound;
        public event DirFound OnDirFound;
       
        public string[] Histoy { get; set; }
        private string term;

        public string Term
        {
            get { return term; }
            set { term = value; }


        }

        private string dir;

        public string Dir
        {
            get { return dir; }
            set { dir = value; }
        }


        public CSearcher(string dir, string term)
        {
            this.dir = dir;
            this.term = term;
        }
/*
        private void Scan(string dir)
        {
            try
            {
                string[] files = Directory.GetFiles(dir);
                string[] dirs = Directory.GetDirectories(dir);

                List<string> allfiles = new List<string>();
                allfiles.AddRange(files);
                allfiles.AddRange(dirs);
                foreach (string s in allfiles)
                {
                    string _s = s.ToLower();
                    string _term = this.term.ToLower();

                    if (Directory.Exists(s) && s != "." && s != "..")
                    {
                        
                        Scan(s);
                       
                        continue;
                    }
                    if (_s.Contains(_term))
                    {
                        OnFileFound(s);

                    }
                }
            }

            catch(Exception ex) {
            }
        }
*/

        private void PScan(string dir)
        {
            try
            {
                string[] files = Directory.GetFiles(dir);
                string[] dirs = Directory.GetDirectories(dir);

                List<string> allfiles = new List<string>();
                allfiles.AddRange(files);
                allfiles.AddRange(dirs);
                Parallel.ForEach(allfiles, s=>
                {
                    string _s = s.ToLower();
                    string _term = this.term.ToLower();

                    if (Directory.Exists(s) && s != "." && s != "..")
                    {

                        PScan(s);

                       
                    }
                    if (_s.Contains(_term))
                    {
                        OnFileFound(s);

                    }
                }
                );
            }

            catch (Exception ex)
            {

            }
        }


        public void Search()
        {
            PScan(this.dir);
            //DriveInfo[] TotalDrives = DriveInfo.GetDrives();

            //Parallel.ForEach(TotalDrives, dir => {
               
            //    if(dir.IsReady)
            //        PScan(dir.Name);
            //}); 
        }


        public void GetDrives()
        {

            int c = 0;

            DriveInfo[] TotalDrives = DriveInfo.GetDrives();

            

            StringBuilder sb = new StringBuilder();
            string[] Drives = new string[TotalDrives.Length];
            foreach (DriveInfo drvinfo in TotalDrives)
            {
                //sb.Append("Drive= " + drvinfo.Name + System.Environment.NewLine + " \tType= " + drvinfo.DriveType + System.Environment.NewLine +
                //    "\t\t Status= " + drvinfo.IsReady + System.Environment.NewLine + System.Environment.NewLine);

                sb.Append("Drive= " + drvinfo.Name + " \t\tType= " + drvinfo.DriveType + "\t\t Status= " + drvinfo.IsReady + System.Environment.NewLine);

                Drives[c] = sb.ToString();
                sb.Clear();
                c++;

            }
            OnDirFound(Drives);
        }

        public void WriteHistory()
        {

            string path = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            string filename = @"history.txt";
            if (File.Exists(Path.Combine(path, filename)))
            {
                File.Delete(Path.Combine(path, filename));
            }
            File.WriteAllLines(Path.Combine(path, filename), Histoy);
        }

        public bool IsHistory()
        {
            if (Histoy == null) {
                return false;
            }
            foreach (var file in Histoy)
            {
                //if (file.ToLower().Contains(Path.Combine(dir.ToLower(),term.ToLower())))
                if (file.ToLower().Contains(term.ToLower()))
                {
                    return true;
                }

            }
            return false;
            //string path = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            //string filename = @"histoy.txt";
            //if (File.Exists(Path.Combine(path, filename)))
            //{

            //}
      
        }

        public void ReadHistory()
        {
            string path = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            string filename = @"history.txt";
            if (File.Exists(Path.Combine(path, filename)))
            {
                var lineCount = File.ReadLines(Path.Combine(path, filename));
                Histoy = lineCount.ToArray();
                
            }
            
        }
    }
}
