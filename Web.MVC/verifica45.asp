<%
Dim arrListaComponentes(48)

arrListaComponentes(0)  = Array( "AB Mailer","ABMailer.Mailman" )
arrListaComponentes(1)  = Array( "ABC Upload","ABCUpload4.XForm" )
arrListaComponentes(2)  = Array( "ActiveFile","ActiveFile.Post" )
arrListaComponentes(3)  = Array( "ActiveX Data Object","ADODB.Connection" )
arrListaComponentes(4)  = Array( "Adiscon SimpleMail","ADISCON.SimpleMail.1" )
arrListaComponentes(5)  = Array( "ASP DNS", "AspDNS.Lookup" )
arrListaComponentes(6)  = Array( "ASP HTTP","AspHTTP.Conn" )
arrListaComponentes(7)  = Array( "ASP Image","AspImage.Image" )
arrListaComponentes(8)  = Array( "ASP Mail","SMTPsvg.Mailer" )
arrListaComponentes(9)  = Array( "ASP NNTP News", "AspNNTP.Conn" )
arrListaComponentes(10) = Array( "ASP POP 3", "POP3svg.Mailer" )
arrListaComponentes(11) = Array( "ASP Simple Upload","ASPSimpleUpload.Upload" )
arrListaComponentes(12) = Array( "ASP Smart Cache","aspSmartCache.SmartCache" )
arrListaComponentes(13) = Array( "ASP Smart Mail","aspSmartMail.SmartMail" )
arrListaComponentes(14) = Array( "ASP Smart Upload","aspSmartUpload.SmartUpload" )
arrListaComponentes(15) = Array( "ASP Tear","SOFTWING.ASPtear" )
arrListaComponentes(16) = Array( "ASP Thumbnailer","ASPThumbnailer.Thumbnail" )
arrListaComponentes(17) = Array( "ASP WhoIs","WhoIs2.WhoIs" )
arrListaComponentes(18) = Array( "ASPSoft NT Object","ASPSoft.NT" )
arrListaComponentes(19) = Array( "ASPSoft Upload","ASPSoft.Upload" )
arrListaComponentes(20) = Array( "CDO NTS","CDONTS.NewMail" )
arrListaComponentes(21) = Array( "Chestysoft Image","csImageFile.Manage" )
arrListaComponentes(22) = Array( "Chestysoft Upload","csASPUpload.Process" )
arrListaComponentes(23) = Array( "Dimac JMail","JMail.Message" )
arrListaComponentes(24) = Array( "Distinct SMTP","DistinctServerSmtp.SmtpCtrl" )
arrListaComponentes(25) = Array( "Dundas Mailer","Dundas.Mailer" )
arrListaComponentes(26) = Array( "Dundas Upload","Dundas.Upload.2" )
arrListaComponentes(27) = Array( "Dundas PieChartServer", "Dundas.ChartServer.2")
arrListaComponentes(28) = Array( "Dundas 2D Chart", "Dundas.ChartServer2D.1")
arrListaComponentes(29) = Array( "Dundas 3D Chart", "Dundas.ChartServer")
arrListaComponentes(30) = Array( "Dynu Encrypt","Dynu.Encrypt" )
arrListaComponentes(31) = Array( "Dynu HTTP","Dynu.HTTP" )
arrListaComponentes(32) = Array( "Dynu Mail","Dynu.Email" )
arrListaComponentes(33) = Array( "Dynu Upload","Dynu.Upload" )
arrListaComponentes(34) = Array( "Dynu WhoIs","Dynu.Whois" )
arrListaComponentes(35) = Array( "Easy Mail","EasyMail.SMTP.5" )
arrListaComponentes(36) = Array( "File System Object","Scripting.FileSystemObject" )
arrListaComponentes(37) = Array( "Ticluse Teknologi HTTP","InteliSource.Online" )
arrListaComponentes(38) = Array( "Last Mod","LastMod.FileObj" )
arrListaComponentes(39) = Array( "Microsoft XML Engine","Microsoft.XMLDOM" )
arrListaComponentes(40) = Array( "Persits ASP JPEG","Persits.Jpeg" )
arrListaComponentes(41) = Array( "Persits ASPEmail","Persits.MailSender" )
arrListaComponentes(42) = Array( "Persits ASPEncrypt","Persits.CryptoManager" )
arrListaComponentes(43) = Array( "Persits File Upload","Persits.Upload.1" )
arrListaComponentes(44) = Array( "SMTP Mailer","SmtpMail.SmtpMail.1" )
arrListaComponentes(45) = Array( "Soft Artisans File Upload","SoftArtisans.FileUp" )
arrListaComponentes(46) = Array( "Image Size", "ImgSize.Check" )
arrListaComponentes(47) = Array( "Microsoft XML HTTP", "Microsoft.XMLHTTP" )
arrListaComponentes(48) = Array( "Dictionary", "Scripting.Dictionary" )

' Rotina que verifica o componente do array � um objeto.
Function VerificaObjeto(pComponente)
Dim objComponente
 On Error Resume Next
 VerificaObjeto = False
 Err.Clear
 Set objComponente = Server.CreateObject(pComponente)
 If Err = 0 Then VerificaObjeto = True
 Set objComponente = Nothing
 Err.Clear
End Function

Public Function VerificaComponentes()
Dim intCont, strTxt
Dim intIndex, strProv

intCont = 0
 strTxt = "<table border='1' bordercolor='black' cellspacing='0' cellpadding='0' align='center' width='400'>"
 For intIndex = LBound(arrListaComponentes) To UBound(arrListaComponentes)
  strProv = intIndex
  strTxt = strTxt & "<tr><td width='200'><font face='tahoma' size='1'>" & arrListaComponentes(intIndex)(0) & "</font></td>"
  If VerificaObjeto(arrListaComponentes(intIndex)(1)) Then
    strTxt = strTxt & "<td align=center><font color='red' face='tahoma' size='1'>Instalado</font></td>"
    intCont = intCont + 1
  Else
    strTxt = strTxt & "<td align='center'><font face='tahoma' size='1'>N�o Instalado</font></td>"
  End If
  strTxt = strTxt & "</tr>"
 Next
 strTxt = strTxt & "</table><p align='center'><font face='tahoma' size='2'><b>" & intCont & "</b> componentes instalados de "
 strTxt = strTxt & "<b>" & UBound(arrListaComponentes) + 1 & "</b> no provedor.</font> </p>"
 VerificaComponentes = strTxt 
End Function

Response.Write VerificaComponentes
%>
