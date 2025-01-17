﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour {
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    [HideInInspector] public SpriteRenderer spriteRenderer;

    [HideInInspector] public PlayerBaseState currentState;

    public readonly PlayerGroundedState GroundedState = new PlayerGroundedState();
    public readonly PlayerWalkingState WalkingState = new PlayerWalkingState();
    public readonly PlayerJumpingState JumpingState = new PlayerJumpingState();
    public readonly PlayerFallingState FallingState = new PlayerFallingState();
    public readonly PlayerDyingState DyingState = new PlayerDyingState();
    public readonly PlayerShootingState ShootingState = new PlayerShootingState();
    public readonly PlayerStompingState StompingState = new PlayerStompingState();


    public bool printDebugStates = false;
    public string debugState;

    public bool isGrounded;
    public float moveSpeed = 10f;
    public float maxFallSpeed = 14f;
    public float fallMultiplier = 10f;
    public float lowJumpMultiplier = 10f;
    public float jumpForce = 10f;
    public float stompForce = 10f;
    [HideInInspector] public int lastDirection = 1;
    [HideInInspector] public bool isDying = false;

    [HideInInspector] public float coyoteTimer;
    public float startCoyoteDurationTime = 0.1f;
    public float invulnerableTime = 1.5f;

    public bool isInvulnerable = false;

    public int maxHP = 5;
    public int hp = 5;

    public GameObject cameraHolder;

    public float shootingCooldownTimer = 0f;
    public float startShootingCooldownTimer = 0.1f;

    public float defaultBulletSpeed = 9f;
    public float shootingForce = 5f;
    public float shootingMaxSpeed = 10f;

    public int maxAmmunition = 6;
    public int ammunition = 6;

    public GameObject defaultBulletPrefab;

    public TextMeshProUGUI healthUI;
    public TextMeshProUGUI ammoUI;

    public GameObject cameraObj;
    public float dyingShakeDuration;
    public float dyingShakeMagnitude;

    public float stompShakeDuration;
    public float stompShakeMagnitude;

    public ParticleSystem shootParticles;
    public ParticleSystem emptyAmmoParticles;
    public ParticleSystem stompParticles;
    public ParticleSystem playerDyingParticles;

    public GameObject backgroundTiles;


    void Start() {
        UpdateHealthUI();
        UpdateAmmoUI();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        TransitionToState(GroundedState);
    }

    void Update() {
        if (backgroundTiles != null && transform.position.y < 0f) {
            float col = ((255f - Mathf.Abs(transform.position.y)) / 255f);
            backgroundTiles.GetComponent<Tilemap>().color = new Color(col, col, col, 1);
        }
        ProcessTimers();

        UpdateFacingSprite();
        currentState.Update(this);

        if (isInvulnerable) {
            Blink();
        }
    }

    public void TransitionToState(PlayerBaseState state) {
        if (isDying) return;
        currentState = state;
        currentState.EnterState(this);

        if (printDebugStates) {
            debugState = GetStateName();
            Debug.Log(debugState);
        }
    }

    public string GetStateName() {
        return currentState.GetType().Name;
    }

    private void UpdateFacingSprite() {
        if (lastDirection == 1) {
            spriteRenderer.flipX = false;
        }
        else if (lastDirection == -1) {
            spriteRenderer.flipX = true;
        }
    }

    private void ProcessTimers() {
        float step = Time.deltaTime;
        if (shootingCooldownTimer >= 0) shootingCooldownTimer -= step;
        if (coyoteTimer >= 0) coyoteTimer -= step;
    }

    public void TakeDamage() {
        hp--;
        UpdateHealthUI();
        Manager.audio.Play("player_being_hit");

        if (hp <= 0) {
            TransitionToState(DyingState);
            return;
        }

        ActivateInvulnerability();
    }

    public void RegenHP() {
        hp = maxHP;
        UpdateHealthUI();
    }

    public void UpdateHealthUI() {
        healthUI.text = "Health: " + hp.ToString() + "/" + maxHP.ToString();
    }

    public void UpdateAmmoUI() {
        ammoUI.text = "Ammo: " + ammunition.ToString() + "/" + maxAmmunition.ToString();
    }

    public void ActivateInvulnerability() {
        StartCoroutine(nameof(InvulnerableTimer), invulnerableTime);
    }

    public void Blink() {
        float step = 0.035f;
        float playerAlpha = spriteRenderer.color.a;
        if (playerAlpha > 0.2f) {
            spriteRenderer.color = new Color(1, 1, 1, playerAlpha - step);
        }
        else {
            spriteRenderer.color = Color.white;
        }
    }

    IEnumerator InvulnerableTimer(float waitTime) {
        isInvulnerable = true;
        yield return new WaitForSeconds(waitTime);
        isInvulnerable = false;
        spriteRenderer.color = Color.white;
    }
}
