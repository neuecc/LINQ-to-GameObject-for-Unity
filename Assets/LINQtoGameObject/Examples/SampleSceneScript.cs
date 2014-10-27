using UnityEngine;
using System.Collections;
using Unity.Linq; // using LINQ to GameObject

// This script attached to Root.
public class SampleSceneScript : MonoBehaviour
{
    void OnGUI()
    {
        if (GUILayout.Button("Builder"))
        {
            var cube = Resources.Load("Prefabs/PrefabCube") as GameObject;

            var tree =
                new GameObjectBuilder(cube,
                    new GameObjectBuilder(cube),
                    new GameObjectBuilder(cube,
                        new GameObjectBuilder(cube)),
                    new GameObjectBuilder(cube));

            tree.Instantiate();
        }
    }
}