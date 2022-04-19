using TMPro;
using UnityEngine;

public class KeplerSlideController : SimulationSlideController
{
    [Header("Control")]
    [SerializeField] private StartButton startButton;
    [SerializeField] private TimerButton timerButton;
    [SerializeField] private bool stopTimerAutomatically = true;
    [SerializeField] private bool resetOnSlideTransition;
    private bool simHasStarted;

    [Header("COM")]
    [SerializeField] private bool showCenterOfMass;

    [Header("Star")]
    [SerializeField] private bool hideStar;
    [SerializeField] private bool showStarLabel;
    [SerializeField] private Material starMaterial;

    [Header("First Planet")]
    [SerializeField] private bool showPositionVector1;
    [SerializeField] private bool showPlanetLabel1;
    [SerializeField] private bool showOrbitSector1;

    [Header("Orbit")]
    [SerializeField] private bool showOrbit;
    [SerializeField] private int numOrbitPoints = 500;
    [SerializeField] private bool showSemiMajorAxis;
    [SerializeField] private bool showSemiMinorAxis;
    [SerializeField] private bool showEllipseCenter;

    [Header("Angular Momentum")]
    [SerializeField] private bool showAngularMomentumVector;
    [SerializeField] private float angularMomentumVectorScale = 1f;

    [Header("Second Planet")]
    [SerializeField] private bool showPlanet2;
    [SerializeField] private bool showPositionVector2;
    [SerializeField] private bool showOrbitSector2;

    [Header("Text Displays")]
    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private TextMeshProUGUI area1;
    [SerializeField] private TextMeshProUGUI area2;

    private KeplerSimulation keplerSim;
    private KeplerPrefabManager prefabs;

    private float timerTime;
    private bool timerIsRunning;
    private float theta0;

    private Material prevMaterial;

    private void Awake()
    {
        keplerSim = (KeplerSimulation)simulation;
        if (!keplerSim.TryGetComponent(out prefabs))
        {
            Debug.LogWarning("KeplerSlideController did not find a KeplerPrefabManager");
        }
    }

    private void Start()
    {
        SetStarVisibility();
        SetAngularMomentumVector();
        SetOrbit();
    }

    private void FixedUpdate()
    {
        if (keplerSim.paused)
        {
            return;
        }

        // No vectors, trails, etc. to update without a prefabManager
        if (prefabs == null)
        {
            return;
        }

        UpdateVectors();

        if (timerIsRunning)
        {
            timerTime += keplerSim.timeScale * Time.fixedDeltaTime;

            if (stopTimerAutomatically)
            {
                // Check if we are back where we started
                float theta1 = keplerSim.theta1;
                if (theta1 < 0 || theta1 < theta0)
                {
                    theta1 += 2 * Mathf.PI;
                }
                float thetaSwept = theta1 - theta0;
                if (Mathf.Abs(2 * Mathf.PI - thetaSwept) / (2 * Mathf.PI) <= 0.005f)
                {
                    ToggleTimer();
                    UpdateTimerText(keplerSim.Period);
                    UpdateAreaTexts(keplerSim.Period);
                    UpdateOrbitSectors(true);
                    return;
                }
            }

            UpdateTimerText(timerTime);
            UpdateAreaTexts(timerTime);
            UpdateOrbitSectors();

            if (timerTime >= 10)
            {
                ToggleTimer();
            }
        }
    }

    public override void ShowAndHideUIElements()
    {
        if (prefabs == null)
        {
            return;
        }

        prefabs.SetCenterOfMassVisibility(showCenterOfMass);
        prefabs.SetStarLabelVisibility(showStarLabel);
        prefabs.SetPositionVector1Visibility(showPositionVector1);
        prefabs.SetOrbitSector1Visibility(showOrbitSector1);
        prefabs.SetOrbit1Visibility(showOrbit);
        prefabs.SetAngularMomentumVectorVisibility(showAngularMomentumVector);
        prefabs.SetPlanetLabelVisibility(showPlanetLabel1);
        prefabs.SetPlanet2Visibility(showPlanet2);
        prefabs.SetPositionVector2Visibility(showPositionVector2);
        prefabs.SetOrbitSector2Visibility(showOrbitSector2);
        prefabs.SetSemiMajorAxisVisibility(showSemiMajorAxis);
        prefabs.SetSemiMinorAxisVisibility(showSemiMinorAxis);

        simHasStarted = !keplerSim.paused;
        SetStartButtonText();
        SetTimerButtonText();

        if (starMaterial)
        {
            MeshRenderer mr = prefabs.star.GetComponent<MeshRenderer>();
            prevMaterial = mr.material;
            mr.material = starMaterial;
        }

        if (resetOnSlideTransition)
        {
            keplerSim.Reset();
        }
    }

    private void OnApplicationQuit()
    {
        prevMaterial = null;
    }

    private void OnDisable()
    {
        timerTime = 0;
        timerIsRunning = false;
        UpdateTimerText(0);
        UpdateAreaTexts(0);
        ClearOrbitSectors();

        if (prevMaterial)
        {
            prefabs.star.GetComponent<MeshRenderer>().material = prevMaterial;
            prevMaterial = null;
        }
    }

    private void UpdateVectors()
    {
        // Radius vectors
        if (showPositionVector1 && prefabs.positionVector1 != null)
        {
            // TODO setting line width every frame is wasteful
            //prefabs.positionVector.SetLineWidth(positionVectorLineWidth);
            prefabs.positionVector1.SetPositions(keplerSim.star.Position, keplerSim.planet1.Position);
            prefabs.positionVector1.Redraw();
        }

        if (showPositionVector2 && prefabs.positionVector2 != null && prefabs.planet2 != null)
        {
            // TODO setting line width every frame is wasteful
            //prefabs.positionVector2.SetLineWidth(positionVectorLineWidth);
            prefabs.positionVector2.SetPositions(keplerSim.star.Position, prefabs.planet2.Position);
            prefabs.positionVector2.Redraw();
        }
    }

    private void UpdateOrbitSectors(bool closeTheLoop = false)
    {
        if (closeTheLoop)
        {
            if (prefabs.orbitSector1)
            {
                prefabs.orbitSector1.CloseSector();
            }

            if (prefabs.orbitSector2)
            {
                prefabs.orbitSector2.CloseSector();
            }

            return;
        }

        if (prefabs.orbitSector1)
        {
            Vector3 newPosition = keplerSim.planet1.Position - keplerSim.star.Position;

            // Always add the first vertex after the center point
            if (prefabs.orbitSector1.PositionCount < 2)
            {
                prefabs.orbitSector1.AddVertex(keplerSim.planet1.Position);
                return;
            }

            Vector3 lastPosition = prefabs.orbitSector1.GetLastPosition() - keplerSim.star.Position;
            float angle = Vector3.Angle(lastPosition, newPosition) * Mathf.Deg2Rad;
            if (angle >= 2 * Mathf.PI / prefabs.orbitSector1.maxNumVertices)
            {
                //Debug.Log("New position at angle " + angle * Mathf.Rad2Deg + " deg");
                prefabs.orbitSector1.AddVertex(keplerSim.planet1.Position);
            }
        }

        if (prefabs.orbitSector2)
        {
            Vector3 newPosition = prefabs.planet2.Position - keplerSim.star.Position;

            // Always add the first vertex after the center point
            if (prefabs.orbitSector2.PositionCount < 2)
            {
                prefabs.orbitSector2.AddVertex(prefabs.planet2.Position);
                return;
            }

            Vector3 lastPosition = prefabs.orbitSector2.GetLastPosition() - keplerSim.star.Position;
            float angle = Vector3.Angle(lastPosition, newPosition) * Mathf.Deg2Rad;
            if (angle >= 2 * Mathf.PI / prefabs.orbitSector2.maxNumVertices)
            {
                prefabs.orbitSector2.AddVertex(prefabs.planet2.Position);
            }
        }
    }

    private void ClearOrbitSectors()
    {
        if (prefabs.orbitSector1)
        {
            prefabs.orbitSector1.Clear();
        }

        if (prefabs.orbitSector2)
        {
            prefabs.orbitSector2.Clear();
        }
    }

    // To be called by startButton
    public void TogglePlayPause()
    {
        keplerSim.TogglePlayPause();
        SetStartButtonText();
    }

    // To be called by resetButton
    public void Reset()
    {
        keplerSim.Reset();
        keplerSim.Pause();
        simHasStarted = false;
        timerTime = 0;
        timerIsRunning = false;
        UpdateVectors();
        ClearOrbitSectors();
        SetStartButtonText();
        SetTimerButtonText();
        UpdateTimerText(0);
        UpdateAreaTexts(0);
    }

    // To be called by timerButton
    public void ToggleTimer()
    {
        if (keplerSim.paused)
        {
            return;
        }

        timerIsRunning = !timerIsRunning;
        if (timerIsRunning)
        {
            timerTime = 0;
            theta0 = keplerSim.theta1;
            if (theta0 < 0)
            {
                theta0 += Mathf.PI;
            }

            if (prefabs.orbitSector1)
            {
                prefabs.orbitSector1.Clear();
                prefabs.orbitSector1.AddVertex(keplerSim.star.Position);
            }

            if (prefabs.orbitSector2)
            {
                prefabs.orbitSector2.Clear();
                prefabs.orbitSector2.AddVertex(keplerSim.star.Position);
            }
        }

        SetTimerButtonText();
    }

    private void SetStartButtonText()
    {
        if (startButton == null)
        {
            return;
        }

        if (keplerSim.paused)
        {
            if (simHasStarted)
            {
                startButton.ShowResumeText();
            }
            else
            {
                startButton.ShowStartText();
                simHasStarted = true;
            }
        }
        else
        {
            startButton.ShowPauseText();
        }
    }

    private void SetTimerButtonText()
    {
        if (timerButton)
        {
            if (timerIsRunning)
            {
                timerButton.ShowStopText();
            }
            else
            {
                timerButton.ShowStartText();
            }
        }
    }

    private void UpdateTimerText(float newTime)
    {
        if (timer != null)
        {
            timer.text = newTime.ToString("0.00");
        }
    }

    private void UpdateAreaTexts(float deltaTime)
    {
        if (area1 != null)
        {
            float area = 0.5f * keplerSim.AngularMomentum * deltaTime;
            area1.text = area.ToString("0.00");

            if (area2 != null)
            {
                area2.text = area1.text;
            }
        }
    }

    private void SetStarVisibility()
    {
        if (hideStar)
        {
            keplerSim.HideStar();
        }
    }

    private void SetAngularMomentumVector()
    {
        if (showAngularMomentumVector && prefabs.angularMomentumVector != null)
        {
            Vector3 tail = keplerSim.star.Position;
            Vector3 head = tail - angularMomentumVectorScale * keplerSim.AngularMomentum * Vector3.forward;
            prefabs.angularMomentumVector.SetPositions(tail, head);
            prefabs.angularMomentumVector.Redraw();
        }
    }

    private void SetOrbit()
    {
        if (showOrbit && prefabs.orbit1 != null)
        {
            Vector3[] positions = new Vector3[numOrbitPoints];
            for (int i = 0; i < numOrbitPoints; i++)
            {
                float theta = i * 2f * Mathf.PI / numOrbitPoints;
                float r = keplerSim.StarToPlanetDistance(theta);
                positions[i] = keplerSim.star.Position + new Vector3(r * Mathf.Cos(theta), r * Mathf.Sin(theta), 0);
            }

            prefabs.orbit1.positionCount = numOrbitPoints;
            prefabs.orbit1.SetPositions(positions);
        }

        if (showSemiMajorAxis && prefabs.semiMajorAxis)
        {
            float a = keplerSim.orbitalParameters.a;
            float e = keplerSim.orbitalParameters.e;

            Vector3 direction = (keplerSim.starAtFocus == KeplerSimulation.Focus.Left) ? Vector3.right : Vector3.left;
            Vector3 tail = keplerSim.star.Position + e * a * direction;
            Vector3 head = tail + a * direction;
            prefabs.semiMajorAxis.SetPositions(tail, head);
            prefabs.semiMajorAxis.Redraw();
        }

        if (showSemiMinorAxis && prefabs.semiMinorAxis)
        {
            float a = keplerSim.orbitalParameters.a;
            float e = keplerSim.orbitalParameters.e;

            Vector3 direction = (keplerSim.starAtFocus == KeplerSimulation.Focus.Left) ? Vector3.right : Vector3.left;
            Vector3 tail = keplerSim.star.Position + e * a * direction;
            Vector3 head = tail + Mathf.Sqrt(1 - e * e) * a * Vector3.up;
            prefabs.semiMinorAxis.SetPositions(tail, head);
            prefabs.semiMinorAxis.Redraw();
        }
    }
}
