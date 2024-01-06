using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkySync
{

    public class AircraftController : MonoBehaviour
    {
        [SerializeField] private float m_MaxEnginePower = 40f;              // The maximum output of the engine.
        public float Altitude { get; private set; }
        public float Thrust { get; private set; }
        public float ForwardSpeed { get; private set; }                     // How fast the aeroplane is traveling in it's forward direction.
        public float EnginePower { get; private set; }                      // How much power the engine is being given.
        public float MaxEnginePower { get { return m_MaxEnginePower; } }    // The maximum output of the engine.

        private AircraftAgent AAgent;

        // Start is called before the first frame update
        void Start()
        {
            AAgent = GetComponent<AircraftAgent>();
        }

        // Update is called once per frame
        void Update()
        {
            Thrust = AAgent.rigidbody.velocity.magnitude;
            EnginePower = Thrust * m_MaxEnginePower;
            CalculateForwardSpeed();
            //print(EnginePower);
        }

        public void CalculateForwardSpeed()
        {
            // Forward speed is the speed in the planes's forward direction (not the same as its velocity, eg if falling in a stall)
            var localVelocity = transform.InverseTransformDirection(AAgent.rigidbody.velocity);
            ForwardSpeed = Mathf.Max(0, localVelocity.z);
        }

        public void CalculateAltitude()
        {
            // Altitude calculations - we raycast downwards from the aeroplane
            // starting a safe distance below the plane to avoid colliding with any of the plane's own colliders
            var ray = new Ray(transform.position - Vector3.up * 10, -Vector3.up);
            RaycastHit hit;
            Altitude = Physics.Raycast(ray, out hit) ? hit.distance + 10 : transform.position.y;
            //print(Altitude);
        }
    }

}
