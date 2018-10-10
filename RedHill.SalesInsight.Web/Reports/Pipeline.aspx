<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Pipeline.aspx.cs" Inherits="RedHill.SalesInsight.Web.Reports.Pipeline" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sales Insight - Pipeline Report</title>
</head>
<body style="margin: 0;">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="scriptManager" runat="server" />
    <div style="overflow: visible;">
        <table style="width:100%">
            <tr>
                <td>
                    <rsweb:ReportViewer ID="reportView" runat="server" Width="100%" Font-Names="Verdana"
                        Font-Size="8pt" SizeToReportContent="True" AsyncRendering="false">
                        <LocalReport ReportPath="Reports\(Definitions)\Pipeline.rdlc" />
                    </rsweb:ReportViewer>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
