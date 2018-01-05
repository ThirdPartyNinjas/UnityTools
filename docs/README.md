# ThirdPartyNinjas - Unity Tools

A collection of tools we use to make games in Unity3D.

## Usage

Clone the repository into the Assets folder of your Unity project. If you only want a specific tool, you can just copy a specific folder to your Assets folder instead. (I recommend creating a ThirdPartyNinjas folder to put them in, but you can stick them directly in Assets or any subfolder of it.)

Some of the tools rely on some shared script files in the root of the repository. Check the documentation for the specific tool to see if you need any of them.

## Tools

* [CatmullRomSpline2D](../CatmullRomSpline2D/README.md) - A simple spline system and editor for creating curve paths.
* [FitCamera2D](../FitCamera2D/README.md) - A self resizing orthographic camera to help with rendering at different aspect ratio.
* [GameObjectAlignment](../GameObjectAlignment/README.md) - Editor tool for moving game objects around.
* [PrefabPool](../PrefabPool/README.md) - Memory pooling for prefab objects.

## Common files

### MonoBehaviourSingleton

An implementation of the [Singleton pattern](https://en.wikipedia.org/wiki/Singleton_pattern) using lazy initialization. This is useful if you want an object that lives at global scope, particularly if you want it to survive scene changes.

#### Usage

Derive your class from MonoBehaviourSingleton. If you don't want the object to persist on Unity scene changes (when it would otherwise be destroyed) override the PersistOnSceneChange property.

```
	public class SingletonExample : MonoBehaviourSingleton<SingletonExample>
	{
		public override bool PersistOnSceneChange { get { return false; } }
	}
```

When you want to get an instance of the object or create one if it doesn't exist, use the Instance property.

```
	var instance = SingletonExample.Instance;
```

If you want to try to grab an instance of the object, but not create one, use ExistingInstance instead. (A good reason to use this is to prevent accidentally creating a new instance during application shutdown.) Make sure to check for a null return.

```
	var instance = SingletonExample.ExistingInstance;
	if(instance != null)
	{
		// do something with it
	}
```

For a more concrete example usage, see [PrefabPool](../PrefabPool/README.md).
