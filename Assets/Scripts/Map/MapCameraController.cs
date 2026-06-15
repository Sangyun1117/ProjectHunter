using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MapCameraController : MonoBehaviour
{
    private Camera cameraComponent;
    [Header("Components")]
    [SerializeField] private MapManager mapManager;

    private Vector3 dragOrigin;
    private bool isDragging;

    [Header("Drag")]
    [SerializeField] private float dragSpeed = 1f;
    [SerializeField] private float clickThreshold = 5f;
    // 드래그 범위 제한을 위한 변수들
    //[SerializeField] private float minX;
    //[SerializeField] private float maxX;
    //[SerializeField] private float minY;
    //[SerializeField] private float maxY;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 30f;

    private Vector2 mouseDownPos;

    private void Awake()
    {
        cameraComponent = GetComponent<Camera>();
    }

    void Update()
    {
        HandleDrag();
        HandleZoom();
    }

    private void HandleDrag()
    {
        // 마우스 누름 시작
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            mouseDownPos = Mouse.current.position.ReadValue(); // 마우스 클릭 위치 저장

            dragOrigin = GetMouseWorldPosition();
            isDragging = true;
        }

        if (Mouse.current.leftButton.isPressed && isDragging == true)
        {
            Vector3 currentMousePos = GetMouseWorldPosition();

            Vector3 difference = dragOrigin - currentMousePos;

            transform.position += difference * dragSpeed;
        }

        // 마우스 뗌
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            Vector2 mouseUpPos = Mouse.current.position.ReadValue();

            float moveDistance = Vector2.Distance(mouseDownPos, mouseUpPos);

            // 거의 안 움직였으면 클릭 처리
            if (moveDistance < clickThreshold)
            {
                Debug.Log("Clicked!");
                HandleTileClick();
            }
            else
            {
                Debug.Log("Dragged!");
            }
            isDragging = false;
        }
    }

    private void HandleZoom()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll == 0)
        {
            return;
        }

        cameraComponent.orthographicSize -= scroll * zoomSpeed * Time.deltaTime;

        cameraComponent.orthographicSize =
            Mathf.Clamp(
                cameraComponent.orthographicSize,
                minZoom,
                maxZoom
            );
    }

    private Vector3 GetMouseWorldPosition()
    {
        //Vector3.forward  //(0,0,1) -> Z=0 평면
        Plane ground = new Plane(Vector3.forward, Vector3.zero);

        Ray ray = cameraComponent.ScreenPointToRay(
            Mouse.current.position.ReadValue()
        );

        if (ground.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return Vector3.zero;
    }
    private void HandleTileClick()
    {
        Vector3 worldPos = GetMouseWorldPosition();
        mapManager.OnClickMap(worldPos);
    }
}