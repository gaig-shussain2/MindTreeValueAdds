<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="xml" indent="yes"/>

  <xsl:template name="CP_Base" match="/" xmlns="urn:schemas-microsoft-com:office:spreadsheet" xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet" xmlns:html="http://www.w3.org/TR/REC-html40">
    <Worksheet ss:Name="Coverages Detailed Report">
      <Names>
        <NamedRange ss:Name="_FilterDatabase"
         ss:RefersTo="='Coverages Detailed Report'!R1C1:R1C16" ss:Hidden="1"/>
      </Names>
      <Table ss:ExpandedColumnCount="16" x:FullColumns="1" x:FullRows="1" ss:DefaultRowHeight="14.4375">
        <Column ss:StyleID="s86" ss:Width="165.75"/>
        <Column ss:Width="70.5"/>
        <Column ss:Width="65.25"/>
        <Column ss:Width="165.75"/>
        <Column ss:StyleID="s86" ss:Width="165.75"/>
        <Column ss:Width="52.5"/>
        <Column ss:Width="71.25"/>
        <Column ss:Width="65.25"/>
        <Column ss:Width="63.75"/>
        <Column ss:AutoFitWidth="0" ss:Width="66.75"/>
        <Column ss:Width="72"/>
        <Column ss:Width="78.75"/>
        <Column ss:Width="63"/>
        <Column ss:AutoFitWidth="0" ss:Width="198.75"/>
        <Row ss:AutoFitHeight="0" ss:Height="15" ss:StyleID="s87">
          <Cell ss:StyleID="s88">
            <Data ss:Type="String">Coverage Name</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s89">
            <Data ss:Type="String">Indicator</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s89">
            <Data ss:Type="String">Deleted</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s88">
            <Data ss:Type="String">Location</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s88">
            <Data ss:Type="String">Type</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s89">
            <Data ss:Type="String">Level</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s89">
            <Data ss:Type="String">Premium</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s89">
            <Data ss:Type="String">Written</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s89">
            <Data ss:Type="String">Change</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s89">
            <Data ss:Type="String">Limit</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s89">
            <Data ss:Type="String">Exposure</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s89">
            <Data ss:Type="String">Class Code</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s89">
            <Data ss:Type="String">Subline</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s88">
            <Data ss:Type="String">Xpath</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
        </Row>

        <!--Build Coverage Row: Start-->
        <xsl:variable name="apos">'</xsl:variable>

        <!--Line: Start-->
        <xsl:for-each select="//session/data/policy/line[Type='Property']/coverage[(Indicator = '1' or @deleted = '1') and Indicators[Type='IsCovEndorsement']/bValue=0 and Type != 'AdditionalInterests' and Type != 'Miscellaneous' and Type != 'LineFinanceCharge']">
          <xsl:call-template name="BuildCoverageRow_CP">
            <xsl:with-param name="CoverageName" select="Type" />
            <xsl:with-param name="Indicator" select="Indicator" />
            <xsl:with-param name="Deleted" select="@deleted" />
            <xsl:with-param name="Location" select="'N/A'" />
            <xsl:with-param name="Type" select="Type" />
            <xsl:with-param name="Level" select="'Policy'" />
            <xsl:with-param name="Premium" select="Premium" />
            <xsl:with-param name="Written" select="written" />
            <xsl:with-param name="Change" select="change" />
            <xsl:with-param name="Limit" select="limit[Type='Standard']/iValue" />
            <xsl:with-param name="Exposure" select="statCode[Type='Exposure']/sValue" />
            <xsl:with-param name="ClassCode" select="statCode[Type='Class']/sValue" />
            <xsl:with-param name="Subline" select="statCode[Type='Subline']/sValue" />
            <xsl:with-param name="Xpath" select="concat('//session/data/policy/line/coverage[@id=', $apos, @id, $apos, ']')" />
          </xsl:call-template>
        </xsl:for-each>

        <!--Risk: Start-->
        <xsl:for-each select="//session/data/policy/line[Type='Property']/risk/coverage[(Indicator = '1' or @deleted = '1') and (Type != 'Manuscript' and Type != 'MiscellaneousAllOther')]">
          <xsl:variable name="LocationID" select="../LocationID" />
          <xsl:call-template name="BuildCoverageRow_CP">
            <xsl:with-param name="CoverageName" select="Type" />
            <xsl:with-param name="Indicator" select="Indicator" />
            <xsl:with-param name="Deleted" select="@deleted" />
            <xsl:with-param name="Location" select="//session/data/account/location[@id=$LocationID]/Description" />
            <xsl:with-param name="Type" select="Type" />
            <xsl:with-param name="Level" select="'Risk'" />
            <xsl:with-param name="Premium" select="Premium" />
            <xsl:with-param name="Written" select="written" />
            <xsl:with-param name="Change" select="change" />
            <xsl:with-param name="Limit" select="limit[Type='Standard']/iValue" />
            <xsl:with-param name="Exposure" select="statCode[Type='Exposure']/sValue" />
            <xsl:with-param name="ClassCode" select="statCode[Type='Class']/sValue" />
            <xsl:with-param name="Subline" select="statCode[Type='Subline']/sValue" />
            <xsl:with-param name="Xpath" select="concat('//session/data/policy/line/risk/coverage[@id=', $apos, @id, $apos, ']')" />
          </xsl:call-template>
        </xsl:for-each>

        <!--Endorsements: Start-->
        <xsl:for-each select="//session/data/policy/line[Type='Property']/coverage[Indicators[Type='IsCovEndorsement']/bValue='1' and (Indicator = '1' or @deleted = '1')]">
          <xsl:call-template name="BuildCoverageRow_CP">
            <xsl:with-param name="CoverageName" select="Type" />
            <xsl:with-param name="Indicator" select="Indicator" />
            <xsl:with-param name="Deleted" select="@deleted" />
            <xsl:with-param name="Location" select="'N/A'" />
            <xsl:with-param name="Type" select="Type" />
            <xsl:with-param name="Level" select="'Policy'" />
            <xsl:with-param name="Premium" select="Premium" />
            <xsl:with-param name="Written" select="written" />
            <xsl:with-param name="Change" select="change" />
            <xsl:with-param name="Limit" select="limit[Type='Standard']/iValue" />
            <xsl:with-param name="Exposure" select="statCode[Type='Exposure']/sValue" />
            <xsl:with-param name="ClassCode" select="statCode[Type='Class']/sValue" />
            <xsl:with-param name="Subline" select="statCode[Type='Subline']/sValue" />
            <xsl:with-param name="Xpath" select="concat('//session/data/policy/line/coverage[@id=', $apos, @id, $apos, ']')" />
          </xsl:call-template>
        </xsl:for-each>

        <!--Misc All Other (Line): Start-->
        <xsl:for-each select="//session/data/policy/line[Type='Property']/coverage[(Indicator = '1' or @deleted = '1') and Type='Miscellaneous']">
          <xsl:call-template name="BuildCoverageRow_CP">
            <xsl:with-param name="CoverageName" select="form[Type='Caption']/Description" />
            <xsl:with-param name="Indicator" select="Indicator" />
            <xsl:with-param name="Deleted" select="@deleted" />
            <xsl:with-param name="Location" select="'N/A'" />
            <xsl:with-param name="Type" select="Type" />
            <xsl:with-param name="Level" select="'Policy'" />
            <xsl:with-param name="Premium" select="Premium" />
            <xsl:with-param name="Written" select="written" />
            <xsl:with-param name="Change" select="change" />
            <xsl:with-param name="Limit" select="limit[Type='CovMiscellaneous']/iValue" />
            <xsl:with-param name="Exposure" select="statCode[Type='Exposure']/sValue" />
            <xsl:with-param name="ClassCode" select="statCode[Type='Class']/sValue" />
            <xsl:with-param name="Subline" select="statCode[Type='Subline']/sValue" />
            <xsl:with-param name="Xpath" select="concat('//session/data/policy/line/coverage[@id=', $apos, @id, $apos, ']')" />
          </xsl:call-template>
        </xsl:for-each>

        <!--Misc All Other (Risk): Start-->
        <xsl:for-each select="//session/data/policy/line[Type='Property']/risk/coverage[Type='MiscellaneousAllOther' and (Indicator = '1' or @deleted = '1')]/coverage[Type='MiscellaneousAllOtherInstances' and (Indicator = '1' or @deleted = '1')]">
          <xsl:variable name="LocationID" select="../../LocationID" />
          <xsl:call-template name="BuildCoverageRow_CP">
            <xsl:with-param name="CoverageName" select="exposure[Type='CoverageTitle']/sValue" />
            <xsl:with-param name="Indicator" select="Indicator" />
            <xsl:with-param name="Deleted" select="@deleted" />
            <xsl:with-param name="Location" select="//session/data/account/location[@id=$LocationID]/Description" />
            <xsl:with-param name="Type" select="../Type" />
            <xsl:with-param name="Level" select="'Risk'" />
            <xsl:with-param name="Premium" select="Premium" />
            <xsl:with-param name="Written" select="written" />
            <xsl:with-param name="Change" select="change" />
            <xsl:with-param name="Limit" select="limit[Type='MiscellaneousAllOther']/iValue" />
            <xsl:with-param name="Exposure" select="statCode[Type='Exposure']/sValue" />
            <xsl:with-param name="ClassCode" select="statCode[Type='Class']/sValue" />
            <xsl:with-param name="Subline" select="statCode[Type='Subline']/sValue" />
            <xsl:with-param name="Xpath" select="concat('//session/data/policy/line/risk/coverage/coverage[@id=', $apos, @id, $apos, ']')" />
          </xsl:call-template>
        </xsl:for-each>

        <!--Manuscript Endorsement (Risk): Start-->
        <xsl:for-each select="//session/data/policy/line[Type='Property']/risk/coverage[Type='Manuscript' and (Indicator = '1' or @deleted = '1')]/coverage[Type='ManuscriptInstances' and (Indicator = '1' or @deleted = '1')]">
          <xsl:variable name="LocationID" select="../../LocationID" />
          <xsl:call-template name="BuildCoverageRow_CP">
            <xsl:with-param name="CoverageName" select="exposure[Type='CoverageTitle']/sValue" />
            <xsl:with-param name="Indicator" select="Indicator" />
            <xsl:with-param name="Deleted" select="@deleted" />
            <xsl:with-param name="Location" select="//session/data/account/location[@id=$LocationID]/Description" />
            <xsl:with-param name="Type" select="../Type" />
            <xsl:with-param name="Level" select="'Risk'" />
            <xsl:with-param name="Premium" select="Premium" />
            <xsl:with-param name="Written" select="written" />
            <xsl:with-param name="Change" select="change" />
            <xsl:with-param name="Limit" select="limit[Type='Standard']/iValue" />
            <xsl:with-param name="Exposure" select="statCode[Type='Exposure']/sValue" />
            <xsl:with-param name="ClassCode" select="statCode[Type='Class']/sValue" />
            <xsl:with-param name="Subline" select="statCode[Type='Subline']/sValue" />
            <xsl:with-param name="Xpath" select="concat('//session/data/policy/line/risk/coverage/coverage[@id=', $apos, @id, $apos, ']')" />
          </xsl:call-template>
        </xsl:for-each>

        <!--Finance Charge: Start-->
        <xsl:for-each select="//session/data/policy/line[Type='Property']/coverage[Type='LineFinanceCharge' and (Indicator='1' or @deleted='1')]">
          <xsl:call-template name="BuildCoverageRow_CP">
            <xsl:with-param name="CoverageName" select="'Finance Charge'" />
            <xsl:with-param name="Indicator" select="Indicator" />
            <xsl:with-param name="Deleted" select="@deleted" />
            <xsl:with-param name="Location" select="'N/A'" />
            <xsl:with-param name="Type" select="Type" />
            <xsl:with-param name="Level" select="'Policy'" />
            <xsl:with-param name="Premium" select="FCPremium" />
            <xsl:with-param name="Written" select="FCWritten" />
            <xsl:with-param name="Change" select="FCChange" />
            <xsl:with-param name="Limit" select="limit[Type='Standard']/iValue" />
            <xsl:with-param name="Exposure" select="statCode[Type='Exposure']/sValue" />
            <xsl:with-param name="ClassCode" select="statCode[Type='Class']/sValue" />
            <xsl:with-param name="Subline" select="statCode[Type='Subline']/sValue" />
            <xsl:with-param name="Xpath" select="concat('//session/data/policy/line/coverage[@id=', $apos, @id, $apos, ']')" />
          </xsl:call-template>
        </xsl:for-each>
        <!--Build Coverage Row: End-->
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
        <Selected/>
        <ProtectObjects>False</ProtectObjects>
        <ProtectScenarios>False</ProtectScenarios>
      </WorksheetOptions>
      <AutoFilter x:Range="R1C1:R1C16" xmlns="urn:schemas-microsoft-com:office:excel">
      </AutoFilter>
    </Worksheet>
  </xsl:template>

  <xsl:template name="BuildCoverageRow_CP"  xmlns="urn:schemas-microsoft-com:office:spreadsheet" xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet">
    <xsl:param name="CoverageName" />
    <xsl:param name="Indicator" />
    <xsl:param name="Deleted" />
    <xsl:param name="Location" />
    <xsl:param name="Type" />
    <xsl:param name="Level" />
    <xsl:param name="Premium" />
    <xsl:param name="Written" />
    <xsl:param name="Change" />
    <xsl:param name="Limit" />
    <xsl:param name="Exposure" />
    <xsl:param name="ClassCode" />
    <xsl:param name="Subline" />
    <xsl:param name="Xpath" />

    <Row ss:AutoFitHeight="0" ss:Height="33.9375" ss:StyleID="s103">
      <Cell ss:StyleID="s90">
        <Data ss:Type="String">
          <xsl:choose>
            <xsl:when test="$CoverageName">
              <xsl:value-of select="$CoverageName"/>
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="'N/A'"/>
            </xsl:otherwise>
          </xsl:choose>
        </Data>
      </Cell>
      <Cell ss:StyleID="s92">
        <Data ss:Type="String">
          <xsl:choose>
            <xsl:when test="$Indicator">
              <xsl:value-of select="$Indicator"/>
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="'N/A'"/>
            </xsl:otherwise>
          </xsl:choose>
        </Data>
      </Cell>
      <Cell ss:StyleID="s92">
        <Data ss:Type="String">
          <xsl:choose>
            <xsl:when test="$Deleted">
              <xsl:value-of select="$Deleted"/>
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="'N/A'"/>
            </xsl:otherwise>
          </xsl:choose>
        </Data>
      </Cell>
      <Cell ss:StyleID="s90">
        <Data ss:Type="String">
          <xsl:choose>
            <xsl:when test="$Location">
              <xsl:value-of select="$Location"/>
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="'N/A'"/>
            </xsl:otherwise>
          </xsl:choose>
        </Data>
      </Cell>
      <Cell ss:StyleID="s90">
        <Data ss:Type="String">
          <xsl:choose>
            <xsl:when test="$Type">
              <xsl:value-of select="$Type"/>
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="'N/A'"/>
            </xsl:otherwise>
          </xsl:choose>
        </Data>
      </Cell>
      <Cell ss:StyleID="s101">
        <Data ss:Type="String">
          <xsl:choose>
            <xsl:when test="$Level">
              <xsl:value-of select="$Level"/>
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="'N/A'"/>
            </xsl:otherwise>
          </xsl:choose>
        </Data>
      </Cell>
      <Cell ss:StyleID="s101">
        <Data ss:Type="String">
          <xsl:variable name="_var" select="$Premium"/>
          <xsl:choose>
            <xsl:when test="$_var and $_var!=0">
              <xsl:value-of select="concat('$', format-number($_var, '###,###,###.00'))" />
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="'$0'"/>
            </xsl:otherwise>
          </xsl:choose>
        </Data>
      </Cell>
      <Cell ss:StyleID="s101">
        <Data ss:Type="String">
          <xsl:variable name="_var" select="$Written"/>
          <xsl:choose>
            <xsl:when test="$_var and $_var!=0">
              <xsl:value-of select="concat('$', format-number($_var, '###,###,###.00'))" />
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="'$0'"/>
            </xsl:otherwise>
          </xsl:choose>
        </Data>
      </Cell>
      <Cell ss:StyleID="s101">
        <Data ss:Type="String">
          <xsl:variable name="_var" select="$Change"/>
          <xsl:choose>
            <xsl:when test="$_var and $_var!=0">
              <xsl:value-of select="concat('$', format-number($_var, '###,###,###.00'))" />
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="'$0'"/>
            </xsl:otherwise>
          </xsl:choose>
        </Data>
      </Cell>
      <Cell ss:StyleID="s101">
        <Data ss:Type="String">
          <xsl:variable name="_var" select="$Limit"/>
          <xsl:choose>
            <xsl:when test="$_var and $_var!=0">
              <xsl:value-of select="format-number($_var, '###,###,###.00')" />
            </xsl:when>
            <xsl:when test="$_var and $_var=0">
              <xsl:value-of select="'0'"/>
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="'N/A'"/>
            </xsl:otherwise>
          </xsl:choose>
        </Data>
      </Cell>
      <Cell ss:StyleID="s102">
        <Data ss:Type="String">
          <xsl:choose>
            <xsl:when test="$Exposure">
              <xsl:value-of select="$Exposure"/>
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="'N/A'"/>
            </xsl:otherwise>
          </xsl:choose>
        </Data>
      </Cell>
      <Cell ss:StyleID="s92">
        <Data ss:Type="String">
          <xsl:choose>
            <xsl:when test="$ClassCode">
              <xsl:value-of select="$ClassCode"/>
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="'N/A'"/>
            </xsl:otherwise>
          </xsl:choose>
        </Data>
      </Cell>
      <Cell ss:StyleID="s92">
        <Data ss:Type="String">
          <xsl:choose>
            <xsl:when test="$Subline">
              <xsl:value-of select="$Subline"/>
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="'N/A'"/>
            </xsl:otherwise>
          </xsl:choose>
        </Data>
      </Cell>
      <Cell ss:StyleID="s90">
        <Data ss:Type="String">
          <xsl:choose>
            <xsl:when test="$Xpath">
              <xsl:value-of select="$Xpath"/>
            </xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="'N/A'"/>
            </xsl:otherwise>
          </xsl:choose>
        </Data>
      </Cell>
    </Row>
  </xsl:template>
</xsl:stylesheet>