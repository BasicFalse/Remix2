#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.InputSystem;
using GorillaLocomotion;
using GorillaNetworking;

public class WASDPlayer : MonoBehaviour
{
    public PlayerBase GorillaPlayer;
    public Transform cameraTransform;
    public float moveSpeed = 7f;
    public float rotationSpeed = 2f;
    public string devRoom;

    private Rigidbody playerRigidbody;
    private Vector3 rotationVector = Vector3.zero;
    private bool didTryConnect = false;

    void Start()
    {
        playerRigidbody = GorillaPlayer.GetComponent<Rigidbody>();
    }

    void Update()
    {
        //if (!GorillaComputer.instance.isInRoom && !didTryConnect && PhotonNetworkController.instance.currentStateVisible == PhotonNetworkController.ConnectionState.ConnectedAndWaiting)
        //{
        //    PhotonNetworkController.instance.AttemptToJoinSpecificRoom("RMXGAMMA");
        //    didTryConnect = true;
        //}
/*
        if (Application.platform != RuntimePlatform.Android)
        {

            Vector3 moveDirection = Vector3.zero;

            if (Keyboard.current.wKey.isPressed)
            {
                moveDirection += cameraTransform.forward;
            }

            if (Keyboard.current.sKey.isPressed)
            {
                moveDirection -= cameraTransform.forward;
            }

            if (Keyboard.current.aKey.isPressed)
            {
                moveDirection -= cameraTransform.right;
            }

            if (Keyboard.current.dKey.isPressed)
            {
                moveDirection += cameraTransform.right;
            }

            moveDirection.y = 0;
            moveDirection.Normalize();

            if (Keyboard.current.leftShiftKey.isPressed)
            {
                moveSpeed = 30f;
            }
            else
            {
                moveSpeed = 7f;
            }

            if (Keyboard.current.spaceKey.isPressed)
            {
                moveDirection -= cameraTransform.up;
                moveDirection.y = 2;
            }

            playerRigidbody.MovePosition(playerRigidbody.position + moveDirection * moveSpeed * Time.deltaTime);

            if (Mouse.current.rightButton.isPressed || Mouse.current.leftButton.isPressed)
            {
                float mouseX = Mouse.current.delta.x.ReadValue() * rotationSpeed;
                float mouseY = Mouse.current.delta.y.ReadValue() * rotationSpeed;

                rotationVector.x -= mouseY;
                rotationVector.y += mouseX;
                rotationVector.x = Mathf.Clamp(rotationVector.x, -90f, 90f);

                cameraTransform.localRotation = Quaternion.Euler(rotationVector);
            }

            if (Keyboard.current.tKey.isPressed)
            {
                PlayerPrefs.SetString("resonator", "done");
                PlayerPrefs.Save();
            }

            if (Keyboard.current.homeKey.isPressed)
            {
                PhotonNetworkController.instance.AttemptToJoinSpecificRoom(devRoom);
            }
        }

*/
    }
}
#endif