using System.Collections.Generic;
using UnityEngine;

public class PatrolPath : MonoBehaviour
{
    [SerializeField] private Transform[] points;

    public IReadOnlyList<Transform> Points
        => points == null ? System.Array.Empty<Transform>() : points;
}