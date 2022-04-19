using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(KeplerSimulation)), CanEditMultipleObjects]
public class KeplerSimulationEditor : Editor
{
    KeplerPrefabManager prefabs;

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        KeplerSimulation sim = (KeplerSimulation)target;

        sim.TryGetComponent(out prefabs);

        EditorGUILayout.LabelField("Simulation", EditorStyles.boldLabel);
        //sim.newtonG = EditorGUILayout.FloatField("Newton G", sim.newtonG);
        sim.numSubsteps = EditorGUILayout.IntField("Num substeps", sim.numSubsteps);
        sim.resetAfterOnePeriod = EditorGUILayout.Toggle("Reset after one period", sim.resetAfterOnePeriod);
        sim.unitTime = (KeplerSimulation.UnitTime)EditorGUILayout.EnumPopup("Unit time", sim.unitTime);
        sim.unitLength = (KeplerSimulation.UnitLength)EditorGUILayout.EnumPopup("Unit length", sim.unitLength);
        sim.unitMass = (KeplerSimulation.UnitMass)EditorGUILayout.EnumPopup("Unit mass", sim.unitMass);
        sim.timeScale = EditorGUILayout.FloatField("Time scale", Mathf.Max(0, sim.timeScale));

        EditorGUILayout.Space();
        //prefabs.starPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefabs.starPrefab, typeof(GameObject), false);
        if (prefabs.starPrefab)
        {
            EditorGUILayout.LabelField("Star", EditorStyles.boldLabel);
            sim.starMass = EditorGUILayout.FloatField("Mass", Mathf.Max(0, sim.starMass));
            sim.starRadius = EditorGUILayout.FloatField("Radius", Mathf.Max(0, sim.starRadius));
            sim.starPosition = EditorGUILayout.Vector2Field("Position", sim.starPosition);
            sim.starAtFocus = (KeplerSimulation.Focus)EditorGUILayout.EnumPopup("At focus", sim.starAtFocus);
        }

        EditorGUILayout.Space();
        //prefabs.planet1Prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefabs.planet1Prefab, typeof(GameObject), false);
        if (prefabs.planet1Prefab)
        {
            EditorGUILayout.LabelField("Planet 1", EditorStyles.boldLabel);
            sim.planet1Radius = EditorGUILayout.FloatField("Radius", Mathf.Max(0, sim.planet1Radius));
            sim.perihelionDistance = EditorGUILayout.FloatField("Perihelion distance", Mathf.Max(0, sim.perihelionDistance));
            sim.startAtPerihelion = EditorGUILayout.Toggle("Start at perihelion", sim.startAtPerihelion);
            sim.eccentricity = EditorGUILayout.FloatField("Eccentricity", Mathf.Max(0, Mathf.Min(1, sim.eccentricity)));
            sim.orbitDirection = (KeplerSimulation.OrbitDirection)EditorGUILayout.EnumPopup("Orbit direction", sim.orbitDirection);
        }

        EditorGUILayout.Space();
        if (prefabs.planet2Prefab)
        {
            EditorGUILayout.LabelField("Planet 2", EditorStyles.boldLabel);
            sim.planet2Radius = EditorGUILayout.FloatField("Radius", Mathf.Max(0, sim.planet2Radius));
            sim.initPlanet2OffsetAngle = EditorGUILayout.FloatField("Init Offset Angle", sim.initPlanet2OffsetAngle);
        }

        //EditorUtility.SetDirty(target);
        //serializedObject.Update();
        //serializedObject.ApplyModifiedProperties();
    }
}
