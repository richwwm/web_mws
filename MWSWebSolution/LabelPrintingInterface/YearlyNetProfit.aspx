<%@ Page Title="" Language="C#" Debug="true" MasterPageFile="~/Site1.Master" AutoEventWireup="true"
    CodeBehind="YearlyNetProfit.aspx.cs" Inherits="LabelPrintingInterface.Reports.YearlyNetProfit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <asp:Panel ID="Panel1" runat="server">
        </asp:Panel>
    </div>
    <asp:ObjectDataSource ID="NetProfitDataSource" runat="server" SelectMethod="GetAllSortedYearlySummaryData"
        TypeName="LabelPrintingInterface.NetProfitDataSource" OnSelecting="NetProfitDataSource_Selecting">
        <SelectParameters>
            <asp:SessionParameter DefaultValue="" Name="sortExpression" SessionField="SortedBy"
                Type="String" />
            <asp:ControlParameter ControlID="DropDownList2" Name="iYear" PropertyName="SelectedValue"
                Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="NetProfitYearPeriodDataSource1" runat="server" SelectMethod="GetNetProfitSummaryDataYearPeriod"
        TypeName="LabelPrintingInterface.NetProfitDataSource"></asp:ObjectDataSource>
    <asp:HiddenField runat="server" ID="_repostcheckcode" />
    <div align="center" style="width: 300px">
        <asp:Label ID="Label2" runat="server" Text="Yearly Net Profit Report:"></asp:Label>
    </div>
    <div align="center" style="width: 300px">
        <asp:Label ID="Label1" Text="Selected Year:" runat="server" Style="font-weight: 700" />
        <asp:DropDownList ID="DropDownList2" runat="server" DataSourceID="NetProfitYearPeriodDataSource1"
            DataTextField="date" DataValueField="date" AutoPostBack="True" OnTextChanged="DropDownList2_TextChanged"
            OnDataBinding="DropDownList2_DataBinding" 
            OnPreRender="DropDownList2_PreRender" ondatabound="DropDownList2_DataBound">
        </asp:DropDownList>

    <table style="padding-left: 25px; padding-right: 25px; text-align: center;">
  

                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" 
                    BorderStyle="None" BorderWidth=0 EmptyDataText="-" GridLines="None" 
                    CellSpacing="3" 
                    onrowdatabound="GridView1_RowDataBound" 
                    onrowcommand="GridView1_RowCommand1">
                    <HeaderStyle BackColor="#81DAF5" />
                    <RowStyle Width="300px" HorizontalAlign="Center" />
                    <AlternatingRowStyle BackColor="#EFEFEF" Width="300px" HorizontalAlign="Center"/>
                </asp:GridView>

    </table>
        </div>
    <br />
</asp:Content>
