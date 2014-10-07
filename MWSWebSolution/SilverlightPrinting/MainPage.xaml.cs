using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.InteropServices.Automation;

namespace SilverlightPrinting
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Printbutton1_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.HasElevatedPermissions && AutomationFactory.IsAvailable)
            {
                RawPrintHelper.SendStringToPrinter(this.PrinterNametextBox2.Text, this.PrinterCommandtextBox1.Text);

            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //AutomationFactory.CreateObject("excel");
            if (Application.Current.HasElevatedPermissions && AutomationFactory.IsAvailable)
            {
                MessageBox.Show("Ava");
            }
        }
    }
}
