using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LabelPrintingInterface
{
    /// <summary>
    /// Summary description for DownloadFile
    /// </summary>
    public class DownloadFile : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Clear();
            context.Response.ContentType = "application/octet-stream";
            context.Response.AddHeader("Content-Disposition", "attachment; filename=LabelPrintServiceSetup.msi");
            context.Response.WriteFile(context.Server.MapPath("LabelPrintServiceSetup.msi"));
            context.Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}