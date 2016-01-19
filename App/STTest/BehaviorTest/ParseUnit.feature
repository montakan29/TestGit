Feature: ParseUnit
	In order to make data readable
	As a support
	I want to convert data in a specific unit

Scenario Outline: Eikon3 Convert data in a specific unit
	Given The Testcases are generated from <StatName>, <StatValue> and <StatDesc>
	When Load xml result from <XMLResult> 
	And Convert Testcases to TestResult object prepared for json converter
	Then The test result value for <StatName> should be <ExpectedValue>
	Examples:
	| XMLResult | StatName		| StatValue						| StatDesc										| ExpectedValue							|
	| ST3.xml	| 112			| 10							| 10 ms											| 10									|
	| ST3.xml	| 124			| 51200							| 50 MB											| 0.05									|
	| ST3.xml	| 3				| 2933							| 2.9 GHz										| 2.9									|
	| ST3.xml	| 39			| 3								| Drive C: has 9.33 GB of free space.			| 9553.92								|
	| ST3.xml	| 4				| 2147483648;2147016704         | 2 GB (usable 2.0 GB)							| 2048									|
	| ST3.xml	| 6				| 32					        | 32 bits.										| 32									|
	| ST3.xml	| 95			| 50000					        | 50000 bps										| 48.83									|
	| ST3.xml	| 96			| 60000					        | 60000 bps										| 58.59									|
	| ST3.xml	| runmode		|						        | 												| auto									|
	| ST3.xml	| total			|						        | 												| 81									|
	| ST3.xml	| pass			|						        | 												| 28									|
	| ST3.xml	| info			|						        | 												| 39									|
	| ST3.xml	| warn			|						        | 												| 5[90,18,95,96,112]					|
	| ST3.xml	| alert			|						        | 												| 9[4,63,77,42,40,111,93,59,60]			|
	| ST3.xml	| fail			|						        | 												| 0										|
	| ST3.xml	| other			|						        | 												| 0										|
	| ST3.xml	| product		|						        | 												| est									|
	| ST3.xml	| servname		|						        | 												| US1I-CTKUITK01						|	
	| ST3.xml	| envname		|						        | 												| AppEngine								|	

	Scenario Outline: Eikon4 Convert data in a specific unit
	Given The Testcases are generated from <StatName>, <StatValue> and <StatDesc>
	When Load xml result from <XMLResult> 
	And Convert Testcases to TestResult object prepared for json converter
	Then The test result value for <StatName> should be <ExpectedValue>
	Examples:
	| XMLResult | StatName | StatValue      | StatDesc                               | ExpectedValue                          |
	| ST4.xml   | 141      | 2              | 2                                      | 2                                      |
	| ST4.xml   | 124      | PASS           | 250 MB                                 | 0.24                                   |
	| ST4.xml   | 3        | CPUSpeedOK     | 3.4 GHz                                | 3.4                                    |
	| ST4.xml   | 39       | PREINSTALL_OK  | Drive C: has 7.51 GB of free space.    | 7690.24                                |
	| ST4.xml   | 4        | memoryOK       | 16 GB (15.9 GB usable)                 | 16384                                  |
	| ST4.xml   | 6        | color_depth_ok | 32-bit                                 | 32                                     |
	| ST4.xml   | 95       | WARNING        | Download Bandwidth cannot be measured. | 0                                      |
	| ST4.xml   | 96       | WARNING        | Upload Bandwidth cannot be measured.   | 0                                      |
	| ST4.xml   | runmode  |                |                                        | manual                                 |
	| ST4.xml   | total    |                |                                        | 76                                     |
	| ST4.xml   | pass     |                |                                        | 34                                     |
	| ST4.xml   | info     |                |                                        | 38                                     |
	| ST4.xml   | warn     |                |                                        | 3[95,96,112]                           |
	| ST4.xml   | alert    |                |                                        | 1[138]                                 |
	| ST4.xml   | fail     |                |                                        | 0                                      |
	| ST4.xml   | other    |                |                                        | 0                                      |
	| ST4.xml   | product  |                |                                        | ewm                                    |
	| ST4.xml   | servname |                |                                        | US1I-CTKUITK01                         |
	| ST4.xml   | envname  |                |                                        | AppEngine                              |
	| ST4_1.xml | warn     |                |                                        | 13[36,137,21,110,1004,7,8,19,11,124,95 |
	| ST4_1.xml	| warn.1   |				| 										 | ,96,112]								  |
	| ST4_1.xml	| alert	   |				| 										 | 2[136,138]							  |
	| ST4_1.xml	| fail	   |				| 										 | 0									  |	