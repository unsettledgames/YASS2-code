using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject PlayerAnchor;
    [SerializeField] private GameObject Player;
    [SerializeField] private float CameraFocusOffsetAmount;

    [SerializeField] private float PositionEasing;
    [SerializeField] private float RotationEasing;
    [SerializeField] private float TiltY;

    private Vector3 m_DyingDistance = Vector3.zero;

    void FixedUpdate()
    {
        if (FrequentlyAccessed.Instance.Player.IsDying && !FrequentlyAccessed.Instance.Player.Dead)
        {
            if (m_DyingDistance == Vector3.zero)
                m_DyingDistance = Player.transform.right * 20.0f;
            transform.position = Vector3.Lerp(transform.position, Player.transform.position + m_DyingDistance, PositionEasing / 8.0f * Time.deltaTime);
            transform.LookAt(Player.transform);
        }
        else if (!FrequentlyAccessed.Instance.Player.Dead)
        {
            float xRot = Mathf.LerpAngle(transform.localEulerAngles.x, Player.transform.localEulerAngles.x, RotationEasing * Time.deltaTime);
            float yRot = Mathf.LerpAngle(transform.localEulerAngles.y, Player.transform.localEulerAngles.y, RotationEasing * Time.deltaTime);
            float zRot = Mathf.LerpAngle(transform.localEulerAngles.z, Player.transform.localEulerAngles.z, RotationEasing * Time.deltaTime);

            transform.position = Vector3.Lerp(transform.position, PlayerAnchor.transform.position, PositionEasing * Time.deltaTime);
            transform.localEulerAngles = new Vector3(xRot, yRot, zRot);

            transform.Rotate(Vector3.right, (Input.mousePosition.y / Screen.height) * -CameraFocusOffsetAmount - TiltY);
            transform.Rotate(Vector3.up, (Input.mousePosition.x / Screen.width) * CameraFocusOffsetAmount);
        }
    }
}