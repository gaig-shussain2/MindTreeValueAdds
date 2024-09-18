<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="xml" indent="yes"/>

  <xsl:template name="Build_Forms_Worksheet" match="/" xmlns="urn:schemas-microsoft-com:office:spreadsheet" xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet" xmlns:html="http://www.w3.org/TR/REC-html40">
    <Worksheet ss:Name="Policy Forms">
      <Names>
        <NamedRange ss:Name="_FilterDatabase" ss:RefersTo="='Policy Forms'!R1C1:R1C8" ss:Hidden="1"/>
      </Names>
      <Table ss:ExpandedColumnCount="8" x:FullColumns="1" x:FullRows="1" ss:DefaultRowHeight="14.4375">
        <Column ss:Width="44.25"/>
        <Column ss:Width="161.25"/>
        <Column ss:Width="291"/>
        <Column ss:Width="88.5"/>
        <Column ss:Width="250.5"/>
        <Row>
          <Cell ss:StyleID="s109">
            <Data ss:Type="String">Order</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s110">
            <Data ss:Type="String">Form Number</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s110">
            <Data ss:Type="String">Caption</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s110">
            <Data ss:Type="String">Print Default</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s110">
            <Data ss:Type="String">Manuscript</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
        </Row>
        <!--Print all forms: Start-->
        <xsl:for-each select="//printJob/printDoc">
          <xsl:call-template name="BuildFormsRow">
            <xsl:with-param name="Order" select="@order" />
            <xsl:with-param name="Number" select="@name" />
            <xsl:with-param name="Caption" select="@caption" />
            <xsl:with-param name="PrintDefault" select="@printDefault" />
            <xsl:with-param name="Manuscript" select="@value" />
          </xsl:call-template>
        </xsl:for-each>
        <!--Print all forms: End-->
      </Table>
      <WorksheetOptions xmlns="urn:schemas-microsoft-com:office:excel">
        <PageSetup>
          <Header x:Margin="0.3"/>
          <Footer x:Margin="0.3"/>
          <PageMargins x:Bottom="0.75" x:Left="0.7" x:Right="0.7" x:Top="0.75"/>
        </PageSetup>
        <Unsynced/>
        <Print>
          <ValidPrinterInfo/>
          <HorizontalResolution>600</HorizontalResolution>
          <VerticalResolution>600</VerticalResolution>
        </Print>
        <ProtectObjects>False</ProtectObjects>
        <ProtectScenarios>False</ProtectScenarios>
      </WorksheetOptions>
      <AutoFilter x:Range="R1C1:R1C8" xmlns="urn:schemas-microsoft-com:office:excel">
      </AutoFilter>
    </Worksheet>
  </xsl:template>

  <xsl:template name="BuildFormsRow" xmlns="urn:schemas-microsoft-com:office:spreadsheet" xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet">
    <xsl:param name="Order" />
    <xsl:param name="Number" />
    <xsl:param name="Caption" />
    <xsl:param name="PrintDefault" />
    <xsl:param name="Manuscript" />

    <Row ss:AutoFitHeight="0" ss:StyleID="s108">
      <Cell ss:StyleID="s111">
        <Data ss:Type="Number">
          <xsl:value-of select="$Order"/>
        </Data>
      </Cell>
      <Cell ss:StyleID="s111">
        <Data ss:Type="String">
          <xsl:value-of select="$Number"/>
        </Data>
      </Cell>
      <Cell ss:StyleID="s111">
        <Data ss:Type="String">
          <xsl:value-of select="$Caption"/>
        </Data>
      </Cell>
      <Cell ss:StyleID="s111">
        <Data ss:Type="String">
          <xsl:value-of select="$PrintDefault"/>
        </Data>
      </Cell>
      <Cell ss:StyleID="s111">
        <Data ss:Type="String">
          <xsl:value-of select="$Manuscript"/>
        </Data>
      </Cell>
    </Row>
  </xsl:template>
</xsl:stylesheet>
