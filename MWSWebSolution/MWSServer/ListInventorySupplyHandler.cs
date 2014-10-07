using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using FBAInventoryServiceMWS;
using FBAInventoryServiceMWS.Model;

namespace MWS
{
    public class ListInventorySupplyHandler
    {
        private DataSet InventorySupplyDataSet;
        private DataTable InventorySupplyTable;
        private string _sLogPath = System.IO.Directory.GetCurrentDirectory() + "\\";
        private string _sClass = "ListInventorySupplyHandler.cs";
        public ListInventorySupplyHandler()
        {
            InitDataSet();
        }

        ~ListInventorySupplyHandler()
        {
            InventorySupplyDataSet = null;
            InventorySupplyTable = null;
        }
        public void InitDataSet()
        {
            if (InventorySupplyDataSet == null)
            {
                InventorySupplyDataSet = new DataSet("InventorySupplyDataSet");

            }
            if (InventorySupplyTable == null)
            {
                InventorySupplyTable = new DataTable("InventorySupplyTable");
                InventorySupplyTable.Columns.Add("FNSKU", typeof(String));
                InventorySupplyTable.Columns.Add("SellerSKU", typeof(String));
                InventorySupplyTable.Columns.Add("ASIN", typeof(String));
                InventorySupplyTable.Columns.Add("Inbound", typeof(Int32));
                InventorySupplyTable.Columns.Add("InStock", typeof(Int32));
                InventorySupplyTable.Columns.Add("Transfer", typeof(Int32));

                if (InventorySupplyDataSet!=null)
                {
                    InventorySupplyDataSet.Tables.Add(InventorySupplyTable);
                }
            }
        }

        private void ClearDataSet()
        {
            InventorySupplyDataSet = null;
            InventorySupplyTable = null;
            InitDataSet();
        }

        public DataSet InvokeListInventorySupply(FBAInventoryServiceMWS.FBAInventoryServiceMWS service, ListInventorySupplyRequest request)
        {

            try
            {

                ListInventorySupplyResponse response = service.ListInventorySupply(request);
                DebugLogHandler.DebugLogHandler.WriteLog(_sLogPath, _sClass, "InvokeListInventorySupply() reponse:" + response.ToXML());
                if (response.IsSetListInventorySupplyResult())
                {
                    ListInventorySupplyResult listInventorySupplyResult = response.ListInventorySupplyResult;
                    if (listInventorySupplyResult.IsSetInventorySupplyList())
                    {
                        InventorySupplyList inventorySupplyList = listInventorySupplyResult.InventorySupplyList;
                        List<InventorySupply> memberList = inventorySupplyList.member;
                        foreach (InventorySupply member in memberList)
                        {
                            int InboundCount = 0;
                            int InStockCount = 0;
                            int TransferCount = 0;
                            //Console.WriteLine("                    member");
                            DataRow InventoryRow = InventorySupplyTable.NewRow();
                            InventoryRow["FNSKU"] = "N/A";
                            InventoryRow["SellerSKU"] = "N/A";
                            InventoryRow["ASIN"] = "N/A";
                            InventoryRow["Inbound"] = InboundCount;
                            InventoryRow["InStock"] = InStockCount;
                            InventoryRow["Transfer"] = TransferCount;
                            if (member.IsSetSellerSKU())
                            {
                                InventoryRow["SellerSKU"] = member.SellerSKU;
                                //Console.WriteLine("                        SellerSKU");
                                //Console.WriteLine("                            {0}", member.SellerSKU);
                            }
                            if (member.IsSetFNSKU())
                            {
                                InventoryRow["FNSKU"] = member.FNSKU;
                                //Console.WriteLine("                        FNSKU");
                                //Console.WriteLine("                            {0}", member.FNSKU);
                            }
                            if (member.IsSetASIN())
                            {
                                InventoryRow["ASIN"] = member.ASIN;
                                //Console.WriteLine("                        ASIN");
                                // Console.WriteLine("                            {0}", member.ASIN);
                            }
                            if (member.IsSetCondition())
                            {
                                //Console.WriteLine("                        Condition");
                                //Console.WriteLine("                            {0}", member.Condition);
                            }
                            if (member.IsSetTotalSupplyQuantity())
                            {
                                //InventoryRow["TotalSupplyQuantity"] = member.TotalSupplyQuantity;
                                //Console.WriteLine("                        TotalSupplyQuantity");
                                //Console.WriteLine("                            {0}", member.TotalSupplyQuantity);
                            }
                            if (member.IsSetInStockSupplyQuantity())
                            {
                                //InventoryRow["InStockSupplyQuantity"] = member.InStockSupplyQuantity;
                                //Console.WriteLine("                        InStockSupplyQuantity");
                                //Console.WriteLine("                            {0}", member.InStockSupplyQuantity);
                            }
                            if (member.IsSetEarliestAvailability())
                            {
                                //Console.WriteLine("                        EarliestAvailability");
                                Timepoint earliestAvailability = member.EarliestAvailability;
                                if (earliestAvailability.IsSetTimepointType())
                                {
                                    //Console.WriteLine("                            TimepointType");
                                    //Console.WriteLine("                                {0}", earliestAvailability.TimepointType);
                                }
                                if (earliestAvailability.IsSetDateTime())
                                {
                                    //Console.WriteLine("                            DateTime");
                                    //Console.WriteLine("                                {0}", earliestAvailability.DateTime);
                                }
                            }
                            if (member.IsSetSupplyDetail())
                            {
                                //Console.WriteLine("                        SupplyDetail");
                                InventorySupplyDetailList supplyDetail = member.SupplyDetail;
                                List<InventorySupplyDetail> member1List = supplyDetail.member;
                                foreach (InventorySupplyDetail member1 in member1List)
                                {
                                    //Console.WriteLine("                            member");
                                    if (member1.IsSetQuantity())
                                    {          
                                        if (member1.SupplyType == "InStock")
                                            InStockCount += Int32.Parse(member1.Quantity.ToString());
                                        if (member1.SupplyType == "Inbound")
                                            InboundCount += Int32.Parse(member1.Quantity.ToString());
                                        if (member1.SupplyType == "Transfer")
                                            TransferCount = Int32.Parse(member.TotalSupplyQuantity.ToString()) -  Int32.Parse(member1.Quantity.ToString());
                                        //InventoryRow[member1.SupplyType] = member1.Quantity;
                                        //Console.WriteLine("                                Quantity");
                                        //Console.WriteLine("                                    {0}", member1.Quantity);
                                    }
                                    if (member1.IsSetSupplyType())
                                    {
                                        //Console.WriteLine("                                SupplyType");
                                        //Console.WriteLine("                                    {0}", member1.SupplyType);
                                    }
                                    if (member1.IsSetEarliestAvailableToPick())
                                    {
                                        //Console.WriteLine("                                EarliestAvailableToPick");
                                        //Timepoint earliestAvailableToPick = member1.EarliestAvailableToPick;
                                        //if (earliestAvailableToPick.IsSetTimepointType())
                                        //{
                                        //    Console.WriteLine("                                    TimepointType");
                                        //    Console.WriteLine("                                        {0}", earliestAvailableToPick.TimepointType);
                                        //}
                                        //if (earliestAvailableToPick.IsSetDateTime())
                                        //{
                                        //    Console.WriteLine("                                    DateTime");
                                        //    Console.WriteLine("                                        {0}", earliestAvailableToPick.DateTime);
                                        //}
                                    }
                                    if (member1.IsSetLatestAvailableToPick())
                                    {
                                        //Console.WriteLine("                                LatestAvailableToPick");
                                        //Timepoint latestAvailableToPick = member1.LatestAvailableToPick;
                                        //if (latestAvailableToPick.IsSetTimepointType())
                                        //{
                                        //    Console.WriteLine("                                    TimepointType");
                                        //    Console.WriteLine("                                        {0}", latestAvailableToPick.TimepointType);
                                        //}
                                        //if (latestAvailableToPick.IsSetDateTime())
                                        //{
                                        //    Console.WriteLine("                                    DateTime");
                                        //    Console.WriteLine("                                        {0}", latestAvailableToPick.DateTime);
                                        //}
                                    }
                                }
                            }//end of foreach member1 list

                            InventoryRow["Inbound"] = InboundCount;
                            InventoryRow["InStock"] = InStockCount;
                            InventoryRow["Transfer"] = TransferCount;

                            InventorySupplyTable.Rows.Add(InventoryRow);
                        } // end of foreach member list


                    }
                    if (listInventorySupplyResult.IsSetNextToken())
                    {
                        //Console.WriteLine("                NextToken");
                        //Console.WriteLine("                    {0}", listInventorySupplyResult.NextToken);
                        ListInventorySupplyByNextTokenRequest nextTokenRequest = new ListInventorySupplyByNextTokenRequest();
                        nextTokenRequest.SellerId = request.SellerId;
                        nextTokenRequest.Marketplace = request.Marketplace;
                        nextTokenRequest.NextToken = listInventorySupplyResult.NextToken;
                        InvokeListInventorySupplyByNextToken(service, nextTokenRequest);
                    }
                }
                if (response.IsSetResponseMetadata())
                {
                    Console.WriteLine("            ResponseMetadata");
                    ResponseMetadata responseMetadata = response.ResponseMetadata;
                    if (responseMetadata.IsSetRequestId())
                    {
                        //Console.WriteLine("                RequestId");
                        //Console.WriteLine("                    {0}", responseMetadata.RequestId);
                    }
                }
            }
            catch (FBAInventoryServiceMWSException ex)
            {
                Console.WriteLine("Caught Exception: " + ex.Message);
                Console.WriteLine("Response Status Code: " + ex.StatusCode);
                Console.WriteLine("Error Code: " + ex.ErrorCode);
                Console.WriteLine("Error Type: " + ex.ErrorType);
                Console.WriteLine("Request ID: " + ex.RequestId);
                Console.WriteLine("XML: " + ex.XML);
                DebugLogHandler.DebugLogHandler.WriteLog(_sLogPath, _sClass, "Caught Exception: " + ex.Message);
                DebugLogHandler.DebugLogHandler.WriteLog(_sLogPath, _sClass, "Response Status Code: " + ex.StatusCode);
                DebugLogHandler.DebugLogHandler.WriteLog(_sLogPath, _sClass, "Error Code: " + ex.ErrorCode);
                DebugLogHandler.DebugLogHandler.WriteLog(_sLogPath, _sClass, "Error Type: " + ex.ErrorType);
                DebugLogHandler.DebugLogHandler.WriteLog(_sLogPath, _sClass, "Request ID: " + ex.RequestId);
                DebugLogHandler.DebugLogHandler.WriteLog(_sLogPath, _sClass, "XML: " + ex.XML); 
            }

            return InventorySupplyDataSet;
        }


        public DataSet InvokeListInventorySupplyByNextToken(FBAInventoryServiceMWS.FBAInventoryServiceMWS service, ListInventorySupplyByNextTokenRequest request)
        {

            try
            {

                ListInventorySupplyByNextTokenResponse response = service.ListInventorySupplyByNextToken(request);

                if (response.IsSetListInventorySupplyByNextTokenResult())
                {
                    ListInventorySupplyByNextTokenResult listInventorySupplyByNextTokenResult = response.ListInventorySupplyByNextTokenResult;
                    if (listInventorySupplyByNextTokenResult.IsSetInventorySupplyList())
                    {
                        InventorySupplyList inventorySupplyList = listInventorySupplyByNextTokenResult.InventorySupplyList;
                        List<InventorySupply> memberList = inventorySupplyList.member;
                        foreach (InventorySupply member in memberList)
                        {
                            int InboundCount = 0;
                            int InStockCount = 0;
                            int TransferCount = 0;
                            //Console.WriteLine("                    member");
                            DataRow InventoryRow = InventorySupplyTable.NewRow();
                            InventoryRow["FNSKU"] = "N/A";
                            InventoryRow["SellerSKU"] = "N/A";
                            InventoryRow["ASIN"] = "N/A";
                            InventoryRow["Inbound"] = InboundCount;
                            InventoryRow["InStock"] = InStockCount;
                            InventoryRow["Transfer"] = TransferCount;
                            if (member.IsSetSellerSKU())
                            {
                                InventoryRow["SellerSKU"] = member.SellerSKU;
                                //Console.WriteLine("                        SellerSKU");
                                //Console.WriteLine("                            {0}", member.SellerSKU);
                            }
                            if (member.IsSetFNSKU())
                            {
                                InventoryRow["FNSKU"] = member.FNSKU;
                                //Console.WriteLine("                        FNSKU");
                                //Console.WriteLine("                            {0}", member.FNSKU);
                            }
                            if (member.IsSetASIN())
                            {
                                InventoryRow["ASIN"] = member.ASIN;
                                //Console.WriteLine("                        ASIN");
                                // Console.WriteLine("                            {0}", member.ASIN);
                            }
                            if (member.IsSetCondition())
                            {
                                //Console.WriteLine("                        Condition");
                                //Console.WriteLine("                            {0}", member.Condition);
                            }
                            if (member.IsSetTotalSupplyQuantity())
                            {
                                //InventoryRow["TotalSupplyQuantity"] = member.TotalSupplyQuantity;
                                //Console.WriteLine("                        TotalSupplyQuantity");
                                //Console.WriteLine("                            {0}", member.TotalSupplyQuantity);
                            }
                            if (member.IsSetInStockSupplyQuantity())
                            {
                                //InventoryRow["InStockSupplyQuantity"] = member.InStockSupplyQuantity;
                                //Console.WriteLine("                        InStockSupplyQuantity");
                                //Console.WriteLine("                            {0}", member.InStockSupplyQuantity);
                            }
                            if (member.IsSetEarliestAvailability())
                            {
                                //Console.WriteLine("                        EarliestAvailability");
                                Timepoint earliestAvailability = member.EarliestAvailability;
                                if (earliestAvailability.IsSetTimepointType())
                                {
                                    //Console.WriteLine("                            TimepointType");
                                    //Console.WriteLine("                                {0}", earliestAvailability.TimepointType);
                                }
                                if (earliestAvailability.IsSetDateTime())
                                {
                                    //Console.WriteLine("                            DateTime");
                                    //Console.WriteLine("                                {0}", earliestAvailability.DateTime);
                                }
                            }
                            if (member.IsSetSupplyDetail())
                            {
                                //Console.WriteLine("                        SupplyDetail");
                                InventorySupplyDetailList supplyDetail = member.SupplyDetail;
                                List<InventorySupplyDetail> member1List = supplyDetail.member;
                                foreach (InventorySupplyDetail member1 in member1List)
                                {
                                    //Console.WriteLine("                            member");
                                    if (member1.IsSetQuantity())
                                    {
                                        if (member1.SupplyType == "InStock")
                                            InStockCount += Int32.Parse(member1.Quantity.ToString());
                                        if (member1.SupplyType == "Inbound")
                                            InboundCount += Int32.Parse(member1.Quantity.ToString());
                                        if (member1.SupplyType == "Transfer")
                                            TransferCount += Int32.Parse(member1.Quantity.ToString());
                                        //InventoryRow[member1.SupplyType] = member1.Quantity;
                                        //Console.WriteLine("                                Quantity");
                                        //Console.WriteLine("                                    {0}", member1.Quantity);
                                    }
                                    if (member1.IsSetSupplyType())
                                    {
                                        //Console.WriteLine("                                SupplyType");
                                        //Console.WriteLine("                                    {0}", member1.SupplyType);
                                    }
                                    if (member1.IsSetEarliestAvailableToPick())
                                    {
                                        //Console.WriteLine("                                EarliestAvailableToPick");
                                        //Timepoint earliestAvailableToPick = member1.EarliestAvailableToPick;
                                        //if (earliestAvailableToPick.IsSetTimepointType())
                                        //{
                                        //    Console.WriteLine("                                    TimepointType");
                                        //    Console.WriteLine("                                        {0}", earliestAvailableToPick.TimepointType);
                                        //}
                                        //if (earliestAvailableToPick.IsSetDateTime())
                                        //{
                                        //    Console.WriteLine("                                    DateTime");
                                        //    Console.WriteLine("                                        {0}", earliestAvailableToPick.DateTime);
                                        //}
                                    }
                                    if (member1.IsSetLatestAvailableToPick())
                                    {
                                        //Console.WriteLine("                                LatestAvailableToPick");
                                        //Timepoint latestAvailableToPick = member1.LatestAvailableToPick;
                                        //if (latestAvailableToPick.IsSetTimepointType())
                                        //{
                                        //    Console.WriteLine("                                    TimepointType");
                                        //    Console.WriteLine("                                        {0}", latestAvailableToPick.TimepointType);
                                        //}
                                        //if (latestAvailableToPick.IsSetDateTime())
                                        //{
                                        //    Console.WriteLine("                                    DateTime");
                                        //    Console.WriteLine("                                        {0}", latestAvailableToPick.DateTime);
                                        //}
                                    }
                                }
                            }//end of foreach member1 list

                            InventoryRow["Inbound"] = InboundCount;
                            InventoryRow["InStock"] = InStockCount;
                            InventoryRow["Transfer"] = TransferCount;

                            InventorySupplyTable.Rows.Add(InventoryRow);
                        } // end of foreach member list


                    }
                    if (listInventorySupplyByNextTokenResult.IsSetNextToken())
                    {
                        Console.WriteLine("                NextToken");
                        Console.WriteLine("                    {0}", listInventorySupplyByNextTokenResult.NextToken);
                        request.NextToken = listInventorySupplyByNextTokenResult.NextToken;
                        InvokeListInventorySupplyByNextToken(service, request);
                    }
                }
                if (response.IsSetResponseMetadata())
                {
                    Console.WriteLine("            ResponseMetadata");
                    ResponseMetadata responseMetadata = response.ResponseMetadata;
                    if (responseMetadata.IsSetRequestId())
                    {
                        //Console.WriteLine("                RequestId");
                        //Console.WriteLine("                    {0}", responseMetadata.RequestId);
                    }
                }
            }
            catch (FBAInventoryServiceMWSException ex)
            {
                Console.WriteLine("Caught Exception: " + ex.Message);
                Console.WriteLine("Response Status Code: " + ex.StatusCode);
                Console.WriteLine("Error Code: " + ex.ErrorCode);
                Console.WriteLine("Error Type: " + ex.ErrorType);
                Console.WriteLine("Request ID: " + ex.RequestId);
                Console.WriteLine("XML: " + ex.XML);
            }

            return InventorySupplyDataSet;
        }
    }
}