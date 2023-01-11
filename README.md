# Call Notification Service

[![.NET](https://github.com/jasonshave/CallNotificationService/actions/workflows/dotnet.yml/badge.svg)](https://github.com/jasonshave/CallNotificationService/actions/workflows/dotnet.yml)

This library contains the deployable Call Notification Service (CNS) and a client library which can manage webhook callback registrations and dispatch inbound call notifications from the Azure Communication Services (ACS) Call Automation platform. CNS builds on how the Call Automation platform sends inbound call notifications to Event Grid by allowing dynamic registration of your webhook endpoint and the phone numbers or ACS endpoints at runtime.

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Fjasonshave%2FCallNotificationService%2Fjasonshave%2Fpackage-updates%2Fdeployment%2Fdeploy-to-azure-bicep.bicep%3Ftoken%3DGHSAT0AAAAAAB27NMTHVT2LE6LEJYW7JGMMY3QJ2QA)

## Features and benefits

- Set your Event Grid subscription once without the need to change it when developing alone or with teams if your workstation's proxy endpoint changes (i.e. when using NGROK).
- Provides configurable JWT bearer token in header of `CallNotification` payload to your webhook endpoint containing `aud` claim matching the application ID used during registration.
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
2. When an `IncomingCall` event is sent to Event Grid from ACS, the following steps are performed:

   - Lookup callback registrations based on the "to" field of the call.
   - For each registration, transform the `IncomingCall` to a `CallNotification` type making sure to obey the flag to send the `IncomingCallContext` or not.
     - Save the notification to Cosmos DB with an automatic expiration of the data (configurable).
     - Send one notification to each host registered using the webhook callback URI constructed by setting the `CallbackHost` and `CallNotificationPath` in the client library.
     - If an error occurs, retry 3 times with a 600ms delay (configurable).

   ![CNS Overview](/images/cns-overview.png)

## Deployment and configuration

Once the Call Notification Service has been deployed, configure the following settings in the Azure Function's Configuration blade in the Azure portal.

| Setting | Purpose |
| -- | -- |
| CosmosDbConfiguration:ConnectionString | Used to connect to Cosmos DB from the Call Notification Service |
| CosmosDbConfiguration:Database | A configurable name used to identify the database containing the containers used by the application |
| CosmosDbConfiguration:Tables | A key/value pair of container name and default time to live (in seconds) for documents written to the container |
| ACS:ConnectionString | Used by the CNS Answer API for handling scenarios where the `IncomingCallContext` isn't sent in the `CallNotification` payload |
| NotificationSettings:EnableSendIncomingCallContext | A flag used to determine if the `IncomingCallContext` property is populated in the `CallNotification` payload |
| NotificationSettings:TimeToLiveInSeconds | The amount of time the `CallNotification` document can reside in the `CallNotifications` table before being automatically removed |
| TokenConfiguration:Secret | The secret used to generate the Json Web Token (JWT) in the header sent with the `CallNotification` payload. |
| TokenConfiguration:TimeToLiveInMinutes | Sets the JWT `exp` claim. |
| TokenConfiguration:Issuer | Sets the JWT `iss` claim. |

### Example Call Notification Service configuration

```json
{
  "CosmosDbConfiguration": {
    "ConnectionString": "[cosmos-db-connection-string]",
    "Database": "CallNotificationService",
    "Tables": {
      "CallbackRegistrations": 86400,
      "CallNotifications": 35
    }
  },
  "ACS": {
    "ConnectionString": "[acs-connection-string]"
  },
  "NotificationSettings": {
    "EnableSendIncomingCallContext": true,
    "TimeToLiveInSeconds": 30
  },
  "TokenConfiguration": {
    "Secret": "[your-token]",
    "TimeToLiveInMinutes": 5,
    "Issuer": "[your-fqdn]"
  }
}
```

## CallNotification schema

| Property | Purpose | Example |
| -- | -- | -- |
| Id | Identifies the unique `CallNotification` payload | `5ffba0c8-f9ee-4a01-ad64-e2ca80930ec4` |
| From | String value representing the caller | `4:+18005551212` |
| To | String representing the called target | `4:+18669876543` or `8:acs:d8f7dce6-b60a-48d8-aa2e-c317b20b3fe9_704702a4-3651-4f08-9e60-7f136ac614b4` |
| CallerDisplayName | Nullable value showing the caller's name | Contoso |
| ApplicationId | The application ID used during registration | `8:acs:d8f7dce6-b60a-48d8-aa2e-c317b20b3fe9_704702a4-3651-4f08-9e60-7f136ac614b4` |
| CorrelationId | The ID used correlate all call events in Call Automation | 02dc8864-97de-4f53-9e97-132ce29f5bfa |
| IncomingCallContext | Nullable value for the call context used to answer, reject, or redirect a call | Note: excluded due to length |
| MidCallEventsUri | The full URI specified during registration to receive mid-call event callbacks. Helpful to reduce stateful registration requirement. | `https://myserver.com/api/callbacks` |

## Call Notification Service Client

The Call Notification Service Client library can be used like an SDK to provide a convenience layer when interacting with your deployed Call Notification Service instance. Additionally, this library is best used with the [CallAutomation.Extensions package on NuGet](https://www.nuget.org/packages/CallAutomation.Extensions/) making it easier to invoke actions and correlate callbacks to previous actions.

### Client configuration

The Call Notification Service uses an Azure Function with **Function code** authorization which means you need to obtain the function URL for each API. To do this, navigate to your deployed function, click on at least the `Register` function to obtain the URL.

![Get Function](/images/function-url-1.png)

Then copy the function URL and save it to be used in the client configuration settings.
![Copy Function UR>](/images/function-url-2.png)

#### CallNotificationClientSettings

```json
  "CallNotificationClientSettings": {
    "SetRegistrationEndpointUri": "[register-function-url]",
    "DeRegisterEndpointUri": "[deregister-function-url]",
    "GetRegistrationEndpointUri": "[getregistration-function-url]",
    "ListRegistrationsEndpointUri": "[listregistrations-function-url]"
  },
```

### Callback registration

Usually during startup, your application will register with CNS and supply the targets it wants to subscribe to. For example, you may identify your application using an [ACS acquired identity](https://learn.microsoft.com/azure/communication-services/concepts/identity-model) and a phone number you've purchased through the Azure portal. In this case you want your application to be notified when an inbound call arrives for either the application identity or the phone number so it can take action such as answering, rejecting, or redirecting the call.

```csharp
var settings = new CallbackRegistrationSettings()
{
    ApplicationId = configuration["ACS:ApplicationId"],
    CallbackHost = configuration["VS_TUNNEL_URL"],
    CallNotificationPath = "api/callNotification",
    MidCallEventsPath = "api/callbacks",
    LifetimeInMinutes = 30,
    Targets = { "4:+18005551212", "4:+18669876543" }
};
CallbackRegistration registration = await client.SetRegistrationAsync(settings);
```

The example above shows the `ApplicationId` setting coming from the `configuration` variable which is an implementation of `IConfiguration` likely injected in the constructor. Lastly, it shows the registration will last for 30 minutes before expiring and deliver `CallNotification` payloads for two PSTN phone numbers.

| Setting | Purpose | Example |
| -- | -- | -- |
| ApplicationId | This can be obtained using the ACS Identity blade in the Azure portal, you can use the ACS Identity SDK, or leave it blank in which the Call Notification Service will automatically generate an ID for you. | `8:acs:63454d3e-3fd6-4e9a-817b-e80314a2b271_066fa785-71dd-4201-91fd-cb4f59c9aa7f` |
| CallbackHost | This is your public FQDN representing your web application managing calls using the Call Automation SDK. | https://myserver.com |
| CallNotificationPath | The path used to receive the `CallNotification` payload. | `api/CallNotification` |
| MidCallEventsPath | The path used to receive `CloudEvent[]` envelopes containing the multitude of events from the Call Automation platform. | `api/callbacks` |
| LifetimeInMinutes | A `double` representing the number of minutes before your registration automatically expires. | `30` |
| Targets | A `List<string>` containing the ACS `rawId` the Call Notification Service should send you notifications for. This can be a number or an ACS identity, similar to the application ID mentioned above. | `{ "4:+18005551212", "4:+18669876543", configuration["ACS:ApplicationId"] }`

### Dynamic configuration

Leveraging the `IOptionsMonitor<T>` interface in [ASP.NET](https://learn.microsoft.com/aspnet/core/fundamentals/configuration/options?view=aspnetcore-7.0), you can store your configuration parameters in either the [User Secrets store in .NET](https://learn.microsoft.com/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=windows) or load them from another configuration provider. You also choose to split the configuration so that the host and path information comes from one provider, and the number management configuration from an external service responsible for that task.

```csharp
public class RegistrationWorker : BackgroundService
{
    // use the options monitor to load settings dynamically
    private readonly IOptionsMonitor<CallbackRegistrationSettings> _settings;
    private readonly ICallNotificationClient _client;

    private CallbackRegistration? _registration;

    public RegistrationWorker(
        IOptionsMonitor<CallbackRegistrationSettings> settings,
        ICallNotificationClient client)
    {
        _settings = settings;
        _client = client;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // use dynamic settings from IOptionsMonitor<CallbackRegistrationSettings>
            _registration = await _client.SetRegistrationAsync(_settings.CurrentValue);

            // pause then renew.
            await Task.Delay(TimeSpan.FromMinutes(_settings.CurrentValue.LifetimeInMinutes / 1.2), stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        // optional: remove registration upon graceful shutdown
        await _client.DeRegister(_registration.ApplicationId);
        await base.StopAsync(cancellationToken);
    }
}
```

## Visual Studio Dev Tunnels

Th [Dev Tunnels feature](https://learn.microsoft.com/connectors/custom-connectors/port-tunneling) released in Visual Studio 17.4.0 which allows for a dynamic public endpoint FQDN for easier development of web endpoints. The tunnel FQDN is also available at runtime through an [Environment Variable](https://devblogs.microsoft.com/visualstudio/introducing-private-preview-port-tunneling-visual-studio-for-asp-net-core-projects/#environment-variable-to-get-the-dev-tunnel-url) which allows you to remove any reference to a fixed public domain name in your application.

The primary benefit to using this feature is that you don't need 3rd party tools like NGROK to do rapid development and testing on your local machine. This feature, combined with the Call Notification Service's ability to dynamically register your webhook callback means you don't have to worry about changing anything in ACS or Event Grid.

>NOTE: From the **Tools, Options** menu in Visual Studio, search for `Tunnel` and locate the **Dev Tunnels, General** section. You may need to sign in with an Outlook or corporate account for this feature to work during public preview in addition to enabling the feature in the **Preview features** section.

### Loading the hostname dynamically

```csharp
public class Sample
{
    public Sample(
        IConfiguration configuration,
        IOptionsMonitor<CallbackRegistrationSettings> settings)
    {
        settings.CurrentValue.CallbackHost = configuration["VS_TUNNEL_URL"] ?? settings.CurrentValue.CallbackHost;
    }
}
```

> NOTE: The environment variable obtained from the `VS_TUNNEL_URL` tag contains a trailing slash `/`. In the above example, be sure to handle the case where your `CallbackHost` has been configured in your application's settings rather than from the environment variable and adjust your callback paths to remove the `/` and add it to the host name otherwise you will have an invalid double slash: `https://myserver.com//api/callbacks` path.
