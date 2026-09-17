using UnityEngine;

public class RenderLoadGenerator : MonoBehaviour
{
    [Header("Object")]
    [SerializeField]
    private GameObject prefab;

    [SerializeField, Min(0.1f)]
    private float objectScale = 1f;

    [Header("Generation")]
    [SerializeField, Min(1)]
    private int objectsPerStep = 100;

    [SerializeField, Min(0.1f)]
    private float baseSpacing = 1.2f;

    [SerializeField]
    private float startDistance = 4f;

    [Header("Scale Control")]
    [SerializeField, Min(0.1f)]
    private float scaleStep = 0.25f;

    public GameObject Prefab => prefab;
    public float ObjectScale => objectScale;
    public int ObjectsPerStep => objectsPerStep;
    public float ScaleStep => scaleStep;

    private float EffectiveSpacing => baseSpacing * objectScale;

    public void AddObjects()
    {
        if (prefab == null)
        {
            Debug.LogError(
                "RenderLoadGenerator: prefab is not assigned.",
                this
            );
            return;
        }

        int existingCount = transform.childCount;
        int targetCount = existingCount + objectsPerStep;
        int side = Mathf.CeilToInt(Mathf.Sqrt(targetCount));

        for (int i = existingCount; i < targetCount; i++)
        {
            int x = i % side;
            int z = i / side;

            Vector3 position = new Vector3(
                (x - side / 2f) * EffectiveSpacing,
                objectScale * 0.5f,
                startDistance + z * EffectiveSpacing
            );

            GameObject instance = Instantiate(
                prefab,
                position,
                Quaternion.identity,
                transform
            );

            instance.name = $"RenderTestObject_{i + 1:0000}";
            instance.transform.localScale = Vector3.one * objectScale;
        }

        RepositionAllObjects();

        Debug.Log(
            $"RenderLoadGenerator: {transform.childCount} objects, scale={objectScale:F2}.",
            this
        );
    }

    public void IncreaseScale()
    {
        objectScale += scaleStep;
        ApplyScaleToAllObjects();
    }

    public void DecreaseScale()
    {
        objectScale = Mathf.Max(0.1f, objectScale - scaleStep);
        ApplyScaleToAllObjects();
    }

    public void ApplyScaleToAllObjects()
    {
        foreach (Transform child in transform)
        {
            child.localScale = Vector3.one * objectScale;
        }

        RepositionAllObjects();
    }

    private void RepositionAllObjects()
    {
        int count = transform.childCount;

        if (count == 0)
            return;

        int side = Mathf.CeilToInt(Mathf.Sqrt(count));

        for (int i = 0; i < count; i++)
        {
            int x = i % side;
            int z = i / side;

            Transform child = transform.GetChild(i);

            child.localPosition = new Vector3(
                (x - side / 2f) * EffectiveSpacing,
                objectScale * 0.5f,
                startDistance + z * EffectiveSpacing
            );
        }
    }

    public void ClearObjects()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;

            if (Application.isPlaying)
                Destroy(child);
            else
                DestroyImmediate(child);
        }
    }
}
