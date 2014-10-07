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

        public DataSet GetAllData()
        {
            string sSelectQuery = "select [MWS].[dbo].[ProductAvailability].[FNSKU] as " +'"'+"SKU"+'"'
                +",[inFlow].[dbo].[BASE_Product].[Name] as "+'"'+"Product Name"+'"'
                +",[inFlow].[dbo].[BASE_ItemPrice].[UnitPrice] as "+'"'+"Cost"+'"'
                +",[MWS].[dbo].[ProductAvailability].[Inbound] as "+'"'+"Inbound"+'"'
                + ",[MWS].[dbo].[ProductAvailability].[Fulfillable] as " + '"' + "Fulfillable" + '"'
                +"FROM [inFlow].[dbo].[BASE_Product] inner join [inFlow].[dbo].[BASE_ItemPrice] ON [inFlow].[dbo].[BASE_Product].[ProdId]=[inFlow].[dbo].[BASE_ItemPrice].[ProdId] inner join [MWS].[dbo].[ProductAvailability] on [MWS].[dbo].[ProductAvailability].SellerSKU = [inFlow].[dbo].[BASE_Product].[Name] where [inFlow].[dbo].[BASE_ItemPrice].[PricingSchemeId] = '100'";
            return TotalCurrentInventoryReport.GetAllData(sSelectQuery);
        }

    }
}