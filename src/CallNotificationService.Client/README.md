# Call Notification Service Client

This client library, together with a deployed instance of the Call Notification Service (CNS), helps developers integrate with the Azure Communication Services (ACS) Call Automation platform. Specifically, the client performs a registration against CNS providing the ACS identity and webhook callback URI so they can be notified of incoming calls.

The ACS platform uses Event Grid to deliver the IncomingCall event payload which contains a critical property used to answer, reject, or redirect a call. These calls can originate from the PSTN or other ACS identities/endpoints such as the client calling SDK or other Call Automation identities. A common Event Grid subscription type used by developers of the Call Automation platform is a webhook and due to the unique requirements of this subscription type, it can be cumbersome for developers to continually update their subscriptions when their public FQDN changes. Additionally, it is common for developers to maintain strict event filters to ensure only IncomingCall events for certain phone numbers or ACS identities are sent to their application.

The the Call Notification Service and Client eliminate the need to continually update the Event Grid subscription and removes the need to define filters altogether.

## Architecture overview

The Call Notification Client communicates with CNS using an HTTP client on a configurable interval using a .NET background worker process. When an IncomingCall event is sent from ACS to your resource's Event Grid subscription, CNS will dispatch a `CallNotification` payload to the webhook your application provided. Additionally, since the act of registration also includes a `Targets` collection, your local application has full control over which identities (PSTN numbers or ACS identities) it receives notifications for.

![Copy Function UR>](https://github.com/jasonshave/CallNotificationService/raw/master/images/cns-overview.png)

## Pre-requisites

- You must have an existing deployment of the Call Notification Service Azure Function in your tenant.
- Obtain the function URL's for the SetRegistration, DeRegister, GetRegistration, and ListRegistrations functions.

## Configuration

After installing the NuGet package, configure your .NET User Secrets file, `appsettings.json`, or any configuration provider with the following JSON properties which allow the client to communicate with the CNS Function App:

```json
"CallNotificationClientSettings": {
    "SetRegistrationEndpointUri": "<insert-register-function-url>",
    "DeRegisterEndpointUri": "<insert-deregister-function-url>",
    "GetRegistrationEndpointUri": "<insert-getregistration-function-url>",
    "ListRegistrationsEndpointUri": "<insert-listregistrations-function-url>",
    "ApplicationId": "<insert-your-acs-id>",
    "RegistrationLifetimeInMinutes": 60,
    "Targets": [ "4:+18005551212", "4:+14255555000"],
    "CallNotificationPath": "api/callNotification"
  }
```

## CallNotificationClientSettings explained

| Setting | Purpose | Example |
| -- | -- | -- |
| RegistrationEndpointUri | Used to set the registration for your application with CNS. | https://myfunction.azurewebsites.net |
| DeRegisterEndpointUri | Used to deregister your application with CNS. | https://myfunction.azurewebsites.net |
| GetRegistrationEndpointUri | Used to get the registration for your application with CNS. | https://myfunction.azurewebsites.net |
| ListRegistrationsEndpointUri | Used to list the registrations with CNS. | https://myfunction.azurewebsites.net |
| ApplicationId | This can be obtained using the ACS Identity blade in the Azure portal, you can use the ACS Identity SDK, or leave it blank in which the Call Notification Service will automatically generate an ID for you. | `8:acs:63454d3e-3fd6-4e9a-817b-e80314a2b271_066fa785-71dd-4201-91fd-cb4f59c9aa7f` |
| RegistrationLifetimeInMinutes | Used by the built-in worker process to re-register on this interval | `60` |
| Targets | A `List<string>` containing the ACS `rawId` the Call Notification Service should send you notifications for. This can be a number or an ACS identity, similar to the application ID mentioned above. | `{ "4:+18005551212", "4:+18669876543", configuration["ACS:ApplicationId"] }`
| CallNotificationPath | The path used to receive the `CallNotification` payload. | `api/CallNotification` |
