<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ProductList.aspx.cs"
    Inherits="LabelPrintingInterface.ProductList" %>

<%@ MasterType VirtualPath="~/Site1.Master" %>
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
        TypeName="LabelPrintingInterface.DataSource.SellerInventorySupplyList"></asp:ObjectDataSource>
    <asp:HiddenField runat="server" ID="_repostcheckcode" />
    <asp:ListView ID="ListView1" runat="server" DataSourceID="ObjectDataSource1" OnItemCommand="ListView1_ItemCommand1">
        <LayoutTemplate>
            <table cellpadding="2" width="630px" border="0" runat="server" id="tblProducts">
                <tr id="Tr1" runat="server" style="background-color: #81DAF5">
                    <th id="Th1" runat="server">
                        SellerSKU
                        <asp:ImageButton ID="SellerSKUSortButton" runat="server" Height="15px" ImageUrl="~/Images/downArrow.png"
                            CommandName="Sort" CommandArgument="SellerSKU" />
                    </th>
                    <th id="Th2" runat="server">
                        ASIN
                        <asp:ImageButton ID="ASINSortButton" runat="server" Height="15px" ImageUrl="~/Images/downArrow.png"
                            CommandName="Sort" CommandArgument="ASIN" />
                    </th>
                    <th id="Th3" runat="server">
                        FNSKU
                        <asp:ImageButton ID="FNSKUSortButton" runat="server" Height="15px" ImageUrl="~/Images/downArrow.png"
                            CommandName="Sort" CommandArgument="FNSKU" />
                    </th>
                    <th id="Th4" runat="server">
                        Inbound
                    </th>
                    <th id="Th5" runat="server">
                        Fulfillable
                    </th>
                    <th id="Th6" runat="server">
                        Reserved
                    </th>
                    <th id="Th7" runat="server">
                        Action
                    </th>
                </tr>
                <tr runat="server" id="itemPlaceholder" />
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr id="Tr2" runat="server">
                <td valign="top">
                    <asp:Label ID="SellerSKULabel" runat="Server" Text='<%#Eval("SellerSKU") %>' Width="300px" />
                </td>
                <td>
                    <asp:HyperLink runat="server" ID="ASINLabel" Width="100px" NavigateUrl='<%# "http://www.amazon.com/dp/"+ Eval("ASIN") %>'
                        Text='<%# Eval("ASIN") %>'></asp:HyperLink>
                </td>
                <td>
                    <asp:Label ID="FNSKULabel" runat="Server" Text='<%#Eval("FNSKU") %>' Width="100px" />
                </td>
                <td align="right">
                    <asp:Label ID="FulfillableSupplyQuantityLabel" runat="Server" Text='<%#Eval("Inbound") %>' />
                </td>
                <td align="right">
                    <asp:Label ID="TotalSupplyLabel" runat="Server" Text='<%#Eval("Fulfillable") %>' />
                </td>
                <td align="right">
                    <asp:Label ID="FBAQuantityLabel" runat="Server" Text='<%#Eval("Reserved") %>' />
                </td>
                <td valign="top">
                    <asp:ImageButton ID="PrintLabelButton" runat="server" Height="30px" ImageUrl="~/Images/addToPrint.png"
                        CommandArgument='<%#Eval("FNSKU") + ","+Eval("SellerSKU") + ","+Eval("ASIN")%>' /><br />
                </td>
            </tr>
        </ItemTemplate>
        <AlternatingItemTemplate>
            <tr style="background-color: #EFEFEF">
                <td valign="top">
                    <asp:Label ID="SellerSKULabel" runat="Server" Text='<%#Eval("SellerSKU") %>' Width="300px" />
                </td>
                <td>
                    <asp:HyperLink runat="server" ID="ASINLabel" NavigateUrl='<%# "http://www.amazon.com/dp/"+ Eval("ASIN") %>'
                        Text='<%# Eval("ASIN") %>'></asp:HyperLink>
                </td>
                <td>
                    <asp:Label ID="FNSKULabel" runat="Server" Text='<%#Eval("FNSKU") %>' />
                </td>
                <td align="right">
                    <asp:Label ID="FulfillableSupplyQuantityLabel" runat="Server" Text='<%#Eval("Inbound") %>' />
                </td>
                <td align="right">
                    <asp:Label ID="TotalSupplyLabel" runat="Server" Text='<%#Eval("Fulfillable") %>' />
                </td>
                <td align="right">
                    <asp:Label ID="FBAQuantityLabel" runat="Server" Text='<%#Eval("Reserved") %>' />
                </td>
                <td valign="top">
                    <asp:ImageButton ID="PrintLabelButton" runat="server" Height="30px" ImageUrl="~/Images/addToPrint.png"
                        CommandArgument='<%#Eval("FNSKU") + ","+Eval("SellerSKU") + ","+Eval("ASIN")%>' /><br />
                </td>
            </tr>
        </AlternatingItemTemplate>
    </asp:ListView>
    <asp:DataPager ID="DataPager1" runat="server" PagedControlID="ListView1">
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
    <br/>
    <asp:Label ID="Label4" runat="server" Text="Print Label List:" Visible="False"></asp:Label>
    <asp:ListView ID="LabelPrintList" runat="server" ItemPlaceholderID="itemPlaceholder2"
        OnItemCommand="LabelPrintList_ItemCommand1">
        <LayoutTemplate>
            <table cellpadding="2" width="630px" border="1" runat="server" id="tblPrintList">
                <tr id="Tr1" runat="server">
                    <th id="Th1" runat="server">
                        SellerSKU
                    </th>
                    <th id="Th2" runat="server">
                        ASIN
                    </th>
                    <th id="Th3" runat="server">
                        FNSKU
                    </th>
                    <th id="Th9" runat="server">
                        Copies
                    </th>
                </tr>
                <tr runat="server" id="itemPlaceholder2" />
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr id="Tr2" runat="server">
                <td valign="top">
                    <asp:Label ID="SellerSKULabel" runat="Server" Text='<%#Eval("SellerSKU") %>' />
                </td>
                <td>
                    <asp:HyperLink runat="server" ID="ASINLabel" NavigateUrl='<%# "http://www.amazon.com/dp/"+ Eval("ASIN") %>'
                        Text='<%# Eval("ASIN") %>'></asp:HyperLink>
                </td>
                <td>
                    <asp:Label ID="FNSKULabel" runat="Server" Text='<%#Eval("FNSKU") %>' />
                </td>
                <td valign="top">
                    <asp:TextBox ID="QuantityTextBox" runat="server" Text='<%#Eval("PrintQuantity")%>' />
                </td>
                <td valign="top">
                    <asp:ImageButton ID="DeleteButton" runat="server" Height="30px" ImageUrl="~/Images/file_delete.png"
                        CommandName="DeleteItem" CommandArgument='<%#Eval("FNSKU") %>' />
                </td>
            </tr>
        </ItemTemplate>
    </asp:ListView>
    <asp:ImageButton ID="ClearAllButton" runat="server" Height="40px" Width="45px" ImageUrl="~/Images/clear.png"
        OnClick="ClearPrintListButton_Click" />
    &nbsp
    <asp:ImageButton ID="PrintAllButton" runat="server" Height="40px" ImageUrl="~/Images/print.png"
        OnClick="PrintAllButton_Click" Width="45px" />
</asp:Content>
