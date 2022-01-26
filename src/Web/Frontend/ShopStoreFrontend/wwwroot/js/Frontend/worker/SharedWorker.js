self.addEventListener('connect', function (event) {
    var port = event.ports[0];
    port.onmessage = function (event) {
        console.log(event.data.type);
        setInterval(function () {

            port.postMessage({'type':'CheckAlive'});

        }, 9000)
    }
})