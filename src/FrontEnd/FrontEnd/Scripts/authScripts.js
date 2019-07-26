// requires jQuery
let server = "http://localhost:56460/api";
let usersApi = server + "/users";
let plugsApi = server + "/plugs";
let samplesApi = server + "/powerusagesamples";
let tasksApi = server + "/tasks";
let authApi = server + "/auth";

function loginUser(loginModel, successFunction, errorFunction, completeFunction) {
    if (loginModel.username === undefined || loginModel.password === undefined || successFunction === undefined) return;

    let request = {
        contentType: "application/json",
        url: authApi + "/login",
        method: "POST",
        data: JSON.stringify(loginModel),
        success: successFunction
    };

    if (errorFunction !== undefined) request.error = errorFunction;
    if (completeFunction !== undefined) request.complete = completeFunction;

    $.ajax(request);
}

function registerUser(registerModel, successFunction, errorFunction, completeFunction) {
    if (registerModel.username === undefined || registerModel.password === undefined || successFunction === undefined) return;

    let request = {
        contentType: "application/json",
        url: authApi + "/register",
        method: "POST",
        data: JSON.stringify(registerModel),
        success: successFunction
    };

    if (errorFunction !== undefined) request.error = errorFunction;
    if (completeFunction !== undefined) request.complete = completeFunction;

    $.ajax(request);
}

function getUser(username, token, successFunction, errorFunction, completeFunction) {
    if (username === undefined || successFunction === undefined) return;

    let request = {
        url: usersApi + "/" + username,
        method: "GET",
        headers: { "Authorization": "bearer " +  token},
        success: successFunction
    };

    if (errorFunction !== undefined) request.error = errorFunction;
    if (completeFunction !== undefined) request.complete = completeFunction;

    $.ajax(request);
}