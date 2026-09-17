using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteAlways]
public class PhysicsLoadGenerator : MonoBehaviour
{
    [Header("Object")]
    [SerializeField]
    private GameObject prefab;

    [SerializeField, Min(0.05f)]
    private float objectScale = 1f;

    [Header("Generation")]
    [SerializeField, Min(1)]
    private int objectsPerStep = 50;

    [SerializeField, Min(0.05f)]
    private float baseSpacing = 1.2f;

    [SerializeField, Min(0.05f)]
    private float scaleStep = 0.05f;

    [SerializeField]
    private float spawnHeight = 3f;

    [SerializeField]
    private float startDistance = 3f;

    [Header("Rigidbody")]
    [SerializeField]
    private bool useGravity = true;

    [SerializeField]
    private bool isKinematic = false;

    [SerializeField, Min(0.001f)]
    private float mass = 1f;

    [Header("Physics Excitation")]
    [SerializeField, Min(0f)]
    private float impulseStrength = 2f;

    [Header("Runtime Input")]
    [SerializeField]
    private InputActionReference impulseAction;

    [SerializeField]
    private InputActionReference resetAction;

    [SerializeField]
    private InputActionReference increaseLoadAction;

    [SerializeField]
    private InputActionReference decreaseLoadAction;

    public int ObjectsPerStep => objectsPerStep;
    public float ObjectScale => objectScale;
    public float ScaleStep => scaleStep;
    public float ImpulseStrength => impulseStrength;

    private float EffectiveSpacing => baseSpacing * objectScale;

    private void OnEnable()
    {
        if (!Application.isPlaying)
            return;

        BindAction(impulseAction, OnImpulsePerformed);
        BindAction(resetAction, OnResetPerformed);
        BindAction(increaseLoadAction, OnIncreaseLoadPerformed);
        BindAction(decreaseLoadAction, OnDecreaseLoadPerformed);
    }

    private void OnDisable()
    {
        if (!Application.isPlaying)
            return;

        UnbindAction(impulseAction, OnImpulsePerformed);
        UnbindAction(resetAction, OnResetPerformed);
        UnbindAction(increaseLoadAction, OnIncreaseLoadPerformed);
        UnbindAction(decreaseLoadAction, OnDecreaseLoadPerformed);
    }

    private static void BindAction(
        InputActionReference actionReference,
        System.Action<InputAction.CallbackContext> callback)
    {
        if (actionReference == null)
            return;

        actionReference.action.performed += callback;
        actionReference.action.Enable();
    }

    private static void UnbindAction(
        InputActionReference actionReference,
        System.Action<InputAction.CallbackContext> callback)
    {
        if (actionReference == null)
            return;

        actionReference.action.performed -= callback;
        actionReference.action.Disable();
    }

    public void AddObjects()
    {
        if (prefab == null)
        {
            Debug.LogError(
                "PhysicsLoadGenerator: prefab is not assigned.",
                this
            );
            return;
        }

        int firstIndex = transform.childCount;

        for (int i = 0; i < objectsPerStep; i++)
        {
            GameObject instance;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                instance = (GameObject)
                    UnityEditor.PrefabUtility.InstantiatePrefab(
                        prefab,
                        transform
                    );

                UnityEditor.Undo.RegisterCreatedObjectUndo(
                    instance,
                    "Add physics load object"
                );
            }
            else
#endif
            {
                instance = Instantiate(prefab, transform);
            }

            int index = firstIndex + i;

            instance.name =
                $"PhysicsObject_{index + 1:0000}";

            instance.transform.localScale =
                Vector3.one * objectScale;

            EnsureRigidbody(instance);
        }

        ApplyScaleAndSpacing();
    }

    public void RemoveObjects()
    {
        int removeCount =
            Mathf.Min(objectsPerStep, transform.childCount);

        for (int i = 0; i < removeCount; i++)
        {
            int index = transform.childCount - 1;

            GameObject child =
                transform.GetChild(index).gameObject;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                UnityEditor.Undo.DestroyObjectImmediate(child);
            else
#endif
                Destroy(child);
        }

        ApplyScaleAndSpacing();
    }

    public void IncreaseScale()
    {
        objectScale += scaleStep;
        ApplyScaleAndSpacing();
    }

    public void DecreaseScale()
    {
        objectScale =
            Mathf.Max(0.05f, objectScale - scaleStep);

        ApplyScaleAndSpacing();
    }

    public void ApplyScaleAndSpacing()
    {
        int count = transform.childCount;

        if (count == 0)
            return;

        int side =
            Mathf.CeilToInt(Mathf.Sqrt(count));

        for (int i = 0; i < count; i++)
        {
            Transform child =
                transform.GetChild(i);

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.Undo.RecordObject(
                    child,
                    "Update physics load layout"
                );
            }
#endif

            child.localScale =
                Vector3.one * objectScale;

            int x = i % side;
            int z = i / side;

            child.localPosition =
                new Vector3(
                    (x - (side - 1) * 0.5f)
                        * EffectiveSpacing,
                    spawnHeight,
                    startDistance
                        + z * EffectiveSpacing
                );
        }
    }

    public void ApplyRigidbodySettings()
    {
        foreach (Transform child in transform)
        {
            Rigidbody rb =
                EnsureRigidbody(child.gameObject);

            ApplyRigidbodySettings(rb);
        }
    }

    public void ImpulseAll()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning(
                "Impulse All requires Play Mode.",
                this
            );
            return;
        }

        foreach (Transform child in transform)
        {
            Rigidbody rb =
                child.GetComponent<Rigidbody>();

            if (rb == null || rb.isKinematic)
                continue;

            Vector3 direction =
                new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(0.5f, 1f),
                    Random.Range(-1f, 1f)
                ).normalized;

            rb.WakeUp();

            rb.AddForce(
                direction * impulseStrength,
                ForceMode.Impulse
            );
        }
    }

    public void ResetExperiment()
    {
        foreach (Transform child in transform)
        {
            Rigidbody rb =
                child.GetComponent<Rigidbody>();

            if (rb == null)
                continue;

            if (Application.isPlaying)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        ApplyScaleAndSpacing();
    }

    public void ClearObjects()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject child =
                transform.GetChild(i).gameObject;

#if UNITY_EDITOR
            if (!Application.isPlaying)
                UnityEditor.Undo.DestroyObjectImmediate(child);
            else
#endif
                Destroy(child);
        }
    }

    private Rigidbody EnsureRigidbody(GameObject instance)
    {
        Rigidbody rb =
            instance.GetComponent<Rigidbody>();

        if (rb == null)
            rb = instance.AddComponent<Rigidbody>();

        ApplyRigidbodySettings(rb);

        return rb;
    }

    private void ApplyRigidbodySettings(Rigidbody rb)
    {
        rb.useGravity = useGravity;
        rb.isKinematic = isKinematic;
        rb.mass = mass;
    }

    private void OnImpulsePerformed(
        InputAction.CallbackContext context)
    {
        ImpulseAll();
    }

    private void OnResetPerformed(
        InputAction.CallbackContext context)
    {
        ResetExperiment();
    }

    private void OnIncreaseLoadPerformed(
        InputAction.CallbackContext context)
    {
        AddObjects();
    }

    private void OnDecreaseLoadPerformed(
        InputAction.CallbackContext context)
    {
        RemoveObjects();
    }
}
