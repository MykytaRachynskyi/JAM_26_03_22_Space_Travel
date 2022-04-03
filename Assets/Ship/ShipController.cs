using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SpaceTravel
{
    public class ShipController : MonoBehaviour
    {
        [SerializeField] MeshRenderer meshRenderer;
        [SerializeField] TargetterController targetter;
        [SerializeField] Rigidbody rb;
        [SerializeField] float launchForce = 5f;
        [SerializeField] float orbitRadius = 1f;
        [Range(0f, 2f)]
        [SerializeField] float orbitSpeed = 2f;
        [SerializeField] float cameraMoveSpeed;
        [SerializeField] float maxVelocity;
        [SerializeField] float timePauseAnimationInSeconds = .3f;
        [SerializeField] float gravityMultiplier = 1f;
        [SerializeField] float blackHoleDeathRadius = .5f;

        [SerializeField] AudioSource shipSFX;

        [SerializeField] UnityEvent onPlayerDied;
        [SerializeField] UnityEvent onDestroyPlanet;
        [SerializeField] UnityEvent onStarted;

        public bool canStart { get; set; }

        bool isInPlanetRange = false;
        bool holdingSpace = false;
        bool hasStarted = false;
        bool aiming = false;
        bool clockwise = false;
        bool useGravity = false;

        PlanetController currentPlanet;
        PlanetController lastPlanet;

        Quaternion rightAngleRotation = Quaternion.Euler(0f, 0f, 90f);

        Vector3 lastPosition;
        Vector3 offsetToPlanet;

        Coroutine timePauseAnimation;

        public void StopShip()
        {
            hasStarted = false;
            rb.velocity = Vector3.zero;
            useGravity = false;
            meshRenderer.gameObject.SetActive(false);
        }

        void InitialLaunch()
        {
            rb.AddForce(this.transform.forward * launchForce, ForceMode.Impulse);
        }

        void Update()
        {
            #region old code
            /*
            if (Input.GetKeyDown(KeyCode.S) && !hasStarted)
            {
                InitialLaunch();
                hasStarted = true;
            }

            if (transform.position.y < -10f || transform.position.y > 10f)
            {
                Die();
                return;
            }

            if (!hasStarted) return;

            if (isInPlanetRange && (currentPlanet != lastPlanet))// && Input.GetKeyDown(KeyCode.Space))
            {
                orbiting = true;
                lastPlanet = currentPlanet;
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                orbiting = false;
            }

            if (orbiting)
            {
                if (!holdingSpace)
                {
                    Vector3 toShip = (transform.position - currentPlanet.position).normalized;
                    transform.position = currentPlanet.position + toShip * orbitRadius;
                }

                Vector3 toPlanetVector = (currentPlanet.position - transform.position).normalized;
                Vector3 forwardVector = (rightAngleRotation * toPlanetVector) * (clockwise ? 1f : -1f);
                forwardVector =
                    Vector3.RotateTowards(forwardVector, toPlanetVector,
                        Mathf.Clamp(Vector3.Distance(currentPlanet.position, transform.position) - orbitRadius, 0f, float.MaxValue), 0f);
                rb.velocity = forwardVector * Mathf.PI * Vector3.Distance(currentPlanet.position, transform.position) * orbitSpeed;

                holdingSpace = true;
            }
            else
            {
                if (holdingSpace)
                {
                    InitialLaunch();
                }
                holdingSpace = false;
                orbiting = false;
            }
            */
            #endregion

            if (Input.GetKeyDown(KeyCode.S) && !hasStarted && canStart)
            {
                InitialLaunch();
                hasStarted = true;
                useGravity = true;
                onStarted?.Invoke();
            }

            if (isInPlanetRange && currentPlanet != lastPlanet && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(1)))
            {
                aiming = true;
                lastPlanet = currentPlanet;
                useGravity = false;

                rb.velocity = Vector3.zero;

                PauseTime();

                offsetToPlanet = this.transform.position - currentPlanet.transform.position;

                shipSFX.Play();
            }

            if (aiming)
            {
                if (currentPlanet == null || !isInPlanetRange)
                {
                    aiming = false;
                    targetter.gameObject.SetActive(false);

                    UnpauseTime();
                }
                else
                {
                    rb.velocity = Vector3.zero;

                    targetter.gameObject.SetActive(true);
                    targetter.transform.position = currentPlanet.transform.position;

                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        this.transform.position = currentPlanet.transform.position;
                        this.transform.localRotation = Quaternion.Euler(targetter.GetAngle(), 90f, 0f);

                        targetter.gameObject.SetActive(false);

                        useGravity = true;
                        aiming = false;

                        currentPlanet.Explode();
                        onDestroyPlanet?.Invoke();

                        UnpauseTime();

                        InitialLaunch();

                        isInPlanetRange = false;
                        meshRenderer.material.color = Color.white;
                        currentPlanet = null;
                    }
                    else
                    {
                        this.transform.position = currentPlanet.transform.position + offsetToPlanet;
                    }
                }
            }

            if (Vector3.Distance(transform.position, Vector3.zero) < blackHoleDeathRadius)
            {
                Die();
                return;
            }

            if (rb.velocity.magnitude > maxVelocity)
            {
                rb.velocity = rb.velocity.normalized * maxVelocity;
            }

            if (useGravity)
            {
                rb.AddForce(-(this.transform.position).normalized * gravityMultiplier, ForceMode.Acceleration);
            }
        }

        void UnpauseTime()
        {
            if (timePauseAnimation != null)
                StopCoroutine(timePauseAnimation);
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }

        void PauseTime()
        {
            if (timePauseAnimation != null)
                StopCoroutine(timePauseAnimation);
            timePauseAnimation = StartCoroutine(AnimatedPauseTime());
        }

        IEnumerator AnimatedPauseTime()
        {
            float progress = 0f;
            while (progress < 1f)
            {
                progress += Time.unscaledDeltaTime / timePauseAnimationInSeconds;
                Time.timeScale = Mathf.Lerp(1f, 0.1f, progress);
                Time.fixedDeltaTime = Time.timeScale * 0.02f;
                yield return null;
            }
        }

        void Die()
        {
            hasStarted = false;
            rb.velocity = Vector3.zero;
            useGravity = false;
            meshRenderer.gameObject.SetActive(false);
            onPlayerDied?.Invoke();
        }

        void LateUpdate()
        {
            Vector3 cameraPosition = this.transform.position;
            cameraPosition.z = -10f;

            Camera.main.transform.position = cameraPosition;
        }

        void FixedUpdate()
        {
            if (!hasStarted || aiming || rb.velocity.magnitude < .2f) return;

            Vector3 newDirection = (this.transform.position - lastPosition) * 50f;
            Debug.DrawRay(transform.position, newDirection, Color.red);
            this.transform.localRotation = Quaternion.LookRotation(newDirection, Vector3.right);
            lastPosition = this.transform.position;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Planet")
            {
                isInPlanetRange = true;
                meshRenderer.material.color = Color.red;
                currentPlanet = other.GetComponent<PlanetController>();

                float signedAngle = Vector3.SignedAngle(currentPlanet.transform.position - transform.position, (this.transform.forward + this.transform.forward * 2f - transform.position), Vector3.forward);

                //StartCoroutine(MoveCamera(other.transform.position.x));

                clockwise = signedAngle > 0f;
            }
        }

        IEnumerator MoveCamera(float targetX)
        {
            float progress = 0f;
            Vector3 startingPos = Camera.main.transform.position;
            Vector3 targetPos = startingPos;
            targetPos.x = targetX;
            while (progress < 1f)
            {
                progress += Time.deltaTime / cameraMoveSpeed;
                Camera.main.transform.position = Vector3.Lerp(startingPos, targetPos, progress);
                yield return null;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.tag == "Planet")
            {
                isInPlanetRange = false;
                meshRenderer.material.color = Color.white;
                currentPlanet = null;
            }
        }
    }
}