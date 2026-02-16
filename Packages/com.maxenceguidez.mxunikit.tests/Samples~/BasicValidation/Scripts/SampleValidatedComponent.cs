using UnityEngine;
using MxUnikit.Tests;

/// <summary>
/// Example component demonstrating RequiredFieldAttribute usage.
/// This script shows how to mark fields as required for validation.
/// </summary>
public class SampleValidatedComponent : MonoBehaviour
{
    [Header("Required Fields - Must be assigned")]
    [SerializeField, RequiredField] private Transform _targetTransform;
    [SerializeField, RequiredField] private Rigidbody _physicsBody;

    [Header("Optional Fields - Not validated")]
    [SerializeField] private Material _optionalMaterial;
    [SerializeField] private int _optionalValue = 10;

    private void Start()
    {
        Debug.Log($"Component initialized with target: {_targetTransform.name}");
    }
}
