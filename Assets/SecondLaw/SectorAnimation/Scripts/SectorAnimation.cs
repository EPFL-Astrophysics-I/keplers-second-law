using System.Collections;
using UnityEngine;

public class SectorAnimation : Simulation
{
    [SerializeField] private GameObject planetPrefab;
    [SerializeField] private GameObject rVectorPrefab;
    [SerializeField] private GameObject vVectorPrefab;
    [SerializeField] private GameObject sectorPrefab;

    public Vector3 origin = Vector3.zero;
    public Vector3 initPosition = 10 * Vector3.right;
    public float angularSpeed = 1;
    public float angleToSweep = 30f;

    private Transform planet;
    private Vector vectorR;
    private Vector vectorV;
    private Sector sector;

    private float theta;
    private float radius;
    private float speed;

    [HideInInspector] public float percentComplete;

    private float AngleToSweep => angleToSweep * Mathf.Deg2Rad;

    private void Awake()
    {
        if (planetPrefab)
        {
            planet = Instantiate(planetPrefab, Vector3.zero, Quaternion.identity, transform).transform;
            planet.name = "Planet";
        }
        else
        {
            Debug.LogError("No planet prefab assigned.");
        }

        if (rVectorPrefab)
        {
            vectorR = Instantiate(rVectorPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<Vector>();
            vectorR.name = "Vector R";
        }
        else
        {
            Debug.LogError("No vector R prefab assigned.");
        }

        if (vVectorPrefab)
        {
            vectorV = Instantiate(vVectorPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<Vector>();
            vectorV.name = "Vector V";
        }
        else
        {
            Debug.LogError("No vector V prefab assigned.");
        }

        if (sectorPrefab)
        {
            sector = Instantiate(sectorPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<Sector>();
            sector.name = "Sector";
        }
        else
        {
            Debug.LogError("No triangle sector prefab assigned.");
        }

        radius = (initPosition - origin).magnitude;
    }

    private void Start()
    {
        Reset();
    }

    private IEnumerator Animate()
    {
        while (theta <= AngleToSweep)
        {
            percentComplete = theta / AngleToSweep;

            // Move planet
            theta += angularSpeed * Time.deltaTime;
            planet.position = origin + radius * (Mathf.Cos(theta) * Vector3.right + Mathf.Sin(theta) * Vector3.up);

            // Update vectors
            vectorR.SetPositions(origin, planet.position);
            vectorR.Redraw();
            Vector3 direction = -Mathf.Sin(theta) * Vector3.right + Mathf.Cos(theta) * Vector3.up;
            vectorV.SetPositions(planet.position, planet.position + speed * direction);
            vectorV.Redraw();

            // Update sector
            sector.AddVertex(planet.position);
            yield return null;
        }

        percentComplete = 1;
        yield return new WaitForSeconds(3);

        Reset();
    }

    public override void Reset()
    {
        StopAllCoroutines();

        theta = 0;
        percentComplete = 0;

        planet.position = initPosition;
        vectorR.SetPositions(origin, planet.position);
        vectorR.Redraw();
        vectorV.SetPositions(planet.position, planet.position + 5 * Vector3.up);
        vectorV.Redraw();
        speed = vectorV.Displacement.magnitude;
        sector.Clear();
        sector.AddVertex(origin);
        sector.AddVertex(planet.position);

        StartCoroutine(Animate());
    }
}
