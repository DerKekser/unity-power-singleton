# Power Singleton

[Power Singleton](https://github.com/DerKekser/unity-power-singleton) is a flexible Singleton Pattern package that's designed to streamline your Unity game development. Unlike traditional Singletons, these are defined merely by an attribute and require only to implement Unity's `MonoBehaviour`.

## Contents
- [Simple Example](#simple-example)
- [Interface](#interface)
- [If Needed Creation](#if-needed-creation)
- [Auto Creation](#auto-creation)
- [DontDestroyOnLoad](#dontdestroyonload)
- [Install via git URL](#install-via-git-url)
- [License](#license)

### Simple Example

You can define a singleton by adding the `PowerSingleton` attribute to a class.
The class must inherit from `MonoBehaviour` and the attribute must be applied to the class.
The type of the singleton must be specified in the attribute.

```csharp
using Kekser.PowerSingleton.Attributes;
using UnityEngine;

[PowerSingleton(typeof(GameManager))]
public class GameManager : MonoBehaviour
{
    public void AddScore(int score)
    {
        // ...
    }
    
    // ...
}
```
You can access the singleton by using the `Singleton` class.

```csharp
Singleton.Get<GameManager>().AddScore(10);
```
### Interface

You can also define a singleton by an interface.
This widened flexibility allows you to use multiple Singleton classes, given they implement the specified interface. The system intuitively chooses the active class in the current scene.

```csharp
public interface IGamemanager
{
    void AddScore(int score);
}
```
```csharp
using Kekser.PowerSingleton.Attributes;
using UnityEngine;

[PowerSingleton(typeof(IGamemanager))]
public class GameManager : MonoBehaviour, IGamemanager
{
    public void AddScore(int score)
    {
        // ...
    }
    
    // ...
}
```
```csharp
Singleton.Get<IGamemanager>().AddScore(10);
```
### If Needed Creation

You can define that the singleton should only be created when it is needed.
This is useful if you want to use the singleton in a scene where it is not yet present.
The singleton will be created as soon as it is needed.

```csharp
using Kekser.PowerSingleton.Attributes;
using UnityEngine;

[PowerSingleton(typeof(GameManager), Creation = PowerSingletonCreation.IfNeeded, CreationName = "GameManager")]
public class GameManager : MonoBehaviour
{
    public void AddScore(int score)
    {
        // ...
    }
    
    // ...
}
```
```csharp
Singleton.Get<GameManager>().AddScore(10);
```
### Auto Creation

You can define that the singleton should be created automatically.
This is useful if you want to use the singleton in a scene where it is not yet present.
The singleton will be created as soon as the scene is loaded.

```csharp
using Kekser.PowerSingleton.Attributes;
using UnityEngine;

[PowerSingleton(typeof(GameManager), Creation = PowerSingletonCreation.Always, CreationName = "GameManager")]
public class GameManager : MonoBehaviour
{
    public void AddScore(int score)
    {
        // ...
    }
    
    // ...
}
```
```csharp
Singleton.Get<GameManager>().AddScore(10);
```
### DontDestroyOnLoad

You can define that the singleton should not be destroyed when the scene is changed.

```csharp
using Kekser.PowerSingleton.Attributes;
using UnityEngine;

[PowerSingleton(typeof(GameManager), DontDestroyOnLoad = true)]
public class GameManager : MonoBehaviour
{
    public void AddScore(int score)
    {
        // ...
    }
    
    // ...
}
```
```csharp
Singleton.Get<GameManager>().AddScore(10);
```
### Install via git URL

You can add this package to your project by adding this git URL

```
https://github.com/DerKekser/unity-power-singleton.git?path=Assets/Kekser/PowerSingleton
```
### License

This library is under the MIT License.