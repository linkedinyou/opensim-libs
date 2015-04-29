﻿<%@ Page Language="C#" AutoEventWireup="true" ContentType="application/xrds+xml" %><?xml version="1.0" encoding="UTF-8"?>
<xrds:XRDS
	xmlns:xrds="xri://$xrds"
	xmlns:openid="http://openid.net/xmlns/1.0"
	xmlns="xri://$xrd*($v*2.0)">
	<XRD>
		<Service priority="1">
			<Type>http://specs.openid.net/auth/2.0/return_to</Type>
			<%-- Every page with an OpenID login should be listed here. --%>
			<URI><%=new Uri(Request.Url, Response.ApplyAppPathModifier("~/RelyingParty.aspx"))%></URI>
		</Service>
	</XRD>
</xrds:XRDS>
