# Call Notification Service

This library contains the deployable Call Notification Service (CNS) and a client library which can manage webhook callback registrations and dispatch inbound call notifications from the Azure Communication Services (ACS) Call Automation platform. CNS builds on how the Call Automation platform sends inbound call notifications to Event Grid by allowing dynamic registration of your webhook endpoint and the phone numbers or ACS endpoints at runtime.

## Use cases and benefits

- Set your Event Grid subscription once without the need to change it when developing alone or with teams if your workstation's proxy endpoint changes (i.e. when using NGROK).
- Improves time to value when starting development with the platform by allowing the developer to focus on their business problem versus setting up infrastructure.
- Removes the need for the answering endpoint to know the mid-call event callback URI as this can be provided during registration.
- Less complexity than using Event Grid webhooks which need continual maintenance and validation of the proxy endpoint changes.
- Allows you to manage listening targets/phone numbers in your local application by subscribing on startup.
- Offers fine-grained webhook retry and error handling logic using ASP.NET and Polly libraries.
- Automatically creates an ACS identity if you don't specify one at the time of registration.
- Configure 'fan-out' event delivery behavior for scenarios where multiple registrations for the same target/number is needed.
- Optionally configure a mobile push notification friendly `CallNotification` payload by omitting the `IncomingCallContext` property. This can be combined with the CNS offering an "answer" endpoint where the `IncomingCallContext` data is saved in Cosmos DB so it can be retrieved on answer.

## How does it work?

1. Using the Call Notification Service Client, register your webhook callback on startup. This saves your registration to Cosmos DB and sets the document to expire based on the time frame chosen during registration.
2. When an `IncomingCall` event is sent to Event Grid from ACS the following steps are performed:

   - Lookup callback registrations based on the "to" field of the call.
   - For each registration, transform the `IncomingCall` to a `CallNotification` type making sure to obey the flag to send the `IncomingCallContext` or not.
     - Save the notification to Cosmos DB with an automatic expiration of the data (configurable).
     - Send one notification to each registration using the webhook callback URI.
     - If an error occurs, retry 3 times with a 600ms time window between each request (configurable).

   ![CNS Overview](/images/cns-overview.png)

## Callback registration

Usually during startup, your application will register with CNS and supply the targets it wants to subscribe to. For example, you may identify your application using an [ACS acquired identity](https://learn.microsoft.com/en-us/azure/communication-services/concepts/identity-model) and a phone number you've purchased through the Azure portal. In this case you want your application to be notified when an inbound call arrives for either the application identity or the phone number so it can take action such as answering, rejecting, or redirecting  the call.

```csharp
var settings = new CallbackRegistrationSettings()
{
    ApplicationId = _configuration["ACS:ApplicationId"],
    CallbackHost = _configuration["VS_TUNNEL_URL"],
    CallNotificationPath = "/api/callNotification",
    MidCallEventsPath = "/api/callbacks",
    LifetimeInMinutes = 30,
    RegisteredTargets = { "4:+18005551212", "4:+18669876543" }
};
CallbackRegistration registration = await client.SetRegistrationAsync(settings);
```

## Call Notification Service Configuration

## Call Notification Client
