using UnityEngine;

// This class is very similar to TwoBodySimulation, except we assume here that the
// mass of one body (the star) is very large compared to the mass of the other (the planet)
//
// Default units used here are
// 1 L = 1 AU
// 1 M = 1 solar mass
// 1 T = 1 year
//
[RequireComponent(typeof(KeplerPrefabManager))]
public class KeplerSimulation : Simulation
{
    private KeplerPrefabManager prefabs;

    [Header("Simulation Properties")]
    public int numSubsteps = 10;
    public bool resetAfterOnePeriod = true;
    public enum UnitTime { Year, Month, Day}
    public UnitTime unitTime = UnitTime.Year;
    public enum UnitLength { AU, SolarRadius }
    public UnitLength unitLength = UnitLength.AU;
    public enum UnitMass { SolarMass }
    public UnitMass unitMass = UnitMass.SolarMass;
    public float timeScale = 1;

    [Header("Star")]
    public float starMass = 1f;
    public float starRadius = 10f;
    public Vector2 starPosition = Vector2.zero;
    public enum Focus { Left, Right }
    public Focus starAtFocus = Focus.Left;

    [Header("Planet(s)")]
    public float planet1Radius = 1f;
    public float perihelionDistance = 21.48f;
    public bool startAtPerihelion = true;
    public float eccentricity = 0.016f;
    public enum OrbitDirection { Clockwise, Counterclockwise }
    public OrbitDirection orbitDirection = OrbitDirection.Counterclockwise;
    public float planet2Radius = 1f;
    public float initPlanet2OffsetAngle = 45f;

    // Celestial bodies
    [HideInInspector] public CelestialBody star;
    [HideInInspector] public CelestialBody planet1;
    [HideInInspector] public CelestialBody planet2;

    private Light planet1Light;
    private Light planet2Light;

    // Orbital parameters
    [HideInInspector] public OrbitalParameters orbitalParameters;
    private float orbitSign;

    // Timer for reinitializing the simulation after one full period
    [HideInInspector] public float theta1;
    [HideInInspector] public float theta2;
    [HideInInspector] public float resetTimer;
    private Vector3 initPlanet1Position;
    private Vector3 initPlanet2Position;

    // Other quantities used by the UI Manager
    [HideInInspector] public Vector3 currentForce;

    // Constants
    private const double newtonG_SI = 6.6743e-11;   // m^3 / kg / s^2
    private const float au_SI = 149597870700f;      // m
    private const double r_sun_SI = 6.9634e8;       // m
    private const double m_sun_SI = 1.98847e30;     // kg
    private const float year_SI = 31556952f;        // s
    private const float month_SI = year_SI / 12f;   // s
    private const float day_SI = 86400f;            // s

    // Gravitational constant
    public float NewtonG
    {
        get
        {
            float t = year_SI;
            if (unitTime == UnitTime.Month)
            {
                t = month_SI;
            }
            else if (unitTime == UnitTime.Day)
            {
                t = day_SI;
            }

            double l = au_SI;
            if (unitLength == UnitLength.SolarRadius)
            {
                l = r_sun_SI;
            }

            return (float)(newtonG_SI * m_sun_SI * t * t / l / l / l);
        }
    }
    // Semi-major axis
    public float SemiMajorAxis
    {
        get { return perihelionDistance / (1 - Eccentricity); }
    }
    // Orbital period
    public float Period
    {
        get
        {
            // Unbound orbit
            if (Eccentricity >= 1)
            {
                return float.PositiveInfinity;
            }

            // Bound orbit
            float a = SemiMajorAxis;
            return 2 * Mathf.PI * Mathf.Sqrt(a * a * a / NewtonG / star.Mass);
        }
    }
    // Energy (specific)
    public float Energy
    {
        get { return -0.5f * NewtonG * star.Mass / SemiMajorAxis; }
    }
    // Angular momentum magnitude (specific)
    public float AngularMomentum
    {
        get
        {
            float sign = (orbitDirection == OrbitDirection.Clockwise) ? -1f : 1f;
            return sign * Mathf.Sqrt(NewtonG * star.Mass * SemiLatusRectum);
        }
    }
    // Orbit eccentricity
    public float Eccentricity
    {
        get { return eccentricity; }
    }
    // Semi-latus Rectum
    public float SemiLatusRectum
    {
        get { return perihelionDistance * (1 + Eccentricity); }
    }
    // Ratio T^2 / a^3
    public float PeriodToSemiMajorAxisRatio
    {
        get { return Period * Period / SemiMajorAxis / SemiMajorAxis / SemiMajorAxis; }
    }

    private void Awake()
    {
        if (!TryGetComponent(out prefabs))
        {
            Debug.LogWarning("No KeplerPrefabs component found.");
            Pause();
            return;
        }

        // Create all objects assigned in the inspector
        prefabs.InstantiateAllPrefabs();

        star = prefabs.star;
        planet1 = prefabs.planet1;
        planet2 = prefabs.planet2;

        Reset();

        if (planet1)
        {
            planet1Light = planet1.GetComponentInChildren<Light>();
        }

        if (planet2)
        {
            planet2Light = planet2.GetComponentInChildren<Light>();
        }

        if (prefabs.centerOfMass)
        {
            prefabs.centerOfMass.localPosition += 0.02f * Vector3.down;
        }
    }

    private void FixedUpdate()
    {
        if (paused)
        {
            return;
        }

        if (!star || !planet1)
        {
            return;
        }

        if (resetAfterOnePeriod)
        {
            // Re-establish the system to exact initial positions after one period to avoid numerical errors
            if (resetTimer >= Period)
            {
                resetTimer = 0;
                planet1.Position = initPlanet1Position;
                Vector3 position1 = initPlanet1Position - star.Position;
                theta1 = Mathf.Atan2(position1.y, position1.x);
                theta2 = theta1 + initPlanet2OffsetAngle * Mathf.Deg2Rad;
                if (planet2)
                {
                    planet2.Position = initPlanet2Position;
                }
                //Debug.Log("Resetting sim...");
            }

            resetTimer += timeScale * Time.fixedDeltaTime;
        }

        // Update planet positions
        float substep = timeScale * Time.fixedDeltaTime / numSubsteps;
        for (int i = 1; i <= numSubsteps; i++)
        {
            StepForward(planet1, ref theta1, substep);

            if (planet2)
            {
                StepForward(planet2, ref theta2, substep);
            }
        }

        if (planet1Light)
        {
            planet1Light.transform.forward = (planet1.Position - star.Position).normalized;
        }

        if (planet2Light)
        {
            planet2Light.transform.forward = (planet2.Position - star.Position).normalized;
        }

        // Update the current force between the two bodies (for UIManager)
        currentForce = -(NewtonG * star.Mass / planet1.Position.sqrMagnitude) * planet1.Position.normalized;
    }

    private void StepForward(CelestialBody planet, ref float theta, float deltaTime)
    {
        // Solve the equation of motion in polar coordinates
        Vector3 vectorR = planet.Position - star.Position;

        // Update position (direction of rotation is given by the signed L value)
        theta += orbitalParameters.L * deltaTime / vectorR.sqrMagnitude;
        float r = StarToPlanetDistance(theta);
        Vector3 position = new Vector3(r * Mathf.Cos(theta), r * Mathf.Sin(theta));
        planet.Position = star.Position + position;
    }

    public override void Reset()
    {
        resetTimer = 0;

        if (!star || !planet1)
        {
            return;
        }

        // Star
        star.Position = starPosition;
        star.Mass = starMass;
        star.Radius = starRadius;  //Mathf.Pow(3f * star.Mass / 4f / Mathf.PI, 0.333f);
        star.CanRotate = false;

        // Planet
        if (startAtPerihelion)
        {
            Vector3 direction = (starAtFocus == Focus.Left) ? Vector3.left : Vector3.right;
            initPlanet1Position = star.Position + perihelionDistance * direction;
        }
        else
        {
            float aphelionDistance = (1 + Eccentricity) * SemiMajorAxis;
            Vector3 direction = (starAtFocus == Focus.Left) ? Vector3.right : Vector3.left;
            initPlanet1Position = star.Position + aphelionDistance * direction;
        }
        planet1.Position = initPlanet1Position;
        planet1.Radius = planet1Radius;
        planet1.CanRotate = false;

        orbitSign = (starAtFocus == Focus.Left) ? -1f : 1f;
        orbitalParameters = new OrbitalParameters(SemiMajorAxis, Eccentricity, SemiLatusRectum, AngularMomentum, Energy, Period);

        Vector3 position1 = initPlanet1Position - star.Position;
        theta1 = Mathf.Atan2(position1.y, position1.x);

        //Debug.Log(transform.name + " semi-major axis is " + SemiMajorAxis);
        //Debug.Log(transform.name + " period is " + Period);
        //Debug.Log(transform.name + " G = " + NewtonG);
        //Debug.Log(transform.name + " P^2/a^3 = " + Period * Period / SemiMajorAxis / SemiMajorAxis / SemiMajorAxis);

        if (planet2)
        {
            planet2.Radius = planet2Radius;
            planet2.CanRotate = false;

            // Initial position
            theta2 = theta1 + initPlanet2OffsetAngle * Mathf.Deg2Rad;
            float rMagnitude = StarToPlanetDistance(theta2);
            Vector3 position2 = new Vector3(rMagnitude * Mathf.Cos(theta2), rMagnitude * Mathf.Sin(theta2));
            planet2.Position = star.Position + position2;
            initPlanet2Position = planet2.Position;
        }
    }

    public void HideStar()
    {
        star.transform.GetComponent<MeshRenderer>().enabled = false;
    }

    public float StarToPlanetDistance(float theta)
    {
        // Theta is always the angle wrt the positive x-axis (positive is CCW)
        return orbitalParameters.p / (1f + orbitSign * orbitalParameters.e * Mathf.Cos(theta));
    }

    public struct OrbitalParameters
    {
        public float a;  // Semimajor axis
        public float e;  // Eccentricity
        public float p;  // Semi-latus rectum
        public float L;  // Angular momentum (specific)
        public float E;  // Energy (specific)
        public float T;  // Orbital period

        public OrbitalParameters(float a, float e, float p, float L, float E, float T)
        {
            this.a = a;
            this.e = e;
            this.p = p;
            this.L = L;
            this.E = E;
            this.T = T;
        }
    }
}
