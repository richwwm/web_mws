<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestPrint.aspx.cs" Inherits="LabelPrintingInterface.TestPrint" %>

<%@ Register assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
    </div>
    <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" 
        AutoDataBind="True" EnableDatabaseLogonPrompt="False" 
        EnableParameterPrompt="False" GroupTreeImagesFolderUrl="" 
        HasCrystalLogo="False" HasSearchButton="False" HasToggleGroupTreeButton="False" 
        HasToggleParameterPanelButton="False" Height="248px" PrintMode="ActiveX" 
        ReportSourceID="CrystalReportSource1" ReuseParameterValuesOnRefresh="True" 
        ShowAllPageIds="True" ToolbarImagesFolderUrl="" ToolPanelView="None" 
        ToolPanelWidth="200px" Width="666px" />
    <CR:CrystalReportSource ID="CrystalReportSource1" runat="server">
        <Report FileName="LabelReport2.rpt">
        </Report>
    </CR:CrystalReportSource>
    </form>
</body>
</html>
