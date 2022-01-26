importScripts('/js/lib/microsoft/signalr/dist/browser/signalr.js');

var serverHub = null;

onmessage = function (e) {
    
    if (serverHub === null) {
        serverHub = new signalR.HubConnectionBuilder()
            .withUrl('http://localhost:6372/ServerHub')
            .build();
    }

    serverHub.start().then(() => {
        postMessage('serverHub connected');
    })
}


//function CreateConn(serverHub) {
    


//    serverHub.start().then(function () {
//        console.log('--- Server Connection Start ---');
//    }).catch(function (err) {
//        console.error('--- fail ---')
//    })
//}