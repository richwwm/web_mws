using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using DebugLogHandler;
using System.IO;
using MWSUser;
namespace MWSServer
{
    class SqlDataHandler
    {
        string _connectionString;
        string _sClass = "SqlDataHandler.cs";
        string _sLogPath = Directory.GetCurrentDirectory() + "\\";
        public SqlDataHandler(string sConnectionString)
        {
            _connectionString = sConnectionString;
        }

        public List<MWSUserProfile> GetAWSLoginProfile()
        {
            List<MWSUserProfile> profileList = new List<MWSUserProfile>();
            SqlCommand cmd;
            string query;

            query = "SELECT * FROM [MWS].[dbo].[AWSLoginToken] order by MerchantID DESC";
            cmd = new SqlCommand(query);

            DataTable dTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.Connection = conn;
                        sda.SelectCommand = cmd;
                        using (DataSet ds = new DataSet())
                        {
                            sda.Fill(dTable);
                        }
                    }
                }
            }

            foreach (DataRow row in dTable.Rows)
            {
                string sAccessKeyID = row["AWSAccessKeyID"].ToString();
                string sSecretKey = row["SecretKey"].ToString();
                string sMerchantID = row["MerchantID"].ToString();
                string sMarketplaceID = row["MarketplaceID"].ToString();
                MWSUserProfile profile = new MWSUserProfile(sAccessKeyID, sSecretKey, sMarketplaceID, sMerchantID, "");
                profileList.Add(profile);
            }

            return profileList;
        }

        public int UpdateInventoryCostHistory()
        {
            int result = -1;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateInventoryCostHistory"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@STARTDATE", new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day - 1, 23, 59, 59));
                    cmd.Parameters.AddWithValue("@ENDDATE", new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 23, 59, 59));
                    cmd.Connection = con;
                    con.Open();
                    result = (Int32)cmd.ExecuteScalar();
                    con.Close();
                }
            }
            return result;
        }

        public string GetCurrencyID(string sSymbol)
        {
            string sCurrencyID = "";
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetCurrencyID"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CurrencySymbol", sSymbol);
                    cmd.Connection = con;
                    con.Open();
                    sCurrencyID = cmd.ExecuteScalar().ToString();
                    con.Close();
                }
            }
            return sCurrencyID;
        }

        public int UpdateNetProfitData(DateTime startDate, DateTime endDate)
        {
            int iResult = -1;
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateNetProfitData"))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);
                    cmd.Connection = con;
                    con.Open();
                    iResult = (Int32)cmd.ExecuteScalar();
                    con.Close();
                }
            }
            return iResult;
        }


        public void UpdateCurrencyExchangeRateHistory(string sTableName, DataSet dsTemp)
        {
            string insertQueryString = "INSERT INTO " + sTableName + " (FromCurrencyID,ToCurrencyID,Rate,Date) " +
        " VALUES (@FromCurrencyID,@ToCurrencyID,@Rate,@Date)";

            string updateQueryString = "UPDATE " + sTableName + " SET Rate = @Rate" +
" WHERE FromCurrencyID = @FromCurrencyID and [ToCurrencyID] = @ToCurrencyID and Date = @Date";
            SqlDataAdapter adapter = new SqlDataAdapter();
            using (SqlConnection connection =
            new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    foreach (DataRow row in dsTemp.Tables[0].Rows)
                    {
                        SqlCommand command = new SqlCommand(updateQueryString, connection);
                        foreach (DataColumn col in dsTemp.Tables[0].Columns)
                        {
                            string paraName = "@" + col.ColumnName;
                            paraName = paraName.Replace('-', '_');
                            object paraVal = row[col.ColumnName];
                            command.Parameters.AddWithValue(paraName, paraVal);
                        }

                        int result = command.ExecuteNonQuery(); //number of rows returned.
                        if (result == 0) //no data updated , thus insert new data to database;
                        {
                            SqlCommand insertCommand = new SqlCommand(insertQueryString, connection);
                            foreach (DataColumn col in dsTemp.Tables[0].Columns)
                            {
                                string paraName = "@" + col.ColumnName;
                                paraName = paraName.Replace('-', '_');
                                object paraVal = row[col.ColumnName];
                                insertCommand.Parameters.AddWithValue(paraName, paraVal);
                            }
                            result = insertCommand.ExecuteNonQuery();
                        }
                    }

                    connection.Close();
                }
                catch (Exception ex)
                {
                    DebugLogHandler.DebugLogHandler.WriteLog(_sLogPath, _sClass, "UpdateCurrencyExchangeRateHistory() exception message: " + ex.Message);
                }
            }
        }

        public void UpdateRaw_FBA_EstimatedFee(string sTableName, DataSet dsTemp)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            string insertQueryString = "INSERT INTO " + sTableName + " (sku,fnsku,asin,product_name,product_group,brand,fulfilled_by,your_price,sales_price,longest_side,median_side,shortest_side,length_and_girth,unit_of_dimension,item_package_weight,unit_of_weight,product_size_tier,currency,estimated_fee,estimated_referral_fee_per_unit,estimated_variable_closing_fee,estimated_order_handling_fee_per_order,estimated_pick_pack_fee_per_unit,estimated_weight_handling_fee_per_unit,MerchantID,last_update_date) " +
        " VALUES (@sku,@fnsku,@asin,@product_name,@product_group,@brand,@fulfilled_by,@your_price,@sales_price,@longest_side,@median_side,@shortest_side,@length_and_girth,@unit_of_dimension,@item_package_weight,@unit_of_weight,@product_size_tier,@currency,@estimated_fee,@estimated_referral_fee_per_unit,@estimated_variable_closing_fee,@estimated_order_handling_fee_per_order,@estimated_pick_pack_fee_per_unit,@estimated_weight_handling_fee_per_unit,@MerchantID,@last_update_date)";

            string updateQueryString = "UPDATE " + sTableName + " SET sku = @sku , fnsku = @fnsku, asin = @asin, product_name = @product_name, product_group = @product_group, brand = @brand, fulfilled_by = @fulfilled_by, your_price = @your_price, sales_price = @sales_price, longest_side = @longest_side, median_side = @median_side, shortest_side = @shortest_side, length_and_girth = length_and_girth, unit_of_dimension = @unit_of_dimension, item_package_weight = @item_package_weight, unit_of_weight = @unit_of_weight, product_size_tier = @product_size_tier, currency = @currency, estimated_fee = @estimated_fee, estimated_referral_fee_per_unit = @estimated_referral_fee_per_unit, estimated_variable_closing_fee = @estimated_variable_closing_fee, estimated_order_handling_fee_per_order = @estimated_order_handling_fee_per_order, estimated_pick_pack_fee_per_unit = @estimated_pick_pack_fee_per_unit, estimated_weight_handling_fee_per_unit=@estimated_weight_handling_fee_per_unit" +
" WHERE fnsku = @fnsku and [MerchantID] = @MerchantID and last_update_date = @last_update_date";

            using (SqlConnection connection =
            new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    foreach (DataRow row in dsTemp.Tables[0].Rows)
                    {
                        SqlCommand command = new SqlCommand(updateQueryString, connection);
                        foreach (DataColumn col in dsTemp.Tables[0].Columns)
                        {
                            string paraName = "@" + col.ColumnName;
                            paraName = paraName.Replace('-', '_');
                            object paraVal = row[col.ColumnName];
                            command.Parameters.AddWithValue(paraName, paraVal);
                        }

                        int result = command.ExecuteNonQuery(); //number of rows returned.
                        if (result == 0) //no data updated , thus insert new data to database;
                        {
                            SqlCommand insertCommand = new SqlCommand(insertQueryString, connection);
                            foreach (DataColumn col in dsTemp.Tables[0].Columns)
                            {
                                string paraName = "@" + col.ColumnName;
                                paraName = paraName.Replace('-', '_');
                                object paraVal = row[col.ColumnName];
                                insertCommand.Parameters.AddWithValue(paraName, paraVal);
                            }
                            result = insertCommand.ExecuteNonQuery();
                        }
                    }

                    connection.Close();
                }
                catch (Exception ex)
                {
                    DebugLogHandler.DebugLogHandler.WriteLog(_sLogPath, _sClass, "UpdateRaw_FBA_EstimatedFee() exception message: " + ex.Message);
                }
            }

        }

        public void UpdateRawSettlementPaymentData(DataSet dsTemp)
        {
            int iRowsAffected = 0;
            int iInsertedRows = 0;
            int iDeletedRows = 0;
            using (SqlConnection connection =
            new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    if (dsTemp.Tables[0].Rows.Count > 0)
                    {
                        DataRow firstRow = dsTemp.Tables[0].Rows[0];
                        SqlCommand command = new SqlCommand("DeleteSettlementPaymentData", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        foreach (DataColumn col in dsTemp.Tables[0].Columns)
                        {
                            string paraName = "@" + col.ColumnName;
                            object paraVal = firstRow[col.ColumnName];
                            command.Parameters.AddWithValue(paraName, paraVal);
                        }
                        command.Connection = connection;
                        iDeletedRows = (Int32)command.ExecuteScalar();
                    }
                    foreach (DataRow row in dsTemp.Tables[0].Rows)
                    {
                        SqlCommand insertCommand = new SqlCommand("AddRawSettlementData", connection);
                        insertCommand.CommandType = CommandType.StoredProcedure;
                        foreach (DataColumn col in dsTemp.Tables[0].Columns)
                        {
                            string paraName = "@" + col.ColumnName;
                            object paraVal = row[col.ColumnName];
                            insertCommand.Parameters.AddWithValue(paraName, paraVal);
                        }
                        insertCommand.Connection = connection;

                        iRowsAffected = (Int32)insertCommand.ExecuteScalar();
                        if (iRowsAffected > 0)
                        {
                            iInsertedRows = iInsertedRows + iRowsAffected;
                        }

                    }
                    DebugLogHandler.DebugLogHandler.WriteLog(_sLogPath, _sClass, "UpdateRawSettlementPaymentData() Call AddRawSettlementData stored procedure rows affected : " + iInsertedRows);
                    DebugLogHandler.DebugLogHandler.WriteLog(_sLogPath, _sClass, "UpdateRawSettlementPaymentData() Call DeleteSettlementPaymentData stored procedure rows affected : " + iDeletedRows);

                    connection.Close();
                }
                catch (Exception ex)
                {
                    DebugLogHandler.DebugLogHandler.WriteLog(_sLogPath, _sClass, "UpdateRawSettlementPaymentData() exception message: " + ex.Message);
                }
            }

        }

        public void UpdateRawFulfilledShipmentsData(DataSet dsTemp)
        {
            int iRowsAffected = 0;
            int iInsertedRows = 0;
            int iUpdatedRows = 0;
            using (SqlConnection connection =
            new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    foreach (DataRow row in dsTemp.Tables[0].Rows)
                    {
                        SqlCommand command = new SqlCommand("UpdateFulfilledShipmentsData", connection);
                        command.CommandType = CommandType.StoredProcedure;
                        foreach (DataColumn col in dsTemp.Tables[0].Columns)
                        {
                            string paraName = "@" + col.ColumnName;
                            object paraVal = row[col.ColumnName];
                            command.Parameters.AddWithValue(paraName, paraVal);
                        }
                        command.Connection = connection;

                        iRowsAffected = (Int32)command.ExecuteScalar();
                        if (iRowsAffected > 0)
                        {
                            iUpdatedRows = iUpdatedRows + iRowsAffected;
                        }
                        else
                        {
                            SqlCommand insertCommand = new SqlCommand("AddFulfilledShipmentsData", connection);
                            insertCommand.CommandType = CommandType.StoredProcedure;
                            foreach (DataColumn col in dsTemp.Tables[0].Columns)
                            {
                                string paraName = "@" + col.ColumnName;
                                object paraVal = row[col.ColumnName];
                                insertCommand.Parameters.AddWithValue(paraName, paraVal);
                            }
                            insertCommand.Connection = connection;

                            iRowsAffected = (Int32)insertCommand.ExecuteScalar();
                            if (iRowsAffected > 0)
                            {
                                iInsertedRows = iInsertedRows + iRowsAffected;
                            }

                        }
                    }
                    DebugLogHandler.DebugLogHandler.WriteLog(_sLogPath, _sClass, "UpdateRawFulfilledShipmentsData() Call AddFulfilledShipmentsData stored procedure rows affected : " + iInsertedRows);
                    DebugLogHandler.DebugLogHandler.WriteLog(_sLogPath, _sClass, "UpdateRawFulfilledShipmentsData() Call UpdateFulfilledShipmentsData stored procedure rows affected : " + iUpdatedRows);

                    connection.Close();
                }
                catch (Exception ex)
                {
                    DebugLogHandler.DebugLogHandler.WriteLog(_sLogPath, _sClass, "UpdateRawFulfilledShipmentsData() exception message: " + ex.Message);
                }
            }

        }



        public void UpdateRawOrderData(string sTableName, DataSet dsTemp)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            string insertQueryString = "INSERT INTO " + sTableName + " ([amazon-order-id],[merchant-order-id],[purchase-date],[last-updated-date],[order-status],[fulfillment-channel],[sales-channel],[order-channel],[url],[ship-service-level],[product-name],[sku],[asin],[item-status],[quantity],[currency],[item-price],[item-tax],[shipping-price],[shipping-tax],[gift-wrap-price],[gift-wrap-tax],[item-promotion-discount],[ship-promotion-discount],[ship-city],[ship-state],[ship-postal-code],[ship-country],[promotion-ids],[MerchantID]) " +
        " VALUES (@amazon_order_id,@merchant_order_id,@purchase_date,@last_updated_date,@order_status,@fulfillment_channel,@sales_channel,@order_channel,@url,@ship_service_level,@product_name,@sku,@asin,@item_status,@quantity,@currency,@item_price,@item_tax,@shipping_price,@shipping_tax,@gift_wrap_price,@gift_wrap_tax,@item_promotion_discount,@ship_promotion_discount,@ship_city,@ship_state,@ship_postal_code,@ship_country,@promotion_ids,@MerchantID)";

            string updateQueryString = "UPDATE " + sTableName + " SET [amazon-order-id]= @amazon_order_id, [merchant-order-id] = @merchant_order_id,  [purchase-date] = @purchase_date,  [last-updated-date] = @last_updated_date, [order-status] = @order_status , [fulfillment-channel] = @fulfillment_channel, [sales-channel] = @sales_channel, [order-channel]=@order_channel, [url] = @url, [ship-service-level]=@ship_service_level, [product-name] = @product_name, [sku]=@sku, [asin]=@asin, [item-status]=@item_status, [quantity]=@quantity, [currency]=@currency, [item-price]=@item_price, [item-tax]=@item_tax, [shipping-price]=@shipping_price, [shipping-tax]=@shipping_tax, [gift-wrap-price]=@gift_wrap_price, [item-promotion-discount]=@item_promotion_discount, [ship-promotion-discount]=@ship_promotion_discount, [ship-city]=@ship_city, [ship-state]=@ship_state, [ship-postal-code]=@ship_postal_code, [ship-country]=@ship_country, [promotion-ids]=@promotion_ids" +
" WHERE [amazon-order-id] = @amazon_order_id and [MerchantID] = @MerchantID";

            using (SqlConnection connection =
            new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    foreach (DataRow row in dsTemp.Tables[0].Rows)
                    {
                        SqlCommand command = new SqlCommand(updateQueryString, connection);
                        foreach (DataColumn col in dsTemp.Tables[0].Columns)
                        {
                            string paraName = "@" + col.ColumnName;
                            paraName = paraName.Replace('-', '_');
                            string paraVal = row[col.ColumnName].ToString();
                            command.Parameters.AddWithValue(paraName, paraVal);
                        }

                        int result = command.ExecuteNonQuery(); //number of rows returned.
                        if (result == 0) //no data updated , thus insert new data to database;
                        {
                            SqlCommand insertCommand = new SqlCommand(insertQueryString, connection);
                            foreach (DataColumn col in dsTemp.Tables[0].Columns)
                            {
                                string paraName = "@" + col.ColumnName;
                                paraName = paraName.Replace('-', '_');
                                string paraVal = row[col.ColumnName].ToString();
                                insertCommand.Parameters.AddWithValue(paraName, paraVal);
                            }
                            result = insertCommand.ExecuteNonQuery();
                        }
                    }

                    connection.Close();
                }
                catch (Exception ex)
                {
                    DebugLogHandler.DebugLogHandler.WriteLog(_sLogPath, _sClass, "UpdateRaw_OrderData() exception message: " + ex.Message);
                }
            }

        }

        public void UpdateProductAvailabilityData(string sTableName, DataSet dsTemp,string sMerchantID)
        {
            SqlDataAdapter adapter = new SqlDataAdapter();
            string insertQueryString = "INSERT INTO " + sTableName + " (FNSKU, SellerSKU,ASIN,ProductName,Inbound,Fulfillable,Unfulfillable,Reserved,Last_update,MerchantID) " +
        " VALUES (@FNSKU, @SellerSKU,@ASIN,@ProductName,@Inbound,@Fulfillable,@Unfulfillable,@Reserved,@Last_update,@MerchantID)";

            int iDeletedRows = 0;

            int result = 0;
            using (SqlConnection connection =
            new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    SqlCommand deleteCommand = new SqlCommand("DeleteProductAvailabilityData", connection);
                    deleteCommand.CommandType = CommandType.StoredProcedure;
                    deleteCommand.Parameters.AddWithValue("MerchantID", sMerchantID);
                    deleteCommand.Connection = connection;
                    iDeletedRows = (Int32)deleteCommand.ExecuteScalar();
                    foreach (DataRow row in dsTemp.Tables[0].Rows)
                    {
                        SqlCommand insertCommand = new SqlCommand(insertQueryString, connection);
                        insertCommand.Parameters.AddWithValue("@FNSKU", row["FNSKU"].ToString());
                        insertCommand.Parameters.AddWithValue("@SellerSKU", row["SellerSKU"].ToString());
                        insertCommand.Parameters.AddWithValue("@ASIN", row["ASIN"].ToString());
                        insertCommand.Parameters.AddWithValue("@ProductName", row["ProductName"].ToString());
                        insertCommand.Parameters.AddWithValue("@Inbound", row["Inbound"].ToString());
                        insertCommand.Parameters.AddWithValue("@Fulfillable", row["Fulfillable"].ToString());
                        insertCommand.Parameters.AddWithValue("@Unfulfillable", row["Unfulfillable"].ToString());
                        insertCommand.Parameters.AddWithValue("@Reserved", row["Reserved"].ToString());
                        insertCommand.Parameters.AddWithValue("@Last_update", DateTime.Now);
                        insertCommand.Parameters.AddWithValue("@MerchantID", row["MerchantID"].ToString());
                        result = insertCommand.ExecuteNonQuery();

                    }

                    connection.Close();
                }
                catch (Exception ex)
                {
                    DebugLogHandler.DebugLogHandler.WriteLog(_sLogPath, _sClass, "UpdateProductAvailabilityData() exception message: " + ex.Message);
                }
            }

        }


    }
}
