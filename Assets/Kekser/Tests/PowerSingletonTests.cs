using System.Collections;
using Kekser.PowerSingleton;
using Kekser.Tests;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class PowerSingletonTests
{
    [UnityTest]
    [LoadScene("Assets/Kekser/Tests/Scenes/PowerSingletonTestScene.unity")]
    public IEnumerator TestNonExistingSingleton()
    {
        yield return null;
        Assert.IsFalse(DefaultSingleton.Created);
        Assert.IsNull(Single<DefaultSingleton>.Instance);
        LogAssert.Expect(LogType.Error, "PowerSingletonManager: No PowerSingletonAttribute for type Kekser.Tests.DefaultSingleton, and no instance in scene");
    }
    
    [UnityTest]
    [LoadScene("Assets/Kekser/Tests/Scenes/PowerSingletonTestScene.unity")]
    public IEnumerator TestDefaultSingleton()
    {
        yield return null;
        Assert.IsFalse(DefaultSingleton.Created);
        new GameObject("DefaultSingleton").AddComponent<DefaultSingleton>();
        yield return null;
        Assert.IsNotNull(Single<DefaultSingleton>.Instance);
    }
    
    [UnityTest]
    [LoadScene("Assets/Kekser/Tests/Scenes/PowerSingletonTestScene.unity")]
    public IEnumerator TestIfNeededSingleton()
    {
        yield return null;
        Assert.IsFalse(IfNeededSingleton.Created);
        Assert.IsNotNull(Single<IfNeededSingleton>.Instance);
        Assert.AreEqual("IfNeededSingleton", Single<IfNeededSingleton>.Instance.name);
    }
    
    [UnityTest]
    [LoadScene("Assets/Kekser/Tests/Scenes/PowerSingletonTestScene.unity")]
    public IEnumerator TestAlwaysSingleton()
    {
        yield return null;
        Assert.IsTrue(AlwaysSingleton.Created);
        Assert.IsNotNull(Single<AlwaysSingleton>.Instance);
        Assert.AreEqual("AlwaysSingleton", Single<AlwaysSingleton>.Instance.name);
    }
    
    [UnityTest]
    [LoadScene("Assets/Kekser/Tests/Scenes/PowerSingletonTestScene.unity")]
    public IEnumerator TestInterfaceSingleton()
    {
        yield return null;
        Assert.IsFalse(InterfaceSingleton.Created);
        new GameObject("InterfaceSingleton").AddComponent<InterfaceSingleton>();
        yield return null;
        Assert.IsNotNull(Single<IInterfaceSingleton>.Instance);
    }
    
    [UnityTest]
    [LoadScene("Assets/Kekser/Tests/Scenes/PowerSingletonTestScene.unity")]
    public IEnumerator TestInterfaceSingleton2()
    {
        yield return null;
        Assert.IsFalse(InterfaceSingleton.Created);
        new GameObject("InterfaceSingleton").AddComponent<InterfaceSingleton>();
        yield return null;
        Assert.IsNotNull(Single<IInterfaceSingleton>.Instance);
        Assert.IsNotNull(Single<InterfaceSingleton>.Instance);
    }
    
    [UnityTest]
    [LoadScene("Assets/Kekser/Tests/Scenes/PowerSingletonTestScene.unity")]
    public IEnumerator TestInterfaceSingleton3()
    {
        yield return null;
        Assert.IsFalse(InterfaceSingleton.Created);
        new GameObject("InterfaceSingleton").AddComponent<InterfaceSingleton>();
        yield return null;
        Assert.IsNotNull(Single<IInterfaceSingleton>.Instance);
        Assert.IsNotNull(Single<InterfaceSingleton>.Instance);
        Assert.AreEqual(Single<IInterfaceSingleton>.Instance, Single<InterfaceSingleton>.Instance);
    }
    
    [UnityTest]
    [LoadScene("Assets/Kekser/Tests/Scenes/PowerSingletonTestScene.unity")]
    public IEnumerator TestDeleteSingleton()
    {
        yield return null;
        Assert.IsFalse(DefaultSingleton.Created);
        new GameObject("DefaultSingleton").AddComponent<DefaultSingleton>();
        yield return null;
        Assert.IsNotNull(Single<DefaultSingleton>.Instance);
        GameObject.DestroyImmediate(Single<DefaultSingleton>.Instance.gameObject);
        yield return null;
        Assert.IsNull(Single<DefaultSingleton>.Instance);
        LogAssert.Expect(LogType.Error, "PowerSingletonManager: No PowerSingletonAttribute for type Kekser.Tests.DefaultSingleton, and no instance in scene");
    }
    
    [UnityTest]
    [LoadScene("Assets/Kekser/Tests/Scenes/PowerSingletonTestScene.unity")]
    public IEnumerator TestBindSingleton()
    {
        yield return null;
        Assert.IsFalse(BindSingleton.Created);
        Single<BindSingleton>.Bind(new GameObject("BindSingleton").AddComponent<BindSingleton>());
        yield return null;
        Assert.IsTrue(BindSingleton.Created);
        Assert.IsNotNull(Single<BindSingleton>.Instance);
    }
    
    [UnityTest]
    [LoadScene("Assets/Kekser/Tests/Scenes/PowerSingletonTestScene.unity")]
    public IEnumerator TestBindWrongSingleton()
    {
        yield return null;
        Assert.IsFalse(BindSingleton.Created);
        Single<BindSingleton>.Bind(new GameObject("BindSingleton").AddComponent<DefaultSingleton>());
        LogAssert.Expect(LogType.Error, "PowerSingleton: Can't bind Kekser.Tests.DefaultSingleton to Kekser.Tests.BindSingleton");
        yield return null;
        Assert.IsFalse(BindSingleton.Created);
        Assert.IsNull(Single<BindSingleton>.Instance);
        LogAssert.Expect(LogType.Error, "PowerSingletonManager: No PowerSingletonAttribute for type Kekser.Tests.BindSingleton, and no instance in scene");
    }
}

public class LoadSceneAttribute : NUnitAttribute, IOuterUnityTestAction
{
    private string _scene;
 
    public LoadSceneAttribute(string scene) => this._scene = scene;
 
    IEnumerator IOuterUnityTestAction.BeforeTest(ITest test)
    {
        Debug.Assert(_scene.EndsWith(".unity"));
        yield return EditorSceneManager.LoadSceneAsyncInPlayMode(_scene, new LoadSceneParameters(LoadSceneMode.Single));
    }
 
    IEnumerator IOuterUnityTestAction.AfterTest(ITest test)
    {
        yield return null;
    }
}
