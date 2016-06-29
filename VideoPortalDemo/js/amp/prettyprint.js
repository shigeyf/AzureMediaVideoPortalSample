var PrettyPrint = {};
function number2(number) {
    return ((number < 10) ? "0" : "") + number;
}

function number3(number) {
    return ((number < 100) ? "0" : "") + number2(number);
}

function toFixed(value, decimalPlaces) {
    if (undefined === value || null === value) {
        return "undefined";
    }
    return value.toFixed(decimalPlaces);
}
PrettyPrint.toFixed = toFixed;

function networkState(networkState) {
    var state;

    if (undefined === networkState || null === networkState) {
        return "undefined";
    }

    switch (networkState) {
        case HTMLMediaElement.NETWORK_EMPTY:
            state = "NETWORK_EMPTY";
            break;
        case HTMLMediaElement.NETWORK_IDLE:
            state = "NETWORK_IDLE";
            break;
        case HTMLMediaElement.NETWORK_LOADING:
            state = "NETWORK_LOADING";
            break;
        case HTMLMediaElement.NETWORK_NO_SOURCE:
            state = "NETWORK_NO_SOURCE";
            break;
        default:
            state = "UNKNOWN NETWORK STATE!";
    }
    return state;
}
PrettyPrint.networkState = networkState;

function readyState(readyState) {
    var state;

    if (undefined === readyState || null === readyState) {
        return "undefined";
    }

    switch (readyState) {
        case HTMLMediaElement.HAVE_NOTHING:
            state = "HAVE_NOTHING";
            break;
        case HTMLMediaElement.HAVE_METADATA:
            state = "HAVE_METADATA";
            break;
        case HTMLMediaElement.HAVE_CURRENT_DATA:
            state = "HAVE_CURRENT_DATA";
            break;
        case HTMLMediaElement.HAVE_FUTURE_DATA:
            state = "HAVE_FUTURE_DATA";
            break;
        case HTMLMediaElement.HAVE_ENOUGH_DATA:
            state = "HAVE_ENOUGH_DATA";
            break;
        default:
            state = "UNKNOWN READY STATE!";
    }
    return state;
}
PrettyPrint.readyState = readyState;

function errorInfo(err) {
    var errMsg = "code: " + toHexString(err.code);

    if (err.message) {
        errMsg += " msg: " + err.message;
    }
    return errMsg;
}
PrettyPrint.errorInfo = errorInfo;

function toHexString(n) {
    if (n < 0) {
        n = 0xFFFFFFFF + n + 1;
    }

    return "0x" + ("00000000" + n.toString(16).toUpperCase()).substr(-8);
}
PrettyPrint.toHexString = toHexString;

function timeTimeRange(range) {
    var output = "";

    if (!range) {
        return "undefined";
    }

    for (var idx = 0; idx < range.length; idx++) {
        output += "[" + range.start(idx).toFixed(3);
        output += ", " + range.end(idx).toFixed(3) + "] ";
    }

    if (range.length === 1) {
        output += " " + (range.end(range.length - 1) - range.start(0)).toFixed(3);
    }

    return output;
}
PrettyPrint.timeTimeRange = timeTimeRange;

function error(errorCode) {
    var errorDesc;
    var uiCodeMask = 0xff00000;
    var uiCode = (errorCode & uiCodeMask) >> 20;
    switch (uiCode) {
        case 0:
            errorDesc = "MEDIA_ERR_CUSTOM";
            break;
        case 1:
            errorDesc = "MEDIA_ERR_ABORTED";
            break;
        case 2:
            errorDesc = "MEDIA_ERR_NETWORK";
            break;
        case 3:
            errorDesc = "MEDIA_ERR_DECODE";
            break;
        case 4:
            errorDesc = "MEDIA_ERR_SRC_NOT_SUPPORTED";
            break;
        case 5:
            errorDesc = "MEDIA_ERR_ENCRYPTED";
            break;
        case 6:
            errorDesc = "SRC_PLAYER_MISMATCH";
            break;
        default:
            errorDesc = "MEDIA_ERR_UNKNOWN";
    }

    return errorDesc;
}
PrettyPrint.error = error;

function timeSec(time) {
    return number2(Math.floor(time / 3600)) + ":" + number2(Math.floor((time % 3600) / 60)) + ":" + number2(Math.floor(time % 60)) + "." + number3(Math.floor(time % 1 * 1000));
}
PrettyPrint.timeSec = timeSec;

function timeMilliSec(time_ms) {
    var time = time_ms / 1000;
    return timeSec(time);
}
PrettyPrint.timeMilliSec = timeMilliSec;

function log(txtLog, line) {
    var now = new Date();
    txtLog.value += now.getHours() + ":" + number2(now.getMinutes()) + ":" + number2(now.getSeconds()) + "." + number3(now.getMilliseconds()) + " | " + line + "\n";
    txtLog.scrollTop = txtLog.scrollHeight;
}
PrettyPrint.log = log;