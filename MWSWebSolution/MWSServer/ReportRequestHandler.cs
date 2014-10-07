using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MarketplaceWebService;
using MarketplaceWebService.Model;
using MWSUser;
using System.IO;
using System.Threading;
using DebugLogHandler;
using System.Data;

namespace MWSServer
{
    class ReportRequestHandler
    {
        string sLogPath = Directory.GetCurrentDirectory() + "\\";
        string sClass = "Main.cs";
        private string _sClass = "ReportRequestHandler.cs";
        private string _sLogPath = System.IO.Directory.GetCurrentDirectory() + "\\";
        MarketplaceWebService.MarketplaceWebService service;
        MWSUserProfile _currentProfile;
        public ReportRequestHandler(MWSUserProfile profile)
        {
            const string applicationName = "<Your Application Name>";
            const string applicationVersion = "<Your Application Version or Build Number or Release Date>";

            /************************************************************************
            * Uncomment to try advanced configuration options. Available options are:
            *
            *  - Proxy Host and Proxy Port
            *  - MWS Service endpoint URL
            *  - User Agent String to be sent to FBA Inventory Service MWS  service
            *
            ***********************************************************************/
            MarketplaceWebServiceConfig config = new MarketplaceWebServiceConfig();
            //config.ProxyHost = "https://PROXY_URL";
            //config.ProxyPort = 9090;
            //
            // IMPORTANT: Uncomment out the appropiate line for the country you wish 
            // to sell in:
            // 
            // US
            config.ServiceURL = profile.SServiceURL;
            //config.ServiceURL = "https://mws.amazonservices.com/FulfillmentInventory/2010-10-01/";
            // UK
            // config.ServiceURL = "https://mws.amazonservices.co.uk/FulfillmentInventory/2010-10-01/";
            // Germany
            // config.ServiceURL = "https://mws.amazonservices.de/FulfillmentInventory/2010-10-01/";
            // France
            // config.ServiceURL = "https://mws.amazonservices.fr/FulfillmentInventory/2010-10-01/";
            // Japan
            // config.ServiceURL = "https://mws.amazonservices.jp/FulfillmentInventory/2010-10-01/";
            // China
            // config.ServiceURL = "https://mws.amazonservices.com.cn/FulfillmentInventory/2010-10-01/";

            // ProxyPort=-1 ; MaxErrorRetry=3
            config.SetUserAgentHeader(
                applicationName,
                applicationVersion,
                "C#",
                "-1", "3");

            service =
             new MarketplaceWebServiceClient(
                 profile.SAccessKeyId,
                 profile.SSecretAccessKey,
                 applicationName,
                 applicationVersion,
                 config);

            _currentProfile = profile;
        }
        public void GenerateReport(string sReportType)
        {
            try
            {
                string sReportRequestID;
                RequestReportRequest _requestReportRequest = new RequestReportRequest();
                _requestReportRequest.Merchant = _currentProfile.SSellerId;
                _requestReportRequest.MarketplaceIdList = new IdList();
                _requestReportRequest.MarketplaceIdList.Id = new List<string>(new string[] { _currentProfile.SMarketplaceId });
                _requestReportRequest.ReportType = sReportType;
                _requestReportRequest.StartDate = DateTime.Today;
                _requestReportRequest.EndDate = DateTime.Now;
                sReportRequestID = InvokeRequestReport(service, _requestReportRequest);

                GetReportRequestListRequest _getReportRequestListRequest = new GetReportRequestListRequest();
                _getReportRequestListRequest.Merchant = _currentProfile.SSellerId;
                _getReportRequestListRequest.Marketplace = _currentProfile.SMarketplaceId;
                _getReportRequestListRequest.ReportRequestIdList = new IdList();
                _getReportRequestListRequest.ReportRequestIdList.Id = new List<string>(new string[] { sReportRequestID });
                List<ReportRequestInfo> reportRequestInfoList = new List<ReportRequestInfo>();
                reportRequestInfoList = InvokeGetReportRequestList(service, _getReportRequestListRequest);
                List<ReportInfo> reportListInfo = new List<ReportInfo>();
                foreach (ReportRequestInfo reportInfo in reportRequestInfoList)
                {
                    if (string.IsNullOrEmpty(reportInfo.GeneratedReportId))
                    {
                        GetReportListRequest _getReportListRequest = new GetReportListRequest();
                        _getReportListRequest.ReportRequestIdList = new IdList();
                        _getReportListRequest.ReportRequestIdList.Id = new List<string>(new string[] { reportInfo.ReportRequestId });
                        _getReportListRequest.Merchant = _currentProfile.SSellerId;
                        _getReportListRequest.Marketplace = _currentProfile.SMarketplaceId;
                        reportListInfo = InvokeGetReportList(service, _getReportListRequest);
                        foreach (ReportInfo reportID in reportListInfo)
                        {
                            int reportNumber = 0;


                            GetReportRequest reportRequest = new GetReportRequest();
                            reportRequest.Marketplace = _currentProfile.SMarketplaceId;
                            reportRequest.Merchant = _currentProfile.SSellerId;
                            reportRequest.ReportId = reportID.ReportId;
                            reportRequest.Report = File.Open("_report_" + reportNumber.ToString() + ".xml", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                            InvokeGetReport(service, reportRequest);
                            DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, sClass, "GenerateReport() start generate report");
                            reportNumber++;

                        }
                    }
                    else
                    {
                        int reportNumber = 0;


                        GetReportRequest reportRequest = new GetReportRequest();
                        reportRequest.Marketplace = _currentProfile.SMarketplaceId;
                        reportRequest.Merchant = _currentProfile.SSellerId;
                        reportRequest.ReportId = reportInfo.GeneratedReportId;
                        reportRequest.Report = File.Open("report_" + reportNumber.ToString() + ".xml", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        InvokeGetReport(service, reportRequest);
                        DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, sClass, "GenerateReport() start generate report");
                        reportNumber++;
                    }

                }

                // InvokeGetReportList(service, _getReportListRequest);
                // InvokeManageReportSchedule(service, _manageReportScheduleRequest);
            }
            catch (MarketplaceWebServiceException ex)
            {

            }
        }
        public void ScheduleReportRequest(string sReportType, string sPeriod)
        {
            try
            {
                ManageReportScheduleRequest _manageReportScheduleRequest = new ManageReportScheduleRequest();
                _manageReportScheduleRequest.ReportType = sReportType;
                _manageReportScheduleRequest.Schedule = sPeriod;
                //_manageReportScheduleRequest.Schedule = "_15_MINUTES_";
                _manageReportScheduleRequest.Merchant = _currentProfile.SSellerId;
                _manageReportScheduleRequest.Marketplace = _currentProfile.SMarketplaceId;

                InvokeManageReportSchedule(service, _manageReportScheduleRequest);
            }
            catch (MarketplaceWebServiceException mwsEx)
            {
                DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, sClass, "ScheduleReportRequest() exception message:"+mwsEx.Message);
            }

        }

        public DataTable GetScheduledReport(string sReportType)
        {
            //Manage Report Schedule
            DataTable reportTable = new DataTable(sReportType);
            try
            {
                GetReportListRequest _getReportListRequest = new GetReportListRequest();
                _getReportListRequest.Merchant = _currentProfile.SSellerId;
                _getReportListRequest.Marketplace = _currentProfile.SMarketplaceId;
                TypeList reportTypeList = new TypeList();
                reportTypeList.Type = new List<string>();
                reportTypeList.Type.Add(sReportType);

                List<ReportInfo> reportListInfo = InvokeGetReportList(service, _getReportListRequest);
                string sTemp;
                UpdateReportAcknowledgementsRequest request = new UpdateReportAcknowledgementsRequest();

                request.Marketplace = _currentProfile.SMarketplaceId;
                request.Merchant = _currentProfile.SSellerId;
                request.ReportIdList = new IdList();
                request.ReportIdList.Id = new List<string>();
                int reportNumber = 0;

                foreach (ReportInfo info in reportListInfo)
                {
                    if (info.ReportType == sReportType && info.Acknowledged == false)
                    //if (info.ReportType == sReportType)
                    {

                        GetReportRequest reportRequest = new GetReportRequest();
                        reportRequest.Marketplace = _currentProfile.SMarketplaceId;
                        reportRequest.Merchant = _currentProfile.SSellerId;
                        reportRequest.ReportId = info.ReportId;
                        //reportRequest.Report = File.Open("report_" + info.ReportType + "_" + DateTime.Now.ToString("MM_dd_yyyy_HH_mm_ss") + ".xml", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        reportRequest.Report = new MemoryStream();
                        sTemp = InvokeGetReport(service, reportRequest);
                        StreamReader strReader = new StreamReader(reportRequest.Report);
                        int iLine = 0;
                        while (strReader.Peek() >= 0)
                        {
                            if (iLine == 0)
                            {
                                string sDataColumnHeader = strReader.ReadLine();
                                string[] sColumns = sDataColumnHeader.Split('\t');
                                foreach (string col in sColumns)
                                {
                                    reportTable.Columns.Add(col, typeof(String));
                                }
                            }
                            else
                            {
                                string sDataRow = strReader.ReadLine();
                                //breakdown data and insert to sql database
                                DataRow reportDataRow = reportTable.NewRow();
                                string[] sDataList = sDataRow.Split('\t');

                                int iRowIndex = 0;
                                foreach (string data in sDataList)
                                {
                                    reportDataRow[iRowIndex] = data;
                                    iRowIndex++;
                                }

                                reportTable.Rows.Add(reportDataRow);
                            }

                            iLine++;
                        }

                        strReader.Close();
                        reportRequest.Report = null;
                        request.ReportIdList.Id.Add(info.ReportId);
                        reportNumber++;
                    }
                }

                if (request.ReportIdList.Id.Count > 0)
                    InvokeUpdateReportAcknowledgements(service, request);

            }
            catch (MarketplaceWebServiceException mwsEx)
            {
                DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, sClass, "ScheduleReportRequest() exception message:" + mwsEx.Message);
            }

            return reportTable;
        }

        private List<ReportRequestInfo> InvokeGetReportRequestList(MarketplaceWebService.MarketplaceWebService service, GetReportRequestListRequest request)
        {
            List<ReportRequestInfo> reportRequestInfoList = new List<ReportRequestInfo>();
            try
            {
                GetReportRequestListResponse response = service.GetReportRequestList(request);


                Console.WriteLine("Service Response");
                Console.WriteLine("=============================================================================");
                Console.WriteLine();

                Console.WriteLine("        GetReportRequestListResponse");

                if (response.IsSetGetReportRequestListResult())
                {
                    Console.WriteLine("            GetReportRequestListResult");
                    GetReportRequestListResult getReportRequestListResult = response.GetReportRequestListResult;
                    reportRequestInfoList = getReportRequestListResult.ReportRequestInfo;
                    while (reportRequestInfoList[0].ReportProcessingStatus != "_DONE_")
                    {
                        Thread.Sleep(20000);
                        if (reportRequestInfoList[0].ReportProcessingStatus == "_CANCELLED_")
                        {
                            DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, sClass, "InvokeGetReportRequestList() report[0] request status  = " + reportRequestInfoList[0].ReportProcessingStatus);
                            break;
                        }
                        response = service.GetReportRequestList(request);
                        getReportRequestListResult = response.GetReportRequestListResult;
                        reportRequestInfoList = getReportRequestListResult.ReportRequestInfo;
                    }
                    if (getReportRequestListResult.IsSetNextToken())
                    {


                    }

                    if (getReportRequestListResult.IsSetHasNext())
                    {

                    }

                    if (!string.IsNullOrEmpty(getReportRequestListResult.NextToken))
                    {
                        List<ReportRequestInfo> nextReportRequestList = new List<ReportRequestInfo>();
                        GetReportRequestListByNextTokenRequest nextTokenRequest = new GetReportRequestListByNextTokenRequest();
                        nextTokenRequest.Marketplace = request.Marketplace;
                        nextTokenRequest.Merchant = request.Merchant;
                        nextTokenRequest.NextToken = getReportRequestListResult.NextToken;
                        nextReportRequestList = InvokeGetReportRequestListByNextToken(service, nextTokenRequest);
                        reportRequestInfoList.Concat(nextReportRequestList);
                    }
                }
                if (response.IsSetResponseMetadata())
                {
                    Console.WriteLine("            ResponseMetadata");
                    ResponseMetadata responseMetadata = response.ResponseMetadata;
                    if (responseMetadata.IsSetRequestId())
                    {
                        Console.WriteLine("                RequestId");
                        Console.WriteLine("                    {0}", responseMetadata.RequestId);
                    }
                }

                Console.WriteLine("            ResponseHeaderMetadata");
                Console.WriteLine("                RequestId");
                Console.WriteLine("                    " + response.ResponseHeaderMetadata.RequestId);
                Console.WriteLine("                ResponseContext");
                Console.WriteLine("                    " + response.ResponseHeaderMetadata.ResponseContext);
                Console.WriteLine("                Timestamp");
                Console.WriteLine("                    " + response.ResponseHeaderMetadata.Timestamp);

            }
            catch (MarketplaceWebServiceException ex)
            {
                Console.WriteLine("Caught Exception: " + ex.Message);
                Console.WriteLine("Response Status Code: " + ex.StatusCode);
                Console.WriteLine("Error Code: " + ex.ErrorCode);
                Console.WriteLine("Error Type: " + ex.ErrorType);
                Console.WriteLine("Request ID: " + ex.RequestId);
                Console.WriteLine("XML: " + ex.XML);
                Console.WriteLine("ResponseHeaderMetadata: " + ex.ResponseHeaderMetadata);
            }
            return reportRequestInfoList;
        }

        private List<ReportRequestInfo> InvokeGetReportRequestListByNextToken(MarketplaceWebService.MarketplaceWebService service, GetReportRequestListByNextTokenRequest request)
        {
            List<ReportRequestInfo> reportRequestInfoList = new List<ReportRequestInfo>();
            try
            {
                GetReportRequestListByNextTokenResponse response = service.GetReportRequestListByNextToken(request);


                Console.WriteLine("Service Response");
                Console.WriteLine("=============================================================================");
                Console.WriteLine();

                Console.WriteLine("        GetReportRequestListByNextTokenResponse");
                if (response.IsSetGetReportRequestListByNextTokenResult())
                {
                    Console.WriteLine("            GetReportRequestListByNextTokenResult");
                    GetReportRequestListByNextTokenResult getReportRequestListByNextTokenResult = response.GetReportRequestListByNextTokenResult;
                    reportRequestInfoList = getReportRequestListByNextTokenResult.ReportRequestInfo;
                    foreach (ReportRequestInfo reportInfo in reportRequestInfoList)
                    {
                        while (reportInfo.ReportProcessingStatus != "_DONE_")
                        {
                            Thread.Sleep(20000);
                            response = service.GetReportRequestListByNextToken(request);
                            getReportRequestListByNextTokenResult = response.GetReportRequestListByNextTokenResult;
                            reportRequestInfoList = getReportRequestListByNextTokenResult.ReportRequestInfo;
                        }
                    }
                    if (getReportRequestListByNextTokenResult.IsSetNextToken())
                    {
                        Console.WriteLine("                NextToken");
                        Console.WriteLine("                    {0}", getReportRequestListByNextTokenResult.NextToken);
                    }
                    if (getReportRequestListByNextTokenResult.IsSetHasNext())
                    {

                        Console.WriteLine("                HasNext");
                        Console.WriteLine("                    {0}", getReportRequestListByNextTokenResult.HasNext);


                    }

                    if (!string.IsNullOrEmpty(getReportRequestListByNextTokenResult.NextToken))
                    {
                        List<ReportRequestInfo> nextReportRequestList = new List<ReportRequestInfo>();
                        nextReportRequestList = InvokeGetReportRequestListByNextToken(service, request);
                        reportRequestInfoList.Concat(nextReportRequestList);
                    }

                }
                if (response.IsSetResponseMetadata())
                {
                    Console.WriteLine("            ResponseMetadata");
                    ResponseMetadata responseMetadata = response.ResponseMetadata;
                    if (responseMetadata.IsSetRequestId())
                    {
                        Console.WriteLine("                RequestId");
                        Console.WriteLine("                    {0}", responseMetadata.RequestId);
                    }
                }

                Console.WriteLine("            ResponseHeaderMetadata");
                Console.WriteLine("                RequestId");
                Console.WriteLine("                    " + response.ResponseHeaderMetadata.RequestId);
                Console.WriteLine("                ResponseContext");
                Console.WriteLine("                    " + response.ResponseHeaderMetadata.ResponseContext);
                Console.WriteLine("                Timestamp");
                Console.WriteLine("                    " + response.ResponseHeaderMetadata.Timestamp);

            }
            catch (MarketplaceWebServiceException ex)
            {
                Console.WriteLine("Caught Exception: " + ex.Message);
                Console.WriteLine("Response Status Code: " + ex.StatusCode);
                Console.WriteLine("Error Code: " + ex.ErrorCode);
                Console.WriteLine("Error Type: " + ex.ErrorType);
                Console.WriteLine("Request ID: " + ex.RequestId);
                Console.WriteLine("XML: " + ex.XML);
                Console.WriteLine("ResponseHeaderMetadata: " + ex.ResponseHeaderMetadata);
            }
            return reportRequestInfoList;
        }

        private void InvokeUpdateReportAcknowledgements(MarketplaceWebService.MarketplaceWebService service, UpdateReportAcknowledgementsRequest request)
        {
            try
            {
                UpdateReportAcknowledgementsResponse response = service.UpdateReportAcknowledgements(request);


                Console.WriteLine("Service Response");
                Console.WriteLine("=============================================================================");
                Console.WriteLine();

                Console.WriteLine("        UpdateReportAcknowledgementsResponse");
                if (response.IsSetUpdateReportAcknowledgementsResult())
                {
                    Console.WriteLine("            UpdateReportAcknowledgementsResult");
                    UpdateReportAcknowledgementsResult updateReportAcknowledgementsResult = response.UpdateReportAcknowledgementsResult;
                    if (updateReportAcknowledgementsResult.IsSetCount())
                    {
                        Console.WriteLine("                Count");
                        Console.WriteLine("                    {0}", updateReportAcknowledgementsResult.Count);
                    }
                    List<ReportInfo> reportInfoList = updateReportAcknowledgementsResult.ReportInfo;
                    foreach (ReportInfo reportInfo in reportInfoList)
                    {
                        Console.WriteLine("                ReportInfo");
                        if (reportInfo.IsSetReportId())
                        {
                            Console.WriteLine("                    ReportId");
                            Console.WriteLine("                        {0}", reportInfo.ReportId);
                        }
                        if (reportInfo.IsSetReportType())
                        {
                            Console.WriteLine("                    ReportType");
                            Console.WriteLine("                        {0}", reportInfo.ReportType);
                        }
                        if (reportInfo.IsSetReportRequestId())
                        {
                            Console.WriteLine("                    ReportRequestId");
                            Console.WriteLine("                        {0}", reportInfo.ReportRequestId);
                        }
                        if (reportInfo.IsSetAvailableDate())
                        {
                            Console.WriteLine("                    AvailableDate");
                            Console.WriteLine("                        {0}", reportInfo.AvailableDate);
                        }
                        if (reportInfo.IsSetAcknowledged())
                        {
                            Console.WriteLine("                    Acknowledged");
                            Console.WriteLine("                        {0}", reportInfo.Acknowledged);
                        }
                        if (reportInfo.IsSetAcknowledgedDate())
                        {
                            Console.WriteLine("                    AcknowledgedDate");
                            Console.WriteLine("                        {0}", reportInfo.AcknowledgedDate);
                        }
                    }
                }
                if (response.IsSetResponseMetadata())
                {
                    Console.WriteLine("            ResponseMetadata");
                    ResponseMetadata responseMetadata = response.ResponseMetadata;
                    if (responseMetadata.IsSetRequestId())
                    {
                        Console.WriteLine("                RequestId");
                        Console.WriteLine("                    {0}", responseMetadata.RequestId);
                    }
                }

                Console.WriteLine("            ResponseHeaderMetadata");
                Console.WriteLine("                RequestId");
                Console.WriteLine("                    " + response.ResponseHeaderMetadata.RequestId);
                Console.WriteLine("                ResponseContext");
                Console.WriteLine("                    " + response.ResponseHeaderMetadata.ResponseContext);
                Console.WriteLine("                Timestamp");
                Console.WriteLine("                    " + response.ResponseHeaderMetadata.Timestamp);

            }
            catch (MarketplaceWebServiceException ex)
            {
                Console.WriteLine("Caught Exception: " + ex.Message);
                Console.WriteLine("Response Status Code: " + ex.StatusCode);
                Console.WriteLine("Error Code: " + ex.ErrorCode);
                Console.WriteLine("Error Type: " + ex.ErrorType);
                Console.WriteLine("Request ID: " + ex.RequestId);
                Console.WriteLine("XML: " + ex.XML);
                Console.WriteLine("ResponseHeaderMetadata: " + ex.ResponseHeaderMetadata);
            }
        }

        private void InvokeManageReportSchedule(MarketplaceWebService.MarketplaceWebService service, ManageReportScheduleRequest request)
        {
            try
            {
                ManageReportScheduleResponse response = service.ManageReportSchedule(request);

                if (response.IsSetManageReportScheduleResult())
                {
                    Console.WriteLine("            ManageReportScheduleResult");
                    ManageReportScheduleResult manageReportScheduleResult = response.ManageReportScheduleResult;
                    if (manageReportScheduleResult.IsSetCount())
                    {
                        Console.WriteLine("                Count");
                        Console.WriteLine("                    {0}", manageReportScheduleResult.Count);
                    }
                    List<ReportSchedule> reportScheduleList = manageReportScheduleResult.ReportSchedule;
                    foreach (ReportSchedule reportSchedule in reportScheduleList)
                    {
                        Console.WriteLine("                ReportSchedule");
                        if (reportSchedule.IsSetReportType())
                        {
                            Console.WriteLine("                    ReportType");
                            Console.WriteLine("                        {0}", reportSchedule.ReportType);
                        }
                        if (reportSchedule.IsSetSchedule())
                        {
                            Console.WriteLine("                    Schedule");
                            Console.WriteLine("                        {0}", reportSchedule.Schedule);
                        }
                        if (reportSchedule.IsSetScheduledDate())
                        {
                            Console.WriteLine("                    ScheduledDate");
                            Console.WriteLine("                        {0}", reportSchedule.ScheduledDate);
                        }
                    }
                }
                if (response.IsSetResponseMetadata())
                {
                    Console.WriteLine("            ResponseMetadata");
                    ResponseMetadata responseMetadata = response.ResponseMetadata;
                    if (responseMetadata.IsSetRequestId())
                    {
                        Console.WriteLine("                RequestId");
                        Console.WriteLine("                    {0}", responseMetadata.RequestId);
                    }
                }

                Console.WriteLine("            ResponseHeaderMetadata");
                Console.WriteLine("                RequestId");
                Console.WriteLine("                    " + response.ResponseHeaderMetadata.RequestId);
                Console.WriteLine("                ResponseContext");
                Console.WriteLine("                    " + response.ResponseHeaderMetadata.ResponseContext);
                Console.WriteLine("                Timestamp");
                Console.WriteLine("                    " + response.ResponseHeaderMetadata.Timestamp);

            }
            catch (MarketplaceWebServiceException ex)
            {
                Console.WriteLine("Caught Exception: " + ex.Message);
                Console.WriteLine("Response Status Code: " + ex.StatusCode);
                Console.WriteLine("Error Code: " + ex.ErrorCode);
                Console.WriteLine("Error Type: " + ex.ErrorType);
                Console.WriteLine("Request ID: " + ex.RequestId);
                Console.WriteLine("XML: " + ex.XML);
                Console.WriteLine("ResponseHeaderMetadata: " + ex.ResponseHeaderMetadata);
            }
        }

        private List<ReportInfo> InvokeGetReportList(MarketplaceWebService.MarketplaceWebService service, GetReportListRequest request)
        {
            List<ReportInfo> reportInfoList = new List<ReportInfo>();
            try
            {
                GetReportListResponse response = service.GetReportList(request);


                Console.WriteLine("Service Response");
                Console.WriteLine("=============================================================================");
                Console.WriteLine();

                Console.WriteLine("        GetReportListResponse");
                if (response.IsSetGetReportListResult())
                {

                    Console.WriteLine("            GetReportListResult");

                    GetReportListResult getReportListResult = response.GetReportListResult;
                    reportInfoList = getReportListResult.ReportInfo;

                    if (getReportListResult.IsSetNextToken())
                    {
                        Console.WriteLine("                NextToken");
                        Console.WriteLine("                    {0}", getReportListResult.NextToken);
                    }
                    if (getReportListResult.IsSetHasNext())
                    {

                        Console.WriteLine("                HasNext");
                        Console.WriteLine("                    {0}", getReportListResult.HasNext);

                    }

                    if (!string.IsNullOrEmpty(getReportListResult.NextToken))
                    {
                        GetReportListByNextTokenRequest getNextTokenRequest = new GetReportListByNextTokenRequest();
                        getNextTokenRequest.NextToken = getReportListResult.NextToken;
                        List<ReportInfo> nextTokenReportList = InvokeGetReportListByNextToken(service, getNextTokenRequest);
                        reportInfoList.Concat(nextTokenReportList);
                    }

                }
                if (response.IsSetResponseMetadata())
                {
                    Console.WriteLine("            ResponseMetadata");
                    ResponseMetadata responseMetadata = response.ResponseMetadata;
                    if (responseMetadata.IsSetRequestId())
                    {
                        Console.WriteLine("                RequestId");
                        Console.WriteLine("                    {0}", responseMetadata.RequestId);
                    }
                }
            }
            catch (MarketplaceWebServiceException ex)
            {
                Console.WriteLine("Caught Exception: " + ex.Message);
                Console.WriteLine("Response Status Code: " + ex.StatusCode);
                Console.WriteLine("Error Code: " + ex.ErrorCode);
                Console.WriteLine("Error Type: " + ex.ErrorType);
                Console.WriteLine("Request ID: " + ex.RequestId);
                Console.WriteLine("XML: " + ex.XML);
                Console.WriteLine("ResponseHeaderMetadata: " + ex.ResponseHeaderMetadata);
            }
            return reportInfoList;
        }

        private List<ReportInfo> InvokeGetReportListByNextToken(MarketplaceWebService.MarketplaceWebService service, GetReportListByNextTokenRequest request)
        {
            List<ReportInfo> nextTokenReportList = new List<ReportInfo>();
            try
            {
                GetReportListByNextTokenResponse response = service.GetReportListByNextToken(request);


                Console.WriteLine("Service Response");
                Console.WriteLine("=============================================================================");
                Console.WriteLine();

                Console.WriteLine("        GetReportListByNextTokenResponse");
                if (response.IsSetGetReportListByNextTokenResult())
                {

                    Console.WriteLine("            GetReportListByNextTokenResult");
                    GetReportListByNextTokenResult getReportListByNextTokenResult = response.GetReportListByNextTokenResult;
                    nextTokenReportList = getReportListByNextTokenResult.ReportInfo;
                    if (getReportListByNextTokenResult.IsSetNextToken())
                    {
                        Console.WriteLine("                NextToken");
                        Console.WriteLine("                    {0}", getReportListByNextTokenResult.NextToken);
                    }
                    if (getReportListByNextTokenResult.IsSetHasNext())
                    {
                        Console.WriteLine("                HasNext");
                        Console.WriteLine("                    {0}", getReportListByNextTokenResult.HasNext);

                    }

                    if (!string.IsNullOrEmpty(getReportListByNextTokenResult.NextToken))
                    {
                        request.NextToken = getReportListByNextTokenResult.NextToken;
                        nextTokenReportList.Concat(InvokeGetReportListByNextToken(service, request));
                    }
                }
                if (response.IsSetResponseMetadata())
                {
                    Console.WriteLine("            ResponseMetadata");
                    ResponseMetadata responseMetadata = response.ResponseMetadata;
                    if (responseMetadata.IsSetRequestId())
                    {
                        Console.WriteLine("                RequestId");
                        Console.WriteLine("                    {0}", responseMetadata.RequestId);
                    }
                }

                Console.WriteLine("            ResponseHeaderMetadata");
                Console.WriteLine("                RequestId");
                Console.WriteLine("                    " + response.ResponseHeaderMetadata.RequestId);
                Console.WriteLine("                ResponseContext");
                Console.WriteLine("                    " + response.ResponseHeaderMetadata.ResponseContext);
                Console.WriteLine("                Timestamp");
                Console.WriteLine("                    " + response.ResponseHeaderMetadata.Timestamp);

            }
            catch (MarketplaceWebServiceException ex)
            {
                Console.WriteLine("Caught Exception: " + ex.Message);
                Console.WriteLine("Response Status Code: " + ex.StatusCode);
                Console.WriteLine("Error Code: " + ex.ErrorCode);
                Console.WriteLine("Error Type: " + ex.ErrorType);
                Console.WriteLine("Request ID: " + ex.RequestId);
                Console.WriteLine("XML: " + ex.XML);
                Console.WriteLine("ResponseHeaderMetadata: " + ex.ResponseHeaderMetadata);
            }

            return nextTokenReportList;
        }

        private string InvokeRequestReport(MarketplaceWebService.MarketplaceWebService service, RequestReportRequest request)
        {
            string sResult = "";
            try
            {
                RequestReportResponse response = service.RequestReport(request);


                Console.WriteLine("Service Response");
                Console.WriteLine("=============================================================================");
                Console.WriteLine();

                Console.WriteLine("        RequestReportResponse");

                if (response.IsSetRequestReportResult())
                {
                    Console.WriteLine("            RequestReportResult");
                    RequestReportResult requestReportResult = response.RequestReportResult;
                    ReportRequestInfo reportRequestInfo = requestReportResult.ReportRequestInfo;
                    Console.WriteLine("                  ReportRequestInfo");

                    if (reportRequestInfo.IsSetReportProcessingStatus())
                    {
                        Console.WriteLine("               ReportProcessingStatus");
                        Console.WriteLine("                                  {0}", reportRequestInfo.ReportProcessingStatus);
                    }
                    if (reportRequestInfo.IsSetReportRequestId())
                    {
                        Console.WriteLine("                      ReportRequestId");
                        Console.WriteLine("                                  {0}", reportRequestInfo.ReportRequestId);
                        sResult = reportRequestInfo.ReportRequestId;
                    }
                    if (reportRequestInfo.IsSetReportType())
                    {
                        Console.WriteLine("                           ReportType");
                        Console.WriteLine("                                  {0}", reportRequestInfo.ReportType);
                    }
                    if (reportRequestInfo.IsSetStartDate())
                    {
                        Console.WriteLine("                            StartDate");
                        Console.WriteLine("                                  {0}", reportRequestInfo.StartDate);
                    }
                    if (reportRequestInfo.IsSetEndDate())
                    {
                        Console.WriteLine("                              EndDate");
                        Console.WriteLine("                                  {0}", reportRequestInfo.EndDate);
                    }
                    if (reportRequestInfo.IsSetSubmittedDate())
                    {
                        Console.WriteLine("                        SubmittedDate");
                        Console.WriteLine("                                  {0}", reportRequestInfo.SubmittedDate);
                    }
                }
                if (response.IsSetResponseMetadata())
                {
                    Console.WriteLine("            ResponseMetadata");
                    ResponseMetadata responseMetadata = response.ResponseMetadata;
                    if (responseMetadata.IsSetRequestId())
                    {
                        Console.WriteLine("                RequestId");
                        Console.WriteLine("                    {0}", responseMetadata.RequestId);
                    }
                }

                Console.WriteLine("            ResponseHeaderMetadata");
                Console.WriteLine("                RequestId");
                Console.WriteLine("                    " + response.ResponseHeaderMetadata.RequestId);
                Console.WriteLine("                ResponseContext");
                Console.WriteLine("                    " + response.ResponseHeaderMetadata.ResponseContext);
                Console.WriteLine("                Timestamp");
                Console.WriteLine("                    " + response.ResponseHeaderMetadata.Timestamp);

            }
            catch (MarketplaceWebServiceException ex)
            {
                Console.WriteLine("Caught Exception: " + ex.Message);
                Console.WriteLine("Response Status Code: " + ex.StatusCode);
                Console.WriteLine("Error Code: " + ex.ErrorCode);
                Console.WriteLine("Error Type: " + ex.ErrorType);
                Console.WriteLine("Request ID: " + ex.RequestId);
                Console.WriteLine("XML: " + ex.XML);
                Console.WriteLine("ResponseHeaderMetadata: " + ex.ResponseHeaderMetadata);
            }

            return sResult;
        }
        private string InvokeGetReport(MarketplaceWebService.MarketplaceWebService service, GetReportRequest request)
        {
            string sReportContent = "";
            try
            {
                GetReportResponse response = service.GetReport(request);


                Console.WriteLine("Service Response");
                Console.WriteLine("=============================================================================");
                Console.WriteLine();

                Console.WriteLine("        GetReportResponse");
                if (response.IsSetGetReportResult())
                {
                    Console.WriteLine("            GetReportResult");
                    GetReportResult getReportResult = response.GetReportResult;
                    if (getReportResult.IsSetContentMD5())
                    {
                        Console.WriteLine("                ContentMD5");
                        Console.WriteLine("                    {0}", getReportResult.ContentMD5);
                    }
                }
                if (response.IsSetResponseMetadata())
                {
                    Console.WriteLine("            ResponseMetadata");
                    ResponseMetadata responseMetadata = response.ResponseMetadata;
                    if (responseMetadata.IsSetRequestId())
                    {
                        Console.WriteLine("                RequestId");
                        Console.WriteLine("                    {0}", responseMetadata.RequestId);
                    }
                }

                Console.WriteLine("            ResponseHeaderMetadata");
                Console.WriteLine("                RequestId");
                Console.WriteLine("                    " + response.ResponseHeaderMetadata.RequestId);
                Console.WriteLine("                ResponseContext");
                Console.WriteLine("                    " + response.ResponseHeaderMetadata.ResponseContext);
                sReportContent = response.ResponseHeaderMetadata.ResponseContext;
                Console.WriteLine("                Timestamp");
                Console.WriteLine("                    " + response.ResponseHeaderMetadata.Timestamp);

            }
            catch (MarketplaceWebServiceException ex)
            {
                Console.WriteLine("Caught Exception: " + ex.Message);
                Console.WriteLine("Response Status Code: " + ex.StatusCode);
                Console.WriteLine("Error Code: " + ex.ErrorCode);
                Console.WriteLine("Error Type: " + ex.ErrorType);
                Console.WriteLine("Request ID: " + ex.RequestId);
                Console.WriteLine("XML: " + ex.XML);
                Console.WriteLine("ResponseHeaderMetadata: " + ex.ResponseHeaderMetadata);
            }
            return sReportContent;
        }
    }
}
