<%@ Page Title="" Language="C#" Debug="true" MasterPageFile="~/Site1.Master" AutoEventWireup="true"
    CodeBehind="TotalCurrentInventory.aspx.cs" Inherits="LabelPrintingInterface.Reports.TotalCurrentInventory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <asp:Panel ID="Panel1" runat="server">
        </asp:Panel>
        <div>
            <asp:TextBox ID="SearchBox" runat="server" Height="22px" Style="margin-left: 0px;
                margin-top: 0px" Width="283px"></asp:TextBox>
            <asp:ImageButton ID="SearchImageButton1" runat="server" Height="25px" ImageUrl="~/Images/search.png"
                OnClick="SearchImageButton1_Click" Width="45px" /></div>
    </div>
    <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="GetAllSortedData"
        TypeName="LabelPrintingInterface.TotalCurrentInventoryDataSource">
        <SelectParameters>
            <asp:SessionParameter DefaultValue="SellerSKU ASC" Name="sortExpression" 
                SessionField="SortedBy" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:HiddenField runat="server" ID="_repostcheckcode" />
    <asp:ListView ID="ListView1" runat="server" DataSourceID="ObjectDataSource1"
        OnPreRender="ListView1_PreRender" OnItemCommand="ListView1_ItemCommand1"
        OnSorting="ListView1_Sorting">
        <LayoutTemplate>
            <table cellpadding="2" width="630px" border="0" runat="server" id="tblProducts">
                <thead>
                    <tr id="Tr1" runat="server" style="background-color: #81DAF5">
                        <th id="Th1" runat="server">
                            SellerSKU
                            <asp:ImageButton ID="SellerSKUSortButton" runat="server" Height="15px" ImageUrl="~/Images/downArrow.png"
                                CommandName="Sort" CommandArgument="SellerSKU DESC" />
                        </th>
                        <th id="Th2" runat="server">
                            Product Title
                        </th>
                        <th id="Th3" runat="server">
                            Product Cost (RMB)
                            <asp:ImageButton ID="ProductCostSortButton" runat="server" Height="15px" ImageUrl="~/Images/downArrow.png"
                                CommandName="Sort" CommandArgument="Cost DESC" />
                        </th>
                        <th id="Th4" runat="server">
                            Inbound
                            <asp:ImageButton ID="InboundSortButton" runat="server" Height="15px" ImageUrl="~/Images/downArrow.png"
                                CommandName="Sort" CommandArgument="Inbound DESC" />
                        </th>
                        <th id="Th5" runat="server">
                            Inbound Total
                            <asp:ImageButton ID="InboundTotalSortButton" runat="server" Height="15px" ImageUrl="~/Images/downArrow.png"
                                CommandName="Sort" CommandArgument="InboundTotal DESC" />
                        </th>
                        <th id="Th6" runat="server">
                            Fulfillable
                            <asp:ImageButton ID="FulfillableSortButton" runat="server" Height="15px" ImageUrl="~/Images/downArrow.png"
                                CommandName="Sort" CommandArgument="Fulfillable DESC" />
                        </th>
                        <th id="Th7" runat="server">
                            Fulfillable Total
                            <asp:ImageButton ID="FulfillableTotalSortButton" runat="server" Height="15px" ImageUrl="~/Images/downArrow.png"
                                CommandName="Sort" CommandArgument="FulfillableTotal DESC" />
                        </th>
                        <th id="Th8" runat="server">
                            Sub Total (RMB)
                            <asp:ImageButton ID="SubTotalSortButton" runat="server" Height="15px" ImageUrl="~/Images/downArrow.png"
                                CommandName="Sort" CommandArgument="SubTotal DESC" />
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr runat="server" id="itemPlaceholder" />
                </tbody>
                <tfoot>
                    <tr>
                        <td>
                            <th />
                            <th />
                            <th id="Th9" runat="server" align="right">
                                <asp:Label ID="TotalTextLabel" runat="Server" Text="Total:" />
                            </th>
                            <th id="Th10" runat="server" align="right">
                                <asp:Label ID="TotalInboundLabel" runat="Server" Text="" />
                            </th>
                            <th />
                            <th id="Th11" runat="server" align="right">
                                <asp:Label ID="TotalFulfillableLabel" runat="Server" Text="" />
                            </th>
                            <th id="Th12" runat="server" align="right">
                                <asp:Label ID="TotalCountLabel" runat="Server" Text="" />
                            </th>
                        </td>
                    </tr>
                </tfoot>
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr id="Tr2" runat="server">
                <td valign="top">
                    <asp:Label ID="SellerSKULabel" runat="Server" Text='<%#Eval("SellerSKU") %>'
                        Width="300px" />
                </td>
                <td align="left">
                    <asp:Label ID="ProductTitleLabel" runat="Server" Text='<%#Eval("ProductTitle") %>' 
                        Width="300px" />
                </td>
                <td align="right">
                    <asp:Label ID="CostLabel" runat="Server" Text='<%#(String.IsNullOrEmpty(Eval("Cost").ToString())?"$0.00":DataBinder.Eval(Container.DataItem,"Cost","{0:c}"))%>' />
                </td>
                <td align="right">
                    <asp:Label ID="InboundLabel" runat="Server" Text='<%#Eval("Inbound") %>' />
                </td>
                <td align="right">
                    <asp:Label ID="InboundTotalLabel" runat="Server" Text='<%#Convert.ToDouble(Eval("InboundTotal")).ToString("c") %>' />
                </td>
                <td align="right">
                    <asp:Label ID="FulfillableLabel" runat="Server" Text='<%#Eval("Fulfillable") %>' />
                </td>
                <td align="right">
                    <asp:Label ID="FulfillableTotalLabel" runat="Server" Text='<%#Convert.ToDouble(Eval("FulfillableTotal")).ToString("c")  %>' />
                </td>
                <td align="right">
                    <asp:Label ID="SubTotalLabel" runat="Server" Text='<%#Convert.ToDouble(Eval("SubTotal")).ToString("c") %>' />
                </td>
            </tr>
        </ItemTemplate>
        <AlternatingItemTemplate>
            <tr style="background-color: #EFEFEF">
                <td valign="top">
                    <asp:Label ID="SellerSKULabel" runat="Server" Text='<%#Eval("SellerSKU") %>'
                        Width="300px" />
                </td>
                <td align="left">
                    <asp:Label ID="ProductTitleLabel" runat="Server" Text='<%#Eval("ProductTitle") %>' 
                        Width="300px" />
                </td>
                <td align="right">
                    <asp:Label ID="CostLabel" runat="Server" Text='<%#(String.IsNullOrEmpty(Eval("Cost").ToString())?"$0.00":DataBinder.Eval(Container.DataItem,"Cost","{0:c}"))%>' />
                </td>
                <td align="right">
                    <asp:Label ID="InboundLabel" runat="Server" Text='<%#Eval("Inbound") %>' />
                </td>
                <td align="right">
                    <asp:Label ID="InboundTotalLabel" runat="Server" Text='<%#Convert.ToDouble(Eval("InboundTotal")).ToString("c") %>' />
                </td>
                <td align="right">
                    <asp:Label ID="FulfillableLabel" runat="Server" Text='<%#Eval("Fulfillable") %>' />
                </td>
                <td align="right">
                    <asp:Label ID="FulfillableTotalLabel" runat="Server" Text='<%#Convert.ToDouble(Eval("FulfillableTotal")).ToString("c") %>' />
                </td>
                <td align="right">
                    <asp:Label ID="SubTotalLabel" runat="Server" Text='<%#Convert.ToDouble(Eval("SubTotal")).ToString("c") %>' />
                </td>
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
