<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Quote.aspx.cs" Inherits="RedHill.SalesInsight.Web.Reports.Quote" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sales Insight - Quote Report</title>
</head>
<body style="margin: 0;">
    <form id="form1" runat="server">
    <div style="overflow: visible;">
        <table style="width: 100%">
            <tr>
                <td style="font-family: Verdana">
                    <rsweb:ReportViewer ID="reportView" runat="server" Width="100%" Font-Names="Verdana"
                        Font-Size="8pt" SizeToReportContent="True" AsyncRendering="false">
                        <LocalReport ReportPath="Reports\(Definitions)\Quote.rdlc" />
                    </rsweb:ReportViewer>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
