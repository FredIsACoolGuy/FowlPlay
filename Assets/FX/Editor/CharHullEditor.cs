﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CharHullEditor : ShaderGUI
{
    public override void OnGUI (MaterialEditor materialEditor, MaterialProperty[] properties) {

        Material mat = materialEditor.target as Material;
        //Material[] mats = materialEditor.targets as Material[];

        EditorGUI.BeginChangeCheck();
        bool shadowsOn = EditorGUILayout.Toggle("Shadows Enabled", mat.IsKeywordEnabled("SHADOWS_SCREEN"));

        if (EditorGUI.EndChangeCheck()) {

            //Material[] mats = materialEditor.targets as Material[];
            if (shadowsOn == true) {
                foreach (Material m in materialEditor.targets) {
                    m.EnableKeyword("SHADOWS_SCREEN");
                }
            }
            else {
                foreach (Material m in materialEditor.targets) {
                    m.DisableKeyword("SHADOWS_SCREEN");
                }
            }
        }

        EditorGUI.BeginChangeCheck();
        bool hullOn = EditorGUILayout.Toggle("Outline Enabled", mat.GetShaderPassEnabled("ForwardBase"));

        if (EditorGUI.EndChangeCheck()) {

            foreach (Material m in materialEditor.targets) {
                m.SetShaderPassEnabled("ForwardBase", hullOn);
            }
        }

        base.OnGUI(materialEditor, properties);
    }
}
