using MalbersAnimations.Scriptables;
using Micosmo.SensorToolkit;
using Micosmo.SensorToolkit.Example;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Malbers.Integration.AITree
{
    public class SecurityCamera : MonoBehaviour
    {
        public UnityEvent onAlarmState;

        public float detectionRadius = 10f; // Radius for detecting nearby AI
        public float RotationSpeed;
        public float ScanTime;
        public float TrackTime;
        public float ScanArcAngle;
        public Color ScanColour;
        public Color TrackColour;
        public Color AlarmColour;
        [Header("References")]
        public TeamMember TeamMember;
        public Light SpotLight;
        public Sensor Sensor;
        public Sensor InformationSensor;
        public TransformVar targetTransformVar;
        public BoolVar inAlarmState;
        bool OngoingAlarm;
        public Vector3Var targetPosition;
        public float alarmDuration=15f;
        Quaternion leftExtreme;
        Quaternion rightExtreme;
        Quaternion targetRotation;

        void Awake()
        {
            leftExtreme = Quaternion.AngleAxis(ScanArcAngle / 2f, Vector3.up) * transform.rotation;
            rightExtreme = Quaternion.AngleAxis(-ScanArcAngle / 2f, Vector3.up) * transform.rotation;
        }

        private void Start()
        {
            inAlarmState.Value = false;
            OngoingAlarm = false;
        }

        void OnEnable()
        {
            targetRotation = transform.rotation;
            transform.rotation = rightExtreme;
            StartCoroutine(ChooseScanDirectionState());
        }

        void Update()
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);

        }

        IEnumerator ChooseScanDirectionState()
        {
            if (targetRotation == leftExtreme)
            {
                targetRotation = rightExtreme;
            }
            else
            {
                targetRotation = leftExtreme;
            }
            yield return StartCoroutine(ScanState());
        }

        IEnumerator ScanState()
        {
            var timer = ScanTime;
            SpotLight.color = ScanColour;
            while (timer >= 0)
            {
                timer -= Time.deltaTime;

                if (AlarmController.Instance.IsAlarmState)
                {
                    StartCoroutine(AlarmState());
                    yield break;
                }
                var nearestEnemy = Sensor.GetNearestDetection(s => TeamMember.IsEnemy(s.Object));
                if (nearestEnemy != null)
                {
                    StartCoroutine(TrackState(nearestEnemy));
                    yield break;
                }
                yield return null;
            }

            StartCoroutine(ChooseScanDirectionState());
        }

        IEnumerator TrackState(GameObject target)
        {
            SpotLight.color = TrackColour;
            var timer = TrackTime;
            if (!OngoingAlarm)
            {
                targetPosition.Value = target.transform.position;
            }
            while (timer > 0f)
            {
                timer -= Time.deltaTime;

                if (!Sensor.IsDetected(target))
                {
                    StartCoroutine(ChooseScanDirectionState());
                    yield break;
                }

                targetRotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
                yield return null;
            }

            AlarmController.Instance.StartAlarm(target);
            StartCoroutine(AlarmState());
        }

        IEnumerator AlarmState()
        {
            targetRotation = transform.rotation;
            SpotLight.color = AlarmColour;
            inAlarmState.Value = true;

            // Set the target transform value only if it's the first time the alarm is triggered


            OngoingAlarm = true;
            onAlarmState?.Invoke();
            yield return new WaitForSeconds(alarmDuration);

            // Reset the alarm state
            ResetAlarmState();
        }
        void ResetAlarmState()
        {
            // Perform any cleanup or reset necessary for the alarm state
            SpotLight.color = ScanColour; // Reset the spotlight color
            inAlarmState.Value = false; // Reset the alarm state variable
            OngoingAlarm = false; // Reset the ongoing alarm flag
                                  // Reset any other variables or states related to the alarm
                                  // Example: targetTransformVar.Value = null; // Reset the target transform variable
                                  // Example: targetPosition.Value = Vector3.zero; // Reset the target position variable
                                  // Example: Perform any other necessary cleanup
        }
    }

}
