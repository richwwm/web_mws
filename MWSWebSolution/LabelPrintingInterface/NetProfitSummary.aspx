<%@ Page Title="" Language="C#" Debug="true" MasterPageFile="~/Site1.Master" AutoEventWireup="true"
    CodeBehind="NetProfitSummary.aspx.cs" Inherits="LabelPrintingInterface.Reports.NetProfitSummary" %>

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
    <asp:ObjectDataSource ID="NetProfitYearPeriodSummaryDataSource1" runat="server" SelectMethod="GetNetProfitSummaryDataYearPeriod"
        TypeName="LabelPrintingInterface.NetProfitDataSource"></asp:ObjectDataSource>
    <asp:HiddenField runat="server" ID="_repostcheckcode" />
    <div align="center" style="width: 300px">
        <asp:Label ID="Label2" runat="server" Text="Net Profit Summary:"></asp:Label>
    </div>
    <div align="center" style="width: 300px">
        <asp:Label ID="Label1" Text="Selected Year:" runat="server" Style="font-weight: 700" />
        <asp:DropDownList ID="DropDownList2" runat="server" DataSourceID="NetProfitYearPeriodSummaryDataSource1"
            DataTextField="date" DataValueField="date" AutoPostBack="True" OnTextChanged="DropDownList2_TextChanged">
        </asp:DropDownList>
        <asp:ListView ID="ListView1" runat="server" DataSourceID="NetProfitDataSource">
            <LayoutTemplate>
                <table cellpadding="2" width="300px" border="0" runat="server" id="tblProducts">
                    <thead>
                        <tr id="Tr1" runat="server" style="background-color: #81DAF5">
                            <th id="Th1" runat="server" align="center">
                                Month
                                <asp:Label ID="SellerSKUSortButton" runat="server" Height="15px" />
                            </th>
                            <th id="Th2" runat="server" align="center">
                                Net Profit
                                <asp:Label ID="NetProfitSortButton" runat="server" Height="15px" />
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr runat="server" id="itemPlaceholder" />
                    </tbody>
                    <tfoot>
                        <tr>
                            <th />
                        </tr>
                    </tfoot>
                </table>
            </LayoutTemplate>
            <ItemTemplate>
                <tr id="Tr2" runat="server">
                    <td align="center">
                        <asp:Label ID="MonthLabel" runat="Server" Text='<%# Eval("Month") %>' Width="50px" />
                    </td>
                    <td align="center">
                        <asp:LinkButton OnCommand="ListViewLinkButton_Command" CommandArgument='<%# Eval("iMonth")+","+DropDownList2.Text%>'
                            runat="server" ID="NetProfitLabel" Width="50px" Text='<%# DataBinder.Eval(Container.DataItem, "profit", "${0:F2}") %>'></asp:LinkButton>
                    </td>
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr style="background-color: #EFEFEF">
                    <td align="center">
                        <asp:Label ID="MonthLabel" runat="Server" Text='<%# Eval("Month") %>' Width="50px" />
                    </td>
                    <td align="center">
                        <asp:LinkButton OnCommand="ListViewLinkButton_Command" CommandArgument='<%# Eval("iMonth")+","+DropDownList2.Text%>'
                            runat="server" ID="NetProfitLabel" Width="50px" Text='<%# DataBinder.Eval(Container.DataItem, "profit", "${0:F2}") %>'></asp:LinkButton>
                    </td>
                </tr>
            </AlternatingItemTemplate>
        </asp:ListView>
    </div>
    <br />
</asp:Content>
