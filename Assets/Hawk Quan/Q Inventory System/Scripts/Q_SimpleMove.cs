using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QInventory
{
    [AddComponentMenu("Q Inventory/Simple Move")]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Q_SimpleMove : MonoBehaviour
    {
        public float myVelocity = 10f;
        //public float jump = 10f;
        public bool facingRight = true;
        private Vector2 velocity;
        private Vector2 moveDistance;

        void Update()
        {
            if (Input.GetKey(KeyCode.D))
            {
                velocity.x = myVelocity;
                if (!facingRight)
                    Flip();
            }

            if (Input.GetKey(KeyCode.A))
            {
                velocity.x = -myVelocity;
                if (facingRight)
                    Flip();
            }

            moveDistance = velocity * Time.deltaTime;
            transform.position += (Vector3)moveDistance;
            velocity = new Vector2(0, 0);
        }

        void Flip()
        {
            // Switch the way the player is labelled as facing.
            facingRight = !facingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }
}
