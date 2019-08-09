class App {
    constructor() {
        this.Ping()
    }

    Ping() {
        console.log("pong");
    }

    SetUID(uid: string) {
        localStorage.setItem("UID", uid);
    }

    GetUID() {
        return localStorage.getItem("UID");
    }
}

let app = new App();