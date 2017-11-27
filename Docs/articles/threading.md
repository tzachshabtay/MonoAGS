# Threading Model

`MonoAGS` has 2 threads that are created at startup and live throughout the lifetime of the application: the render thread and the update thread.

The render thread draws all of the game objects onto the screen, and the update thread processes all of the game logic to prepare all the objects to be drawn.

The update thread is the thread in which the [Repeatedly Execute](game.md#on-repeatedly-execute-event) event is triggered and all your logic code is running on. Naturally, this is the thread the modifies the game state.

The rendering thread, is the thread that interacts with OpenGL.
Due to limitations of OpenGL, only the rendering thread can access the OpenGL context and interact with it. Therefore, all custom rendering will be performed on the rendering thread. So if you implement your own custom renderer (`ICustomRenderer`), or your own border (`IBorderStyle`) or your own room transition (`IRoomTransition`) do be aware that those take place on the rendering thread, not on the update thread, and you should be very careful when working with the game state.

## Thread Pool

Besides the 2 long living threads, short living tasks (like loading resources, for example) use the thread pool: a random thread from the thread pool will be "borrowed", the task will be performed and the thread will return to the pool.
This is done in order to do some heavy-lifting outside the main logic loop so not to slow it down needlessly. Note that the dotnet thread pool knows and utilizes available CPU cores on your machine to do work in parallel. If you need to, you can take a thread from the pool yourself with `Task.Run` and take advantage of this functionality.

## Integrating with async/await

By default, if you run code on your own thread which awaits a task that runs from another thread, when the `await` returns, your code will run on a new thread (from the thread pool).

Here's an example of that in action:
```csharp
Debug.WriteLine($"Hello, I'm currently running in thread ${Environment.CurrentManagedThreadId}.");// running on thread 1
await Task.Run(() => Debug.WriteLine($"Now I'm running in thread ${Environment.CurrentManagedThreadId}.")); //Now running in thread 2

Debug.WriteLine($"After await, I'm now running in thread ${Environment.CurrentManagedThreadId}."); //Now running in thread 3
```

If you'll run this code from either the render or the update thread, however, you'll see that the last line runs on the first thread and not on a new thread. This is done by utilizing the [synchronization context](https://blogs.msdn.microsoft.com/pfxteam/2012/01/20/await-synchronizationcontext-and-console-apps/) concept. We're doing this in the rendering thread because if we await something on that thread we must go back to the same thread or we won't be able to interact with OpenGL in the follow-up code. We do it in the update thread because getting back to the same thread helps us avoid a plethora of potential multi-threading related bugs that might occur due to race conditions when accessing state.

Note, however, that if you write some module code that awaits something on those threads, but is completely isolated and doesn't touch the game state, you might want the await to return on a thread pool thread (to take advantage of the thread pool). In this case you can accomplish this by using `ConfigureAwait(false)` which tells the runtime not to use the synchronization context here:

```csharp
await Task.Run(() => doSomething()).ConfigureAwait(false);

//Now (after thw await returns) we're DEFINITELY using a thread from the thread pool, no matter which thread we started this code on.
```