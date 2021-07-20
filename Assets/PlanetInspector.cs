using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Planet))]
public class PlanetInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Planet planet = (Planet)target;
        if (GUILayout.Button("Generate new Crater Values"))
        {
            planet.GenerateRandomCrater();
            planet.UpdateValues();
        }
    }
}
