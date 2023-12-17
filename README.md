# Unity - Power Singleton

[Power Singleton](https://github.com/DerKekser/unity-power-singleton) is a flexible Singleton Pattern package that's designed to streamline your Unity game development. Unlike traditional Singletons, these are defined merely by an attribute and require only to implement Unity's `MonoBehaviour`.

## Contents
- [Simple Example](#simple-example)
- [Interface / Abstract Class](#interface--abstract-class)
- [Creation](#creation)
    - [If Needed Creation](#if-needed-creation)
    - [Auto Creation](#auto-creation)
- [DontDestroyOnLoad](#dontdestroyonload)
- [Manual Binding](#manual-binding)
- [Install](#install)
    - [Install via Unity Package](#install-via-unity-package)
    - [Install via git URL](#install-via-git-url)
- [License](#license)

### Simple Example

You can define a singleton by adding the `PowerSingleton` attribute to a class.
The class must inherit from `MonoBehaviour` and the attribute must be applied to the class.

```csharp
using Kekser.PowerSingleton.Attributes;
using UnityEngine;

[PowerSingleton]
public class GameManager : MonoBehaviour
{
    public void AddScore(int score)
    {
        // ...
    }
    
    // ...
}
```
You can access the singleton by using the `Single` class.

```csharp
using Kekser.PowerSingleton;

Single<GameManager>.Instance.AddScore(10);
```
### Interface / Abstract Class

You can also define a singleton by an interface or an abstract class.
This widened flexibility allows you to use multiple Singleton classes, given they implement the specified interface. The system intuitively chooses the active class in the current scene.

```csharp
public interface IGameManager
{
    void AddScore(int score);
}
```
```csharp
using Kekser.PowerSingleton.Attributes;
using UnityEngine;

[PowerSingleton(typeof(IGameManager))]
public class GameManager : MonoBehaviour, IGameManager
{
    public void AddScore(int score)
    {
        // ...
    }
    
    // ...
}
```
You can access the singleton by using the `Single` class and specifying the interface.
```csharp
using Kekser.PowerSingleton;

Single<IGameManager>.Instance.AddScore(10);
```
### Creation

#### If Needed Creation

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
#### Auto Creation

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
### Manual Binding

You can manually bind a MonoBehaviour to a singleton.
This is useful if you cannot use the `PowerSingleton` attribute or if you want to change the singleton at runtime.

```csharp
using Kekser.PowerSingleton;

GameManager gameManager = new GameObject("GameManager").AddComponent<GameManager>();
Single<GameManager>.Bind(gameManager);
```
### Install

#### Install via Unity Package

Download the latest [release](https://github.com/DerKekser/unity-power-singleton/releases) and import the package into your Unity project.
#### Install via git URL

You can add this package to your project by adding this git URL in the Package Manager:
```
https://github.com/DerKekser/unity-power-singleton.git?path=Assets/Kekser/PowerSingleton
```
![Package Manager](/Assets/Kekser/Screenshots/package_manager.png)
### License

This library is under the MIT License.