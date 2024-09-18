<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="xml" indent="yes"/>
  <xsl:template name="Build_Summary_Worksheet" match="/" xmlns="urn:schemas-microsoft-com:office:spreadsheet" xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet" xmlns:html="http://www.w3.org/TR/REC-html40">
    <Worksheet ss:Name="Summary Page">
      <Table ss:ExpandedColumnCount="6" ss:ExpandedRowCount="23" x:FullColumns="1" x:FullRows="1" ss:StyleID="s63" ss:DefaultRowHeight="15">
        <Column ss:Index="2" ss:StyleID="s63" ss:AutoFitWidth="0" ss:Width="188.25"/>
        <Column ss:StyleID="s63" ss:AutoFitWidth="0" ss:Width="108.75"/>
        <Column ss:StyleID="s63" ss:AutoFitWidth="0" ss:Width="54.75"/>
        <Column ss:StyleID="s63" ss:Width="89.25"/>
        <Column ss:StyleID="s63" ss:Width="131.25"/>
        <Row ss:AutoFitHeight="0"/>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s64">
            <Data ss:Type="String">Policy/Submission Summary</Data>
          </Cell>
          <Cell ss:StyleID="s65"/>
          <Cell ss:StyleID="s65"/>
          <Cell ss:StyleID="s65"/>
          <Cell ss:StyleID="s66"/>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s67">
            <Data ss:Type="String">Submission ID:</Data>
          </Cell>
          <Cell ss:StyleID="s68">
            <Data ss:Type="Number">
              <xsl:value-of select="//session/data/policy/SubmissionID"/>
            </Data>
          </Cell>
          <Cell ss:StyleID="s69"/>
          <Cell ss:StyleID="s67">
            <Data ss:Type="String">Line of Business:</Data>
          </Cell>
          <Cell ss:StyleID="s67">
            <Data ss:Type="String">
              <xsl:value-of select="//session/data/policy/LineOfBusiness"/>
            </Data>
          </Cell>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s67">
            <Data ss:Type="String">Policy Number:</Data>
          </Cell>
          <Cell ss:StyleID="s68">
            <Data ss:Type="String">
              <xsl:value-of select="//session/data/policy/PolicyNumber"/>
            </Data>
          </Cell>
          <Cell ss:StyleID="s69"/>
          <Cell ss:StyleID="s67">
            <Data ss:Type="String">Policy Symbol:</Data>
          </Cell>
          <Cell ss:StyleID="s67">
            <Data ss:Type="String">
              <xsl:value-of select="//session/data/policy/PolicyNumberSymbol"/>
            </Data>
          </Cell>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s67">
            <Data ss:Type="String">Transection Type:</Data>
          </Cell>
          <Cell ss:StyleID="s68">
            <Data ss:Type="String">
              <xsl:value-of select="//session/data/policy/CurrentTransaction"/>
            </Data>
          </Cell>
          <Cell ss:StyleID="s69"/>
          <Cell ss:Index="6" ss:StyleID="s70"/>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s67">
            <Data ss:Type="String">Policy Effective Date: </Data>
          </Cell>
          <Cell ss:StyleID="s71">
            <Data ss:Type="DateTime">
              <xsl:value-of select="//session/data/policy/EffectiveDate"/>
            </Data>
          </Cell>
          <Cell ss:StyleID="s72"/>
          <Cell ss:StyleID="s67">
            <Data ss:Type="String">Business Unit:</Data>
          </Cell>
          <Cell ss:StyleID="s67">
            <Data ss:Type="String">
              <xsl:value-of select="//session/data/policy/businessUnit/Code"/>
            </Data>
          </Cell>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s67">
            <Data ss:Type="String">Policy Expiration Date: </Data>
          </Cell>
          <Cell ss:StyleID="s71">
            <Data ss:Type="DateTime">
              <xsl:value-of select="//session/data/policy/ExpirationDate"/>
            </Data>
          </Cell>
          <Cell ss:StyleID="s72"/>
          <Cell ss:StyleID="s67">
            <Data ss:Type="String">Writing Company:</Data>
          </Cell>
          <Cell ss:StyleID="s67">
            <Data ss:Type="String">
              <xsl:value-of select="//session/data/policy/WritingCompany"/>
            </Data>
          </Cell>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s67">
            <Data ss:Type="String">Quoted Date:</Data>
          </Cell>
          <Cell ss:StyleID="s71">
            <Data ss:Type="DateTime">
              <xsl:value-of select="//session/data/policy/QuotedDate"/>
            </Data>
          </Cell>
          <Cell ss:StyleID="s72"/>
          <Cell ss:StyleID="s67">
            <Data ss:Type="String">NAICS Code:</Data>
          </Cell>
          <Cell ss:StyleID="s73">
            <Data ss:Type="Number">
              <xsl:value-of select="//session/data/policy/NAICSCode"/>
            </Data>
          </Cell>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s67">
            <Data ss:Type="String">Primary Rating State:</Data>
          </Cell>
          <Cell ss:StyleID="s68">
            <Data ss:Type="String">
              <xsl:value-of select="//session/data/policy/PrimaryRatingState"/>
            </Data>
          </Cell>
          <Cell ss:StyleID="s69"/>
          <Cell ss:StyleID="s67">
            <Data ss:Type="String">SIC Code:</Data>
          </Cell>
          <Cell ss:StyleID="s73">
            <Data ss:Type="Number">
              <xsl:value-of select="//session/data/policy/SICCode"/>
            </Data>
          </Cell>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s67">
            <Data ss:Type="String">Term:</Data>
          </Cell>
          <Cell ss:StyleID="s68">
            <Data ss:Type="String">
              <xsl:value-of select="concat(//session/data/policy/TermFactor, ' Year')"/>
            </Data>
          </Cell>
          <Cell ss:StyleID="s69"/>
          <Cell ss:StyleID="s67">
            <Data ss:Type="String">Producer Code:</Data>
          </Cell>
          <Cell ss:StyleID="s73">
            <Data ss:Type="Number">
              <xsl:value-of select="//session/data/producerDetails/Code"/>
            </Data>
          </Cell>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s74"/>
          <Cell ss:Index="6" ss:StyleID="s70"/>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s75">
            <Data ss:Type="String">Premium Summary</Data>
          </Cell>
          <Cell ss:StyleID="s76"/>
          <Cell ss:StyleID="s76"/>
          <Cell ss:StyleID="s75">
            <Data ss:Type="String">Detailed Reports</Data>
          </Cell>
          <Cell ss:StyleID="s77"/>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s78">
            <Data ss:Type="String">Balance to Minimum Premium</Data>
          </Cell>
          <Cell ss:StyleID="s79">
            <Data ss:Type="Number">
              <xsl:value-of select="//session/data/policy/line/coverage[Type='PolicyMinimum']/Premium"/>
            </Data>
          </Cell>
          <Cell ss:StyleID="s80"/>
          <Cell ss:StyleID="s81"/>
          <Cell ss:StyleID="s83" ss:HRef="#'Coverages Detailed Report'!A1">
            <Data ss:Type="String">Coverages Detailed Report</Data>
          </Cell>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s67">
            <Data ss:Type="String">Total Pre-Tax</Data>
          </Cell>
          <Cell ss:StyleID="s79">
            <Data ss:Type="Number">
              <xsl:value-of select="//session/data/TotalPurePremium/text()"/>
            </Data>
          </Cell>
          <Cell ss:StyleID="s84"/>
          <Cell ss:StyleID="s85"/>
          <Cell ss:StyleID="s86" ss:HRef="#'Policy Forms'!A1">
            <Data ss:Type="String">Policy Forms</Data>
          </Cell>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s67">
            <Data ss:Type="String">Taxes &amp; Surcharges </Data>
          </Cell>
          <Cell ss:StyleID="s79">
            <Data ss:Type="Number">
              <xsl:value-of select="//session/data/TotalTaxesSurcharges/text()"/>
            </Data>
          </Cell>
          <Cell ss:StyleID="s80"/>
          <Cell ss:Index="6" ss:StyleID="s70"/>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s67">
            <Data ss:Type="String">Total Pre-Finance Charge</Data>
          </Cell>
          <Cell ss:StyleID="s79">
            <Data ss:Type="Number">
              <xsl:value-of select="//session/data/TotalPurePremium/text() - sum(//session/data/policy/line/coverage[Type='LineFinanceCharge']/FCPremium)"/>
            </Data>
          </Cell>
          <Cell ss:StyleID="s80"/>
          <Cell ss:Index="6" ss:StyleID="s70"/>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s67">
            <Data ss:Type="String">Finance Charge</Data>
          </Cell>
          <Cell ss:StyleID="s79">
            <Data ss:Type="Number">
              <xsl:choose>
                <xsl:when test="//session/data/policy/LineOfBusiness='CommercialAuto'">
                  <xsl:value-of select="//session/data/policy/InstallmentFee"/>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="sum(//session/data/policy/line/coverage[Type='LineFinanceCharge']/FCPremium)"/>
                </xsl:otherwise>
              </xsl:choose>
            </Data>
          </Cell>
          <Cell ss:StyleID="s80"/>
          <Cell ss:Index="6" ss:StyleID="s70"/>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s87">
            <Data ss:Type="String">Total Premium:</Data>
          </Cell>
          <Cell ss:StyleID="s79">
            <Data ss:Type="Number">
              <xsl:value-of select="//session/data/policy/Premium"/>
            </Data>
          </Cell>
          <Cell ss:StyleID="s88"/>
          <Cell ss:Index="6" ss:StyleID="s70"/>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s74"/>
          <Cell ss:StyleID="s89"/>
          <Cell ss:StyleID="s89"/>
          <Cell ss:Index="6" ss:StyleID="s70"/>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s87">
            <Data ss:Type="String">Total Premium Written: </Data>
          </Cell>
          <Cell ss:StyleID="s79">
            <Data ss:Type="Number">
              <xsl:value-of select="//session/data/policy/PremiumWritten"/>
            </Data>
          </Cell>
          <Cell ss:StyleID="s88"/>
          <Cell ss:Index="6" ss:StyleID="s70"/>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s87">
            <Data ss:Type="String">Total Premium Change: </Data>
          </Cell>
          <Cell ss:StyleID="s79">
            <Data ss:Type="Number">
              <xsl:value-of select="//session/data/policy/PremiumChange"/>
            </Data>
          </Cell>
          <Cell ss:StyleID="s88"/>
          <Cell ss:Index="6" ss:StyleID="s70"/>
        </Row>
        <Row ss:AutoFitHeight="0">
          <Cell ss:Index="2" ss:StyleID="s87">
            <Data ss:Type="String">Total Premium Prior:</Data>
          </Cell>
          <Cell ss:StyleID="s79">
            <Data ss:Type="Number">
              <xsl:value-of select="//session/data/policy/PremiumPrior"/>
            </Data>
          </Cell>
          <Cell ss:StyleID="s90"/>
          <Cell ss:StyleID="s91"/>
          <Cell ss:StyleID="s92"/>
        </Row>
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
        <Panes>
          <Pane>
            <Number>3</Number>
            <ActiveRow>2</ActiveRow>
            <ActiveCol>1</ActiveCol>
          </Pane>
        </Panes>
        <ProtectObjects>False</ProtectObjects>
        <ProtectScenarios>False</ProtectScenarios>
      </WorksheetOptions>
    </Worksheet>
  </xsl:template>
</xsl:stylesheet>