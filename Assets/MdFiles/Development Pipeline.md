# Development Pipeline

## 1. Project File Structure

The project should follow a feature-based architecture.

### Shared Rollic Games Code

Code that is shared between multiple games should be placed under:

```text
Assets/
└── RollicGames/
    └── FeatureX/
        └── Runtime/
            ├── Art/
            ├── Prefabs/
            └── Scripts/
                ├── Model/
                ├── View/
                ├── Controller/
                └── Injection/
```

### Common Code

Code that provides reusable systems applicable to every game should be placed under:

```text
Assets/
└── RollicGames/
    └── FeatureZ/
        └── Runtime/
```

Examples include:

- Object pooling
- Generic factories
- Common utilities
- Shared infrastructure
- Reusable game systems

---

## 2. Assembly Definitions

Every `Model`, `View`, `Controller`, and `Injection` folder must have its own Assembly Definition (`.asmdef`).

### Game Assembly Naming

For game-specific code:

```text
RollicGames.SampleGame.FeatureX.Runtime.Model
RollicGames.SampleGame.FeatureX.Runtime.View
RollicGames.SampleGame.FeatureX.Runtime.Controller
RollicGames.SampleGame.FeatureX.Runtime.Injection
```

### Shared RollicGames Assembly Naming

For shared RollicGames code:

```text
RollicGames.FeatureX.Runtime.Model
RollicGames.FeatureX.Runtime.View
RollicGames.FeatureX.Runtime.Controller
RollicGames.FeatureX.Runtime.Injection
```

Assembly dependencies must respect the architecture described below.

---

# Coding Principles

## 3. SOLID

All code should follow SOLID principles.

### Single Responsibility Principle

Make classes as atomic as reasonably possible.

A class should have one clear responsibility and one primary reason to change.

Avoid large "manager" classes that handle unrelated responsibilities.

### Open-Closed Principle

Features should be:

- Open for extension
- Closed for modification

Prefer extending existing behavior through abstractions, composition, or new implementations rather than repeatedly modifying stable existing code.

### Liskov Substitution Principle

Derived classes must be usable wherever their base class or abstraction is expected.

Do not create inheritance relationships where the derived type cannot correctly fulfill the contract of the base type.

### Interface Segregation Principle

Do not force classes to implement interfaces containing methods irrelevant to them.

Prefer small, focused interfaces.

If an interface contains unrelated responsibilities, consider splitting it into more atomic interfaces.

### Dependency Inversion Principle

Do not create unnecessary direct dependencies between concrete classes.

Depend on abstractions instead.

High-level systems should not depend directly on low-level implementations.

Use dependency injection where appropriate.

---

# MVC Architecture

## 4. General Structure

Use a strict Model-View-Controller architecture.

```text
Model
  ↑
Controller
  ↓
View
```

The responsibilities of each layer must remain clearly separated.

---

## 5. Model

The Model represents data and game/domain state.

### Rules

- Model must not reference any Unity Engine module.
- Model must not contain presentation logic.
- Model should not contain gameplay processing logic that belongs to Controllers.
- Keep Models as simple and deterministic as possible.
- Avoid unnecessary methods that perform operations better suited to Controllers.

For example, do not put collection-processing or gameplay operations inside the Model simply because the Model owns the data.

Controllers should process and manipulate Model data.

---

## 6. View

The View represents Unity-specific presentation and interaction.

### Rules

- View is the only MVC layer that may reference Unity Engine modules.
- View should be responsible for presentation.
- View should not contain core gameplay/business logic.
- View should expose the functionality required by Controllers through abstractions/interfaces.
- Views should be replaceable without requiring changes to the Controller's core logic.
- Do not make null checks for views in Controller layer, let the running throws exception to see where the problem is.

Examples:

- MonoBehaviours
- GameObjects
- Animations
- Visual effects
- Unity UI
- Transform manipulation
- Audio playback

---

## 7. Controller

The Controller contains gameplay/application logic and coordinates Models and Views.

### Rules

- Controller must not depend on concrete View implementations.
- Controller should depend on View abstractions/interfaces.
- Controllers should be testable independently from Unity.
- Controllers should be able to operate without a View being present.
- Controllers are responsible for processing and manipulating Model data.
- Controllers coordinate the interaction between Model and View.

### Important Requirement

**Controllers must work without View classes.**

This is extremely important for:

- Unit testing
- Headless execution
- Extendibility
- Reusability
- Separation of concerns

A Controller should be able to execute its core logic even if no visual representation exists.

---

# Presenter

## 8. Exceptional Presenter Layer

A fourth layer, `Presenter`, may be introduced when necessary.

The Presenter is intended for exceptional cases where View-specific or Unity-specific behavior must be separated from the Controller.

The Presenter:

- May reference Unity Engine.
- May act similarly to a Controller from a presentation perspective.
- Must not become a replacement for the Controller.
- Should only be introduced when MVC alone cannot provide a clean architecture.

### Dependency Direction

The Controller may reference the Presenter.

The Presenter must **not** reference the Controller.

```text
Controller
    ↓
Presenter
    ↓
Unity / View
```

Never create a reverse dependency:

```text
Presenter
    ✕
    ↓
Controller
```

---

# Architecture Restrictions

## 9. Unity Engine Dependencies

The Model and Controller assembly definitions must **never** reference Unity Engine modules.

Only View-related assemblies may reference Unity Engine modules.

This restriction should be treated as an architectural rule, not merely a guideline.

### Allowed

```text
Model
  → Pure C# / project abstractions

Controller
  → Model
  → Abstractions
  → Other non-Unity systems

View
  → Unity Engine
  → Controller abstractions
```

### Forbidden

```text
Model
  → UnityEngine

Controller
  → UnityEngine

Model
  → View

Controller
  → Concrete View
```

---

# Libraries

## 10. Approved Libraries

### DoTween

Use **DoTween** for animations and tween-based transitions.

Avoid implementing custom tweening systems unless there is a strong technical reason.

### UniTask

Use **UniTask** for asynchronous operations.

Benefits:

- Async/await syntax
- Low allocation compared to common alternatives
- Good Unity integration
- Compatible with DoTween

Prefer UniTask over unnecessary coroutine-based implementations when asynchronous control flow is required.

### Odin Inspector

Use **Odin Inspector** when it provides meaningful improvements to:

- Inspector workflows
- Editor tooling
- Serialization
- Debugging
- Development productivity

Do not use Odin features unnecessarily.

### Zenject

Use **Zenject** for dependency injection.

Zenject should handle dependency injection and object composition.

However, do **not** rely on Zenject for performance-sensitive infrastructure such as:

- Object pooling
- Pool factories
- Signal bus implementations

For these systems, use optimized custom implementations where appropriate.

---

# Unit Testing

## 11. General Rules

Code must be designed with testability in mind.

Do not add methods, fields, properties, or other structures to production features solely for the purpose of making tests possible.

Production architecture should remain clean and meaningful independently of tests.

### Testing Principles

- Prefer testing Controllers independently from Unity.
- Keep Models deterministic where possible.
- Avoid unnecessary Unity dependencies in testable logic.
- Use abstractions to isolate external dependencies.

---

## 12. Mocking

**NSubstitute** may be used for mocking dependencies.

Views should generally be mocked through their interfaces rather than instantiated as real Unity objects.

Example concept:

```text
Controller
    ↓
IExampleView
    ↑
NSubstitute Mock
```

This allows Controller tests to run without requiring Unity scenes or GameObjects.

---

## 13. Unity-Independent Game Loop

If a game's logic does not depend on Unity's `Time.deltaTime` or other Unity runtime state, the entire game loop should be testable without running Unity.

Prefer deterministic, Unity-independent logic wherever practical.

For example:

```text
Input
  ↓
Controller
  ↓
Model
  ↓
Game State
```

This should ideally be executable in a standard C# unit test environment.

---

# General Development Rules

## 14. Before Implementing a Feature

When implementing a new feature:

1. Determine whether the feature is game-specific or shared.
2. Place it in the correct `Assets/Game` or `Assets/RollicGames` hierarchy.
3. Separate Model, View, Controller, and Injection responsibilities.
4. Create the required Assembly Definitions.
5. Verify assembly dependencies follow the architecture rules.
6. Keep Model and Controller free from Unity Engine dependencies.
7. Prefer abstractions over concrete dependencies.
8. Make Controllers testable without Views.
9. Avoid adding unnecessary complexity or abstractions.
10. Reuse existing systems and libraries where appropriate.

---

# Architectural Priority

When making implementation decisions, prioritize the following:

1. **Clear separation of responsibilities**
2. **Testability**
3. **Dependency inversion**
4. **Low coupling**
5. **Reusability**
6. **Maintainability**
7. **Performance**
8. **Convenience**

Do not sacrifice architectural boundaries merely to make a feature faster to implement.

When an exception to these rules is necessary, keep the exception as localized as possible and avoid allowing it to spread into the rest of the architecture.