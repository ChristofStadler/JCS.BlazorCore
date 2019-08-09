var App = (function () {
    function App() {
        this.Ping();
    }
    App.prototype.Ping = function () {
        console.log("pong");
    };
    App.prototype.SetUID = function (uid) {
        localStorage.setItem("UID", uid);
    };
    App.prototype.GetUID = function () {
        return localStorage.getItem("UID");
    };
    return App;
}());
var app = new App();
//# sourceMappingURL=app.js.map