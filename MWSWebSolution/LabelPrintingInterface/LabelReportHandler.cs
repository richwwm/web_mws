using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Drawing.Printing;
using CrystalDecisions.CrystalReports;
using CrystalDecisions.CrystalReports.Engine;
using System.Drawing;
using System.IO;
using DebugLogHandler;
using System.Collections;

namespace LabelPrintingInterface
{
    public class LabelReportHandler
    {
        private DataSet labelDataSet;
        static string sLogPath = HttpContext.Current.Server.MapPath("~/");
        public LabelReportHandler(DataSet LabelDataSet)
        {
            this.labelDataSet = LabelDataSet;
        }
        private string sCrystalReportPath;

        public void GenerateLabelImg(string barcode)
        {

            try
            {
                DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, "LabelReportHandler", "GenerateLabelImg started");
                string savePath = HttpContext.Current.Server.MapPath(@"Images\" + barcode + ".jpg");
                BarcodeLib.Barcode barcodeLabel = new BarcodeLib.Barcode();
                barcodeLabel.Alignment = BarcodeLib.AlignmentPositions.CENTER;
                BarcodeLib.TYPE type = BarcodeLib.TYPE.CODE128A;
                barcodeLabel.RotateFlipType = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), "RotateNoneFlipNone", true);
                barcodeLabel.LabelPosition = BarcodeLib.LabelPositions.BOTTOMCENTER;
                System.Drawing.Image barcodeImage = barcodeLabel.Encode(type, barcode, Color.Black, Color.White, 300, 150);
                barcodeLabel.SaveImage(savePath, BarcodeLib.SaveTypes.JPG);

            }
            catch (Exception ex)
            {
                DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, "LabelReportHandler", ex.Message);
            }
        }
        public void PrintLabel()
        {
            try
            {
                ArrayList labelReportArray = new ArrayList();
                sCrystalReportPath = HttpContext.Current.Server.MapPath("~/LabelReport2.rpt");
                ReportDocument rpt = new ReportDocument();
                rpt.Load(sCrystalReportPath);
                string sImgPath = HttpContext.Current.Server.MapPath("~/Images/");
                //check label exist or not , if not generate img first
                foreach (DataRow rowImg in this.labelDataSet.Tables[0].Rows)
                {

                    string sTempFNSKU = rowImg["FNSKU"].ToString();
                    string sTempImgPath = sImgPath + sTempFNSKU + ".jpg";

                    if (!File.Exists(sTempImgPath))
                    {
                        DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, "LabelReportHandler", "PrintLabel() " + sTempFNSKU + " image not exsits");
                        GenerateLabelImg(sTempFNSKU);
                    }
                    else
                    {
                        DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, "LabelReportHandler", "PrintLabel() " + sTempFNSKU + " image exsits");
                    }
                }

                DataTable printLabelTable = this.labelDataSet.Tables[0];
                if (printLabelTable != null && printLabelTable.Rows.Count > 0)
                {
                    string sLeftImgPath = "";
                    string sRightImgPath = "";
                    int i = 0;
                    LabelReportRow labelRow = new LabelReportRow();
                    while (i < printLabelTable.Rows.Count)
                    {
                        DataRow tempRow = printLabelTable.Rows[i];
                        int QuantityForThisRow = Int32.Parse(tempRow["PrintQuantity"].ToString());
                        int iCopiesToPrint = 0;
                        if (labelRow.sLeftFNSKU == "")
                        {
                            iCopiesToPrint = QuantityForThisRow / 2;
                            if (iCopiesToPrint != 0)
                            {
                                sLeftImgPath = sImgPath + tempRow["FNSKU"] + ".jpg";
                                labelRow.sLeftFNSKU = tempRow["FNSKU"].ToString() ;
                                labelRow.sLeftImg =  sLeftImgPath ;
                                labelRow.sLeftSellerSKU = "KM *" + tempRow["SellerSKU"].ToString();
                                labelRow.sRightFNSKU = labelRow.sLeftFNSKU;
                                labelRow.sRightImg = labelRow.sLeftImg;
                                labelRow.sRightSellerSKU = labelRow.sLeftSellerSKU;

                                for (int k = 0; k < iCopiesToPrint; k++)
                                {
                                    labelReportArray.Add(labelRow);
                                }

                                labelRow = new LabelReportRow();

                                QuantityForThisRow = QuantityForThisRow - (iCopiesToPrint * 2);
                            }
                            if (QuantityForThisRow == 1)
                            {
                                sLeftImgPath = sImgPath + tempRow["FNSKU"] + ".jpg";
                                labelRow = new LabelReportRow(tempRow["FNSKU"].ToString() ,  sLeftImgPath , "KM *" + tempRow["SellerSKU"].ToString() );

                                if (i + 1 == printLabelTable.Rows.Count) // this is last data row , print it out 
                                {
                                    QuantityForThisRow = QuantityForThisRow - 1;
                                    labelReportArray.Add(labelRow);
                                }
                            }
                        }
                        else
                        {
                            //Since the previous Left label havent printed, so save to right label and print it out
                            iCopiesToPrint = 1;
                            if (QuantityForThisRow > 0)
                            {
                            
                                sRightImgPath = sImgPath + tempRow["FNSKU"] + ".jpg";
                                string sRightImg = sRightImgPath ;
                                string sRightFNSKU =  tempRow["FNSKU"].ToString() ;
                                string sRightSellerSKU =  "KM *" + tempRow["SellerSKU"].ToString() ;
                                labelRow.sRightFNSKU = sRightFNSKU;
                                labelRow.sRightImg = sRightImg;
                                labelRow.sRightSellerSKU = sRightSellerSKU;

                                labelReportArray.Add(labelRow);
                                QuantityForThisRow = QuantityForThisRow - 1;

                                labelRow = new LabelReportRow();
                            }
                            iCopiesToPrint = QuantityForThisRow / 2;
                            if (iCopiesToPrint != 0)
                            {
                                sLeftImgPath = sImgPath + tempRow["FNSKU"] + ".jpg";
                                labelRow.sLeftFNSKU = tempRow["FNSKU"].ToString() ;
                                labelRow.sLeftImg = sLeftImgPath ;
                                labelRow.sLeftSellerSKU =  "KM *" + tempRow["SellerSKU"].ToString() ;
                                labelRow.sRightFNSKU = labelRow.sLeftFNSKU;
                                labelRow.sRightImg = labelRow.sLeftImg;
                                labelRow.sRightSellerSKU = labelRow.sLeftSellerSKU;

                                iCopiesToPrint = QuantityForThisRow / 2;
                                rpt.Refresh();
                                for (int k = 0; k < iCopiesToPrint; k++)
                                {
                                    labelReportArray.Add(labelRow);
                                }
                                labelRow = new LabelReportRow();
                                

                                QuantityForThisRow = QuantityForThisRow - (iCopiesToPrint * 2);
                            }
                            if (QuantityForThisRow == 1)
                            {
                                sLeftImgPath = sImgPath + tempRow["FNSKU"] + ".jpg";
                                labelRow = new LabelReportRow(tempRow["FNSKU"].ToString() ,  sLeftImgPath , "KM *" + tempRow["SellerSKU"].ToString() );

                                if (i + 1 == printLabelTable.Rows.Count) // this is last data row , print it out 
                                {
                                    QuantityForThisRow = QuantityForThisRow - 1;
                                    labelReportArray.Add(labelRow);
                                }
                            }
                        }
                        i++;
                    }

                }

                rpt.SetDataSource(labelReportArray);
                if (labelReportArray.Count > 0)
                {
                    foreach (LabelReportRow row in labelReportArray)
                    {
                        DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, "LabelReportHandler", "PrintLabel() " + "Left Label " + row.sLeftFNSKU + " "+ row.sLeftSellerSKU);
                        DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, "LabelReportHandler", "PrintLabel() " + "Right Label " + row.sRightFNSKU + " " + row.sRightSellerSKU);
                    }
                    rpt.ExportToHttpResponse(
                        CrystalDecisions.Shared.ExportFormatType.PortableDocFormat,
                        System.Web.HttpContext.Current.Response,
                        true,
                        "LabelReport" + DateTime.Now);

                }
                //rpt.PrintToPrinter(1, false, 0, 0); 
            }
            catch (Exception ex)
            {
                DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, "LabelReportHandler", "PrintLabel()" + ex.Message);
            }
        }

    }
}