using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BoredomTracker
{
    /// <summary>
    /// Interaction logic for InfoPanel.xaml
    /// </summary>
    public partial class InfoPanel : Window
    {
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        DateTime boredomStarted;
        TimeSpan currentBoredom = new TimeSpan(0, 0, 0);
        TimeSpan totalBoredom = new TimeSpan(0, 0, 0);
        TimeSpan newTotalBoredom = new TimeSpan(0, 0, 0);
        public bool isBored = false;
        float salary = 13;
        public InfoPanel()
        {
            InitializeComponent();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            LoadData();
        }
        private void LoadData()
        {
            if (File.Exists("boredom.txt"))
            {
                string[] lines = File.ReadAllLines(@"boredom.txt");
                foreach (string line in lines)
                {
                    JObject jsonObject = JObject.Parse(line);
                    totalBoredom = TimeSpan.Parse((string)jsonObject["TotalBoredom"]);
                    newTotalBoredom = totalBoredom;
                    isBored = (bool)jsonObject["Bored"];
                    if (isBored)
                    {
                        boredomStarted = (DateTime)jsonObject["BoredomStarted"];
                        dispatcherTimer.Start();
                        newTotalBoredom += DateTime.Now.Subtract(boredomStarted);
                    }
                    UpdateData();
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            base.Hide();
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            UpdateData();
        }
        public void UpdateData()
        {
            currentBoredom = DateTime.Now.Subtract(boredomStarted);
            newTotalBoredom += TimeSpan.FromSeconds(1);
            if (isBored)
            {
                txtCurrentBoredom.Text = currentBoredom.ToString(@"dd\.hh\:mm\:ss");
            }
            txtTotalBoredom.Text = newTotalBoredom.ToString(@"dd\.hh\:mm\:ss");

            if (isBored) lblBored.Content = "Currently: BORED";
            else lblBored.Content = "Currently: NOT BORED";

            float money = ((float)newTotalBoredom.Hours * salary) + ((float)newTotalBoredom.Minutes / 60 * salary) + ((float)newTotalBoredom.Seconds / 60 / 60 * salary);
            lblFreeMoney.Content = "€" + money.ToString("0.00");
        }
        public void Start()
        {
            isBored = true;
            boredomStarted = DateTime.Now;
            dispatcherTimer.Start();
        }
        public void Stop()
        {
            totalBoredom += DateTime.Now.Subtract(boredomStarted);
            isBored = false;
            dispatcherTimer.Stop();
            lblBored.Content = "Currently: NOT BORED";
            Save();
        }
        public void Save()
        {
            var path = "boredom.txt";
            var data = new
            {
                TotalBoredom = totalBoredom,
                Bored = isBored,
                BoredomStarted = boredomStarted,
            };
            string jsonData = JsonConvert.SerializeObject(data);
            File.WriteAllText(path, jsonData);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Save();
        }
    }
}
