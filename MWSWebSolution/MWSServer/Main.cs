using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MWS;
using MWS.DataSource;
using System.IO;
using System.Threading;
using DebugLogHandler;
using MWSUser;
using MarketplaceWebService;
using MarketplaceWebService.Model;
namespace MWSServer
{
    public partial class Main : Form
    {
        SellerInventorySupplyList supplyList;
        string _ConnectionString = "";
        Thread UploadThread;
        bool doWork;
        string sLogPath = Directory.GetCurrentDirectory() + "\\";
        string sClass = "Main.cs";
        MWSUserProfile usMWSProfile;
        public Main()
        {
            InitializeComponent();
            supplyList = new SellerInventorySupplyList();
            string sDBFile = "Database1.mdf";
            string sDBpath = Directory.GetCurrentDirectory() + "\\" + sDBFile;
            //use local db file to debug
            //_ConnectionString = "Data Source=.\\SQLEXPRESS;AttachDbFilename=" + sDBpath + ";Integrated Security=True;User Instance=True;";
            //use production database
            _ConnectionString = "Data Source=192.168.103.150\\INFLOWSQL;Initial Catalog=MWS;User ID=mws;Password=p@ssw0rd";
            String accessKeyId = "AKIAJ2IS6ZEEGQPJV7WA";
            String secretAccessKey = "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";
            const string marketplaceId = "ATVPDKIKX0DER";
            const string sellerId = "A2TYS339AQK0NU";
            string serviceURL = MWSServer.Properties.Settings.Default["MWS_ServiceURL"].ToString();
            usMWSProfile = new MWSUserProfile(accessKeyId, secretAccessKey, marketplaceId, sellerId, serviceURL);

            DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, sClass, "Main() connectionString = " + _ConnectionString);
            ReportRequestHandler getReportHandler = new ReportRequestHandler(usMWSProfile);
            string sReportType = "_GET_MERCHANT_LISTINGS_DATA_";
            //_GET_FBA_FULFILLMENT_INVENTORY_HEALTH_DATA_
            //_GET_AFN_INVENTORY_DATA_
            //_GET_FBA_MYI_ALL_INVENTORY_DATA_
            //_GET_FLAT_FILE_OPEN_LISTINGS_DATA_
            //_GET_FBA_MYI_UNSUPPRESSED_INVENTORY_DATA_
            //_GET_MERCHANT_LISTINGS_DATA_
            getReportHandler.GenerateReport(sReportType);
            //UploadThread = new Thread(new ThreadStart(GetUpdatedInventoryData));
        }

        private void Main_Load(object sender, EventArgs e)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                Hide();
            }));
            doWork = true;
            UploadThread.Start();

        }

        private void Main_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                this.Hide();
            }

            else if (FormWindowState.Normal == this.WindowState)
            {
                this.Show();
            }

        }

        private void GetUpdatedInventoryData()
        {
            ReportRequestHandler getReportHandler = new ReportRequestHandler(usMWSProfile);
            string sReportType = "_GET_FBA_MYI_ALL_INVENTORY_DATA_";
            string sPeriod = "_15_MINUTES_";
            getReportHandler.ScheduleReportRequest(sReportType, sPeriod);
            DataTable InventoryTable = new DataTable();
            while (doWork)
            {
                try
                {
                    SqlDataHandler fillDataHandler = new SqlDataHandler(_ConnectionString);
                    InventoryTable = getReportHandler.GetScheduledReport(sReportType);
                    DataSet ds = new DataSet();
                    DataTable ProductAvailabilityTable = new DataTable();
                    ProductAvailabilityTable.Columns.Add("FNSKU", typeof(String));
                    ProductAvailabilityTable.Columns.Add("SellerSKU", typeof(String));
                    ProductAvailabilityTable.Columns.Add("ASIN", typeof(String));
                    ProductAvailabilityTable.Columns.Add("ProductName", typeof(String));
                    ProductAvailabilityTable.Columns.Add("Inbound", typeof(Int32));
                    ProductAvailabilityTable.Columns.Add("Fulfillable", typeof(Int32));
                    ProductAvailabilityTable.Columns.Add("Unfulfillable", typeof(Int32));
                    ProductAvailabilityTable.Columns.Add("Reserved", typeof(Int32));
                    foreach (DataRow row in InventoryTable.Rows)
                    {
                        DataRow ProductRow = ProductAvailabilityTable.NewRow();
                        ProductRow["FNSKU"] = row[1];
                        ProductRow["SellerSKU"] = row[0];
                        ProductRow["ASIN"] = row[2];
                        ProductRow["ProductName"] = row[3];
                        ProductRow["Inbound"] = Int32.Parse(row[16].ToString());
                        ProductRow["Fulfillable"] = Int32.Parse(row[10].ToString());
                        ProductRow["Unfulfillable"] = Int32.Parse(row[11].ToString());
                        ProductRow["Reserved"] = Int32.Parse(row[12].ToString());
                        ProductAvailabilityTable.Rows.Add(ProductRow);
                    }
                    if (ProductAvailabilityTable.Rows.Count > 0)
                    {
                        ds.Tables.Add(ProductAvailabilityTable);
                        fillDataHandler.UpdateData("ProductAvailability", ds);
                    }

                    fillDataHandler = null;
                    ProductAvailabilityTable = null;
                    ds = null;

                }
                catch (Exception ex)
                {
                    DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, sClass, "GetUpdatedInventoryData() exception message:" + ex.Message);
                }
                Thread.Sleep(1000*60*1);
            }
        }

        private void UploadProdcutSupplyList()
        {
            while (doWork)
            {
                try
                {
                    DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, sClass, "UploadProdcutSupplyList() doWork");
                    SqlDataHandler fillDataHandler = new SqlDataHandler(_ConnectionString);
                    DataSet ds = supplyList.GetSellerInventorySupplyList();

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        fillDataHandler.UpdateData("ProductAvailability", ds);
                    }
                    fillDataHandler = null;
                }
                catch (Exception ex)
                {
                    DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, sClass, "UploadProdcutSupplyList() exception message:" + ex.Message);
                }
                Thread.Sleep(1000);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, sClass, "exitToolStripMenuItem_Click() worker thread set to false");
            doWork = false;
            DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, sClass, "exitToolStripMenuItem_Click() Application will exit");
            Application.Exit();
        }
    }
}
