using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using FBAInventoryServiceMWS;
using FBAInventoryServiceMWS.Model;
using DebugLogHandler;
namespace MWS.DataSource
{
    public class SellerInventorySupplyList
    {
        private DataSet InventorySupplyList = new DataSet("InventorySupplyList");
        const string marketplaceId = "ATVPDKIKX0DER";
        const string sellerId = "A2TYS339AQK0NU";
        FBAInventoryServiceMWS.FBAInventoryServiceMWS service = null;
        private string _sClass = "SellerInventorySupplyList.cs";
        private string _sLogPath = System.IO.Directory.GetCurrentDirectory() + "\\";

        public SellerInventorySupplyList()
        {
            /************************************************************************
            * Access Key ID and Secret Acess Key ID, obtained from:
            * http://mws.amazon.com
            ***********************************************************************/

            String accessKeyId = "AKIAJ2IS6ZEEGQPJV7WA";
            String secretAccessKey = "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";

            /************************************************************************
             * Marketplace and Seller IDs are required parameters for all 
             * MWS calls.
             ***********************************************************************/


            /************************************************************************
             * The application name and version are included in each MWS call's
             * HTTP User-Agent field. These are required fields.
             ***********************************************************************/
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
            FBAInventoryServiceMWSConfig config = new FBAInventoryServiceMWSConfig();
            //config.ProxyHost = "https://PROXY_URL";
            //config.ProxyPort = 9090;
            //
            // IMPORTANT: Uncomment out the appropiate line for the country you wish 
            // to sell in:
            // 
            // US
            config.ServiceURL =  MWSServer.Properties.Settings.Default["US_ServiceURL"].ToString();
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

            /************************************************************************
            * Instantiate Implementation of FBA Inventory Service MWS 
            ***********************************************************************/
               service =
                new FBAInventoryServiceMWSClient(
                    accessKeyId,
                    secretAccessKey,
                    applicationName,
                    applicationVersion,
                    config); 
        }

        public DataSet GetSellerInventorySupplyList()
        {
            DebugLogHandler.DebugLogHandler.WriteLog(_sLogPath, _sClass, "GetSellerInventorySupplyList() started.");
            ListInventorySupplyRequest request = new ListInventorySupplyRequest();
            // @TODO: set request parameters here
            request.SellerId = sellerId;
            request.Marketplace = marketplaceId;
            request.ResponseGroup = "Detailed";
            request.QueryStartDateTime = DateTime.Today;
            DebugLogHandler.DebugLogHandler.WriteLog(_sLogPath, _sClass, "GetSellerInventorySupplyList() request date:" + request.QueryStartDateTime.ToString());
            ListInventorySupplyHandler getSupplyHandler = new ListInventorySupplyHandler();
            DebugLogHandler.DebugLogHandler.WriteLog(_sLogPath, _sClass, "GetSellerInventorySupplyList() InvokeListInventorySupply");
            InventorySupplyList = getSupplyHandler.InvokeListInventorySupply(service, request);
            return InventorySupplyList;
        }
    }
}