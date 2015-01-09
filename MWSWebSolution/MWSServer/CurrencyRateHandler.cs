using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;

namespace MWSServer
{


    class CurrencyRateQuote
    {
        string sExchangeType;

        public string SExchangeType
        {
            get { return sExchangeType; }
            set { sExchangeType = value; }
        }
        string quotedDatetime;

        public string QuotedDatetime
        {
            get { return quotedDatetime; }
            set { quotedDatetime = value; }
        }
        double fRate;

        public double FRate
        {
            get { return fRate; }
            set { fRate = value; }
        }
    }

    class CurrencyRateHandler
    {
    }

    class YahooCurrenyRateHandler : CurrencyRateHandler
    {
        public YahooCurrenyRateHandler()
        {

        }
        string sClass = "SqlDataHandler.cs";
        string sLogPath = Directory.GetCurrentDirectory() + "\\";

        public List<CurrencyRateQuote> getAllCurrencyExchangeRate()
        {
            List<CurrencyRateQuote> listAllCurrencyQuote = new List<CurrencyRateQuote>();
            string webLink = "http://finance.yahoo.com/webservice/v1/symbols/allcurrencies/quote";
            WebRequest webQuery = WebRequest.Create(webLink);
            webQuery.Timeout = 1000 * 60 * 15;
            try
            {
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)webQuery;
                myHttpWebRequest.Timeout = 1000 * 60  * 15;
                myHttpWebRequest.Method = "GET";
                myHttpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; Windows NT 5.2; Windows NT 6.0; Windows NT 6.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727; MS-RTC LM 8; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; .NET CLR 4.0C; .NET CLR 4.0E)";
                WebResponse myWebResponse = myHttpWebRequest.GetResponse();

                using (Stream myStream = myWebResponse.GetResponseStream())
                {
                    using (StreamReader myStreamReader = new StreamReader(myStream))
                    {
                        string xmlString = myStreamReader.ReadToEnd();
                        string sCurrenyExchangeType = "";
                        string symbol;
                        string ts;
                        string type;
                        double fPrice = 0;
                        string quotedDate;

                        using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
                        {
                            while (reader.ReadToFollowing("resource"))
                            {
                                reader.ReadToFollowing("field");
                                sCurrenyExchangeType = reader.ReadElementContentAsString();
                                reader.ReadToFollowing("field");
                                fPrice = reader.ReadElementContentAsDouble();
                                reader.ReadToFollowing("field");
                                symbol = reader.ReadElementContentAsString();
                                reader.ReadToFollowing("field");
                                ts = reader.ReadElementContentAsString();
                                reader.ReadToFollowing("field");
                                type = reader.ReadElementContentAsString();
                                reader.ReadToFollowing("field");
                                quotedDate = reader.ReadElementContentAsString();

                                if (!string.IsNullOrEmpty(sCurrenyExchangeType) 
                                    && fPrice != 0
                                    && !string.IsNullOrEmpty(quotedDate)
                                    && sCurrenyExchangeType.IndexOf("/")>0
                                    )
                                {
                                    CurrencyRateQuote quoteRate = new CurrencyRateQuote();
                                    quoteRate.SExchangeType = sCurrenyExchangeType;
                                    quoteRate.FRate = fPrice;
                                    quoteRate.QuotedDatetime = quotedDate;
                                    listAllCurrencyQuote.Add(quoteRate);
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                DebugLogHandler.DebugLogHandler.WriteLog(sLogPath, sClass, " getAllCurrencyExchangeRate exception:" + ex.Message);
            }
            return listAllCurrencyQuote;
        }
    }
}
