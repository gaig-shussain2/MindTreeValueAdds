<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="xml" indent="yes"/>
  <xsl:template name="CP_Base" match="/" xmlns="urn:schemas-microsoft-com:office:spreadsheet" xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet" xmlns:html="http://www.w3.org/TR/REC-html40">
    <Worksheet ss:Name="Coverages Detailed Report">
      <Names>
        <NamedRange ss:Name="_FilterDatabase" ss:RefersTo="='Coverages Detailed Report'!R1C1:R1C16" ss:Hidden="1"/>
      </Names>
      <Table x:FullColumns="1" x:FullRows="1" ss:DefaultRowHeight="15">
        <Column ss:AutoFitWidth="0" ss:Width="187.5"/>
        <Column ss:Width="70.5"/>
        <Column ss:Width="66.75"/>
        <Column ss:AutoFitWidth="0" ss:Width="114"/>
        <Column ss:AutoFitWidth="0" ss:Width="150"/>
        <Column ss:AutoFitWidth="0" ss:Width="80.25"/>
        <Column ss:AutoFitWidth="0" ss:Width="75.75"/>
        <Column ss:AutoFitWidth="0" ss:Width="75" ss:Span="2"/>
        <Column ss:Index="11" ss:Width="80.25"/>
        <Column ss:AutoFitWidth="0" ss:Width="75"/>
        <Column ss:Width="78.75"/>
        <Column ss:AutoFitWidth="0" ss:Width="75"/>
        <Column ss:Width="155.25"/>
        <Column ss:AutoFitWidth="0" ss:Width="289.5"/>
        <Row ss:AutoFitHeight="0">
          <Cell ss:StyleID="s93">
            <Data ss:Type="String">Coverage Name</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s94">
            <Data ss:Type="String">Indicator</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s94">
            <Data ss:Type="String">Deleted</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s93">
            <Data ss:Type="String">Location</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s93">
            <Data ss:Type="String">Type</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s93">
            <Data ss:Type="String">Level</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s94">
            <Data ss:Type="String">Premium</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s94">
            <Data ss:Type="String">Written</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s94">
            <Data ss:Type="String">Change</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s93">
            <Data ss:Type="String">Limit</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s93">
            <Data ss:Type="String">Deductible</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s94">
            <Data ss:Type="String">Exposure</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s94">
            <Data ss:Type="String">Class Code</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s94">
            <Data ss:Type="String">Subline</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s94">
            <Data ss:Type="String">AnnualStatementLOBCode</Data>
            <NamedCell ss:Name="_FilterDatabase"/>
          </Cell>
          <Cell ss:StyleID="s93">
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
            <xsl:with-param name="Location" select="'NA'" />
            <xsl:with-param name="Type" select="Type" />
            <xsl:with-param name="Level" select="'Line'" />
            <xsl:with-param name="Premium" select="Premium" />
            <xsl:with-param name="Written" select="written" />
            <xsl:with-param name="Change" select="change" />
            <xsl:with-param name="Limit">
              <xsl:choose>
                <xsl:when test="limit[Type='Standard']/iValue">
                  <xsl:value-of  select="limit[Type='Standard']/iValue" />
                </xsl:when>
                <xsl:when test="limit[Type='Standard']/sValue">
                  <xsl:value-of  select="limit[Type='Standard']/sValue" />
                </xsl:when>
                <xsl:when test="limit[Type='$Type']/iValue">
                  <xsl:value-of  select="limit[Type='$Type']/iValue" />
                </xsl:when>
                <xsl:when test="limit[Type='$Type']/sValue">
                  <xsl:value-of  select="limit[Type='$Type']/sValue" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of  select="'0'" />
                </xsl:otherwise>
              </xsl:choose>
            </xsl:with-param>
            <xsl:with-param name="Deductible">
              <xsl:choose>
                <xsl:when test="deductible[Type='Standard']/iValue">
                  <xsl:value-of  select="deductible[Type='Standard']/iValue" />
                </xsl:when>
                <xsl:when test="deductible[Type='Standard']/sValue">
                  <xsl:value-of  select="deductible[Type='Standard']/sValue" />
                </xsl:when>
                <xsl:when test="deductible[Type='$Type']/iValue">
                  <xsl:value-of  select="deductible[Type='$Type']/iValue" />
                </xsl:when>
                <xsl:when test="deductible[Type='$Type']/sValue">
                  <xsl:value-of  select="deductible[Type='$Type']/sValue" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of  select="'0'" />
                </xsl:otherwise>
              </xsl:choose>
            </xsl:with-param>
            <xsl:with-param name="Exposure" select="statCode[Type='Exposure']/sValue" />
            <xsl:with-param name="ClassCode" select="statCode[Type='Class']/sValue" />
            <xsl:with-param name="Subline" select="statCode[Type='Subline']/sValue" />
            <xsl:with-param name="ALS" select="statCode[Type='AnnualStatementLOBCode']/sValue" />
            <xsl:with-param name="Xpath" select="concat('//session/data/policy/line/coverage[@id=', $apos, @id, $apos, ']')" />
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
            <xsl:with-param name="Level" select="'Line'" />
            <xsl:with-param name="Premium" select="Premium" />
            <xsl:with-param name="Written" select="written" />
            <xsl:with-param name="Change" select="change" />
            <xsl:with-param name="Limit">
              <xsl:choose>
                <xsl:when test="limit[Type='Standard']/iValue">
                  <xsl:value-of  select="limit[Type='Standard']/iValue" />
                </xsl:when>
                <xsl:when test="limit[Type='Standard']/sValue">
                  <xsl:value-of  select="limit[Type='Standard']/sValue" />
                </xsl:when>
                <xsl:when test="limit[Type='$Type']/iValue">
                  <xsl:value-of  select="limit[Type='$Type']/iValue" />
                </xsl:when>
                <xsl:when test="limit[Type='$Type']/sValue">
                  <xsl:value-of  select="limit[Type='$Type']/sValue" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of  select="'0'" />
                </xsl:otherwise>
              </xsl:choose>
            </xsl:with-param>
            <xsl:with-param name="Deductible">
              <xsl:choose>
                <xsl:when test="deductible[Type='Standard']/iValue">
                  <xsl:value-of  select="deductible[Type='Standard']/iValue" />
                </xsl:when>
                <xsl:when test="deductible[Type='Standard']/sValue">
                  <xsl:value-of  select="deductible[Type='Standard']/sValue" />
                </xsl:when>
                <xsl:when test="deductible[Type='$Type']/iValue">
                  <xsl:value-of  select="deductible[Type='$Type']/iValue" />
                </xsl:when>
                <xsl:when test="deductible[Type='$Type']/sValue">
                  <xsl:value-of  select="deductible[Type='$Type']/sValue" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of  select="'0'" />
                </xsl:otherwise>
              </xsl:choose>
            </xsl:with-param>
            <xsl:with-param name="Exposure" select="statCode[Type='Exposure']/sValue" />
            <xsl:with-param name="ClassCode" select="statCode[Type='Class']/sValue" />
            <xsl:with-param name="Subline" select="statCode[Type='Subline']/sValue" />
            <xsl:with-param name="ALS" select="statCode[Type='AnnualStatementLOBCode']/sValue" />
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
            <xsl:with-param name="Limit">
              <xsl:choose>
                <xsl:when test="limit[Type='Standard']/iValue">
                  <xsl:value-of  select="limit[Type='Standard']/iValue" />
                </xsl:when>
                <xsl:when test="limit[Type='Standard']/sValue">
                  <xsl:value-of  select="limit[Type='Standard']/sValue" />
                </xsl:when>
                <xsl:when test="limit[Type='$Type']/iValue">
                  <xsl:value-of  select="limit[Type='$Type']/iValue" />
                </xsl:when>
                <xsl:when test="limit[Type='$Type']/sValue">
                  <xsl:value-of  select="limit[Type='$Type']/sValue" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of  select="'0'" />
                </xsl:otherwise>
              </xsl:choose>
            </xsl:with-param>
            <xsl:with-param name="Deductible">
              <xsl:choose>
                <xsl:when test="deductible[Type='Standard']/iValue">
                  <xsl:value-of  select="deductible[Type='Standard']/iValue" />
                </xsl:when>
                <xsl:when test="deductible[Type='Standard']/sValue">
                  <xsl:value-of  select="deductible[Type='Standard']/sValue" />
                </xsl:when>
                <xsl:when test="deductible[Type='$Type']/iValue">
                  <xsl:value-of  select="deductible[Type='$Type']/iValue" />
                </xsl:when>
                <xsl:when test="deductible[Type='$Type']/sValue">
                  <xsl:value-of  select="deductible[Type='$Type']/sValue" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of  select="'0'" />
                </xsl:otherwise>
              </xsl:choose>
            </xsl:with-param>
            <xsl:with-param name="Exposure" select="statCode[Type='Exposure']/sValue" />
            <xsl:with-param name="ClassCode" select="statCode[Type='Class']/sValue" />
            <xsl:with-param name="Subline" select="statCode[Type='Subline']/sValue" />
            <xsl:with-param name="ALS" select="statCode[Type='AnnualStatementLOBCode']/sValue" />
            <xsl:with-param name="Xpath" select="concat('//session/data/policy/line/coverage[@id=', $apos, @id, $apos, ']')" />
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
            <xsl:with-param name="Limit">
              <xsl:choose>
                <xsl:when test="limit[Type='Standard']/iValue">
                  <xsl:value-of  select="limit[Type='Standard']/iValue" />
                </xsl:when>
                <xsl:when test="limit[Type='Standard']/sValue">
                  <xsl:value-of  select="limit[Type='Standard']/sValue" />
                </xsl:when>
                <xsl:when test="limit[Type='$Type']/iValue">
                  <xsl:value-of  select="limit[Type='$Type']/iValue" />
                </xsl:when>
                <xsl:when test="limit[Type='$Type']/sValue">
                  <xsl:value-of  select="limit[Type='$Type']/sValue" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of  select="'0'" />
                </xsl:otherwise>
              </xsl:choose>
            </xsl:with-param>
            <xsl:with-param name="Deductible">
              <xsl:choose>
                <xsl:when test="deductible[Type='Standard']/iValue">
                  <xsl:value-of  select="deductible[Type='Standard']/iValue" />
                </xsl:when>
                <xsl:when test="deductible[Type='Standard']/sValue">
                  <xsl:value-of  select="deductible[Type='Standard']/sValue" />
                </xsl:when>
                <xsl:when test="deductible[Type='$Type']/iValue">
                  <xsl:value-of  select="deductible[Type='$Type']/iValue" />
                </xsl:when>
                <xsl:when test="deductible[Type='$Type']/sValue">
                  <xsl:value-of  select="deductible[Type='$Type']/sValue" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of  select="'0'" />
                </xsl:otherwise>
              </xsl:choose>
            </xsl:with-param>
            <xsl:with-param name="Exposure" select="statCode[Type='Exposure']/sValue" />
            <xsl:with-param name="ClassCode" select="statCode[Type='Class']/sValue" />
            <xsl:with-param name="Subline" select="statCode[Type='Subline']/sValue" />
            <xsl:with-param name="ALS" select="statCode[Type='AnnualStatementLOBCode']/sValue" />
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
            <xsl:with-param name="Limit">
              <xsl:choose>
                <xsl:when test="limit[Type='Standard']/iValue">
                  <xsl:value-of  select="limit[Type='Standard']/iValue" />
                </xsl:when>
                <xsl:when test="limit[Type='Standard']/sValue">
                  <xsl:value-of  select="limit[Type='Standard']/sValue" />
                </xsl:when>
                <xsl:when test="limit[Type='$Type']/iValue">
                  <xsl:value-of  select="limit[Type='$Type']/iValue" />
                </xsl:when>
                <xsl:when test="limit[Type='$Type']/sValue">
                  <xsl:value-of  select="limit[Type='$Type']/sValue" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of  select="'0'" />
                </xsl:otherwise>
              </xsl:choose>
            </xsl:with-param>
            <xsl:with-param name="Deductible">
              <xsl:choose>
                <xsl:when test="deductible[Type='Standard']/iValue">
                  <xsl:value-of  select="deductible[Type='Standard']/iValue" />
                </xsl:when>
                <xsl:when test="deductible[Type='Standard']/sValue">
                  <xsl:value-of  select="deductible[Type='Standard']/sValue" />
                </xsl:when>
                <xsl:when test="deductible[Type='$Type']/iValue">
                  <xsl:value-of  select="deductible[Type='$Type']/iValue" />
                </xsl:when>
                <xsl:when test="deductible[Type='$Type']/sValue">
                  <xsl:value-of  select="deductible[Type='$Type']/sValue" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of  select="'0'" />
                </xsl:otherwise>
              </xsl:choose>
            </xsl:with-param>
            <xsl:with-param name="Exposure" select="statCode[Type='Exposure']/sValue" />
            <xsl:with-param name="ClassCode" select="statCode[Type='Class']/sValue" />
            <xsl:with-param name="Subline" select="statCode[Type='Subline']/sValue" />
            <xsl:with-param name="ALS" select="statCode[Type='AnnualStatementLOBCode']/sValue" />
            <xsl:with-param name="Xpath" select="concat('//session/data/policy/line/risk/coverage[@id=', $apos, @id, $apos, ']')" />
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
            <xsl:with-param name="Limit">
              <xsl:choose>
                <xsl:when test="limit[Type='MiscellaneousAllOther']/iValue">
                  <xsl:value-of  select="limit[Type='Standard']/iValue" />
                </xsl:when>
                <xsl:when test="limit[Type='MiscellaneousAllOther']/sValue">
                  <xsl:value-of  select="limit[Type='Standard']/sValue" />
                </xsl:when>
                <xsl:when test="limit[Type='$Type']/iValue">
                  <xsl:value-of  select="limit[Type='$Type']/iValue" />
                </xsl:when>
                <xsl:when test="limit[Type='$Type']/sValue">
                  <xsl:value-of  select="limit[Type='$Type']/sValue" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of  select="'0'" />
                </xsl:otherwise>
              </xsl:choose>
            </xsl:with-param>
            <xsl:with-param name="Deductible">
              <xsl:choose>
                <xsl:when test="deductible[Type='MiscellaneousAllOther']/iValue">
                  <xsl:value-of  select="deductible[Type='MiscellaneousAllOther']/iValue" />
                </xsl:when>
                <xsl:when test="deductible[Type='Standard']/sValue">
                  <xsl:value-of  select="deductible[Type='Standard']/sValue" />
                </xsl:when>
                <xsl:when test="deductible[Type='$Type']/iValue">
                  <xsl:value-of  select="deductible[Type='$Type']/iValue" />
                </xsl:when>
                <xsl:when test="deductible[Type='$Type']/sValue">
                  <xsl:value-of  select="deductible[Type='$Type']/sValue" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of  select="'0'" />
                </xsl:otherwise>
              </xsl:choose>
            </xsl:with-param>
            <xsl:with-param name="Exposure" select="statCode[Type='Exposure']/sValue" />
            <xsl:with-param name="ClassCode" select="statCode[Type='Class']/sValue" />
            <xsl:with-param name="Subline" select="statCode[Type='Subline']/sValue" />
            <xsl:with-param name="ALS" select="statCode[Type='AnnualStatementLOBCode']/sValue" />
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
            <xsl:with-param name="Limit">
              <xsl:choose>
                <xsl:when test="limit[Type='MiscellaneousAllOther']/iValue">
                  <xsl:value-of  select="limit[Type='Standard']/iValue" />
                </xsl:when>
                <xsl:when test="limit[Type='MiscellaneousAllOther']/sValue">
                  <xsl:value-of  select="limit[Type='Standard']/sValue" />
                </xsl:when>
                <xsl:when test="limit[Type='$Type']/iValue">
                  <xsl:value-of  select="limit[Type='$Type']/iValue" />
                </xsl:when>
                <xsl:when test="limit[Type='$Type']/sValue">
                  <xsl:value-of  select="limit[Type='$Type']/sValue" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of  select="'0'" />
                </xsl:otherwise>
              </xsl:choose>
            </xsl:with-param>
            <xsl:with-param name="Deductible">
              <xsl:choose>
                <xsl:when test="deductible[Type='MiscellaneousAllOther']/iValue">
                  <xsl:value-of  select="deductible[Type='MiscellaneousAllOther']/iValue" />
                </xsl:when>
                <xsl:when test="deductible[Type='Standard']/sValue">
                  <xsl:value-of  select="deductible[Type='Standard']/sValue" />
                </xsl:when>
                <xsl:when test="deductible[Type='$Type']/iValue">
                  <xsl:value-of  select="deductible[Type='$Type']/iValue" />
                </xsl:when>
                <xsl:when test="deductible[Type='$Type']/sValue">
                  <xsl:value-of  select="deductible[Type='$Type']/sValue" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of  select="'0'" />
                </xsl:otherwise>
              </xsl:choose>
            </xsl:with-param>
            <xsl:with-param name="Exposure" select="statCode[Type='Exposure']/sValue" />
            <xsl:with-param name="ClassCode" select="statCode[Type='Class']/sValue" />
            <xsl:with-param name="Subline" select="statCode[Type='Subline']/sValue" />
            <xsl:with-param name="ALS" select="statCode[Type='AnnualStatementLOBCode']/sValue" />
            <xsl:with-param name="Xpath" select="concat('//session/data/policy/line/risk/coverage/coverage[@id=', $apos, @id, $apos, ']')" />
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
        <Panes>
          <Pane>
            <Number>3</Number>
            <ActiveRow>1</ActiveRow>
          </Pane>
        </Panes>
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
    <xsl:param name="Deductible" />
    <xsl:param name="Exposure" />
    <xsl:param name="ClassCode" />
    <xsl:param name="Subline" />
    <xsl:param name="ALS" />
    <xsl:param name="Xpath" />

    <Row ss:AutoFitHeight="0" ss:Height="37.5" ss:StyleID="s95">
      <Cell ss:StyleID="s96">
        <Data ss:Type="String">
          <xsl:value-of select="$CoverageName"/>
        </Data>
      </Cell>
      <Cell ss:StyleID="s97">
        <Data ss:Type="Number">
          <xsl:value-of select="$Indicator"/>
        </Data>
      </Cell>
      <Cell ss:StyleID="s97">
        <Data ss:Type="Number">
          <xsl:value-of select="$Deleted"/>
        </Data>
      </Cell>
      <Cell ss:StyleID="s96">
        <Data ss:Type="String">
          <xsl:value-of select="$Location"/>
        </Data>
      </Cell>
      <Cell ss:StyleID="s96">
        <Data ss:Type="String">
          <xsl:value-of select="$Type"/>
        </Data>
      </Cell>
      <Cell ss:StyleID="s98">
        <Data ss:Type="String">
          <xsl:value-of select="$Level"/>
        </Data>
      </Cell>
      <Cell ss:StyleID="s99">
        <Data ss:Type="Number">
          <xsl:value-of select="$Premium"/>
        </Data>
      </Cell>
      <Cell ss:StyleID="s99">
        <Data ss:Type="Number">
          <xsl:value-of select="$Written"/>
        </Data>
      </Cell>
      <Cell ss:StyleID="s99">
        <Data ss:Type="Number">
          <xsl:value-of select="$Change"/>
        </Data>
      </Cell>
      <Cell ss:StyleID="s96">
        <Data ss:Type="Number">
          <xsl:value-of select="$Limit"/>
        </Data>
      </Cell>
      <Cell ss:StyleID="s96">
        <Data ss:Type="Number">
          <xsl:value-of select="$Deductible"/>
        </Data>
      </Cell>
      <Cell ss:StyleID="s99">
        <Data ss:Type="String">
          <xsl:value-of select="$Exposure"/>
        </Data>
      </Cell>
      <Cell ss:StyleID="s99">
        <Data ss:Type="String">
          <xsl:value-of select="$ClassCode"/>
        </Data>
      </Cell>
      <Cell ss:StyleID="s99">
        <Data ss:Type="String">
          <xsl:value-of select="$Subline"/>
        </Data>
      </Cell>
      <Cell ss:StyleID="s99">
        <Data ss:Type="String">
          <xsl:value-of select="$ALS"/>
        </Data>
      </Cell>
      <Cell ss:StyleID="s96">
        <Data ss:Type="String">
          <xsl:value-of select="$Xpath"/>
        </Data>
      </Cell>
    </Row>
  </xsl:template>
</xsl:stylesheet>