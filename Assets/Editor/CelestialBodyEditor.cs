using UnityEditor;

[CustomEditor(typeof(CelestialBody))]
public class CelestialBodyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CelestialBody cb = (CelestialBody)target;

        cb.Mass = EditorGUILayout.FloatField("Mass", cb.Mass);
        cb.Radius = EditorGUILayout.FloatField("Radius", cb.Radius);
        cb.maxRadius = EditorGUILayout.FloatField("Max Radius", cb.maxRadius);
        cb.CanRotate = EditorGUILayout.Toggle("Can Rotate", cb.CanRotate);
        if (cb.CanRotate)
        {
            cb.RotationPeriod = EditorGUILayout.FloatField("Rotation Period", cb.RotationPeriod);
        }
    }
}
