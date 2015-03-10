#pragma strict

static var spawner : Spawner;

var caches : ObjectCache[];

var activeCachedObjects : Hashtable;


class ObjectCache {
	var prefab : GameObject;
	var cacheSize : int = 10;

	private var objects : GameObject[];
	private var cacheIndex : int = 0;

	function Initialize ()
	{
		objects = new GameObject[cacheSize];

		// Instantiate the objects in the array and set them to be inactive
		for (var i = 0; i < cacheSize; i++)
		{
			objects[i] = MonoBehaviour.Instantiate (prefab) as GameObject;
			objects[i].SetActive (false);
			objects[i].name = objects[i].name + i;
		}
	}

	function GetNextObjectInCache () : GameObject {
		var obj : GameObject = null;

		// The cacheIndex starts out at the position of the object created
		// the longest time ago, so that one is usually free,
		// but in case not, loop through the cache until we find a free one.
		for (var i : int = 0; i < cacheSize; i++) {
			obj = objects[cacheIndex];

			// If we found an inactive object in the cache, use that.
			if (!obj.activeSelf)
				break;

			// If not, increment index and make it loop around
			// if it exceeds the size of the cache
			cacheIndex = (cacheIndex + 1) % cacheSize;
		}

		// The object should be inactive. If it's not, log a warning and use
		// the object created the longest ago even though it's still active.
		if (obj.activeSelf) {
			Debug.LogWarning (
				"Spawn of " + prefab.name +
				" exceeds cache size of " + cacheSize +
				"! Reusing already active object.", obj);
			Spawner.Destroy (obj);
		}

		// Increment index and make it loop around
		// if it exceeds the size of the cache
		cacheIndex = (cacheIndex + 1) % cacheSize;

		return obj;
	}
}

function Awake () {
	// Set the global variable
	spawner = this;

	// Total number of cached objects
	var amount : int = 0;

	// Loop through the caches
	for (var i = 0; i < caches.length; i++) {
		// Initialize each cache
		caches[i].Initialize ();

		// Count
		amount += caches[i].cacheSize;
	}

	// Create a hashtable with the capacity set to the amount of cached objects specified
	activeCachedObjects = new Hashtable (amount);
}

static function Spawn (prefab : GameObject, position : Vector3, rotation : Quaternion) : GameObject {
	var cache : ObjectCache = null;

	// Find the cache for the specified prefab
	if (spawner) {
		for (var i = 0; i < spawner.caches.length; i++) {
			if (spawner.caches[i].prefab == prefab) {
				cache = spawner.caches[i];
			}
		}
	}

	// If there's no cache for this prefab type, just instantiate normally
	if (cache == null) {
		return Instantiate (prefab, position, rotation) as GameObject;
	}

	// Find the next object in the cache
	var obj : GameObject = cache.GetNextObjectInCache ();

	// Set the position and rotation of the object
	obj.transform.position = position;
	obj.transform.rotation = rotation;

	// Set the object to be active
	obj.SetActive (true);
	spawner.activeCachedObjects[obj] = true;

	return obj;
}

static function Destroy (objectToDestroy : GameObject) {
	if (spawner && spawner.activeCachedObjects.ContainsKey (objectToDestroy)) {
		objectToDestroy.SetActive (false);
		spawner.activeCachedObjects[objectToDestroy] = false;
	}
	else {
		objectToDestroy.Destroy (objectToDestroy);
	}
}