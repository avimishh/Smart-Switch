// requires jQuery
const server = "https://backend.conveyor.cloud/api";
const usersApi = server + "/users";
const plugsApi = server + "/plugs";
const samplesApi = server + "/powerusagesamples";
const tasksApi = server + "/tasks";
const authApi = server + "/auth";

const tokenStorageKeyString = "smart-switch-api-token";

/**
 * Defines the possible operations we can call on a plug, either turn it on or off.
 * @enum Operations
 * */
const Operations = {
    /**
     * The operation that calls for a plug to turn on.
     * @type {integer} 
     * */
    TurnOn: 0,
    /**
     * The operation that calls for a plug to turn off.
     * @type {integer}
     * */
    TurnOff: 1
};


/**
 * Defines the possible types a task can, either repeating or activating just once.
 * @enum TaskTypes
 * */
const TaskTypes = {
    /**
     * The type of task that executes once.
     * @type {integer}
     * */
    OneTime: 0,
    /**
     * The type of task that executes repeatedly.
     * @type {integer}
     * */
    Repeated: 1
};

/**
 * Defines the possible priorities a plug can have, a plug can be essential, non-essential or neither (what we call irrelevant).
 * @enum Priorities
 * */
const Priorities = {
    /**
     * The priority that states a plug is essential.
     * @type {integer}
     * */
    Essential: 0,
    /**
     * The priority that states a plug is non-essential.
     * @type {integer}
     * */
    Nonessential: 1,
    /**
    * The priority that states a plug's priority is irrelevant.
    * @type {integer}
    * */
    Irrelevant: 2
};

$.ajaxSetup({
    statusCode: {
        401: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.responseText !== "5") {
                alert("Your session has expired, please login again.");
                parent.logout();
            }
        }
    }
});

/**
 * Registers a new user.
 * @param {string}      username            The new user's username.
 * @param {string}      password            The new user's password.
 * @param {function=}   successFunction     Function to execute upon success.
 * @param {function=}   errorFunction       Function to execute upon failure.
 * @param {function=}   completeFunction    Function to execute upon completion.
 */
function registerUser(username, password, successFunction, errorFunction, completeFunction) {
    if (username === undefined || password === undefined || successFunction === undefined) return;

    let request = {
        contentType: "application/json",
        url: authApi + "/register",
        method: "POST",
        data: JSON.stringify({
            Username: username,
            Password: password
        }),
        success: successFunction
    };

    if (errorFunction !== undefined) request.error = errorFunction;
    if (completeFunction !== undefined) request.complete = completeFunction;

    $.ajax(request);
}

/**
 * Signs a user in. (Gets an API token).
 * @param {string}      username            The new user's username.
 * @param {string}      password            The new user's password.
 * @param {function}    successFunction     Function to execute upon success.
 * @param {function=}   errorFunction       Function to execute upon failure.
 * @param {function=}   completeFunction    Function to execute upon completion.
 */
function loginUser(username, password, successFunction, errorFunction, completeFunction) {
    if (username === undefined || password === undefined || successFunction === undefined) return;

    let request = {
        contentType: "application/json",
        url: authApi + "/login",
        method: "POST",
        data: JSON.stringify({
            Username: username,
            Password: password
        }),
        success: function (data, textStatus, jqXHR) {
            sessionStorage.setItem(tokenStorageKeyString, data.token);
            successFunction(data, textStatus, jqXHR);
        }
    };

    if (errorFunction !== undefined) request.error = errorFunction;
    if (completeFunction !== undefined) request.complete = completeFunction;

    $.ajax(request);
}

/**
 * Changes an existing user's password.
 * @param {string}      username            The existing user's username.
 * @param {string}      oldPassword         The user's old password.
 * @param {string}      newPassword         The user's new password.
 * @param {function}    successFunction     Function to execute upon success.
 * @param {function=}   errorFunction       Function to execute upon failure.
 * @param {function=}   completeFunction    Function to execute upon completion.
 */
function changePassword(username, oldPassword, newPassword, successFunction, errorFunction, completeFunction) {
    if (username === undefined || oldPassword === undefined || newPassword === undefined) return;

    let request = {
        url: usersApi + "/" + username + "/password",
        contentType: "application/json",
        headers: { "Authorization": "bearer " + sessionStorage.getItem(tokenStorageKeyString) },
        method: "PUT",
        data: JSON.stringify({ "OldPassword": oldPassword, "NewPassword": newPassword })
    };

    if (successFunction !== undefined) request.success = successFunction;
    if (errorFunction !== undefined) request.error = errorFunction;
    if (completeFunction !== undefined) request.complete = completeFunction;

    $.ajax(request);
}

/**
 * Gets a plug JSON containing its mac, nickname, isOn, approved, priority, addedAt and iconNumber properties.
 * @param {string}      mac                 The plug's mac address.
 * @param {function}    successFunction     Function to execute upon success.
 * @param {function=}   errorFunction       Function to execute upon failure.
 * @param {function=}   completeFunction    Function to execute upon completion.
 */
function getPlug(mac, successFunction, errorFunction, completeFunction) {
    if (mac === undefined || successFunction === undefined) return;

    let request = {
        url: plugsApi + "/" + mac,
        method: "GET",
        headers: { "Authorization": "bearer " + sessionStorage.getItem(tokenStorageKeyString) },
        success: function (data, textStatus, jqXHR) {
            data.addedAt = new Date(data.addedAt);
            successFunction(data, textStatus, jqXHR);
        }
    };

    if (errorFunction !== undefined) request.error = errorFunction;
    if (completeFunction !== undefined) request.complete = completeFunction;

    $.ajax(request);
}

/**
 * Gets an array plug JSONs owned by a given user containing their mac, nickname, isOn, approved, priority, addedAt and iconNumber properties.
 * @param {string}      username            The username of the owner of the plugs.
 * @param {function}    successFunction     Function to execute upon success.
 * @param {function=}   errorFunction       Function to execute upon failure.
 * @param {function=}   completeFunction    Function to execute upon completion.
 */
function getUserPlugs(username, successFunction, errorFunction, completeFunction) {
    if (username === undefined || successFunction === undefined) return;

    let request = {
        url: plugsApi + "/user/" + username,
        method: "GET",
        headers: { "Authorization": "bearer " + sessionStorage.getItem(tokenStorageKeyString) },
        success: function (data, textStatus, jqXHR) {
            for (let i in data) {
                data[i].addedAt = new Date(data[i].addedAt);
            }
            successFunction(data, textStatus, jqXHR);
        }
    };

    if (errorFunction !== undefined) request.error = errorFunction;
    if (completeFunction !== undefined) request.complete = completeFunction;

    $.ajax(request);
}

/**
 * Updates a given plug's nickname and priority properties.
 * 
 * All properties of the plug will be updated (nickname and priority).
 * @param {object}      plugProperties      A JSON containing the plug's properties to be updated.
 * @param {function=}   successFunction     Function to execute upon success.
 * @param {function=}   errorFunction       Function to execute upon failure.
 * @param {function=}   completeFunction    Function to execute upon completion.
 * @example
 * let plugProperties = {
 *     "mac": "BC:DD:B2:23:D6:60",
 *     "nickname": "Fan",
 *     "iconUrl": "https://images-na.ssl-images-amazon.com/images/I/51zLZbEVSTL._SX679_.jpg",
 *     "priority": Priorities.Nonessential
 * }
 * 
 * function print(data) {
 *     console.log(JSON.stringify(data));
 * }
 * 
 * updatePlug(plugProperties, print);
 */
function updatePlug(plugProperties, successFunction, errorFunction, completeFunction) {
    if (plugProperties === undefined) return;

    let request = {
        url: plugsApi,
        method: "PUT",
        contentType: "application/json",
        headers: { "Authorization": "bearer " + sessionStorage.getItem(tokenStorageKeyString) },
        data: JSON.stringify(plugProperties)
    };

    if (successFunction !== undefined) request.success = successFunction;
    if (errorFunction !== undefined) request.error = errorFunction;
    if (completeFunction !== undefined) request.complete = completeFunction;

    $.ajax(request);
}

/**
 * Turns a given plug on or off.
 * @param {string}      mac                 The plug's mac address.
 * @param {integer}     _operation          The operation to perform, either Operations.TURNON or Operations.TURNOFF.
 * @param {function=}   successFunction     Function to execute upon success.
 * @param {function=}   errorFunction       Function to execute upon failure.
 * @param {function=}   completeFunction    Function to execute upon completion.
 * @example turnPlug("BC:DD:BD:23:D6:60", Operations.TurnOn);
 */
function turnPlug(mac, _operation, successFunction, errorFunction, completeFunction) {
    if (mac === undefined || _operation === undefined) return;

    let request = {
        url: plugsApi + "/" + mac + "?op=" + _operation,
        headers: { "Authorization": "bearer " + sessionStorage.getItem(tokenStorageKeyString) },
        method: "PUT"
    };

    if (successFunction !== undefined) request.success = successFunction;
    if (errorFunction !== undefined) request.error = errorFunction;
    if (completeFunction !== undefined) request.complete = completeFunction;

    $.ajax(request);
}

/**
 * Approves a given plug.
 * @param {string}      mac                 The plug's mac address.
 * @param {function=}   successFunction     Function to execute upon success.
 * @param {function=}   errorFunction       Function to execute upon failure.
 * @param {function=}   completeFunction    Function to execute upon completion.
 */
function approvePlug(mac, successFunction, errorFunction, completeFunction) {
    if (mac === undefined) return;

    let request = {
        url: plugsApi + "/" + mac + "?approved=true",
        headers: { "Authorization": "bearer " + sessionStorage.getItem(tokenStorageKeyString) },
        method: "PUT"
    };

    if (successFunction !== undefined) request.success = successFunction;
    if (errorFunction !== undefined) request.error = errorFunction;
    if (completeFunction !== undefined) request.complete = completeFunction;

    $.ajax(request);
}

/**
 * Denies a given plug.
 * @param {string}      mac                 The plug's mac address.
 * @param {function=}   successFunction     Function to execute upon success.
 * @param {function=}   errorFunction       Function to execute upon failure.
 * @param {function=}   completeFunction    Function to execute upon completion.
 */
function denyPlug(mac, successFunction, errorFunction, completeFunction) {
    if (mac === undefined) return;

    let request = {
        url: plugsApi + "/" + mac + "?approved=false",
        headers: { "Authorization": "bearer " + sessionStorage.getItem(tokenStorageKeyString) },
        method: "PUT"
    };

    if (successFunction !== undefined) request.success = successFunction;
    if (errorFunction !== undefined) request.error = errorFunction;
    if (completeFunction !== undefined) request.complete = completeFunction;

    $.ajax(request);
}

/**
 * Gets an array of powerUsageSample JSONs of samples between the given dates of a given plug, containing their sampleDate, current and voltage properties.
 * 
 * The array will be ordered by date, most recent last. Each sample is for one minute following its date.
 * @param {string}      mac                 The plug's (whose samples are requested) mac address.
 * @param {Date}        beginDate           The date from which to start getting samples.
 * @param {Date}        endDate             The date from which to stop getting samples.
 * @param {function}    successFunction     Function to execute upon success.
 * @param {function=}   errorFunction       Function to execute upon failure.
 * @param {function=}   completeFunction    Function to execute upon completion.
 * @example
 * let d = new Date();
 * d.setMinutes(d.getMinutes() - 5);
 * getPlugSamples("BB:DD:C2:23:D6:60", d, new Date(), function (samples) {
 *     for (let i in samples) {
 *         console.log(samples[i]);
 *     }
 * })
 */
function getPlugSamples(mac, beginDate, endDate, successFunction, errorFunction, completeFunction) {
    if (mac === undefined || beginDate === undefined || endDate === undefined || successFunction === undefined || endDate < beginDate) return;

    let request = {
        url: samplesApi + "/plug",
        method: "POST",
        headers: { "Authorization": "bearer " + sessionStorage.getItem(tokenStorageKeyString) },
        success: function (data, textStatus, jqXHR) {
            for (let i in data) {
                data[i].sampleDate = new Date(data[i].sampleDate);
            }
            successFunction(data, textStatus, jqXHR);
        },
        contentType: "application/json",
        data: JSON.stringify({
            mac: mac,
            earlierDate: correctToJSON(beginDate),
            laterDate: correctToJSON(endDate)
        })
    };

    if (errorFunction !== undefined) request.error = errorFunction;
    if (completeFunction !== undefined) request.complete = completeFunction;

    $.ajax(request);
}

/**
 * Gets an array of task JSONs of all tasks of a given plug, containing their taskId, description, operation, deviceMac, taskType, repeatEvery and startDate.
 * @param {string}      mac                 The plug's (whose tasks are requested) mac address.
 * @param {function}    successFunction     Function to execute upon success.
 * @param {function=}   errorFunction       Function to execute upon failure.
 * @param {function=}   completeFunction    Function to execute upon completion.
 */
function getPlugTasks(mac, successFunction, errorFunction, completeFunction) {
    if (mac === undefined || successFunction === undefined) return;

    let request = {
        url: tasksApi + "/plug/" + mac,
        method: "GET",
        headers: { "Authorization": "bearer " + sessionStorage.getItem(tokenStorageKeyString) },
        success: function (data, textStatus, jqXHR) {
            for (let i in data) {
                data[i].startDate = new Date(data[i].startDate);
            }
            successFunction(data, textStatus, jqXHR);
        }
    };

    if (errorFunction !== undefined) request.error = errorFunction;
    if (completeFunction !== undefined) request.complete = completeFunction;

    $.ajax(request);
}


/**
 * Adds a new task. Task is an object with the following properties: taskId, description, operation, deviceMac, taskType, repeatEvery and startDate.
 * @param {object}      task                The new task object.
 * @param {function=}   successFunction     Function to execute upon success.
 * @param {function=}   errorFunction       Function to execute upon failure.
 * @param {function=}   completeFunction    Function to execute upon completion.
 * @example
 * // a task that turns a plug on every day
 * let newTask = {
 *     operation: Operations.TurnOn,
 *     deviceMac: "BC:DD:C2:23:D6:69",
 *     taskType: TaskTypes.Repeated,
 *     description: "Turn on every day",
 *     repeatEvery: 60 * 24,
 *     startDate: new Date()
 * }
 * 
 * // a task that will turn a plug off in 5 minutes
 * let d = new Date();
 * d.setMinutes(d.getMinutes() + 5);
 * let newTask = {
 *     operation: Operations.TurnOff,
 *     deviceMac: "BC:DD:C2:28:D6:60",
 *     description: "Turn off in 5 minutes",
 *     taskType: TaskTypes.OneTime,
 *     startDate: d
 * }
 * 
 * addTask(newTask);
 */
function addTask(task, successFunction, errorFunction, completeFunction) {
    if (task === undefined) return;

    let taskToSend = {};
    Object.assign(taskToSend, task);
    taskToSend.startDate = correctToJSON(taskToSend.startDate);

    let request = {
        url: tasksApi,
        method: "POST",
        headers: { "Authorization": "bearer " + sessionStorage.getItem(tokenStorageKeyString) },
        contentType: "application/json",
        data: JSON.stringify(taskToSend),
        success: function (data, textStatus, jqXHR) {
            task.taskId = data.taskId;
            if (successFunction !== undefined) successFunction(data, textStatus, jqXHR);
        }
    };

    if (errorFunction !== undefined) request.error = errorFunction;
    if (completeFunction !== undefined) request.complete = completeFunction;

    $.ajax(request);
}

/**
 * Deletes a task.
 * @param {object}      task                The task object to delete. The task object must contain the task id received by the server.
 * @param {function=}   successFunction     Function to execute upon success.
 * @param {function=}   errorFunction       Function to execute upon failure.
 * @param {function=}   completeFunction    Function to execute upon completion.
 */
function removeTask(task, successFunction, errorFunction, completeFunction) {
    if (task === undefined) return;

    let request = {
        url: tasksApi + "/" + task.taskId,
        method: "DELETE",
        headers: { "Authorization": "bearer " + sessionStorage.getItem(tokenStorageKeyString) },
    };

    if (successFunction !== undefined) request.success = successFunction;
    if (errorFunction !== undefined) request.error = errorFunction;
    if (completeFunction !== undefined) request.complete = completeFunction;

    $.ajax(request);
}

/** Calculates a correct dateString including time zone.
 * 
 * Private function for date calculation, taken from: https://stackoverflow.com/a/36643588/7414734.
 * @access private
 * @param       {Date}      date    A date object to convert to a dateString with time zone.
 * @returns     {string}            A dateString containg all information about the given date including time zone.
 * */
function correctToJSON(date) {
    var timezoneOffsetInHours = -(date.getTimezoneOffset() / 60); //UTC minus local time
    var sign = timezoneOffsetInHours >= 0 ? '+' : '-';
    var leadingZero = Math.abs(timezoneOffsetInHours) < 10 ? '0' : '';

    //It's a bit unfortunate that we need to construct a new Date instance 
    //(we don't want _this_ Date instance to be modified)
    var correctedDate = new Date(date.getFullYear(), date.getMonth(),
        date.getDate(), date.getHours(), date.getMinutes(), date.getSeconds(),
        date.getMilliseconds());
    correctedDate.setHours(date.getHours() + timezoneOffsetInHours);
    var iso = correctedDate.toISOString().replace('Z', '');

    return iso + sign + leadingZero + Math.abs(timezoneOffsetInHours).toString() + ':00';
}