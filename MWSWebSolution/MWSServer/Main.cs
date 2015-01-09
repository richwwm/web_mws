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
using System.Data.SqlTypes;
using System.Globalization;
namespace MWSServer
{
    public partial class Main : Form
    {
        SellerInventorySupplyList supplyList;
        string _ConnectionString = "";

        bool doWork;
        string sLogPath = Directory.GetCurrentDirectory() + "\\";
        string sClass = "Main.cs";
        List<bool> threadParameterList;
        List<MWSUserProfile> profileList;

        int UPDATE_CURRENCYRATE_PERIOD = 1; //unit (min)
        int UPDATE_ESTIMATED_FBA_FEE_PERIOD = 15; //unit (min)
        int UPDATE_INVENTORY_COST_PERIOD = 1; //unit (min)
        int UPDATE_FULFILLED_SHIPMENTS_DATA_PERIOD = 15; //unit (min)
        DateTime ESTIMATED_FBA_FEE_REPORT_ENDDATE = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day - 1, 23, 59, 59);
        DateTime ESTIMATED_FBA_FEE_REPORT_STARTDATE = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day - 1, 0, 0, 0);
        DateTime FULFILLED_SHIPMENTS_DATA_STARTDATE = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day - 1, 0, 0, 0);
        DateTime FULFILLED_SHIPMENTS_DATA_ENDDATE = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day - 1, 23, 59, 59);
        DateTime SETTLEMENT_PAYMENT_DATA_ENDDATE = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day , 23, 59, 59);
        
        public Main()
        {
            InitializeComponent();
            profileList = new List<MWSUserProfile>();
            threadParameterList = new List<bool>();
            supplyList = new SellerInventorySupplyList();
            string sDBFile = "Database1.mdf";
            string sDBpath = Directory.GetCurrentDirectory() + "\\" + sDBFile;
            //use local db file to debug
            //_ConnectionString = "Data Source=.\\SQLEXPRESS;AttachDbFilename=" + sDBpath + ";Integrated Security=True;User Instance=True;";
            //use production database
            _ConnectionString = "Data Source=192.168.103.150\\INFLOWSQL;Initial Catalog=MWS;User ID=mws;Password=p@ssw0rd";

            SqlDataHandler profileHandler = new SqlDataHandler(_ConnectionString);
            profileList = profileHandler.GetAWSLoginProfile();
            bool doWork = true;
            Thread workerThread = new Thread(new ParameterizedThreadStart(DoThreadRoutine));
            threadParameterList.Add(doWork);
            workerThread.Start(doWork);

            //MWSUserProfile profile = profileList[0];
            //profile.SServiceURL = MWSServer.Properties.Settings.Default["MWS_ServiceURL"].ToString();
            //Thread getOrderDataThread;
            //getOrderDataThread = new Thread(new ParameterizedThreadStart(GetUpdatedOrderData));
            //workerThreadParameter threadPara2 = new workerThreadParameter(profile);
            //threadParameterList.Add(threadPara2);
            //getOrderDataThread.Start(threadPara2);

            //MWSUserProfile profile = profileList[0];
            //profile.SServiceURL = MWSServer.Properties.Settings.Default["MWS_ServiceURL"].ToString();
            //Thread getFBAEstimatedFeeThread;
            //getFBAEstimatedFeeThread = new Thread(new ParameterizedThreadStart(GetFBA_EstimatedFeeData));
            //workerThreadParameter threadPara3 = new workerThreadParameter(profile);
            //threadParameterList.Add(threadPara3);
            //getFBAEstimatedFeeThread.Start(threadPara3);

            //MWSUserProfile profile = profileList[0];
            //Thread getCurrencyRateThread;
            //getCurrencyRateThread = new Thread(new ParameterizedThreadStart(GetCurrencyExchangeRate));
            //workerThreadParameter threadPara4 = new workerThreadParameter(profile);
            //threadParameterList.Add(threadPara4);
            //getCurrencyRateThread.Start(threadPara4);

            //MWSUserProfile profile = profileList[0];
            //Thread getInventoryCostThread;
            //getInventoryCostThread = new Thread(new ParameterizedThreadStart(GetInventoryCost));
            //workerThreadParameter threadPara5 = new workerThreadParameter(profile);
            //threadParameterList.Add(threadPara5);
            //getInventoryCostThread.Start(threadPara5);

            //MWSUserProfile profile = profileList[0];
            //profile.SServiceURL = MWSServer.Properties.Settings.Default["MWS_ServiceURL"].ToString();
            //Thread getFulfilledShipmentsDataThread;
            //getFulfilledShipmentsDataThread = new Thread(new ParameterizedThreadStart(GetUpdatedFulfilledShipmentsData));
            //workerThreadParameter threadPara6 = new workerThreadParameter(profile);
            //threadParameterList.Add(threadPara6);
            //getFulfilledShipmentsDataThread.Start(threadPara6);

            //MWSUserProfile profile = profileList[0];
            //profile.SServiceURL = MWSServer.Properties.Settings.Default["MWS_ServiceURL"].ToString();
            //Thread updateNetProfitDataThread;
            //updateNetProfitDataThread = new Thread(new ParameterizedThreadStart(UpdateNetProfitData));
            //workerThreadParameter threadPara7 = new workerThreadParameter(profile);
            //threadParameterList.Add(threadPara7);
            //updateNetProfitDataThread.Start(threadPara7);

        }

        private void Main_Load(object sender, EventArgs e)
        {
            BeginInvoke(new MethodInvoker(delegate
            {
                Hide();
            }));
            doWork = true;

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

        private void DoThreadRoutine(object threadpara)
        {
            bool doWork = (bool)threadpara;
            SqlDataHandler profileHandler = new SqlDataHandler(_ConnectionString);
            profileList = profileHandler.GetAWSLoginProfile();
            int iInventoryCostHistoryRowsAffected = 0;
            DateTime finishedRoutineDateTime = new DateTime();
            while (threadParameterList[0])
            {
                GetCurrencyExchangeRate();

                if (finishedRoutineDateTime.Date != DateTime.Now.Date)
                    iInventoryCostHistoryRowsAffected = GetInventoryCost();
                foreach (MWSUserProfile profile in profileList)
                {
                    profile.SServiceURL = MWSServer.Properties.Settings.Default["MWS_ServiceURL"].ToString();
                    if (DateTime.Now.Hour > 0 && DateTime.Now.Hour < 3)
                        GetTransactionData(profile);
                    else
                        GetUpdatedInventoryData(profile);
                    Thread.Sleep(1000 * 5); //sleep 5 secound
                }

                finishedRoutineDateTime = DateTime.Now;

                Thread.Sleep(1000 * 60 * 5); //sleep 5 min
            }
        }

        private int GetInventoryCost()
        {
            SqlDataHandler inventoryCostHandler = new SqlDataHandler(_ConnectionString);
            int iRowsAffected = inventoryCostHandler.UpdateInventoryCostHistory();

            if (iRowsAffected > 0)
                DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, sClass, "GetInventoryCost() rows affected:" + iRowsAffected);

            Thread.Sleep(1000 * 60 * UPDATE_INVENTORY_COST_PERIOD); //sleep XXX min
            return iRowsAffected;
        }

        private void UpdateNetProfitData(object threadpara)
        {
            workerThreadParameter para = (workerThreadParameter)threadpara;
            int iRowsAffected;
            while (para.doWork)
            {
                DateTime cutoffTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0);
                //DateTime startDate = new DateTime(DateTime.UtcNow.Year, 12, 2);
                //DateTime endDate = startDate.AddDays(1);
                DateTime startDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0);
                DateTime endDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 23, 59, 59);
                SqlDataHandler UpdateNetProfitDataHandler = new SqlDataHandler(_ConnectionString);
                iRowsAffected = UpdateNetProfitDataHandler.UpdateNetProfitData(startDate, endDate);
                DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, sClass, "UpdateNetProfitData() rows affected:" + iRowsAffected);
                Thread.Sleep(1000 * 60 * UPDATE_CURRENCYRATE_PERIOD); //sleep 1 min
            }
        }

        private void GetCurrencyExchangeRate()
        {
            if (DateTime.UtcNow.Hour == 0)
            {
                YahooCurrenyRateHandler currencyHandler = new YahooCurrenyRateHandler();
                List<CurrencyRateQuote> listAllCurrencyQuote = currencyHandler.getAllCurrencyExchangeRate();
                if (listAllCurrencyQuote.Count > 0)
                {
                    DataTable tableCurrencyExchangeRateHistory = new DataTable();
                    tableCurrencyExchangeRateHistory.Columns.Add("FromCurrencyID", typeof(String));
                    tableCurrencyExchangeRateHistory.Columns.Add("ToCurrencyID", typeof(String));
                    tableCurrencyExchangeRateHistory.Columns.Add("Rate", typeof(float));
                    tableCurrencyExchangeRateHistory.Columns.Add("Date", typeof(DateTime));
                    SqlDataHandler fillDataHandler = new SqlDataHandler(_ConnectionString);
                    DataSet ds = new DataSet();

                    foreach (CurrencyRateQuote quote in listAllCurrencyQuote)
                    {
                        DataRow rowData = tableCurrencyExchangeRateHistory.NewRow();
                        string[] stemp = quote.SExchangeType.Split('/');
                        string sFromCurrencySymbol = stemp[0];
                        string sToCurrencySymbol = stemp[1];
                        string sFromCurrencyID = fillDataHandler.GetCurrencyID(sFromCurrencySymbol);
                        string sToCurrencyID = fillDataHandler.GetCurrencyID(sToCurrencySymbol);
                        if (string.IsNullOrEmpty(sFromCurrencyID)
                            || string.IsNullOrEmpty(sToCurrencyID)
                            || sToCurrencyID == sFromCurrencyID)
                        {
                            continue; //skip this currency 
                        }
                        else
                        {
                            rowData[0] = sFromCurrencyID;
                            rowData[1] = sToCurrencyID;
                            rowData[2] = (float)quote.FRate;

                            rowData[3] = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0);
                        }
                        tableCurrencyExchangeRateHistory.Rows.Add(rowData);
                    }

                    if (tableCurrencyExchangeRateHistory.Rows.Count > 0)
                    {
                        ds.Tables.Add(tableCurrencyExchangeRateHistory);
                        fillDataHandler.UpdateCurrencyExchangeRateHistory("CurrencyExchangeRateHistory", ds);
                    }
                    tableCurrencyExchangeRateHistory = null;
                    ds = null;
                    fillDataHandler = null;

                }
            }
        }


        private void GetTransactionData(MWSUserProfile profile)
        {
            string sReportType = "_GET_ALT_FLAT_FILE_PAYMENT_SETTLEMENT_DATA_";
            string sMechantID = profile.SSellerId;
            List<DataTable> srcTableList = null;
            ReportRequestHandler getReportHandler = new ReportRequestHandler(profile);
            DateTime reportEndDate = SETTLEMENT_PAYMENT_DATA_ENDDATE;
            DateTime reportStartDate = SETTLEMENT_PAYMENT_DATA_ENDDATE.AddDays(-89);

            try
            {
                srcTableList = getReportHandler.GetScheduledReport(sReportType, reportStartDate, reportEndDate);
                SqlDataHandler fillDataHandler = new SqlDataHandler(_ConnectionString);
                if (srcTableList.Count > 0)
                {
                    DateTime defaultDateTime = new DateTime(1900, 1, 1);
                    foreach (DataTable srcTable in srcTableList)
                    {
                        if (null != srcTable && srcTable.Rows.Count > 0)
                        {
                            DataTable tableWithMerchantID = new DataTable();
                            foreach (DataColumn col in srcTable.Columns)
                            {
                                if (col.ColumnName.Contains("date"))
                                    tableWithMerchantID.Columns.Add(col.ColumnName, typeof(DateTime));
                                else if (col.ColumnName == "total_amount" || col.ColumnName == "amount")
                                    tableWithMerchantID.Columns.Add(col.ColumnName, typeof(float));
                                else if (col.ColumnName == "quantity_purchased")
                                    tableWithMerchantID.Columns.Add(col.ColumnName, typeof(Int32));
                                else
                                    tableWithMerchantID.Columns.Add(col.ColumnName, typeof(String));
                            }
                            tableWithMerchantID.Columns.Add("MerchantID", typeof(String));
                            tableWithMerchantID.Columns.Add("last_update_date", typeof(DateTime));
                            string sOrderID = "";
                            DateTime dtSettlementStartDate = defaultDateTime;
                            DateTime dtSettlementEndDate = defaultDateTime;
                            DateTime dtDepositDate = defaultDateTime;
                            DateTime dtPostedDate = defaultDateTime;
                            DateTime dtPostedDate_time = defaultDateTime;
                            foreach (DataRow row in srcTable.Rows)
                            {
                                DataRow rowSettlemnetData = tableWithMerchantID.NewRow();

                                sOrderID = row["order_id"].ToString();
                                rowSettlemnetData = tableWithMerchantID.NewRow();
                                foreach (DataColumn col in tableWithMerchantID.Columns)
                                {

                                    if (col.ColumnName == "posted_date_time")
                                    {
                                        if (!string.IsNullOrEmpty(row[col.ColumnName].ToString()))
                                        {
                                            string temp = row[col.ColumnName].ToString();

                                            //2014-11-30 07:01:39
                                            dtPostedDate_time = PSTTimeString2UTCDateTime(temp);
                                        }

                                        rowSettlemnetData[col.ColumnName] = dtPostedDate_time;
                                    }
                                    else
                                        if (col.ColumnName == "settlement_start_date")
                                        {
                                            if (!string.IsNullOrEmpty(row[col.ColumnName].ToString()))
                                            {
                                                string temp = row[col.ColumnName].ToString();

                                                //2014-11-30 07:01:39
                                                dtSettlementStartDate = PSTTimeString2UTCDateTime(temp);
                                            }

                                            rowSettlemnetData[col.ColumnName] = dtSettlementStartDate;
                                        }
                                        else
                                            if (col.ColumnName == "settlement_end_date")
                                            {
                                                if (!string.IsNullOrEmpty(row[col.ColumnName].ToString()))
                                                {
                                                    string temp = row[col.ColumnName].ToString();
                                                    dtSettlementEndDate = PSTTimeString2UTCDateTime(temp);
                                                }

                                                rowSettlemnetData[col.ColumnName] = dtSettlementEndDate;
                                            }
                                            else
                                                if (col.ColumnName == "deposit_date")
                                                {
                                                    if (!string.IsNullOrEmpty(row[col.ColumnName].ToString()))
                                                    {
                                                        string temp = row[col.ColumnName].ToString();


                                                        dtDepositDate = PSTTimeString2UTCDateTime(temp);
                                                    }

                                                    rowSettlemnetData[col.ColumnName] = dtDepositDate;
                                                }
                                                else
                                                    if (col.ColumnName == "MerchantID")
                                                        rowSettlemnetData[col.ColumnName] = sMechantID;
                                                    else
                                                        if (col.ColumnName == "last_update_date")
                                                        {
                                                            rowSettlemnetData[col.ColumnName] = DateTime.UtcNow;
                                                        }
                                                        else
                                                            if (col.ColumnName == "posted_date")
                                                            {
                                                                if (!string.IsNullOrEmpty(row[col.ColumnName].ToString()))
                                                                {
                                                                    string temp = row[col.ColumnName].ToString();

                                                                    //2014-11-30 07:01:39
                                                                    dtPostedDate = PSTTimeString2UTCDateTime(temp);
                                                                }

                                                                rowSettlemnetData[col.ColumnName] = dtPostedDate;
                                                            }
                                                            else
                                                                if (col.ColumnName == "total_amount" || col.ColumnName == "amount" || col.ColumnName == "quantity_purchased")
                                                                {
                                                                    string temp = row[col.ColumnName].ToString();
                                                                    if (string.IsNullOrEmpty(temp))
                                                                        rowSettlemnetData[col.ColumnName] = 0;
                                                                    else
                                                                        rowSettlemnetData[col.ColumnName] = row[col.ColumnName];
                                                                }
                                                                else
                                                                    rowSettlemnetData[col.ColumnName] = row[col.ColumnName];
                                }


                                tableWithMerchantID.Rows.Add(rowSettlemnetData);
                            }
                            DataSet ds = new DataSet();
                            if (tableWithMerchantID.Rows.Count > 0)
                            {
                                ds.Tables.Add(tableWithMerchantID);
                                fillDataHandler.UpdateRawSettlementPaymentData(ds);
                            }
                            ds = null;
                        }
                        else
                        {
                            DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, sClass, "GetTransactionData() no report data.");
                        }
                    }
                }
                fillDataHandler = null;
            }
            catch (Exception ex)
            {
                DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, sClass, "GetTransactionData() exception message:" + ex.Message);
            }

            Thread.Sleep(1000 * 60 * UPDATE_ESTIMATED_FBA_FEE_PERIOD);

        }

        private void GetFBA_EstimatedFeeData(MWSUserProfile profile)
        {
            DataTable srcTable = null;
            ReportRequestHandler getReportHandler = new ReportRequestHandler(profile);
            string sReportType = "";
            sReportType = "_GET_FBA_ESTIMATED_FBA_FEES_TXT_DATA_";
            string sMechantID = profile.SSellerId;

            DateTime reportEndDate = ESTIMATED_FBA_FEE_REPORT_ENDDATE;
            DateTime reportStartDate = ESTIMATED_FBA_FEE_REPORT_STARTDATE;


            try
            {
                SqlDataHandler fillDataHandler = new SqlDataHandler(_ConnectionString);
                srcTable = getReportHandler.GenerateReport(sReportType, reportStartDate, reportEndDate);
                if (null != srcTable)
                {
                    DataTable tableWithMerchantID = new DataTable();
                    foreach (DataColumn col in srcTable.Columns)
                    {
                        tableWithMerchantID.Columns.Add(col.ColumnName, typeof(String));
                    }
                    tableWithMerchantID.Columns.Add("MerchantID", typeof(String));
                    tableWithMerchantID.Columns.Add("last_update_date", typeof(DateTime));
                    foreach (DataRow row in srcTable.Rows)
                    {
                        DataRow rowOrder_Data = tableWithMerchantID.NewRow();
                        foreach (DataColumn col in tableWithMerchantID.Columns)
                        {
                            if (col.ColumnName == "MerchantID")
                                rowOrder_Data[col.ColumnName] = sMechantID;
                            else if (col.ColumnName == "last_update_date")
                            {
                                rowOrder_Data[col.ColumnName] = DateTime.UtcNow;
                            }
                            else
                                rowOrder_Data[col.ColumnName] = row[col.ColumnName];
                        }
                        tableWithMerchantID.Rows.Add(rowOrder_Data);
                    }
                    DataSet ds = new DataSet();
                    if (tableWithMerchantID.Rows.Count > 0)
                    {
                        ds.Tables.Add(tableWithMerchantID);
                        fillDataHandler.UpdateRaw_FBA_EstimatedFee("Raw_FBA_Estimated_Fee", ds);
                    }

                    fillDataHandler = null;
                    srcTable = null;
                    ds = null;
                }
                else
                {
                    DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, sClass, "GetFBA_EstimatedFeeData() no report data.");
                }
            }
            catch (Exception ex)
            {
                DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, sClass, "GetFBA_EstimatedFeeData() exception message:" + ex.Message);
            }
            Thread.Sleep(1000 * 60 * UPDATE_ESTIMATED_FBA_FEE_PERIOD);

        }

        private void GetUpdatedFulfilledShipmentsData(MWSUserProfile profile)
        {
            DataTable fulfilledShipmentsDataTable = null;
            ReportRequestHandler getReportHandler = new ReportRequestHandler(profile);
            string sReportType = "";
            sReportType = "_GET_AMAZON_FULFILLED_SHIPMENTS_DATA_";
            string sMechantID = profile.SSellerId;
            DateTime reportStartDate = FULFILLED_SHIPMENTS_DATA_STARTDATE;
            DateTime reportEndDate = FULFILLED_SHIPMENTS_DATA_ENDDATE;

            try
            {
                SqlDataHandler fillDataHandler = new SqlDataHandler(_ConnectionString);
                fulfilledShipmentsDataTable = getReportHandler.GenerateReport(sReportType, reportStartDate, reportEndDate);
                if (null != fulfilledShipmentsDataTable)
                {
                    DataTable fulfilledShipmentDataTableWithMerchantID = new DataTable();
                    foreach (DataColumn col in fulfilledShipmentsDataTable.Columns)
                    {
                        if (col.ColumnName.Contains("date"))
                            fulfilledShipmentDataTableWithMerchantID.Columns.Add(col.ColumnName, typeof(DateTime));
                        else
                            fulfilledShipmentDataTableWithMerchantID.Columns.Add(col.ColumnName, typeof(String));
                    }
                    fulfilledShipmentDataTableWithMerchantID.Columns.Add("MerchantID", typeof(String));
                    foreach (DataRow row in fulfilledShipmentsDataTable.Rows)
                    {
                        DataRow row_FulfilledShipmentData = fulfilledShipmentDataTableWithMerchantID.NewRow();
                        foreach (DataColumn col in fulfilledShipmentDataTableWithMerchantID.Columns)
                        {
                            if (col.ColumnName == "MerchantID")
                                row_FulfilledShipmentData[col.ColumnName] = sMechantID;
                            else if (col.ColumnName.Contains("date"))
                            {
                                DateTime tmpDateTime = AmazonTimeString2DateTime(row[col.ColumnName].ToString());
                                row_FulfilledShipmentData[col.ColumnName] = tmpDateTime;
                            }
                            else
                                row_FulfilledShipmentData[col.ColumnName] = row[col.ColumnName];
                        }
                        fulfilledShipmentDataTableWithMerchantID.Rows.Add(row_FulfilledShipmentData);
                    }
                    DataSet ds = new DataSet();
                    if (fulfilledShipmentDataTableWithMerchantID.Rows.Count > 0)
                    {
                        ds.Tables.Add(fulfilledShipmentDataTableWithMerchantID);
                        fillDataHandler.UpdateRawFulfilledShipmentsData(ds);
                    }

                    fillDataHandler = null;
                    fulfilledShipmentsDataTable = null;
                    ds = null;
                }
            }
            catch (Exception ex)
            {
                DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, sClass, "GetUpdatedFulfilledShipmentsData() exception message:" + ex.Message);
            }
            Thread.Sleep(1000 * 60 * UPDATE_FULFILLED_SHIPMENTS_DATA_PERIOD);// wait XXX min 

        }


        private void GetUpdatedOrderData(object threadpara)
        {
            DataTable OrderTable = null;
            workerThreadParameter para = (workerThreadParameter)threadpara;
            ReportRequestHandler getReportHandler = new ReportRequestHandler(para._profile);
            string sReportType = "";
            sReportType = "_GET_FLAT_FILE_ALL_ORDERS_DATA_BY_ORDER_DATE_";
            string sMechantID = para._profile.SSellerId;
            DateTime reportStartDate = new DateTime(DateTime.Now.Year, 11, 1);
            DateTime reportEndDate = new DateTime(DateTime.Now.Year, 11, 30);
            while (para.doWork)
            {
                try
                {
                    SqlDataHandler fillDataHandler = new SqlDataHandler(_ConnectionString);
                    OrderTable = getReportHandler.GenerateReport(sReportType, reportStartDate, reportEndDate);
                    DataTable orderTableWithMerchantID = new DataTable();
                    foreach (DataColumn col in OrderTable.Columns)
                    {
                        orderTableWithMerchantID.Columns.Add(col.ColumnName, typeof(String));
                    }
                    orderTableWithMerchantID.Columns.Add("MerchantID", typeof(String));
                    foreach (DataRow row in OrderTable.Rows)
                    {
                        DataRow rowOrder_Data = orderTableWithMerchantID.NewRow();
                        foreach (DataColumn col in orderTableWithMerchantID.Columns)
                        {
                            if (col.ColumnName == "MerchantID")
                                rowOrder_Data[col.ColumnName] = sMechantID;
                            else
                                rowOrder_Data[col.ColumnName] = row[col.ColumnName];
                        }
                        orderTableWithMerchantID.Rows.Add(rowOrder_Data);
                    }
                    DataSet ds = new DataSet();
                    if (orderTableWithMerchantID.Rows.Count > 0)
                    {
                        ds.Tables.Add(orderTableWithMerchantID);
                        fillDataHandler.UpdateRawOrderData("Raw_Order_Data", ds);
                    }

                    fillDataHandler = null;
                    OrderTable = null;
                    ds = null;
                }
                catch (Exception ex)
                {
                    DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, sClass, "GetUpdatedOrderData() exception message:" + ex.Message);
                }
                Thread.Sleep(1000 * 60 * 15);
            }
        }

        private void GetUpdatedInventoryData(MWSUserProfile profile)
        {
            List<DataTable> InventoryTableList = null;
            ReportRequestHandler getReportHandler = new ReportRequestHandler(profile);
            string sReportType = "_GET_FBA_MYI_ALL_INVENTORY_DATA_";
            string sPeriod = "_15_MINUTES_";
            getReportHandler.ScheduleReportRequest(sReportType, sPeriod);
            string sMechantID = profile.SSellerId;
            DateTime startedDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, 0, 0);
            DateTime endDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, 59, 59);
            try
            {

                InventoryTableList = getReportHandler.GetScheduledReport(sReportType, startedDate, endDate);
                foreach (DataTable InventoryTable in InventoryTableList)
                {
                    if (InventoryTable != null & InventoryTable.Rows.Count > 0)
                    {
                        SqlDataHandler fillDataHandler = new SqlDataHandler(_ConnectionString);
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
                        ProductAvailabilityTable.Columns.Add("MerchantID", typeof(String));
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
                            ProductRow["MerchantID"] = sMechantID;
                            ProductAvailabilityTable.Rows.Add(ProductRow);
                        }
                        if (ProductAvailabilityTable.Rows.Count > 0)
                        {
                            ds.Tables.Add(ProductAvailabilityTable);
                            fillDataHandler.UpdateProductAvailabilityData("ProductAvailability", ds,profile.SSellerId);
                        }

                        fillDataHandler = null;
                        ProductAvailabilityTable = null;
                        ds = null;
                    }
                }

            }
            catch (Exception ex)
            {
                DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, sClass, "GetUpdatedInventoryData() exception message:" + ex.Message);
            }
        }

       
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, sClass, "exitToolStripMenuItem_Click() worker thread set to false");
            for (int i = 0; i < threadParameterList.Count; i++)
                threadParameterList[i] = false;
            DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, sClass, "exitToolStripMenuItem_Click() Application will exit");
            Application.Exit();
        }

        private DateTime PSTTimeString2UTCDateTime(string sTimeString)
        {
            DateTime convertedDateTime = new DateTime();
            int iIndexOfPST = sTimeString.IndexOf("PST");
            if (iIndexOfPST > 0)
            {
                sTimeString = sTimeString.Remove(iIndexOfPST);
            }
            sTimeString = sTimeString.Trim();
            convertedDateTime = Convert.ToDateTime(sTimeString);
            convertedDateTime = convertedDateTime.AddHours(8);
            return convertedDateTime;
        }

        private DateTime AmazonTimeString2DateTime(string sTimeString)
        {
            DateTime convertedDateTime = new DateTime();
            //e.g. sTimString = 2014-12-01T04:46:26+00:00
            string[] sTemp = sTimeString.Split('T');
            if (sTemp.Length > 1)
            {
                int year = 1985;
                int month = 1;
                int day = 1;
                int hour = 0;
                int minute = 0;
                int second = 0;
                string[] sDate = sTemp[0].Split('-');
                string[] sTimePart = sTemp[1].Split('+');
                year = Convert.ToInt32(sDate[0]);
                month = Convert.ToInt32(sDate[1]);
                day = Convert.ToInt32(sDate[2]);
                if (sTimePart.Length > 0)
                {
                    string[] sTime = sTimePart[0].Split(':');
                    hour = Convert.ToInt32(sTime[0]);
                    minute = Convert.ToInt32(sTime[1]);
                    second = Convert.ToInt32(sTime[2]);
                }
                convertedDateTime = new DateTime(year, month, day, hour, minute, second);

            }
            return convertedDateTime;
        }

    }

    class workerThreadParameter
    {
        public bool doWork = true;
        public MWSUserProfile _profile;
        public workerThreadParameter(MWSUserProfile profile)
        {
            _profile = profile;
        }
    }


}
