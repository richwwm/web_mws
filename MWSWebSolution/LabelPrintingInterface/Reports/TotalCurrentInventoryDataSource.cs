using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace LabelPrintingInterface
{
    public class TotalCurrentInventoryDataSource
    {
        ReportHandler TotalCurrentInventoryReport;
        public TotalCurrentInventoryDataSource()
        {
            TotalCurrentInventoryReport = new ReportHandler();
        }

        private DataSet AddCalculatedColumn(DataSet originalDataSet, string columnName, string expression)
        {
            originalDataSet.Tables[0].Columns.Add(columnName, typeof(double), expression);

            return originalDataSet;
        }

        private DataSet FilterSortData(DataSet originalDataSet, string filter, string sortExpression)
        {
            DataSet sortDS = originalDataSet.Clone();
            DataRow[] drs = originalDataSet.Tables[0].Select(filter, sortExpression);

            foreach (DataRow dr in drs)
            {
                sortDS.Tables[0].ImportRow(dr);
            }

            return sortDS;
        }

        //public DataSet GetAllData()
        //{
        //    return GetAllSortedDataByMerchantID("");
        //}

        public DataSet GetAllSortedDataByMerchantID(string sortExpression,string sMerchantID)
        {
            return GetAllSortedData("");
        }

        public DataSet GetAllSortedData(string sortExpression)
        {
            string sSelectQuery = "SELECT [MWS].[dbo].[ProductAvailability].[FNSKU] AS " + '"' + "FNSKU" + '"'
                 + ",[inFlow].[dbo].[BASE_Product].[Name] AS " + '"' + "SellerSKU" + '"'
                 + ",[inFlow].[dbo].[BASE_Product].[Description] AS " + '"' + "ProductTitle" + '"'
                 + ",ISNULL([inFlow].[dbo].[Base_InventoryCost].[AverageCost],0) AS " + '"' + "Cost" + '"'
                 + ",[MWS].[dbo].[ProductAvailability].[Inbound] AS " + '"' + "Inbound" + '"'
                 + ",[MWS].[dbo].[ProductAvailability].[Fulfillable] AS " + '"' + "Fulfillable" + '"'
                 + ",[MWS].[dbo].[ProductAvailability].[MerchantID]"
                 + "FROM [inFlow].[dbo].[BASE_Product] "
                 + "INNER JOIN [inFlow].[dbo].[BASE_InventoryCost] ON [inFlow].[dbo].[BASE_Product].[ProdId]=[inFlow].[dbo].[BASE_InventoryCost].[ProdId] "
                 + "INNER JOIN [MWS].[dbo].[ProductAvailability] ON [MWS].[dbo].[ProductAvailability].SellerSKU = [inFlow].[dbo].[BASE_Product].[Name] "
                 + "WHERE [inFlow].[dbo].[BASE_InventoryCost].[CurrencyId] = '8'"
                 + "AND ( MerchantID ="  + "'" + sMerchantID + "')";

            /*if (!String.IsNullOrEmpty(sortExpression.Trim()))
            {
                sortExpression = sortExpression.Replace("Ascending", "ASC");
                sortExpression = sortExpression.Replace("Descending", "DESC");

                sSelectQuery += " ORDER BY " + sortExpression;
            }*/

            DataSet ds = TotalCurrentInventoryReport.GetAllData(sSelectQuery);
            ds = AddCalculatedColumn(ds, "InboundTotal", "Inbound * Cost");
            ds = AddCalculatedColumn(ds, "FulfillableTotal", "Fulfillable * Cost");
            ds = AddCalculatedColumn(ds, "SubTotal", "InboundTotal + FulfillableTotal");

            if (!String.IsNullOrEmpty(sortExpression.Trim()))
            {
                ds = FilterSortData(ds, "", sortExpression);
            }

            return ds;
        }

        public DataSet GetDataByCriteria(string parameter, string value)
        {
            string query = "SELECT [MWS].[dbo].[ProductAvailability].[FNSKU] AS " + '"' + "FNSKU" + '"'
                + ",[inFlow].[dbo].[BASE_Product].[Name] AS " + '"' + "SellerSKU" + '"'
                + ",[inFlow].[dbo].[BASE_Product].[Description] AS " + '"' + "" + '"'
                + ",ISNULL([inFlow].[dbo].[Base_InventoryCost].[AverageCost],0) AS " + '"' + "Cost" + '"'
                + ",[MWS].[dbo].[ProductAvailability].[Inbound] AS " + '"' + "Inbound" + '"'
                + ",[MWS].[dbo].[ProductAvailability].[Fulfillable] AS " + '"' + "Fulfillable" + '"'
                + "FROM [inFlow].[dbo].[BASE_Product] "
                + "INNER JOIN [inFlow].[dbo].[BASE_InventoryCost] ON [inFlow].[dbo].[BASE_Product].[ProdId]=[inFlow].[dbo].[BASE_InventoryCost].[ProdId] "
                + "INNER JOIN [MWS].[dbo].[ProductAvailability] ON [MWS].[dbo].[ProductAvailability].SellerSKU = [inFlow].[dbo].[BASE_Product].[Name] "
                + "WHERE [inFlow].[dbo].[BASE_InventoryCost].[CurrencyId] = '8'"
                + "AND (" + parameter + " LIKE '%'+@parameter+'%')";

            DataSet ds = TotalCurrentInventoryReport.GetDataByCriteria(query, value);
            ds = AddCalculatedColumn(ds, "InboundTotal", "Inbound * Cost");
            ds = AddCalculatedColumn(ds, "FulfillableTotal", "Fulfillable * Cost");
            ds = AddCalculatedColumn(ds, "SubTotal", "InboundTotal + FulfillableTotal");

            ds = FilterSortData(ds, "", "SellerSKU ASC");

            return ds;
        }
    }
}