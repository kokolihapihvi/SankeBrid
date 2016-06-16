using UnityEngine;

public class LocalPlayer : MonoBehaviour {

    public SnakeController snake;
    public float moveTimer = 0.5f;

    private float lastMove = 0;

    void Start() {

    }

    void Update() {
        Vector3 move = Vector3.right * Mathf.Round( Input.GetAxisRaw( "Horizontal" ) );

        //If there was no horizontal input
        if(move.magnitude == 0)
            move += Vector3.up * Mathf.Round( Input.GetAxisRaw( "Vertical" ) );

        //If input says we should move
        if(move.magnitude > 0) {
            //If move timer has elapsed
            if(Time.time - lastMove > moveTimer) {
                //If snake was able to move
                snake.CmdMove( move );

                //Set move timer
                lastMove = Time.time;

                if(Input.GetKey( KeyCode.Space )) {
                    snake.CmdExtend();
                }
            }
        }
    }
}
