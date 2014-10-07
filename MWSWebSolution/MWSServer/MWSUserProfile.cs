using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MWSUser
{
    class MWSUserProfile
    {
        private string sAccessKeyId;

        public string SAccessKeyId
        {
            get { return sAccessKeyId; }
            set { sAccessKeyId = value; }
        }
        private string sSecretAccessKey;

        public string SSecretAccessKey
        {
            get { return sSecretAccessKey; }
            set { sSecretAccessKey = value; }
        }
        private string sMarketplaceId;

        public string SMarketplaceId
        {
            get { return sMarketplaceId; }
            set { sMarketplaceId = value; }
        }
        private string sSellerId;

        public string SSellerId
        {
            get { return sSellerId; }
            set { sSellerId = value; }
        }
        private string sServiceURL;

        public string SServiceURL
        {
            get { return sServiceURL; }
            set { sServiceURL = value; }
        }

        public MWSUserProfile(string sAccessKeyId, string sSecretAccessKey, string sMarketplaceId, string sSellerId, string sServiceURL)
        {
            SAccessKeyId = sAccessKeyId;
            SSecretAccessKey = sSecretAccessKey;
            SMarketplaceId = sMarketplaceId;
            SSellerId = sSellerId;
            SServiceURL = sServiceURL;
        }


    }
}
