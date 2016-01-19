$(function () {
    $('[data-resize]').each(function () {
        var dataResizer = this;
        $(dataResizer).bind("mouseover", function (e) {
            this.style.position = "relative";
        });
        var resizer = document.createElement("div");
        resizer.setAttribute("class", "resizer-" + $(this).attr("data-resize"));
        resizer.min = $(this).attr("data-resize-min");
        resizer.max = $(this).attr("data-resize-max");
        var position = new Object();
        $(resizer).bind("dragstart", function (e) {
            position.x = e.clientX;
            $(this).addClass("resizer-hover");
        });
        $(resizer).bind("drag", function (e) {
            $(dataResizer).addClass("data-resize-hover");
            dataResizer.style.position = "relative";
            var newSize = this.parentNode.offsetWidth + e.clientX - position.x;
            if (newSize < this.max && newSize > this.min) {
                this.parentNode.style.minWidth = newSize + "px";
                $(this).removeClass("resizer-alert");
            } else $(this).addClass("resizer-alert");
            position.x = e.clientX;
        });
        $(resizer).bind("dragend", function (e) {
            $(this).removeClass("resizer-hover").removeClass("resizer-alert");
            $(dataResizer).removeClass("data-resize-hover");
            dataResizer.style.position = "static";
            try { $(window).resize(); } catch (e) { }
        });
        $(this).append(resizer);
    });

    $('[data-collapsible]').each(function () {
        var collapsibleTarget = "#" + $(this).attr("data-collapsible");
        var collapsible = this;
        var collapsibleminHeight = $(collapsible).closest('.flex-item')[0].style.minHeight;
        $(collapsible).each(function () {
            $(this).bind("click", function (e) {
                var iconBlock = $(this).children('.icon-block');
                if (iconBlock.length) {
                    if (iconBlock.hasClass("icon-show")) {
                        $(collapsibleTarget).hide();
                        iconBlock.addClass("icon-hidden").removeClass("icon-show");
                        $(collapsible).closest('.flex-item')[0].style.height = 0;
                        $(collapsible).closest('.flex-item')[0].style.minHeight = collapsible.offsetHeight + "px";

                    } else {
                        $(collapsibleTarget).show();
                        iconBlock.addClass("icon-show").removeClass("icon-hidden");
                        $(collapsible).closest('.flex-item')[0].style.height = "auto";
                        $(collapsible).closest('.flex-item')[0].style.minHeight = collapsibleminHeight + "px";
                    }

                    // Resize the grids
                    chart.resize();
                    statRawDataGrid.resize();
                }
            });
        });
    });
});


// Selected item info
var currentSelectedGridRow = { statUserGridRow: undefined, statListGridRow: undefined, statRawDataGridRow: undefined };
var uiFilter = { showHidden: false, showDiscards: false, showCodes: false, selectProduct: "est" };
//currentStatDetail[data.stats[i].statName] = { statName: data.stats[i].statName, statDisplayName: data.stats[i].statDisplayName, discard: data.stats[i].discard, hidden: data.stats[i].hidden, unit: data.stats[i].unit, statLatestVal: data.stats[i].statLatestVal};

var gridSortIds = [];
var gridSortFields = [];
var gridSortDirections = []; // ascending: true, descending: false
var chartLoadingFailedMessage = 'Load failed';
var chartLoadingMessage = 'Loading...';

var usersAutocomplete;
function createAutocomplete(filter) {
    $.ui.autocomplete.prototype._renderItem = function (ul, item) {
        return $("<li></li>")
            .data("item.autocomplete", item)
            .append($("<a></a>").html(item.label))
            .appendTo(ul);
    };

    usersAutocomplete = new AjaxAutocomplete("#usersAutocomplete", {
        rpcUrl: urlWithAppendToPath("FindMachineInstall"),
        select: userSelected,
        filter: filter,
        product: uiFilter.selectProduct
    });
}

function userSelected(event, ui) {
    updateURLParam(ui);
    return true;
}

function highlightString(searchItem, pattern) {
    var matchStrings = [];
    var subStrings = pattern.split(" ");
    var subStringsLen = subStrings.length;
    for (var i = 0; i < subStringsLen; i++) {
        var subString = subStrings[i];
        matchStrings.push($.ui.autocomplete.escapeRegex(subString));
    }
    var matchRegex = matchStrings.join("|");
    var matcher = new RegExp("(" + matchRegex + ")", "ig");
    return searchItem.replace(matcher, '<strong class="autocomplete-match">$1</strong>');
}

function buildAutoCompleteResponse(findMachInstRes, searchString) {
    var res = [];
    if (findMachInstRes.items === undefined) {
        return res;
    }
    var machInstArr = findMachInstRes.items;
    for (var a=0; a<machInstArr.length; ++a) {
        var item = machInstArr[a];
        //if (item.machInstInfoList.length > 0) {
        //    for (var i = 0; i < item.machInstInfoList.length; ++i) {
                res.push({
                    label: highlightString(item.email, searchString) +
                        "<span>" + highlightString(item.uuid, searchString) + "</span>",
                        //"<span>" + item.machInstInfoList[i].hostName + "</span>" +
                        //"<span>Latest stat: " + formatToLocalDate(moment.utc(item.machInstInfoList[i].dateLastSeen).valueOf()) + "</span>",
                    value: "",
                    uuid: item.uuid
                    /*instGuid: item.machInstInfoList[i].instGUID,
                    machGuid: item.machInstInfoList[i].machGUID,
                    dateLastSeen: item.machInstInfoList[i].dateLastSeen,
                    hostName: item.machInstInfoList[i].hostName*/
                });
           // }
       // }
      /*  else {
            res.push({
                label: highlightString(item.email, searchString) +
                    "<span>" + highlightString(item.uuid, searchString) + "</span>" +
                    "<span>There is no stat available</span>",
                value: "",
                uuid: item.uuid,
                instGuid: "",
                machGuid: "",
                dateLastSeen: "1990-01-01T12:00:00.0",
                hostName: ""
            });
        }*/
    }
    return res.sort(function (a, b) {
        return moment.utc(b.dateLastSeen).valueOf() - moment.utc(a.dateLastSeen).valueOf();
    });
}

function AjaxAutocomplete(selector, props) {
    var url = props.rpcUrl, select = props.select, filter = props.filter, product = props.product;
    $(selector).autocomplete({
        source: function (request, response) {
            $.ajax({
                type: "POST",
                url: url,
                dataType: "json",
                data: JSON.stringify({ searchString: request.term, filter: filter, product: uiFilter.selectProduct }),
                success: function (data) {
                    response(buildAutoCompleteResponse(data, request.term));
                }
            });
        },
        minLength: 1,
        select: select,
        position: {
            my: "left top",
            at: "left bottom",
            collision: "flip none",
            using: function (position) {
                var listHeight = 0;

                $(this).find("a").each(function () {
                    listHeight += $(this).outerHeight(true);
                });

                $(this).css({
                    "top": position.top,
                    "left": position.left,
                    "height": (window.innerHeight - position.top - 20 < listHeight) ? window.innerHeight - position.top - 20 : "auto"
                })
            }
        }
    });
}

var version = "none";
function callServiceSynchronously(url, payload) {
    var rawResponse = $.ajax({
        dataType: "json",
        type: "POST",
        timeout: 30000, //<-- wait 30 seconds
        data: payload,
        url: url,
        async: false,
        contentType: "application/json"
    }).responseText;

    return $.parseJSON(rawResponse);
}

function callStatelessApp(method, input) {
    return callServiceSynchronously(urlWithAppendToPath(method), input);
}

function callStatelessAppAsync(method, input, callback) {
    return callServiceAsynchronously(urlWithAppendToPath(method), input, callback);
}

function callServiceAsynchronously(url, payload, callback) {
    $.ajax({
        dataType: "json",
        type: "POST",
        timeout: 30000, //<-- wait 30 seconds
        data: payload,
        url: url,
        success: callback,
        error: function (jqXhr, status, error) { console.log("ajax call error: " + status); },
        contentType: "application/json"
    });
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

function urlWithAppendToPath(toAppend) {
    var url = urlCombine(window.location.pathname, toAppend) + window.location.search;
    return url;
}

function useMockData() {
    return "file:" == document.location.protocol || window.parent.location.search.indexOf("useMockData") >= 0;
}

function hasAdminPermission() {
    var result = false;
    $.ajax({
        type: 'GET',
        url: "/Apps/DesktopDeployment/api/User/HasAdminPermission",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        error: function () {
        },
        success: function () {
            result = true;
        }
    });

    return result;
};

function downloadExcel(infoType) {
    if (infoType == 'DownloadHistoricalData') {
        apphitsApi.logHit("app", "STOpsConsole", "DLHist");
        var url = '/Apps/STOpsConsole/Export';
        data = JSON.stringify(CreateGetStatRequestObj(false, currentSelectedGridRow.statListGridRow)).replace(/\"/g, '&quot;');
        var s = '<form style="display: none;" action="' + urlWithAppendToPath('DownloadStatData') + '" method="POST">';
        s += '<input type="text" name="json" value="' + data + '" />';
        s += '</form>';

        // add the form to the document 
        var $form = $(s).appendTo('body');

        // submit it 
        $form.submit();
    } else if (infoType == 'DownloadUserData') {
        if (stPivotData != undefined) {
            apphitsApi.logHit("app", "STOpsConsole", "DLUserSum");
            var data = [["System Reports", "User Summary Data"]];
            data.push([""]);
            data.push(["User Name", currentUserInfo.firstName, currentUserInfo.lastName]);
            data.push(["User UUID", currentUserInfo.uuid]);
            data.push(["Location Name", currentUserInfo.locationName]);
            data.push(["Location ID", currentUserInfo.locationid]);
            data.push([""]);

            for (var i = 0; i < stPivotData.length; i++) {
                data.push(stPivotData[i]);
            }

            var csvContent = "data:text/csv;charset=utf-8,";
            data.forEach(function (infoArray, index) {
                dataString = infoArray.join(",");
                csvContent += index < data.length ? dataString + "\n" : dataString;
            });

            var fileName = "SystemReports-Data-" + currentUserInfo.uuid + "--" + dateFormat(new Date(), "yyyymmdd-hhMMss") + ".csv";
            var encodedUri = encodeURI(csvContent);
            var link = document.createElement("a");
            link.setAttribute("href", encodedUri);
            link.setAttribute("download", fileName);
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        }
    }else if (infoType == 'DownloadLocationStatData') {
        if (currentUserInfo.locationid !== undefined) {
            apphitsApi.logHit("app", "STOpsConsole", "DLLoc");
            var url = '/Apps/STOpsConsole/Export';
            data = JSON.stringify(CreateGetLocationStatDumpRequestObj(currentUserInfo.locationid)).replace(/\"/g, '&quot;');
            var s = '<form style="display: none;" action="' + urlWithAppendToPath('DownloadLocationStatData') + '" method="POST">';
            s += '<input type="text" name="json" value="' + data + '" />';
            s += '</form>';

            // add the form to the document 
            var $form = $(s).appendTo('body');

            // submit it 
            $form.submit();
        }
    } else if (infoType == 'DownloadStatData') {
        if (currentStatDetail != undefined) {
            var data = [["System Reports", "stat info"]];
            data.push([""]);
            data.push(["User Name", currentUserInfo.firstName, currentUserInfo.lastName]);
            data.push(["User UUID", currentUserInfo.uuid]);
            data.push(["Location Name", currentUserInfo.locationName]);
            data.push(["Location ID", currentUserInfo.locationid]);
            data.push([""]);
            data.push(["display name","value","unit"]);
            for (var i = 0; i < currentStatDetail.length; i++) {
                if(currentStatDetail[i]!=undefined){
                    data.push([currentStatDetail[i].statDisplayName, currentStatDetail[i].statLatestVal, currentStatDetail[i].unit]);
                }
            }

            var csvContent = "data:text/csv;charset=utf-8,";
            data.forEach(function (infoArray, index) {
                dataString = infoArray.join(",");
                csvContent += index < data.length ? dataString + "\n" : dataString;
            });

            var fileName = "StatInfoReports-Data-" + currentUserInfo.uuid + "--" + dateFormat(new Date(), "yyyymmdd-hhMMss") + ".csv";
            var encodedUri = encodeURI(csvContent);
            var link = document.createElement("a");
            link.setAttribute("href", encodedUri);
            link.setAttribute("download", fileName);
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        }
    }

}

// beforeSendCallback(xhr), successCallback(data), errorCallback(jqXhr, status, error)
function getMetaVersion(beforeSendCallback, successCallback, errorCallback) {
    $.ajax({
        dataType: "json",
        type: "POST",
        data: JSON.stringify({ product: uiFilter.selectProduct }),
        url: '/Apps/STOpsConsole/GetMetadataVersion',
        beforeSend: beforeSendCallback,
        success: successCallback,
        error: errorCallback,
        contentType: "application/json"
    });
}
// beforeSendCallback(xhr), successCallback(data), errorCallback(jqXhr, status, error)
function getMetadata(beforeSendCallback, successCallback, errorCallback) {
    $.ajax({
        dataType: "json",
        type: "POST",
        data: JSON.stringify({ product: uiFilter.selectProduct }),
        url: '/Apps/STOpsConsole/GetMetadata',
        beforeSend: beforeSendCallback,
        success: successCallback,
        error: errorCallback,
        contentType: "application/json"
    });
}

var statListGrid;
var groupItemMetadataProvider;
function createStatListGrid(statListDataView) {
    if (statListGrid === undefined) {
        var columns = [
            { id: "number", name: "", field: "number", minWidth: 23, maxWidth: 23, cssClass: "cell-title", headerCssClass: "cell-align-left" },
            { id: "display", name: "Title", field: "display", minWidth: 50, cssClass: "cell-title", headerCssClass: "cell-align-left", sortable: true },
            { id: "value", name: "Value", field: "value", width: 40, cssClass: "cell-align-right", headerCssClass: "cell-align-right", formatter: formatStatListGridValue }
        ];
        groupItemMetadataProvider = new Slick.Data.GroupItemMetadataProvider();

        var gridStatListOptions = {
            editable: false,
            enableCellNavigation: true,
            forceFitColumns: true,
            topPanelHeight: 25,
            rowHeight: 25,
            enableColumnReorder: false,
            enableTextSelectionOnCells: true
        };

        statListGrid = new Grid("#gridStatList", { data: statListDataView, columns: columns, options: gridStatListOptions, groupMetadataProvider: groupItemMetadataProvider, events: { onclick: OnClickStatListGrid, onContextMenu: onContextMenuStatListGrid } });
        var statListGridObj = statListGrid.getGridObj();
        var dataView = statListGrid.getDataView();

        dataView.onRowCountChanged.subscribe(function (e, args) {
            statListGridObj.updateRowCount();
            statListGridObj.render();
        });

        dataView.setGrouping({
            getter: "category",
            formatter: function (g) {
                return g.value + "&nbsp;<span>(&nbsp;" + g.count + "&nbsp;items)</span>";
            },
            collapsed: false,
            aggregateCollapsed: true,
            lazyTotalsCalculation: true
        });

        if (statListDataView.length > 0) {
            // Sort ascending on client by default
            var ascendingSort = true;
            var sortfieldId = "display";
            statListGridObj.setSortColumn(sortfieldId, ascendingSort);
            initializeGridSortCache(statListGrid, sortfieldId, ascendingSort);
            sortGridCorrectly(statListGrid);

            // Navigate from the dashboard (on the first load)
            if (currentQueryStringParam.statCode !== undefined && currentSelectedGridRow.statListGridRow === undefined) {
                currentSelectedGridRow.statListGridRow = currentQueryStringParam.statCode;
            }
            // Load default stat history (the first row in statListGrid)
            statListGrid.fixRowSelection();
        }
        else {
            createChart(null, [], "", false, "");
            createStatRawDataGrid(null, [], null);
        }

        //$("#inlineFilterPanel").appendTo(statListGrid.getGridObj().getTopPanel()).show();
        dataView.setFilterArgs({
            searchString: ""
        });
        dataView.setFilter(statFilter);
    }
    else {
        updateStatListGrid(statListDataView);
    }
    $('#gridStatList').css('height', mainLayout.state.west.innerHeight - $('#statInfoHeader').height() - 1);
    statListGrid.getGridObj().resizeCanvas();
    var rowIdx = statListGrid.getGridObj().getViewIndexByID(currentSelectedGridRow.statListGridRow);
    statListGrid.getGridObj().scrollRowIntoView(rowIdx, true);
}

//function toggleFilterPanel() {
//    var statListGridObj = statListGrid.getGridObj();
//    statListGridObj.setTopPanelVisibility(!statListGridObj.getOptions().showTopPanel);
//}

function statFilter(item, args) {
    var statTitle = item['display'].toLowerCase();
    var locaseSearchString = args.searchString.toLowerCase();
    if (locaseSearchString != "" && statTitle.indexOf(locaseSearchString) == -1) {
        return false;
    }
    return true;
}

$("#statFilterTextInput").keyup(function (e) {
    // clear on Esc
    if (e.which == 27) {
        this.value = "";
    }
    updateStatFilter(this.value);
});

function updateStatFilter(statFilterString) {
    var dataView = statListGrid.getDataView();
    dataView.setFilterArgs({
        searchString: statFilterString
    });
    dataView.refresh();
    statListGrid.fixRowSelection();
}

function updateStatListGrid(statListDataView) {
    var dataView;
    var statListGridObj = statListGrid.getGridObj();

    statListGrid.setData(statListDataView);
    dataView = statListGrid.getDataView();

    dataView.refresh();

    statListGridObj.invalidate();
    sortGridCorrectly(statListGrid);
    statListGrid.fixRowSelection();
}

//var userInfoGrid;
//function createUserInfoGrid(data) {
//    var dataView;
//    var userInfoGridObj;
//    if (userInfoGrid == null) {
//        var columns = [
//            { id: "number", name: "#", field: "number", minWidth: 28, maxWidth: 28, cssClass: "cell-align-right monospace", headerCssClass: "cell-align-right" },
//            { id: "display", name: "Topic", field: "display", minWidth: 50, cssClass: "cell-title", sortable: true },
//            { id: "value", name: "Value", field: "value", width: 200, minWidth: 50, cssClass: "cell-align-right monospace", headerCssClass: "cell-align-right" }
//        ];
//        var gridUserInfoOptions = {
//            editable: false,
//            enableAddRow: false,
//            enableCellNavigation: true,
//            forceFitColumns: true,
//            topPanelHeight: 25,
//            rowHeight: 25,
//            enableTextSelectionOnCells: true
//        };
//        userInfoGrid = new Grid("#gridUserInfo", { data: data, columns: columns, options: gridUserInfoOptions, events: { onclick: OnClickUserInfoGrid } });
//        var ascendingSort = true;
//        var sortfieldId = "display";
//        userInfoGridObj = userInfoGrid.getGridObj();
//        userInfoGridObj.setSortColumn('display', ascendingSort);
//        initializeGridSortCache(userInfoGrid, sortfieldId, ascendingSort);
//        userInfoGridObj.invalidate();
//    } else {
//        userInfoGridObj = userInfoGrid.getGridObj();

//        userInfoGrid.setData(data);
//        dataView = userInfoGrid.getDataView();
//        dataView.refresh();
//        userInfoGridObj.invalidate();
//    }
//    sortGridCorrectly(userInfoGrid);
//}

function ConvertStatValue(type, rawValue) {
    var value;
    if (type === statDataType.Integer || type === statDataType.Float) {
        if (rawValue !== "") {
            value = (type === statDataType.Integer) ? Number(Math.floor(rawValue)) : Number(rawValue);
            if (isNaN(value)) {
                value = null;
            }
        }
        else {
            value = null;
        }
    }
    else if (type === statDataType.Boolean) {
        if (rawValue === "0" || rawValue === "1") {
            value = rawValue;
        }
        else {
            value = null;
        }
    }
    else {
        value = rawValue;
    }
    return value;
}

function convertIso8601DurationToYLabel(rawValue, unit) {    
    var duration = moment.duration(rawValue);
    if (duration == null) {
        return rawValue;
    }
    
    if (duration.as('milliseconds') == 0) {
        return "0";
    }
    
    var timeUnit = getUnitString()[unitCatEnum.Time];
    var step = timeUnit.indexOf(unitString_ms);
    var unitStep = timeUnit.indexOf(unit);
    if (unitStep == step + 3) { // convert to hour
        return duration.as('hours').toString();
    } else if (unitStep == step + 2) {// convert to minute
        return duration.as('minutes').toString();
    }
    return duration.as('seconds').toString();
}

function convertIso8601DurationToString(rawValue) {
    var duration = moment.duration(rawValue);
    if (duration == null) {
        return rawValue;
    }
    return convertDurationToString(duration);
}

function convertDurationToString(duration) {
    var timeUnit = getUnitString()[unitCatEnum.Time];
    var step = timeUnit.indexOf(unitString_ms);
    if (duration.asMilliseconds() < 1000) {
        return Math.floor(duration.milliseconds()) + " " + timeUnit[step];
    } else if (duration.asMilliseconds() < 60000) {
        return duration.as('seconds') + " " + timeUnit[step + 1];
    } else {
        return zeroPadding(Math.floor(duration.as('hours'))) + ":" + zeroPadding(duration.minutes()) + ":" + zeroPadding(duration.seconds());
    }
}

function zeroPadding(intValue) {
    return intValue < 10 ? '0' + intValue : intValue;
}

function convertBooleanToString(boolVal) {
    return (boolVal === '1') ? "True" : "False";
}

function formatGridValue(type, rawValue, unit, precision, convertUnit) {
    if (type === statDataType.Integer || type === statDataType.Float) {
        if (rawValue === null) {
            return "-";
        }
        var formattedValue;
        if (unit !== undefined) {
            var val = (convertUnit === true) ? convertNumberToLargerUnit(rawValue, unit) : { value: rawValue, unit: unit };
            formattedValue = ((precision !== -1) ? accounting.formatNumber(val.value, (val.unit !== unit) ? precision : (convertUnit === true) ? 0 : precision) : numberWithCommas(val.value)) + " " + val.unit;
        }
        else {
            formattedValue = (precision !== -1) ? accounting.formatNumber(rawValue, (type === statDataType.Integer)? 0: precision) : numberWithCommas(rawValue);
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
        return convertBooleanToString(rawValue);
    }
    else if (type === statDataType.Iso8601Duration) {
        if (rawValue === null) {
            return "-";
        }
        return convertIso8601DurationToString(rawValue);
    }
    else {
        return rawValue;
    }
}

function formatToLocalDate(utcInMs) {
    return moment.utc(utcInMs).local().format('lll');
}

function formatStatRawDataGridValue(row, cell, value, columnDef, dataContext) {
    var type = undefined;
    if (getCurrentStatDetail()[currentSelectedGridRow.statListGridRow] == undefined)
        type = getCurrentStatDetail()[currentSelectedGridRow.statListGridRow].dataType;   
    return formatGridValue(type, value, undefined, 2, false);
}

function formatBlankColumn(row, cell, value, columnDef, dataContext) {
    return "";
}


function formatStatRawDataGridDate(row, cell, value, columnDef, dataContext) {
    return formatToLocalDate(value);
}

function formatStatListGridValue(row, cell, value, columnDef, dataContext) {
    if (dataContext.id == "fail" || dataContext.id == "alert" || dataContext.id == "warn") {
        value = value.split("[")[0];
    }
    var val = ConvertStatValue(dataContext.dataType, value);
    if (dataContext.id == "124") // Temporary Internet Files folder size => unit in GB
        return formatGridValue(dataContext.dataType, val, dataContext.unit, 2, false);
    return formatGridValue(dataContext.dataType, val, dataContext.unit, 2, true);
}

var statRawDataGrid;
function createStatRawDataGrid(type, data, unit) {
    //var selStat = getCurrentStatDetail()[currentSelectedGridRow.statListGridRow];
    var valueColHeader = "Values";
    var displayUnit = "";
    if (type !== statDataType.Boolean && unit !== undefined && unit != null && unit.length > 0) {
        displayUnit = " (" + unit + ")";
    }
    valueColHeader = valueColHeader + displayUnit;
    var columns = [
        { id: "number", name: "#", field: "number", minWidth: 28, maxWidth: 40, cssClass: "cell-align-right monospace", headerCssClass: "cell-align-right" },
        { id: "date", name: "Date", field: "date", minWidth: 170, width:170, cssClass: "cell-align-left monospace", sortable: true, defaultSortAsc: false, formatter: formatStatRawDataGridDate },
        { id: "value", name: valueColHeader, field: "value", minWidth: 160, width: 160, cssClass: "cell-align-right monospace", headerCssClass: "cell-align-right", sortable: true, defaultSortAsc: false, formatter: formatStatRawDataGridValue },
        { id: "", name: "", field: "", minWidth: 1, width:800, cssClass: "cell-align-left monospace", headerCssClass: "cell-align-left", sortable: false, defaultSortAsc: false, formatter: formatBlankColumn }
    ];

    var gridObj;
    if (statRawDataGrid == null) {
        var gridStatRawDataOptions = {
            editable: false,
            enableAddRow: false,
            enableCellNavigation: true,
            forceFitColumns: true,
            topPanelHeight: 25,
            rowHeight: 25,
            enableTextSelectionOnCells: true
        };
        statRawDataGrid = new Grid("#gridStatRawData", { data: data, columns: columns, options: gridStatRawDataOptions, events: { onclick: OnClickStatRawDataGrid } });
        var ascendingSort = false;
        var sortfieldId = "date";
        gridObj = statRawDataGrid.getGridObj();
        gridObj.setSortColumn(sortfieldId, ascendingSort);
        initializeGridSortCache(statRawDataGrid, sortfieldId);
        gridObj.invalidate();
    } else {
        gridObj = statRawDataGrid.getGridObj();
        var columns = gridObj.getColumns();
        columns[gridObj.getColumnIndex("value")].name = valueColHeader;
        gridObj.setColumns(columns);
        statRawDataGrid.setData(data);
        // the invalidate is needed or else the top row shows the wrong values when using the remote service
        // not sure why this doesn't seem to be needed when using mock data
        gridObj.invalidate();
    }
    sortGridCorrectly(statRawDataGrid);

    $('#gridStatRawData').css('height', mainLayout.state.east.innerHeight - $('#statHistoryHeader').height() - $('#statHistoricalDataHeader').height() - chart.chartHeight - 1);
    statRawDataGrid.getGridObj().resizeCanvas();
}

function displayNamePrefix(stat) {
    var prefixes = [];
    if (stat.hidden) prefixes.push("H");
    if (stat.discard) prefixes.push("D");
    if (prefixes.length == 0) return "";
    return "(" + prefixes.join('') + ") ";
}

function getStatDisplayName(stat) {
    var displayName = "";
    if (stat !== undefined) {
        if (uiFilter.showCodes)
            displayName = stat.statName;
        else if (stat.statDisplayName === undefined || stat.statDisplayName.length == 0)
            displayName = stat.statName;
        else
            displayName = stat.statDisplayName;
    }
    return displayName;
}

//function getStatDisplayNameWithUnit(stat) {
//    var displayName = getStatDisplayName(stat);
//    if (stat.unit !== undefined && stat.unit != null && stat.unit.length > 0) {
//        displayName = displayName + " (" + stat.unit + ")";
//    }
//    return displayName;
//}

setChartTheme();
var chart = null;
function createChart(type, data, unit, allowDecimalsOnYAxis, title) {
    if (data === undefined || data  == null) {
        return;
    }

    var option;
    if (type === statDataType.Boolean) {
        option = boolChartOption(data);
    }
    else if (type === statDataType.Integer || type === statDataType.Float) {
        option = numberChartOption(data, unit, allowDecimalsOnYAxis);
    }
    else if (type === statDataType.Iso8601Duration) {
        option = durationChartOption(data, unit);
    }
    else if (type !== null) {
        option = textChartOption(data, unit);    
    }
    else {
        option = noDataChartOption(title);
    }
    if (chart !== null) {
        chart.destroy();
    }
    chart = new Highcharts.Chart(option);
    chart.setSize(mainLayout.state.east.innerWidth, chart.chartHeight, false);
}

function noDataChartOption(title) {
    return {
        title: {
            style: {
                color: '#ccc',
                font: 'Bold 12px Arial, Helvetica, sans-serif'
            },
            text: title,
            floating: true,
            align: 'center',
            y: 24,
            x: -25
        },
        chart: {
            renderTo: 'chartWrapper'
        },
        yAxis: {
            title: {
                text: ' '
            }
        },
        legend: {
            enabled: false
        },
        series: [{
            data: []
        }]
    };
}

function findMin(data) {
    var min = Number.MAX_VALUE;
    for (var i = 0; i < data.length; ++i) {
        if (data[i].value !== null && data[i].value < min) {
            min = data[i].value;
        }
    }
    return min;
}

function convertToLargerUnit(data, unit) {
    var min = findMin(data);

    var val = toLargerUnit(min, unit, 1);
    var stepsToConvert = 0;
    while (val.result === true && val.value >= 1) {
        ++stepsToConvert;
        val = toLargerUnit(val.value, val.unit, 1);
    }
    for (var i = 0; i < data.length; ++i) {
        if (data[i].value !== null) {
            var val = toLargerUnit(data[i].value, unit, stepsToConvert);
            data[i].value = val.value;
        }
    }
    return val.unit;
}

function numberChartOption(data, unit, allowDecimalsOnYAxis) {
    var selStat = getCurrentStatDetail()[currentSelectedGridRow.statListGridRow];
    var chartTitle = getStatDisplayName(selStat);
    var seriesName = chartTitle;
    var displayUnit = "";
    if (unit !== undefined && unit != null && unit.length > 0) {
        displayUnit = " (" + unit + ")";
    }
    seriesName = seriesName + displayUnit;
    var min = Number.MAX_VALUE;
    var max = Number.MIN_VALUE;
    for (var i = 0; i < data.length; ++i) {
        if (data[i].value > max) {
            max = data[i].value;
        }
        if (data[i].value < min) {
            min = data[i].value;
        }
    }
    var diff = max - min;
    var yTitle = "Values";
    yTitle = yTitle + displayUnit;
    var divisor = 1;

    var margin;
    if (allowDecimalsOnYAxis) {
        $('#dummy').text(accounting.formatNumber(max, 7));
        margin = $('#dummy').width();
    }
    else {
        $('#dummy').text(accounting.formatNumber(max));
        margin = $('#dummy').width() + 15;
    }
    //if ((min / 1000) >= 100 && (max / 1000) >= 100 && diff >= 100000) {
    //    divisor = 1000;
    //    yTitle = "Values (x1,000)";
    //}
    var adjMin = min - (diff / 7)/2;
    var option = {
        chart: {
            renderTo: 'chartWrapper',
            marginRight: margin,
            zoomType: 'xy',
            panning: true,
            panKey: 'shift'
        },
        yAxis: {
            allowDecimals: allowDecimalsOnYAxis,
            min: adjMin > 0 ? adjMin : 0,
            labels: {
                formatter: function () {
                    return numberWithCommas(this.value / divisor);
                }
            },
            title: {
                offset: 7,
                x: -7,
                text: yTitle
            }
        },
        title: {
            text: chartTitle
        },
        tooltip: {
            formatter: function () {
                var type = getCurrentStatDetail()[currentSelectedGridRow.statListGridRow].dataType;
                var formattedValue = (type === statDataType.Integer) ? numberWithCommas(this.y) : accounting.formatNumber(this.y, 2);
                return '<span style="color:#ccc;font:Bold 11px Arial, Helvetica, sans-serif;">' + formatToLocalDate(this.key) + '</span>' +
                    '<table><tr><td style="font-size:12px;color: ' + this.series.color + '">' + this.series.name + ':&nbsp;</td>' +
                    '<td style="text-align: right"><span>' + formattedValue + '</span></td></tr></table>';
            }
        },
        series: [{
            name: seriesName,
            data: data.map(function (cur) {
                return [cur.date, cur.value];
            })
        }]
    };
    return option;
}

function textChartOption(data, unit) {
    var selStat = getCurrentStatDetail()[currentSelectedGridRow.statListGridRow];
    var chartTitle = getStatDisplayName(selStat);
    var seriesName = chartTitle;
    var displayUnit = "";
    if (unit !== undefined && unit != null && unit.length > 0) {
        displayUnit = " (" + unit + ")";
    }
    seriesName = seriesName + displayUnit;
    var allStringValues = {};
    for (var i = 0; i < data.length; ++i) {
        allStringValues[data[i].value] = 0;
    }

    var longestString = "";
    var allValues = [""];
    for (var stringValue in allStringValues) {
        if (stringValue.length > 0) {
            if (stringValue.length > longestString.length) {
                longestString = stringValue;
            }
            allValues.push(stringValue);
        }
    }

    $('#dummy').text(longestString);
    var width = $('#dummy').width() + 10;
    var yTitle = "Values";
    yTitle = yTitle + displayUnit;
    var option = {
        chart: {
            renderTo: 'chartWrapper',
            marginRight: width,
            zoomType: 'xy'
        },
        title: {
            text: chartTitle
        },
        yAxis: {
            showFirstLabel: true,
            showLastLabel: true,
            labels: {
                formatter: function () {
                    return allValues[this.value];
                }

            },
            title: {
                offset: 7,
                x: -7,
                text: yTitle
            }
            //categories: allValues
        },
        tooltip: {
            formatter: function () {
                return '<span style="color:#ccc;font:Bold 11px Arial, Helvetica, sans-serif;">' + formatToLocalDate(this.key) + '</span>' +
                    '<table><tr><td style="font-size:12px;color: ' + this.series.color + '">' + this.series.name + ':&nbsp;</td>' +
                    '<td style="text-align: right"><span>' + allValues[this.y] + '</span></td></tr></table>';
            }
        },
        series: [{
            name: seriesName,
            data: data.map(function (cur) {
                return [(new Date(cur.date)).getTime(), allValues.indexOf(cur.value)];
          
            })
        }]
    };
    return option;
}

function durationChartOption(data, unit) {
    var selStat = getCurrentStatDetail()[currentSelectedGridRow.statListGridRow];
    var chartTitle = getStatDisplayName(selStat);
    var seriesName = chartTitle;

    var maxDuration = Math.max.apply(Math, data.map(function (cur) { return moment.duration(cur.value).valueOf(); }));
    var maxDurationMoment = moment.duration(maxDuration);
    var longestString = convertDurationToString(maxDurationMoment);

    var timeUnit = getUnitString()[unitCatEnum.Time];
    var step = timeUnit.indexOf(unitString_ms);
    var modifyUnit = "";
    if (maxDurationMoment.as('hours') >= 1) {
        modifyUnit = timeUnit[step + 3];
    } else if (maxDurationMoment.as('minutes') >= 1) {
        modifyUnit = timeUnit[step + 2];
    } else {
        modifyUnit = timeUnit[step + 1];
    }
    
    var displayUnit = "";
    if (unit !== undefined && unit != null && unit.length > 0) {
        displayUnit = " (" + unit + ")";
    } else {
        displayUnit = " (" + modifyUnit + ")";
    }
    seriesName = seriesName;

    var durationScale = getDurationScale(maxDurationMoment);

    $('#dummy').text(longestString);
    var width = $('#dummy').width() + 10;
    var yTitle = "Values";
    yTitle = yTitle + displayUnit;
    var option = {
        chart: {
            renderTo: 'chartWrapper',
            marginRight: width,
            zoomType: 'xy'
        },
        title: {
            text: chartTitle
        },
        yAxis: {
            tickInterval: durationScale,
            showFirstLabel: true,
            showLastLabel: true,
            labels: {
                formatter: function () {
                    return convertIso8601DurationToYLabel(this.value, modifyUnit);
                }
            },
            title: {
                offset: 7,
                x: -7,
                text: yTitle
            }
        },
        tooltip: {
            formatter: function () {
                return '<span style="color:#ccc;font:Bold 11px Arial, Helvetica, sans-serif;">' + formatToLocalDate(this.key) + '</span>' +
                    '<table><tr><td style="font-size:12px;color: ' + this.series.color + '">' + this.series.name + ':&nbsp;</td>' +
                    '<td style="text-align: right"><span>' + convertIso8601DurationToString(this.y) + '</span></td></tr></table>';
            }
        },
        series: [{
            name: seriesName,
            data: data.map(function (cur) {
                return [(new Date(cur.date)).getTime(), moment.duration(cur.value).valueOf()]; 
            })
        }]
    };
    return option;
}

function getDurationScale(hoursDuration) {
    var milisecInHour = 3600000;
    var hoursWithDecimal = hoursDuration.as('hours');
    if ((1.0 <= hoursWithDecimal) && (hoursWithDecimal < 3.0)) {
        return 0.5 * milisecInHour;
    } else if (hoursWithDecimal >= 3.0) {
        var hours = Math.floor(hoursWithDecimal);
        var gridInterval1 = 1;
        var gridInterval2 = 2;
        var gridInterval5 = 5;
        var multiplier = Math.pow(10, hours.toString().length - 1);
        if (multiplier > 1) {
            var lessThan35 = (hours / multiplier) < 3.5;
            var gridMultiplier = lessThan35 ? (multiplier / 10) : multiplier;
            gridInterval1 = gridInterval1 * gridMultiplier;
            gridInterval2 = gridInterval2 * gridMultiplier;
            gridInterval5 = gridInterval5 * gridMultiplier;
        }
        var noOfGrid1 = hours / gridInterval1;
        if ((3 < noOfGrid1) && (noOfGrid1 < 7)) {
            return gridInterval1 * milisecInHour;
        }
        var noOfGrid2 = hours / gridInterval2;
        if ((3 < noOfGrid2) && (noOfGrid2 < 7)) {
            return gridInterval2 * milisecInHour;
        }
        return gridInterval5 * milisecInHour;
    }
    if (30 <= hoursDuration.minutes()) {
        return 600000; // 10 minutes = 600000 milisec
    }
    if (10 <= hoursDuration.minutes()) {
        return 300000; // 5 minutes = 300000 milisec
    }
    if (4 <= hoursDuration.minutes()) {
        return 120000; // 2 minutes = 120000 milisec
    }
    if (1 <= hoursDuration.minutes()) {
        return 60000; // 1 minutes = 60000 milisec
    }
    if (30 <= hoursDuration.seconds()) {
        return 10000; // 10 sec = 10000 milisec
    }
    if (10 <= hoursDuration.seconds()) {
        return 5000; // 5 sec = 5000 milisec
    }
    if (4 <= hoursDuration.seconds()) {
        return 2000; // 2 sec = 2000 milisec
    }
    return 1000; // 1 sec = 1000 milisec
}
    
function boolChartOption(data) {
    var selStat = getCurrentStatDetail()[currentSelectedGridRow.statListGridRow];
    var chartTitle = getStatDisplayName(selStat);
    var seriesName = chartTitle;

    var minDate = Number.MAX_VALUE;
    var maxDate = Number.MIN_VALUE;
    var minTime = Number.MAX_VALUE;
    var maxTime = Number.MIN_VALUE;
    for (var i=0; i < data.length; ++i) {
        var dt = moment.utc(data[i].date).local();                      // data[i].date is in UTC
        var d = new Date(dt.year(), dt.month(), dt.date()).getTime();   // Create Date object in local time
        var t = new Date(1970, 0, 1, dt.hour(), dt.minute()).getTime();
        if (d < minDate) {
            minDate = d;
        }
        if (d > maxDate) {
            maxDate = d;
        }
        if (t < minTime) {
            minTime = t;
        }
        if (t > maxTime) {
            maxTime = t;
        }
    }

    var allDates = [];
    var oneDayTick = Date.UTC(0, 0, 2) - Date.UTC(0, 0, 1);
    for (var cur = minDate; cur <= maxDate; cur += oneDayTick) {
        allDates.push(cur);
    }

    var transformFunc = function (cur) {
        var dt = moment.utc(cur.date).local();
        return [allDates.indexOf((new Date(dt.year(), dt.month(), dt.date())).getTime()), (new Date(1970, 0, 1, dt.hour(), dt.minute())).getTime()];
    };

    $('#dummy').text("00:00 PM");
    var width = $('#dummy').width() + 12;

    var option = {
        chart: {
            renderTo: 'chartWrapper',
            marginRight: width,
            type: "scatter",
            zoomType: 'xy'
        },
        title: {
            text: chartTitle
        },
        colors: ["#4b8b3e", "red"],
        yAxis: {
            min: minTime,
            max: maxTime,
            title: { text: "Time" },
            labels: {
                formatter: function () {
                    return moment(this.value).format("h:mm A");
                }
            }
        },
        xAxis: {
            showFirstLabel: true,
            showLastLabel: true,
            title: {
                offset: 7,
                x: -7
            },
            /*categories: allDates.map(function (cur) {
                return moment(cur).format("D MMM");
            })*/
            labels: {
                formatter: function () {
                    return moment(allDates[this.value]).format("D MMM");
                }
            },
        },
        tooltip: {
            formatter: function () {
                return '<table><tr><td style="color: ' + this.series.color + '">' + this.series.name + ':&nbsp;</td>' +
                        '<td style="text-align: right"><span>' + moment(allDates[this.x]).format('D MMM YY') + ' ' +
                        moment(this.y).format('h:mm A') + '</span></td></tr></table>';
            }
        },
        plotOptions: {
            scatter: {
                marker: {
                    radius: 4.4,
                    states: {
                        hover: {
                            enabled: true,
                            lineColor: 'rgb(100,100,100)'
                        }
                    }
                },
                states: {
                    hover: {
                        marker: {
                            enabled: false
                        }
                    }
                }
            }
        },
        series: [{
            name: "True",
            marker: {
                symbol: 'circle'
            },
            data: data.filter(function (cur) {
                return cur.value == 1;
            }).map(transformFunc)
        },
        {
            name: "False",
            marker: {
                symbol: 'circle'
            },
            data: data.filter(function (cur) {
                return cur.value == 0;
            }).map(transformFunc)
        }]
    };
    return option;
}

function setChartTheme() {
    Highcharts.theme = {
        global: {
            useUTC: false
        },
        colors: ["#f49b23", "#4b8b3e", "#1e9ff1", "#9d51cd"],
        chart: {
            marginRight: 55,
            type: 'area',
            backgroundColor: '#242424',
            borderWidth: 0,
            borderColor: '#000',
            borderRadius: 0,
            plotBackgroundColor: null,
            plotShadow: false,
            plotBorderWidth: 1,
            plotBorderColor: '#000',
            marginBottom: 20,
            style: {
                color: '#ccc',
                font: '12px Arial, Helvetica, sans-serif'
            }
        },
        plotOptions: {
            series: {
                animation: {
                    duration: 250
                },
                fillOpacity: 0.1,
                marker: {
                    enabled: true
                },
                connectNulls: true
            },
            column: {
                negativeColor: '#872021'
            }
        },
        credits: {
            enabled: false
        },
        title: {
            align: 'left',
            style: {
                color: '#ccc',
                font: 'Bold 14px Arial, Helvetica, sans-serif'
            },
            margin: 40
        },
        exporting: {
            enabled: false
        },
        legend: {
            align: 'left',
            y: 16,
            verticalAlign: 'top',
            floating: true,
            borderWidth: 0,
            itemDistance: 16,
            itemStyle: {
                color: '#ccc',
                fontSize: '12px'
            },
            itemHoverStyle: {
                color: '#fff'
            },
            symbolHeight: 10,
            symbolWidth: 10,
            symbolRadius: 2,
            lineHeight: 14
        },
        xAxis: {
            type: 'datetime',
            dateTimeLabelFormats: {
                millisecond: '%I:%M %p',
                second: '%l:%M %p',
                minute: '%l:%M %p',
                hour: '%l:%M %p',
                day: '%e %b',
                week: '%e %b',
                month: '%b %y',
                year: '%Y'
            },
            gridLineColor: '#2d2d2d',
            gridLineWidth: 1,
            minorGridLineColor: '#2d2d2d',
            alternateGridColor: '#1f1f1f',
            lineWidth: 1,
            lineColor: '#1f1f1f',
            tickWidth: 1,
            tickColor: '#2d2d2d',
            type: "datetime",
            showFirstLabel: false,
            labels: {
                style: {
                    color: '#999999',
                    font: 'Bold 11px Arial, Helvetica, sans-serif'
                }/*,
                formatter: function () {
                    return moment(this.value).format('D MMM');
                }*/
            }
        },
        yAxis: {
            allowDecimals: true,
            gridLineColor: '#2d2d2d',
            gridLineWidth: 1,
            minorGridLineColor: '#2d2d2d',
            minorTickLength: 1,
            lineWidth: 0,
            showFirstLabel: false,
            showLastLabel: false,
            opposite: true,
            tickWidth: 0,
            labels: {
                style: {
                    color: '#ccc',
                    font: 'Bold 11px Arial, Helvetica, sans-serif'
                }
            },
            title: {
                align: 'high',
                offset: 0,
                rotation: 0,
                floating: true,
                y: -6,
                x: 0,
                style: {
                    color: '#ccc',
                    font: 'Bold 11px Arial, Helvetica, sans-serif'
                }
            }
        },
        tooltip: {
            followPointer: false,
            backgroundColor: '#2d2c2c',
            borderColor: '#1f1f1f',
            crosshairs: [{
                width: 1,
                color: '#2d2c2c'
            }],
            style: {
                color: '#ccc',
                fontSize: '12px',
                padding: '8px'
            },
            useHTML: true
        },
        loading: {
            labelStyle: {
                color: '#ccc'
            },
            style: {
                backgroundColor: '#323232',
                opacity: 0.7
            }
        },
        lang: {
            numericSymbols: null
        },
        noData: {
            style: {
                fontSize: '12px',
                color: '#ccc',
                font: 'Bold 12px Arial, Helvetica, sans-serif'
            }
        },
    };

    Highcharts.setOptions(Highcharts.theme);
}

function showCurrentMetadataVersion() {
    getMetaVersion(//beforeSendCallback
                    function (xhr) {
                        $('#metadataVersion').text("Retrieving....");
                    },
                    //successCallback
                    function (data) {
                        $("#metadataVersion").text(data.version);
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
                        $('#metadataVersion').text(errorMsg);
                    });
}

function showCurrentMetadataContent() {
    getMetadata(//beforeSendCallback
                function (xhr) {
                    $('#metadataContent').val("Retrieving....");
                },
                //successCallback
                function (data) {
                    var jsonstr = JSON.stringify(data, null, "  ");
                    if (jsonstr !== "{}") {
                        $('#metadataContent').val(jsonstr);
                    }
                    else {
                        $('#metadataContent').val("");
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
                });
}

function presetUploadMetadataDialog() {
    showCurrentMetadataVersion();
    showCurrentMetadataContent();
}

var uploadMetadataDialogInstance;
function uploadMetadataDialog() {
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
                    uploadMetadataDialogInstance.dialog("close");
                }
            }];
    var callback = function () { };
    uploadMetadataDialogInstance = uploadMetadataDialogInstance || $("#dialogUploadMetadata").dialog({
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
            uploadMetadataDialogInstance = null;
        }
    });

    // It returns a function that is called when the dialog closes
    callback = UploadMetadataFile(uploadMetadataDialogInstance);
    return uploadMetadataDialogInstance.dialog("open");
}

function UploadMetadataFile(dialog, metadataContent) {
    var data = { metadataForm: null };
    presetUploadMetadataDialog();

    // The returned function is called when the dialog closes
    return function () {
        // After this call the data object has the values that were edited
        dialog.syncData(data, true);
        var fd = new FormData(data.metadataForm);
        fd.append('product', uiFilter.selectProduct);
        $.ajax({
            url: data.metadataForm.action,
            type: data.metadataForm.method,
            beforeSend: function (xhr) { $('#uploadStatus').text("Uploading..."); },
            success: function (responseStatus)
            {
                $('#uploadStatus').text(responseStatus.description);
                presetUploadMetadataDialog();
            },
            error: function (jqXhr, status, error)
            {
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


function createEditStatReq(statName, statDisplayName, category, discard, hidden) {
    return {
        statName: statName,
        statDisplayName: statDisplayName,
        category: category,
        discard: discard,
        hidden: hidden,
        product: uiFilter.selectProduct
    };
}

function createEditStatDataModel(statName, statDisplayName, category, discard, hidden, desc, rawDataType, basedUnit, interval, enabled) {
    var displayInterval = '-';
    if (interval !== undefined) {
        var convertedValue = convertNumberToLargerUnit(interval, 'min');
        displayInterval = interval + ' min';
        if (convertedValue.result === true) {
            displayInterval = convertedValue.value + ' ' + convertedValue.unit;
        }
    }
    return {
        statName: statName,
        statDisplayName: statDisplayName,
        category: category,
        discard: discard,
        hidden: hidden,
        desc: desc,
        rawDataType: rawDataType,
        basedUnit: (basedUnit === undefined) ? '-' : basedUnit,
        interval: displayInterval,
        enabled: enabled
    };
}

function openRph(selectedStatInfo)
{    
    window.open('https://customers.thomsonreuters.com/rph/instl/sliver.aspx?topic=' + selectedStatInfo.stat.id);
}

var editStatDialogInstance;
function editStatDialog(selectedStatInfo) {
    var dialogID;
    var editStatDetailDataModel;
    var dialogTitle;
    if (selectedStatInfo.isInsert === true) {
        editStatDetailDataModel = { statName: "", statDisplayName: "", category: "", discard: false, hidden: false, /*unit: "",*/ enable: true, interval: "", category: "PC.INFO", type: "short", aggregate:"last" };
        dialogID = '#dialogInsertStat';
        dialogTitle = "System Test - Insert New Stat";
    }
    else {
        var curStat = getCurrentStatDetail()[selectedStatInfo.stat.id];

        editStatDetailDataModel = createEditStatDataModel(curStat.statName, curStat.statDisplayName, curStat.category, curStat.discard, curStat.hidden, curStat.desc, curStat.rawDataType, curStat.unit, curStat.interval, curStat.enabled);
        dialogID = '#dialogEditStat';
        dialogTitle = "System Test - Edit Stat";
    }

    var callback = function () { };
    editStatDialogInstance = editStatDialogInstance || $(dialogID).dialog({
        width: 600,
        dialogClass: "dialog-class",
        resizable: false,
        dragable: false,
        title: dialogTitle,
        closeText: "",
        modal: true,
        buttons: [
            {
                text: "Save",
                click: function () {
                    callback();
                    editStatDialogInstance.dialog("close");
                }
            },
            {
                text: "Cancel",
                click: function () {
                    editStatDialogInstance.dialog("close");
                }
            }],
        close: function (e) {
            $(this).dialog('destroy');
            editStatDialogInstance = null;
        }
    });

    // It returns a function that is called when the dialog closes
    callback = updateStat(editStatDialogInstance, editStatDetailDataModel, selectedStatInfo.isInsert);
    return editStatDialogInstance.dialog("open");
}

function updateStat(dialog, editStatDataMode, isInsert) {
    // The initial call syncs the object data into the dialog
    dialog.syncData(editStatDataMode);

    // The returned function is called when the dialog closes
    if (isInsert === true) {
        return function () {
            // After this call the editStatDataMode object has the values that were edited
            dialog.syncData(editStatDataMode, true);
        };
    }
    else {
        return function () {
            // After this call the editStatDataMode object has the values that were edited
            dialog.syncData(editStatDataMode, true);
            editStatDataMode.statDisplayName = editStatDataMode.statDisplayName.trim();
            editStatDataMode.category = editStatDataMode.category.trim();

            var req = createEditStatReq(editStatDataMode.statName,
                                        editStatDataMode.statDisplayName,
                                        editStatDataMode.category,
                                        editStatDataMode.discard,
                                        editStatDataMode.hidden);
            var resp = callStatelessApp('/Apps/STOpsConsole/EditStat', JSON.stringify(req));
            if (resp.success != undefined && resp.success) {
                // Update the currentStatDetail
                var curStat = getCurrentStatDetail()[editStatDataMode.statName];
                curStat.statDisplayName = editStatDataMode.statDisplayName;
                curStat.discard = editStatDataMode.discard;
                curStat.hidden = editStatDataMode.hidden;
                //curStat.unit = editStatDataMode.unit;
                curStat.category = editStatDataMode.category;
                updateStatListGrid(getCurrentStatListView());
                chart.setTitle({ text: getStatDisplayName(curStat) });
            }
        };
    }
}

var checkUserScopeDialogInstance;
function checkUserScopeDialog() {
    var buttonProp = [
            {
                id: "checkBtn",
                text: "Check",
                click: function () {
                    callback();
                }
            },
            {
                text: "Close",
                click: function () {
                    checkUserScopeDialogInstance.dialog("close");
                }
            }];
    var callback = function () { };
    checkUserScopeDialogInstance = checkUserScopeDialogInstance || $("#dialogCheckUserScope").dialog({
        width: 600,
        dialogClass: "dialog-class",
        resizable: false,
        dragable: false,
        title: "Check User Scope",
        closeText: "",
        modal: true,
        buttons: buttonProp,
        close: function (e) {
            $(this).dialog('destroy');
            checkUserScopeDialogInstance = null;
        }
    });

    // It returns a function that is called when the dialog closes
    callback = CheckUserScope(checkUserScopeDialogInstance);
    return checkUserScopeDialogInstance.dialog("open");
}

function CheckUserScope(dialog) {
    var data = { checkUserScopeForm: null };    

    // The returned function is called when the dialog closes
    return function () {
        var loginUser = $('#loginUser').val();
        var lookupUser = $('#lookupUser').val();
        var searchString = $('#searchString').val();
        $.ajax({
            url: "/Apps/STOpsConsole/GetUserScope",
            type: "POST",
            beforeSend: function(xhr) {
                $('#checkUserScopeStatus').text("Checking...");
                $('#checkUserContent').text("");
            },
            success: function (response) {
                $('#checkUserScopeStatus').text(response.status);
                $('#checkUserContent').text(JSON.stringify(response,null,2));
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
                $('#checkUserScopeStatus').text("Error response");
                $('#checkUserScopeStatus').text(errorMsg);
            },
            data: JSON.stringify(createCheckUserScopeReq(loginUser, lookupUser, searchString)),
            contentType: "application/json",
            dataType: 'json'
        });
    };
}

function createCheckUserScopeReq(loginUser, lookedUpUser, searchString) {
    return {
        loginUser: loginUser,
        lookedUpUser: lookedUpUser,
        searchString: searchString
    };
}

function initializeGridSortCache(grid, sortField, sortAscending) {
    var gridObj = grid.getGridObj();
    var container = gridObj.getContainer();
    var columns = gridObj.getColumns();
    var sortColumn = gridObj.getSortColumns()[0];

    gridSortDirections[container] = sortAscending? sortAscending: false;
    if (typeof (sortColumn) != 'undefined') {
        gridSortIds[container] = sortColumn.columnId;
    }
    gridSortFields[container] = sortField ? sortField : columns[columns.length - 1].field;
}

// sort a grid using user selected column
function sortGridCorrectly(grid) {
    var container = grid.getContainer();
    grid.getGridObj().setSortColumn(gridSortIds[container], gridSortDirections[container]);
    grid.sort(gridSortFields[container], gridSortDirections[container]);
}

function getCurrentStatListView() {
    var curStatDetail = getCurrentStatDetail();
    var statList = [];
    var index = 0;
    for (var stat in curStatDetail) {
        if ((!uiFilter.showHidden && curStatDetail[stat].hidden) ||
            (!uiFilter.showDiscards && curStatDetail[stat].discard)) {
            continue;
        }
        //var displayName = displayNamePrefix(curStatDetail[stat]) + getStatDisplayNameWithUnit(curStatDetail[stat]);
        var displayName = displayNamePrefix(curStatDetail[stat]) + getStatDisplayName(curStatDetail[stat]);

        statList[index++] = {
            id: stat,
            display: displayName,
            value: curStatDetail[stat].statLatestVal,
            category: curStatDetail[stat].category !== '' ? curStatDetail[stat].category : 'Default',
            dataType: curStatDetail[stat].dataType,
            unit: curStatDetail[stat].unit
        }
    }
    return statList;
}

function getCurrentStatMetadata() {
    return currentStatMetadata;
}

var currentStatMetadata;
function loadStatMetadata() {
    getMetadata(function () { currentStatMetadata = undefined; },
                function (data) {
                    //currentStatMetadata = data;
                    currentStatMetadata = createStatMetadata(data);
                    if (getCurrentStatDetail() !== undefined) {
                        OnDataReady();
                    }
                },
                function (jqXhr, status, error) {
                    currentStatMetadata = null;
                });
}

function getCurrentStatDetail() {
    return currentStatDetail;
}

function getStats(beforeSendCallback, successCallback, errorCallback, data) {
    $.ajax({
        dataType: "json",
        type: "POST",
        data: data,
        url: '/Apps/STOpsConsole/GetStats',
        beforeSend: beforeSendCallback,
        success: successCallback,
        error: errorCallback,
        contentType: "application/json"
    });
}

function loadHistoricalStat(statName) {
    var reqHistoricalStat = CreateGetStatRequestObj(false, statName);
    //beforeSendCallback
    var beforeSend = function (xhr) {
        if (chart !== null) {
            if (chart.options.title.text !== chartLoadingFailedMessage && chart.options.title.text !== chartLoadingMessage) {
                chart.showLoading();
            }
            else {
                chart.setTitle({ text: chartLoadingMessage });
            }
        }
    };

    //successCallback
    var onSuccess = function (data) {
        if (getCurrentStatDetail() != undefined && getCurrentStatDetail()[statName] != undefined)
        {
            var unit = getCurrentStatDetail()[statName].unit;
            var allowDecimalsOnYAxis = false;

            // In some units, decimals are not necessary, because they are too small.
            // So we change the data type to statDataType.Integer so that the decimal part get removed and don't show 99.00 on the UI.
            if (unit === unitString_ms || unit === unitString_us || unit === unitString_ns) {
                getCurrentStatDetail()[statName].dataType = statDataType.Integer;
            }

            var type = getCurrentStatDetail()[statName].dataType;
            var statRawData = data.history.rows.map(function (row, i) {
                if (statName == "fail" || statName == "alert" || statName == "warn") {
                    row.statVal = row.statVal.split("[")[0];
                }
                return { id: i + 1, date: moment.utc(row.timeStamp).valueOf(), value: ConvertStatValue(type, row.statVal) };
            });

            if (unit != undefined && (type == statDataType.Float || type == statDataType.Integer)) {
                var previousUnit = unit;
                unit = convertToLargerUnit(statRawData, unit);
                if (previousUnit !== unit) {
                    type = statDataType.Float;
                }
            }
            if (type == statDataType.Float) {
                allowDecimalsOnYAxis = true;
            }
            createChart(type, statRawData, unit, allowDecimalsOnYAxis);
            createStatRawDataGrid(type, statRawData, unit);
        }        
    };

    //errorCallback
    var onError = function (jqXhr, status, error) {
        createChart(null, [], "", false, chartLoadingFailedMessage);
        createStatRawDataGrid(null, [], null);
    };

    getStats(beforeSend, onSuccess, onError, JSON.stringify(reqHistoricalStat));
    currentQueryStringParam.statCode = statName;
    updateBrowserHistory();
}

function CreateGetStatRequestObj(isGetAllStats, statName, testid, onlyTestid) {
    return {
        getAllStats: isGetAllStats,
        getStatHistory: statName !== undefined ? true : false,
        getStatHistoryFor: statName !== undefined ? statName : "",
        installGuid: currentQueryStringParam.iid !== undefined && !onlyTestid ? currentQueryStringParam.iid : "",
        machineGuid: currentQueryStringParam.mid !== undefined && !onlyTestid ? currentQueryStringParam.mid : "",
        uuid: currentQueryStringParam.uuid !== undefined && !onlyTestid ? currentQueryStringParam.uuid : "",
        product: "est",
        testid: !isNaN(Number(testid)) && testid !== undefined ? Number(testid) : 0
    };
}

$(document).ready(function () {
    getUserPermissions();
});

function setPageStatus() {
    //$('#filterPanelToggle').show();
    $('#histStatDownload').show();
    $('#statFilterTextInput').prop('disabled', false);
    $('#chkHidden').prop('disabled', false);
    $('#chkDiscards').prop('disabled', false);
    $('#chkShowCodes').prop('disabled', false);
}

function displayUserInfo(userInfo) {
    $('#HeaderName').text(userInfo.firstName + " " + userInfo.lastName);
    $('#HeaderUuid').text("("+userInfo.uuid+")");
    //$('#HeaderHostName').text(userInfo.hostName);
    $('#HeaderLocation').text(userInfo.locationName); 
    $('#HeaderLocationid').text("(" + userInfo.locationid+")");
    $('#locStatDownload').show();
}

var currentStatDetail;
var testStatus = {};

function loadStatDetail(uuid, testid, updateSTSummary, onlytestid) {
    var reqAllStat = CreateGetStatRequestObj(true, undefined, testid, onlytestid);
    currentStatDetail = undefined;

    //beforeSendCallback
    var beforeSend = function (xhr) { };

    //successCallback
    var onSuccess = function (data) {
        statDetailLoaded = true;
        currentStatDetail = [];
        currentUserInfo.firstName = data.firstName;
        currentUserInfo.lastName = data.lastName;
        currentUserInfo.uuid = data.uuid;
        currentUserInfo.email = data.email;
        currentUserInfo.locationName = data.locationName;
        currentUserInfo.locationid = data.locationId;
        testStatus = {};
        if (data.stats === undefined) {
            return;
        }
        for (var i = 0, len = data.stats.length; i < len; ++i) {
            currentStatDetail[data.stats[i].statName] = {
                statName: data.stats[i].statName,
                statDisplayName: data.stats[i].statDisplayName,
                category: (data.stats[i].category !== null && data.stats[i].category !== undefined) ? data.stats[i].category : "",
                discard: data.stats[i].discard,
                hidden: data.stats[i].hidden,
                unit: "",
                statLatestVal: data.stats[i].statLatestVal,
                dataType: "",
                rawDataType: "",
                desc: "",
                interval: "",
                enabled:""
            };
            if (data.stats[i].statName == 'alert' || data.stats[i].statName == 'fail' || data.stats[i].statName == 'warn') {
                var str = data.stats[i].statLatestVal.split('[');
                if (str.length > 1) {
                    str = str[1].substring(0, str[1].length - 1);
                    testStatus[data.stats[i].statName] = str.split(',');
                }
            }
        }
        if (currentStatDetail["CMPNAME"] !== undefined && currentStatDetail["CMPNAME"].statLatestVal !== undefined) {
            currentUserInfo.hostName = currentStatDetail["CMPNAME"].statLatestVal;
        }
        displayUserInfo(currentUserInfo);
        currentQueryStringParam.mid = data.machineGuid;
        currentQueryStringParam.iid = data.installGuid;
        currentQueryStringParam.uuid = data.uuid;
        if (updateSTSummary) {
            loadSystemTestSummaryData(data.uuid, "est");
        }
        if (getCurrentStatMetadata() !== undefined) {
            OnDataReady();
        }
        $('#statInfoDownload').show();
    };

    var onError = function (jqXhr, status, error) {
        createSystemTestStatUserGrid("", [], "");
        $('#statInfoDownload').hide();
    };

    //errorCallback
    var onError = function (jqXhr, status, error) { };

    getStats(beforeSend, onSuccess, onError, JSON.stringify(reqAllStat));
    currentQueryStringParam.testID = testid;    
}

function getParameterByName(name) {
    var url = parent.document.URL;
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(url);
    if (results == null)
        return "";
    else
        return decodeURIComponent(results[1].replace(/\+/g, " "));
}

//var currentUserInstallGuid;
//var currentUserMachineGuid;
//var currentUserUUID;
var mainLayout;
function OnDocumentReady() {
    var opsConsoleVersion = getOpsConsoleVersion();
    if (opsConsoleVersion != null && opsConsoleVersion != undefined) {
        $("#opsConsoleVersion").text("(" + opsConsoleVersion + ")");
    }
    window.parent.document.title = "User Stats - System Test";
    uiFilter.showDiscards = $("#chkDiscards").is(":checked");
    uiFilter.showHidden = $("#chkHidden").is(":checked");
    uiFilter.showCodes = $("#chkShowCodes").prop("checked");

    if (!isAllowToUploadMetadata()) {
        $('#metaDataBtn').css('display', "none");
        $('#checkUserBtn').css('display', "none");
    }

    if (!isInternalUser()) {
        $('#OpsConsoleBtn').css('display', "none");
        $('#LegacyBtn').css('display', "none");
    }

    mainLayout = $('body').layout({
        north__initClosed: false,
        north__resizable: false,
        north__slidable: false,
        west__initHidden: false,
        west__size: .35,
        west__onresize: function (pane, $pane, state, options) {
            $('#gridStatUserList').css('height', mainLayout.state.west.innerHeight - $('#statUserHeader').height() - 1);
            statUserGrid.getGridObj().resizeCanvas();
        },
        center__initHidden: false,
        center__onresize: function (pane, $pane, state, options) {
            $('#gridStatList').css('height', mainLayout.state.west.innerHeight - $('#statInfoHeader').height() - 1);
            statListGrid.getGridObj().resizeCanvas();
        },
        east__initHidden: false,
        east__onresize: function (pane, $pane, state, options) {
            chart.setSize(mainLayout.state.east.innerWidth, chart.chartHeight, false);
            $('#gridStatRawData').css('height', mainLayout.state.east.innerHeight - $('#statHistoryHeader').height() - $('#statHistoricalDataHeader').height() - chart.chartHeight - 1);
            statRawDataGrid.getGridObj().resizeCanvas();
        }
    });

    currentQueryStringParam = getQueryParams();
    // if filter = false, auto-complete returns only one installation that has the latest updated stats on a particular machine
    // Otherwise, it returns all installations including ones that have been installed on that machine
    var filter = true;
    if (currentQueryStringParam.filter !== undefined) {
        filter = (currentQueryStringParam.filter === 'false') ? false : true;
    }

    if (currentQueryStringParam.product !== undefined) {
        uiFilter.selectProduct = currentQueryStringParam.product;
    }
    else {
        uiFilter.selectProduct = "est";
    }
    $("#productSelect").val(getProductName(uiFilter.selectProduct));
    statDetailLoaded = false;
    createAutocomplete(filter);

    if (currentQueryStringParam.uuid !== undefined && currentQueryStringParam.uuid !== '') {
        loadStatMetadata();
        loadSystemTestSummaryData(currentQueryStringParam.uuid, uiFilter.selectProduct);        
        //loadStatDetail(currentQueryStringParam.mid, currentQueryStringParam.iid, currentQueryStringParam.uuid, false);        
    }
    //$.ajax({
    //    dataType: "json",
    //    type: "GET",
    //    timeout: 30000, //<-- wait 30 seconds
    //    contentType: "application/json",
    //    url: '/Apps/DesktopDeployment/api/installation/Details?id=' + getParameterByName("id"),
    //    success: function (data) {
    //        loadStatDetail(data.MachineUUID, data.InstGuid, data.UserKey);
    //        var userInformation = [
    //            { id: "1", display: "InstallationID", value: data.InstGuid },
    //            { id: "2", display: "MachineID", value: data.MachineUUID },
    //            { id: "3", display: "Installation Type", value: data.InstType },
    //            { id: "4", display: "Deployment Mode", value: data.DeploymentMode },
    //            { id: "5", display: "Computer", value: data.InstMachineName },
    //            { id: "6", display: "Windows Log-in", value: data.InstWindowsLogin },
    //            { id: "7", display: "User UUID", value: data.UserKey },
    //            { id: "8", display: "Eikon Login", value: data.UserLogin },
    //            { id: "9", display: "Location", value: data.AAALocationID },
    //            { id: "10", display: "Update service", value: data.UpdateServiceStatus },
    //            { id: "11", display: "EAP", value: data.IsInEAP },
    //            { id: "12", display: "Pre Release", value: data.IsInPreRelease },
    //            { id: "13", display: "Registration Date", value: moment.utc(data.ModifyDate).local().calendar() },
    //            { id: "14", display: "Current Products", value: data.CurrentCore },
    //            { id: "15", display: "Update", value: data.TargetedCore },
    //            { id: "16", display: "Set Next Update By", value: data.TargetedBy },
    //            { id: "17", display: "Set Next Update Date", value: data.TargetDate != null ? moment.utc(data.TargetDate).local().calendar() : "-" },
    //            { id: "18", display: "EikonDM Version", value: data.EikonDMVersion },
    //            { id: "19", display: "EikonUpdater Version", value: data.EikonUpdaterVersion }
    //        ];
    //        if (hasAdminPermission()) {
    //            userInformation.concat([
    //                { id: "20", display: "Customer User Login Count", value: data.CustomerCount },
    //                { id: "21", display: "Internal User Login Count", value: data.InternalCount },
    //                { id: "22", display: "Is Internal Machine", value: data.IsInternal },
    //                { id: "23", display: "Latest Internal User Login", value: data.InternalUser },
    //                { id: "24", display: "Has TR Certificate", value: data.TRCertificateExists },
    //                { id: "25", display: "Proxy Name", value: data.ProxyName },
    //                { id: "26", display: "Default Email Account", value: data.DefaultEmailAccount },
    //                { id: "27", display: "DNS Suffix", value: data.DnsSuffix },
    //                { id: "28", display: "Domain Name", value: data.DomainName }
    //            ]);
    //        }
    //        displayEntirePage(false);
    //        createUserInfoGrid(userInformation);
    //    },
    //    error: function (jqXhr, status, error) {
    //        displayEntirePage(true);
    //        console.log("ajax call error: " + status);
    //    }
    //});
}

// This is called after both loadStatDetail() and loadMetadata() get data sucessfully
// Note: getCurrentStatDetail() and getCurrentStatMetadata() have data
function OnDataReady() {
    var statMetadata = getCurrentStatMetadata();
    var statDetail = getCurrentStatDetail();

    //if (statMetadata.testschema !== undefined &&
    //    statMetadata.testschema.length !== undefined) {
    //    // Assign dataType and unit from metadata to currentStatDetail.
    //    // nameextensionas from the metadata is also taken into account.
    //    for (var i = 0; i < statMetadata.testschema.length; ++i) {
    //        if (statMetadata.testschema[i].allownameextension === "true" &&
    //            statMetadata.testschema[i].nameextensionas !== undefined &&
    //            statMetadata.testschema[i].name !== undefined &&
    //            statMetadata.enumdict !== undefined &&
    //            statMetadata.enumdict.length !== undefined) {
    //            for (var j = 0; j < statMetadata.enumdict.length; ++j) {
    //                if (statMetadata.enumdict[j].name === statMetadata.testschema[i].nameextensionas) {
    //                    if (statMetadata.enumdict[j].map !== undefined && statMetadata.enumdict[j].map.length !== undefined) {
    //                        for (var k = 0; k < statMetadata.enumdict[j].map.length; ++k) {
    //                            var statNameExtension = statMetadata.testschema[i].name + statMetadata.enumdict[j].map[k].id;
    //                            if (statDetail[statNameExtension] !== undefined) {
    //                                statDetail[statNameExtension].dataType = ConvertToStatDataTypeEnum(statMetadata.testschema[i].type);
    //                                statDetail[statNameExtension].unit = statMetadata.testschema[i].unit;
    //                                statDetail[statNameExtension].allownameextension = true;
    //                                statDetail[statNameExtension].nameextensionValue = statMetadata.enumdict[j].map[k].value;
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        else if (statMetadata.testschema[i].name !== undefined && statDetail[statMetadata.testschema[i].name] !== undefined) {
    //            statDetail[statMetadata.testschema[i].name].dataType = ConvertToStatDataTypeEnum(statMetadata.testschema[i].type);
    //            statDetail[statMetadata.testschema[i].name].unit = statMetadata.testschema[i].unit;
    //        }            
    //    }
    //}
    for (var statName in statDetail) {
        if (statMetadata[statName] !== undefined) {
            statDetail[statName].dataType = statMetadata[statName].dataType;
            statDetail[statName].rawDataType = statMetadata[statName].rawDataType;
            statDetail[statName].unit = statMetadata[statName].unit;
            statDetail[statName].desc = statMetadata[statName].desc;
            statDetail[statName].interval = statMetadata[statName].interval;
            statDetail[statName].enabled = statMetadata[statName].enabled;
        }
    }
    setPageStatus();
    createStatListGrid(getCurrentStatListView());
}

function OnClickStatUserGrid(e, args) {
        //if (currentSelectedGridRow.statUserGridRow != args.rowData.testID) {
        currentSelectedGridRow.statUserGridRow = args.rowData.testID;
        loadStatDetail(currentQueryStringParam.uuid, currentSelectedGridRow.statUserGridRow, false, true);
   // }
}

function OnClickStatListGrid(e, args) {
    if (!(args.rowData instanceof Slick.Group)) {
        if (args.rowData != undefined && args.rowData.id != undefined) {
            currentSelectedGridRow.statListGridRow = args.rowData.id;
            loadHistoricalStat(currentSelectedGridRow.statListGridRow);
        }
    }
}

function OnClickStatRawDataGrid(e, args) {
    if (currentSelectedGridRow.statRawDataGridRow != args.rowData.id) {
        currentSelectedGridRow.statRawDataGridRow = args.rowData.id;
    }
}

function OnClickMetadataBtn() {
    uploadMetadataDialog();
    if (!isAllowToUploadMetadata()) {
        //$('#uploadBtn').prop('disabled', true);
        $('#uploadBtn').css('display', "none");
        $("#uploadMetadataForm").css("display", "none");
    }
}
//OnClickCheckUserBtn
function OnClickCheckUserBtn() {
    checkUserScopeDialog();
}

function OnClickLocationStatBtn() {
    var productParam = "";
    productParam = "&product=" + uiFilter.selectProduct;

    if (productParam != "") {
        var newurl = '/Apps/STOpsConsole?page=SystemTestDashboard.html' + productParam;
        //parent.window.location.href = newurl;
        window.top.location.href = newurl;
    }
}

function OnClickOpsConsoleBtn() {
    apphitsApi.logHit("app", "STOpsConsole", "ClickOpsConsoleBtn");
    var newurl = '/Apps/OpsConsole';
    window.top.location.href = newurl;
}

function OnClickLegacySTBtn() {
    apphitsApi.logHit("app", "STOpsConsole", "ClickLLegacySTBtn");
    var uuidParam = "";
    if (typeof currentUserInfo.uuid != 'undefined') {
        uuidParam = currentUserInfo.uuid;
    }

    if (uuidParam != "") {
        var newurl = 'https://emea1.test.cp.ime.reuters.com/SystemTest/IRS/ReportByUUID.aspx?uuid=' + uuidParam;
        //parent.window.location.href = newurl;
        window.top.location.href = newurl;
    }
}

//function OnClickUserInfoGrid(e, args) {
//    if (currentSelectedGridRow.userInfoGridRow != args.rowData.id) {
//        currentSelectedGridRow.userInfoGridRow = args.rowData.id;
//    }
//}

function onChkDiscards_Changed() {
    uiFilter.showDiscards = $("#chkDiscards").is(":checked");
    updateStatListGrid(getCurrentStatListView());
}

function onChkHidden_Changed() {
    uiFilter.showHidden = $("#chkHidden").is(":checked");
    updateStatListGrid(getCurrentStatListView());
}

function chkShowCodes_Changed() {
    uiFilter.showCodes = $("#chkShowCodes").prop("checked");
    updateStatListGrid(getCurrentStatListView());

    if (chart != null) {
        var selStat = getCurrentStatDetail()[currentSelectedGridRow.statListGridRow];
        chart.setTitle({ text: getStatDisplayName(selStat) });
    }
}

function onProductSelect_Changed(ui) {
    uiFilter.selectProduct = getProductCode($("#productSelect").prop("value"));
}

function onContextMenuStatListGrid(e, grid) {
    var cell = this.getCellFromEvent(e);
    var data = this.getDataItem(cell.row);

    if (!(data instanceof Slick.Group)) {
        e.preventDefault();
        var menuItems = [];
        var cellNode = this.getCellNode(cell.row, cell.cell);
        menuItems.push({ label: "Help", action: function () { openRph({ stat: data }); } });
        if (isAllowToUploadMetadata())
            menuItems.push({ label: "Edit", action: function () { editStatDialog({ stat: data, cell: cell, isInsert: false }); } });

        $(cellNode).contextMenu({ items: menuItems, position: { top: e.pageY, left: e.pageX } });
        e.stopPropagation();
    }
}


//*****************************************************************************
//*  System Test User Stat Pane Logics                                        *
//*****************************************************************************

//*****************************************************************************
//*  Global variable                                                          *
//*****************************************************************************
var currentUserInfo = {
    mid: undefined,
    iid: undefined,
    uuid: undefined,
    stat: undefined,
    hostName: undefined,
    firstName: undefined,
    lastName: undefined,
    locationName: undefined,
    locationid: undefined
}
//Global Grid raw data
var stPivotData = undefined;
//**** End Global Variable ****************************************************

function loadSystemTestSummaryData(uuid, product) {
    // No stat data for the selected user
    if (!uuid) {
        return;
    }

    apphitsApi.logHit("app", "STOpsConsole", "UserSum");

    var data = JSON.stringify({
        uuid: uuid,
        products: ['est', 'ewm'],
        getStatHistoryFor: ['CMPNAME', 'runmode', 'total', 'pass', 'fail', 'alert', 'warn', 'info', 'other','testID']
    });
    var onSuccess = function (data) {
        if (data.history === undefined || data.history.length == 0) {
            return;
        }

        stPivotData = convertPivotData(data, uuid);

        if (stPivotData.length == 0 || stPivotData[0].length < 9) {
            createSystemTestStatUserGrid("", [], "");
            return;
        }
        createSystemTestStatUserGrid("", convertGridData(stPivotData), "");
        $('#userStatDownload').show();
    };

    var onError = function (jqXhr, status, error) {
        createSystemTestStatUserGrid("", [], "");
        $('#userStatDownload').hide();
    };

    sendWebRequest('GetSystemTestSummary', data, undefined, onSuccess, onError);
}

var statUserGrid;
function createSystemTestStatUserGrid(type, data, unit) {
    $.each(data, function () {
        this.type = type;
    });

    var valueColHeader = "Values";
    var displayUnit = "";
    if (type !== statDataType.Boolean && unit !== undefined && unit != null && unit.length > 0) {
        displayUnit = " (" + unit + ")";
    }
    valueColHeader = valueColHeader + displayUnit;
    var columns = [
        { id: "number", name: "#", field: "number", minWidth: 28, maxWidth: 40, cssClass: "cell-align-right monospace", headerCssClass: "cell-align-right" },
        { id: "date", name: "Date", field: "date", minWidth: 160, width: 160, cssClass: "cell-align-left monospace", sortable: true, defaultSortAsc: false, formatter: formatStatRawDataGridDate },
        { id: "testID", name: "Test ID", field: "testID", minWidth: 60, width: 100, cssClass: "cell-align-right monospace", headerCssClass: "cell-align-right", sortable: true, defaultSortAsc: false },
        { id: "CMPNAME", name: "PC Name", field: "CMPNAME", minWidth: 120, width: 150, cssClass: "cell-align-right monospace", headerCssClass: "cell-align-right", sortable: true, defaultSortAsc: false },
        { id: "runmode", name: "Run Mode", field: "runmode", minWidth: 65, width: 65, cssClass: "cell-align-right monospace", headerCssClass: "cell-align-right", sortable: true, defaultSortAsc: false },        
        { id: "fail", name: "Fail", field: "fail", minWidth: 40, width: 40, cssClass: "cell-align-right monospace", headerCssClass: "cell-align-right", sortable: true, defaultSortAsc: false },
        { id: "alert", name: "Alert", field: "alert", minWidth: 40, width: 40, cssClass: "cell-align-right monospace", headerCssClass: "cell-align-right", sortable: true, defaultSortAsc: false },
        { id: "warn", name: "Warning", field: "warn", minWidth: 40, width: 40, cssClass: "cell-align-right monospace", headerCssClass: "cell-align-right", sortable: true, defaultSortAsc: false },
        { id: "info", name: "Info", field: "info", minWidth: 40, width: 40, cssClass: "cell-align-right monospace", headerCssClass: "cell-align-right", sortable: true, defaultSortAsc: false },
        { id: "other", name: "Other", field: "other", minWidth: 40, width: 40, cssClass: "cell-align-right monospace", headerCssClass: "cell-align-right", sortable: true, defaultSortAsc: false },        
        { id: "pass", name: "Pass", field: "pass", minWidth: 40, width: 40, cssClass: "cell-align-right monospace", headerCssClass: "cell-align-right", sortable: true, defaultSortAsc: false },
        { id: "total", name: "Total", field: "total", minWidth: 40, width: 40, cssClass: "cell-align-right monospace", headerCssClass: "cell-align-right", sortable: true, defaultSortAsc: false },
        { id: "", name: "", field: "", minWidth: 1, width: 800, cssClass: "cell-align-left monospace", headerCssClass: "cell-align-left", sortable: false, defaultSortAsc: false, formatter: formatBlankColumn }
    ];

    var gridObj;
    if (statUserGrid == null) {
        var gridStatUserOptions = {
            editable: false,
            enableAddRow: false,
            enableCellNavigation: true,
            forceFitColumns: true,
            topPanelHeight: 25,
            rowHeight: 25,
            enableTextSelectionOnCells: true
        };
        statUserGrid = new Grid("#gridStatUserList", { data: data, columns: columns, options: gridStatUserOptions, events: { onclick: OnClickStatUserGrid } }, "opsTab");
        var ascendingSort = false;
        var sortfieldId = "date";
        gridObj = statUserGrid.getGridObj();
        gridObj.setSortColumn(sortfieldId, ascendingSort);
        initializeGridSortCache(statUserGrid, sortfieldId);
        gridObj.invalidate();
    } else {
        gridObj = statUserGrid.getGridObj();
        statUserGrid.setData(data);
        // the invalidate is needed or else the top row shows the wrong values when using the remote service
        // not sure why this doesn't seem to be needed when using mock data
        gridObj.invalidate();
    }
    sortGridCorrectly(statUserGrid);
    // Navigate from the dashboard (on the first load)
    if (currentQueryStringParam.testID !== undefined) {
        currentSelectedGridRow.statUserGridRow = currentQueryStringParam.testID;
    }
    statUserGrid.fixRowSelection();
    statUserGrid.getGridObj().resizeCanvas();
}

function convertPivotData(webData, uuid) {
    var key;
    var statArray = [];
    webData.history.map(function (row, index) {
        row.rows.map(function (r, i) {
            key = uuid + "/" + row.installGuid + "/" + row.machineGuid + "/" + row.product + "/" + r.timeStamp;
            statArray.push([key, row.statName, r.statVal])
            statArray.push([key, "date", r.timeStamp]);
            statArray.push([key, "product", row.product]);
            statArray.push([key, "mid", row.machineGuid]);
            statArray.push([key, "iid", row.installGuid]);
        });
    });

    var pivotData = getPivotArray(statArray, 0, 1, 2);
    return pivotData;
}

function convertGridData(pivotData) {
    var result = [];
    var columnRow = [];
    var tmpRow = [];
    var tmpRowStr;
    var rowLength;
    columnRow = pivotData[0];
    for (var i = 1; i < pivotData.length; i++) {
        tmpRow = pivotData[i];
        rowLength = tmpRow.length;
        tmpRowStr = '{ "id":"' + i + '",';
        for (var j = 0; j < rowLength; j++) {
            tmpRowStr += '"' + columnRow[j] + '":'
            var num = parseInt(tmpRow[j]);
            if (isNaN(num) || columnRow[j] == 'date') {
                tmpRowStr += '"' + tmpRow[j] + '"';
            } else {
                tmpRowStr += num;
            }
            if (j < rowLength - 1) { tmpRowStr += ","; }
        }
        tmpRowStr += "}";
        result.push(jQuery.parseJSON(tmpRowStr));
    }
    return result;
}

function getPivotArray(dataArray, rowIndex, colIndex, dataIndex) {
    var result = {}, ret = [];
    var newCols = [];
    for (var i = 0; i < dataArray.length; i++) {

        if (!result[dataArray[i][rowIndex]]) {
            result[dataArray[i][rowIndex]] = {};
        }
        result[dataArray[i][rowIndex]][dataArray[i][colIndex]] = dataArray[i][dataIndex];

        //To get column names
        if (newCols.indexOf(dataArray[i][colIndex]) == -1) {
            newCols.push(dataArray[i][colIndex]);
        }
    }

    newCols.sort();
    var item = [];

    //Add Header Row
    item.push('Item');
    item.push.apply(item, newCols);
    ret.push(item);

    //Add content 
    for (var key in result) {
        item = [];
        item.push(key);
        for (var i = 0; i < newCols.length; i++) {
            if (newCols[i] == 'fail' || newCols[i] == 'alert' || newCols[i] == 'warn')
            {
                if (result[key][newCols[i]] == undefined)
                    item.push("-");
                else
                {
                    var resSplit = result[key][newCols[i]].split("[");
                    if (resSplit[1] != undefined)
                        item.push(resSplit[0]);
                    else
                        item.push(result[key][newCols[i]] || "-");
                }
            }
            else
                item.push(result[key][newCols[i]] || "-");
        }
        ret.push(item);
    }
    return ret;
}

function onTestidSearch_Entered() {
    if (getCurrentStatMetadata() === undefined) {
        loadStatMetadata();
    }
    var updateSTSummary = true;
    var onlyTestID = true;
    statDetailLoaded = false;
    loadStatDetail("", $('#testidsearch').val(), updateSTSummary, onlyTestID);
}
