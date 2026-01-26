# MxUnikit - Extensions

Provides a collection of Unity extension methods for common operations

> WARN: This package is experimental and subject to frequent changes. It is not intended for public use.

## Features

### Transform Extensions

#### DestroyChildren
Destroys all children of a Transform with optional exclusions.

```csharp
// Destroy all children
transform.DestroyChildren();

// Ignore specific transforms
transform.DestroyChildren(child1, child2, child3);

// Ignore a list of transforms
List<Transform> keepThese = new List<Transform> { child1, child2 };
transform.DestroyChildren(keepThese);

// Chain with other operations
transform.DestroyChildren().SetParent(newParent);
```
