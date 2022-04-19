using System.Collections;
using UnityEngine;

public class TriangleAnimation : Simulation
{
    [SerializeField] private GameObject planetPrefab;
    [SerializeField] private GameObject rVectorPrefab;
    [SerializeField] private GameObject vVectorPrefab;
    [SerializeField] private GameObject trianglePrefab;

    public Vector3 origin = Vector3.zero;
    public Vector3 initPosition = 10 * Vector3.right;
    public Vector3 velocity = 1 * Vector3.up;
    public float verticalDistance = 10f;

    private Transform planet;
    private Vector vectorR;
    private Vector vectorV;
    private Triangle triangle;

    [HideInInspector] public float percentComplete;

    private void Awake()
    {
        if (planetPrefab != null)
        {
            planet = Instantiate(planetPrefab, Vector3.zero, Quaternion.identity, transform).transform;
            planet.name = "Planet";
        }
        else
        {
            Debug.LogError("No planet prefab assigned.");
        }

        if (rVectorPrefab != null)
        {
            vectorR = Instantiate(rVectorPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<Vector>();
            vectorR.name = "Vector R";
        }
        else
        {
            Debug.LogError("No vector R prefab assigned.");
        }

        if (vVectorPrefab != null)
        {
            vectorV = Instantiate(vVectorPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<Vector>();
            vectorV.name = "Vector V";
        }
        else
        {
            Debug.LogError("No vector V prefab assigned.");
        }

        if (trianglePrefab != null)
        {
            triangle = Instantiate(trianglePrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<Triangle>();
            triangle.name = "Triangle";
        }
        else
        {
            Debug.LogError("No triangle sector prefab assigned.");
        }
    }

    private void Start()
    {
        Reset();
    }

    private IEnumerator Animate()
    {
        while ((planet.position - initPosition).magnitude <= verticalDistance)
        {
            percentComplete = (planet.position - initPosition).magnitude / verticalDistance;

            planet.position += velocity * Time.deltaTime;
            //vectorR.SetHeadPosition(planet.position);
            vectorV.SetPositions(planet.position, planet.position + velocity);
            vectorV.Redraw();
            triangle.SetVertex(2, planet.position);
            yield return null;
        }

        percentComplete = 1;
        yield return new WaitForSeconds(3);

        Reset();
    }

    public override void Reset()
    {
        StopAllCoroutines();
        planet.position = initPosition;
        vectorR.SetPositions(origin, planet.position);
        vectorR.Redraw();
        vectorV.SetPositions(planet.position, planet.position + velocity);
        vectorV.Redraw();
        triangle.Clear();
        triangle.SetVertex(0, origin);
        triangle.SetVertex(1, planet.position);
        percentComplete = 0;

        StartCoroutine(Animate());
    }
}
