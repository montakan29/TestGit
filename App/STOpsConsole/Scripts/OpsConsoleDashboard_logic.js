var statStatusEnum = { None: "N/A", Critical: "C", Warning: "W", Normal: "N" };
var statStatusEnumWeightMapping = { C: 3, W: 2, N: 1 };
var statLocationGrid;
var statLocationDetailGrid;
var criticalUserGrid;
var mainLayout;
var innerLayout;
var currentSelectedLocationId = undefined;
var currentSelectedLocationStat = undefined;
var currentSelectedCriticalUser = undefined;
var currentUserStatQueryString = '';
var currentStartPeriodRank = 0;
var currentLatestStartPeriodRequested = undefined;
var currentTimeout = undefined;
var msTimeout = 60000; // 60 sec
var currentInterval = undefined;
var uploadAggStatMetadataDialogInstance;
var uiControlData = { showLatestPeriod: false, showStatCodes: false, selectProduct: "est"};
var rawLocationStatRecordDetailList = undefined;
var rawCriticalUserList = undefined;
var currentStatMetadata = undefined;

function convertStatusRawValueToStatStatusEnum(rawStatus) {
    if (rawStatus === "n") {
        return statStatusEnum.Normal;
    }
    else if (rawStatus === "w") {
        return statStatusEnum.Warning;
    }
    else if (rawStatus === "c") {
        return statStatusEnum.Critical;
    }
    return statStatusEnum.None;
}

//function createGetLocationStatusRequestObj(intervalParam, locationIDParam, startPeriod, onlyWarningAndCritical) {
//    return {
//        minInterval: intervalParam,
//        locationID: locationIDParam,
//        beginStartPeriod: startPeriod,
//        endStartPeriod: startPeriod,
//        onlyWarningAndCritical: onlyWarningAndCritical,
//        snapReqDateToInterval: true,
//        product: uiControlData.selectProduct
//    };
//}

function createGetTop100LocationAtRiskRequestObj(product, startPeriod, onlyWarningAndCritical) {
    return {
        startPeriod: startPeriod,
        onlyWarningAndCritical: onlyWarningAndCritical,
        snapReqDateToInterval: true,
        product: product
    };
}

function createGetLocationStatDetailRequestObj(intervalParam, locationIDParam, startPeriod) {
    return {
        minInterval: intervalParam,
        locationID: (locationIDParam !== '')? locationIDParam: '_',
        beginStartPeriod: startPeriod,
        endStartPeriod: startPeriod,
        snapReqDateToInterval: true,
        product: uiControlData.selectProduct
    };
}

function createGetCriticalUsersRequestObj(intervalParam, locationIDParam, startPeriod, statCode) {
    return {
        minInterval: intervalParam,
        locationID: (locationIDParam !== '') ? locationIDParam : '_',
        beginStartPeriod: startPeriod,
        endStartPeriod: startPeriod,
        snapReqDateToInterval: true,
        statCode: statCode,
        product: uiControlData.selectProduct
    };
}

//function createStatusRecord(criticalCount, warningCount, normalCount) {
//    return { criticalCount: criticalCount, warningCount: warningCount, normalCount: normalCount };
//}

function createAggStatMetadataRecord(statCode,
                                     valueUsing,
                                     enable,
                                     criticalLo,
                                     warningLo,
                                     warningHi,
                                     criticalHi) {
    return {
        statCode: statCode,
        valueUsing: valueUsing,
        enable: enable,
        warningHi: warningHi,
        warningLo: warningLo,
        criticalHi: criticalHi,
        criticalLo: criticalLo
    };
}

//function createLocationStatRecord(id, locationID, statCode, interval, startPeriod, count, min, max, sum, lastUpdated, firstSeen, lastSeen) {
//    return { id: id, locationID: locationID, statCode: statCode, interval: interval, startPeriod: startPeriod, count: count, min: min, max: max, sum: sum, lastUpdated: lastUpdated, firstSeen: firstSeen, lastSeen: lastSeen };
//}

function createCriticalUserRecord(id,
                                  name,
                                  value,
                                  dataType,
                                  unit,
                                  status,
                                  lastUpdated,
                                  uuid,
                                  mid,
                                  iid) {
    // In some units, decimals are not necessary, because they are too small.
    // So we change the data type to statDataType.Integer so that the decimal part get removed and don't show 99.00 on the UI.
    if (unit === unitString_ms || unit === unitString_us || unit === unitString_ns) {
        dataType = statDataType.Integer;
    }
    value = (dataType === statDataType.Integer) ? Number(Math.floor(value)) : Number(value);
    return {
        id: id,
        name: name,
        value: value,
        dataType: dataType,
        unit:unit,
        status: status,
        lastUpdated: lastUpdated,
        uuid: uuid,
        mid: mid,
        iid: iid,
        detail: "detail"
    };
}

function createLocationStatDetailRecord(id,
                                        locationID,
                                        statDisplayName,
                                        statCode,
                                        interval,
                                        startPeriod,
                                        count,
                                        min,
                                        max,
                                        sum,
                                        valueUsed,
                                        lastUpdated,
                                        firstSeen,
                                        lastSeen,
                                        status,
                                        dataType,
                                        unit) {
    // In some units, decimals are not necessary, because they are too small.
    // So we change the data type to statDataType.Integer so that the decimal part get removed and don't show 99.00 on the UI.
    if (unit === unitString_ms || unit === unitString_us || unit === unitString_ns) {
        dataType = statDataType.Integer;
    }
    return {
        id: id,
        locationID: locationID,
        statDisplayName: statDisplayName,
        statCode: statCode,
        interval: interval,
        startPeriod: startPeriod,
        count: count,
        min: min,
        max: max,
        sum: sum,
        value: valueUsed,
        lastUpdated: lastUpdated,
        firstSeen: firstSeen,
        lastSeen: lastSeen,
        status: status,
        dataType: dataType,
        unit:unit
    };
}

function createLocationStatusRecord(locationID,
                                    company,
                                    address,
                                    city,
                                    country,
                                    criticalCount,
                                    warningCount,
                                    normalCount,
                                    maxAggCount) {
    return {
        id: locationID,
        locationID: locationID,
        company: company,
        address: address,
        city: city,
        country: country,
        criticalCount: criticalCount,
        warningCount: warningCount,
        normalCount: normalCount,
        maxAggCount: maxAggCount
    };
}

function getCriticalUsers(beforeSendCallback, successCallback, errorCallback, request) {
    $.ajax({
        dataType: "json",
        type: "POST",
        data: request,
        url: '/Apps/STOpsConsole/GetCriticalUsers',
        beforeSend: beforeSendCallback,
        success: successCallback,
        error: errorCallback,
        contentType: "application/json"
    });
}
function getLocationStatDetail(beforeSendCallback, successCallback, errorCallback, request) {
    $.ajax({
        dataType: "json",
        type: "POST",
        data: request,
        url: '/Apps/STOpsConsole/GetLocationStatDetail',
        beforeSend: beforeSendCallback,
        success: successCallback,
        error: errorCallback,
        contentType: "application/json"
    });
}

function getLocationStatus(beforeSendCallback, successCallback, errorCallback, request) {
    $.ajax({
        dataType: "json",
        type: "POST",
        data: request,
        //url: '/Apps/STOpsConsole/GetLocationStatus',
        url: '/Apps/STOpsConsole/GetTop100LocationAtRisk',
        beforeSend: beforeSendCallback,
        success: successCallback,
        error: errorCallback,
        contentType: "application/json"
    });
}

$(document).ready(function () {
    getUserPermissions();
});

function updateStatListGrid(statListDataView) {
    var dataView;
    var statListGridObj = statListGrid.getGridObj();

    statListGrid.setData(statListDataView);
    dataView = statListGrid.getDataView();

    dataView.refresh();

    statListGridObj.invalidate();
    //sortGridCorrectly(statListGrid);
    //statListGrid.fixRowSelection();
}

//var aggStatMetadata = {};
//function initializeAggStatMetadata() {
//    aggStatMetadata['AVGLATENCY'] = createAggStatMetadataRecord('AVGLATENCY', 'avg', true, null, null, 1000, 2000);
//    aggStatMetadata['BWDTHDOWN'] = createAggStatMetadataRecord('BWDTHDOWN', 'avg', true, 500, 1000, null, null);
//    aggStatMetadata['BWDTHUP'] = createAggStatMetadataRecord('BWDTHUP', 'avg', true, 250, 500, null, null);
//    aggStatMetadata['ISCONNPRIM'] = createAggStatMetadataRecord('ISCONNPRIM', 'avg', true, 0.25, 0.5, null, null);
//    aggStatMetadata['RTHBEAT'] = createAggStatMetadataRecord('RTHBEAT', 'avg', true, null, null, 1000, 1200);
//    aggStatMetadata['RTLOGIN'] = createAggStatMetadataRecord('RTLOGIN', 'avg', true, null, null, 3000, 4000);
//    aggStatMetadata['SRCHNAVUP'] = createAggStatMetadataRecord('SRCHNAVUP', 'avg', true, 0.25, 0.5, null, null);
//}

function getAggStatMetadata() {
    return aggStatMetadata;
}

//function formatToLocalDate(utcInMs) {
//    return moment.utc(utcInMs).local().format('lll');
//}

function formatDate(momnetDate) {
    //return momnetLocalDate.format('lll');
    return momnetDate.format('MMM D YYYY h:mm A');
}

function momnetDate(stringDateTimeUtc) {
    return moment.utc(stringDateTimeUtc).local();
}

function formatGridDate(row, cell, value, columnDef, dataContext) {
    return formatDate(momnetDate(value));
}

function formatUserStatLink(row, cell, value, columnDef, dataContext) {
    //return "<a style='text-decoration:underline;color:#8c8c8c;' href='./'>" + value + "</a>";
    var queryString = '../?mid=' + dataContext.mid + '&iid=' + dataContext.iid + '&uuid=' + dataContext.uuid + '&statCode=' + currentSelectedLocationStat;
    return "<a class='UserStatIcon' href='" + queryString + "' target='_blank'></a>"
}


//function formatGridStatCondition(row, cell, value, columnDef, dataContext) {
//    return value.criticalCount + " Criticals, " + value.warningCount + "Warnings, " + value.normalCount + "Normals";
//}

function formatLocationStatDetailGridValue(row, cell, value, columnDef, dataContext) {
    return formatGridValue(dataContext.dataType, value, dataContext.unit, 2, true);
}

function formatLocationStatDetailGridValueDebug(row, cell, value, columnDef, dataContext) {
    return formatNumber(value);
}

function formatNumber(num) {
    return accounting.formatNumber(num, 2);
}

function formatGridStatus(row, cell, value, columnDef, dataContext) {
    if (value === statStatusEnum.Normal) {
        return 'Normal';
    }
    else if (value === statStatusEnum.Warning) {
        return 'Warning';
    }
    else if (value === statStatusEnum.Critical) {
        return 'Critical';
    }
    else if (value === "") {
        return "N/A";
    }
}

//function formatStatStatus(row, cell, value, columnDef, dataContext) {
//    return calculateStatCondition(dataContext.statCode, dataContext.count, dataContext.min, dataContext.max, dataContext.sum);
//}

//function calculateStatCondition(statCode, count, min, max, sum) {
//    var metadata = getAggStatMetadata();
//    if (metadata[statCode] != undefined) {
//        var valueUsing = metadata[statCode].valueUsing;
//        var criticalHi = metadata[statCode].criticalHi;
//        var criticalLo = metadata[statCode].criticalLo;
//        var warningHi = metadata[statCode].warningHi;
//        var warningLo = metadata[statCode].warningLo;
//        var value;
//        switch (valueUsing) {
//            case 'avg':
//                value = sum / count;
//                break;
//            case 'sum':
//                value = sum;
//                break;
//            case 'min':
//                value = min;
//                break;
//            case 'max':
//                value = max;
//                break;
//            default:
//                value = sum / count;
//                break;
//        }
//        if (criticalHi != null && value >= criticalHi) {
//            return statStatusEnum.Critical;
//        }
//        else if (warningHi != null && value >= warningHi) {
//            return statStatusEnum.Warning;
//        }
//        else if (criticalLo != null && value <= criticalLo) {
//            return statStatusEnum.Critical;
//        }
//        else if (warningLo != null && value <= warningLo) {
//            return statStatusEnum.Warning;
//        }
//        else {
//            return statStatusEnum.Normal;
//        }
//    }
//    return statStatusEnum.None;
//}

// rawLocationStats = statsForAllIntervals.statsForAllPeriods.stats
//function calculateAggStatCondition(rawLocationStats) {
//    var criticalCount = 0;
//    var warningCount = 0;
//    var normalCount = 0;
//    for (var i = 0; i < rawLocationStats.length; ++i) {
//        var status = calculateStatCondition(rawLocationStats[i].statCode, rawLocationStats[i].count, rawLocationStats[i].min, rawLocationStats[i].max, rawLocationStats[i].sum);
//        if (status === statStatusEnum.Critical) {
//            ++criticalCount;
//        }
//        else if (status === statStatusEnum.Warning) {
//            ++warningCount;
//        }
//        else if (status === statStatusEnum.Normal) {
//            ++normalCount;
//        }
//    }
//    return createAggStatCondition(criticalCount, warningCount, normalCount);
//}

function sortStatLocationDetailGrid() {
    var ascendingSort = false;
    var sortfieldId = "";
    statLocationDetailGrid.sort(sortfieldId, ascendingSort);
}

function sortCriticalUserGrid() {
    var ascendingSort = false;
    var sortfieldId = "";
    criticalUserGrid.sort(sortfieldId, ascendingSort);
}

function updateStatLocationGrid(aggStatRecords) {
    var dataView = statLocationGrid.getDataView();
    var gridObj = statLocationGrid.getGridObj();
    dataView.beginUpdate();
    dataView.setItems(aggStatRecords);
    dataView.endUpdate();
    dataView.syncGridSelection(gridObj, true);
    gridObj.updateRowCount();
    gridObj.resizeCanvas();
    gridObj.invalidate();

    //The code below causes a callback handle leak if sortStatLocationGrid() is called after statLocationGrid.setData()
    //In OpsConsoleLib.js, for (var i = 0; i < handlers.length && !(e.isPropagationStopped() || e.isImmediatePropagationStopped()); i++) {
    //handlers.length is incremented by 1 for every updateStatLocationGrid() call
    //Note: updateStatLocationGrid() is called periodically
    //statLocationGrid.setData(aggStatRecords);
    //statLocationGrid.getGridObj().invalidate();
    //sortStatLocationGrid();
}

function createStatLocationGrid(locationStatusRecords) {
    if (statLocationGrid === undefined) {
        var columns = [
                { id: "number", name: "", field: "number", minWidth: 30, maxWidth: 40, cssClass: "cell-title", headerCssClass: "cell-align-left" },
                { id: "company", name: "Company", field: "company", minWidth: 100, width: 100, cssClass: "cell-align-left", headerCssClass: "cell-align-left" },
                { id: "address", name: "Address", field: "address", minWidth: 80, cssClass: "cell-title", headerCssClass: "cell-align-left" },
                { id: "city", name: "City", field: "city", minWidth: 60, width: 70, cssClass: "cell-title", headerCssClass: "cell-align-left" },
                { id: "country", name: "Country", field: "country", minWidth: 60, width: 70, cssClass: "cell-title", headerCssClass: "cell-align-left" },
                { id: "criticalCount", name: "Critical", field: "criticalCount", minWidth: 25, width: 35, cssClass: "cell-align-left", headerCssClass: "cell-align-left" },
                { id: "warningCount", name: "Warning", field: "warningCount", minWidth: 30, width: 38, cssClass: "cell-align-left", headerCssClass: "cell-align-left" },
                //{ id: "normalCount", name: "Normal", field: "normalCount", minWidth: 30, maxWidth: 65, width: 65, cssClass: "cell-align-left", headerCssClass: "cell-align-left" },
                { id: "maxAggCount", name: "Users", field: "maxAggCount", minWidth: 30, width: 35, cssClass: "cell-align-left", headerCssClass: "cell-align-left" }
            ];

        var gridStatLocationOptions = {
            editable: false,
            enableCellNavigation: true,
            forceFitColumns: true,
            topPanelHeight: 25,
            rowHeight: 25,
            enableColumnReorder: false,
            enableTextSelectionOnCells: true
        };

        statLocationGrid = new Grid("#gridLocationStat", { data: locationStatusRecords, columns: columns, options: gridStatLocationOptions, events: { onclick: OnClickStatLocationGrid } });
        var dataView = statLocationGrid.getDataView();
        dataView.onRowCountChanged.subscribe(function (e, args) {
            statLocationGrid.getGridObj().updateRowCount();
            statLocationGrid.getGridObj().render();
        });
        dataView.setFilterArgs({
            searchString: ""
        });
        dataView.setFilter(locationFilter);
        adjustLocationStatGridHight();
        statLocationGrid.fixRowSelection();
    }
    else {
        updateStatLocationGrid(locationStatusRecords);
        statLocationGrid.fixRowSelection();
        //var dv = statLocationGrid.getDataView();
        //var grid = statLocationGrid.getGridObj();
        //grid.setSelectedRows([dv.getRowById(currentSelectedLocationId)]);
        //var locationStatusRecord = dv.getItemById(currentSelectedLocationId);
        //if (locationStatusRecord === undefined) {
        //    locationStatusRecord = dv.getItem(0);
        //    if (locationStatusRecord !== undefined) {
        //        currentSelectedLocationId = locationStatusRecord.id;
        //    }
        //    else {
        //        currentSelectedLocationId = undefined;
        //    }
        //}

        loadLoactionStatAndCriticalUserFromCurSelGridRows();
    }
}

function locationFilter(item, args) {
    var company = item['company'].toLowerCase();
    var address = item['address'].toLowerCase();
    var city = item['city'].toLowerCase();
    var country = item['country'].toLowerCase();
    var locaseSearchString = args.searchString.toLowerCase();
    if (locaseSearchString != "" && company.indexOf(locaseSearchString) == -1 && address.indexOf(locaseSearchString) == -1 && city.indexOf(locaseSearchString) == -1 && country.indexOf(locaseSearchString) == -1) {
        return false;
    }
    return true;
}

$("#locationFilterTextInput").keyup(function (e) {
    // clear on Esc
    if (e.which == 27) {
        this.value = "";
    }
    updateLocationFilter(this.value);
});

function updateLocationFilter(locationFilterString) {
    var dataView = statLocationGrid.getDataView();
    dataView.setFilterArgs({
        searchString: locationFilterString
    });
    statLocationGrid.getGridObj().resetActiveCell();
    dataView.refresh();
    statLocationGrid.fixRowSelection();
}

function updateLocationDetailGrid(aggStatRecord) {
    statLocationDetailGrid.setData(aggStatRecord);
    statLocationDetailGrid.getGridObj().invalidate();
}

function updateCriticalUserGrid(criticalUserRecords) {
    criticalUserGrid.setData(criticalUserRecords);
    criticalUserGrid.getGridObj().invalidate();
}

function createStatLocationDetailGrid(locationStatRecordDetailList) {
    updateSpanLocation();
    if (statLocationDetailGrid === undefined) {
        var columns = [
                { id: "number", name: "", field: "number", minWidth: 25, maxWidth: 30, width: 30, cssClass: "cell-title", headerCssClass: "cell-align-left" },
                { id: "statDisplayName", name: "Stat", field: "statDisplayName", minWidth: 170, cssClass: "cell-align-left", headerCssClass: "cell-align-left" },
                { id: "status", name: "Status", field: "status", minWidth: 40, width:50, cssClass: "cell-align-left", headerCssClass: "cell-align-left", formatter: formatGridStatus },
                //{ id: "count", name: "Count", field: "count", minWidth: 30, cssClass: "cell-align-right", headerCssClass: "cell-align-right" },
                { id: "value", name: "Value", field: "value", minWidth: 50, width:60, cssClass: "cell-align-right", headerCssClass: "cell-align-right", formatter: formatLocationStatDetailGridValue }
            ];

        var gridStatListOptions = {
            editable: false,
            enableCellNavigation: true,
            forceFitColumns: true,
            topPanelHeight: 25,
            rowHeight: 25,
            enableColumnReorder: false,
            enableTextSelectionOnCells: true
        };
        statLocationDetailGrid = new Grid("#gridLocationStatDetail", { data: locationStatRecordDetailList, columns: columns, options: gridStatListOptions, events: { onclick: OnClickStatLocationDetailGrid } });
        var dataView = statLocationDetailGrid.getDataView();
        dataView.onRowCountChanged.subscribe(function (e, args) {
            statLocationDetailGrid.getGridObj().updateRowCount();
            statLocationDetailGrid.getGridObj().render();
        });
        sortStatLocationDetailGrid();
        adjustLocationStatDetailGridHight();
    }
    else {
        updateLocationDetailGrid(locationStatRecordDetailList);
        sortStatLocationDetailGrid();
    }
    statLocationDetailGrid.fixRowSelection();
}

function createCriticalUserGrid(criticalUserList) {
    updateSpanStatName();
    if (criticalUserGrid === undefined) {
        var columns = [
                { id: "number", name: "", field: "number", minWidth: 25, maxWidth: 30, width: 30, cssClass: "cell-title", headerCssClass: "cell-align-left" },
                { id: "detail", name: "", field: "detail", minWidth: 22, maxWidth: 22, width: 22, cssClass: "cell-align-left", headerCssClass: "cell-align-left", formatter: formatUserStatLink },
                { id: "name", name: "Name", field: "name", minWidth: 50, width:85, cssClass: "cell-align-left", headerCssClass: "cell-align-left" },
                { id: "status", name: "Most Recent Status", field: "status", minWidth: 40, width: 55, cssClass: "cell-align-left", headerCssClass: "cell-align-left", formatter: formatGridStatus },
                { id: "lastUpdated", name: "Last Updated", field: "lastUpdated", minWidth: 50, width: 80, cssClass: "cell-align-left monospace", headerCssClass: "cell-align-left", formatter: formatGridDate },
                { id: "value", name: "Value", field: "value", minWidth: 40, width: 40, cssClass: "cell-align-right monospace", headerCssClass: "cell-align-right", formatter: formatLocationStatDetailGridValue }
        ];
        var gridOptions = {
            editable: false,
            enableCellNavigation: true,
            forceFitColumns: true,
            topPanelHeight: 25,
            rowHeight: 25,
            enableColumnReorder: false,
            enableTextSelectionOnCells: true
        };
        criticalUserGrid = new Grid("#gridCriticalUsers", { data: criticalUserList, columns: columns, options: gridOptions, events: { onclick: OnClickCriticalUserGrid } });
        var dataView = criticalUserGrid.getDataView();
        dataView.onRowCountChanged.subscribe(function (e, args) {
            criticalUserGrid.getGridObj().updateRowCount();
            criticalUserGrid.getGridObj().render();
        });
        sortCriticalUserGrid();
        adjustCriticalUserGridHight();
    }
    else {
        updateCriticalUserGrid(criticalUserList);
        sortCriticalUserGrid();
    }
    criticalUserGrid.fixRowSelection();
}

//function extractLocationStatusLoadResult(interval, locationStatusesForAllLocations, selectedPeriodIndexOfEachLocation) {
//    var index = 0;
//    var locationStatuses = [];
//    for (var j = 0, len = locationStatusesForAllLocations.length; j < len; ++j) {
//        var locationID = locationStatusesForAllLocations[j].locationID;
//        var company = locationStatusesForAllLocations[j].name;
//        var address = locationStatusesForAllLocations[j].address;
//        var city = locationStatusesForAllLocations[j].city;
//        var country = locationStatusesForAllLocations[j].country;

//        if (locationStatusesForAllLocations[j].locationStatusesForAllPeriods === undefined ||
//            locationStatusesForAllLocations[j].locationStatusesForAllPeriods.length === 0 ||
//            locationStatusesForAllLocations[j].locationStatusesForAllPeriods[selectedPeriodIndexOfEachLocation] === undefined) {
//            continue;
//        }

//        var locationStatusRecord = locationStatusesForAllLocations[j].locationStatusesForAllPeriods[selectedPeriodIndexOfEachLocation];
//        locationStatuses[index++] = createLocationStatusRecord(locationID, company, address !== 'N/A' ? address : locationID, city, country, locationStatusRecord.startPeriod, locationStatusRecord.minInterval, locationStatusRecord.criticalCount, locationStatusRecord.warningCount, locationStatusRecord.normalCount, locationStatusRecord.maxAggCount);
//    }
//    return locationStatuses;
//}

function getTop100LocationAtRisk(locationStatus) {
    return locationStatus.map(function (item) {
        return createLocationStatusRecord(item.locationID, item.name, item.address !== 'N/A' ? item.address : item.locationID, item.city, item.country, item.criticalCount, item.warningCount, item.normalCount, item.maxAggCount);
    });
}

function loadInterval() {
    //beforeSendCallback
    var beforeSend = function (xhr) {
        currentInterval = undefined;
    };

    //successCallback
    var onSuccess = function (data) {
        currentInterval = data.currentMinInterval;
        if (currentStatMetadata !== undefined) {
            OnDataReady();
        }
    }

    //errorCallback
    var onError = function (jqXhr, status, error) {
        $('#entirePage').hide();
        $('#pageLoadingStatus').show();
        $('#pageLoadingStatus').text('');
        $('#pageLoadingStatus').append('Failed to get aggregated interval');
    };

    // There is no interval 9999, so no any data is expected, but currentMinInterval
    getLocationStatDetail(beforeSend, onSuccess, onError, JSON.stringify(createGetLocationStatDetailRequestObj(9999, "xx", "")));
}

function loadLocationStatus() {
    //beforeSendCallback
    var beforeSend = function (xhr) {
        disableNavigateBtnWhileLoading(true);
        $('#spanUpdatedAt').text("Loading...");
        rawLocationStatRecordDetailList = undefined;
    };

    //successCallback
    var onSuccess = function (data) {
        if (data.currentMinInterval !== undefined && data.locationStatus !== undefined) {
            $('#spanUpdatedAt').text(moment().format('MMM D YYYY h:mm:ss A'));
            currentInterval = data.currentMinInterval;
            //$('#spanInterval').text("(" + currentInterval + " mins interval)");
            $('#spanAggPeriod').text(formatDate(momnetDate(data.startPeriod)));
            if (data.locationStatus.length < 1) {
                createStatLocationGrid([]);
            }
            else {
                var locationStatusRecords = getTop100LocationAtRisk(data.locationStatus);
                createStatLocationGrid(locationStatusRecords);
            }
        }
        else {
            // error
            $('#spanUpdatedAt').text('');
            $('<span class="status-critical"></span>').text('Load failed at ' + moment().format('MMM D YYYY h:mm:ss A')).appendTo('#spanUpdatedAt');
        }
        currentTimeout = setTimeout(loadLocationStatus, msTimeout);
        disableNavigateBtnWhileLoading(false);
    };
    
    //errorCallback
    var onError = function (jqXhr, status, error) {
        var errorMsg = '';
        if (error !== "") {
            errorMsg = ' - ' + error;
        }
        else if (status !== "") {
            errorMsg = ' - ' + status;
        }
        $('#spanUpdatedAt').text('');
        $('<span class="status-critical"></span>').text('Load failed at ' + moment().format('MMM D YYYY h:mm:ss A') + errorMsg).appendTo('#spanUpdatedAt');
        currentTimeout = setTimeout(loadLocationStatus, msTimeout);
        createStatLocationGrid([]);
        disableNavigateBtnWhileLoading(false);
    };
    if (uiControlData.showLatestPeriod === true) {
        // make the latest slot shown in UI to be the slot before the latest slot
        currentLatestStartPeriodRequested = moment().utc().subtract(currentInterval, 'minutes');
    }
    var startPeriodReq = formatDate(moment(currentLatestStartPeriodRequested).subtract(1, 'days').subtract(currentInterval * currentStartPeriodRank, 'minutes'));
    getLocationStatus(beforeSend, onSuccess, onError, JSON.stringify(createGetTop100LocationAtRiskRequestObj(uiControlData.selectProduct, startPeriodReq, true)));
}

function getStatDisplayName(stat) {
    var displayName = "";
    if (stat !== undefined) {
        if (uiControlData.showStatCodes)
            displayName = stat.statCode;
        else if (stat.statDisplayName === undefined || stat.statDisplayName.length == 0)
            displayName = stat.statCode;
        else
            displayName = stat.statDisplayName;
    }
    return displayName;
}

function adjustSizeDivLocationStatDetailOverlay(statusText) {
    //var $locationStatDetailGrid = $('#divLocationStatDetail');
    var $locationStatDetailGrid = $('#gridLocationStatDetail');
    var $overlay = $('#divLoactionStatDetailLoading');
    $overlay.css({
        width: $locationStatDetailGrid.outerWidth(),
        height: $locationStatDetailGrid.outerHeight() - 4,
        top: $locationStatDetailGrid.position().top,
        left: $locationStatDetailGrid.position().left
    });
    if (statusText !== undefined) {
        $overlay.text(statusText);
    }
}

function adjustSizeDivCriticalUserOverlay(statusText) {
    //var $locationStatDetailGrid = $('#divLocationStatDetail');
    var $criticalUsersGrid = $('#gridCriticalUsers');
    var $overlay = $('#divCriticalUserLoading');
    $overlay.css({
        width: $criticalUsersGrid.outerWidth(),
        height: $criticalUsersGrid.outerHeight() - 4,
        top: $criticalUsersGrid.position().top,
        left: $criticalUsersGrid.position().left
    });
    if (statusText !== undefined) {
        $overlay.text(statusText);
    }
}

function showDivCriticalUserOverlay() {
    $('#divCriticalUserLoading').show();
}

function hideDivCriticalUserOverlay() {
    $('#divCriticalUserLoading').hide();
}

function showDivLocationStatDetailOverlay() {
    $('#divLoactionStatDetailLoading').show();
}

function hideDivLocationStatDetailOverlay() {
    $('#divLoactionStatDetailLoading').hide();
}

function loadCriticalUsers() {
    //beforeSendCallback
    var beforeSend = function (xhr) {
        adjustSizeDivCriticalUserOverlay('Loading...');
        showDivCriticalUserOverlay();
        rawCriticalUserList = undefined;
    };

    //successCallback
    var onSuccess = function (data) {
        if (data.criticalUsers === undefined) {
            // error
            createCriticalUserGrid([]);
            adjustSizeDivCriticalUserOverlay('Load failed');
            showDivCriticalUserOverlay();
            return;
        }
        if (data.criticalUsers.length > 0) {
            rawCriticalUserList = data.criticalUsers;
            hideDivCriticalUserOverlay();
            createCriticalUserGrid(getCriticalUserReocrdListView(rawCriticalUserList));
        }
        else {
            // no data
            hideDivCriticalUserOverlay();
            createCriticalUserGrid([]);
        }
    };

    //errorCallback
    var onError = function (jqXhr, status, error) {
        // error
        var errorMsg = '';
        if (error !== "") {
            errorMsg = ' - ' + error;
        }
        else if (status !== "") {
            errorMsg = ' - ' + status;
        }
        createCriticalUserGrid([]);
        adjustSizeDivCriticalUserOverlay('Load failed' + errorMsg);
        showDivCriticalUserOverlay();
    };
    //var factor = currentStartPeriodRank;
    //if (currentStartPeriodRank == -1) {
    //    factor = 0;
    //}
    var startPeriodReq = formatDate(moment(currentLatestStartPeriodRequested).subtract(1, 'days').subtract(currentInterval * currentStartPeriodRank, 'minutes'))
    getCriticalUsers(beforeSend, onSuccess, onError, JSON.stringify(createGetCriticalUsersRequestObj(-1, currentSelectedLocationId, startPeriodReq, currentSelectedLocationStat)));
}

function loadLocationStatDetail() {
    //beforeSendCallback
    var beforeSend = function (xhr) {
        adjustSizeDivLocationStatDetailOverlay('Loading...');
        showDivLocationStatDetailOverlay();
        rawLocationStatRecordDetailList = undefined;
    };

    //successCallback
    var onSuccess = function (data) {
        if (data.statsForAllIntervals === undefined) {
            // error
            createStatLocationDetailGrid([]);
            adjustSizeDivLocationStatDetailOverlay('Load failed');
            showDivLocationStatDetailOverlay();
            return;
        }
        if (data.statsForAllIntervals.length > 0 &&
            data.statsForAllIntervals[0].statsForAllLocations.length > 0 &&
            data.statsForAllIntervals[0].statsForAllLocations[0].statsForAllPeriods.length > 0 &&
            data.statsForAllIntervals[0].statsForAllLocations[0].statsForAllPeriods[0].stats.length > 0) {
            rawLocationStatRecordDetailList = data.statsForAllIntervals[0].statsForAllLocations[0].statsForAllPeriods[0].stats;
            hideDivLocationStatDetailOverlay();
            createStatLocationDetailGrid(getLocationStatReocrdDetailListView(rawLocationStatRecordDetailList));
        }
        else {
            // no data
            hideDivLocationStatDetailOverlay();
            createStatLocationDetailGrid([]);
        }
    };

    //errorCallback
    var onError = function (jqXhr, status, error) {
        // error
        var errorMsg = '';
        if (error !== "") {
            errorMsg = ' - ' + error;
        }
        else if (status !== "") {
            errorMsg = ' - ' + status;
        }
        createStatLocationDetailGrid([]);
        adjustSizeDivLocationStatDetailOverlay('Load failed' + errorMsg);
        showDivLocationStatDetailOverlay();
    };
    //var factor = currentStartPeriodRank;
    //if (currentStartPeriodRank == -1) {
    //    factor = 0;
    //}
    var startPeriodReq = formatDate(moment(currentLatestStartPeriodRequested).subtract(1, 'days').subtract(currentInterval * currentStartPeriodRank, 'minutes'));
    getLocationStatDetail(beforeSend, onSuccess, onError, JSON.stringify(createGetLocationStatDetailRequestObj(-1, currentSelectedLocationId, startPeriodReq)));
}

function adjustLocationStatGridHight() {
    if (statLocationGrid !== undefined) {
        $('#gridLocationStat').css('height', mainLayout.state.east.innerHeight - $('#locationStatHeader').height() - 1);
        statLocationGrid.getGridObj().resizeCanvas();
    }
}

function adjustLocationStatDetailGridHight() {
    if (statLocationDetailGrid !== undefined) {
        $('#gridLocationStatDetail').css('height', innerLayout.state.center.innerHeight - $('#locationStatDetailHeader').height() - 1);
        statLocationDetailGrid.getGridObj().resizeCanvas();
    }
}

function adjustCriticalUserGridHight() {
    if (criticalUserGrid !== undefined) {
        $('#gridCriticalUsers').css('height', innerLayout.state.south.innerHeight - $('#criticalUserHeader').height() - 1);
        criticalUserGrid.getGridObj().resizeCanvas();
    }
}

function initializeLayout() {
    mainLayout = $('body').layout({
        north__initClosed: false,
        north__resizable: false,
        north__slidable: false,
        north__closable: true,
        north__resizerDblClickToggle: true,
        //north__size: .14,
        east__initHidden: false,
        west__initHidden: true,
        east__size: .45,
        east__onresize: function (pane, $pane, state, options) {
            //$('#gridStatList').css('height', mainLayout.state.west.innerHeight - $('#statInfoHeader').height() - 1);
            //statListGrid.getGridObj().resizeCanvas();
            adjustLocationStatDetailGridHight();
            adjustCriticalUserGridHight();
            var display = $('#divLoactionStatDetailLoading').css('display');
            if (display !== 'none') {
                adjustSizeDivLocationStatDetailOverlay();
            }
            var display = $('#divCriticalUserLoading').css('display');
            if (display !== 'none') {
                adjustSizeDivCriticalUserOverlay();
            }
        },
        center__initHidden: false,
        center__onresize: function (pane, $pane, state, options) {
            adjustLocationStatGridHight();
            var display = $('#divLoactionStatDetailLoading').css('display');
            if (display !== 'none') {
                adjustSizeDivLocationStatDetailOverlay();
            }
            var display = $('#divCriticalUserLoading').css('display');
            if (display !== 'none') {
                adjustSizeDivCriticalUserOverlay();
            }
        }
    });

    mainLayout.sizePane("north", 93);

    innerLayout = $('#inner').layout({
        north__initHidden: true,
        east__initHidden: true,
        west__initHidden: true,
        center__initHidden: false,
        south__initHidden: false,
        center__onresize: function (pane, $pane, state, options) {
            //$('#gridStatList').css('height', mainLayout.state.west.innerHeight - $('#statInfoHeader').height() - 1);
            //statListGrid.getGridObj().resizeCanvas();
            adjustLocationStatDetailGridHight();
            adjustCriticalUserGridHight();
            var display = $('#divLoactionStatDetailLoading').css('display');
            if (display !== 'none') {
                adjustSizeDivLocationStatDetailOverlay();
            }
            var display = $('#divCriticalUserLoading').css('display');
            if (display !== 'none') {
                adjustSizeDivCriticalUserOverlay();
            }
        },
    });

    innerLayout.sizePane("south", 0.40);
}

function initializeControlUI() {
    initializeLayout();
    var opsConsoleVersion = getOpsConsoleVersion();
    if (opsConsoleVersion != null && opsConsoleVersion != undefined) {
        $("#opsConsoleVersion").text("(" + opsConsoleVersion + ")");
    }
    window.parent.document.title = "Location Stats - System Test";
    $('#chkShowLatestPeriod').prop('disabled', false);
    $('#chkShowStatCodes').prop('disabled', false);
    $('#locationFilterTextInput').prop('disabled', false);

    $("#chkShowStatCodes").prop("checked", false);
    $("#chkShowLatestPeriod").prop("checked", true);
    $('#locationFilterTextInput').prop('checked', true);

    uiControlData.showLatestPeriod = $("#chkShowLatestPeriod").prop("checked");
    uiControlData.showStatCodes = $("#chkShowStatCodes").prop("checked");
}

// beforeSendCallback(xhr), successCallback(data), errorCallback(jqXhr, status, error)
function getMetadata(beforeSendCallback, successCallback, errorCallback) {
    $.ajax({
        dataType: "json",
        type: "POST",
        data: JSON.stringify({ product: uiControlData.selectProduct }),
        url: '/Apps/STOpsConsole/GetMetadata',
        beforeSend: beforeSendCallback,
        success: successCallback,
        error: errorCallback,
        contentType: "application/json"
    });
}

function loadStatMetadata() {
    var beforeReq = function () {
        currentStatMetadata = undefined;
    };

    var success = function (data) {
        currentStatMetadata = createStatMetadata(data);
        if (currentInterval !== undefined) {
            OnDataReady();
        }
    };

    var error = function (jqXhr, status, error) {
        $('#entirePage').hide();
        $('#pageLoadingStatus').show();
        $('#pageLoadingStatus').text('');
        $('#pageLoadingStatus').append('Failed to get the stat metadata');
    };

    getMetadata(beforeReq, success, error);
}

function OnDocumentReady() {
    //if (currentQueryStringParam.debug !== undefined) {
    //    debug = (currentQueryStringParam.debug === 'true') ? true : false;
    //}
    
    apphitsApi.logHit("app", "STOpsConsole", "LocSum");

    if (currentQueryStringParam.product !== undefined) {
        uiControlData.selectProduct = currentQueryStringParam.product;
    }

    if (!isAllowToUploadMetadata()) {
        $('#thresholdBtn').css('display', "none");
    }

    loadStatMetadata();
    loadInterval();
}

function getCriticalUserReocrdListView(rawCriticalUserReocrds) {
    var criticalUserListView = rawCriticalUserReocrds.map(function (row, i) {
        var statMetadataRec = currentStatMetadata[row.statCode];
        if (statMetadataRec === undefined) {
            statMetadataRec = createStatMetadataRecord(row.statCode,
                                                       ConvertToStatDataTypeEnum(""),
                                                       "");
        }
        var name = row.firstName + " " + row.lastName;
        return createCriticalUserRecord(name,
                                        name,
                                        row.value,
                                        statMetadataRec.dataType,
                                        statMetadataRec.unit,
                                        convertStatusRawValueToStatStatusEnum(row.status),
                                        row.lastUpdated,
                                        row.uuid,
                                        row.machineGuid,
                                        row.installGuid);
    });
    return criticalUserListView;
}

function getLocationStatReocrdDetailListView(rawLocationStatReocrds) {
    var locationStatRecordDetailList = rawLocationStatReocrds.map(function (row, i) {
        var statMetadataRec = currentStatMetadata[row.statCode];
        if (statMetadataRec === undefined) {
            statMetadataRec = createStatMetadataRecord(row.statCode,
                                                       ConvertToStatDataTypeEnum(""),
                                                       "");
        }
        return createLocationStatDetailRecord(row.statCode,
                                              row.locationID,
                                              getStatDisplayName(row),
                                              row.statCode,
                                              row.minInterval,
                                              row.startPeriod,
                                              row.count,
                                              row.min,
                                              row.max,
                                              row.sum,
                                              row.valueUsed,
                                              row.lastUpdated,
                                              row.firstSeen,
                                              row.lastSeen,
                                              convertStatusRawValueToStatStatusEnum(row.status),
                                              statMetadataRec.dataType,
                                              statMetadataRec.unit);
    });
    return locationStatRecordDetailList;
}

function updateSpanLocation() {
    var value = null;
    if (statLocationGrid !== undefined) {
        var activeCellObj = statLocationGrid.getGridObj().getActiveCell();
        if (activeCellObj !== undefined && activeCellObj !== null) {
            var data = statLocationGrid.getGridObj().getDataItem(activeCellObj.row);
            if (data !== undefined && data !== null) {
                value = data.company;
            }
        }
    }
    if (value !== null) {
        $("#spanLocation").text(value);
    }
}

function updateSpanStatName() {
    var value = null;
    if (statLocationDetailGrid !== undefined) {
        var activeCellObj = statLocationDetailGrid.getGridObj().getActiveCell();
        if (activeCellObj !== undefined && activeCellObj !== null) {
            var data = statLocationDetailGrid.getGridObj().getDataItem(activeCellObj.row);
            if (data !== undefined && data !== null) {
                value = data.statDisplayName;
            }
        }
    }
    if (value !== null) {
        $("#spanStatName").text(value);
    }
}

function getCurrentSelectedCriticalUser() {
    var queryString = '';
    var activeCellObj = criticalUserGrid.getGridObj().getActiveCell();
    if (activeCellObj !== undefined && activeCellObj !== null) {
        var data = criticalUserGrid.getGridObj().getDataItem(activeCellObj.row);
        if (data !== undefined && data !== null) {
            var statCodeParam = '';
            if (currentSelectedLocationStat !== undefined) {
                statCodeParam = '&statCode=' + currentSelectedLocationStat;
            }
            queryString = '?mid=' + data.mid + '&iid=' + data.iid + '&uuid=' + data.uuid + statCodeParam;
        }
    }
    return queryString;
}

function OnClickCriticalUserGrid(e, args) {
    if (args.rowData !== undefined) {
        currentSelectedCriticalUser = args.rowData.id;
    }
}

function OnClickStatLocationDetailGrid(e, args) {
    if (args.rowData !== undefined) {
        currentSelectedLocationStat = args.rowData.id;
    }
    if (currentSelectedLocationId !== undefined) {
        loadCriticalUsers();
    }
    else {
        createCriticalUserGrid([]);
    }
}

function loadLoactionStatAndCriticalUserFromCurSelGridRows() {
    // To prevent loadLocationStatDetail() and loadCriticalUsers() from loading
    // when there is no currentSelectedLocationId in the location stat grid
    var activeCellObj = statLocationGrid.getGridObj().getActiveCell();
    if (activeCellObj !== undefined && activeCellObj !== null) {
        if (currentSelectedLocationId !== undefined) {
            loadLocationStatDetail();
            if (currentSelectedLocationStat !== undefined) {
                loadCriticalUsers();
            }
            else {
                createCriticalUserGrid([]);
            }
        }
        else {
            createStatLocationDetailGrid([]);
        }
    }
    else {
        createCriticalUserGrid([]);
        createStatLocationDetailGrid([]);
    }
}

function OnClickStatLocationGrid(e, args) {
    if (args.rowData !== undefined) {
        currentSelectedLocationId = args.rowData.id;
    }
    loadLoactionStatAndCriticalUserFromCurSelGridRows();
}

function ordinalSuffixOf(i) {
    var j = i % 10,
        k = i % 100;
    if (j == 1 && k != 11) {
        return i + "st";
    }
    if (j == 2 && k != 12) {
        return i + "nd";
    }
    if (j == 3 && k != 13) {
        return i + "rd";
    }
    return i + "th";
}

function disableNavigateBtnWhileLoading(disable) {
    if (disable == false && uiControlData.showLatestPeriod === true) {
        $('#nextPeriodBtn').prop('disabled', true);
        $('#previousPeriodBtn').prop('disabled', true);
    }
    else {
        $('#nextPeriodBtn').prop('disabled', disable);
        $('#previousPeriodBtn').prop('disabled', disable);
    }
}

function uploadAggStatMetadataDialog() {
    var buttonProp = [
            {
                id: "uploadBtn",
                text: "Upload",
                click: function () {
                    callback();
                }
            },
            {
                text: "Close",
                click: function () {
                    uploadAggStatMetadataDialogInstance.dialog("close");
                }
            }];
    var callback = function () { };
    uploadAggStatMetadataDialogInstance = uploadAggStatMetadataDialogInstance || $("#dialogUploadAggStatMetadata").dialog({
        width: 600,
        dialogClass: "dialog-class",
        resizable: false,
        dragable: false,
        title: "Upload Metadata File",
        closeText: "",
        modal: true,
        buttons: buttonProp,
        close: function (e) {
            $(this).dialog('destroy');
            uploadAggStatMetadataDialogInstance = null;
        }
    });

    // It returns a function that is called when the dialog closes
    callback = uploadAggStatMetadataFile(uploadAggStatMetadataDialogInstance);
    return uploadAggStatMetadataDialogInstance.dialog("open");
}

// beforeSendCallback(xhr), successCallback(data), errorCallback(jqXhr, status, error)
function getAggStatMetadata(beforeSendCallback, successCallback, errorCallback) {
    $.ajax({
        dataType: "json",
        type: "POST",
        data: JSON.stringify({ product: uiControlData.selectProduct }),
        url: '/Apps/STOpsConsole/GetAggStatMetadata',
        beforeSend: beforeSendCallback,
        success: successCallback,
        error: errorCallback,
        contentType: "application/json"
    });
}

function showCurrentAggStatMetadataContent() {
    getAggStatMetadata(//beforeSendCallback
                function (xhr) {
                    $('#spanAggInterval').text("");
                    $('#metadataContent').val("Retrieving....");
                },
                //successCallback
                function (data) {
                    if (data !== "{}") {
                        $('#spanAggInterval').text(data.currentMinInterval);
                        var dataTemp = {};
                        for(var prop in data)
                        {
                            if (prop !== 'currentMinInterval') {
                                dataTemp[prop] = data[prop];
                            }
                        }
                        var jsonstr = JSON.stringify(dataTemp, null, "  ");
                        $('#metadataContent').val(jsonstr);
                    }
                    else {
                        $('#metadataContent').val("error");
                    }
                },
                //errorCallback
                function (jqXhr, status, error) {
                    var errorMsg;
                    if (error !== "") {
                        errorMsg = error;
                    }
                    else if (status !== "") {
                        errorMsg = status;
                    }
                    else {
                        errorMsg = "Failed";
                    }
                    $('#metadataContent').val(errorMsg);
                    $('#spanAggInterval').text("N/A");
                });
}

function presetUploadMetadataDialog() {
    showCurrentAggStatMetadataContent();
}

function uploadAggStatMetadataFile(dialog, metadataContent) {
    var data = { metadataForm: null };
    presetUploadMetadataDialog();

    // The returned function is called when the dialog closes
    return function () {
        // After this call the data object has the values that were edited
        dialog.syncData(data, true);
        var fd = new FormData(data.metadataForm);
        fd.append('product', uiControlData.selectProduct);
        $.ajax({
            url: data.metadataForm.action,
            type: data.metadataForm.method,
            beforeSend: function (xhr) { $('#uploadStatus').text("Uploading..."); },
            success: function (responseStatus) {
                $('#uploadStatus').text(responseStatus.description);
                presetUploadMetadataDialog();
            },
            error: function (jqXhr, status, error) {
                var errorMsg;
                if (error !== "") {
                    errorMsg = error;
                }
                else if (status !== "") {
                    errorMsg = status;
                }
                else {
                    errorMsg = "Failed";
                }
                $('#uploadStatus').text(errorMsg);
                presetUploadMetadataDialog();
            },
            data: fd,
            cache: false,
            contentType: false,
            processData: false,
            dataType: 'json'
        });
    };
}

function OnClickAggStatMetadataBtn() {
    uploadAggStatMetadataDialog();
    if (!isAllowToUploadMetadata()) {
        //$('#uploadBtn').prop('disabled', true);
        $('#uploadBtn').css('display', "none");
        $("#uploadMetadataForm").css("display", "none");
    }
}

function goPreviousPeriod() {
    if (currentTimeout !== undefined) {
        clearTimeout(currentTimeout);
    }
    ++currentStartPeriodRank;
    if (currentStartPeriodRank > 0) {
        $('#nextPeriodBtn').prop('disabled', false);
    }
    loadLocationStatus();
}

function goNextPeriod() {
    if (currentTimeout !== undefined) {
        clearTimeout(currentTimeout);
    }
    --currentStartPeriodRank;
    if (currentStartPeriodRank === 0) {
        $('#nextPeriodBtn').prop('disabled', true);
    }
    loadLocationStatus();
}

function OnDataReady() {
    initializeControlUI();
    loadLocationStatus();
}

function chkShowStatCodes_Changed() {
    uiControlData.showStatCodes = $("#chkShowStatCodes").prop("checked");
    if (rawLocationStatRecordDetailList !== undefined) {
        createStatLocationDetailGrid(getLocationStatReocrdDetailListView(rawLocationStatRecordDetailList));
        updateSpanStatName();
    }
}

function chkShowLatestPeriod_Changed() {
    uiControlData.showLatestPeriod = $("#chkShowLatestPeriod").prop("checked");
    if (uiControlData.showLatestPeriod === true) {
        $('#previousPeriodBtn').prop('disabled', true);
        currentStartPeriodRank = 1;
        goNextPeriod();
    }
    else {
        disableNavigateBtnWhileLoading(false);
    }
}

function formatGridValue(type, rawValue, unit, precision, convertUnit) {
    if (type === statDataType.Integer || type === statDataType.Float) {
        if (rawValue === null) {
            return "-";
        }
        var formattedValue;
        if (unit !== undefined) {
            var val = (convertUnit === true) ? convertNumberToLargerUnit(rawValue, unit) : { value: rawValue, unit: unit };
            formattedValue = ((precision !== -1) ? accounting.formatNumber(val.value, (val.unit !== unit) ? precision : 0) : numberWithCommas(val.value)) + " " + val.unit;
        }
        else {
            formattedValue = (precision !== -1) ? accounting.formatNumber(rawValue, (type === statDataType.Integer) ? 0 : precision) : numberWithCommas(rawValue);
        }
        return formattedValue;
    }
    else if (type === statDataType.Text) {
        var displayUnit = "";
        if (unit !== undefined && unit != null && unit.length > 0) {
            displayUnit = " " + unit;
        }
        return rawValue + displayUnit;
    }
    else if (type === statDataType.Boolean) {
        if (rawValue === null) {
            return "-";
        }
        if (rawValue == '0' || rawValue == '1') {
            return rawValue;
        }
        else {
            return accounting.formatNumber(rawValue, precision);
        }
    }
    else {
        return rawValue;
    }
}

function downloadExcel(infoType) {
    if (infoType == 'DownloadLocationStatData') {
        if (currentSelectedLocationId !== undefined) {

            apphitsApi.logHit("app", "STOpsConsole", "DLLocLoc");

            var url = '/Apps/STOpsConsole/Export';
            data = JSON.stringify(CreateGetLocationStatDumpRequestObj(currentSelectedLocationId)).replace(/\"/g, '&quot;');
            var s = '<form style="display: none;" action="' + urlWithAppendToPath('DownloadLocationStatData') + '" method="POST">';
            s += '<input type="text" name="json" value="' + data + '" />';
            s += '</form>';

            // add the form to the document 
            var $form = $(s).appendTo('body');

            // submit it 
            $form.submit();
        }
    }
}

function urlWithAppendToPath(toAppend) {
    var url = urlCombine(window.location.pathname, toAppend) + window.location.search;
    return url;
}

function urlCombine(left, right) {
    // initialize locals
    var leftEndsInSlash = false;
    var rightBegsInSlash = false;

    // set locals to real values
    if (left != null && left.length != 0)
        leftEndsInSlash = left.charAt(left.length - 1) == '/';
    if (right != null && right.length != 0)
        rightBegsInSlash = right.charAt(0) == '/';

    // if right is relative to the root, return just the right
    if (rightBegsInSlash)
        return right;

    // if left ends in slash return the two combined
    if (leftEndsInSlash)
        return left + right;

    // we need a slash, so combine the two adding a slash separator
    return left + '/' + right;
}

