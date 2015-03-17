<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true"
    CodeBehind="NetProfitSimPage.aspx.cs" Inherits="LabelPrintingInterface.NetProfitSimPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div align="center" style="width: 300px">
        <asp:Label ID="AccountLabel" runat="server" Text="Selected Merchant:"></asp:Label>
        <asp:CheckBoxList ID="AmazonAccountCheckBoxList" runat="server" DataSourceID="AmazonAccountDataSource"
            DataTextField="description" DataValueField="MerchantID" 
            OnSelectedIndexChanged="AmazonAccountCheckBoxList_SelectedIndexChanged" 
            AutoPostBack="True" ondatabound="AmazonAccountCheckBoxList_DataBound" 
            RepeatColumns="2" RepeatDirection="Horizontal">
        </asp:CheckBoxList>
        <asp:ObjectDataSource ID="AmazonAccountDataSource" runat="server" SelectMethod="GetAmazonAccountListByUserID"
            TypeName="LabelPrintingInterface.AmazonAccountDataSource">
            <SelectParameters>
                <asp:SessionParameter Name="sUserID" SessionField="UserID" Type="String" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <br />
        <asp:Label ID="SearchLabel" runat="server" Text="Enter Keyword To Search:"></asp:Label>
        <asp:TextBox ID="KeywordSearchTextBox" runat="server"></asp:TextBox>
        <asp:Button ID="SearchButton" runat="server" onclick="SearchButton_Click" 
            Text="Search" />
        &nbsp;&nbsp;
        <asp:Button ID="GenerateNewFileButton" runat="server" 
            onclick="GenerateNewFileButton_Click" Text="New File" />
        <br />
        <table style="padding-left: 25px; padding-right: 25px; text-align: center;">
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="false" BorderStyle="None"
                BorderWidth="0" EmptyDataText="-" GridLines="None" CellSpacing="3" OnRowDataBound="GridView1_RowDataBound"
                OnRowCommand="GridView1_RowCommand1" AllowPaging="true" PageSize="30">
                <HeaderStyle BackColor="#81DAF5" />
                <RowStyle Width="300px" HorizontalAlign="Center" />
                <AlternatingRowStyle BackColor="#EFEFEF" Width="300px" HorizontalAlign="Center" />
            </asp:GridView>
        </table>
        <br />
    </div>
</asp:Content>
