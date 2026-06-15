(function () {
    function authorizeSwagger(token) {
        const tryAuthorize = () => {
            if (!window.ui?.authActions?.authorize) return false;
            window.ui.authActions.authorize({
                Bearer: {
                    name: "Bearer",
                    schema: { type: "http", scheme: "bearer", bearerFormat: "JWT" },
                    value: token
                }
            });
            return true;
        };

        let attempts = 0;
        const interval = setInterval(() => {
            if (tryAuthorize() || ++attempts > 20) clearInterval(interval);
        }, 300);
    }

    function handleLoginResponse(url, responseText) {
        if (!url || !url.includes("/api/auth/login")) return;
        try {
            const data = JSON.parse(responseText);
            const token = data?.data?.token;
            if (token) {
                localStorage.setItem("riven_jwt", token);
                authorizeSwagger(token);
            }
        } catch (_) { /* ignore */ }
    }

    const originalSend = XMLHttpRequest.prototype.send;
    XMLHttpRequest.prototype.send = function () {
        this.addEventListener("load", function () {
            handleLoginResponse(this._rivenUrl, this.responseText);
        });
        return originalSend.apply(this, arguments);
    };

    const originalOpen = XMLHttpRequest.prototype.open;
    XMLHttpRequest.prototype.open = function (method, url) {
        this._rivenUrl = url;
        return originalOpen.apply(this, arguments);
    };

    const saved = localStorage.getItem("riven_jwt");
    if (saved) authorizeSwagger(saved);
})();
