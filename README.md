# Project README

## How to Open and Run the Project

- **Starting the game from the editor:** Press **Ctrl+Shift+R**, or open the **Root** scene located in `Assets/Game/Scene` and press Play.

## How to Open the Editor and Create a Level

1. From the Unity top menu, select **Tool > Level Editor** to open the Level Editor.
2. In the first menu that appears, choose **Create New Level** or **Load Level**.
3. When a level is created, the entire grid is filled with cell objects by default.

### Tools

| Tool | Description |
|------|-------------|
| **Select Tool** | Selects placed Doors or Blocks. The properties of the selected object are shown in the panel on the right. |
| **Delete Tool** | Deletes Cells, Doors, and Blocks. When no object is selected, clicking an object deletes it entirely. When an object is selected, it lets you delete it piece by piece. |
| **Block Tool** | Draws Blocks. To turn the drawn Block into an object, click **Finish Object** at the top. After finishing, you can edit the new object's properties in the panel on the right. |
| **Door Tool** | Draws Doors, working the same way as the Block Tool. |
| **Move Tool** | Moves the selected object to a valid position. |

## Architecture Decisions and Rationale

- **MVC architecture following SOLID principles.** Each structure adheres to the Single Responsibility Principle and keeps the codebase open to extension and makes it easier to work on as part of a development team. I considered this a good fit for a project of this kind.
- **Dependency Injection.** This design pattern helped keep dependencies between structures to a minimum. It also means that if we were to write unit tests for this project later, we would not need to change any code in the View classes or other classes we don't intend to test; adding mock classes would be enough.
- **Reusable library folder.** Structures that can be reused in other projects were moved into a folder named `RollicGames`. This effectively creates a shared library of components that can be used in every game.

## LLM Tools Used and What They Were Used For

- Two Md files were added to the project: the first describes the rules of the development process, and the second is the Level Editor documentation.
- **No AI** was used for Block and Wall view generation.
- The core foundation of the game (for both the Home scene and the Level scene) was designed structurally by hand, with minimal AI assistance.
- Once the structure began to settle, development continued together with **Claude Sonnet 5.5**. Some code was written manually and some with AI assistance.
- After the game was fully complete, I wrote the documentation for the Level Editor and implemented the Level Editor with **Claude Opus 5.5**.

## Approximate Work Time: 36 Hours

| Time | Task | AI Usage |
|------|------|----------|
| 4 h | Project setup and Home menu implementation | No AI |
| 30 min | Player feature and Save system | With AI |
| 30 min | Root and scene management | With AI |
| 2 h | Settings popup prefab and code implementation | No AI |
| 1 h | Popup system that can be opened in any scene | Core without AI, remaining work with AI |
| 2 h | Level scene setup (excluding core); opening the Settings popup and returning to Home | No AI |
| 6 h | Core game structure (models), rendering Blocks as 9-slice visuals, GPU Instancing | — |
| 2 h | Level grid spawning, Door prefab creation and spawning | — |
| 4 h | Assigning IDs to level objects, defining the overall level flow, writing `LevelMoveController` and `LevelInputController`, build testing | — |
| 1 h | Smoothing object dragging | AI code review |
| 30 min | Caching the previous scene instead of destroying it when moving to the next scene | AI code review |
| 30 min | Pool system for the Model layer | AI code review |
| 1 h | Pool system for the View layer | AI code review |
| 30 min | Level timer | AI code review |
| 30 min | Level fail flow | AI code review |
| 3 h | Wall generation | No AI |
| 3 h | Door absorbing system + shader | Minimal AI |
| 1 h | Level goal and Level Complete popup flow | Mostly AI |
| 2 h | Level Editor documentation written and editor implemented with AI, then reviewed | With AI |
| 1 h | Final fixes and additions | — |

## Plugins Used

- **Zenject** – Dependency Injection
- **DOTween** – Animation
- **UniTask** – Async/await support
- **Newtonsoft JSON** – JSON serialization
- **Addressables** – Asset management
- **Search Extensions** – Editor search utilities