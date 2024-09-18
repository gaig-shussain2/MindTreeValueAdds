<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl">
  <xsl:output method="xml" indent="yes"/>
  <xsl:template name="CovCountBasedOnLOB">
    <xsl:param name="Type" />
    <xsl:param name="LOB" />
    <xsl:variable name="Count">
      <xsl:choose>
        <xsl:when test="$LOB='Property'">
          <xsl:choose>
            <xsl:when test="$Type='line'">
              <xsl:value-of select="count(//session/data/policy/line[Type='Property']/coverage[(Indicator=1 or @deleted=1) and Indicators[Type='IsCovEndorsement']/bValue=0 and Type != 'AdditionalInterests' and Type != 'Miscellaneous'])" />
            </xsl:when>
            <xsl:when test="$Type='risk'">
              <xsl:value-of select="count(//session/data/policy/line[Type='Property']/risk/coverage[(Indicator=1 or @deleted=1) and (Type != 'Manuscript' and Type != 'MiscellaneousAllOther')])" />
            </xsl:when>
            <xsl:when test="$Type='endorsement'">
              <xsl:value-of select="count(//session/data/policy/line[Type='Property']/coverage[Indicators[Type='IsCovEndorsement']/bValue=1 and (Indicator=1 or @deleted=1)])" />
            </xsl:when>
            <xsl:when test="$Type='miscallother'">
              <xsl:value-of select="count(//session/data/policy/line[Type='Property']/coverage[(Indicator=1 or @deleted=1) and Type='Miscellaneous']) + count(/session/data/policy/line[Type='Property']/risk/coverage[Type='MiscellaneousAllOther' and (Indicator=1 or @deleted=1)]/coverage[Type='MiscellaneousAllOtherInstances' and (Indicator=1 or @deleted=1)])" />
            </xsl:when>
            <xsl:when test="$Type='msendorsement'">
              <xsl:value-of select="count(//session/data/policy/line[Type='Property']/risk/coverage[Type='Manuscript' and (Indicator=1 or @deleted=1)]/coverage[Type='ManuscriptInstances' and (Indicator=1 or @deleted=1)])" />
            </xsl:when>
            <xsl:when test="$Type='addnlendorse'">
              <xsl:value-of select="count(//session/data/account/additionalOtherInterest)" />
            </xsl:when>
            <xsl:when test="$Type='location'">
              <xsl:value-of select="count(//session/data/account/location)" />
            </xsl:when>
          </xsl:choose>
        </xsl:when>
      </xsl:choose>
    </xsl:variable>
    <xsl:value-of select="$Count"/>
  </xsl:template>
  <!--<xsl:template name="BuildCoverageRow"  xmlns="urn:schemas-microsoft-com:office:spreadsheet" xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet">
    <xsl:param name="CoverageName" />
    <xsl:param name="Indicator" />
    <xsl:param name="Deleted" />
    <xsl:param name="Location" />
    <xsl:param name="Classification" />
    <xsl:param name="Type" />
    <xsl:param name="PremiumBasis" />
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
            <xsl:when test="$Classification">
              <xsl:value-of select="$Classification"/>
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
      <Cell ss:StyleID="s90">
        <Data ss:Type="String">
          <xsl:choose>
            <xsl:when test="$PremiumBasis">
              <xsl:value-of select="$PremiumBasis"/>
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
            <xsl:when test="$_var and $_var=0">
              <xsl:value-of select="'$0'"/>
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
            <xsl:when test="$_var and $_var=0">
              <xsl:value-of select="'$0'"/>
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
            <xsl:when test="$_var and $_var=0">
              <xsl:value-of select="'$0'"/>
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
              <xsl:value-of select="concat('', format-number($_var, '###,###,###.00'))" />
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
  </xsl:template>-->
</xsl:stylesheet>
