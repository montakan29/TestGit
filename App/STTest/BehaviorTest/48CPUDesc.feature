Feature: 48CPUDesc
	In order to put data into OpsConsole which only support 40 charector
	As a support
	I want to convert data in order to contrain it into OpsConsole db

	Scenario Outline: 48. CPU Description
	Given The Testcases are generated from <StatName>, <StatValue> and <StatDesc>
	When Load xml result from <XMLResult> 
	And Convert Testcases to TestResult object prepared for json converter
	Then The test result value for <StatName> should be <ExpectedValue>
	Examples:
	| XMLResult | StatName| StatValue| StatDesc							 | ExpectedValue							|
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           L5520  @ 2.27GHz  | Intel(R) Xeon(R) CPU L5520               |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           L5520  @ 2.27GHz  | Intel(R) Xeon(R) CPU L5520               |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4770 CPU @ 3.40GHz          | Intel(R) Core(TM) i7-4770 CPU            |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           E5606  @ 2.13GHz  | Intel(R) Xeon(R) CPU E5606               |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4670K CPU @ 3.40GHz         | Intel(R) Core(TM) i5-4670K CPU           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E3-1246 v3 @ 3.50GHz        | Intel(R) Xeon(R) CPU E3-1246 v3          |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E3-1245 v3 @ 3.40GHz        | Intel(R) Xeon(R) CPU E3-1245 v3          |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-1603 0 @ 2.80GHz         | Intel(R) Xeon(R) CPU E5-1603 0           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E3-1245 V2 @ 3.40GHz        | Intel(R) Xeon(R) CPU E3-1245 V2          |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2650 0 @ 2.00GHz         | Intel(R) Xeon(R) CPU E5-2650 0           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-5820K CPU @ 3.30GHz         | Intel(R) Core(TM) i7-5820K CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7 CPU         960  @ 3.20GHz  | Intel(R) Core(TM) i7 CPU 960             |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2630 v3 @ 2.40GHz        | Intel(R) Xeon(R) CPU E5-2630 v3          |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-1607 v3 @ 3.10GHz        | Intel(R) Xeon(R) CPU E5-1607 v3          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4790 CPU @ 3.60GHz          | Intel(R) Core(TM) i7-4790 CPU            |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-1607 0 @ 3.00GHz         | Intel(R) Xeon(R) CPU E5-1607 0           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7 CPU         870  @ 2.93GHz  | Intel(R) Core(TM) i7 CPU 870             |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           E5607  @ 2.27GHz  | Intel(R) Xeon(R) CPU E5607               |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-3820 CPU @ 3.60GHz          | Intel(R) Core(TM) i7-3820 CPU            |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2620 0 @ 2.00GHz         | Intel(R) Xeon(R) CPU E5-2620 0           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2670 0 @ 2.60GHz         | Intel(R) Xeon(R) CPU E5-2670 0           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7 CPU         920  @ 2.67GHz  | Intel(R) Core(TM) i7 CPU 920             |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           W3550  @ 3.07GHz  | Intel(R) Xeon(R) CPU W3550               |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-2400 CPU @ 3.10GHz          | Intel(R) Core(TM) i5-2400 CPU            |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2620 v3 @ 2.40GHz        | Intel(R) Xeon(R) CPU E5-2620 v3          |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2698 v3 @ 2.30GHz        | Intel(R) Xeon(R) CPU E5-2698 v3          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4440S CPU @ 2.80GHz         | Intel(R) Core(TM) i5-4440S CPU           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E31220 @ 3.10GHz            | Intel(R) Xeon(R) CPU E31220              |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5 CPU         650  @ 3.20GHz  | Intel(R) Core(TM) i5 CPU 650             |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E3-1240 V2 @ 3.40GHz        | Intel(R) Xeon(R) CPU E3-1240 V2          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-3770 CPU @ 3.40GHz          | Intel(R) Core(TM) i7-3770 CPU            |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     E8400  @ 3.00GHz  | Intel(R) Core(TM)2 Duo CPU E8400         |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-2600 CPU @ 3.40GHz          | Intel(R) Core(TM) i7-2600 CPU            |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7 CPU         860  @ 2.80GHz  | Intel(R) Core(TM) i7 CPU 860             |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     E7500  @ 2.93GHz  | Intel(R) Core(TM)2 Duo CPU E7500         |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           X5672  @ 3.20GHz  | Intel(R) Xeon(R) CPU X5672               |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-3570 CPU @ 3.40GHz          | Intel(R) Core(TM) i5-3570 CPU            |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           W3680  @ 3.33GHz  | Intel(R) Xeon(R) CPU W3680               |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-1620 0 @ 3.60GHz         | Intel(R) Xeon(R) CPU E5-1620 0           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-2600K CPU @ 3.40GHz         | Intel(R) Core(TM) i7-2600K CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-3470 CPU @ 3.20GHz          | Intel(R) Core(TM) i5-3470 CPU            |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2620 v2 @ 2.10GHz        | Intel(R) Xeon(R) CPU E5-2620 v2          |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           E5620  @ 2.40GHz  | Intel(R) Xeon(R) CPU E5620               |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           X5550  @ 2.67GHz  | Intel(R) Xeon(R) CPU X5550               |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           W3503  @ 2.40GHz  | Intel(R) Xeon(R) CPU W3503               |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2687W v3 @ 3.10GHz       | Intel(R) Xeon(R) CPU E5-2687W v3         |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2643 v2 @ 3.50GHz        | Intel(R) Xeon(R) CPU E5-2643 v2          |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2643 0 @ 3.30GHz         | Intel(R) Xeon(R) CPU E5-2643 0           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-2120 CPU @ 3.30GHz          | Intel(R) Core(TM) i3-2120 CPU            |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           E5630  @ 2.53GHz  | Intel(R) Xeon(R) CPU E5630               |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2697 v2 @ 2.70GHz        | Intel(R) Xeon(R) CPU E5-2697 v2          |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           X5650  @ 2.67GHz  | Intel(R) Xeon(R) CPU X5650               |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-3220 CPU @ 3.30GHz          | Intel(R) Core(TM) i3-3220 CPU            |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2695 v2 @ 2.40GHz        | Intel(R) Xeon(R) CPU E5-2695 v2          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-2500 CPU @ 3.30GHz          | Intel(R) Core(TM) i5-2500 CPU            |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2640 0 @ 2.50GHz         | Intel(R) Xeon(R) CPU E5-2640 0           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           X7550  @ 2.00GHz  | Intel(R) Xeon(R) CPU X7550               |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2670 v2 @ 2.50GHz        | Intel(R) Xeon(R) CPU E5-2670 v2          |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           E5540  @ 2.53GHz  | Intel(R) Xeon(R) CPU E5540               |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4670 CPU @ 3.40GHz          | Intel(R) Core(TM) i5-4670 CPU            |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4570 CPU @ 3.20GHz          | Intel(R) Core(TM) i5-4570 CPU            |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-3930K CPU @ 3.20GHz         | Intel(R) Core(TM) i7-3930K CPU           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           W3530  @ 2.80GHz  | Intel(R) Xeon(R) CPU W3530               |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2603 0 @ 1.80GHz         | Intel(R) Xeon(R) CPU E5-2603 0           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4590 CPU @ 3.30GHz          | Intel(R) Core(TM) i5-4590 CPU            |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-2310 CPU @ 2.90GHz          | Intel(R) Core(TM) i5-2310 CPU            |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-1630 v3 @ 3.70GHz        | Intel(R) Xeon(R) CPU E5-1630 v3          |
	| ST4.xml | 48 |  | AMD A4-5300 APU with Radeon(tm) HD Graphics      | AMD A4-5300 APU with #4                |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-1620 v3 @ 3.50GHz        | Intel(R) Xeon(R) CPU E5-1620 v3          |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-1620 v2 @ 3.70GHz        | Intel(R) Xeon(R) CPU E5-1620 v2          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4690 CPU @ 3.50GHz          | Intel(R) Core(TM) i5-4690 CPU            |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4210U CPU @ 1.70GHz         | Intel(R) Core(TM) i5-4210U CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-3330 CPU @ 3.00GHz          | Intel(R) Core(TM) i5-3330 CPU            |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E3-1280 V2 @ 3.60GHz        | Intel(R) Xeon(R) CPU E3-1280 V2          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-3320M CPU @ 2.60GHz         | Intel(R) Core(TM) i5-3320M CPU           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2687W v2 @ 3.40GHz       | Intel(R) Xeon(R) CPU E5-2687W v2         |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4200U CPU @ 1.60GHz         | Intel(R) Core(TM) i5-4200U CPU           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-1660 0 @ 3.30GHz         | Intel(R) Xeon(R) CPU E5-1660 0           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           E5520  @ 2.27GHz  | Intel(R) Xeon(R) CPU E5520               |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           X5647  @ 2.93GHz  | Intel(R) Xeon(R) CPU X5647               |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2630 0 @ 2.30GHz         | Intel(R) Xeon(R) CPU E5-2630 0           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7 CPU         975  @ 3.33GHz  | Intel(R) Core(TM) i7 CPU 975             |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4460  CPU @ 3.20GHz         | Intel(R) Core(TM) i5-4460 CPU            |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) CPU G2120 @ 3.10GHz          | Intel(R) Pentium(R) CPU G2120            |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2623 v3 @ 3.00GHz        | Intel(R) Xeon(R) CPU E5-2623 v3          |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           W3690  @ 3.47GHz  | Intel(R) Xeon(R) CPU W3690               |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           X5690  @ 3.47GHz  | Intel(R) Xeon(R) CPU X5690               |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           E5640  @ 2.67GHz  | Intel(R) Xeon(R) CPU E5640               |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           E5405  @ 2.00GHz  | Intel(R) Xeon(R) CPU E5405               |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           X5660  @ 2.80GHz  | Intel(R) Xeon(R) CPU X5660               |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) CPU G2030 @ 3.00GHz          | Intel(R) Pentium(R) CPU G2030            |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7 CPU         950  @ 3.07GHz  | Intel(R) Core(TM) i7 CPU 950             |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-1607 v2 @ 3.00GHz        | Intel(R) Xeon(R) CPU E5-1607 v2          |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           X5680  @ 3.33GHz  | Intel(R) Xeon(R) CPU X5680               |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-2520M CPU @ 2.50GHz         | Intel(R) Core(TM) i5-2520M CPU           |
	| ST4.xml | 48 |  | AMD Athlon(tm) 64 X2 Dual Core Processor 5000+   | #1 64 X2 Dual Core Processor 5000+     |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-4150 CPU @ 3.50GHz          | Intel(R) Core(TM) i3-4150 CPU            |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           X5570  @ 2.93GHz  | Intel(R) Xeon(R) CPU X5570               |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4790S CPU @ 3.20GHz         | Intel(R) Core(TM) i7-4790S CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Quad CPU    Q9505  @ 2.83GHz  | Intel(R) Core(TM)2 Quad CPU Q9505        |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2630 v2 @ 2.60GHz        | Intel(R) Xeon(R) CPU E5-2630 v2          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4300U CPU @ 1.90GHz         | Intel(R) Core(TM) i5-4300U CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-3667U CPU @ 2.00GHz         | Intel(R) Core(TM) i7-3667U CPU           |
	| ST4.xml | 48 |  | AMD FX(tm)-6300 Six-Core Processor               | AMD FX(tm)-6300 Six-Core Processor       |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-3240 CPU @ 3.40GHz          | Intel(R) Core(TM) i3-3240 CPU            |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-1650 v2 @ 3.50GHz        | Intel(R) Xeon(R) CPU E5-1650 v2          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4930K CPU @ 3.40GHz         | Intel(R) Core(TM) i7-4930K CPU           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2609 0 @ 2.40GHz         | Intel(R) Xeon(R) CPU E5-2609 0           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2609 v2 @ 2.50GHz        | Intel(R) Xeon(R) CPU E5-2609 v2          |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           X5687  @ 3.60GHz  | Intel(R) Xeon(R) CPU X5687               |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-3770K CPU @ 3.50GHz         | Intel(R) Core(TM) i7-3770K CPU           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2660 0 @ 2.20GHz         | Intel(R) Xeon(R) CPU E5-2660 0           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           X5670  @ 2.93GHz  | Intel(R) Xeon(R) CPU X5670               |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4710MQ CPU @ 2.50GHz        | Intel(R) Core(TM) i7-4710MQ CPU          |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           E5649  @ 2.53GHz  | Intel(R) Xeon(R) CPU E5649               |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Quad CPU    Q8200  @ 2.33GHz  | Intel(R) Core(TM)2 Quad CPU Q8200        |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           L5630  @ 2.13GHz  | Intel(R) Xeon(R) CPU L5630               |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E3-1225 v3 @ 3.20GHz        | Intel(R) Xeon(R) CPU E3-1225 v3          |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           E5506  @ 2.13GHz  | Intel(R) Xeon(R) CPU E5506               |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4600U CPU @ 2.10GHz         | Intel(R) Core(TM) i7-4600U CPU           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2687W 0 @ 3.10GHz        | Intel(R) Xeon(R) CPU E5-2687W 0          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5 CPU         750  @ 2.67GHz  | Intel(R) Core(TM) i5 CPU 750             |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-3550 CPU @ 3.30GHz          | Intel(R) Core(TM) i5-3550 CPU            |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Extreme CPU X9650  @ 3.00GHz  | Intel(R) Core(TM)2 Extreme CPU X9650     |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-2100 CPU @ 3.10GHz          | Intel(R) Core(TM) i3-2100 CPU            |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4810MQ CPU @ 2.80GHz        | Intel(R) Core(TM) i7-4810MQ CPU          |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           W3565  @ 3.20GHz  | Intel(R) Xeon(R) CPU W3565               |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4430 CPU @ 3.00GHz          | Intel(R) Core(TM) i5-4430 CPU            |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-3340M CPU @ 2.70GHz         | Intel(R) Core(TM) i5-3340M CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-5300U CPU @ 2.30GHz         | Intel(R) Core(TM) i5-5300U CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4570S CPU @ 2.90GHz         | Intel(R) Core(TM) i5-4570S CPU           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E3-1270 v3 @ 3.50GHz        | Intel(R) Xeon(R) CPU E3-1270 v3          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-3470S CPU @ 2.90GHz         | Intel(R) Core(TM) i5-3470S CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4790K CPU @ 4.00GHz         | Intel(R) Core(TM) i7-4790K CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5 CPU         660  @ 3.33GHz  | Intel(R) Core(TM) i5 CPU 660             |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4690K CPU @ 3.50GHz         | Intel(R) Core(TM) i5-4690K CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-4130 CPU @ 3.40GHz          | Intel(R) Core(TM) i3-4130 CPU            |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           W3580  @ 3.33GHz  | Intel(R) Xeon(R) CPU W3580               |
	| ST4.xml | 48 |  | AMD FX(tm)-8320 Eight-Core Processor             | AMD FX(tm)-8320 Eight-Core Processor     |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4600M CPU @ 2.90GHz         | Intel(R) Core(TM) i7-4600M CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4310U CPU @ 2.00GHz         | Intel(R) Core(TM) i5-4310U CPU           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           W3540  @ 2.93GHz  | Intel(R) Xeon(R) CPU W3540               |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-3450 CPU @ 3.10GHz          | Intel(R) Core(TM) i5-3450 CPU            |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E3-1230 V2 @ 3.30GHz        | Intel(R) Xeon(R) CPU E3-1230 V2          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4300M CPU @ 2.60GHz         | Intel(R) Core(TM) i5-4300M CPU           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           W3520  @ 2.67GHz  | Intel(R) Xeon(R) CPU W3520               |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4710HQ CPU @ 2.50GHz        | Intel(R) Core(TM) i7-4710HQ CPU          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4500U CPU @ 1.80GHz         | Intel(R) Core(TM) i7-4500U CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4670S CPU @ 3.10GHz         | Intel(R) Core(TM) i5-4670S CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-5200U CPU @ 2.20GHz         | Intel(R) Core(TM) i5-5200U CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-2620M CPU @ 2.70GHz         | Intel(R) Core(TM) i7-2620M CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-5600U CPU @ 2.60GHz         | Intel(R) Core(TM) i7-5600U CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4910MQ CPU @ 2.90GHz        | Intel(R) Core(TM) i7-4910MQ CPU          |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2640 v2 @ 2.00GHz        | Intel(R) Xeon(R) CPU E5-2640 v2          |
	| ST4.xml | 48 |  | AMD Phenom(tm) II X6 1045T Processor             | AMD Phenom(tm) II X6 1045T Processor     |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-3437U CPU @ 1.90GHz         | Intel(R) Core(TM) i5-3437U CPU           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2637 v2 @ 3.50GHz        | Intel(R) Xeon(R) CPU E5-2637 v2          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-3740QM CPU @ 2.70GHz        | Intel(R) Core(TM) i7-3740QM CPU          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-3210M CPU @ 2.50GHz         | Intel(R) Core(TM) i5-3210M CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3 CPU         550  @ 3.20GHz  | Intel(R) Core(TM) i3 CPU 550             |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           X5560  @ 2.80GHz  | Intel(R) Xeon(R) CPU X5560               |
	| ST4.xml | 48 |  | Pentium(R) Dual-Core  CPU      E5300  @ 2.60GHz  | Pentium(R) Dual-Core CPU E5300           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2680 0 @ 2.70GHz         | Intel(R) Xeon(R) CPU E5-2680 0           |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     E8500  @ 3.16GHz  | Intel(R) Core(TM)2 Duo CPU E8500         |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Quad CPU    Q9650  @ 3.00GHz  | Intel(R) Core(TM)2 Quad CPU Q9650        |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2667 v2 @ 3.30GHz        | Intel(R) Xeon(R) CPU E5-2667 v2          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4200M CPU @ 2.50GHz         | Intel(R) Core(TM) i5-4200M CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-3517U CPU @ 1.90GHz         | Intel(R) Core(TM) i7-3517U CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-3540M CPU @ 3.00GHz         | Intel(R) Core(TM) i7-3540M CPU           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E3-1240 v3 @ 3.40GHz        | Intel(R) Xeon(R) CPU E3-1240 v3          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-4030U CPU @ 1.90GHz         | Intel(R) Core(TM) i3-4030U CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-4130T CPU @ 2.90GHz         | Intel(R) Core(TM) i3-4130T CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-3632QM CPU @ 2.20GHz        | Intel(R) Core(TM) i7-3632QM CPU          |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E3-1225 V2 @ 3.20GHz        | Intel(R) Xeon(R) CPU E3-1225 V2          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-4010U CPU @ 1.70GHz         | Intel(R) Core(TM) i3-4010U CPU           |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) CPU G3220 @ 3.00GHz          | Intel(R) Pentium(R) CPU G3220            |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4440 CPU @ 3.10GHz          | Intel(R) Core(TM) i5-4440 CPU            |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-3470T CPU @ 2.90GHz         | Intel(R) Core(TM) i5-3470T CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4770S CPU @ 3.10GHz         | Intel(R) Core(TM) i7-4770S CPU           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2695 v3 @ 2.30GHz        | Intel(R) Xeon(R) CPU E5-2695 v3          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-2300 CPU @ 2.80GHz          | Intel(R) Core(TM) i5-2300 CPU            |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-3340 CPU @ 3.10GHz          | Intel(R) Core(TM) i5-3340 CPU            |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2658A v3 @ 2.20GHz       | Intel(R) Xeon(R) CPU E5-2658A v3         |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     E7200  @ 2.53GHz  | Intel(R) Core(TM)2 Duo CPU E7200         |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-3630QM CPU @ 2.40GHz        | Intel(R) Core(TM) i7-3630QM CPU          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4510U CPU @ 2.00GHz         | Intel(R) Core(TM) i7-4510U CPU           |
	| ST4.xml | 48 |  | AMD Phenom(tm) II X4 840T Processor              | AMD Phenom(tm) II X4 840T Processor      |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-2400S CPU @ 2.50GHz         | Intel(R) Core(TM) i5-2400S CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     E6550  @ 2.33GHz  | Intel(R) Core(TM)2 Duo CPU E6550         |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-3350P CPU @ 3.10GHz         | Intel(R) Core(TM) i5-3350P CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-2130 CPU @ 3.40GHz          | Intel(R) Core(TM) i3-2130 CPU            |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Quad CPU    Q8400  @ 2.66GHz  | Intel(R) Core(TM)2 Quad CPU Q8400        |
	| ST4.xml | 48 |  | Intel(R) Celeron(R) CPU  J1900  @ 1.99GHz        | Intel(R) Celeron(R) CPU J1900            |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-3520M CPU @ 2.90GHz         | Intel(R) Core(TM) i7-3520M CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4590S CPU @ 3.00GHz         | Intel(R) Core(TM) i5-4590S CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4702HQ CPU @ 2.20GHz        | Intel(R) Core(TM) i7-4702HQ CPU          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-2637M CPU @ 1.70GHz         | Intel(R) Core(TM) i7-2637M CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5 CPU       M 520  @ 2.40GHz  | Intel(R) Core(TM) i5 CPU M 520           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4590T CPU @ 2.00GHz         | Intel(R) Core(TM) i5-4590T CPU           |
	| ST4.xml | 48 |  | AMD A8-5600K APU with Radeon(tm) HD Graphics     | AMD A8-5600K APU with #4               |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4570T CPU @ 2.90GHz         | Intel(R) Core(TM) i5-4570T CPU           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2670 v3 @ 2.30GHz        | Intel(R) Xeon(R) CPU E5-2670 v3          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-3337U CPU @ 1.80GHz         | Intel(R) Core(TM) i5-3337U CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-2760QM CPU @ 2.40GHz        | Intel(R) Core(TM) i7-2760QM CPU          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-2720QM CPU @ 2.20GHz        | Intel(R) Core(TM) i7-2720QM CPU          |
	| ST4.xml | 48 |  | AMD Phenom(tm) 9650 Quad-Core Processor          | AMD Phenom(tm) 9650 Quad-Core Processor  |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-1650 0 @ 3.20GHz         | Intel(R) Xeon(R) CPU E5-1650 0           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-3570S CPU @ 3.10GHz         | Intel(R) Core(TM) i5-3570S CPU           |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) 4 CPU 3.20GHz                | Intel(R) Pentium(R) 4 CPU 3.20GHz        |
	| ST4.xml | 48 |  | AMD Phenom(tm) II X4 960T Processor              | AMD Phenom(tm) II X4 960T Processor      |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     E4500  @ 2.20GHz  | Intel(R) Core(TM)2 Duo CPU E4500         |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     E8600  @ 3.33GHz  | Intel(R) Core(TM)2 Duo CPU E8600         |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           E5504  @ 2.00GHz  | Intel(R) Xeon(R) CPU E5504               |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-4160 CPU @ 3.60GHz          | Intel(R) Core(TM) i3-4160 CPU            |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-2640M CPU @ 2.80GHz         | Intel(R) Core(TM) i7-2640M CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Quad CPU    Q9400  @ 2.66GHz  | Intel(R) Core(TM)2 Quad CPU Q9400        |
	| ST4.xml | 48 |  | Pentium(R) Dual-Core  CPU      E6700  @ 3.20GHz  | Pentium(R) Dual-Core CPU E6700           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-3687U CPU @ 2.10GHz         | Intel(R) Core(TM) i7-3687U CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4820K CPU @ 3.70GHz         | Intel(R) Core(TM) i7-4820K CPU           |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) CPU G2020 @ 2.90GHz          | Intel(R) Pentium(R) CPU G2020            |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) CPU G850 @ 2.90GHz           | Intel(R) Pentium(R) CPU G850             |
	| ST4.xml | 48 |  | Pentium(R) Dual-Core  CPU      E6600  @ 3.06GHz  | Pentium(R) Dual-Core CPU E6600           |
	| ST4.xml | 48 |  | AMD Athlon(tm) II X2 B24 Processor               | AMD Athlon(tm) II X2 B24 Processor       |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E31225 @ 3.10GHz            | Intel(R) Xeon(R) CPU E31225              |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) CPU G3250 @ 3.20GHz          | Intel(R) Pentium(R) CPU G3250            |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-3220T CPU @ 2.80GHz         | Intel(R) Core(TM) i3-3220T CPU           |
	| ST4.xml | 48 |  | Pentium(R) Dual-Core  CPU      E5500  @ 2.80GHz  | Pentium(R) Dual-Core CPU E5500           |
	| ST4.xml | 48 |  | AMD Athlon(tm) II X2 260 Processor               | AMD Athlon(tm) II X2 260 Processor       |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2403 v2 @ 1.80GHz        | Intel(R) Xeon(R) CPU E5-2403 v2          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3 CPU         540  @ 3.07GHz  | Intel(R) Core(TM) i3 CPU 540             |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2603 v2 @ 1.80GHz        | Intel(R) Xeon(R) CPU E5-2603 v2          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5 CPU       M 560  @ 2.67GHz  | Intel(R) Core(TM) i5 CPU M 560           |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Quad CPU    Q9500  @ 2.83GHz  | Intel(R) Core(TM)2 Quad CPU Q9500        |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) CPU G2130 @ 3.20GHz          | Intel(R) Pentium(R) CPU G2130            |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) CPU G640 @ 2.80GHz           | Intel(R) Pentium(R) CPU G640             |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-3230M CPU @ 2.60GHz         | Intel(R) Core(TM) i5-3230M CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-5500U CPU @ 2.40GHz         | Intel(R) Core(TM) i7-5500U CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 CPU          6700  @ 2.66GHz  | Intel(R) Core(TM)2 CPU 6700              |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-2320 CPU @ 3.00GHz          | Intel(R) Core(TM) i5-2320 CPU            |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-3380M CPU @ 2.90GHz         | Intel(R) Core(TM) i5-3380M CPU           |
	| ST4.xml | 48 |  | AMD Phenom(tm) II X3 710 Processor               | AMD Phenom(tm) II X3 710 Processor       |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     E8200  @ 2.66GHz  | Intel(R) Core(TM)2 Duo CPU E8200         |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) CPU G3220T @ 2.60GHz         | Intel(R) Pentium(R) CPU G3220T           |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) Dual  CPU  E2180  @ 2.00GHz  | Intel(R) Pentium(R) Dual CPU E2180       |
	| ST4.xml | 48 |  | AMD A10 PRO-7800B R7, 12 Compute Cores 4C+8G     | AMD A10 PRO-7800B R7, 12 #6 4C+8G      |
	| ST4.xml | 48 |  | AMD A8-5500 APU with Radeon(tm) HD Graphics      | AMD A8-5500 APU with #4                |
	| ST4.xml | 48 |  | Intel(R) Celeron(R) CPU G1840 @ 2.80GHz          | Intel(R) Celeron(R) CPU G1840            |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-2540M CPU @ 2.60GHz         | Intel(R) Core(TM) i5-2540M CPU           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           E5410  @ 2.33GHz  | Intel(R) Xeon(R) CPU E5410               |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4610M CPU @ 3.00GHz         | Intel(R) Core(TM) i7-4610M CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5 CPU         760  @ 2.80GHz  | Intel(R) Core(TM) i5 CPU 760             |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) CPU G3430 @ 3.30GHz          | Intel(R) Pentium(R) CPU G3430            |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-3210 CPU @ 3.20GHz          | Intel(R) Core(TM) i3-3210 CPU            |
	| ST4.xml | 48 |  | Pentium(R) Dual-Core  CPU      E6300  @ 2.80GHz  | Pentium(R) Dual-Core CPU E6300           |
	| ST4.xml | 48 |  | AMD Phenom(tm) II X6 1090T Processor             | AMD Phenom(tm) II X6 1090T Processor     |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4770K CPU @ 3.50GHz         | Intel(R) Core(TM) i7-4770K CPU           |
	| ST4.xml | 48 |  | Pentium(R) Dual-Core  CPU      E5700  @ 3.00GHz  | Pentium(R) Dual-Core CPU E5700           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-3427U CPU @ 1.80GHz         | Intel(R) Core(TM) i5-3427U CPU           |
	| ST4.xml | 48 |  | Pentium(R) Dual-Core  CPU      E6500  @ 2.93GHz  | Pentium(R) Dual-Core CPU E6500           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3 CPU         530  @ 2.93GHz  | Intel(R) Core(TM) i3 CPU 530             |
	| ST4.xml | 48 |  | Pentium(R) Dual-Core  CPU      E5800  @ 3.20GHz  | Pentium(R) Dual-Core CPU E5800           |
	| ST4.xml | 48 |  | Pentium(R) Dual-Core  CPU      E5400  @ 2.70GHz  | Pentium(R) Dual-Core CPU E5400           |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 CPU          6600  @ 2.40GHz  | Intel(R) Core(TM)2 CPU 6600              |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     E7400  @ 2.80GHz  | Intel(R) Core(TM)2 Duo CPU E7400         |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4210M CPU @ 2.60GHz         | Intel(R) Core(TM) i5-4210M CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3 CPU         560  @ 3.33GHz  | Intel(R) Core(TM) i3 CPU 560             |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4430S CPU @ 2.70GHz         | Intel(R) Core(TM) i5-4430S CPU           |
	| ST4.xml | 48 |  | Intel(R) Celeron(R) CPU G550 @ 2.60GHz           | Intel(R) Celeron(R) CPU G550             |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     E7600  @ 3.06GHz  | Intel(R) Core(TM)2 Duo CPU E7600         |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7 CPU       L 640  @ 2.13GHz  | Intel(R) Core(TM) i7 CPU L 640           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-4170 CPU @ 3.70GHz          | Intel(R) Core(TM) i3-4170 CPU            |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-2120T CPU @ 2.60GHz         | Intel(R) Core(TM) i3-2120T CPU           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-1650 v3 @ 3.50GHz        | Intel(R) Xeon(R) CPU E5-1650 v3          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4790T CPU @ 2.70GHz         | Intel(R) Core(TM) i7-4790T CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     E8300  @ 2.83GHz  | Intel(R) Core(TM)2 Duo CPU E8300         |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E3-1231 v3 @ 3.40GHz        | Intel(R) Xeon(R) CPU E3-1231 v3          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4250U CPU @ 1.30GHz         | Intel(R) Core(TM) i5-4250U CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-3820QM CPU @ 2.70GHz        | Intel(R) Core(TM) i7-3820QM CPU          |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     P8600  @ 2.40GHz  | Intel(R) Core(TM)2 Duo CPU P8600         |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-6600 CPU @ 3.30GHz          | Intel(R) Core(TM) i5-6600 CPU            |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-2630QM CPU @ 2.00GHz        | Intel(R) Core(TM) i7-2630QM CPU          |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) CPU G645 @ 2.90GHz           | Intel(R) Pentium(R) CPU G645             |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4700MQ CPU @ 2.40GHz        | Intel(R) Core(TM) i7-4700MQ CPU          |
	| ST4.xml | 48 |  | AMD Phenom(tm) II X2 B59 Processor               | AMD Phenom(tm) II X2 B59 Processor       |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-3240T CPU @ 2.90GHz         | Intel(R) Core(TM) i3-3240T CPU           |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) CPU G3420 @ 3.20GHz          | Intel(R) Pentium(R) CPU G3420            |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     E6850  @ 3.00GHz  | Intel(R) Core(TM)2 Duo CPU E6850         |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5 CPU       M 480  @ 2.67GHz  | Intel(R) Core(TM) i5 CPU M 480           |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     E6750  @ 2.66GHz  | Intel(R) Core(TM)2 Duo CPU E6750         |
	| ST4.xml | 48 |  | AMD Athlon(tm) X4 760K Quad Core Processor       | #1 X4 760K Quad Core Processor         |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4712MQ CPU @ 2.30GHz        | Intel(R) Core(TM) i7-4712MQ CPU          |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) CPU G630 @ 2.70GHz           | Intel(R) Pentium(R) CPU G630             |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     P8800  @ 2.66GHz  | Intel(R) Core(TM)2 Duo CPU P8800         |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-2430M CPU @ 2.40GHz         | Intel(R) Core(TM) i5-2430M CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5 CPU       M 540  @ 2.53GHz  | Intel(R) Core(TM) i5 CPU M 540           |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     T7500  @ 2.20GHz  | Intel(R) Core(TM)2 Duo CPU T7500         |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     P8700  @ 2.53GHz  | Intel(R) Core(TM)2 Duo CPU P8700         |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E31230 @ 3.20GHz            | Intel(R) Xeon(R) CPU E31230              |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-2350M CPU @ 2.30GHz         | Intel(R) Core(TM) i3-2350M CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Quad CPU    Q9550  @ 2.83GHz  | Intel(R) Core(TM)2 Quad CPU Q9550        |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5 CPU       M 460  @ 2.53GHz  | Intel(R) Core(TM) i5 CPU M 460           |
	| ST4.xml | 48 |  | AMD A10-4600M APU with Radeon(tm) HD Graphics    | AMD A10-4600M APU with #4              |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-3317U CPU @ 1.70GHz         | Intel(R) Core(TM) i5-3317U CPU           |
	| ST4.xml | 48 |  | AMD A8-6500 APU with Radeon(tm) HD Graphics      | AMD A8-6500 APU with #4                |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3 CPU       M 330  @ 2.13GHz  | Intel(R) Core(TM) i3 CPU M 330           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-6400 CPU @ 2.70GHz          | Intel(R) Core(TM) i5-6400 CPU            |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5 CPU         661  @ 3.33GHz  | Intel(R) Core(TM) i5 CPU 661             |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-3537U CPU @ 2.00GHz         | Intel(R) Core(TM) i7-3537U CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-3110M CPU @ 2.40GHz         | Intel(R) Core(TM) i3-3110M CPU           |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) CPU G3240 @ 3.10GHz          | Intel(R) Pentium(R) CPU G3240            |
	| ST4.xml | 48 |  | AMD Athlon(tm) II X2 B28 Processor               | AMD Athlon(tm) II X2 B28 Processor       |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4700HQ CPU @ 2.40GHz        | Intel(R) Core(TM) i7-4700HQ CPU          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-4005U CPU @ 1.70GHz         | Intel(R) Core(TM) i3-4005U CPU           |
	| ST4.xml | 48 |  | AMD A6-5400K APU with Radeon(tm) HD Graphics     | AMD A6-5400K APU with #4               |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-3227U CPU @ 1.90GHz         | Intel(R) Core(TM) i3-3227U CPU           |
	| ST4.xml | 48 |  | AMD A10-5745M APU with Radeon(tm) HD Graphics    | AMD A10-5745M APU with #4              |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-2410M CPU @ 2.30GHz         | Intel(R) Core(TM) i5-2410M CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     E4700  @ 2.60GHz  | Intel(R) Core(TM)2 Duo CPU E4700         |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4550U CPU @ 1.50GHz         | Intel(R) Core(TM) i7-4550U CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-2330M CPU @ 2.20GHz         | Intel(R) Core(TM) i3-2330M CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     T6600  @ 2.20GHz  | Intel(R) Core(TM)2 Duo CPU T6600         |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-3610QM CPU @ 2.30GHz        | Intel(R) Core(TM) i7-3610QM CPU          |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) CPU G840 @ 2.80GHz           | Intel(R) Pentium(R) CPU G840             |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5 CPU       M 580  @ 2.67GHz  | Intel(R) Core(TM) i5 CPU M 580           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) M-5Y51 CPU @ 1.10GHz           | Intel(R) Core(TM) M-5Y51 CPU             |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-4330 CPU @ 3.50GHz          | Intel(R) Core(TM) i3-4330 CPU            |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-3120M CPU @ 2.50GHz         | Intel(R) Core(TM) i3-3120M CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 CPU          6300  @ 1.86GHz  | Intel(R) Core(TM)2 CPU 6300              |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7 CPU       Q 740  @ 1.73GHz  | Intel(R) Core(TM) i7 CPU Q 740           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2697 v3 @ 2.60GHz        | Intel(R) Xeon(R) CPU E5-2697 v3          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-2377M CPU @ 1.50GHz         | Intel(R) Core(TM) i3-2377M CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7 CPU       M 620  @ 2.67GHz  | Intel(R) Core(TM) i7 CPU M 620           |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) CPU G870 @ 3.10GHz           | Intel(R) Pentium(R) CPU G870             |
	| ST4.xml | 48 |  | Quad-Core AMD Opteron(tm) Processor 8381 HE      | #5 Opteron(tm) Processor 8381 HE       |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-2450M CPU @ 2.50GHz         | Intel(R) Core(TM) i5-2450M CPU           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2690 0 @ 2.90GHz         | Intel(R) Xeon(R) CPU E5-2690 0           |
	| ST4.xml | 48 |  | AMD E-450 APU with Radeon(tm) HD Graphics        | AMD E-450 APU with #4                  |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2680 v2 @ 2.80GHz        | Intel(R) Xeon(R) CPU E5-2680 v2          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4771 CPU @ 3.50GHz          | Intel(R) Core(TM) i7-4771 CPU            |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5 CPU       M 430  @ 2.27GHz  | Intel(R) Core(TM) i5 CPU M 430           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-3770S CPU @ 3.10GHz         | Intel(R) Core(TM) i7-3770S CPU           |
	| ST4.xml | 48 |  | Intel(R) Atom(TM) CPU D2550   @ 1.86GHz          | Intel(R) Atom(TM) CPU D2550              |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4460S CPU @ 2.90GHz         | Intel(R) Core(TM) i5-4460S CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-3330S CPU @ 2.70GHz         | Intel(R) Core(TM) i5-3330S CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4770T CPU @ 2.50GHz         | Intel(R) Core(TM) i7-4770T CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4765T CPU @ 2.00GHz         | Intel(R) Core(TM) i7-4765T CPU           |
	| ST4.xml | 48 |  | AMD E2-3800 APU with Radeon(TM) HD Graphics      | AMD E2-3800 APU with #4                |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     E7300  @ 2.66GHz  | Intel(R) Core(TM)2 Duo CPU E7300         |
	| ST4.xml | 48 |  | AMD E1-6010 APU with AMD Radeon R2 Graphics      | AMD E1-6010 APU with AMD #4            |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4870HQ CPU @ 2.50GHz        | Intel(R) Core(TM) i7-4870HQ CPU          |
	| ST4.xml | 48 |  | AMD FX(tm)-8120 Eight-Core Processor             | AMD FX(tm)-8120 Eight-Core Processor     |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Quad CPU    Q6600  @ 2.40GHz  | Intel(R) Core(TM)2 Quad CPU Q6600        |
	| ST4.xml | 48 |  | AMD Athlon(tm) 64 Processor 3200+                | AMD Athlon(tm) 64 Processor 3200+        |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-3450S CPU @ 2.80GHz         | Intel(R) Core(TM) i5-3450S CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-4170T CPU @ 3.20GHz         | Intel(R) Core(TM) i3-4170T CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-2820QM CPU @ 2.30GHz        | Intel(R) Core(TM) i7-2820QM CPU          |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E3-1220 V2 @ 3.10GHz        | Intel(R) Xeon(R) CPU E3-1220 V2          |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Quad  CPU   Q9550  @ 2.83GHz  | Intel(R) Core(TM)2 Quad CPU Q9550        |
	| ST4.xml | 48 |  | AMD Athlon(tm) II X3 440 Processor               | AMD Athlon(tm) II X3 440 Processor       |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2637 v3 @ 3.50GHz        | Intel(R) Xeon(R) CPU E5-2637 v3          |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) CPU G620 @ 2.60GHz           | Intel(R) Pentium(R) CPU G620             |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-2677M CPU @ 1.80GHz         | Intel(R) Core(TM) i7-2677M CPU           |
	| ST4.xml | 48 |  | Intel(R) Celeron(R) CPU        E3400  @ 2.60GHz  | Intel(R) Celeron(R) CPU E3400            |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-4160T CPU @ 3.10GHz         | Intel(R) Core(TM) i3-4160T CPU           |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) D CPU 2.80GHz                | Intel(R) Pentium(R) D CPU 2.80GHz        |
	| ST4.xml | 48 |  | AMD Athlon(tm) II X2 245 Processor               | AMD Athlon(tm) II X2 245 Processor       |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 CPU          6420  @ 2.13GHz  | Intel(R) Core(TM)2 CPU 6420              |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) Dual  CPU  E2160  @ 1.80GHz  | Intel(R) Pentium(R) Dual CPU E2160       |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) CPU G3250T @ 2.80GHz         | Intel(R) Pentium(R) CPU G3250T           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4702MQ CPU @ 2.20GHz        | Intel(R) Core(TM) i7-4702MQ CPU          |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) CPU G3460 @ 3.50GHz          | Intel(R) Pentium(R) CPU G3460            |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) 4 CPU 3.00GHz                | Intel(R) Pentium(R) 4 CPU 3.00GHz        |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-4000M CPU @ 2.40GHz         | Intel(R) Core(TM) i3-4000M CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4850HQ CPU @ 2.30GHz        | Intel(R) Core(TM) i7-4850HQ CPU          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4900MQ CPU @ 2.80GHz        | Intel(R) Core(TM) i7-4900MQ CPU          |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     E4600  @ 2.40GHz  | Intel(R) Core(TM)2 Duo CPU E4600         |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-4150T CPU @ 3.00GHz         | Intel(R) Core(TM) i3-4150T CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     T7700  @ 2.40GHz  | Intel(R) Core(TM)2 Duo CPU T7700         |
	| ST4.xml | 48 |  | AMD A6-6400K APU with Radeon(tm) HD Graphics     | AMD A6-6400K APU with #4               |
	| ST4.xml | 48 |  | AMD Turion(tm) 64 X2 Mobile Technology TL-60     | #3 64 X2 Mobile Technology TL-60       |
	| ST4.xml | 48 |  | Intel(R) Celeron(R) CPU 1037U @ 1.80GHz          | Intel(R) Celeron(R) CPU 1037U            |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) CPU        G6950  @ 2.80GHz  | Intel(R) Pentium(R) CPU G6950            |
	| ST4.xml | 48 |  | Pentium(R) Dual-Core  CPU      E5200  @ 2.50GHz  | Pentium(R) Dual-Core CPU E5200           |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) CPU G2020T @ 2.50GHz         | Intel(R) Pentium(R) CPU G2020T           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4308U CPU @ 2.80GHz         | Intel(R) Core(TM) i5-4308U CPU           |
	| ST4.xml | 48 |  | AMD A8-6500B APU with Radeon(tm) HD Graphics     | AMD A8-6500B APU with #4               |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-5257U CPU @ 2.70GHz         | Intel(R) Core(TM) i5-5257U CPU           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E31245 @ 3.30GHz            | Intel(R) Xeon(R) CPU E31245              |
	| ST4.xml | 48 |  | AMD Phenom(tm) II X2 550 Processor               | AMD Phenom(tm) II X2 550 Processor       |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-3612QM CPU @ 2.10GHz        | Intel(R) Core(TM) i7-3612QM CPU          |
	| ST4.xml | 48 |  | AMD Athlon(tm) Dual Core Processor 5000B         | AMD Athlon(tm) Dual Core Processor 5000B |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     T9400  @ 2.53GHz  | Intel(R) Core(TM)2 Duo CPU T9400         |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4260U CPU @ 1.40GHz         | Intel(R) Core(TM) i5-4260U CPU           |
	| ST4.xml | 48 |  | Intel(R) Celeron(R) CPU G1610 @ 2.60GHz          | Intel(R) Celeron(R) CPU G1610            |
	| ST4.xml | 48 |  | AMD Phenom(tm) II X4 B95 Processor               | AMD Phenom(tm) II X4 B95 Processor       |
	| ST4.xml | 48 |  | AMD Athlon(tm) II X3 460 Processor               | AMD Athlon(tm) II X3 460 Processor       |
	| ST4.xml | 48 |  | AMD Athlon(tm) II X2 220 Processor               | AMD Athlon(tm) II X2 220 Processor       |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) CPU G2010 @ 2.80GHz          | Intel(R) Pentium(R) CPU G2010            |
	| ST4.xml | 48 |  | AMD Phenom(tm) II X4 955 Processor               | AMD Phenom(tm) II X4 955 Processor       |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4310M CPU @ 2.70GHz         | Intel(R) Core(TM) i5-4310M CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-2600S CPU @ 2.80GHz         | Intel(R) Core(TM) i7-2600S CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     P9700  @ 2.80GHz  | Intel(R) Core(TM)2 Duo CPU P9700         |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4288U CPU @ 2.60GHz         | Intel(R) Core(TM) i5-4288U CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-3615QM CPU @ 2.30GHz        | Intel(R) Core(TM) i7-3615QM CPU          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-3360M CPU @ 2.80GHz         | Intel(R) Core(TM) i5-3360M CPU           |
	| ST4.xml | 48 |  | AMD Athlon(tm) II X2 B26 Processor               | AMD Athlon(tm) II X2 B26 Processor       |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Quad CPU    Q8300  @ 2.50GHz  | Intel(R) Core(TM)2 Quad CPU Q8300        |
	| ST4.xml | 48 |  | AMD Athlon(tm) 64 X2 Dual Core Processor 3800+   | #1 64 X2 Dual Core Processor 3800+     |
	| ST4.xml | 48 |  | AMD A8-5500B APU with Radeon(tm) HD Graphics     | AMD A8-5500B APU with #4               |
	| ST4.xml | 48 |  | AMD Phenom(tm) II X4 965 Processor               | AMD Phenom(tm) II X4 965 Processor       |
	| ST4.xml | 48 |  | AMD FX(tm)-B4150 Quad-Core Processor             | AMD FX(tm)-B4150 Quad-Core Processor     |
	| ST4.xml | 48 |  | AMD A8-6600K APU with Radeon(tm) HD Graphics     | AMD A8-6600K APU with #4               |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-3570K CPU @ 3.40GHz         | Intel(R) Core(TM) i5-3570K CPU           |
	| ST4.xml | 48 |  | AMD Athlon(tm) Dual Core Processor 5200B         | AMD Athlon(tm) Dual Core Processor 5200B |
	| ST4.xml | 48 |  | Intel(R) Celeron(R) CPU G1620 @ 2.70GHz          | Intel(R) Celeron(R) CPU G1620            |
	| ST4.xml | 48 |  | Pentium(R) Dual-Core CPU       T4200  @ 2.00GHz  | Pentium(R) Dual-Core CPU T4200           |
	| ST4.xml | 48 |  | Intel(R) Atom(TM) CPU D525   @ 1.80GHz           | Intel(R) Atom(TM) CPU D525               |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4570R CPU @ 2.70GHz         | Intel(R) Core(TM) i5-4570R CPU           |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) CPU 2020M @ 2.40GHz          | Intel(R) Pentium(R) CPU 2020M            |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4720HQ CPU @ 2.60GHz        | Intel(R) Core(TM) i7-4720HQ CPU          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-3635QM CPU @ 2.40GHz        | Intel(R) Core(TM) i7-3635QM CPU          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4770HQ CPU @ 2.20GHz        | Intel(R) Core(TM) i7-4770HQ CPU          |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     T9300  @ 2.50GHz  | Intel(R) Core(TM)2 Duo CPU T9300         |
	| ST4.xml | 48 |  | Intel(R) Processor 5Y70 CPU @ 1.10GHz            | Intel(R) Processor 5Y70 CPU              |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2690 v3 @ 2.60GHz        | Intel(R) Xeon(R) CPU E5-2690 v3          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4650U CPU @ 1.70GHz         | Intel(R) Core(TM) i7-4650U CPU           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           X5675  @ 3.07GHz  | Intel(R) Xeon(R) CPU X5675               |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2690 v2 @ 3.00GHz        | Intel(R) Xeon(R) CPU E5-2690 v2          |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) 4 CPU 3.40GHz                | Intel(R) Pentium(R) 4 CPU 3.40GHz        |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4258U CPU @ 2.40GHz         | Intel(R) Core(TM) i5-4258U CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     T9600  @ 2.80GHz  | Intel(R) Core(TM)2 Duo CPU T9600         |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           E5530  @ 2.40GHz  | Intel(R) Xeon(R) CPU E5530               |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-2670QM CPU @ 2.20GHz        | Intel(R) Core(TM) i7-2670QM CPU          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-4278U CPU @ 2.60GHz         | Intel(R) Core(TM) i5-4278U CPU           |
	| ST4.xml | 48 |  | AMD Opteron(tm) Processor 6380                   | AMD Opteron(tm) Processor 6380           |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Quad  CPU   Q9300  @ 2.50GHz  | Intel(R) Core(TM)2 Quad CPU Q9300        |
	| ST4.xml | 48 |  | AMD Athlon(tm) 64 X2 Dual Core Processor 4600+   | #1 64 X2 Dual Core Processor 4600+     |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4800MQ CPU @ 2.70GHz        | Intel(R) Core(TM) i7-4800MQ CPU          |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           E5645  @ 2.40GHz  | Intel(R) Xeon(R) CPU E5645               |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7-4712HQ CPU @ 2.30GHz        | Intel(R) Core(TM) i7-4712HQ CPU          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) M-5Y10c CPU @ 0.80GHz          | Intel(R) Core(TM) M-5Y10c CPU            |
	| ST4.xml | 48 |  | QEMU Virtual CPU version (cpu64-rhel6)           | QEMU Virtual CPU version (cpu64-rhel6)   |
	| ST4.xml | 48 |  | AMD A10-7300 Radeon R6, 10 Compute Cores 4C+6G   | AMD A10-7300 Radeon R6, 10 #6 4C+6G    |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7 CPU       M 640  @ 2.80GHz  | Intel(R) Core(TM) i7 CPU M 640           |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     T6400  @ 2.00GHz  | Intel(R) Core(TM)2 Duo CPU T6400         |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-2328M CPU @ 2.20GHz         | Intel(R) Core(TM) i3-2328M CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     T8100  @ 2.10GHz  | Intel(R) Core(TM)2 Duo CPU T8100         |
	| ST4.xml | 48 |  | AMD E-350 Processor                              | AMD E-350 Processor                      |
	| ST4.xml | 48 |  | Intel(R) Xeon(TM) CPU 3.80GHz                    | Intel(R) Xeon(TM) CPU 3.80GHz            |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 CPU          6320  @ 1.86GHz  | Intel(R) Core(TM)2 CPU 6320              |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5 CPU       M 450  @ 2.40GHz  | Intel(R) Core(TM) i5 CPU M 450           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i7 CPU       Q 720  @ 1.60GHz  | Intel(R) Core(TM) i7 CPU Q 720           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E7- 8870  @ 2.40GHz         | Intel(R) Xeon(R) CPU E7- 8870            |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) CPU  N3510  @ 1.99GHz        | Intel(R) Pentium(R) CPU N3510            |
	| ST4.xml | 48 |  | Pentium(R) Dual-Core CPU       T4400  @ 2.20GHz  | Pentium(R) Dual-Core CPU T4400           |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) D CPU 3.40GHz                | Intel(R) Pentium(R) D CPU 3.40GHz        |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     P9300  @ 2.26GHz  | Intel(R) Core(TM)2 Duo CPU P9300         |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3 CPU       M 370  @ 2.40GHz  | Intel(R) Core(TM) i3 CPU M 370           |
	| ST4.xml | 48 |  | AMD A10-5750M APU with Radeon(tm) HD Graphics    | AMD A10-5750M APU with #4              |
	| ST4.xml | 48 |  | AMD E1-2500 APU with Radeon(TM) HD Graphics      | AMD E1-2500 APU with #4                |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-3217U CPU @ 1.80GHz         | Intel(R) Core(TM) i3-3217U CPU           |
	| ST4.xml | 48 |  | AMD A4-5300B APU with Radeon(tm) HD Graphics     | AMD A4-5300B APU with #4               |
	| ST4.xml | 48 |  | Intel(R) Celeron(R) CPU  N2840  @ 2.16GHz        | Intel(R) Celeron(R) CPU N2840            |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-2310M CPU @ 2.10GHz         | Intel(R) Core(TM) i3-2310M CPU           |
	| ST4.xml | 48 |  | AMD A4-4355M APU with Radeon(tm) HD Graphics     | AMD A4-4355M APU with #4               |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     T6670  @ 2.20GHz  | Intel(R) Core(TM)2 Duo CPU T6670         |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           X3430  @ 2.40GHz  | Intel(R) Xeon(R) CPU X3430               |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-2640 v3 @ 2.60GHz        | Intel(R) Xeon(R) CPU E5-2640 v3          |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3 CPU       U 380  @ 1.33GHz  | Intel(R) Core(TM) i3 CPU U 380           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU           E5335  @ 2.00GHz  | Intel(R) Xeon(R) CPU E5335               |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i5-2467M CPU @ 1.60GHz         | Intel(R) Core(TM) i5-2467M CPU           |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3 CPU       M 380  @ 2.53GHz  | Intel(R) Core(TM) i3 CPU M 380           |
	| ST4.xml | 48 |  | Intel(R) Xeon(R) CPU E5-4610 0 @ 2.40GHz         | Intel(R) Xeon(R) CPU E5-4610 0           |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) CPU  N3530  @ 2.16GHz        | Intel(R) Pentium(R) CPU N3530            |
	| ST4.xml | 48 |  | AMD E2-1800 APU with Radeon(tm) HD Graphics      | AMD E2-1800 APU with #4                |
	| ST4.xml | 48 |  | AMD A6-5350M APU with Radeon(tm) HD Graphics     | AMD A6-5350M APU with #4               |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) CPU B960 @ 2.20GHz           | Intel(R) Pentium(R) CPU B960             |
	| ST4.xml | 48 |  | Genuine Intel(R) CPU           U7300  @ 1.30GHz  | Genuine Intel(R) CPU U7300               |
	| ST4.xml | 48 |  | Intel(R) Celeron(R) CPU          540  @ 1.86GHz  | Intel(R) Celeron(R) CPU 540              |
	| ST4.xml | 48 |  | Intel(R) Pentium(R) CPU 987 @ 1.50GHz            | Intel(R) Pentium(R) CPU 987              |
	| ST4.xml | 48 |  | AMD Phenom(tm) II P920 Quad-Core Processor       | #2 II P920 Quad-Core Processor         |
	| ST4.xml | 48 |  | AMD A6-4400M APU with Radeon(tm) HD Graphics     | AMD A6-4400M APU with #4               |
	| ST4.xml | 48 |  | AMD A8-4500M APU with Radeon(tm) HD Graphics     | AMD A8-4500M APU with #4               |
	| ST4.xml | 48 |  | Intel(R) Core(TM)2 Duo CPU     T5800  @ 2.00GHz  | Intel(R) Core(TM)2 Duo CPU T5800         |
	| ST4.xml | 48 |  | AMD Athlon(tm) II X2 250 Processor               | AMD Athlon(tm) II X2 250 Processor       |
	| ST4.xml | 48 |  | AMD A8-7410 APU with AMD Radeon R5 Graphics      | AMD A8-7410 APU with AMD #4            |
	| ST4.xml | 48 |  | Intel(R) Celeron(R) CPU 1000M @ 1.80GHz          | Intel(R) Celeron(R) CPU 1000M            |
	| ST4.xml | 48 |  | Intel(R) Core(TM) i3-2370M CPU @ 2.40GHz         | Intel(R) Core(TM) i3-2370M CPU           |
	| ST4.xml | 48 |  | AMD Sempron(tm) M100                             | AMD Sempron(tm) M100                     |
