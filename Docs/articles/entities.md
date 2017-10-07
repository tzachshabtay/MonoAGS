# Entities & Components

We talked about [objects](objects.md), [characters](characters.md), [guis](guis.md) and [room areas](areas.md), but what we haven't discussed is that those are all just entities with pre-set components added to them.

An Entities & Components system gives great flexibility to the engine to really let you fulfill even your craziest dreams. 

An entity is basically an empty canvas, until you start filling it up with components.
Each component added to the entity gives it new skills, so you can pick and choose the skills you need your entity to have, and you can add/remove components from the entity at any time during the game.

For example, the `object` is an entity with the following components:

- `IHasRoom`: attaches the object to a room.
- `IAnimationContainer`: adds the animation skill.
- `IInObjectTree`: adds the composition skill.
- `ICollider`: adds collision checking skill.
- `ITranslateComponent`: adds (x,y,z).
- And so on...

What this essentially means, is that an object does not have to stay an "object" during the whole course of the game. We can change it to a button, for example, by adding the necessary components for a button.

Or we can add the `IDraggableComponent`, which will allow us to the drag the object using the mouse.
We have multiple ways for adding components:

```csharp

//Indirect: engine will create the component for you
oInterestingBox.AddComponent<IDraggableComponent>();

//Another indirect version:
oInterestingBox.AddComponent(typeof(IDraggableComponent));

//Direct: you create the component yourself
IDraggableComponent component = doSomethingToCreateMyComponent();
oInterestingBox.AddComponent(component);

```

## Writing your own components

You can write your own components by inheriting from `AGSComponent`.
Here are some things you can do with your component:

### Constructor

In your component class constructor, you can have parameters which will be auto-filled by the engine
with correct values if somebody adds your component using the indirect version(s).
The parameters which the engine can auto-fill are the ones which the engine knows about, usually all the built-in system interfaces. If you try having parameters that the engine doesn't know how to fill (like a string), it will throw an exception.

```csharp

public MyComponent(IGame game) //this will work
{
    _game = game;
}

public MyComponent(int x) //this will crash
{
    _x = x;
}

```

### Component Dependencies

Your component might have a dependency on another component. For example, the `IDraggableComponent` has a dependency on the `ITranslateComponent`, as to drag an object you have to change its X and Y.

To get the required component, we can override the `Init` method which passes us the entity:

```csharp

public override Init(IEntity entity)
{
    _translateComponent = entity.GetComponent<ITranslateComponent>();
}

```

We might want to use that component right away, but note that during the `Init` method, the component we got might not be initialized itself (which might or might not be important for us).
If we want to guarantee we'll only use the component after it was initialized, we can override the `AfterInit` method:

```csharp

public override AfterInit()
{
    _translateComponent.X += 10;
}

```

Note, though, that components can also be removed from an entity at runtime, so if you have a dependency on a component, you can `Bind` to it and control how your own component handles a missing dependency. The `Bind` function gets 2 functions as  parameters, the first function gets called when the dependency component is added (it is also gets called if it's already there when you bind), and the second function gets called when the dependency component is removed.
So let's rewrite our code with `Bind`:

```csharp

public override Init(IEntity entity)
{
    entity.Bind<ITranslateComponent>(c => _translateComponent = c, _ => _translateComponent = null);
}

public override AfterInit()
{
    _translateComponent?.X += 10;
}

```

Note that we used the [Elvis operator](http://www.informit.com/articles/article.aspx?p=2421572) when accessing `_translateComponent` to make sure we only change `X` if the translate component is available.

Finally, we want to declare to the world (but mostly to the editor, when we'll have one) that we are dependent on the `ITranslateComponent`, for this we can use the `RequiredComponent` attribute which we use to decorate our class:

```csharp

[RequiredComponent(typeof(ITranslateComponent))]
public class MySuperUsefulComponent : AGSComponent
{
    //...
}

```