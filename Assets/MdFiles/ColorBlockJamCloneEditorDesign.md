Design a tool that we can create, edit, save and load new or previous levels. 
Save levels as json with the unique id like: "level_id". File name is zero padded to 5 digits: "level_00001.json".
Open the tool from Tools > Level Editor.
JSON Example:
{
  "schemaVersion": 1,
  "levelId": 1,
  "duration": 60.0,
  "grid": { "width": 5, "height": 6, "cells": [ {"x":0,"y":0}, ... ] },
  "blocks": [ { "id":1, "type":"Block", "color":"Blue", "position":{...}, "localPositions":[...]} ],
  "doors":  [ { "id":1, "type":"Door", "color":"Blue", "position":{"x":2,"y":-1}, "length":3, "absorbDirection":"Up"} ]
}
- JSON is written with Newtonsoft (com.unity.nuget.newtonsoft-json). Enums are written as strings.
- Block and door ids are renumbered as 1, 2, 3... while saving.
- Grid coordinates: x grows to the right, y grows down (y = 0 is the top row, Up = (0,-1)).

Objects in Level:
- Cell
- Block
- Door

- Creating Level:
	- Load previous saved level option.
	- Create New Level Option: Enter Level ID (unique), Enter Level Duration as seconds, Width, Height
	- After finishing editing level => Save Level Option (This is the only option if you create a new), Save As New(Additional if you loaded previous saved level)
	- Save Level Option: It gives error and doesn't save if the level id is not unique.
	- Save As New Option: It gives error and doesn't save if the level id is not unique includes loaded level id.
	- Create grid with given width, height. Grid has filled with the cells as default.
	- Level ID, Duration, Width and Height are all editable after the level is created or loaded.
	- Save Level on a loaded level with a changed Level ID renames the file. Uniqueness is checked against the other saved levels.
	- Save As New is also shown after a new level has been saved once.
	- Resizing the grid removes the cells, blocks and doors outside the new size (asks for confirmation when shrinking). New area is filled with cells, except spots that have a door.
	- Leaving the level or closing the window with unsaved changes asks for confirmation.

Grid Cell Rules:
- Select Cell object to place
- Can be placed anywhere in grid that has not door on it.
- Cell Object has 1x1 Space
- Deleting the cell is deleting the whole block object on it.
- Doors that are left without a neighbour cell after a cell is deleted are deleted too.
	
Block Rules:
- Select Block object to place
- Blocks can be only placed on Grid Cells.
- Once you started to draw, it's an object until you click finish object.
- Place 1 1x1 block in an empty space first.
- After first placement, you need to place blocks to empty spaces that neighbour of the previous placed spaces of that object.
- You can not place the blocks after first placement that is not neighbour of the filled spaces with that object blocks.
- After clicking Finish Object (or Enter), object is created and placed as you placed. And object is selected. Cancel (or Esc) discards it. Switching to another tool also discards it.
- New objects are Blue by default.
- Grid position of a block is the upper-left corner of its bounding box, even if there is no 1x1 block on that corner. Local positions are relative to it. Example: a mirrored L with tips (2,0) and (0,2) has position (0,0).
- You can edit the selected block object like selecting the object color in the right inspector.
- You can move the shape into another empty spaces that same shape with the object. Moving is done with a separate Move tool: drag the object, the preview turns green where it fits and red where it doesn't. Releasing on red cancels the move.
- Delete tool deletes block object as one when any block object is not selected. If Block object selected, delete tool deletes only one 1x1 of block that is mouse on. If the remaining 1x1 blocks of the object stay connected after deleting the mouse on 1x1 block, delete tool icon turns to green and it can be deleted. Otherwise (the object would be split into separate parts) delete tool icon turns to red and it can't be deleted. Doors follow the same rule.
- When an object is selected, clicking another object (or an empty cell) with the delete tool only deselects the current object. The next click uses the not selected flow.
- Delete tool order: when the cell has a block on it, the block is deleted first. Clicking the empty cell after that deletes the cell.

Door Rules:
- Select Door object to place
- Drawing door is nearly same with the block but except, it only can be drawn as vertical or horizontal, not both.
- Doors can be only placed neighbor to cells in grid, not on the cells. You can think they are in the same layer.
- Every segment of a door must be neighbour of a cell. Doors can be placed in the one cell ring around the grid or in empty spots (holes) inside the grid.
- Length will be auto calculated while saving. We can see the color and absorb direction attributes in inspector of a door.
- Absorb direction is chosen automatically (pointing away from the cells the door touches) and can always be changed in the inspector. Horizontal doors can be Up or Down, vertical doors Left or Right, 1 length doors any direction.
- Grid position of a door is its leftmost (horizontal) or topmost (vertical) segment.
- Doors follow the same delete, select and move rules as blocks.

Development:
- Create a Editor folder under the Game/Scripts/Level director.
- Use the current implementations for like LevelColor, LevelDirection or LevelGridModel. Do not create anything extra.
- Editor folder will have it's own assembly definition file.
- Do not use MVC but keep code clean and follow the Development MD for SOLID or other things. Editor will be open to develop but closed to change.
- Level save directory: "Game/Levels".
- The editor shows the level in a visual form. It does not show only numeric fields