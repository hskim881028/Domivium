using UnityEngine;
using UnityEngine.InputSystem;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public InputActionReference moveAction;

    private Vector2 movementDirection;

    void Update()
    {
        // 1. Action의 현재 값을 읽습니다.
        // OnScreenStick은 Vector2 값을 파이프합니다.
        movementDirection = moveAction.action.ReadValue<Vector2>();
        this.Log(movementDirection);

        // 2. 읽은 값을 사용하여 이동 로직을 구현합니다.
        // 예: transform.Translate(movementDirection * speed * Time.deltaTime);
    }
}
