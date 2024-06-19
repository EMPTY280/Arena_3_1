using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace ArenaGame
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EntityMovement : ArenaBaseObjectMono
    {
        private Rigidbody2D rb;

        private float speedStat = 10f;
        [SerializeField][Min(0f)] private float moveSpeedAccel = 3f;
        [SerializeField][Min(0f)] private float moveSpeedMax = 10f;
        [SerializeField][Min(0f)] private float moveDrag = 1f;
        [SerializeField] private bool inverse = false;

        public float MoveSpeed
        {
            get
            {
                return moveSpeedMax;
            }
            set
            {
                speedStat = value;
                float speedClamp = Mathf.Clamp(speedStat, 0f, 60f);
                moveSpeedMax = speedClamp;
                moveSpeedAccel = speedClamp * 6;
                moveDrag = Mathf.Max(1f, moveSpeedAccel * 0.2f);
            }
        }

        public float TrueMoveSpeed
        {
            get { return speedStat; }
        }

        private void Awake()
        {
            if (!TryGetComponent(out rb))
                enabled = false;
            InitRigibbody();
            speedStat = moveSpeedMax;
        }

        // Initializeing RB2D
        private void InitRigibbody()
        {
            rb.gravityScale = 0f;
            rb.drag = moveDrag;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.freezeRotation = true;
        }

        /// <summary>
        /// Add force to direction
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="deltaTime"></param>
        public void AddForce(Vector2 dir, float deltaTime)
        {
            /*rb.AddForce(dir * moveSpeedAccel * deltaTime, ForceMode2D.Impulse);
            if (rb.velocity.magnitude > moveSpeedMax)
                rb.velocity = rb.velocity.normalized * moveSpeedMax;*/
            float powerBuffer = rb.velocity.magnitude;

            Vector2 newVelocity = rb.velocity + dir * moveSpeedAccel * deltaTime * (inverse ? -1 : 1);
            Vector2 newDir = newVelocity.normalized;
            float newPower = newVelocity.magnitude;

            Vector2 finalVelocity = Vector2.zero;
            if (powerBuffer >= moveSpeedMax)
                finalVelocity = newDir * powerBuffer;
            else
            {
                if (newPower > moveSpeedMax)
                    newPower = moveSpeedMax;
                finalVelocity = newDir * newPower;
            }

            rb.velocity = finalVelocity;
        }

        /// <summary>
        /// Set movement drag is enabled
        /// </summary>
        /// <param name="b"></param>
        public void SetDragEnabled(bool b)
        {
            if (b)
                rb.drag = moveDrag;
            else
                rb.drag = 0f;
        }
    }

}