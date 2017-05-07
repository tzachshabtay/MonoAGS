# Game Structure

The main steps of creating your game, generally speaking, from the technical side (we will not be discussing the creative side here) are:

1. Compose your assets (graphics & audio) and add to the project
2. Create the rooms in which the game takes place- add graphics to the room and set the areas in which the characters can walk (and optionally areas for scaling the characters, zooming the camera, walk behind the graphics, etc)
3. Create the objects that can be interacted with in the rooms, and place them in their starting rooms
4. Create the characters- player character(s) and NPCs, and place them in their starting rooms
5. Create all the inventory items which your player might pick up
6. Create your GUIs (graphical user interface)- decide how you want the game to be played (will it be a "right click to look" and "left click to interact" or do you want a rotating cursor system between multiple verbs, or maybe a list of verbs to choose from, etc) and create your buttons, menus, inventory window, mouse cursors and other controls.
7. Create animations for all of the above as desired
8. Code all of the interactions between the player character and other characters, objects, inventory items ang GUIs, including puzzles, dialogs and cut-scenes.
9. ...
10. Profit!

That's the basic layout of everything you need to do for a "classic" point & click game, the rest of this section will be dedicated to fleshing out all of those concepts and things you can do with them in the engine. 