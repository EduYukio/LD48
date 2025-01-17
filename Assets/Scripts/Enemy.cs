﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public float maxHP;
    public float hp;

    [HideInInspector] public Animator animator;
    [HideInInspector] public Rigidbody2D rb;

    private void OnTriggerStay2D(Collider2D other) {
        GameObject otherObj = other.gameObject;
        if (otherObj.CompareTag("Player")) {
            Player player = otherObj.GetComponent<Player>();
            if (player.isInvulnerable) return;

            if (player.hp > 0) player.TakeDamage();
        }
    }

    public static void DieAction(GameObject enemy) {
        //animação de death?
        Destroy(enemy);
    }
}
