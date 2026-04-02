using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSystem : MonoBehaviour
{
    [Header("Rope")]
    [SerializeField] private byte _numberOfRopeSegments = 50;
    [SerializeField] private float _ropeSegmentLength = 0.225f;

    [Header("Physics")]
    [SerializeField] private Vector2 _gravityForce = new Vector2(0f, 2f);
    [SerializeField] private float _dampingFactor = 0.98f;

    [Header("Constraints")]
    [SerializeField] private byte _numberOfConstraintRuns = 50;

    private LineRenderer _lineRenderer;
    private List<RopeSegment> _segments;
    private Vector3 _ropeStartingPoint;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = _numberOfRopeSegments;
        _ropeStartingPoint = transform.position;
    }

}

public struct RopeSegment
{
    public Vector2 CurrentPosition;
    public Vector2 OldPosition;

    public RopeSegment(Vector2 inPos)
    {
        CurrentPosition = inPos;
        OldPosition = inPos;
    }
}