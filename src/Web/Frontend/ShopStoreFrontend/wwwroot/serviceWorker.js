importScripts('/js/lib/microsoft/signalr/dist/browser/signalr.js');

var serverHub;
var messageChannel;

//安裝階段時觸發
self.addEventListener('install', function (event) {
    console.log('[Service Worker] Installing Service Worker ...', event);
});

//激活階段時觸發
self.addEventListener('activate', function (event) {
    console.log('[Service Worker] Activating Service Worker ...', event);
    return self.clients.claim(); //確保被載入&激活
})

//監聽 fetch 
//self.addEventListener('fetch', function (event) {
//    console.log('[Service Worker] Fetch somthing ...', event);
//    //event.respondwith 攔截外部請求
//    event.respondWith(fetch(event.request));
//})

//監聽Main Thread的訊息
self.addEventListener('message', function (event) {
    var data = event.data;

    if (data.command == 'CreateServerHub') {
        CreateServerHub(data.url);
        messageChannel = event.ports;

    }

    if (data.command == 'StopServerHub') {
        StopServerHub();
    }
})

//建立長連接
function CreateServerHub(url) {

    if (serverHub != undefined && serverHub.state == 'Connected') {           
        return;
    }

    serverHub = new signalR.HubConnectionBuilder()
        .withUrl(url)
        .build();
    
    serverHub.start().then(() => {        

    }).catch((res) => {
        console.error('serverHub 連接錯誤');
        console.log(res);
    })

    //接聽來自Server端的通知
    serverHub.on('SendMessageToFrontedUser', function (orderId, stateMsg) {
        SendMsgToPage(orderId, stateMsg);
    })

}

//SW接收到訊息後，透過 messageChannel 將訊息回傳至Main thread
function SendMsgToPage(orderId, stateMsg) {
    messageChannel[0].postMessage(
        {
            'type': 'UserAlert',
            'orderId': orderId,
            'stateMsg': stateMsg
        }
    )
}

//關閉連接
function StopServerHub() {

    if (serverHub != undefined && serverHub.state == 'Connected') {
        serverHub.stop();
        console.log('serverHub 已斷開')
    }

}