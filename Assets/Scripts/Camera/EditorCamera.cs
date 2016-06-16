using UnityEngine;

public class EditorCamera : MonoBehaviour {

    private Camera cm;

    // Use this for initialization
    void Start() {
        cm = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update() {
        //Zoom
        cm.orthographicSize = Mathf.Clamp( cm.orthographicSize - Input.mouseScrollDelta.y, 1, 15 );

        //Pan
        if(Input.GetMouseButton( 2 ))
            transform.position -= (Vector3.right * Input.GetAxis( "Mouse X" ) + Vector3.up * Input.GetAxis( "Mouse Y" )) * (cm.orthographicSize / 15f);
    }
}
