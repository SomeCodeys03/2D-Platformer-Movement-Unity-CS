using Audio;
using Weapons;
using UnityEngine;

namespace Actors
{
    public class Actor : MonoBehaviour
    {
        public float moveSpeed = 1200;
        public float maxMoveSpeed = 10;
        private float currentJumps;
        public float maxJumps = 2;
        public float jumpForce = 400;
        public float groundCheckRad = 0.6f;
        public float startHealth = 100;

        private bool grounded;
        private bool moving;
        private bool facingRight;

        public Vector2 crouchScale;
        private Vector2 originalScale;
        protected Vector2 lookPos;

        public LayerMask whatIsGround;

        public GameObject gun;

        public Color damageColor;
        private Color originalColor;

        public Gun weaponScript;
        public HealthBar healthBar;

        private Rigidbody2D rb;
        private SpriteRenderer sr;

        public float Health
        {
            get;
            set;
        }

        protected void ActorStart()
        {
            if (gameObject == Game.instance.currentPlayer)
                healthBar.SetMaxHealth(100);
            rb = GetComponent<Rigidbody2D>();
            sr = GetComponent<SpriteRenderer>();
            Health = startHealth;
            originalScale = (Vector2)transform.localScale;
            originalColor = sr.color;
            weaponScript = gun.GetComponent<Gun>();
        }

        private void FixedUpdate()
        {
            SlowDown();
            if (currentJumps < maxJumps && grounded)
                currentJumps = maxJumps;
        }

        private void LateUpdate()
        {
            grounded = Physics2D.OverlapCircle(transform.position, groundCheckRad, whatIsGround);
        }

        private void SlowDown()
        {
            if (moving)
                return;

            float velX = rb.velocity.x;

            if (velX > 0.6f)
                rb.AddForce((moveSpeed + 1000) * Time.deltaTime * -Vector2.right);
            else if (velX < -0.6f)
                rb.AddForce((moveSpeed + 1000) * Time.deltaTime * Vector2.right);
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
            AudioManager.Play("PlayerJump");
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
            transform.localScale = crouchScale;
        }

        private void StopCrouch()
        {
            transform.localScale = originalScale;
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
            if (Game.instance.currentPlayer == null || gun == null)
                return;

            Vector2 playerPos = Game.instance.currentPlayer.transform.position;
            lookPos = playerPos - (Vector2)transform.position;
            float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;
            gun.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }

        protected void Shoot()
        {
            if (gun == null || !weaponScript.ready || Barrel.isExploding)
                return;
            weaponScript.Shoot();
            rb.AddForce(weaponScript.recoil * -gun.transform.up, ForceMode2D.Impulse);
        }

        public void TakeDamage(float damage)
        {
            Health -= damage;
            if (gameObject == Game.instance.currentPlayer)
                healthBar.SetHealth((int)Health);

            DamageFlash(0.2f);
            CheckDead();
        }

        private void CheckDead()
        {
            if (Health <= 0)
                Die();
        }

        private void Die()
        {
            Destroy(gameObject);
            if (gameObject.name == "Player")
                AudioManager.Play("PlayerDie");
            else
                AudioManager.Play("EnemyDie");
        }
        
        private void DamageFlash(float length)
        {
            sr.color = damageColor;
            Invoke("ResetColor", length);
        }

        private void ResetColor()
        {
            sr.color = originalColor;
        }

        public Rigidbody2D GetRb()
        {
            return rb;
        }
    }
}
