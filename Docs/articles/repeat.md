# Repeat

`Repeat` is a utility class for repeating actions a set number of times. `Repeat.OnceOnly(key)` is probably the most useful function there and is the equivalent of `Game.DoOnceOnly` from "classic" AGS.

```csharp
private void onInteractWithMicrophone(object sender, AGSEventArgs args) {
    if (Repeat.OnceOnly("sing_a_song")) {
        cHero.Say("Mary had a little lamb!");
    }
    else cHero.Say("Nah, I already sang earlier...");
}
```

Other similar functions there include: `Repeat.Exactly(key, number_of_times)` for doing an action exactly X times, and also `Repeat.MoreThen(key, number_of_times)` and `Repeat.LessThen(key, number_of_times)` for repeating actions more than or less than X times.

Another useful function is `Repeat.Rotate(key, actions)` which rotates around a list of actions to perform (i.e on each call it performs the next action on the list, and eventually goes back to the beginning). This can be useful when wanting to have multiple responses for an interaction which repeat themselves:

```csharp
private void someDefaultInteraction()
{
    Repeat.Rotate("default interaction",
        () => cHero.Say("I'm not going to do that."),
        () => cHero.Say("Nope, I don't think so."),
        () => cHero.Say("I really don't want to, sorry."));
}
```