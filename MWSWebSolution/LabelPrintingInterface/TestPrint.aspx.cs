using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports;
using CrystalDecisions.CrystalReports.Engine;
using System.Data;
using DebugLogHandler;
using System.Drawing.Printing;
using System.Diagnostics;

namespace LabelPrintingInterface
{
    public partial class TestPrint : System.Web.UI.Page
    {
        private ArrayList ProductValues;
        private ReportDocument ProductObjectsReport;
        private void GenerateReport()
        {
            PrintDocument pt = new PrintDocument();

            
            ProductValues = new ArrayList();
            string sImgPath = HttpContext.Current.Server.MapPath("~/Images/");
            //ProductLabel product = new ProductLabel("X000E1RC8J", sImgPath + "X000E1RC8J" + ".jpg", "KM * X000E1RC8J");
            //ProductLabel product2 = new ProductLabel("X000EPIRPH", sImgPath + "X000EPIRPH" + ".jpg", "KM * X000EPIRPH");
            LabelReportRow row = new LabelReportRow("X000E1RC8J", sImgPath + "X000E1RC8J" + ".jpg", "KM * X000E1RC8J", "X000EPIRPH", sImgPath + "X000EPIRPH" + ".jpg", "KM * X000EPIRPH");
            LabelReportRow row2 = new LabelReportRow("X000E1RC8J", sImgPath + "X000E1RC8J" + ".jpg", "KM * X000E1RC8J");
            //ProductValues.Add(product);
            //ProductValues.Add(product2);
            ProductValues.Add(row);
            ProductValues.Add(row2);
            string sCrystalReportPath = HttpContext.Current.Server.MapPath("~/LabelReport2.rpt");
            ProductObjectsReport = new ReportDocument();
            ProductObjectsReport.Load(sCrystalReportPath);
            ProductObjectsReport.SetDataSource(ProductValues);
            string sPrinterName = Properties.Settings.Default["PrinterName"].ToString();
            ProductObjectsReport.PrintOptions.PrinterName = sPrinterName; //read printer name from config
            //DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, "LabelReportHandler", "PrintLabel()" + "Config file printer name is " + sPrinterName);
            string sDefaultPrinter = pt.PrinterSettings.PrinterName;
            this.CrystalReportViewer1.ReportSource = ProductObjectsReport;
            //ProductObjectsReport.PrintToPrinter(1, false, 0, 0);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Process printProcess = new Process();
                printProcess.StartInfo.UseShellExecute = false;
                // You can start any process, HelloWorld is a do-nothing example.
                printProcess.StartInfo.FileName = "C:\\Windows\\system32\\notepad.exe";
                printProcess.StartInfo.CreateNoWindow = true;
                printProcess.Start();
                //GenerateReport();
                //string sImgPath = HttpContext.Current.Server.MapPath("~/Images/");
                //DataSet tmpDataSet = new DataSet();
                //DataTable tmpTable = new DataTable();
                //tmpTable.Columns.Add("LeftFNSKU", typeof(String));
                //tmpTable.Columns.Add("RightFNSKU", typeof(String));
                //tmpTable.Columns.Add("LeftLabel", typeof(String));
                //tmpTable.Columns.Add("RightLabel", typeof(String));

                //DataRow newRow = tmpTable.NewRow();
                //newRow[0] = "X000E1RC8J";
                //newRow[1] = "X000EPIRPH";
                //newRow[2] = sImgPath + "X000E1RC8J" + ".jpg";
                //newRow[3] = sImgPath + "X000EPIRPH" + ".jpg";
                //tmpTable.Rows.Add(newRow);
                //newRow = tmpTable.NewRow();
                //newRow[1] = "X000E1RC8J";
                //newRow[0] = "X000EPIRPH";
                //newRow[3] = sImgPath + "X000E1RC8J" + ".jpg";
                //newRow[2] = sImgPath + "X000EPIRPH" + ".jpg";
                //tmpTable.Rows.Add(newRow);
                //tmpDataSet.Tables.Add(tmpTable);
                //string sCrystalReportPath = HttpContext.Current.Server.MapPath("~/LabelReport2.rpt");

                //ReportDocument rpt = new ReportDocument();
                //DebugLogHandler.DebugLogHandler.WriteLog(HttpContext.Current.Server.MapPath("~/"), "TestPrint", "Page_Load->Crystal report print testing started");
                //rpt.Load(sCrystalReportPath);
                //rpt.SetDataSource(tmpDataSet.Tables[0]);
            }
            catch (Exception ex)
            {
                DebugLogHandler.DebugLogHandler.WriteLog(HttpContext.Current.Server.MapPath("~/"), "TestPrint", ex.Message);
            }
        }
    }
}