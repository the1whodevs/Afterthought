using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//using UnityEngine.Experimental.Rendering.HDPipeline;

namespace Knife.RealBlood.SimpleController
{
    /// <summary>
    /// Simple player controller with run, jump and crouching
    /// </summary>
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerController : MonoBehaviour
    {
        public GameObject control;

        public MouseLook look;
        public Headbob headBob;

        public AnimationCurve headBobBlendCurve;
        public AnimationCurve headBobPeriodBlendCurve;

        public string forwardAxis = "Vertical";
        public string strafeAxis = "Horizontal";
        public Transform directionReference;
        public float crouchSpeedMultiplier = 0.75f;
        public float runSpeedMultiplier = 1.5f;
        public float runIncreaseSpeedTime = 1f;
        public float runSpeedThreshold = 1f;
        public AnimationCurve runIncreaseSpeedCurve;
        public float collisionScale = 1f;

        public float speed = 1f;
        public LayerMask groundLayer;
        public float threshold = 0.1f;
        public float gravity = 9.81f;
        public float stickToGround = 9.81f;

        public PlayerStandState standState;
        public PlayerStandState crouchState;
        public float stateChangeSpeed = 3.666f;
        public AnimationCurve stateChangeCurve;
        public float maxSpeed = 1f;
        public float weightSmooth = 3f;
        public float jumpSpeed = 5f;
        public Camera playerCamera;
        public Transform controlCamera;
        public Transform handsHeadBobTarget;
        public float cameraHeadbobWeight = 1f;
        public float handsHeadbobWeight = 0.3f;
        public float handsHeadbobMultiplier = 1;

        public PlayerFreezeChangedEvent playerFreezeChanged = new PlayerFreezeChangedEvent();

        [System.Serializable]
        public class PlayerStandState
        {
            public float controlCameraHeight;
            public float colliderHeight;
            public float colliderCenterHeight;
        }

        public Vector3 PlayerVelocity
        {
            get
            {
                return controller.velocity;
            }
        }

        public bool IsRunning { get; private set; }

        public bool IsCrouching { get; private set; }

        public bool IsGrounded => controller.isGrounded;
        public float DefaultHandsHeadbobWeight { get; private set; }

        public UnityAction runStartEvent;
        public UnityAction jumpStartEvent;
        public UnityAction jumpFallEvent;
        public UnityAction jumpEndEvent;
        public UnityAction crouchEvent;
        public UnityAction standUpEvent;

        CapsuleCollider charactarCollider;
        CharacterController controller;

        Vector3 playerVelocity = Vector3.zero;
        Vector3 oldPlayerVelocity = Vector3.zero;

        Vector3 oldHandHeadBobPos;
        Vector3 oldCameraHeadBobPos;

        Vector3 controlCameraPosition;
        float standStateBlend;

        float runTime = 0f;
        float standChangeTime;

        bool oldIsGrounded = false;

        private bool isPaused = false;

        public bool IsFreezed { get; private set; } = false;

        private void Start()
        {
            charactarCollider = GetComponent<CapsuleCollider>();
            controller = GetComponent<CharacterController>();

            look.Init(transform, controlCamera);

            DefaultHandsHeadbobWeight = handsHeadbobWeight;

            controlCameraPosition = controlCamera.localPosition;

            PausePlayer();
        }

        public void UpdateDefaultDeath()
        {
            look.RotateCameraSmoothlyTo(0, Time.deltaTime);
        }

        private void PausePlayer()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            control.SetActive(true);
            Freeze(true);
            isPaused = true;
        }

        private void UnpausePlayer()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            control.SetActive(false);
            Freeze(false);
            isPaused = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                    UnpausePlayer();
                else
                    PausePlayer();
            }
            
            UpdatePlayer();
        }

        private void UpdatePlayer()
        {
            look.LookRotation(Time.deltaTime);

            if (controller.isGrounded)
            {
                Move();
            }
            else
            {
                ApplyGravity();
            }

            if (oldPlayerVelocity.y > 0 && playerVelocity.y < 0)
            {
                jumpFallEvent?.Invoke();
            }

            if (controller.isGrounded && !oldIsGrounded)
            {
                jumpEndEvent?.Invoke();
            }

            oldPlayerVelocity = playerVelocity;
            oldIsGrounded = controller.isGrounded;
        }

        public void Freeze(bool value)
        {
            //if (!value) // and is dead
            //    return;

            look.enabled = !value;
            IsFreezed = value;

            playerFreezeChanged.Invoke(IsFreezed);
        }

        private void Move()
        {
            var h = Input.GetAxis(strafeAxis);
            var v = Input.GetAxis(forwardAxis);

            if (IsFreezed)
            {
                h = 0;
                v = 0;
            }

            headBob.CalcHeadbob(Time.time);

            var localPosition = handsHeadBobTarget.localPosition;
            localPosition -= oldHandHeadBobPos;

            // you can do any HandsHeadBobTarget position set

            localPosition += headBob.HeadBobPos * handsHeadbobWeight * handsHeadbobMultiplier;
            handsHeadBobTarget.localPosition = localPosition;

            var position = controlCamera.localPosition;
            position -= oldCameraHeadBobPos;

            position = controlCameraPosition;
            // you can do any ControlCamera position set

            position += headBob.HeadBobPos * cameraHeadbobWeight;
            controlCamera.localPosition = position;

            oldHandHeadBobPos = headBob.HeadBobPos * (handsHeadbobWeight * handsHeadbobMultiplier);
            oldCameraHeadBobPos = headBob.HeadBobPos * cameraHeadbobWeight;

            var forward = directionReference.forward;
            forward.y = 0;
            forward.Normalize();
            var moveVector = forward * v + directionReference.right * h;
            var playerXZVelocity = Vector3.Scale(playerVelocity, new Vector3(1, 0, 1));

            var speed = this.speed;
            if (Input.GetKey(KeyCode.LeftShift) && !Input.GetMouseButton(1) && !Input.GetMouseButton(0) && playerXZVelocity.magnitude >= runSpeedThreshold && !IsCrouching)
            {
                //speed *= RunSpeedMultiplier;
                runTime += Time.deltaTime;
                if (!IsRunning)
                {
                    IsRunning = true;
                    runStartEvent?.Invoke();
                }
            }
            else
            {
                runTime -= Time.deltaTime;
                
                IsRunning = false;
            }
            
            /*
            if(Input.GetKeyDown(KeyCode.LeftControl) && !freezeControl)
            {
                isCrouching = true;
                
                if(CrouchEvent != null)
                {
                    CrouchEvent();
                }
            }

            if(isCrouching)
            {

            }

            if ((Input.GetKeyUp(KeyCode.LeftControl) && !freezeControl) || (Input.GetKey(KeyCode.LeftControl) && freezeControl && isCrouching))
            {
                isCrouching = false;

                if (true) // if dead
                {
                    if (StandUpEvent != null)
                    {
                        StandUpEvent();
                    }
                }

            }*/
            
            if (Input.GetKeyDown(KeyCode.LeftControl) && !IsFreezed)
            {
                IsCrouching = !IsCrouching;

                if (IsCrouching)
                {
                    crouchEvent?.Invoke();
                }
                else
                {
                    standUpEvent?.Invoke();
                }
            }

            StandStateChange();

            runTime = Mathf.Clamp(runTime, 0, runIncreaseSpeedTime);

            var runTimeFraction = runTime / runIncreaseSpeedTime;
            var runMultiplier = Mathf.Lerp(1, runSpeedMultiplier, runIncreaseSpeedCurve.Evaluate(runTimeFraction));
            speed *= runMultiplier;
            if (IsCrouching)
                speed *= crouchSpeedMultiplier;

            var r = new Ray(transform.position, Vector3.down);

            Physics.SphereCast(r, charactarCollider.radius, out var hitInfo, charactarCollider.height / 2f, groundLayer);

            var desiredVelocity = Vector3.ProjectOnPlane(moveVector, hitInfo.normal) * speed;
            playerVelocity.x = desiredVelocity.x;
            playerVelocity.z = desiredVelocity.z;
            playerVelocity.y = -stickToGround;

            var calculatedVelocity = playerVelocity;
            calculatedVelocity.y = 0;

            var speedFraction = calculatedVelocity.magnitude / maxSpeed;
            headBob.headBobWeight = Mathf.Lerp(headBob.headBobWeight, headBobBlendCurve.Evaluate(speedFraction), weightSmooth * Time.deltaTime);
            headBob.headBobPeriod = headBobPeriodBlendCurve.Evaluate(speedFraction);

            if (!controller.isGrounded) return;
            
            if (Input.GetKeyDown(KeyCode.Space) && !IsCrouching && !IsFreezed)
            {
                playerVelocity.y = jumpSpeed;
                jumpStartEvent?.Invoke();
            }
            
            controller.Move(playerVelocity * Time.deltaTime);
        }

        private void StandStateChange()
        {
            standStateBlend = Mathf.MoveTowards(standStateBlend, IsCrouching ? 1f : 0f, Time.deltaTime * stateChangeSpeed);

            charactarCollider.height = Mathf.Lerp(
                standState.colliderHeight,
                crouchState.colliderHeight,
                stateChangeCurve.Evaluate(standStateBlend)
                );


            var colliderCenter = charactarCollider.center;

            colliderCenter.y = Mathf.Lerp(
                standState.colliderCenterHeight,
                crouchState.colliderCenterHeight,
                stateChangeCurve.Evaluate(standStateBlend)
                );
            charactarCollider.center = colliderCenter;

            controller.height = charactarCollider.height;
            controller.center = charactarCollider.center;

            controlCameraPosition.y = Mathf.Lerp(
                standState.controlCameraHeight,
                crouchState.controlCameraHeight,
                stateChangeCurve.Evaluate(standStateBlend)
                );
        }

        public void SetSensivityMultiplier(float multiplier)
        {
            look.sensivityMultiplier = multiplier;
        }

        private void ApplyGravity()
        {
            playerVelocity += Vector3.down * (gravity * Time.deltaTime);
            controller.Move(playerVelocity * Time.deltaTime);
        }

        private void OnCollisionStay(Collision collision)
        {
            foreach(var c in collision.contacts)
            {
                if(c.otherCollider.attachedRigidbody != null)
                {
                    c.otherCollider.attachedRigidbody.AddForceAtPosition(PlayerVelocity * Time.fixedDeltaTime * collisionScale, c.point);
                }
            }
        }

        [System.Serializable]
        public class Headbob
        {
            public bool enabled = true;
            public float headBobWeight = 1f;
            public Vector2 headBobAmount = new Vector2(0.11f, 0.08f);
            public float headBobPeriod = 1f;
            public AnimationCurve headBobCurveX;
            public AnimationCurve headBobCurveY;

            public Vector3 HeadBobPos { get; private set; }

            public void CalcHeadbob(float currentTime)
            {
                var headBob = Mathf.PingPong(currentTime, headBobPeriod) / headBobPeriod;

                var headBobVector = new Vector3();

                headBobVector.x = headBobCurveX.Evaluate(headBob) * headBobAmount.x;
                headBobVector.y = headBobCurveY.Evaluate(headBob) * headBobAmount.y;

                headBobVector = Vector3.LerpUnclamped(Vector3.zero, headBobVector, headBobWeight);

                if (!Application.isPlaying)
                {
                    headBobVector = Vector2.zero;
                }

                if (enabled)
                {
                    HeadBobPos = headBobVector;
                }
            }
        }

        [System.Serializable]
        public class MouseLook
        {
            public bool enabled;
            public float xSensitivity = 2f;
            public float ySensitivity = 2f;
            public float sensivityMultiplier = 1f;
            public float minimumX = -90F;
            public float maximumX = 90F;
            public float smoothTime = 15f;
            public bool clampVerticalRotation = true;

            public string axisXName = "Mouse X";
            public string axisYName = "Mouse Y";

            private Quaternion characterTargetRot;
            private Quaternion cameraTargetRot;

            private Transform character;
            private Transform camera;

            public void Init(Transform character, Transform camera)
            {
                characterTargetRot = character.localRotation;
                cameraTargetRot = camera.localRotation;

                this.character = character;
                this.camera = camera;
            }

            public void LookRotation(float deltaTime)
            {
                if (!enabled)
                    return;

                LookRotation(Input.GetAxis(axisXName) * xSensitivity * sensivityMultiplier, Input.GetAxis(axisYName) * ySensitivity * sensivityMultiplier, deltaTime);
            }

            public void LookRotation(float yRot, float xRot, float deltaTime)
            {
                characterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
                cameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

                if (clampVerticalRotation)
                    cameraTargetRot = ClampRotationAroundXAxis(cameraTargetRot);

                character.localRotation = Quaternion.Slerp(character.localRotation, characterTargetRot, smoothTime * deltaTime);
                camera.localRotation = Quaternion.Slerp(camera.localRotation, cameraTargetRot, smoothTime * deltaTime);
            }

            public void RotateCameraSmoothlyTo(float xRot, float deltaTime)
            {
                cameraTargetRot = Quaternion.Euler(xRot, 0f, 0f);

                if (clampVerticalRotation)
                    cameraTargetRot = ClampRotationAroundXAxis(cameraTargetRot);

                camera.localRotation = Quaternion.Slerp(camera.localRotation, cameraTargetRot, smoothTime * deltaTime);
            }

            private Quaternion ClampRotationAroundXAxis(Quaternion q)
            {
                q.x /= q.w;
                q.y /= q.w;
                q.z /= q.w;
                q.w = 1.0f;

                var angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

                angleX = Mathf.Clamp(angleX, minimumX, maximumX);

                q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

                return q;
            }

        }
    }

    public class PlayerFreezeChangedEvent : UnityEvent<bool>
    { }
}