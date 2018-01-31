# Lykke.Service.AssetDisclaimers

Lykke entities and assets disclaimers service

Client: [Nuget](https://www.nuget.org/packages/Lykke.Service.AssetDisclaimers.Client/)

# Client usage

Register client services in container:

```cs
ContainerBuilder builder;
...
var settings = new AssetDisclaimersServiceClientSettings("http://<service>:[port]/");
builder.RegisterInstance(new AssetDisclaimersClient(settings))
    .As<IAssetDisclaimersClient>()
    .SingleInstance();
```

Now you can use:

* IAssetDisclaimersClient - HTTP client for service API