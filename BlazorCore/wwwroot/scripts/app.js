var App = {
    Init: function() {
        //this.Ping();
    },
    Ping: function () {
        console.log("pong");
    },
    LocalStorageSet: function (item, uid) {
        localStorage.setItem(item, uid);
    },
    LocalStorageGet: function (item) {
        return localStorage.getItem(item);
    },
    RenderUI: function () {
        console.log("RenderUI Started");
        setInterval(function () {
            DotNet.invokeMethodAsync('BlazorCore','Render').then(r => console.log(r));
        }, 1000);
    },
    InputListener: function () {
        var uid = App.LocalStorageGet("UID");
        var session = App.LocalStorageGet("Session");

        // Text Keys
        document.onkeypress = function (e) {
            e = e || window.event;
            var charCode = e.keyCode || e.which;
            var charStr = String.fromCharCode(charCode);
            DotNet.invokeMethodAsync('BlazorCore', 'KeyPressed', charStr, uid, session).then(r => console.log(r));
            return;
        };

        // Arrow Keys
        document.onkeydown = function (e) {
            var charStr = "";
            switch (e.keyCode) {
                case 37:
                    charStr = 'left';
                    e.view.event.preventDefault();
                    break;
                case 38:
                    charStr = 'up';
                    e.view.event.preventDefault();
                    break;
                case 39:
                    charStr = 'right';
                    e.view.event.preventDefault();
                    break;
                case 40:
                    charStr = 'down';
                    e.view.event.preventDefault();
                    break;
            }
            DotNet.invokeMethodAsync('BlazorCore', 'KeyPressed', charStr, uid, session).then(r => console.log(r));
            return;
        };
    }
};
App.Init();