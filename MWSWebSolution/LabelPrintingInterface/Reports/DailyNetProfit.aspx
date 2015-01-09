<%@ Page Title="" Language="C#" Debug="true" MasterPageFile="~/Site1.Master" AutoEventWireup="true"
    CodeBehind="DailyNetProfit.aspx.cs" Inherits="LabelPrintingInterface.Reports.DailyNetProfit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <asp:Panel ID="Panel1" runat="server">
        </asp:Panel>
    </div>
    <asp:ObjectDataSource ID="NetProfitDataSource" runat="server" SelectMethod="GetAllSortedDataByMerchantID"
        TypeName="LabelPrintingInterface.NetProfitDataSource" OnSelecting="NetProfitDataSource_Selecting">
        <SelectParameters>
            <asp:SessionParameter DefaultValue="ProductName DESC" Name="sortExpression" SessionField="SortedBy"
                Type="String" />
            <asp:ControlParameter ControlID="DropDownList1" DefaultValue="" Name="sMerchantID"
                PropertyName="SelectedValue" Type="String" />
            <asp:ControlParameter ControlID="DropDownList2" Name="startDateTime" PropertyName="SelectedValue"
                Type="DateTime" />
            <asp:ControlParameter ControlID="DropDownList2" Name="endDateTime" PropertyName="SelectedValue"
                Type="DateTime" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="AmazonAccountDataSource" runat="server" SelectMethod="GetAmazonAccountListByUserID"
        TypeName="LabelPrintingInterface.AmazonAccountDataSource" OnSelecting="AmazonAccountDataSource2_Selecting">
        <SelectParameters>
            <asp:SessionParameter Name="sUserID" SessionField="UserID" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="NetProfitDayPeriodDataSource1" runat="server" SelectMethod="GetDayPeriodInNetProfitDataByMerchantID"
        TypeName="LabelPrintingInterface.NetProfitDataSource">
        <SelectParameters>
            <asp:ControlParameter ControlID="DropDownList1" Name="sMerchantID" PropertyName="SelectedValue"
                Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:HiddenField runat="server" ID="_repostcheckcode" />
    <div align="left" style="width: 1042px">
        <asp:Label Text="Viewing Amazon account:" runat="server" Style="font-weight: 700" />
        <asp:DropDownList ID="DropDownList1" runat="server" Height="17px" Width="193px" DataSourceID="AmazonAccountDataSource"
            AutoPostBack="True" DataTextField="description" DataValueField="MerchantID" OnTextChanged="DropDownList1_TextChanged">
        </asp:DropDownList>
        <asp:Label ID="Label1" Text="Selected Date:" runat="server" Style="font-weight: 700" />
        <asp:DropDownList ID="DropDownList2" runat="server" DataSourceID="NetProfitDayPeriodDataSource1"
            DataTextField="date" DataValueField="date" AutoPostBack="True" OnTextChanged="DropDownList2_TextChanged"
            OnDataBinding="DropDownList2_DataBinding" OnPreRender="DropDownList2_PreRender">
        </asp:DropDownList>
    </div>
    <asp:ListView ID="ListView1" runat="server" DataSourceID="NetProfitDataSource" OnPreRender="ListView1_PreRender"
        OnItemCommand="ListView1_ItemCommand1" OnSorting="ListView1_Sorting">
        <LayoutTemplate>
            <table cellpadding="2" width="630px" border="0" runat="server" id="tblProducts">
                <thead>
                    <tr id="Tr1" runat="server" style="background-color: #81DAF5">
                        <th id="Th1" runat="server">
                            Product Title
                            <asp:ImageButton ID="SellerSKUSortButton" runat="server" Height="15px" ImageUrl="~/Images/downArrow.png"
                                CommandName="Sort" CommandArgument="ProductName ASC" />
                        </th>
                        <th id="Th2" runat="server">
                            Net Profit Per Item
                            <asp:ImageButton ID="NetProfitSortButton" runat="server" Height="15px" ImageUrl="~/Images/downArrow.png"
                                CommandName="Sort" CommandArgument="NetProfit_USD DESC" />
                        </th>
                        <%--                        <th id="Th3" runat="server">
                            Product Cost (RMB)
                            <asp:ImageButton ID="ProductCostSortButton" runat="server" Height="15px" ImageUrl="~/Images/downArrow.png"
                                CommandName="Sort" CommandArgument="Cost DESC" />
                        </th>--%>
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
                <td valign="top">
                    <asp:Label ID="ProductTitleLabel" runat="Server" Text='<%#Eval("ProductName") %>'
                        Width="300px" />
                </td>
                <td align="center">
                    <asp:Label ID="NetProfitLabel" runat="Server" Text='<%# DataBinder.Eval(Container.DataItem, "NetProfit_USD", "${0:F2}") %>'
                        Width="150px" />
                </td>
                <%--                <td align="right">
                    <asp:Label ID="CostLabel" runat="Server" Text='<%#(String.IsNullOrEmpty(Eval("Cost").ToString())?"$0.00":DataBinder.Eval(Container.DataItem,"Cost","{0:c}"))%>' />
                </td>--%>
            </tr>
        </ItemTemplate>
        <AlternatingItemTemplate>
            <tr style="background-color: #EFEFEF">
                <td valign="top">
                    <asp:Label ID="ProductTitleLabel" runat="Server" Text='<%#Eval("ProductName") %>'
                        Width="300px" />
                </td>
                <td align="center">
                    <asp:Label ID="NetProfitLabel" runat="Server" Text='<%# DataBinder.Eval(Container.DataItem, "NetProfit_USD", "${0:F2}") %>'
                        Width="150px" />
                </td>
                <%--                <td align="right">
                    <asp:Label ID="CostLabel" runat="Server" Text='<%#(String.IsNullOrEmpty(Eval("Cost").ToString())?"$0.00":DataBinder.Eval(Container.DataItem,"Cost","{0:c}"))%>' />
                </td>--%>
            </tr>
        </AlternatingItemTemplate>
    </asp:ListView>
    <asp:DataPager ID="DataPager1" runat="server" PagedControlID="ListView1" OnPreRender="DataPager1_PreRender">
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
