# PrefabPool

Memory pooling is a common strategy used to avoid allocating and deallocating objects, because we want to avoid angering the unpredictable fire-breathing Garbage Collector monster.

This application of an object pool specifically focuses on reusing spawned prefabs.

## Usage

To use a prefab with PrefabPool, your game object must be derived from PrefabMonoBehaviour instead of Unity's standard MonoBehaviour class. It's a very thin wrapper that adds a field so that we can tag it with the prefab the game object was spawned from. This is used to sort back into the correct pool collection when the object is returned.

While it's not required, it's recommended to add a public Reset method to your class and handle your initialization there. You can't rely on Awake and Start for initialization, because the object is only spawned once and recycled. OnEnable and OnDisable will still work normally, though.

For example:

```
public class PoolDemoShip : PrefabMonoBehaviour
{
	Vector3 speed = Vector3.zero;

	public void Reset()
	{
		transform.position = Vector3.zero;
		speed.x = Random.Range(-10.0f, 10.0f);
		speed.y = Random.Range(-10.0f, 10.0f);
		speed.z = 0.0f;
	}
}
```

To take or return game objects from the pool, simply call the TakeObject or ReturnObject methods. TakeObject takes and optional Transform parameter if you want the retrieved object to be parented for you.

Taking example:

```
GameObject shipPrefab;

void Update()
{
	if(readyToTake)
	{
		var ship = PrefabPool.Instance.TakeObject<PoolDemoShip>(shipPrefab);
		ship.Reset();
	}
}
```

Returning example:

```
// in the PoolDemoShip class

void Update()
{
	if(readyToReturn)
	{
		PrefabPool.Instance.ReturnObject(this);
	}
}
```

### Note

PrefabPool relies on MonoBehaviourSingleton, so make sure you're also adding it to your project. For information about that class see the [documentation here](../docs/README.md).