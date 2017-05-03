# Hotspots

You can make any object to be a "hotspot" which will allow the player to interact with it.

## Hotspot Text

You can supply a hotspot text for the object. This will be shown via a hotspot label (a label which shows the hotspot text of an object when the mouse hovers over it).

## Walk Point

You can also set a walk point for this object. This is the point to which the player character will walk to before triggering an interaction script. Note though that this depends on which "Approach style" (if any) you chose for your character, see [Approaching](characters.md#approaching).

## Interactions

For each hotspot you should subscribe to interaction events, which define what happens when interacting with the hotspot. The interaction model, is very simple, you only have 2 types of interactions with objects:

### Interact Event

This is used for all "verb" interactions with the object. Depending on your game, you might have different verbs which you can use. A 2 buttons style game usually has 2 verbs, "Look" and "Interact". A Sierra style game might have a rotating cursor system with "Look", "Interact" and "Talk" verbs. A Monkey Island style game might have 9 different verbs to choose from, and a verb coin style game might have different verbs per object.

The engine allows all of those modes. By using the `IInteractions` interface, you'll request an event for the verb you're interested in and subscribe to it.

As an example, you can write:

```csharp
oLockedDoor.Interactions.OnInteract("Look").Subscribe(onLookingAtLockedDoor);

private void onLookingAtLockedDoor(object sender, ObjectEventArgs args)
{
    cHero.Say("This door is locked.");
}
```

Note that in c#, you can also write this in one line using anonymous functions:

```csharp
oLockedDoor.Interactions.OnInteract("Look").Subscribe((sender, args) => cHero.Say("This door is locked."));
```

Also note that for "Look" and "Interact", the two most common verbs in adventure games today, the engine has some defaults built in, and it's recommended (but not mandatory) that you use the predefined `AGSInteractions.LOOK` and `AGSInteractions.INTERACT` instead of coming up with your own names.

### Inventory Interact Event

Inventory interactions is for when a character is holding an inventory item and wants to use it on an object. Depending on your game, you might have different verbs here too. Most games today only have a "Use inventory item on object" but some games might also additionally have a "Give item to object" for example. Therefore the inventory interact also gets a verb, and otherwise acts the same as the interact event, only you get the used inventory item as part of the event arguments:

```csharp
oLockedDoor.Interactions.OnInventoryInteract(AGSInteractions.INTERACT).Subscribe(onUsingInventoryOnDoor);

private void onUsingInventoryOnDoor(object sender, InventoryInteractEventArgs args)
{
    if (args.Item == iKey)
    {
        cHero.Say("Hooray! I opened the door.");
    }
    else
    {
        cHero.Say("I don't understand how this would help me to open the door.");
    }
}
```

### Default Interactions

You might not want to write interactions for all your objects and verbs. Depending on the amount of objects, verbs and inventory items you have, this might prove an impossible task.
The solution in this case is to have a fallback, usually a default generic sentence (or a random sentence out of a list) that the character says, something along the line of "I can't do that".
That being said, "I can't do that", might be seem as a tad too generic, maybe we can do a little bit more.

This is why, there are several ways you can configure defaults for your interactions. For once, for each hotspot object, with the 2 previous interaction events we showed, you can also pass `AGSInteractions.DEFAULT` as your verb. This will be used as the default interaction for this object when you did not subscribe to a specific verb.

Additionally, there's also a global `IInteractions` object (you can find it in `IGame.Events.DefaultInteractions`) for which you can subscribe to supply default scripts which are not per a specific object. As this uses the same interface as the hotspot interactions, you use the same events as you saw on the examples here, to have a default per verb but not per object. Finally, you can also pass the `AGSInteractions.DEFAULT` verb to the default interactions object, for a super generic default behavior ("I can't do that"). This will be only used when everything else fails.

To recap, when the engine tries to match an interaction to a script, this is the order it uses:

1. Object's verb script
2. Object's default verb script
3. Default interactions verb script
4. Default interactions default verb script

