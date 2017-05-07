# Objects

Objects are basically everything that can move between rooms and can be interacted with.
So characters and GUIs are also objects.

## Object's components:

### Rooms

You can query an object's current room, it's previous room, and move an object to a different room.

### Tree

You can compose an object to be a part of a hierarchy. So you can have an object which has a parent and/or children.
Having an object in a tree has the following implications:
1. The position of the object is relative to its parent, and not relative to the screen.
2. If the parent is invisible the object will also be invisible (even if you made it visible).
3. If the parent is disabled the object will also be disabled (even if you made it enabled).

One practical use in games for object composition, for example, is to create a character while having each limb as a separate object,
so 4 objects will represent both legs and hands, and those will be parented by a "body" object. You can then animate and move the hands and legs separately from the body, and whenever you move the body the hands and legs will move with it.

Object composition is also useful for GUIs, so for example you can have a panel with 3 buttons: the panel will be the parent of those buttons,
so if you move the panel around all the buttons will move with it, if you hide the panel it will hide all the buttons, and same for disabling.

### Scale

You can scale all objects to make them bigger or smaller. You can either scale by a factor (i.e scale by 2 to have double the size- note that this refers to scaling the original 
size by 2, not the current size), or scale to a specific size (i.e I want my object to have a size of (200,500) pixels.
There are also convenience methods for flipping the object vertically and/or horizontally (this will shift the scale and also change the anchor accordingly).

Note that if the object is in a scaling area, then the scaling area takes over scaling, which you can disable by setting `IgnoreScalingArea` to true.

### Rotation

You can set (and get) an angle (in degrees) for every object you desire.

### Anchor

Anchor is the pivot point for the scale, rotation and position.
The anchor's units are normalized to the object's size, so (0,0) is the bottom left and (1,1) is the top right.
So if, for example your anchor is (0.5,0.5) and you rotate the object, the object will pivot on its center (so it would mainly stay in the same place), and all (x,y) positions for the object would refer to its center.

### Position (Translate)

Allows you to set (and get) the position of an object (x,y,z). (0,0) is the bottom left corner of the screen, and (max x, max y) for top right corner will be based on the virtual resolution
you selected to work with (either the game's virtual resolution, or a specific resolution that your object's render layer might be using to override the global virtual resolution).

Note that if the object is in a tree and have a parent, then the position will be relative to the parent and not to the screen. So if a button is in a panel and has an (0,0) position, it will always
sit at the bottom-left corner of the panel, no matter how the panel moves around the screen.

Also, the position is mapped to the position on the screen based on the viewport location (what the camera is looking at). So if the viewport is showing the room from (100,0) - (500, 0) and your object
is in (150,0) it will be mapped to (50, 0) to adjust for the viewport. This is the default for all objects which basically means that they are part of the game world.
This can be disabled by setting `IgnoreViewport` which is the default for UI objects, as you would normally want them to be static on the screen and not moving with the camera.

As for the Z property, it controls the rendering order within the object's rendering layer. By default the Z property binds to the Y property, i.e when Y moves Z will move with it, which is what you
would usually want for 2D perspective games (this way the more an object is further down the screen, the closer it will appear). 
If you set Z to be something else than Y, then it will stop moving along with Y, so you can set it independently. If you set Z to be Y, then binding will re-commence as before.

### Tint (Color)

Sets (or gets) the color (either RGBA or HSLA) in which the object is tinted (the default is white, i.e not tinted). 
You can also set the opacity (the alpha) separately, for convenience.

### Render Layer

A rendering layer is attached to each object. This decides rendering order, and also allow to set parallax effects and individual resolution. Please refer to the [Render Layers](render-layers.md) section for more details. 

### Visible/Enabled

Allows to show/hide an object, and to disable/enable an object (a disabled object will not respond to mouse hover/clicks). Note that if an object is part of a tree then setting the object to visible (and same goes for enabled) will not actually show (or enable) the object, if its parent (or grandparent, and so on) is not visible (or disabled). Refer to the [Tree](#tree) section for more details.

### Animation/Image

You can set either an image or an animation for your object (an image is just a shorthand convenience for an animation with one frame). Refer to the [Animations](animations.md) section for more details on animations.

#### Custom Rendering

You can also set a custom renderer for your object and implement your own logic of how to render the image/animation (for that you need to implement the `ICustomRenderer` interface and set it to the object). This is for custom software rendering. For custom hardware rendering you should use a shader.

### Shader

A shader is a program for rendering graphics on the GPU, making it potentially extremely fast and capable of doing effect not plausible with simple software rendering.
You can write a shader for a specific object (or objects) or for the entire game.
The shaders are written with [GLSL] (http://nehe.gamedev.net/article/glsl_an_introduction/25007/), a language specifically written for writing shaders for OpenGL. You should write a vertex shader and a fragment shader (those are the 2 common shaders, other shaders like the geometry shader which are less common are not currently supported). Then you can create your `IShader` object by either calling `GLShader.FromText` or `GLShader.FromResource` (from resource if you want to embed shader files).

### Border

Each object can have a border which surrounds it. A border can be drawn both behind and in front of the object, which opens up a lot of possibilities. Refer to the [Borders](borders.md) section for more details.

### Hotspot

Each object can be a hotspot with which the player can interact. Refer to the [Hotspots](hotspots.md) section for more details.

### Collisions

Each object can be checked for collisions with a specific point on the screen.
This is used by the engine for interacting with [Hotspots](hotspots.md).
You can select if the object's collisions will be based on a bounding box that surrounds the object, or a pixel-perfect collision, whereas the mouse has to specifically hover over a pixel which is not transparent to trigger the collision.

Pixel perfect is the default, as it more suitable in most cases, but in specific incidents (imagine an object with a small amount pixels which is hard to touch) you can turn it off which will switch the collision checks to use the bounding box.

### Custom Properties

You can set custom properties at run-time to any object (those can be strings, numbers, etc), which can be used for custom scripting purposes.

