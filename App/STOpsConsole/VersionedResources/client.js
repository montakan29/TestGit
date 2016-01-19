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

function callServiceAsynchronously(url, payload, callback) {
    var xhr = new XMLHttpRequest();
    xhr.onload = function () {
        if (callback) {
            callback(xhr.responseText);
        }
    };

    xhr.onerror = function (args) {
        callback("XHR failed async");
    };

    xhr.open("POST", url);
    xhr.send(payload);
}

function callServiceSynchronously(url, payload) {
    var xhr = new XMLHttpRequest();
    var response = null;
    xhr.onload = function () {
        response = xhr.responseText;
    };

    xhr.onerror = function (args) {
        response = "XHR failed";
    };

    xhr.open("POST", url, false);
    xhr.send(payload);

    return response;
}

function callStatelessApp(method, input) {
    return callServiceSynchronously(urlWithAppendToPath(method), input);
}

function callStatelessAppAsync(method, input, callback) {
    return callServiceAsynchronously(urlWithAppendToPath(method), input, callback);
}
