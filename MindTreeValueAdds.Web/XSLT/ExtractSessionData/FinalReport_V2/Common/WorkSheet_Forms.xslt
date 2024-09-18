<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="xml" indent="yes"/>
  <xsl:template name="Build_Forms_Worksheet" match="/" xmlns="urn:schemas-microsoft-com:office:spreadsheet" xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet" xmlns:html="http://www.w3.org/TR/REC-html40">
    <Worksheet ss:Name="Policy Forms">
      <Names>
        <NamedRange ss:Name="_FilterDatabase" ss:RefersTo="='Policy Forms'!R1C1:R2C7" ss:Hidden="1"/>
      </Names>
      <Table x:FullColumns="1" x:FullRows="1" ss:DefaultRowHeight="15">
        <Column ss:Width="144.75"/>
        <Column ss:Width="86.25"/>
        <Column ss:Width="287.25"/>
        <Column ss:Width="56.25"/>
        <Column ss:Width="69.75"/>
        <Column ss:Width="106.5"/>
        <Column ss:Width="106.5"/>
        <Column ss:Width="289.5"/>
        <Row ss:AutoFitHeight="0">
          <Cell ss:StyleID="s93">
            <Data ss:Type="String">Form Number</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s94">
            <Data ss:Type="String">Edition Date</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s93">
            <Data ss:Type="String">Caption</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s94">
            <Data ss:Type="String">Order</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s94">
            <Data ss:Type="String">Reviewed</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s94">
            <Data ss:Type="String">System Selected</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s94">
            <Data ss:Type="String">User Selected</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s93">
            <Data ss:Type="String">Manuscript</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
        </Row>

        <!--Print all forms: Start-->
        <xsl:variable name="CurrentTransaction" select="//session/data/policy/CurrentTransaction" />
        <xsl:choose>
          <xsl:when test="$CurrentTransaction = 'New' or $CurrentTransaction = 'Renew'">
            <xsl:for-each select="//printJob/printDoc">
              <xsl:call-template name="CallBuildFormRow">
                <xsl:with-param name="order" select="@order" />
                <xsl:with-param name="name" select="@name" />
                <xsl:with-param name="caption" select="@caption" />
                <xsl:with-param name="selected" select="@selected" />
                <xsl:with-param name="systemSelected" select="@systemSelected" />
                <xsl:with-param name="review" select="@review" />
                <xsl:with-param name="value" select="@value" />
              </xsl:call-template>
            </xsl:for-each>
          </xsl:when>
          <xsl:otherwise>
            <xsl:for-each select="//printJob/printDoc[@editStatus>0 or @selected='2' or @systemSelected='2' or @hasMerge='1']">
              <xsl:call-template name="CallBuildFormRow">
                <xsl:with-param name="order" select="@order" />
                <xsl:with-param name="name" select="@name" />
                <xsl:with-param name="caption" select="@caption" />
                <xsl:with-param name="selected" select="@selected" />
                <xsl:with-param name="systemSelected" select="@systemSelected" />
                <xsl:with-param name="review" select="@review" />
                <xsl:with-param name="value" select="@value" />
              </xsl:call-template>
            </xsl:for-each>
          </xsl:otherwise>
        </xsl:choose>
        <!--Print all forms: End-->
      </Table>
      <WorksheetOptions xmlns="urn:schemas-microsoft-com:office:excel">
        <PageSetup>
          <Header x:Margin="0.3"/>
          <Footer x:Margin="0.3"/>
          <PageMargins x:Bottom="0.75" x:Left="0.7" x:Right="0.7" x:Top="0.75"/>
        </PageSetup>
        <Unsynced/>
        <Panes>
          <Pane>
            <Number>3</Number>
            <ActiveRow>1</ActiveRow>
          </Pane>
        </Panes>
        <ProtectObjects>False</ProtectObjects>
        <ProtectScenarios>False</ProtectScenarios>
      </WorksheetOptions>
      <AutoFilter x:Range="R1C1:R2C7" xmlns="urn:schemas-microsoft-com:office:excel">
      </AutoFilter>
    </Worksheet>
  </xsl:template>

  <xsl:template name="CallBuildFormRow">
    <xsl:param name="order" />
    <xsl:param name="name" />
    <xsl:param name="caption" />
    <xsl:param name="selected" />
    <xsl:param name="systemSelected" />
    <xsl:param name="review" />
    <xsl:param name="value" />

    <xsl:call-template name="BuildFormsRow">
      <xsl:with-param name="Order" select="$order"/>
      <xsl:with-param name="Number" select="$name"/>
      <xsl:with-param name="EditionDate">
        <xsl:variable name="Month" select="substring(substring($name, string-length($name) - 3), 1, 2)"/>
        <xsl:variable name="Year" select="substring(substring($name, string-length($name) - 3), 3)"/>
        <xsl:choose>
          <xsl:when test="$Month and $Year">
            <xsl:value-of select="concat($Month, '/', $Year)"/>
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="'NA'"/>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:with-param>
      <xsl:with-param name="Caption" select="$caption"/>
      <xsl:with-param name="Selected">
        <xsl:choose>
          <xsl:when test="$selected='2'">
            <xsl:value-of select="'1'"/>
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="'0'"/>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:with-param>
      <xsl:with-param name="SystemSelected">
        <xsl:choose>
          <xsl:when test="$systemSelected='2'">
            <xsl:value-of select="'1'"/>
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="'0'"/>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:with-param>
      <xsl:with-param name="Review">
        <xsl:choose>
          <xsl:when test="$review">
            <xsl:value-of select="$review"/>
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="'NA'"/>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:with-param>
      <xsl:with-param name="Manuscript" select="$value"/>
    </xsl:call-template>
  </xsl:template>

  <xsl:template name="BuildFormsRow" xmlns="urn:schemas-microsoft-com:office:spreadsheet" xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet">
    <xsl:param name="Order" />
    <xsl:param name="Number" />
    <xsl:param name="EditionDate" />
    <xsl:param name="Caption" />
    <xsl:param name="Selected" />
    <xsl:param name="SystemSelected" />
    <xsl:param name="Review" />
    <xsl:param name="Manuscript" />

    <Row ss:AutoFitHeight="0" ss:Height="37.5" ss:StyleID="s100">
      <Cell ss:StyleID="s96">
        <Data ss:Type="String">
          <xsl:value-of select="$Number"/>
        </Data>
        <NamedCell ss:Name="_FilterDatabase"/>
      </Cell>
      <Cell ss:StyleID="s101">
        <Data ss:Type="String">
          <xsl:value-of select="$EditionDate"/>
        </Data>
        <NamedCell ss:Name="_FilterDatabase"/>
      </Cell>
      <Cell ss:StyleID="s102">
        <Data ss:Type="String">
          <xsl:value-of select="$Caption"/>
        </Data>
        <NamedCell ss:Name="_FilterDatabase"/>
      </Cell>
      <Cell ss:StyleID="s97">
        <Data ss:Type="Number">
          <xsl:value-of select="$Order"/>
        </Data>
        <NamedCell ss:Name="_FilterDatabase"/>
      </Cell>
      <Cell ss:StyleID="s97">
        <Data>
          <xsl:attribute name="ss:Type">
            <xsl:choose>
              <xsl:when test="$Review = '0' or $Review = '1'">
                <xsl:value-of select="'Number'"/>
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="'String'"/>
              </xsl:otherwise>
            </xsl:choose>
          </xsl:attribute>
          <xsl:value-of select="$Review"/>
        </Data>
        <NamedCell ss:Name="_FilterDatabase"/>
      </Cell>
      <Cell ss:StyleID="s97">
        <Data ss:Type="Number">
          <xsl:value-of select="$SystemSelected"/>
        </Data>
        <NamedCell ss:Name="_FilterDatabase"/>
      </Cell>
      <Cell ss:StyleID="s97">
        <xsl:variable name="tempSelect">
          <xsl:choose>
            <xsl:when test="$SystemSelected='1'">
              <xsl:value-of select="'0'"/>
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="$Selected"/>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:variable>
        <Data ss:Type="Number">
          <xsl:value-of select="$tempSelect"/>
        </Data>
        <NamedCell ss:Name="_FilterDatabase"/>
      </Cell>
      <Cell ss:StyleID="s96">
        <Data ss:Type="String">
          <xsl:value-of select="$Manuscript"/>
        </Data>
        <NamedCell ss:Name="_FilterDatabase"/>
      </Cell>
    </Row>
  </xsl:template>
</xsl:stylesheet>