using RenownedGames.AITree.PerceptionSystem;
using RenownedGames.Apex;
using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Malbers.Integration.AITree
{
    [SearchContent("LookConfig", Image = "Images/Icons/Perception/EyeIcon.png")]
    public class LookConfig : AIPerceptionConfig
    {
        [Tooltip("Angle of Vision of the Animal")]
        [Range(0, 360)]
        public float lookAngle = 120;
        [Tooltip("Range for Looking forward and Finding something")]
        public float lookRange = 15;

        public Color gizmoColor = new Color(0.5f, 0f, 0f, 0.1f);
        public Vector3 offset = new Vector3(0, 1, 0);
        public LayerMask obstacleLayerMask;

        public bool useDisc = true;
        public bool useConeRays = true;

        [Range(0, 90)]
        public float halfAngle = 45f;
        [Range(-180, 180)]
        public float coneDirectionAngle = 0f;
        [Range(10, 360)]
        public int resolution = 10;

        public LayerMask playerLayer;
        public LayerMask obstacleLayer;

        private List<Transform> visibleTransforms = new List<Transform>();

        public override event Action<AIPerceptionSource> OnTargetUpdated;

        public List<Transform> GetVisibleTransforms()
        {
            return visibleTransforms;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();


        }

        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            if (useDisc)
            {
                AIPerceptionSource target = CheckDiscVisibility();
                OnTargetUpdated?.Invoke(target);

                //CheckDiscVisibility();
            }
            if (useConeRays)
            {
                AIPerceptionSource target = CheckRayConeVisibility();
                OnTargetUpdated?.Invoke(target);

                //CheckRayConeVisibility();
            }
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            if (useDisc)
            {
                DrawDisc();
            }
            if (useConeRays)
            {
                DrawRays();
            }
        }



        private AIPerceptionSource CheckDiscVisibility()
        {
            Vector3 center = GetOwner().transform.position + offset;

            Vector3 forward = GetOwner().transform.forward;


            Vector3 eyesForward = Vector3.ProjectOnPlane(forward, Vector3.up);
            Vector3 rotatedForward = Quaternion.Euler(0, 0, -lookAngle * 0.5f) * eyesForward;

            Collider[] colliders = Physics.OverlapSphere(center, lookRange, playerLayer);

            foreach (Collider collider in colliders)
            {
                if (!Physics.Linecast(center, collider.transform.position, obstacleLayer))
                {
                    Vector3 toCollider = (collider.transform.position - center).normalized;

                    float angle = Vector3.Angle(rotatedForward, toCollider);
                    if (angle <= lookAngle * 0.5f && Vector3.Dot(toCollider, forward) > 0)
                    {
                        AIPerceptionSource target = collider.GetComponentInParent<AIPerceptionSource>();
                        if (target != null)
                        {
                            //Debug.Log("Target: " + target.name);

                            visibleTransforms.Add(collider.transform);
                            return target;
                        }
                    }
                }
            }
            return null;
        }

        private void DrawDisc()
        {
            Vector3 center = GetOwner().transform.position + offset;
            Vector3 forward = GetOwner().transform.forward;

            Color c = gizmoColor;
            c.a = 0.2f;
#if UNITY_EDITOR
            UnityEditor.Handles.color = Color.cyan;
            UnityEditor.Handles.DrawLine(GetOwner().transform.position, GetOwner().transform.position + forward);
            Vector3 eyesForward = Vector3.ProjectOnPlane(forward, Vector3.up);
            Vector3 rotatedForward = Quaternion.Euler(0, -lookAngle * 0.5f, 0) * eyesForward;

            UnityEditor.Handles.color = c;
            UnityEditor.Handles.DrawWireArc(center, Vector3.up, rotatedForward, lookAngle, lookRange);
            UnityEditor.Handles.color = gizmoColor;
            UnityEditor.Handles.DrawSolidArc(center, Vector3.up, rotatedForward, lookAngle, lookRange);
#endif
        }


        private AIPerceptionSource CheckRayConeVisibility()
        {
            RayRepresentation[] rayRepresentations = GenerateRays();
            Ray[] rays = new Ray[rayRepresentations.Length];

            for (int i = 0; i < rayRepresentations.Length; i++)
            {
                rays[i] = (Ray)rayRepresentations[i];
            }

            Dictionary<Ray, RaycastHit> hits = GetHitInfo(rays);

            foreach (RaycastHit hit in hits.Values)
            {
                if (((1 << hit.transform.gameObject.layer) & playerLayer) != 0)
                {
                    AIPerceptionSource target = hit.collider.GetComponentInParent<AIPerceptionSource>();
                    if (target != null)
                    {
                        visibleTransforms.Add(hit.collider.transform);

                        //Debug.Log("Target: " + target.name);
                        return target;
                    }
                }
            }
            return null;
        }

        private void DrawRays()
        {

            RayRepresentation[] rayRepresentations = GenerateRays();
            Ray[] rays = new Ray[rayRepresentations.Length];

            for (int i = 0; i < rayRepresentations.Length; i++)
            {
                rays[i] = (Ray)rayRepresentations[i];
            }

            Dictionary<Ray, RaycastHit> hits = new Dictionary<Ray, RaycastHit>();
            hits = GetHitInfo(rays);

            for (int i = 0; i < rays.Length; i++)
            {
                RayRepresentation rayRepresentation = rayRepresentations[i];
                Ray ray = rays[i];

                if (hits.ContainsKey(ray))
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = gizmoColor;
                }
                Gizmos.DrawRay(rayRepresentation.start, rayRepresentation.direction);
            }
        }

        private Dictionary<Ray, RaycastHit> GetHitInfo(Ray[] rays)
        {
            Dictionary<Ray, RaycastHit> hits = new Dictionary<Ray, RaycastHit>();
            for (int i = 0; i < rays.Length; i++)
            {
                RaycastHit hitInfo;

                if (Physics.Raycast(rays[i], out hitInfo, lookRange))
                {
                    hits.Add(rays[i], hitInfo);
                }
            }
            return hits;
        }

        private RayRepresentation[] GenerateRays()
        {
            Quaternion offsetRotation = Quaternion.AngleAxis(coneDirectionAngle, GetOwner().transform.right);
            Quaternion rotation = offsetRotation * GetOwner().transform.rotation;

            float radius = Mathf.Tan(Mathf.Deg2Rad * halfAngle);

            Vector2[] points = SunflowerDistribution(radius, resolution, 2);
            Vector3 rotatedOffset = GetOwner().transform.rotation * offset;

            RayRepresentation[] rays = new RayRepresentation[points.Length];

            for (int i = 0; i < rays.Length; i++)
            {
                Vector3 modified = GetOwner().transform.position + (rotation * (Vector3.forward + (Vector3)points[i]));
                Vector3 direction = (modified + rotatedOffset - (GetOwner().transform.position + rotatedOffset)) * lookRange;
                rays[i] = new RayRepresentation(GetOwner().transform.position + rotatedOffset, direction);
            }
            return rays;
        }

        private Vector2[] SunflowerDistribution(float radius, int numPoints, int alpha)
        {
            int b = Mathf.RoundToInt(alpha * Mathf.Sqrt(numPoints));
            float phi = (Mathf.Sqrt(5) + 1f) / 2f;

            Vector2[] values = new Vector2[numPoints];

            for (int i = 1; i <= numPoints; i++)
            {
                float r = GetRadius(i, numPoints, b);
                float theta = 2 * Mathf.PI * i / Mathf.Pow(phi, 2);

                values[i - 1] = new Vector2(r * Mathf.Cos(theta), r * Mathf.Sin(theta)) * radius;
            }
            return values;
        }

        private float GetRadius(int k, int n, int b)
        {
            if (k > n - b)
            {
                return 1.0f;
            }
            return Mathf.Sqrt(k - 0.5f) / Mathf.Sqrt(n - ((b + 1) / 2f));
        }

        private struct RayRepresentation
        {
            public readonly Vector3 start;
            public Vector3 direction;

            public RayRepresentation(Vector3 start, Vector3 direction)
            {
                this.start = start;
                this.direction = direction;
            }

            public static explicit operator Ray(RayRepresentation rayRepresentation)
            {
                return new Ray(rayRepresentation.start, rayRepresentation.direction.normalized);
            }
        }



    }
}
