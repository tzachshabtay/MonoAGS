# Effects

We plan of offering a lot of built in effects which you'll be able to use in the game.
Currently, though, we only offer one effect:

## Shake

The `Shake` effect can shake either the screen or a specific target.
You can give it a strength (how aggressive is the shake) and a decay (how fast/slow the shake fades out).
You can run it for a set period of time, or run it endlessly (with the ability to stop it at any time).
Here's an example for shaking the screen for 5 seconds with the default strength and decay.

```csharp

ShakeEffect effect = new ShakeEffect ();
effect.RunBlocking(TimeSpan.FromSeconds(5));

```

