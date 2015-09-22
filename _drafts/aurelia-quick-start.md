Create new project using http://www.oliver-lohmann.me/a-very-basic-asp-net-web-api-single-page-application-template/

Navigate to folder and run `jspm init`

`jspm install aurelia-framework`
`jspm install aurelia-bootstrapper`

Prepare the index.html file...

```
<!DOCTYPE html>
<html>
<head>
    <title>My Sample App</title>
    <meta name="viewport" content="wdith=device-width, initial-scale=1">
</head>
<body aurelia-app>
    <script src="jspm_packages/system.js"></script>
    <script src="config.js"></script>
    <script>
        System.import("aurelia-bootstrapper").catch(console.error.bind(console));
    </script>
</body>
</html>
```

Add app.html
```
<template>
    <h1>Hello, world! Aurelia here.</h1>
</template>
```

Add app.js
```
export class App {
}
```

Add in Google platform dependencies to index.html
```
<script src="https://apis.google.com/js/platform.js" async defer></script>
<meta name="google-signin-client_id" content="YOUR_CLIENT_ID.apps.googleusercontent.com">
```

It would be nice to load this via System.js but that turns out to be problematic. Google doesn't enable CORS on the platform.js file. The standard System.js loader use xhr to load the script and this fails due to the same origin policy. We could probably work around this by creating a loader plugin to inject our script tag into the header but 

Add login div to app.html
```
<div class="g-signin2"></div>
```

gapi.auth2.init({
            client_id: '686248902900b-djmru94fql3enuqumju3itpn7v00uhq.apps.googleusercontent.com'
        });


        gapi.signin2.render('g-signin2');

return new Promise(() => {
            gapi.load('auth2', function() {
                gapi.auth2.init({
                    client_id: '686248902900-bdjmru94fql3enuqumju3itpn7v00uhq.apps.googleusercontent.com'
                });
            });
        });




jspm install aurelia-templating-resources