using System.Collections.Generic;
using UnityEngine;

namespace PixelGameAssets.Scripts.Actor.ControllerSys
{
    [RequireComponent(typeof(Collider2D))]
    public class PhysicalObject2DDebug : MonoBehaviour
    {
        private const float MIN_MOVE_DISTANCE = 0.001f;

        private Collider2D _collider2D;
        public Rigidbody2D Rigidbody2D;
        private ContactFilter2D _contactFilter2D;
        private readonly List<RaycastHit2D> _raycastHit2DList = new List<RaycastHit2D>();
        private readonly List<RaycastHit2D> _tangentRaycastHit2DList = new List<RaycastHit2D>();
        
        private readonly List<RaycastHit2D> _raycastHit2DList2 = new List<RaycastHit2D>();
        private readonly List<RaycastHit2D> _tangentRaycastHit2DList2 = new List<RaycastHit2D>();

        public LayerMask layerMask;

        [HideInInspector] public Vector2 velocity;

        protected void Awake()
        {
            _collider2D = GetComponent<Collider2D>();
            Rigidbody2D = GetComponent<Rigidbody2D>();

            if (Rigidbody2D == null)
                Rigidbody2D = gameObject.AddComponent<Rigidbody2D>();

            Rigidbody2D.hideFlags = HideFlags.NotEditable;
            Rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            Rigidbody2D.simulated = true;
            Rigidbody2D.useFullKinematicContacts = false;
            Rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            Rigidbody2D.sleepMode = RigidbodySleepMode2D.NeverSleep;
            Rigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
            Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            Rigidbody2D.gravityScale = 0;

            _contactFilter2D = new ContactFilter2D
            {
                useLayerMask = true,
                useTriggers = false,
                layerMask = layerMask
            };
        }

        private void Update()
        {
            velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        private void FixedUpdate()
        {
            Movement(velocity * Time.deltaTime * 5f);
        }
        
        private void OnValidate()
        {
            _contactFilter2D.layerMask = layerMask;
        }

        protected void Movement(Vector2 deltaPosition)
        {
            MovementX(deltaPosition.x);
            MovementY(deltaPosition.y);
        }

        protected void MovementX(float moveX)
        {
            Vector2 updateDeltaPosition = Vector2.zero;
            Vector2 deltaPosition = new Vector2(moveX, 0);

            float distance = deltaPosition.magnitude;
            Vector2 direction = deltaPosition.normalized;

            if (distance <= MIN_MOVE_DISTANCE)
                distance = MIN_MOVE_DISTANCE;

            Rigidbody2D.Cast(direction, _contactFilter2D, _raycastHit2DList, distance);

            Vector2 finalDirection = direction;
            float finalDistance = distance;

            foreach (var hit in _raycastHit2DList)
            {
                float moveDistance = hit.distance;

                Debug.DrawLine(hit.point, hit.point + hit.normal, Color.white);
                Debug.DrawLine(hit.point, hit.point + direction, Color.yellow);

                float projection = Vector2.Dot(hit.normal, direction);

                if (projection >= 0)
                {
                    moveDistance = distance;
                }
                else
                {
                    Vector2 tangentDirection = new Vector2(hit.normal.y, -hit.normal.x);

                    float tangentDot = Vector2.Dot(tangentDirection, direction);

                    if (tangentDot < 0)
                    {
                        tangentDirection = -tangentDirection;
                        tangentDot = -tangentDot;
                    }

                    float tangentDistance = tangentDot * distance;

                    if (tangentDot != 0)
                    {
                        Rigidbody2D.Cast(tangentDirection, _contactFilter2D, _tangentRaycastHit2DList, tangentDistance);

                        foreach (var tangentHit in _tangentRaycastHit2DList)
                        {
                            Debug.DrawLine(tangentHit.point, tangentHit.point + tangentDirection, Color.magenta);

                            if (Vector2.Dot(tangentHit.normal, tangentDirection) >= 0)
                                continue;

                            if (tangentHit.distance < tangentDistance)
                                tangentDistance = tangentHit.distance;
                        }

                        updateDeltaPosition += tangentDirection * tangentDistance;
                    }
                }

                if (moveDistance < finalDistance)
                {
                    finalDistance = moveDistance;
                }
            }

            updateDeltaPosition += finalDirection * finalDistance;
            Rigidbody2D.position += updateDeltaPosition;
        }

        protected void MovementY(float moveY)
        {
            Vector2 updateDeltaPosition = Vector2.zero;
            Vector2 deltaPosition = new Vector2(0, moveY);

            float distance = deltaPosition.magnitude;
            Vector2 direction = deltaPosition.normalized;

            if (distance <= MIN_MOVE_DISTANCE)
                distance = MIN_MOVE_DISTANCE;

            Rigidbody2D.Cast(direction, _contactFilter2D, _raycastHit2DList2, distance);

            Vector2 finalDirection = direction;
            float finalDistance = distance;

            foreach (var hit in _raycastHit2DList2)
            {
                float moveDistance = hit.distance;

                Debug.DrawLine(hit.point, hit.point + hit.normal, Color.white);
                Debug.DrawLine(hit.point, hit.point + direction, Color.yellow);

                float projection = Vector2.Dot(hit.normal, direction);

                if (projection >= 0)
                {
                    moveDistance = distance;
                }
                else
                {
                    Vector2 tangentDirection = new Vector2(hit.normal.y, -hit.normal.x);

                    float tangentDot = Vector2.Dot(tangentDirection, direction);

                    if (tangentDot < 0)
                    {
                        tangentDirection = -tangentDirection;
                        tangentDot = -tangentDot;
                    }

                    float tangentDistance = tangentDot * distance;

                    if (tangentDot != 0)
                    {
                        Rigidbody2D.Cast(tangentDirection, _contactFilter2D, _tangentRaycastHit2DList2, tangentDistance);

                        foreach (var tangentHit in _tangentRaycastHit2DList2)
                        {
                            Debug.DrawLine(tangentHit.point, tangentHit.point + tangentDirection, Color.magenta);

                            if (Vector2.Dot(tangentHit.normal, tangentDirection) >= 0)
                                continue;

                            if (tangentHit.distance < tangentDistance)
                                tangentDistance = tangentHit.distance;
                        }

                        updateDeltaPosition += tangentDirection * tangentDistance;
                    }
                }

                if (moveDistance < finalDistance)
                {
                    finalDistance = moveDistance;
                }
            }

            updateDeltaPosition += finalDirection * finalDistance;
            Rigidbody2D.position += updateDeltaPosition;
        }
    }
}