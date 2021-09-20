using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.IO;
using System.ComponentModel;


namespace Searcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BackgroundWorker worker=new BackgroundWorker();
        private CSearcher searcher;

        public MainWindow()
        {
            InitializeComponent();
            this.searcher = new CSearcher(null, null);
            this.searcher.OnFileFound += FileFound;
            this.searcher.OnDirFound += DirFound;
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;
            searcher.ReadHistory();
            worker.DoWork += WorkInBackground;
            worker.RunWorkerCompleted += WorkerCompleted;
           
           
            
        }
        private void FileFound(string path)
        {
            //textBox2.Text = path;
            listbox2.Dispatcher.BeginInvoke((Action)delegate ()
            {
                listbox2.Items.Add(path);
               
            });
        }

        private void DirFound(string[] paths)
        {
            //textBox2.Text = path;
            listbox2.Dispatcher.BeginInvoke((Action)delegate ()
            {
               
                foreach (var path in paths) {
                    listbox2.Items.Add(path);
                   
                }
                    

            });
        }
        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
           
            if (listbox2.Items.Count == 0)
            {
                MessageBox.Show("Not Found !!");
                listbox2.Items.CopyTo(searcher.Histoy, 0);
                return;
            }
            searcher.Histoy = new string[listbox2.Items.Count];
            listbox2.Items.CopyTo(searcher.Histoy, 0);
            searcher.WriteHistory();
            MessageBox.Show("Done!");
        }

         private void WorkInBackground(object sender, DoWorkEventArgs e)
        {
            if (worker.CancellationPending == true)
            {

                e.Cancel = true;

                return;
            }
            searcher.Search();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.searcher.Term = textBox1.Text.ToLower();
            this.searcher.Dir = textBox.Text.ToLower();
            searcher.ReadHistory();
            if (worker.IsBusy)
            {
                worker.CancelAsync();
                MessageBox.Show("Busy");
                return;
            }

            if (searcher.IsHistory() && searcher.Term !="")
            {
                listbox2.Items.Clear();
                foreach (var path in searcher.Histoy)
                    if (path.ToLower().Contains(searcher.Term.ToLower()))
                    {
                        listbox2.Items.Add(path);
                    }
               
                MessageBox.Show("History Found!!");
                return;
            }
            
            listbox2.Items.Clear();
            worker.RunWorkerAsync();
        }
        private void button_Click1(object sender, RoutedEventArgs e)
        {
            if (worker.IsBusy) {
                worker.CancelAsync();
                MessageBox.Show("Busy");
                return;
            }

            this.searcher.Term = textBox1.Text;
            this.searcher.Dir = textBox.Text;
            listbox2.Items.Clear();
            worker.RunWorkerAsync();
        }
        private void button_Click2(object sender, RoutedEventArgs e)
        {
           worker.CancelAsync();
        }

        private void button_ClickDir(object sender, RoutedEventArgs e)
        {
            listbox2.Items.Clear();
            searcher.GetDrives();
        }

       
    }
}
