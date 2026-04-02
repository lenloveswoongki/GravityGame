using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GogoGaga.OptimizedRopesAndCables
{
    [ExecuteAlways]
    [RequireComponent(typeof(LineRenderer))]
    public class Rope : MonoBehaviour
    {
        public event Action OnPointsChanged;

        [Header("Rope Transforms")]
        [SerializeField] private Transform startPoint;
        public Transform StartPoint => startPoint;

        [SerializeField] private Transform midPoint;
        public Transform MidPoint => midPoint;

        [SerializeField] private Transform endPoint;
        public Transform EndPoint => endPoint;

        [Header("Rope Settings")]
        [Range(2, 100)] public int linePoints = 10;
        public float stiffness = 350f;
        public float damping = 15f;
        public float ropeLength = 15;
        public float ropeWidth = 0.1f;

        [Header("Rational Bezier Weight Control")]
        [Range(1, 15)] public float midPointWeight = 1f;
        private const float StartPointWeight = 1f;
        private const float EndPointWeight = 1f;

        [Header("Midpoint Position")]
        [Range(0f, 1f)] public float midPointPosition = 0.5f;

        [Header("Gravity Source")]
        [SerializeField] private bool use2DGravity = true;
        [SerializeField] private Vector2 customGravity2D = new Vector2(0f, -9.81f);

        [Header("Motion")]
        [Range(0.1f, 60f)] public float maxMidPointSpeed = 12f;

        private Vector3 currentValue;
        private Vector3 currentVelocity;
        private Vector3 targetValue;

        public Vector3 otherPhysicsFactors { get; set; }

        private const float valueThreshold = 0.01f;
        private const float velocityThreshold = 0.01f;

        private LineRenderer lineRenderer;
        private bool isFirstFrame = true;
        private bool hasInitializedMid = false;

        private Vector3 prevStartPointPosition;
        private Vector3 prevEndPointPosition;
        private float prevMidPointPosition;
        private float prevMidPointWeight;

        private float prevLineQuality;
        private float prevRopeWidth;
        private float prevstiffness;
        private float prevDampness;
        private float prevRopeLength;

        public bool IsPrefab => gameObject.scene.rootCount == 0;

        private void Start()
        {
            InitializeLineRenderer();

            if (AreEndPointsValid())
            {
                InitializeMidValue();
                SetSplinePoint();
            }
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                InitializeLineRenderer();

                if (AreEndPointsValid())
                {
                    RecalculateRope();
                    SimulatePhysics();
                }
                else
                {
                    if (lineRenderer) lineRenderer.positionCount = 0;
                }
            }
        }

        private void InitializeLineRenderer()
        {
            if (!lineRenderer)
            {
                lineRenderer = GetComponent<LineRenderer>();
            }

            lineRenderer.startWidth = ropeWidth;
            lineRenderer.endWidth = ropeWidth;
        }

        private void Update()
        {
            if (IsPrefab) return;
            if (!AreEndPointsValid()) return;

            SetSplinePoint();

            if (!Application.isPlaying && (IsPointsMoved() || IsRopeSettingsChanged()))
            {
                SimulatePhysics();
                NotifyPointsChanged();
            }

            prevStartPointPosition = startPoint.position;
            prevEndPointPosition = endPoint.position;
            prevMidPointPosition = midPointPosition;
            prevMidPointWeight = midPointWeight;

            prevLineQuality = linePoints;
            prevRopeWidth = ropeWidth;
            prevstiffness = stiffness;
            prevDampness = damping;
            prevRopeLength = ropeLength;
        }

        private bool AreEndPointsValid()
        {
            return startPoint != null && endPoint != null;
        }

        private void InitializeMidValue()
        {
            Vector3 baseMid = GetBaseMidPoint();
            currentValue = baseMid;
            targetValue = baseMid;
            currentVelocity = Vector3.zero;
            hasInitializedMid = true;
        }

        private void SetSplinePoint()
        {
            if (!hasInitializedMid)
                InitializeMidValue();

            if (lineRenderer.positionCount != linePoints + 1)
                lineRenderer.positionCount = linePoints + 1;

            if (Application.isPlaying)
            {
                Vector3 g = GetGravityVector3();
                if (g.sqrMagnitude < 0.000001f)
                {
                    targetValue = currentValue;
                }
                else
                {
                    Vector3 baseMid = GetBaseMidPoint();
                    Vector3 gravityOffset = GetGravityOffset(baseMid);
                    targetValue = baseMid + gravityOffset;
                }
            }
            else
            {
                Vector3 baseMid = GetBaseMidPoint();
                targetValue = baseMid;
                currentValue = baseMid;
                currentVelocity = Vector3.zero;
            }

            Vector3 mid = currentValue;

            if (midPoint != null)
            {
                midPoint.position = GetRationalBezierPoint(startPoint.position, mid, endPoint.position, midPointPosition, StartPointWeight, midPointWeight, EndPointWeight);
            }

            for (int i = 0; i < linePoints; i++)
            {
                Vector3 p = GetRationalBezierPoint(startPoint.position, mid, endPoint.position, i / (float)linePoints, StartPointWeight, midPointWeight, EndPointWeight);
                lineRenderer.SetPosition(i, p);
            }

            lineRenderer.SetPosition(linePoints, endPoint.position);
        }

        private float CalculateYFactorAdjustment(float weight)
        {
            float k = Mathf.Lerp(0.493f, 0.323f, Mathf.InverseLerp(1, 15, weight));
            float w = 1f + k * Mathf.Log(weight);
            return w;
        }

        private float GetSagAmount()
        {
            Vector3 a = startPoint.position;
            Vector3 b = endPoint.position;

            float dist = Vector3.Distance(a, b);
            float slack = Mathf.Max(0f, ropeLength - dist);
            return slack / CalculateYFactorAdjustment(midPointWeight);
        }

        private Vector3 GetBaseMidPoint()
        {
            Vector3 a = startPoint.position;
            Vector3 b = endPoint.position;

            Vector3 mid = Vector3.Lerp(a, b, midPointPosition);
            float sag = GetSagAmount();

            mid += Vector3.down * sag;

            return mid;
        }

        private Vector3 GetGravityVector3()
        {
            Vector2 g2 = use2DGravity ? Physics2D.gravity : customGravity2D;
            return new Vector3(g2.x, g2.y, 0f);
        }

        private Vector3 GetGravityOffset(Vector3 baseMid)
        {
            Vector3 g = GetGravityVector3();
            if (g.sqrMagnitude < 0.000001f) return Vector3.zero;

            float sag = GetSagAmount();
            float gravityScale = Mathf.Clamp01(g.magnitude / 9.81f);
            return g.normalized * (sag * gravityScale);
        }

        private Vector3 GetRationalBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t, float w0, float w1, float w2)
        {
            Vector3 wp0 = w0 * p0;
            Vector3 wp1 = w1 * p1;
            Vector3 wp2 = w2 * p2;

            float denominator = w0 * Mathf.Pow(1 - t, 2) + 2 * w1 * (1 - t) * t + w2 * Mathf.Pow(t, 2);
            Vector3 point = (wp0 * Mathf.Pow(1 - t, 2) + wp1 * 2 * (1 - t) * t + wp2 * Mathf.Pow(t, 2)) / denominator;

            return point;
        }

        public Vector3 GetPointAt(float t)
        {
            if (!AreEndPointsValid())
            {
                Debug.LogError("StartPoint or EndPoint is not assigned.", gameObject);
                return Vector3.zero;
            }

            return GetRationalBezierPoint(startPoint.position, currentValue, endPoint.position, t, StartPointWeight, midPointWeight, EndPointWeight);
        }

        private void FixedUpdate()
        {
            if (IsPrefab) return;
            if (!AreEndPointsValid()) return;

            if (!isFirstFrame)
                SimulatePhysics();

            isFirstFrame = false;
        }

        private void SimulatePhysics()
        {
            if (!hasInitializedMid)
                InitializeMidValue();

            if (Application.isPlaying)
            {
                Vector3 g = GetGravityVector3();
                if (g.sqrMagnitude < 0.000001f)
                {
                    targetValue = currentValue;
                    currentVelocity = Vector3.zero;
                    return;
                }
            }

            float dampingFactor = Mathf.Max(0, 1 - damping * Time.fixedDeltaTime);
            Vector3 accel = (targetValue - currentValue) * stiffness * Time.fixedDeltaTime;

            currentVelocity = currentVelocity * dampingFactor + accel + otherPhysicsFactors;

            float maxVel = Mathf.Max(0.001f, maxMidPointSpeed);
            float vMag = currentVelocity.magnitude;
            if (vMag > maxVel)
                currentVelocity = currentVelocity * (maxVel / vMag);

            currentValue += currentVelocity * Time.fixedDeltaTime;

            if (Vector3.Distance(currentValue, targetValue) < valueThreshold && currentVelocity.magnitude < velocityThreshold)
            {
                currentValue = targetValue;
                currentVelocity = Vector3.zero;
            }
        }

        public void SetStartPoint(Transform newStartPoint, bool instantAssign = false)
        {
            startPoint = newStartPoint;
            prevStartPointPosition = startPoint == null ? Vector3.zero : startPoint.position;

            if (instantAssign || newStartPoint == null)
                RecalculateRope();

            NotifyPointsChanged();
        }

        public void SetMidPoint(Transform newMidPoint, bool instantAssign = false)
        {
            midPoint = newMidPoint;
            prevMidPointPosition = midPoint == null ? 0.5f : midPointPosition;

            if (instantAssign || newMidPoint == null)
                RecalculateRope();

            NotifyPointsChanged();
        }

        public void SetEndPoint(Transform newEndPoint, bool instantAssign = false)
        {
            endPoint = newEndPoint;
            prevEndPointPosition = endPoint == null ? Vector3.zero : endPoint.position;

            if (instantAssign || newEndPoint == null)
                RecalculateRope();

            NotifyPointsChanged();
        }

        public void RecalculateRope()
        {
            if (!AreEndPointsValid())
            {
                if (lineRenderer) lineRenderer.positionCount = 0;
                return;
            }

            if (!hasInitializedMid)
            {
                InitializeMidValue();
            }
            else
            {
                if (!Application.isPlaying)
                {
                    Vector3 baseMid = GetBaseMidPoint();
                    currentValue = baseMid;
                    targetValue = baseMid;
                    currentVelocity = Vector3.zero;
                }
                else
                {
                    Vector3 g = GetGravityVector3();
                    if (g.sqrMagnitude >= 0.000001f)
                    {
                        Vector3 baseMid = GetBaseMidPoint();
                        targetValue = baseMid + GetGravityOffset(baseMid);
                    }
                }
            }

            SetSplinePoint();
        }

        private void NotifyPointsChanged()
        {
            OnPointsChanged?.Invoke();
        }

        private bool IsPointsMoved()
        {
            var startMoved = startPoint.position != prevStartPointPosition;
            var endMoved = endPoint.position != prevEndPointPosition;
            return startMoved || endMoved;
        }

        private bool IsRopeSettingsChanged()
        {
            var lineQualityChanged = !Mathf.Approximately(linePoints, prevLineQuality);
            var ropeWidthChanged = !Mathf.Approximately(ropeWidth, prevRopeWidth);
            var stiffnessChanged = !Mathf.Approximately(stiffness, prevstiffness);
            var dampnessChanged = !Mathf.Approximately(damping, prevDampness);
            var ropeLengthChanged = !Mathf.Approximately(ropeLength, prevRopeLength);
            var midPointPositionChanged = !Mathf.Approximately(midPointPosition, prevMidPointPosition);
            var midPointWeightChanged = !Mathf.Approximately(midPointWeight, prevMidPointWeight);

            return lineQualityChanged
                   || ropeWidthChanged
                   || stiffnessChanged
                   || dampnessChanged
                   || ropeLengthChanged
                   || midPointPositionChanged
                   || midPointWeightChanged;
        }
    }
}