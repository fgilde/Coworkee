"use strict";
var version = "v2/";
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/eventHub")
    .withAutomaticReconnect()
    .build();


var loadEntities = function() {
    var list = document.getElementById("entityList");
    list.innerHTML = '';
    return fetch('/api/' + version +'myentity/index').then(function (res) {
        return res.json().then(function (array) {
            array.forEach(function (e) {
                var li = document.createElement("li");
                li.textContent = e.name + '  ( ' + e.id + ' )';
                list.appendChild(li);
            });
        });

    });
};

//loadEntities().then(function() {
//    connection.on("EventRecieved", function (event) {
//        var li = document.createElement("li");
//        li.textContent = 'Received ' + event.eventName + ' with following arguments ' + JSON.stringify(event.arguments);
//        if (event.eventName.toLowerCase().includes('entity')) {
//            loadEntities();
//        }
//        document.getElementById("eventList").appendChild(li);
//    });
//});

connection.on("EventRecieved", function (event) {
    var li = document.createElement("li");
    li.textContent = 'Received ' + event.eventName + ' with following arguments ' + JSON.stringify(event.arguments);
    if (event.eventName.toLowerCase().includes('entity')) {
        loadEntities();
    }
    document.getElementById("eventList").appendChild(li);
});

connection.start().then(() => document.getElementById('status').innerHTML = 'Connected !').catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    fetch('/api/' + version +'myentity/create/' + (Math.random().toString(36).substring(6))).then(function(res) {
        console.log(res);
    });

    event.preventDefault();
});

document.getElementById("serviceBus").addEventListener("click", function (event) {
    var data = {
        id: 1,
        name: 'test'
    };
    fetch('/api/' + version +'system/SendOnServiceBus/TestQueue', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data) // body data type must match "Content-Type" header
    });
    
    event.preventDefault();
});