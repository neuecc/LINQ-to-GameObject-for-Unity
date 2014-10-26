using UnityEngine;
using System.Collections;
using Unity.Linq; // using LINQ to GameObject

public class MainScene : MonoBehaviour
{
    GameObject cube;

    void Start()
    {
        cube = Resources.Load("Prefabs/PrefabCube") as GameObject;
    }

    void OnGUI()
    {
        if (GUILayout.Button("Builder"))
        {
            var builder = 
                new GameObjectBuilder(cube,
                    new GameObjectBuilder(cube),
                    new GameObjectBuilder(cube,
                        new GameObjectBuilder(cube)),
                    new GameObjectBuilder(cube));

            builder.Instantiate();
        }
    }
}