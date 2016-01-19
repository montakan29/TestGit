USE [OpsConsole]
GO
	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'CPU Type',
		[Category] = 'PC Information'
	WHERE [StatCode] = '1'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Service Pack',
		[Category] = 'PC Information'
	WHERE [StatCode] = '10'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Gateway Service',
		[Category] = 'TR Platform'
	WHERE [StatCode] = '1001'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Search and Navigation Services',
		[Category] = 'TR Platform'
	WHERE [StatCode] = '1003'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Visual Basic for Applications - Microsoft Excel compatible',
		[Category] = 'PC Information'
	WHERE [StatCode] = '1004'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Browsing Version',
		[Category] = 'PC Information'
	WHERE [StatCode] = '11'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Visual Basic for Applications Version',
		[Category] = 'PC Information'
	WHERE [StatCode] = '110'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'IDN Permission Uploader',
		[Category] = 'TR Platform'
	WHERE [StatCode] = '111'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Average Latency (One Way)',
		[Category] = 'TR Platform'
	WHERE [StatCode] = '112'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Platform Name',
		[Category] = 'Eikon'
	WHERE [StatCode] = '120'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'HTTPS Connection Check',
		[Category] = 'TR Platform'
	WHERE [StatCode] = '121'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'User Country',
		[Category] = 'Eikon'
	WHERE [StatCode] = '122'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Update Proxy Connection Check',
		[Category] = 'TR Platform'
	WHERE [StatCode] = '123'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Temporary Internet Files folder size',
		[Category] = 'PC Usage'
	WHERE [StatCode] = '124'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Internet Availability Test',
		[Category] = 'TR Platform'
	WHERE [StatCode] = '125'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Reuters Inside',
		[Category] = 'TR Platform'
	WHERE [StatCode] = '126'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Product ID String',
		[Category] = 'Eikon Desktop Add-on'
	WHERE [StatCode] = '127'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Eikon Add-on',
		[Category] = 'Eikon Desktop Add-on'
	WHERE [StatCode] = '127ADF'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Eikon Deployment Manager',
		[Category] = 'Eikon'
	WHERE [StatCode] = '127EDM'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Product Version (ID String)',
		[Category] = 'Eikon'
	WHERE [StatCode] = '127EIKON'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'InsertLink Add-on',
		[Category] = 'Eikon Desktop Add-on'
	WHERE [StatCode] = '127INSL'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Japanese Language Pack',
		[Category] = 'Eikon Desktop Add-on'
	WHERE [StatCode] = '127LPja-JP'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Chinese Languaga Pack',
		[Category] = 'Eikon Desktop Add-on'
	WHERE [StatCode] = '127LPzh-CN'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Regional Model for Asia Add-on',
		[Category] = 'Eikon Desktop Add-on'
	WHERE [StatCode] = '127MODASIA'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = '.NET Framework Version (ID String)',
		[Category] = 'PC Information'
	WHERE [StatCode] = '127NET'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Eikon Service Patch',
		[Category] = 'Eikon'
	WHERE [StatCode] = '127SR'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Eikon System Test',
		[Category] = 'Eikon'
	WHERE [StatCode] = '127ST'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Product Version',
		[Category] = 'Eikon'
	WHERE [StatCode] = '128'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Machine ID',
		[Category] = 'PC Information'
	WHERE [StatCode] = '129'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Product MSI Version',
		[Category] = 'PC Information'
	WHERE [StatCode] = '130'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Update Agent Lastest Status',
		[Category] = 'Eikon Update'
	WHERE [StatCode] = '131'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Site Connectivity Assessment',
		[Category] = 'TR Platform'
	WHERE [StatCode] = '132'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'IE LAN Settings Status',
		[Category] = 'PC Information'
	WHERE [StatCode] = '133'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'WMI Service',
		[Category] = 'PC Information'
	WHERE [StatCode] = '134'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Delete Browsing History on Exit',
		[Category] = 'PC Information'
	WHERE [StatCode] = '135'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Microsoft Word Version',
		[Category] = 'Microsoft Office'
	WHERE [StatCode] = '136'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Microsoft PowerPoint Version',
		[Category] = 'Microsoft Office'
	WHERE [StatCode] = '137'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Microsoft Office',
		[Category] = 'Microsoft Office'
	WHERE [StatCode] = '138'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'System Test Process Information',
		[Category] = 'Eikon'
	WHERE [StatCode] = '140'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Screens',
		[Category] = 'PC Information'
	WHERE [StatCode] = '141'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Screen Details',
		[Category] = 'PC Information'
	WHERE [StatCode] = '142'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Register Product',
		[Category] = 'Eikon'
	WHERE [StatCode] = '143'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Microsoft Hotfix for Eikon Office',
		[Category] = 'Microsoft Office'
	WHERE [StatCode] = '144'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Thomson Reuters Drive Service',
		[Category] = 'TR Platform'
	WHERE [StatCode] = '16'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = '.NET Framework Version',
		[Category] = 'PC Information'
	WHERE [StatCode] = '17'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'HTTP 1.1 Service Availability',
		[Category] = 'TR Platform'
	WHERE [StatCode] = '18'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'MSXML 6.0 Version',
		[Category] = 'PC Information'
	WHERE [StatCode] = '19'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'CPU Vendor',
		[Category] = 'PC Information'
	WHERE [StatCode] = '2'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Operating System Language',
		[Category] = 'PC Information'
	WHERE [StatCode] = '21'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Windows User Account Name',
		[Category] = 'PC Information'
	WHERE [StatCode] = '22'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'User Locale',
		[Category] = 'PC Information'
	WHERE [StatCode] = '23'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Computer Name',
		[Category] = 'PC Information'
	WHERE [StatCode] = '24'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Computer Domain',
		[Category] = 'PC Information'
	WHERE [StatCode] = '25'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'User Rights',
		[Category] = 'PC Information'
	WHERE [StatCode] = '26'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'User Time Zone',
		[Category] = 'PC Information'
	WHERE [StatCode] = '27'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Thin Client Run Mode',
		[Category] = 'PC Information'
	WHERE [StatCode] = '28'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Microsoft Office Languge',
		[Category] = 'Microsoft Office'
	WHERE [StatCode] = '29'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'CPU Speed',
		[Category] = 'PC Information'
	WHERE [StatCode] = '3'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Views Service',
		[Category] = 'TR Platform'
	WHERE [StatCode] = '31'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Update Service Agent',
		[Category] = 'Eikon Update'
	WHERE [StatCode] = '32'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Update Agent State',
		[Category] = 'Eikon Update'
	WHERE [StatCode] = '35'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Microsoft Excel Version',
		[Category] = 'Microsoft Office'
	WHERE [StatCode] = '36'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Browser JavaScript',
		[Category] = 'PC Information'
	WHERE [StatCode] = '37'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Free Disk Space',
		[Category] = 'PC Usage'
	WHERE [StatCode] = '39'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Installed Memory',
		[Category] = 'PC Information'
	WHERE [StatCode] = '4'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Streaming Availability',
		[Category] = 'TR Platform'
	WHERE [StatCode] = '40'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'News Headlines Streaming',
		[Category] = 'TR Platform'
	WHERE [StatCode] = '42'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'CPU Description',
		[Category] = 'PC Information'
	WHERE [StatCode] = '48'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Installation Path',
		[Category] = 'Eikon'
	WHERE [StatCode] = '49'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Screen Resolution',
		[Category] = 'PC Information'
	WHERE [StatCode] = '5'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Data Retrieval',
		[Category] = 'TR Platform'
	WHERE [StatCode] = '50'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Time Series Snapshot Availability',
		[Category] = 'TR Platform'
	WHERE [StatCode] = '59'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Color Depth',
		[Category] = 'PC Information'
	WHERE [StatCode] = '6'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Time Series Streaming Availability',
		[Category] = 'TR Platform'
	WHERE [StatCode] = '60'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'User ID',
		[Category] = 'Eikon'
	WHERE [StatCode] = '61'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Product Name',
		[Category] = 'Eikon'
	WHERE [StatCode] = '62'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'OMM Session',
		[Category] = 'Eikon'
	WHERE [StatCode] = '63'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Display Template Path',
		[Category] = 'Eikon'
	WHERE [StatCode] = '64'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'DACS User Name',
		[Category] = 'Eikon'
	WHERE [StatCode] = '65'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'DACS Position',
		[Category] = 'Eikon'
	WHERE [StatCode] = '66'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Desktop Scheduler Settings',
		[Category] = 'Eikon Update'
	WHERE [StatCode] = '67'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Update Agent Check Time',
		[Category] = 'Eikon Update'
	WHERE [StatCode] = '68'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Desktop Scheduler Last Check for Updates',
		[Category] = 'Eikon Update'
	WHERE [StatCode] = '69'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'MSI Version',
		[Category] = 'PC Information'
	WHERE [StatCode] = '7'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Update Agent Last Check for Updates',
		[Category] = 'Eikon Update'
	WHERE [StatCode] = '70'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Desktop Scheduler Setting for Updates',
		[Category] = 'Eikon Update'
	WHERE [StatCode] = '71'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Update Agent Setting for Updates',
		[Category] = 'Eikon Update'
	WHERE [StatCode] = '72'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Local Time',
		[Category] = 'PC Information'
	WHERE [StatCode] = '73'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Market Feed Session',
		[Category] = 'Eikon'
	WHERE [StatCode] = '77'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Legacy Thomson Reuters Products',
		[Category] = 'Eikon'
	WHERE [StatCode] = '78'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Public API Installation Status',
		[Category] = 'Eikon'
	WHERE [StatCode] = '79'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'MSXML 3.0 Version',
		[Category] = 'PC Information'
	WHERE [StatCode] = '8'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Transaction URLs',
		[Category] = 'Eikon'
	WHERE [StatCode] = '81'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Transaction Add-on URLs',
		[Category] = 'Eikon'
	WHERE [StatCode] = '82'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Transaction Add-on Version',
		[Category] = 'Eikon'
	WHERE [StatCode] = '83'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Transaction Add-on JRE Version',
		[Category] = 'Eikon'
	WHERE [StatCode] = '84'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Saving Encrypted Pages to Disk',
		[Category] = 'PC Information'
	WHERE [StatCode] = '86'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'HTTP 1.1',
		[Category] = 'PC Information'
	WHERE [StatCode] = '87'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'HTTP 1.1 Through Proxy Connections',
		[Category] = 'PC Information'
	WHERE [StatCode] = '88'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Operation System',
		[Category] = 'PC Information'
	WHERE [StatCode] = '9'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Flash Plug-in Version',
		[Category] = 'PC Information'
	WHERE [StatCode] = '90'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Platform Connection Check',
		[Category] = 'TR Platform'
	WHERE [StatCode] = '92'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Current Real-time Deployment Model',
		[Category] = 'Eikon'
	WHERE [StatCode] = '93'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Current Real-time Network',
		[Category] = 'Eikon'
	WHERE [StatCode] = '94'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Download Bandwidth',
		[Category] = 'TR Platform'
	WHERE [StatCode] = '95'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Upload Bandwidth',
		[Category] = 'TR Platform'
	WHERE [StatCode] = '96'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Secondary Platform Connection Check',
		[Category] = 'TR Platform'
	WHERE [StatCode] = '97'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Silverlight Plug-in Version',
		[Category] = 'PC Information'
	WHERE [StatCode] = '99'

	UPDATE [dbo].[Stats]
	SET [DisplayName] = 'Computer Name',
		[Category] = 'PC Information'
	WHERE [StatCode] = 'CMPNAME'
GO