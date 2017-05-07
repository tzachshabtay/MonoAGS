# Async & Await

[Async & Await](https://docs.microsoft.com/en-us/dotnet/articles/csharp/async) is a model for asynchronous programming which is heavily used by the engine.

It allows you to do things in parallel and wait for them asynchronously while still keeping the logical flow of the code in one sequence (thus avoiding the notorious [callback hell](http://callbackhell.com/) which javascript developers know all too well).

Before getting into the nitty gritty, let's look at a few examples.

```csharp

cHero.Walk(100,100);

await cHero.WalkAsync(100,100);

```

Those 2 lines have the same logical meaning, the character will walk to (100,100), in terms of the user there's no difference.
The key to understanding the benefit for the async model, is that we don't have to `await` the walk right away.
The `await cHero.WalkAsync` is actually 2 parts: the `WalkAsync` starts the walk and the `await` waits for it to complete.
So we can split it like this:

```csharp

Task walking = cHero.WalkAsync(100,100);

//We can do stuff here, while the character is walking

//Doing some more stuff

//Ok, done, now we can wait for the walk to complete before moving on
await walking;

//Now we can do more stuff, after the character finished walking

```

The `WalkAsync` method returns a [task](https://msdn.microsoft.com/en-us/library/dd537609(v=vs.110).aspx), an asynchronous operation which we can wait for (or not), whenever we please. We can also benefit from the rich API the dot net task provides us, including `ContinueWith` for chaining tasks, and waiting for multiple tasks together, either with `Task.WhenAll` which will wait for all tasks to complete, or with `Task.WhenAny` which will wait for the first task to complete.

We can think of a real game scenario which can be relatively difficult to code with the "classic" AGS, but trivial to code using async/await: A guard which walks in circles endlessly in the background:

```csharp

private async Task guardWalk()
{
    while (someConditionApplies())
    {
        await cGuard.WalkAsync(100,100);
        await cGuard.WalkAsync(200,100);
        if (cHero.X < 200) 
        {
            await cGuard.SayAsync("Go away!!");
        }
        await cGuard.WalkAsync(200,200);
        await cGuard.WalkAsync(100,200);
    }
}

```

Now think of what it takes to code something like this with 'classic' AGS. You have to add and track state on each tick, remember where you were and where you're going, etc.
Async & Await helps you execute things in parallel with great ease.

## The Nitty Gritty

The `async` keyword is used to mark a method that might run asynchronously, and the `await` keyword is used to "asynchronously wait" for an asynchronous method to complete.
If I'm awaiting an asynchronous method to complete, it means that my method is also asynchronous and should therefore be marked with `async`.

If I'm an asynchronous method, I can either return `void` (i.e nothing), a `Task` or a `Task<TResult>`. If I return `void` it means that my method cannot be awaited by another method, making my method a fire-and-forget method. It can be useful in some scenarios, but usually it's not desired.
So usually the method will return `Task` which lets other methods `await` it, or `Task<TResult>` which allows the method to return an actual result (asynchronously):

```csharp

private readonly HttpClient _httpClient = new HttpClient();
var text = await _httpClient.GetStringAsync(url); //Will download text from a url asynchronously and return the text
```

One important gotcha here, is that you might be tempted to synchronously wait (i.e block) on an asynchronous method. This can technically be done, for example:

```csharp

cHero.WalkAsync(100,100).Wait();

//or:

var text = _httpClient.GetStringAsync(url).Result;

```

However due to how async/await is implemented behind the scene (a complicated state machine) this might lead to deadlocks (the computer hanging) in some scenarios, especially if you try doing this from the rendering thread.
Therefore it is not recommended blocking on an asynchronous method unless you really know what you're doing.

The implication of this, is that usually once you go async, the entire calling chain should go async (everything calling you will go async, and so on until the end of the calling chain).
The end of the calling chain, for `MonoAGS` is usually subscribing to an event. The `IEvent` method allows you to subscribe to an asynchronous callback (`SubscribeToAsync`) which you should use to "end" the chain.

When subscribing to an event, if you suspect you might need it to be asynchronous in the future, you can subscribe to the async version even if you are not currently async:

```csharp

oBottle.Interactions.OnInteract(AGSInteractions.Look).SubscribeToAsync(onLookBottle);

private Task onLookBottle(object sender, ObjectEventArgs args)
{
    cHero.Say("It's a bottle.");
    return Task.CompletedTask;
}

```

By returning `Task.CompletedTask`, I'm "faking" an asynchronous result so other methods can still `await` my method, without knowing it isn't really asynchronous.

Later on, when I add my asynchronous stuff, I'll remove it:

```csharp

private async Task onLookBottle(object sender, ObjectEventArgs args)
{
    await cHero.SayAsync("It's a bottle.");
}

```



