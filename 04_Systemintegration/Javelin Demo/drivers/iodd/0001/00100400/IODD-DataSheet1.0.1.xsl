<?xml version="1.0" encoding="ISO-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:iodd="http://www.io-link.com/IODD/2009/11">
	<xsl:output method="html" encoding="UTF-8"/>


<!--============
Global Params/Variables/Functions
============-->

	<!-- Info about this stylesheet  -->
	<xsl:variable name="DocumentInfo" select="'IODD-DataSheet1.0.1.xsl V0.3 2010-02-25'"/>


<!--============
Matched Templates (main)
============-->

	<!-- Main Template -->
	<xsl:template match="/">
		<html>
			<head>
<!--				<META http-equiv="Content-Type" content="text/html; charset=UTF-8"/>-->
				<style type="text/css">

 a:link { text-decoration:none; color:#294d96; }
 a:visited { text-decoration:none; color:#294d96; }
 a:hover { text-decoration:underline; color:#000000; }

p { font-size:7pt;
	 font-family:Arial;
	 color:#A0A0A0; }

table { border:1px solid gray;
		background-color:#f7f7f7;
		padding:10;
		margin:0;
		empty-cells:show;
		border-collapse:collapse;
		margin-top:5;
		margin-bottom:5;
        font-family:Arial;
        font-weight:normal;
        text-align:left; }

th { border:1px solid gray;
		font-size:9pt;
        font-family:Arial;
        font-weight:bold;
        color:#FFFFFF;
        background-color:#4c7fbc; }

td { border:1px solid gainsboro;
		font-size:9pt;
        font-family:Arial;
        font-weight:normal;
        color:black; }

table.doc { border:0px solid gray;
		background-color:#FFFFFF;
		padding:10;
		margin:0;
		empty-cells:show;
		border-collapse:collapse;
		margin-top:5;
		margin-bottom:5;
        font-family:Arial;
        font-weight:normal; }

td.doc { border:1px solid gainsboro;
		font-size:9pt;
        font-family:Arial;
        font-weight:normal;
        color:black; }

				</style>
			</head>
			<body bgcolor="white">
				<!-- Show a table of all files together with the languages contained -->
				<table class="doc" width="100%">
					<tbody>
						<tr>
							<td class="doc" width="20%">Version: <xsl:value-of select="iodd:IODevice/iodd:DocumentInfo/@version"/></td>
							<td colspan="2" class="doc">Release Date: <xsl:value-of select="iodd:IODevice/iodd:DocumentInfo/@releaseDate"/></td>
						</tr>
					</tbody>
				</table>
				<br/>
				<table width="100%">
					<tbody>
						<xsl:apply-templates select="iodd:IODevice/iodd:ProfileBody"/>
						<xsl:apply-templates select="iodd:IODevice/iodd:CommNetworkProfile"/>
						<xsl:apply-templates select="iodd:IODevice/iodd:ProfileBody/iodd:DeviceFunction/iodd:ProcessDataCollection"/>
						<xsl:apply-templates select="iodd:IODevice/iodd:ProfileBody/iodd:DeviceIdentity/iodd:DeviceVariantCollection"/>
					</tbody>
				</table>
				<p>Created by <xsl:value-of select="$DocumentInfo"/>.</p>
			</body>
		</html>
	</xsl:template>


<!-- Template for ProfileBody representation -->
	<xsl:template match="iodd:ProfileBody">
		<!-- overview -->
		<tr>
			<th width="20%" colspan="3" style="font-size:14pt;">
				<xsl:call-template name="GetText">
					<xsl:with-param name="textId" select="iodd:DeviceIdentity/iodd:DeviceFamily/@textId"/>
				</xsl:call-template>
			</th>
		</tr>
		<tr>
			<td>Vendor ID</td>
			<td>
				<xsl:value-of select="iodd:DeviceIdentity/@vendorId"/>
				(0x<xsl:call-template name="PrintHex16"><xsl:with-param name="number" select="iodd:DeviceIdentity/@vendorId"/></xsl:call-template>)
			</td>
			<td rowspan="5" align="center">
 				<xsl:if test="iodd:DeviceIdentity/iodd:VendorLogo">
					<img><xsl:attribute name="src"><xsl:value-of select="iodd:DeviceIdentity/iodd:VendorLogo/@name"/></xsl:attribute><xsl:attribute name="alt">Vendor Logo</xsl:attribute></img>
				</xsl:if>
			</td>
		</tr>
		<tr>
			<td>Vendor Name</td>
			<td><xsl:value-of select="iodd:DeviceIdentity/@vendorName"/></td>
		</tr>
		<tr>
			<td>Vendor Text</td>
			<td>
				<xsl:call-template name="GetText">
					<xsl:with-param name="textId" select="iodd:DeviceIdentity/iodd:VendorText/@textId"/>
				</xsl:call-template>
			</td>
		</tr>
		<tr>
			<td>Vendor URL</td>
			<td>
				<a>
					<xsl:attribute name="href">
						<xsl:call-template name="GetText">
							<xsl:with-param name="textId" select="iodd:DeviceIdentity/iodd:VendorUrl/@textId"/>
						</xsl:call-template>
					</xsl:attribute>
					<xsl:call-template name="GetText">
						<xsl:with-param name="textId" select="iodd:DeviceIdentity/iodd:VendorUrl/@textId"/>
					</xsl:call-template>
				</a>
			</td>
		</tr>
		<tr>
			<td>Device ID</td>
			<td>
				<xsl:value-of select="iodd:DeviceIdentity/@deviceId"/>
				(0x<xsl:call-template name="PrintHex24"><xsl:with-param name="number" select="iodd:DeviceIdentity/@deviceId"/></xsl:call-template>)
			</td>
		</tr>
	</xsl:template>


	<!-- Template for CommunicationNetworkProfile representation -->
	<xsl:template match="iodd:CommNetworkProfile">
		<tr>
			<th width="20%" colspan="3">Communication</th>
		</tr>
		<tr>
			<td>IO-Link Revision</td>
			<td colspan="2"><xsl:value-of select="@iolinkRevision"/></td>
		</tr>
		<tr>
			<td>Minimum Cycle Time</td>
			<td colspan="2"><xsl:value-of select="iodd:TransportLayers/iodd:PhysicalLayer/@minCycleTime div 1000"/> ms</td>
		</tr>
		<tr>
			<td>SIO Mode Supported</td>
			<td colspan="2">
				<xsl:call-template name="PrintBoolean">
					<xsl:with-param name="Value" select="iodd:TransportLayers/iodd:PhysicalLayer/@sioSupported"/>
					<xsl:with-param name="BDefault" select="'Yes'"/>
				</xsl:call-template>
			</td>
		</tr>
	</xsl:template>


	<!-- Template for DeviceVariant representation -->
	<xsl:template match="iodd:ProcessDataCollection">
		<tr>
			<th width="20%" colspan="3">Process Data</th>
		</tr>
		<xsl:if test="iodd:ProcessData/iodd:ProcessDataIn">
		<tr>
    			<td>Input Length in Bits</td>
				<td colspan="2"><xsl:value-of select="iodd:ProcessData/iodd:ProcessDataIn/@bitLength"/></td>
            </tr>
		</xsl:if>
		<xsl:if test="iodd:ProcessData/iodd:ProcessDataOut">
			<tr>
				<td>Output Length in Bits</td>
				<td colspan="2"><xsl:value-of select="iodd:ProcessData/iodd:ProcessDataOut/@bitLength"/></td>
			</tr>
		</xsl:if>
	</xsl:template>


	<!-- Template for DeviceVariant representation -->
	<xsl:template match="iodd:DeviceVariantCollection">
		<xsl:for-each select="iodd:DeviceVariant">
			<tr>
				<th width="20%">Device Variant</th>
				<th colspan="2">
					<xsl:call-template name="GetText">
						<xsl:with-param name="textId" select="iodd:ProductName/@textId"/>
					</xsl:call-template>
				</th>
			</tr>
			<tr>
				<td>Product Text</td>
				<td colspan="2">
					<xsl:call-template name="GetText">
						<xsl:with-param name="textId" select="iodd:ProductText/@textId"/>
					</xsl:call-template>
				</td>
			</tr>
			<tr>
				<td>Product ID</td>
				<td colspan="2"><xsl:value-of select="@productId"/></td>
			</tr>
			<xsl:if test="@deviceIcon">
				<tr>
					<td>Device Icon</td>
					<td colspan="2">
						<img><xsl:attribute name="src"><xsl:value-of select="@deviceIcon"/></xsl:attribute><xsl:attribute name="alt">Device Icon</xsl:attribute></img>
					</td>
				</tr>
			</xsl:if>
			<xsl:if test="@deviceSymbol">
				<tr>
					<td>Device Symbol</td>
					<td colspan="2">
						<img><xsl:attribute name="src"><xsl:value-of select="@deviceSymbol"/></xsl:attribute><xsl:attribute name="alt">Device Symbol</xsl:attribute><xsl:attribute name="width">48px</xsl:attribute><xsl:attribute name="heigth">48px</xsl:attribute></img>
					</td>
				</tr>
			</xsl:if>
		</xsl:for-each>
	</xsl:template>


<!--============
Called templates
============-->

	<!-- Prints the text with "textId" from the ExternalTextList or an ExternalTextDocument according to the $Language -->
	<xsl:template name="GetText">
		<xsl:param name="textId"/>
		<xsl:value-of select="/iodd:IODevice/iodd:ExternalTextCollection/iodd:PrimaryLanguage/iodd:Text[@id=$textId]/@value"/>
	</xsl:template>

	<!-- Prints one byte in hexadecimal (without 0x prefix) -->
	<xsl:template name="PrintHex8">
		<xsl:param name="number"/>
		<xsl:variable name="table" select="'0123456789ABCDEF'"/>
		<xsl:value-of select="substring($table,floor($number div 16) + 1,1)"/>
		<xsl:value-of select="substring($table,($number mod 16) + 1,1)"/>
	</xsl:template>

	<!-- Prints a two-byte value in hexadecimal (without 0x prefix) -->
	<xsl:template name="PrintHex16">
		<xsl:param name="number"/>
		<xsl:call-template name="PrintHex8">
			<xsl:with-param name="number" select="floor($number div 256)"/>
		</xsl:call-template>
		<xsl:call-template name="PrintHex8">
			<xsl:with-param name="number" select="$number mod 256"/>
		</xsl:call-template>
	</xsl:template>

	<!-- Prints a three-byte value hexadecimal (without 0x prefix) -->
	<xsl:template name="PrintHex24">
		<xsl:param name="number"/>
		<xsl:call-template name="PrintHex8">
			<xsl:with-param name="number" select="floor($number div 65536)"/>
		</xsl:call-template>
		<xsl:call-template name="PrintHex16">
			<xsl:with-param name="number" select="$number mod 65536"/>
		</xsl:call-template>
	</xsl:template>

	<xsl:template name="PrintBoolean">
		<xsl:param name="Value"/>
		<xsl:param name="BDefault">No</xsl:param>
			<xsl:choose>
				<xsl:when test="$Value='true'">Yes</xsl:when>
				<xsl:when test="$Value='1'">Yes</xsl:when>
				<xsl:when test="$Value='false'">No</xsl:when>
				<xsl:when test="$Value='0'">No</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="$BDefault"/>
				</xsl:otherwise>
			</xsl:choose>
	</xsl:template>

</xsl:stylesheet>


