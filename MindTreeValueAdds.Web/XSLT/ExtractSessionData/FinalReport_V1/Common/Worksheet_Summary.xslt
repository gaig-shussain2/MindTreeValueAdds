<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <!--Imposrt all XSLT: Start-->
  <xsl:import href="Common.xslt"/>
  <!--Imposrt all XSLT: End-->

  <xsl:output method="xml" indent="yes"/>
  <xsl:template name="Build_Summary_Worksheet" match="/" xmlns="urn:schemas-microsoft-com:office:spreadsheet" xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet" xmlns:html="http://www.w3.org/TR/REC-html40">
    <Worksheet ss:Name="Summary Page">
      <Table ss:ExpandedColumnCount="6" ss:ExpandedRowCount="22" x:FullColumns="1"
       x:FullRows="1" ss:StyleID="s63" ss:DefaultRowHeight="14.4375">
        <Column ss:Index="2" ss:StyleID="s63" ss:Width="213.75"/>
        <Column ss:StyleID="s63" ss:Width="78"/>
        <Column ss:Index="5" ss:StyleID="s63" ss:Width="87.75"/>
        <Column ss:StyleID="s63" ss:Width="144.75"/>
        <Row ss:AutoFitHeight="0" ss:Height="15"/>
        <!--Policy Info: Start-->
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s64">
            <Data ss:Type="String">Line of Business:</Data>
          </Cell>
          <Cell ss:StyleID="s65">
            <Data ss:Type="String">
              <xsl:value-of select="//session/data/policy/LineOfBusiness"/>
            </Data>
          </Cell>
          <Cell ss:StyleID="s66"/>
          <Cell ss:StyleID="s66"/>
          <Cell ss:StyleID="s67"/>
        </Row>
        <Row ss:AutoFitHeight="0" ss:Height="15">
          <Cell ss:Index="2" ss:StyleID="s64">
            <Data ss:Type="String">Policy Symbol:</Data>
          </Cell>
          <Cell ss:StyleID="s65">
            <Data ss:Type="String">
              <xsl:value-of select="//session/data/policy/PolicyNumberSymbol"/>
            </Data>
          </Cell>
          <Cell ss:Index="6" ss:StyleID="s68"/>
        </Row>
        <Row ss:AutoFitHeight="0" ss:Height="15">
          <Cell ss:Index="2" ss:StyleID="s69"/>
          <Cell ss:Index="6" ss:StyleID="s68"/>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s64">
            <Data ss:Type="String">Policy Effective Date: </Data>
          </Cell>
          <Cell ss:StyleID="s70">
            <Data ss:Type="DateTime">
              <xsl:value-of select="//session/data/policy/EffectiveDate"/>
            </Data>
          </Cell>
          <Cell ss:Index="5" ss:StyleID="s64">
            <Data ss:Type="String">Business Unit:</Data>
          </Cell>
          <Cell ss:StyleID="s65">
            <Data ss:Type="String">
              <xsl:value-of select="//session/data/policy/businessUnit/Code"/>
            </Data>
          </Cell>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s64">
            <Data ss:Type="String">Policy Expiration Date: </Data>
          </Cell>
          <Cell ss:StyleID="s70">
            <Data ss:Type="DateTime">
              <xsl:value-of select="//session/data/policy/ExpirationDate"/>
            </Data>
          </Cell>
          <Cell ss:Index="5" ss:StyleID="s64">
            <Data ss:Type="String">Writing Company:</Data>
          </Cell>
          <Cell ss:StyleID="s65">
            <Data ss:Type="String">
              <xsl:value-of select="//session/data/policy/WritingCompany"/>
            </Data>
          </Cell>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s64">
            <Data ss:Type="String">Quoted Date:</Data>
          </Cell>
          <Cell ss:StyleID="s70">
            <Data ss:Type="DateTime">
              <xsl:value-of select="//session/data/policy/QuotedDate"/>
            </Data>
          </Cell>
          <Cell ss:Index="5" ss:StyleID="s64">
            <Data ss:Type="String">NAICS Code:</Data>
          </Cell>
          <Cell ss:StyleID="s65">
            <Data ss:Type="String">
              <xsl:value-of select="//session/data/policy/NAICSCode"/>
            </Data>
          </Cell>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s64">
            <Data ss:Type="String">Primary Rating State:</Data>
          </Cell>
          <Cell ss:StyleID="s71">
            <Data ss:Type="String">
              <xsl:value-of select="//session/data/policy/PrimaryRatingState"/>
            </Data>
          </Cell>
          <Cell ss:Index="5" ss:StyleID="s64">
            <Data ss:Type="String">SIC Code:</Data>
          </Cell>
          <Cell ss:StyleID="s65">
            <Data ss:Type="String">
              <xsl:value-of select="//session/data/policy/SICCode"/>
            </Data>
          </Cell>
        </Row>
        <Row ss:AutoFitHeight="0" ss:Height="15">
          <Cell ss:Index="2" ss:StyleID="s64">
            <Data ss:Type="String">Term:</Data>
          </Cell>
          <Cell ss:StyleID="s65">
            <Data ss:Type="String">
              <xsl:value-of select="concat(//session/data/policy/TermFactor, ' Year')"/>
            </Data>
          </Cell>
          <Cell ss:Index="5" ss:StyleID="s64">
            <Data ss:Type="String">Producer Code:</Data>
          </Cell>
          <Cell ss:StyleID="s72">
            <Data ss:Type="String">
              <xsl:value-of select="//session/data/producerDetails/Code"/>
            </Data>
          </Cell>
        </Row>
        <Row ss:AutoFitHeight="0" ss:Height="15">
          <Cell ss:Index="2" ss:StyleID="s69"/>
          <Cell ss:Index="5" ss:StyleID="s73"/>
          <Cell ss:StyleID="s74"/>
        </Row>
        <!--Policy Info: End-->

        <!--Hyper Links: Start-->
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s76" ss:HRef="#'Coverages Detailed Report'!A1">
            <Data ss:Type="String">Coverages Detailed Report</Data>
          </Cell>
          <Cell ss:Index="6" ss:StyleID="s68"/>
        </Row>
        <Row ss:AutoFitHeight="0" ss:Height="15">
          <Cell ss:Index="2" ss:StyleID="s76" ss:HRef="#'Policy Forms'!A1">
            <Data ss:Type="String">Policy Forms</Data>
          </Cell>
          <Cell ss:Index="6" ss:StyleID="s68"/>
        </Row>
        <Row ss:AutoFitHeight="0" ss:Height="15">
          <Cell ss:Index="2" ss:StyleID="s77"/>
          <Cell ss:Index="6" ss:StyleID="s68"/>
        </Row>
        <!--Hyper Links: End-->

        <!--Premium Summary: Start-->
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s78">
            <Data ss:Type="String">Premium Summary</Data>
          </Cell>
          <Cell ss:StyleID="s79"/>
          <Cell ss:Index="6" ss:StyleID="s68"/>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s80">
            <Data ss:Type="String">Balance to Minimum Premium</Data>
          </Cell>
          <Cell ss:StyleID="s81">
            <Data ss:Type="String">
              <xsl:variable name="Premium" select="//session/data/policy/line[Type='Property']/coverage[Type='PolicyMinimum']/Premium"/>
              <xsl:choose>
                <xsl:when test="$Premium and $Premium!=0">
                  <xsl:value-of select="concat('$', format-number($Premium, '###,###,###.00'))" />
                </xsl:when>
                <xsl:when test="$Premium and $Premium=0">
                  <xsl:value-of select="'$0'"/>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="'N/A'"/>
                </xsl:otherwise>
              </xsl:choose>
            </Data>
          </Cell>
          <Cell ss:Index="6" ss:StyleID="s68"/>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s80">
            <Data ss:Type="String">Total Pre-Tax</Data>
          </Cell>
          <Cell ss:StyleID="s81">
            <Data ss:Type="String">
              <xsl:variable name="Premium" select="//session/data/policy/line[Type='Property']/PurePremium"/>
              <xsl:choose>
                <xsl:when test="$Premium and $Premium!=0">
                  <xsl:value-of select="concat('$', format-number($Premium, '###,###,###.00'))" />
                </xsl:when>
                <xsl:when test="$Premium and $Premium=0">
                  <xsl:value-of select="'$0'"/>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="'N/A'"/>
                </xsl:otherwise>
              </xsl:choose>
            </Data>
          </Cell>
          <Cell ss:Index="6" ss:StyleID="s68"/>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s80">
            <Data ss:Type="String">Tax Premium</Data>
          </Cell>
          <Cell ss:StyleID="s81">
            <Data ss:Type="String">
              <xsl:variable name="Premium" select="//session/data/policy/line[Type='Property']/TaxesSurcharges"/>
              <xsl:choose>
                <xsl:when test="$Premium and $Premium!=0">
                  <xsl:value-of select="concat('$', format-number($Premium, '###,###,###.00'))" />
                </xsl:when>
                <xsl:when test="$Premium and $Premium=0">
                  <xsl:value-of select="'$0'"/>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="'N/A'"/>
                </xsl:otherwise>
              </xsl:choose>
            </Data>
          </Cell>
          <Cell ss:Index="6" ss:StyleID="s68"/>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s80">
            <Data ss:Type="String">Total Pre-Finance Charge</Data>
          </Cell>
          <Cell ss:StyleID="s81">
            <Data ss:Type="String">
              <xsl:variable name="Premium" select="''"/>
              <xsl:choose>
                <xsl:when test="$Premium and $Premium!=0">
                  <xsl:value-of select="concat('$', format-number($Premium, '###,###,###.00'))" />
                </xsl:when>
                <xsl:when test="$Premium and $Premium=0">
                  <xsl:value-of select="'$0'"/>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="'N/A'"/>
                </xsl:otherwise>
              </xsl:choose>
            </Data>
          </Cell>
          <Cell ss:Index="6" ss:StyleID="s68"/>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s80">
            <Data ss:Type="String">Finance Charge</Data>
          </Cell>
          <Cell ss:StyleID="s81">
            <Data ss:Type="String">
              <xsl:variable name="Premium" select="//session/data/policy/line[Type='Property']/coverage[Type='LineFinanceCharge']/FCPremium"/>
              <xsl:choose>
                <xsl:when test="$Premium and $Premium!=0">
                  <xsl:value-of select="concat('$', format-number($Premium, '###,###,###.00'))" />
                </xsl:when>
                <xsl:when test="$Premium and $Premium=0">
                  <xsl:value-of select="'$0'"/>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="'N/A'"/>
                </xsl:otherwise>
              </xsl:choose>
            </Data>
          </Cell>
          <Cell ss:Index="6" ss:StyleID="s68"/>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s78">
            <Data ss:Type="String">Total Premium:</Data>
          </Cell>
          <Cell ss:StyleID="s82">
            <Data ss:Type="String">
              <xsl:variable name="Premium" select="//session/data/policy/line[Type='Property']/Premium"/>
              <xsl:choose>
                <xsl:when test="$Premium and $Premium!=0">
                  <xsl:value-of select="concat('$', format-number($Premium, '###,###,###.00'))" />
                </xsl:when>
                <xsl:when test="$Premium and $Premium=0">
                  <xsl:value-of select="'$0'"/>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="'N/A'"/>
                </xsl:otherwise>
              </xsl:choose>
            </Data>
          </Cell>
          <Cell ss:Index="6" ss:StyleID="s68"/>
        </Row>
        <Row ss:AutoFitHeight="0" ss:Height="15">
          <Cell ss:Index="2" ss:StyleID="s78">
            <Data ss:Type="String">Total Premium Written: </Data>
          </Cell>
          <Cell ss:StyleID="s82">
            <Data ss:Type="String">
              <xsl:variable name="Premium" select="//session/data/policy/line[Type='Property']/@written"/>
              <xsl:choose>
                <xsl:when test="$Premium and $Premium!=0">
                  <xsl:value-of select="concat('$', format-number($Premium, '###,###,###.00'))" />
                </xsl:when>
                <xsl:when test="$Premium and $Premium=0">
                  <xsl:value-of select="'$0'"/>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="'N/A'"/>
                </xsl:otherwise>
              </xsl:choose>
            </Data>
          </Cell>
          <Cell ss:StyleID="s83"/>
          <Cell ss:StyleID="s83"/>
          <Cell ss:StyleID="s84"/>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s85"/>
        </Row>
        <!--Premium Summary: End-->
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
            <ActiveCol>1</ActiveCol>
          </Pane>
        </Panes>
        <ProtectObjects>False</ProtectObjects>
        <ProtectScenarios>False</ProtectScenarios>
      </WorksheetOptions>
    </Worksheet>
  </xsl:template>
</xsl:stylesheet>
