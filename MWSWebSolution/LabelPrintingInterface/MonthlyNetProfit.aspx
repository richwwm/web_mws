<%@ Page Title="" Language="C#" Debug="true" MasterPageFile="~/Site1.Master" AutoEventWireup="true"
    CodeBehind="MonthlyNetProfit.aspx.cs" Inherits="LabelPrintingInterface.Reports.MonthlyNetProfit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <asp:Panel ID="Panel1" runat="server">
        </asp:Panel>
    </div>
    <asp:ObjectDataSource ID="NetProfitDataSource" runat="server" SelectMethod="GetAllSortedMonthlyDataByMerchantID"
        TypeName="LabelPrintingInterface.NetProfitDataSource" OnSelecting="NetProfitDataSource_Selecting">
        <SelectParameters>
            <asp:SessionParameter DefaultValue="" Name="sortExpression" SessionField="SortedBy"
                Type="String" />
            <asp:ControlParameter ControlID="DropDownList1" DefaultValue="" Name="sMerchantID"
                PropertyName="SelectedValue" Type="String" />
            <asp:ControlParameter ControlID="DropDownList3" Name="iYear" 
                PropertyName="SelectedValue" Type="Int32" />
            <asp:ControlParameter ControlID="DropDownList2" Name="iMonth" 
                PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="AmazonAccountDataSource" runat="server" SelectMethod="GetAmazonAccountListByUserID"
        TypeName="LabelPrintingInterface.AmazonAccountDataSource" OnSelecting="AmazonAccountDataSource2_Selecting">
        <SelectParameters>
            <asp:SessionParameter Name="sUserID" SessionField="UserID" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="NetProfitYearPeriodDataSource1" runat="server" SelectMethod="GetYearPeriodInNetProfitDataByMerchantID"
        TypeName="LabelPrintingInterface.NetProfitDataSource">
        <SelectParameters>
            <asp:ControlParameter ControlID="DropDownList1" Name="sMerchantID" PropertyName="SelectedValue"
                Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="NetProfitMonthPeriodDataSource1" runat="server" SelectMethod="GetMonthPeriodInNetProfitDataByMerchantID"
        TypeName="LabelPrintingInterface.NetProfitDataSource">
        <SelectParameters>
            <asp:ControlParameter ControlID="DropDownList1" Name="sMerchantID" PropertyName="SelectedValue"
                Type="String" />
            <asp:ControlParameter ControlID="DropDownList3" Name="iYear" 
                PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:HiddenField runat="server" ID="_repostcheckcode" />
    <div align="center" style="width: 600px">
        <asp:Label ID="Label2" runat="server" Text="Monthly Net Profit Report:"></asp:Label>
    </div>
    <div align="center" style="width: 600px">
        <asp:Label Text="Viewing Amazon account:" runat="server" Style="font-weight: 700" />
        <asp:DropDownList ID="DropDownList1" runat="server" Height="17px" Width="193px" DataSourceID="AmazonAccountDataSource"
            AutoPostBack="True" DataTextField="description" 
            DataValueField="MerchantID" OnTextChanged="DropDownList1_TextChanged" 
            ondatabound="DropDownList1_DataBound">
        </asp:DropDownList>
        </div>
        <div align="center" style="width: 600px">
        <asp:Label ID="Label3" Text="Selected Year:" runat="server" Style="font-weight: 700" />
        <asp:DropDownList ID="DropDownList3" runat="server" DataSourceID="NetProfitYearPeriodDataSource1"
            DataTextField="date" DataValueField="date" AutoPostBack="True" 
            OnTextChanged="DropDownList3_TextChanged" ondatabound="DropDownList3_DataBound">
        </asp:DropDownList>
        <asp:Label ID="Label1" Text="Selected Month:" runat="server" Style="font-weight: 700" />
        <asp:DropDownList ID="DropDownList2" runat="server" DataSourceID="NetProfitMonthPeriodDataSource1"
            DataTextField="Displayed_date" DataValueField="date" AutoPostBack="True" 
            OnTextChanged="DropDownList2_TextChanged" ondatabound="DropDownList2_DataBound">
        </asp:DropDownList>
    
    <asp:ListView ID="ListView1" runat="server" DataSourceID="NetProfitDataSource" OnPreRender="ListView1_PreRender"
        OnItemCommand="ListView1_ItemCommand1" OnSorting="ListView1_Sorting">
        <LayoutTemplate>
            <table cellpadding="2" width="630px" border="0" runat="server" id="tblProducts">
                <thead>
                    <tr id="Tr1" runat="server" style="background-color: #81DAF5">
                        <th id="Th1" runat="server">
                            Day
                            <asp:ImageButton ID="DaySortButton" runat="server" Height="15px" ImageUrl="~/Images/downArrow.png"
                                CommandName="Sort" CommandArgument="" />
                        </th>
                        <th id="Th2" runat="server">
                            Net Profit
                            <asp:ImageButton ID="NetProfitSortButton" runat="server" Height="15px" ImageUrl="~/Images/downArrow.png"
                                CommandName="Sort" CommandArgument="profit DESC" />
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr runat="server" id="itemPlaceholder" />
                </tbody>
                <tfoot>
                    <tr>
                        <th />
                        <th id="Th9" runat="server" align="right">
                            <asp:Label ID="SubNetProfitTotalTextLabel" runat="Server" Text="Sub Total:$" />
                        </th>
                    </tr>
                </tfoot>
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr id="Tr2" runat="server">
                <td align="center">
                    <asp:LinkButton  OnCommand="ListViewLinkButton_Command" CommandArgument='<%#DropDownList1.Text+","+ Eval("Displayed_date")%>' runat="server" ID="MonthLabel" Width="100px"
                        Text='<%# Eval("Displayed_date") %>'></asp:LinkButton>
                </td>
                <td align="center">
                    <asp:Label ID="NetProfitLabel" runat="Server" Text='<%# DataBinder.Eval(Container.DataItem, "profit", "${0:F2}") %>'
                        Width="150px" />
                </td>
            </tr>
        </ItemTemplate>
        <AlternatingItemTemplate>
            <tr style="background-color: #EFEFEF">
                <td align="center">
                    <asp:LinkButton  OnCommand="ListViewLinkButton_Command" CommandArgument='<%#DropDownList1.Text+","+ Eval("Displayed_date")%>' runat="server" ID="MonthLabel" Width="100px"
                        Text='<%# Eval("Displayed_date") %>'></asp:LinkButton>
                </td>
                <td align="center">
                    <asp:Label ID="NetProfitLabel" runat="Server" Text='<%# DataBinder.Eval(Container.DataItem, "profit", "${0:F2}") %>'
                        Width="150px" />
                </td>
            </tr>
        </AlternatingItemTemplate>
    </asp:ListView>
    </div>
    <asp:DataPager ID="DataPager1" runat="server" PagedControlID="ListView1" 
        OnPreRender="DataPager1_PreRender" PageSize="31">
        <Fields>
            <asp:NumericPagerField ButtonCount="10" />
            <asp:TemplatePagerField>
                <PagerTemplate>
                    <asp:Label ID="TotalLabel" runat="server" Text="Maxium records:" />
                    <asp:TextBox ID="MaxiumRecordTextBox" runat="server" OnTextChanged="MaxiumRecordTextBox_OnTextChanged" />
                </PagerTemplate>
            </asp:TemplatePagerField>
        </Fields>
    </asp:DataPager>
    <br />
</asp:Content>
