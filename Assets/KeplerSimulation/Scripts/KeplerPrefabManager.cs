using UnityEngine;

[RequireComponent(typeof(KeplerSimulation))]
public class KeplerPrefabManager : MonoBehaviour
{
    [Header("Star")]
    public GameObject starPrefab;

    [Header("Planet 1")]
    public GameObject planet1Prefab;
    [SerializeField] private GameObject positionVector1Prefab;
    [SerializeField] private GameObject orbit1Prefab;
    [SerializeField] private GameObject orbitSector1Prefab;

    [Header("Planet 2")]
    public GameObject planet2Prefab;
    [SerializeField] private GameObject orbitSector2Prefab;
    [SerializeField] private GameObject positionVector2Prefab;

    [Header("Others")]
    [SerializeField] private GameObject centerOfMassPrefab;
    [SerializeField] private GameObject angularMomentumVectorPrefab;
    [SerializeField] private GameObject semiMajorAxisPrefab;
    [SerializeField] private GameObject semiMinorAxisPrefab;

    [HideInInspector] public CelestialBody star;
    [HideInInspector] public CelestialBody planet1;
    [HideInInspector] public Transform centerOfMass;
    [HideInInspector] public Vector positionVector1;
    [HideInInspector] public LineRenderer orbit1;
    [HideInInspector] public Sector orbitSector1;
    [HideInInspector] public Vector angularMomentumVector;
    [HideInInspector] public CelestialBody planet2;
    [HideInInspector] public Sector orbitSector2;
    [HideInInspector] public Vector positionVector2;
    [HideInInspector] public Vector semiMajorAxis;
    [HideInInspector] public Vector semiMinorAxis;

    public void SetCenterOfMassVisibility(bool visible)
    {
        if (centerOfMass)
        {
            centerOfMass.gameObject.SetActive(visible);
        }
    }

    public void SetPositionVector1Visibility(bool visible)
    {
        if (positionVector1)
        {
            positionVector1.gameObject.SetActive(visible);
        }
    }

    public void SetOrbit1Visibility(bool visible)
    {
        if (orbit1)
        {
            orbit1.gameObject.SetActive(visible);
        }
    }

    public void SetOrbitSector1Visibility(bool visible)
    {
        if (orbitSector1)
        {
            orbitSector1.gameObject.SetActive(visible);
        }
    }

    public void SetAngularMomentumVectorVisibility(bool visible)
    {
        if (angularMomentumVector)
        {
            angularMomentumVector.gameObject.SetActive(visible);
        }
    }

    public void SetStarLabelVisibility(bool visible)
    {
        Transform label = star.transform.Find("Label");
        if (label)
        {
            label.gameObject.SetActive(visible);
        }
    }

    public void SetPlanetLabelVisibility(bool visible)
    {
        Transform label = planet1.transform.Find("Label");
        if (label)
        {
            label.gameObject.SetActive(visible);
        }
    }

    public void SetPlanet2Visibility(bool visible)
    {
        if (planet2)
        {
            planet2.gameObject.SetActive(visible);
        }
    }

    public void SetPositionVector2Visibility(bool visible)
    {
        if (positionVector2)
        {
            positionVector2.gameObject.SetActive(visible);
        }
    }

    public void SetOrbitSector2Visibility(bool visible)
    {
        if (orbitSector2)
        {
            orbitSector2.gameObject.SetActive(visible);
        }
    }

    public void SetSemiMajorAxisVisibility(bool visible)
    {
        if (semiMajorAxis)
        {
            semiMajorAxis.gameObject.SetActive(visible);
        }
    }

    public void SetSemiMinorAxisVisibility(bool visible)
    {
        if (semiMinorAxis)
        {
            semiMinorAxis.gameObject.SetActive(visible);
        }
    }

    public void InstantiateAllPrefabs()
    {
        if (starPrefab)
        {
            star = Instantiate(starPrefab, transform).GetComponent<CelestialBody>();
            star.gameObject.name = "Star";
        }

        if (centerOfMassPrefab)
        {
            centerOfMass = Instantiate(centerOfMassPrefab, transform).transform;
            centerOfMass.name = "Center of Mass";
        }

        if (planet1Prefab)
        {
            planet1 = Instantiate(planet1Prefab, Vector3.zero, Quaternion.identity, transform).GetComponent<CelestialBody>();
            planet1.gameObject.name = "Planet 1";
        }

        if (positionVector1Prefab)
        {
            positionVector1 = Instantiate(positionVector1Prefab, transform).GetComponent<Vector>();
            positionVector1.SetPositions(Vector3.zero, Vector3.zero);
            positionVector1.name = "Position Vector 1";
        }

        if (orbit1Prefab)
        {
            orbit1 = Instantiate(orbit1Prefab, transform).GetComponent<LineRenderer>();
            orbit1.positionCount = 0;
            orbit1.name = "Orbit 1";
        }

        if (orbitSector1Prefab)
        {
            orbitSector1 = Instantiate(orbitSector1Prefab, transform).GetComponent<Sector>();
            orbitSector1.name = "Orbit Sector 1";
        }

        if (angularMomentumVectorPrefab)
        {
            angularMomentumVector = Instantiate(angularMomentumVectorPrefab, transform).GetComponent<Vector>();
            angularMomentumVector.SetPositions(Vector3.zero, Vector3.zero);
            angularMomentumVector.name = "Angular Momentum";
        }

        if (planet2Prefab)
        {
            planet2 = Instantiate(planet2Prefab, transform).GetComponent<CelestialBody>();
            planet2.gameObject.name = "Planet 2";
        }

        if (orbitSector2Prefab)
        {
            orbitSector2 = Instantiate(orbitSector2Prefab, transform).GetComponent<Sector>();
            orbitSector2.name = "Orbit Sector 2";
        }

        if (positionVector2Prefab)
        {
            positionVector2 = Instantiate(positionVector2Prefab, transform).GetComponent<Vector>();
            positionVector2.SetPositions(Vector3.zero, Vector3.zero);
            positionVector2.name = "Position Vector 2";
        }

        if (semiMajorAxisPrefab)
        {
            semiMajorAxis = Instantiate(semiMajorAxisPrefab, transform).GetComponent<Vector>();
            semiMajorAxis.SetPositions(Vector3.zero, Vector3.zero);
            semiMajorAxis.name = "Semi-Major Axis";
        }

        if (semiMinorAxisPrefab)
        {
            semiMinorAxis = Instantiate(semiMinorAxisPrefab, transform).GetComponent<Vector>();
            semiMinorAxis.SetPositions(Vector3.zero, Vector3.zero);
            semiMinorAxis.name = "Semi-Minor Axis";
        }
    }
}
