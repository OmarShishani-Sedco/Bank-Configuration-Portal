(function () {
    // Works at site root or under a virtual directory, e.g. /MyApp
    var m = location.pathname.match(/^(.*)\/swagger\/ui\/.*/);
    var base = m ? m[1] : '';
    var specUrl = base + '/swagger/docs/v1';

    window.ui = SwaggerUIBundle({
        url: specUrl,
        dom_id: '#swagger-ui',
        deepLinking: true,
        layout: 'StandaloneLayout',
        presets: [SwaggerUIBundle.presets.apis, SwaggerUIStandalonePreset],
        docExpansion: 'list',
        defaultModelsExpandDepth: -1,
        syntaxHighlight: { activate: false },
        displayRequestDuration: true,
        persistAuthorization: true,
        validatorUrl: null,
        // Auto-prefix "Bearer " if the user pasted just the token
        requestInterceptor: function (req) {
            try {
                var auth = ui.authSelectors.authorized().toJS().Bearer;
                if (auth && auth.value && !/^Bearer\s/i.test(auth.value)) {
                    req.headers.Authorization = 'Bearer ' + auth.value;
                }
            } catch (e) { }
            return req;
        }
    });
})();
