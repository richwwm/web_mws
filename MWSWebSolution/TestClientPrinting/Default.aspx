<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server" ID="Head1">
    <title>Test ThermalLabel Client Printing</title>
    <script src="http://ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery.min.js" type="text/javascript"></script>
    <script src="Scripts/TLClientPrint.js" type="text/javascript"></script>
</head>
<body>
    <form id="Form2" runat="server">
    <div>
    <h1>ThermalLabel SDK Client Print Samples</h1><br />
    <input type="button" value="Print Basic Label..." onclick="javascript:printThermalLabel()" />
    <br />
    <br />
    <cite>Remember that the client must install the TLClientPrint Utility first!</cite>
    </div>    
    </form>
</body>
</html>