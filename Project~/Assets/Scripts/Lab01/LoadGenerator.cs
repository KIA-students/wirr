// LoadGenerator jest sztucznym obciążeniem CPU i nie reprezentuje typowego błędu produkcyjnego. Służy tylko do kontrolowanego eksperymentu.

using UnityEngine;

public class LoadGenerator : MonoBehaviour
{
    [SerializeField, Min(0)]
    private int iterationsPerFrame = 0;
    [SerializeField] bool addLoad;

    private float sink;

    private void Update()
    {
        float value = sink;

        if (addLoad)
        {
            iterationsPerFrame += 1000;
            addLoad = false;
        }

        for (int i = 0; i < iterationsPerFrame; i++)
        {
            value += Mathf.Sqrt(i + 1f) * Mathf.Sin(i);
        }

        sink = value;
    }
}
