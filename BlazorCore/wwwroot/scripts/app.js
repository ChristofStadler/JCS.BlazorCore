var App = {
    Init: function() {
        this.Ping();
        this.InputListener();
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
        document.onkeypress = function (evt) {
            evt = evt || window.event;
            var charCode = evt.keyCode || evt.which;
            var charStr = String.fromCharCode(charCode);
            var uid = App.LocalStorageGet("UID");
            var session = App.LocalStorageGet("Session");
            DotNet.invokeMethodAsync('BlazorCore', 'KeyPressed', charStr, uid, session).then(r => console.log(r));
        };
    }
};
App.Init();