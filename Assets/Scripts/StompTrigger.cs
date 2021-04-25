﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StompTrigger : MonoBehaviour {
    Player player;

    private void Start() {
        player = FindObjectOfType<Player>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        GameObject otherObj = other.gameObject;
        if (otherObj.CompareTag("PlayerStompCollider")) {
            Player player = otherObj.transform.parent.gameObject.GetComponent<Player>();
            string playerState = player.GetStateName();

            bool playerIsFalling = playerState == "PlayerFallingState" && player.rb.velocity.y < 0f;
            bool playerIsShooting = playerState == "PlayerShootingState";

            if (playerIsFalling || playerIsShooting) {
                Stomp();
            }
        }
    }

    public void Stomp() {
        // particulas
        // screen shake
        player.TransitionToState(player.StompingState);
        // recarrega municao
        Destroy(gameObject.transform.parent.gameObject);
    }
}