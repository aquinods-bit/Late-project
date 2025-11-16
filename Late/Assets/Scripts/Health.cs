using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class Health : MonoBehaviour
{
    public enum LifeState { Healthy, Injured, Dead }

    [Header("Health")]
    public float maxHealth = 100f;
    [Tooltip("Optional: damage applied when colliding with an object tagged 'Enemy'")]
    public float contactDamage = 10f;

    [Header("Damage/Invuln")]
    [Tooltip("Seconds of invulnerability after taking damage to avoid multiple hits")]
    public float invulnerabilitySeconds = 0.5f;

    [Header("Events")]
    public UnityEvent OnHurt;
    public UnityEvent OnDied;
    public UnityEvent OnHealed;

    // runtime
    public float currentHealth { get; private set; }
    public LifeState CurrentState { get; private set; } = LifeState.Healthy;

    float invulnTimer = 0f;

    void Awake()
    {
        currentHealth = maxHealth;
        UpdateState();
    }

    void Update()
    {
        if (invulnTimer > 0f)
            invulnTimer -= Time.deltaTime;
    }

    // Call this to apply damage to this object. Returns true if damage was applied.
    public bool TakeDamage(float amount)
    {
        if (CurrentState == LifeState.Dead) return false;
        if (invulnTimer > 0f) return false;

        currentHealth -= amount;
        currentHealth = Mathf.Max(0f, currentHealth);
        invulnTimer = invulnerabilitySeconds;

        if (currentHealth <= 0f)
        {
            UpdateState();
            HandleDeath();
            return true;
        }
        else
        {
            UpdateState();
            if (OnHurt != null) OnHurt.Invoke();
            Debug.Log($"[Health] {gameObject.name} hurt: {amount} dmg -> {currentHealth}/{maxHealth}");
            return true;
        }
    }

    public void Heal(float amount)
    {
        if (CurrentState == LifeState.Dead) return;
        currentHealth += amount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);
        UpdateState();
        if (OnHealed != null) OnHealed.Invoke();
        Debug.Log($"[Health] {gameObject.name} healed: {amount} -> {currentHealth}/{maxHealth}");
    }

    void UpdateState()
    {
        if (currentHealth <= 0f)
            CurrentState = LifeState.Dead;
        else if (Mathf.Approximately(currentHealth, maxHealth))
            CurrentState = LifeState.Healthy;
        else
            CurrentState = LifeState.Injured;
    }

    void HandleDeath()
    {
        UpdateState();
        Debug.Log($"[Health] {gameObject.name} died.");
        if (OnDied != null) OnDied.Invoke();
        // Default death behaviour: disable this GameObject's behaviour scripts (except Health)
        var behaviours = GetComponents<MonoBehaviour>();
        foreach (var b in behaviours)
        {
            if (b != this)
                b.enabled = false;
        }
        // Disable common movement components so the player/enemy stops moving
        // CharacterController
        var cc = GetComponent<CharacterController>();
        if (cc != null)
            cc.enabled = false;

        // NavMeshAgent
        var nav = GetComponent<NavMeshAgent>();
        if (nav != null)
            nav.enabled = false;

        // Rigidbody - stop motion and lock
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    // Optional: if enemy collides with player, apply contact damage
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            bool applied = TakeDamage(contactDamage);
            if (applied)
                Debug.Log($"[Health] {gameObject.name} took {contactDamage} contact damage from {collision.gameObject.name}");
        }
    }

    // Also support triggers
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            bool applied = TakeDamage(contactDamage);
            if (applied)
                Debug.Log($"[Health] {gameObject.name} took {contactDamage} contact damage from {other.gameObject.name}");
        }
    }

    // Utility: instantly kill
    public void Kill()
    {
        if (CurrentState == LifeState.Dead) return;
        currentHealth = 0f;
        HandleDeath();
    }
}
