# Customizations

Customizability/Extensibility is one the main design goals of the engine, and as such it was written from the bottom up so that everything (or, almost everything) can be replaced.
It's doing that by harnessing [Autofac](https://autofac.org/), a library which enables you to create an [inversion-of-control container](https://en.wikipedia.org/wiki/Inversion_of_control) to setup how to inject dependencies into classes.
This might sound complicated, but in practice it's very simple to use:

If you want to change how the engine behaves, find the interface that defines the behavior you want to change, implement it yourself, then set up the `Resolver` to use that implementation instead of the engine built-in implementation. From that point on, your implementation will "magically flow" to all the code that requires it.

Let's, for example, try to change where speech is rendered. As a simple example, let's always render the speech at the same position.
First we'll look at an appropriate interface: in this case, we'll want to implement `ISayLocationProvider`. 
This has one method which we need to implement which gets the text and speech configuration and needs to return a say location (text location and portrait location).

```csharp

public class MyCustomSayLocationProvider : ISayLocationProvider
{
    public ISayLocation GetLocation(string text, ISayConfig config)
    {
        PointF textLocation = new PointF(400f,100f);
        PointF portraitLocation = new PointF(100f, 100f);
        return new AGSSayLocation(textLocation, portraitLocation);
    }
}

```

The text and the portrait will always be rendered in the same place regardless of the game's resolution, where the character stands, the font size, etc, making this a useless example for practical purposes, but good enough to illustrate how extensions work.
All that's left now is to hook up this interface instead of the default implementation (`AGSSayLocationProvider`).

We do that by overriding the resolver (this needs to be done before creating the `IGame` object):

```csharp

Resolver.Override(resolver => 
    resolver.Builder.RegisterType<MyCustomSayLocationProvider>().As<ISayLocationProvider>());

```

And we're done.

## Points of interest

- Our custom implementation can get parameters in its constructor which the engine will know to fill up (using the same IOC container system). The same rules apply here as for writing a [constructor for a component](entities.md#constructor).

- Sometimes you'd want to use the default implementation for some things and your custom implementation only in specific scenarios. A way to do this would be to have an instance of the default implementation which you can delegate the incoming calls if needed. Let's extend our previous example by only return the static speech location when calling it for the first time:

```csharp

public class MyCustomSayLocationProvider : ISayLocationProvider
{
    private ISayLocationProvider _defaultProvider;

    public MyCustomSayLocationProvider(AGSSayLocationProvider defaultProvider)
    {
        _defaultProvider = defaultProvider;
    }

    public ISayLocation GetLocation(string text, ISayConfig config)
    {
        if (Repeat.OnceOnly("Custom Say Location"))
        {
            PointF textLocation = new PointF(400f,100f);
            PointF portraitLocation = new PointF(100f, 100f);
            return new AGSSayLocation(textLocation, portraitLocation);
        }
        else return _defaultProvider.GetLocation(text, config);
    }
}

```

- Sometimes you'd want your implementation to save state and then it's important to establish whether you can have multiple instances of your behavior or only one. For example, we could have written our previous example using a boolean variable instead of using [Repeat.OnceOnly](repeat.md):

```csharp

private bool _alreadyUsedCustomLocation;

//...

if (!_alreadyUsedCustomLocation)
{
    _alreadyUsedCustomLocation = true;
    //do our custom thing
}
else return _defaultProvider.GetLocation(text, config);

```

However, the `ISayLocationProvider` is requested by `ISayComponent` component, and there's one component for each speaking character (or entity), and by default the resolver will create a new instance for our implementation for each character, which means that that each character will have its own instance of the boolean variable, and our custom behavior will happen once for each character, not once for the entire game which might not be what we want.

Well, we can solve this by changing the `Resolver` override we did before and adding the `SingleInstance` command:

```csharp

Resolver.Override(resolver => 
    resolver.Builder.RegisterType<MyCustomSayLocationProvider>().SingleInstance().As<ISayLocationProvider>());

```

- Another thing to think about when we have state in our implementation, is that we might want to save this state in our game saves. One way to do that, is by adding our state as global variables (or to the entity's custom properties, or to the room's custom properties, depending on which system we're replacing). We plan to have easier ways to customize saving in the future.

- One final thing to note: in this example, and perhaps many others, it's possible that we didn't actually need to implement our own `ISayLocationProvider`, as the engine already gives us a way to change the location where the speech is rendered without overriding any interface. The `ISayComponent` component has an `OnBeforeSay` event which we can subscribe to. This event gives us the label that will be shown on the screen after its location was already decided, but we're then free to change its location to where we want it to be:

```csharp

cHero.OnBeforeSay.Subscribe(args => 
{
    args.Label.X = 400f;
    args.Label.Y = 100f;
});

```

So the conclusion here is that there might be a built in way to achieve what you want (or more than one way), so if you're not sure how to achieve something), please ask!