﻿#pragma checksum "D:\Development\A Project\web_mws\MWSWebSolution\SilverlightPrinting\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "B3C16AC1A0CF9D2CA1570447D620FA00"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18063
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace SilverlightPrinting {
    
    
    public partial class MainPage : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Button Printbutton1;
        
        internal System.Windows.Controls.TextBox PrinterCommandtextBox1;
        
        internal System.Windows.Controls.TextBox PrinterNametextBox2;
        
        internal System.Windows.Controls.Label PrinterNameLabel;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/SilverlightPrinting;component/MainPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.Printbutton1 = ((System.Windows.Controls.Button)(this.FindName("Printbutton1")));
            this.PrinterCommandtextBox1 = ((System.Windows.Controls.TextBox)(this.FindName("PrinterCommandtextBox1")));
            this.PrinterNametextBox2 = ((System.Windows.Controls.TextBox)(this.FindName("PrinterNametextBox2")));
            this.PrinterNameLabel = ((System.Windows.Controls.Label)(this.FindName("PrinterNameLabel")));
        }
    }
}

