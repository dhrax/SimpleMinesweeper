using UnityEngine;

public class CameraMovementManager : MonoBehaviour
{
    private Camera cam;

    private int minCamSize = 2, maxCamSize = 100, zoomStep = 10;

    private float zoomTouchStep = 0.01f;

    private Vector3 dragOrigin;
    private Vector3 dragEnd;

    private bool touchingScreen;
    private bool clickToBeHandled;

    void Start(){
        cam = GetComponent<Camera>();
        touchingScreen = false;
        clickToBeHandled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchSupported){
            handleCameraByTouch();
        }else{
            handleCamera();
        }
    }
    /// <summary>
    /// Handle camera movement by mouse
    /// </summary>
    private void handleCamera()
    {
        MoveCamera();
        float newSize = cam.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * zoomStep;
        ZoomCamera(newSize);
        
    }

    /// <summary>
    /// Handles camera movement by touch input
    /// </summary>
    private void handleCameraByTouch()
    {
        switch (Input.touchCount)
        {
            case 0:
                touchingScreen = false;
                //if we have to handle the click and the touch position has not moved (the user has just clicked the screen) 
                if (clickToBeHandled && (dragOrigin == dragEnd))
                {
                    GameManager.Instance.performActionInBoxClicked(dragOrigin);
                }
                clickToBeHandled = false;
                break;
            case 1:
                //if we were not touching the screen previosuly, we get the starting position and set the click to be handled
                if(!touchingScreen){
                    dragOrigin = cam.ScreenToWorldPoint(Input.GetTouch(0).position);
                    touchingScreen = true;
                    clickToBeHandled = true;
                }else{
                    //if we were touching the screen previosly (we are dragging the finger), we get the end position and move the camera
                    dragEnd = cam.ScreenToWorldPoint(Input.GetTouch(0).position);
                    Vector3 difference = dragOrigin - dragEnd;
                    
                    cam.transform.position += difference;
                }
                
                break;
            case 2:
                // Pinch to zoom
                // get current touch positions
                Touch tZero = Input.GetTouch(0);
                Touch tOne = Input.GetTouch(1);
                // get touch position from the previous frame
                Vector2 tZeroPrevious = tZero.position - tZero.deltaPosition;
                Vector2 tOnePrevious = tOne.position - tOne.deltaPosition;

                float oldTouchDistance = Vector2.Distance (tZeroPrevious, tOnePrevious);
                float currentTouchDistance = Vector2.Distance (tZero.position, tOne.position);

                // get offset value
                float deltaDistance = oldTouchDistance - currentTouchDistance;
                float newSize = cam.orthographicSize + deltaDistance * zoomTouchStep;
                ZoomCamera(newSize);
                break;
        }
        
    }


    /// <summary>
    /// Sets the camera orthographicSize
    /// </summary>
    /// <param name="newSize">new camera orthographicSize</param>
    private void ZoomCamera(float newSize)
    {
        cam.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);
    }

    /// <summary>
    /// Move camera by mouse input
    /// </summary>
    private void MoveCamera()
    {
        if(Input.GetMouseButtonDown(0)){
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if(Input.GetMouseButton(0)){
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);

            cam.transform.position += difference;
        }
    }
}
