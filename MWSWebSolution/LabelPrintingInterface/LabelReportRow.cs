using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LabelPrintingInterface
{
    public class LabelReportRow
    {
        private string _sLeftImg;

        public string sLeftImg
        {
            get { return _sLeftImg; }
            set { _sLeftImg = value; }
        }
        private string _sLeftFNSKU;

        public string sLeftFNSKU
        {
            get { return _sLeftFNSKU; }
            set { _sLeftFNSKU = value; }
        }
        private string _sLeftSellerSKU;

        public string sLeftSellerSKU
        {
            get { return _sLeftSellerSKU; }
            set { _sLeftSellerSKU = value; }
        }
        private string _sRightImg;

        public string sRightImg
        {
            get { return _sRightImg; }
            set { _sRightImg = value; }
        }
        private string _sRightFNSKU;

        public string sRightFNSKU
        {
            get { return _sRightFNSKU; }
            set { _sRightFNSKU = value; }
        }
        private string _sRightSellerSKU;

        public string sRightSellerSKU
        {
            get { return _sRightSellerSKU; }
            set { _sRightSellerSKU = value; }
        }

        public void EmptyLeftLabel()
        {
            _sLeftImg = "";
            _sLeftFNSKU = "";
            _sLeftSellerSKU = "";
        }

        public void EmptyRightLabel()
        {
            _sLeftImg = "";
            _sLeftFNSKU = "";
            _sLeftSellerSKU = "";
        }


        public LabelReportRow()
        {
            _sLeftImg = "";
            _sLeftFNSKU = "";
            _sLeftSellerSKU = "";
            _sRightImg = "";
            _sRightFNSKU = "";
            _sRightSellerSKU = "";
        }
        public LabelReportRow(string sLeftFNSKU, string sLeftImg, string sLeftSellerSKU)
        {
            _sLeftImg = sLeftImg;
            _sLeftFNSKU = sLeftFNSKU;
            _sLeftSellerSKU = sLeftSellerSKU;
            _sRightImg = "";
            _sRightFNSKU = "";
            _sRightSellerSKU = "";
        }

        public LabelReportRow(string sLeftFNSKU, string sLeftImg, string sLeftSellerSKU, string sRightFNSKU, string sRightImg, string sRightSellerSKU)
        {
            _sLeftImg = sLeftImg;
            _sLeftFNSKU = sLeftFNSKU;
            _sLeftSellerSKU = sLeftSellerSKU;
            _sRightImg = sRightImg;
            _sRightFNSKU = sRightFNSKU;
            _sRightSellerSKU = sRightSellerSKU;
        }
    }
}