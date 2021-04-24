using UnityEngine;

public abstract class PlayerBaseState {
    public abstract void EnterState(Player player);
    public abstract void Update(Player player);


    public virtual bool CheckTransitionToGrounded(Player player) {
        if (player.isGrounded) {
            player.TransitionToState(player.GroundedState);
            return true;
        }

        return false;
    }

    public virtual bool CheckTransitionToWalking(Player player) {
        float xInput = Input.GetAxisRaw("Horizontal");
        if (xInput != 0) {
            player.TransitionToState(player.WalkingState);
            return true;
        }

        return false;
    }

    public void ProcessMovementInput(Player player) {
        float xInput = Input.GetAxisRaw("Horizontal");
        int direction = GetRawDirection(xInput);
        // if (direction != 0) {
        //     player.lastDirection = direction;
        // }

        player.rb.velocity = new Vector2(direction * player.moveSpeed, player.rb.velocity.y);
    }

    public int GetRawDirection(float input) {
        int direction = 0;
        if (input > 0) {
            direction = 1;
        }
        else if (input < 0) {
            direction = -1;
        }

        return direction;
    }
}