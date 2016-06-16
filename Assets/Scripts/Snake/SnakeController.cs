using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SnakeController : NetworkBehaviour {

    public TilemapLoader tilemapLoader;

    public GameObject head;
    public List<GameObject> tail;

    public GameObject tailBitPrefab;

    private Vector3 lastTailPos;

    // Use this for initialization
    void Start() {
        if(tilemapLoader == null)
            tilemapLoader = FindObjectOfType<TilemapLoader>();

        lastTailPos = head.transform.position;

        ApplyGravity();
    }

    public override void OnStartLocalPlayer() {
        gameObject.AddComponent<LocalPlayer>().snake = this;
    }

    private void DoMove(Vector3 direction) {
        //Send rpc so clients do move logic as well
        if(isServer)
            RpcMove( direction );

        //If we have a tail
        if(tail.Count > 0) {
            //Set the expand position to where the last tail piece was
            lastTailPos = tail[tail.Count - 1].transform.position;

            //Move every other tail piece to where the previous piece is
            if(tail.Count > 1)
                for(int i = tail.Count - 1; i > 0; i--) {
                    MovePiece( tail[i], tail[i - 1].transform.position - tail[i].transform.position );
                }

            //Move first tail piece to where head is
            tail[0].transform.position = head.transform.position;
        } else {
            //Set the expand position to where the head was
            lastTailPos = head.transform.position;
        }

        //Move the head
        MovePiece( head, direction );

        //Apply gravity after moving
        ApplyGravity();

        //Check for any other snakes
        foreach(SnakeController othersnake in FindObjectsOfType<SnakeController>()) {
            //Ignore self
            if(othersnake == this)
                continue;

            //Force a gravity check for other snakes
            othersnake.ApplyGravity();
        }
    }

    internal void LoadSpawn(SnakeSpawn snakeSpawn) {
        //Move the head to where it belongs
        SetPiecePosition( 0, tilemapLoader.TileToWorld( snakeSpawn.head ) );
        RpcSetPiecePosition( 0, tilemapLoader.TileToWorld( snakeSpawn.head ) );

        for(int i = 0; i < snakeSpawn.tail.Count; i++) {
            //Extend tail by 1 and set the tail piece to where it belongs
            Extend();
            SetPiecePosition( i + 1, tilemapLoader.TileToWorld( snakeSpawn.tail[i] ) );

            RpcExtend();
            RpcSetPiecePosition( i + 1, tilemapLoader.TileToWorld( snakeSpawn.tail[i] ) );
        }
    }

    [ClientRpc]
    private void RpcSetPiecePosition(int piece, Vector3 pos) {
        SetPiecePosition( piece, pos );
    }

    private void SetPiecePosition(int piece, Vector3 pos) {
        if(piece == 0)
            head.transform.position = pos;
        else
            tail[--piece].transform.position = pos;
    }

    private void MovePiece(GameObject piece, Vector3 direction) {
        piece.transform.position += direction;
    }

    private bool ValidMovePosition(Vector3 pos) {
        //Check for any other snakes
        foreach(SnakeController othersnake in FindObjectsOfType<SnakeController>()) {
            //Ignore self
            if(othersnake == this)
                continue;

            //Check if their head is in the way
            if(othersnake.head.transform.position == pos) {
                return false;
            }

            //Check if their tail is in the way
            foreach(GameObject tailpiece in othersnake.tail) {
                if(tailpiece.transform.position == pos) {
                    return false;
                }
            }
        }

        //Get tile in the position we are moving into
        Tile t = tilemapLoader.tilemap.GetTile( tilemapLoader.WorldToTile( pos ) );

        //If there is no tile or tile is not solid
        return t == null || !t.solid;
    }

    private bool CanPieceMove(GameObject piece, Vector3 direction) {
        //If valid move
        if(ValidMovePosition( piece.transform.position + direction )) {
            //If we hit no tile, check if we hit our tail
            foreach(GameObject tailpiece in tail)
                if(tailpiece.transform.position == piece.transform.position + direction)
                    return false;

            return true;
        }

        return false;
    }

    private bool CanMove(Vector3 direction) {
        return CanPieceMove( head, direction );
    }

    [Command]
    public void CmdMove(Vector3 direction) {
        if(direction.magnitude > 0) {
            //If we can move in that direction
            if(CanMove( direction )) {
                //Move
                DoMove( direction );
            }
        }
    }

    [ClientRpc]
    private void RpcMove(Vector3 direction) {
        //Server does this before sending the rpc
        if(isServer)
            return;

        DoMove( direction );
    }


    [Command]
    public void CmdExtend() {
        RpcExtend();
    }

    [ClientRpc]
    public void RpcExtend() {
        Extend();
    }

    private void Extend() {
        //Spawn new tail piece and add it to the tail
        GameObject newtail = Instantiate( tailBitPrefab, lastTailPos, Quaternion.identity ) as GameObject;
        newtail.transform.SetParent( this.transform );

        tail.Insert( tail.Count, newtail );
    }

    private void ApplyGravity() {
        //Do gravity loop
        while(ShouldFall()) {
            DropSnake();
        }
    }

    private bool ShouldFall() {
        //If any tailpiece cannot move down
        if(tail.Count > 0) {
            foreach(GameObject tailpiece in tail) {
                if(!ValidMovePosition( tailpiece.transform.position + Vector3.down )) {
                    return false;
                }
            }
        }

        //If all tail pieces can move down, check head
        return ValidMovePosition( head.transform.position + Vector3.down );
    }

    private void DropSnake() {
        if(tail.Count > 0) {
            lastTailPos = tail[tail.Count - 1].transform.position;

            foreach(GameObject tailpiece in tail) {
                MovePiece( tailpiece, Vector3.down );
            }
        } else {
            lastTailPos = head.transform.position;
        }

        MovePiece( head, Vector3.down );
    }
}
