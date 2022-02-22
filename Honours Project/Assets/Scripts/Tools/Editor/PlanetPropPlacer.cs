using UnityEngine;
using UnityEditor;

public class PlanetPropPlacer : EditorWindow
{
    PlanetGravity source;
    PropScheme scheme;
    int numberOfProps;
    string parentName;
    bool checkCollision = true;

    int progress = 0;
    float radius = 0;
    Transform parent;

    [MenuItem("Custom Tools/Planet Prop Placer")]
    public static void ShowWindow()
    {
        GetWindow<PlanetPropPlacer>("Planet Prop Placer");
    }

    private void OnGUI()
    {
        source = (PlanetGravity)EditorGUILayout.ObjectField("Planet", source, typeof(PlanetGravity), true);

        if (source == null) return;

        scheme = (PropScheme)EditorGUILayout.ObjectField("Prop Scheme", scheme, typeof(PropScheme), false);

        if (scheme == null) return;

        parentName = EditorGUILayout.TextField("Prop Holder", parentName);

        if (parentName == "") return;

        numberOfProps = EditorGUILayout.IntField("Number of Props", numberOfProps);

        if (numberOfProps <= 0) return;

        checkCollision = EditorGUILayout.Toggle("Check for Collision", checkCollision);

        if (GUILayout.Button("Generate Props"))
        {
            GenerateProps();
        }

        GUILayout.Label("Progress: " + (progress / numberOfProps * 100) + "%");
    }

    void GenerateProps()
    {
        progress = 0;
        scheme.CalculateWeights();
        radius = source.GetDistance() + 10;
        CheckParent();
        while (progress < numberOfProps)
        {
            PlaceableProp prop = scheme.GetRandomProp();
            if (prop == null) continue;

            Vector3 pos = FindPosition();

            if (pos != Vector3.zero)
            {
                Vector3 rot = GetRotation(pos);

                prop.Spawn(parent, pos, rot);
            }

            progress++;
        }
    }

    void CheckParent()
    {
        Transform propHolder = source.transform.Find("Generated Props");
        if(propHolder == null)
        {
            propHolder = CreateParent(source.transform, "Generated Props").transform;
        }

        parent = propHolder.Find(parentName);
        if(parent == null)
        {
            parent = CreateParent(propHolder, parentName).transform;
        }
    }

    GameObject CreateParent(Transform originalParent, string name)
    {
        GameObject gameObject = new GameObject(name);
        gameObject.transform.parent = originalParent;
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localEulerAngles = Vector3.zero;
        gameObject.transform.localScale = Vector3.one;

        return gameObject;
    }

    Vector3 FindPosition()
    {
        int numberOfAttempts = 50;
        while(numberOfAttempts > 0)
        {
            numberOfAttempts--;

            Vector3 dir = Random.onUnitSphere * radius;

            Vector3 pos = source.transform.position + dir;

            if (!Physics.Raycast(pos, -dir, out RaycastHit hit, radius)) continue;
            

            if (hit.collider.gameObject.layer != 8 && checkCollision) continue;

            return hit.point;
        }

        Debug.Log("COULDN'T PLACE PROP");
        return Vector3.zero;
    }

    Vector3 GetRotation(Vector3 pos)
    {
        return (pos - source.transform.position);
    }
}
