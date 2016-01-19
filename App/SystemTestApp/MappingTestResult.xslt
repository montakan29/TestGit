<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:n="http://www.reuters.com/rst" xmlns:xs="http://www.w3.org/2001/XMLSchema" exclude-result-prefixes="xs xsi xsl" xmlns="http://www.reuters.com/rst">
  <xsl:namespace-alias stylesheet-prefix="n" result-prefix="#default"/>
  <xsl:output method="xml" encoding="UTF-8" indent="yes"/>
  <xsl:template match="/n:rstResult">
    <n:rstResult>
      <xsl:for-each select="n:UUIDTest">
        <n:UUIDTest>
          <xsl:value-of select="."/>
        </n:UUIDTest>
      </xsl:for-each>
      <xsl:for-each select="n:testID">
        <xsl:variable name="Vvar261_testID_long" select="number(.)"/>
        <n:testID>
          <xsl:value-of select="$Vvar261_testID_long"/>
        </n:testID>
      </xsl:for-each>
      <xsl:for-each select="n:userProfile">
        <n:userProfile>
          <xsl:for-each select="n:UUID">
            <xsl:variable name="Vvar267_UUID_string" select="string(.)"/>
            <n:UUID>
              <xsl:value-of select="$Vvar267_UUID_string"/>
            </n:UUID>
          </xsl:for-each>
          <xsl:for-each select="n:accountDomain">
            <xsl:variable name="Vvar271_accountDomain_string" select="string(.)"/>
            <n:accountDomain>
              <xsl:value-of select="$Vvar271_accountDomain_string"/>
            </n:accountDomain>
          </xsl:for-each>
          <xsl:for-each select="n:userName">
            <xsl:variable name="Vvar275_userName_string" select="string(.)"/>
            <n:userName>
              <xsl:value-of select="$Vvar275_userName_string"/>
            </n:userName>
          </xsl:for-each>
          <xsl:for-each select="n:email">
            <xsl:variable name="Vvar276_email_string" select="string(.)"/>
            <n:email>
              <xsl:value-of select="$Vvar276_email_string"/>
            </n:email>
          </xsl:for-each>
          <xsl:for-each select="n:contactName">
            <xsl:variable name="Vvar279_contactName_string" select="string(.)"/>
            <n:contactName>
              <xsl:value-of select="$Vvar279_contactName_string"/>
            </n:contactName>
          </xsl:for-each>
          <xsl:for-each select="n:location">
            <xsl:variable name="Vvar287_location_string" select="string(.)"/>
            <n:location>
              <xsl:for-each select="@CID">
                <xsl:variable name="Vvar286_CID_anySimpleType" select="."/>
                <xsl:attribute name="CID">
                  <xsl:value-of select="$Vvar286_CID_anySimpleType"/>
                </xsl:attribute>
              </xsl:for-each>
              <xsl:value-of select="$Vvar287_location_string"/>
            </n:location>
          </xsl:for-each>
          <xsl:for-each select="n:country">
            <xsl:variable name="Vvar295_country_string" select="string(.)"/>
            <n:country>
              <xsl:for-each select="@CID">
                <xsl:variable name="Vvar294_CID_anySimpleType" select="."/>
                <xsl:attribute name="CID">
                  <xsl:value-of select="$Vvar294_CID_anySimpleType"/>
                </xsl:attribute>
              </xsl:for-each>
              <xsl:value-of select="$Vvar295_country_string"/>
            </n:country>
          </xsl:for-each>
          <xsl:for-each select="n:global">
            <xsl:variable name="Vvar303_global_string" select="string(.)"/>
            <n:global>
              <xsl:for-each select="@CID">
                <xsl:variable name="Vvar302_CID_anySimpleType" select="."/>
                <xsl:attribute name="CID">
                  <xsl:value-of select="$Vvar302_CID_anySimpleType"/>
                </xsl:attribute>
              </xsl:for-each>
              <xsl:value-of select="$Vvar303_global_string"/>
            </n:global>
          </xsl:for-each>
          <xsl:for-each select="n:geoRegion">
            <xsl:variable name="Vvar307_geoRegion_string" select="string(.)"/>
            <n:geoRegion>
              <xsl:value-of select="$Vvar307_geoRegion_string"/>
            </n:geoRegion>
          </xsl:for-each>
          <xsl:for-each select="n:geoSubRegion">
            <xsl:variable name="Vvar311_geoSubRegion_string" select="string(.)"/>
            <n:geoSubRegion>
              <xsl:value-of select="$Vvar311_geoSubRegion_string"/>
            </n:geoSubRegion>
          </xsl:for-each>
          <xsl:for-each select="n:geoCountry">
            <xsl:variable name="Vvar315_geoCountry_string" select="string(.)"/>
            <n:geoCountry>
              <xsl:value-of select="$Vvar315_geoCountry_string"/>
            </n:geoCountry>
          </xsl:for-each>
        </n:userProfile>
      </xsl:for-each>
      <xsl:for-each select="n:server">
        <n:server>
          <xsl:for-each select="@name">
            <xsl:variable name="Vvar321_name_string" select="string(.)"/>
            <xsl:attribute name="name">
              <xsl:value-of select="$Vvar321_name_string"/>
            </xsl:attribute>
          </xsl:for-each>
          <xsl:for-each select="@version">
            <xsl:variable name="Vvar325_version_string" select="string(.)"/>
            <xsl:attribute name="version">
              <xsl:value-of select="$Vvar325_version_string"/>
            </xsl:attribute>
          </xsl:for-each>
          <xsl:for-each select="@compName">
            <xsl:variable name="Vvar329_compName_string" select="string(.)"/>
            <xsl:attribute name="compName">
              <xsl:value-of select="$Vvar329_compName_string"/>
            </xsl:attribute>
          </xsl:for-each>
          <xsl:for-each select="n:dataCenter">
            <xsl:variable name="Vvar333_dataCenter_string" select="string(.)"/>
            <n:dataCenter>
              <xsl:value-of select="$Vvar333_dataCenter_string"/>
            </n:dataCenter>
          </xsl:for-each>
          <xsl:for-each select="n:wasDateTime">
            <xsl:variable name="Vvar337_wasDateTime_string" select="string(.)"/>
            <n:wasDateTime>
              <xsl:value-of select="$Vvar337_wasDateTime_string"/>
            </n:wasDateTime>
          </xsl:for-each>
          <xsl:for-each select="n:dbsDateTime">
            <xsl:variable name="Vvar341_dbsDateTime_string" select="string(.)"/>
            <n:dbsDateTime>
              <xsl:value-of select="$Vvar341_dbsDateTime_string"/>
            </n:dbsDateTime>
          </xsl:for-each>
        </n:server>
      </xsl:for-each>
      <xsl:for-each select="n:client">
        <xsl:variable name="Vvar353_client_string" select="string(.)"/>
        <n:client>
          <xsl:for-each select="@name">
            <xsl:variable name="Vvar348_name_string" select="string(.)"/>
            <xsl:attribute name="name">
              <xsl:value-of select="$Vvar348_name_string"/>
            </xsl:attribute>
          </xsl:for-each>
          <xsl:for-each select="@version">
            <xsl:variable name="Vvar352_version_string" select="string(.)"/>
            <xsl:attribute name="version">
              <xsl:value-of select="$Vvar352_version_string"/>
            </xsl:attribute>
          </xsl:for-each>
          <xsl:value-of select="$Vvar353_client_string"/>
        </n:client>
      </xsl:for-each>
      <xsl:for-each select="n:product">
        <xsl:variable name="Vvar365_product_string" select="string(.)"/>
        <n:product>
          <xsl:for-each select="@name">
            <xsl:variable name="Vvar360_name_string" select="string(.)"/>
            <xsl:attribute name="name">
              <xsl:value-of select="$Vvar360_name_string"/>
            </xsl:attribute>
          </xsl:for-each>
          <xsl:for-each select="@version">
            <xsl:variable name="Vvar364_version_string" select="string(.)"/>
            <xsl:attribute name="version">
              <xsl:value-of select="$Vvar364_version_string"/>
            </xsl:attribute>
          </xsl:for-each>
          <xsl:value-of select="$Vvar365_product_string"/>
        </n:product>
      </xsl:for-each>
      <xsl:for-each select="n:machineID">
        <xsl:variable name="Vvar369_machineID_string" select="string(.)"/>
        <n:machineID>
          <xsl:value-of select="$Vvar369_machineID_string"/>
        </n:machineID>
      </xsl:for-each>
      <xsl:for-each select="n:runningMode">
        <xsl:variable name="Vvar373_runningMode_string" select="string(.)"/>
        <n:runningMode>
          <xsl:value-of select="$Vvar373_runningMode_string"/>
        </n:runningMode>
      </xsl:for-each>
      <xsl:for-each select="n:hasResultValue">
        <xsl:variable name="Vvar377_hasResultValue_string" select="string(.)"/>
        <n:hasResultValue>
          <xsl:value-of select="$Vvar377_hasResultValue_string"/>
        </n:hasResultValue>
      </xsl:for-each>
      <xsl:for-each select="n:localDateTime">
        <xsl:variable name="Vvar381_localDateTime_string" select="string(.)"/>
        <n:localDateTime>
          <xsl:value-of select="$Vvar381_localDateTime_string"/>
        </n:localDateTime>
      </xsl:for-each>
      <xsl:for-each select="n:requestXMLString">
        <xsl:variable name="Vvar385_requestXMLString_string" select="string(.)"/>
        <n:requestXMLString>
          <xsl:value-of select="$Vvar385_requestXMLString_string"/>
        </n:requestXMLString>
      </xsl:for-each>
      <xsl:for-each select="n:testedLanguage">
        <xsl:variable name="Vvar389_testedLanguage_language" select="string(.)"/>
        <n:testedLanguage>
          <xsl:value-of select="$Vvar389_testedLanguage_language"/>
        </n:testedLanguage>
      </xsl:for-each>
      <xsl:for-each select="n:refID">
        <xsl:variable name="Vvar401_refID_string" select="string(.)"/>
        <n:refID>
          <xsl:for-each select="@round">
            <xsl:variable name="Vvar396_round_int" select="number(.)"/>
            <xsl:attribute name="round">
              <xsl:value-of select="$Vvar396_round_int"/>
            </xsl:attribute>
          </xsl:for-each>
          <xsl:for-each select="@leftRounds">
            <xsl:variable name="Vvar400_leftRounds_int" select="number(.)"/>
            <xsl:attribute name="leftRounds">
              <xsl:value-of select="$Vvar400_leftRounds_int"/>
            </xsl:attribute>
          </xsl:for-each>
          <xsl:value-of select="$Vvar401_refID_string"/>
        </n:refID>
      </xsl:for-each>
      <xsl:for-each select="n:testGroups">
        <n:testGroups>
          <xsl:for-each select="n:testGroup">
            <xsl:variable name="Vvar415_testGroup_string" select="string(.)"/>
            <n:testGroup>
              <xsl:for-each select="@id">
                <xsl:variable name="Vvar410_id_string" select="string(.)"/>
                <xsl:attribute name="id">
                  <xsl:value-of select="$Vvar410_id_string"/>
                </xsl:attribute>
              </xsl:for-each>
              <xsl:for-each select="@lang">
                <xsl:variable name="Vvar414_lang_language" select="string(.)"/>
                <xsl:attribute name="lang">
                  <xsl:value-of select="$Vvar414_lang_language"/>
                </xsl:attribute>
              </xsl:for-each>
              <xsl:value-of select="$Vvar415_testGroup_string"/>
            </n:testGroup>
          </xsl:for-each>
        </n:testGroups>
      </xsl:for-each>
      <xsl:for-each select="n:testCases">
        <n:testCases>
          <xsl:for-each select="n:testCase">
            <n:testCase>
              <xsl:for-each select="@id">
                <xsl:variable name="Vvar423_id_string" select="string(.)"/>
                <xsl:attribute name="id">
                  <xsl:value-of select="$Vvar423_id_string"/>
                </xsl:attribute>
              </xsl:for-each>
              <xsl:for-each select="@type">
                <xsl:variable name="Vvar427_type_string" select="string(.)"/>
                <xsl:attribute name="type">
                  <xsl:value-of select="$Vvar427_type_string"/>
                </xsl:attribute>
              </xsl:for-each>
              <xsl:for-each select="@shouldBeRerun">
                <xsl:variable name="Vvar431_shouldBeRerun_string" select="string(.)"/>
                <xsl:attribute name="shouldBeRerun">
                  <xsl:value-of select="$Vvar431_shouldBeRerun_string"/>
                </xsl:attribute>
              </xsl:for-each>
              <xsl:for-each select="@ssiID">
                <xsl:variable name="Vvar435_ssiID_long" select="number(.)"/>
                <xsl:attribute name="ssiID">
                  <xsl:value-of select="$Vvar435_ssiID_long"/>
                </xsl:attribute>
              </xsl:for-each>
              <xsl:for-each select="n:title">
                <xsl:variable name="Vvar443_title_string" select="string(.)"/>
                <n:title>
                  <xsl:for-each select="@lang">
                    <xsl:variable name="Vvar442_lang_language" select="string(.)"/>
                    <xsl:attribute name="lang">
                      <xsl:value-of select="$Vvar442_lang_language"/>
                    </xsl:attribute>
                  </xsl:for-each>
                  <xsl:value-of select="$Vvar443_title_string"/>
                </n:title>
              </xsl:for-each>
              <xsl:for-each select="n:group">
                <xsl:variable name="Vvar447_group_string" select="string(.)"/>
                <n:group>
                  <xsl:value-of select="$Vvar447_group_string"/>
                </n:group>
              </xsl:for-each>
              <xsl:for-each select="n:valid">
                <xsl:variable name="Vvar451_valid_string" select="string(.)"/>
                <n:valid>
                  <xsl:value-of select="$Vvar451_valid_string"/>
                </n:valid>
              </xsl:for-each>
              <xsl:for-each select="n:ovalid">
                <xsl:variable name="Vvar455_ovalid_string" select="string(.)"/>
                <n:ovalid>
                  <xsl:value-of select="$Vvar455_ovalid_string"/>
                </n:ovalid>
              </xsl:for-each>
              <xsl:for-each select="n:value">
                <xsl:variable name="Vvar459_value_string" select="string(.)"/>
                <n:value>
                  <xsl:value-of select="$Vvar459_value_string"/>
                </n:value>
              </xsl:for-each>
              <xsl:for-each select="n:description">
                <xsl:variable name="Vvar467_description_string" select="string(.)"/>
                <n:description>
                  <xsl:for-each select="@lang">
                    <xsl:variable name="Vvar466_lang_language" select="string(.)"/>
                    <xsl:attribute name="lang">
                      <xsl:value-of select="$Vvar466_lang_language"/>
                    </xsl:attribute>
                  </xsl:for-each>
                  <xsl:value-of select="$Vvar467_description_string"/>
                </n:description>
              </xsl:for-each>
              <xsl:for-each select="n:recommendation">
                <xsl:variable name="Vvar475_recommendation_string" select="string(.)"/>
                <n:recommendation>
                  <xsl:for-each select="@lang">
                    <xsl:variable name="Vvar474_lang_language" select="string(.)"/>
                    <xsl:attribute name="lang">
                      <xsl:value-of select="$Vvar474_lang_language"/>
                    </xsl:attribute>
                  </xsl:for-each>
                  <xsl:value-of select="$Vvar475_recommendation_string"/>
                </n:recommendation>
              </xsl:for-each>
              <xsl:for-each select="n:orecommendation">
                <xsl:variable name="Vvar483_orecommendation_string" select="string(.)"/>
                <n:orecommendation>
                  <xsl:for-each select="@lang">
                    <xsl:variable name="Vvar482_lang_language" select="string(.)"/>
                    <xsl:attribute name="lang">
                      <xsl:value-of select="$Vvar482_lang_language"/>
                    </xsl:attribute>
                  </xsl:for-each>
                  <xsl:value-of select="$Vvar483_orecommendation_string"/>
                </n:orecommendation>
              </xsl:for-each>
              <xsl:for-each select="n:object">
                <xsl:variable name="Vvar487_object_string" select="string(.)"/>
                <n:object>
                  <xsl:value-of select="$Vvar487_object_string"/>
                </n:object>
              </xsl:for-each>
              <xsl:for-each select="n:rphURL">
                <xsl:variable name="Vvar491_rphURL_string" select="string(.)"/>
                <n:rphURL>
                  <xsl:value-of select="$Vvar491_rphURL_string"/>
                </n:rphURL>
              </xsl:for-each>
              <xsl:for-each select="n:resultID">
                <xsl:variable name="Vvar495_resultID_string" select="string(.)"/>
                <n:resultID>
                  <xsl:value-of select="$Vvar495_resultID_string"/>
                </n:resultID>
              </xsl:for-each>
              <xsl:for-each select="n:measurement">
                <xsl:variable name="Vvar503_measurement_string" select="string(.)"/>
                <n:measurement>
                  <xsl:for-each select="@unit">
                    <xsl:variable name="Vvar502_unit_string" select="string(.)"/>
                    <xsl:attribute name="unit">
                      <xsl:value-of select="$Vvar502_unit_string"/>
                    </xsl:attribute>
                  </xsl:for-each>
                  <xsl:value-of select="$Vvar503_measurement_string"/>
                </n:measurement>
              </xsl:for-each>
              <xsl:for-each select="n:hasTroubleShootingTests">
                <n:hasTroubleShootingTests>
                  <xsl:for-each select="n:testID">
                    <xsl:variable name="Vvar509_testID_string" select="string(.)"/>
                    <n:testID>
                      <xsl:value-of select="$Vvar509_testID_string"/>
                    </n:testID>
                  </xsl:for-each>
                </n:hasTroubleShootingTests>
              </xsl:for-each>
            </n:testCase>
          </xsl:for-each>
        </n:testCases>
      </xsl:for-each>
    </n:rstResult>
  </xsl:template>
</xsl:stylesheet>
