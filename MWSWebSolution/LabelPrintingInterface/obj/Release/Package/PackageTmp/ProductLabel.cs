using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LabelPrintingInterface
{
    public class ProductLabel
    {
        private string _sFNSKU;
        private string _sImgPath;
        private string _sSellerSKU;
        public ProductLabel(string sFNSKU,string sImgPath,string sSellerSKU)
        {
            _sFNSKU = sFNSKU;
            _sImgPath = sImgPath;
            _sSellerSKU = sSellerSKU;
        }
        public string sSellerSKU
        {
            get { return _sSellerSKU; }
            set { _sSellerSKU = value; }
        }
        public string sImgPath
        {
            get { return _sImgPath; }
            set { _sImgPath = value; }
        }
        public string sFNSKU
        {
            get { return _sFNSKU; }
            set { _sFNSKU = value; }
        }
    }
}