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
    <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="GetAllData"
        TypeName="LabelPrintingInterface.TotalCurrentInventoryDataSource"></asp:ObjectDataSource>
    <asp:HiddenField runat="server" ID="_repostcheckcode" />
    <asp:ListView ID="ListView1" runat="server" DataSourceID="ObjectDataSource1" 
        onitemdatabound="ListView1_ItemDataBound" onprerender="ListView1_PreRender">
        <LayoutTemplate>
            <table cellpadding="2" width="630px" border="0" runat="server" id="tblProducts">
                <thead>
                    <tr id="Tr1" runat="server" style="background-color: #81DAF5">
                        <th id="Th1" runat="server">
                            SKU
                        </th>
                        <th id="Th2" runat="server">
                            Product Name
                        </th>
                        <th id="Th3" runat="server">
                            Product Cost
                            (RMB)
                        </th>
                        <th id="Th4" runat="server">
                            Inbound
                        </th>
                        <th id="Th5" runat="server">
                            Inbound Total
                        </th>
                        <th id="Th6" runat="server">
                            Fulfillable
                        </th>
                        <th id="Th7" runat="server">
                            Fulfillable Total
                        </th>
                        <th id="Th8" runat="server">
                            Sub Total
                            (RMB)
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
                    <asp:Label ID="SellerSKULabel" runat="Server" Text='<%#Eval("FNSKU") %>' />
                </td>
                <td>
                    <asp:Label ID="ProductNameLabel" runat="Server" Text='<%#Eval("Product Name") %>'
                        Width="300px" />
                </td>
                <td align="right">
                    <asp:Label ID="CostLabel" runat="Server" Text='<%#(String.IsNullOrEmpty(Eval("Cost").ToString())?"$0.00":DataBinder.Eval(Container.DataItem,"Cost","{0:c}"))%>' />
                </td>
                <td align="right">
                    <asp:Label ID="InboundLabel" runat="Server" Text='<%#Eval("Inbound") %>' />
                </td>
                <td align="right">
                    <asp:Label ID="InboundTotalLabel" runat="Server" Text="0.00" />
                </td>
                <td align="right">
                    <asp:Label ID="FulfillableLabel" runat="Server" Text='<%#Eval("Fulfillable") %>' />
                </td>
                <td align="right">
                    <asp:Label ID="FulfillableTotalLabel" runat="Server" Text="0.00" />
                </td>
                <td align="right">
                    <asp:Label ID="SubTotalLabel" runat="Server" Text="0.00" />
                </td>
            </tr>
        </ItemTemplate>
        <AlternatingItemTemplate>
            <tr style="background-color: #EFEFEF">
                <td valign="top">
                    <asp:Label ID="SellerSKULabel" runat="Server" Text='<%#Eval("FNSKU") %>' />
                </td>
                <td>
                    <asp:Label ID="ProductNameLabel" runat="Server" Text='<%#Eval("Product Name") %>'
                        Width="300px" />
                </td>
                <td align="right">
                    <asp:Label ID="CostLabel" runat="Server" Text='<%#(String.IsNullOrEmpty(Eval("Cost").ToString())?"$0.00":DataBinder.Eval(Container.DataItem,"Cost","{0:c}"))%>' />
                </td>
                <td align="right">
                    <asp:Label ID="InboundLabel" runat="Server" Text='<%#Eval("Inbound") %>' />
                </td>
                <td align="right">
                    <asp:Label ID="InboundTotalLabel" runat="Server" Text="0.00" />
                </td>
                <td align="right">
                    <asp:Label ID="FulfillableLabel" runat="Server" Text='<%#Eval("Fulfillable") %>' />
                </td>
                <td align="right">
                    <asp:Label ID="FulfillableTotalLabel" runat="Server" Text="0.00" />
                </td>
                <td align="right">
                    <asp:Label ID="SubTotalLabel" runat="Server" Text="0.00" />
                </td>
            </tr>
        </AlternatingItemTemplate>
    </asp:ListView>
    <asp:DataPager ID="DataPager1" runat="server" PagedControlID="ListView1" 
        onprerender="DataPager1_PreRender">
        <Fields>
            <asp:NumericPagerField ButtonCount="10" />
            <asp:TemplatePagerField>
                <PagerTemplate>
                    <asp:Label ID="TotalLabel" runat="server" Text="Maxium records:" />
                    <asp:TextBox ID="MaxiumRecordTextBox" runat="server" OnTextChanged="MaxiumRecordTextBox_OnTextChanged"/>
                </PagerTemplate>
            </asp:TemplatePagerField>
        </Fields>
    </asp:DataPager>
    <br />
</asp:Content>
