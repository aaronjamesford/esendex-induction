Esendex Induction Application
=================
This is a small MVC app for induction task at Esendex. The demo is available [here](http://ajf-esendex.azurewebsites.net)

To enable push notifications, log into echo and configure these endpoints respectively

```
http://ajf-esendex.azurewebsites.net/api/InboundMessage
http://ajf-esendex.azurewebsites.net/api/MessageDelivered
http://ajf-esendex.azurewebsites.net/api/MessageFailed
```

To enable the legacy Account EventHandler notifications, log into Echo and configure this endpoint for the latest FormPost push notifications.

```
http://ajf-esendex.azurewebsites.net/api/FormPostPushNotification
```
