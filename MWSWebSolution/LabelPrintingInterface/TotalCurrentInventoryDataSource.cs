﻿using System;
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

        public DataSet GetAllSortedData(string sortExpression)
        {
            if (sortExpression.Trim().Length == 0)
            {
                return GetAllData();
            }

            sortExpression = sortExpression.Replace("Ascending", "ASC");
            sortExpression = sortExpression.Replace("Descending", "DESC");

            string sSelectQuery = "SELECT [MWS].[dbo].[ProductAvailability].[FNSKU] AS " + '"' + "FNSKU" + '"'
                 + ",[inFlow].[dbo].[BASE_Product].[Name] AS " + '"' + "SellerSKU" + '"'
                 + ",[inFlow].[dbo].[BASE_Product].[Description] AS " + '"' + "ProductTitle" + '"'
                 + ",ISNULL([inFlow].[dbo].[Base_InventoryCost].[AverageCost],0) AS " + '"' + "Cost" + '"'
                 + ",[MWS].[dbo].[ProductAvailability].[Inbound] AS " + '"' + "Inbound" + '"'
                 + ",[MWS].[dbo].[ProductAvailability].[Fulfillable] AS " + '"' + "Fulfillable" + '"'
                 + "FROM [inFlow].[dbo].[BASE_Product] "
                 + "INNER JOIN [inFlow].[dbo].[BASE_InventoryCost] ON [inFlow].[dbo].[BASE_Product].[ProdId]=[inFlow].[dbo].[BASE_InventoryCost].[ProdId] "
                 + "INNER JOIN [MWS].[dbo].[ProductAvailability] ON [MWS].[dbo].[ProductAvailability].SellerSKU = [inFlow].[dbo].[BASE_Product].[Name] "
                 + "WHERE [inFlow].[dbo].[BASE_InventoryCost].[CurrencyId] = '8'"
                 + "ORDER BY " + sortExpression;

            return TotalCurrentInventoryReport.GetAllData(sSelectQuery);
        }

        public DataSet GetAllData()
        {
            string sSelectQuery = "SELECT [MWS].[dbo].[ProductAvailability].[FNSKU] AS " + '"' + "FNSKU" + '"'
                 + ",[inFlow].[dbo].[BASE_Product].[Name] AS " + '"' + "SellerSKU" + '"'
                 + ",ISNULL([inFlow].[dbo].[Base_InventoryCost].[AverageCost],0) AS " + '"' + "Cost" + '"'
                 + ",[MWS].[dbo].[ProductAvailability].[Inbound] AS " + '"' + "Inbound" + '"'
                 + ",[MWS].[dbo].[ProductAvailability].[Fulfillable] AS " + '"' + "Fulfillable" + '"'
                 + "FROM [inFlow].[dbo].[BASE_Product] "
                 + "INNER JOIN [inFlow].[dbo].[BASE_InventoryCost] ON [inFlow].[dbo].[BASE_Product].[ProdId]=[inFlow].[dbo].[BASE_InventoryCost].[ProdId] "
                 + "INNER JOIN [MWS].[dbo].[ProductAvailability] ON [MWS].[dbo].[ProductAvailability].SellerSKU = [inFlow].[dbo].[BASE_Product].[Name] "
                 + "WHERE [inFlow].[dbo].[BASE_InventoryCost].[CurrencyId] = '8'";
            return TotalCurrentInventoryReport.GetAllData(sSelectQuery);
        }

    }
}