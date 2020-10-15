using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actors
{
    public class Actor : MonoBehaviour
    {
        //floats
        public float moveSpeed = 1200;
        public float maxMoveSpeed = 10;
        private float currentJumps;
        public float maxJumps = 2;
        public float jumpForce;

        //bools
        private bool grounded;
        private bool moving;
        private bool facingRight;

        //layermasks
        public LayerMask whatIsGround;

        //transforms
        public Transform groundCheckPos;

        //gameobjects
        public GameObject gun;

        //components
        private Rigidbody2D rb;
        private SpriteRenderer sr;

        protected void ActorStart()
        {
            rb = GetComponent<Rigidbody2D>();
            sr = GetComponent<SpriteRenderer>();
        }

        private void FixedUpdate()
        {
            SlowDown();

            //reset jumps
            if (currentJumps < maxJumps && grounded)
                currentJumps = maxJumps;
        }

        private void LateUpdate()
        {
            grounded = Physics2D.OverlapCircle(groundCheckPos.position, 0.6f, whatIsGround);
        }

        private void SlowDown()
        {
            if (moving)
                return;

            float velX = rb.velocity.x;

            if (velX > 0.6f)
                rb.AddForce(moveSpeed * Time.deltaTime * -Vector2.right);
            else if (velX < -0.6f)
                rb.AddForce(moveSpeed * Time.deltaTime * Vector2.right);
        }

        protected void Move(short dir)
        {
            Flip(dir);
            moving = true;

            float velX = rb.velocity.x;

            if (velX < maxMoveSpeed && dir > 0)
                rb.AddForce(moveSpeed * Time.deltaTime * Vector2.right * dir);
            if (velX > -maxMoveSpeed && dir < 0)
                rb.AddForce(moveSpeed * Time.deltaTime * Vector2.right * dir);
            if (velX > 0.2f && dir < 0)
                rb.AddForce(moveSpeed * 3.5f * Time.deltaTime * -Vector2.right);
            if (velX < -0.2f && dir > 0)
                rb.AddForce(moveSpeed * 3.5f * Time.deltaTime * Vector2.right);
            if (dir == 0 && currentJumps > 1)
                Jump();
        }

        private void Jump()
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(jumpForce * Vector2.up);
            currentJumps--;
        }

        private void Flip(short dir)
        {
            if ((facingRight && dir < 0) || !facingRight && dir > 0)
            {
                facingRight = !facingRight;
                sr.flipX = !sr.flipX;
            }
        }

        protected void Crouch(bool shouldCrouch)
        {
            if (shouldCrouch)
                StartCrouch();
            else
                StopCrouch();
        }

        private void StartCrouch()
        {

        }

        private void StopCrouch()
        {

        }

        protected void PlayerRotateGun()
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dirRelativeToPlayer = (mousePos - (Vector2)transform.position).normalized;
            mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f);
            Vector3 lookPos = Camera.main.ScreenToWorldPoint(mousePos);
            lookPos -= transform.position;
            float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
            gun.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }

        protected void EnemyRotateGun()
        {
            
        }

        protected void Shoot()
        {

        }

        public void TakeDamage()
        {

        }

        private void CheckDead()
        {

        }

        private void Die()
        {

        }
    }
}
