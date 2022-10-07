using UnityEngine;
using UnityEditor;

namespace Nightvision
{
    [CustomEditor(typeof(Nightvision))]
    public class NightvisionEditor : Editor
    {
        Camera camera;
        bool showVignetteMode = true;
        bool showNoiseMode = true;
        Nightvision nv;

        public override void OnInspectorGUI()
        {
            nv = (Nightvision)target;

            DisplayHDRHelp();
            DisplayPositiveFloat("Power", ref nv.Settings.Power, NightvisionSettings.DefaultPower);

            EditorGUILayout.Space();

            DisplayVignetteSettings();
            DisplayNoiseSettings();
            DisplayColor("Effect color", ref nv.Settings.EffectColor, Color.green);

            nv.UpdateMaterial();
        }

        void DisplayHDRHelp()
        {
            if (camera == null)
            {
                camera = nv.GetComponent<Camera>();
            }

            if (camera.allowHDR == false)
            {
                EditorGUILayout.HelpBox("Highly recommended enable HDR in your camera to get good output image quality.", MessageType.Info);
            }
        }
        void DisplayVignetteSettings()
        {
            EditorGUILayout.BeginHorizontal();
            showVignetteMode = EditorGUILayout.Foldout(showVignetteMode, "Vignette", true);
            nv.Settings.Vignette.Mode = (VignetteSettings.VignetteMode)EditorGUILayout.EnumPopup(nv.Settings.Vignette.Mode);
            EditorGUILayout.EndHorizontal();
            if (showVignetteMode)
            {
                EditorGUI.indentLevel++;
                if (nv.Settings.Vignette.Mode == VignetteSettings.VignetteMode.Off)
                {
                    EditorGUILayout.LabelField("Nothing to show");
                }
                else if (nv.Settings.Vignette.Mode == VignetteSettings.VignetteMode.Texture)
                {
                    DisplayColor("Color", ref nv.Settings.Vignette.color, Color.black);
                    nv.Settings.Vignette.Texture.Texture = EditorGUILayout.ObjectField("Texture", nv.Settings.Vignette.Texture.Texture, typeof(Texture2D), true) as Texture2D;
                }
                else if (nv.Settings.Vignette.Mode == VignetteSettings.VignetteMode.Procedural)
                {
                    DisplayColor("Color", ref nv.Settings.Vignette.color, Color.black);
                    DisplayPositiveFloat("Radius", ref nv.Settings.Vignette.Procedural.Radius, VignetteSettings.ProceduralSettings.DefaultRadius);
                    DisplayFloat("Sharpness", ref nv.Settings.Vignette.Procedural.Sharpness, VignetteSettings.ProceduralSettings.DefaultSharpness);
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }
        }
        void DisplayNoiseSettings()
        {
            EditorGUILayout.BeginHorizontal();
            showNoiseMode = EditorGUILayout.Foldout(showNoiseMode, "Noise", true);
            nv.Settings.Noise.Mode = (NoiseSettings.NoiseMode)EditorGUILayout.EnumPopup(nv.Settings.Noise.Mode);
            EditorGUILayout.EndHorizontal();
            if (showNoiseMode)
            {
                EditorGUI.indentLevel++;
                if (nv.Settings.Noise.Mode == NoiseSettings.NoiseMode.Off)
                {
                    EditorGUILayout.LabelField("Nothing to show");
                }
                else if (nv.Settings.Noise.Mode == NoiseSettings.NoiseMode.Texture)
                {
                    nv.Settings.Noise.Texture.Texture = EditorGUILayout.ObjectField("Texture", nv.Settings.Noise.Texture.Texture, typeof(Texture2D), true) as Texture2D;
                    DisplayPositiveFloat("Scale", ref nv.Settings.Noise.Texture.Scale, NoiseSettings.TextureSettings.DefaultScale);
                    DisplayPositiveFloat("Update Time", ref nv.Settings.Noise.UpdateTime, NoiseSettings.DefaultUpdateTime);
                    DisplayFloat("Power", ref nv.Settings.Noise.Power, NoiseSettings.DefaultPower);
                }
                else if (nv.Settings.Noise.Mode == NoiseSettings.NoiseMode.Procedural)
                {
                    DisplayPositiveFloat("Update Time", ref nv.Settings.Noise.UpdateTime, NoiseSettings.DefaultUpdateTime);
                    DisplayFloat("Power", ref nv.Settings.Noise.Power, NoiseSettings.DefaultPower);
                }
                EditorGUI.indentLevel--;
            }
        }

        //Property

        void DisplayColor(string Name, ref Color Value, Color DefaultValue)
        {
            EditorGUILayout.BeginHorizontal();

            Value = EditorGUILayout.ColorField(Name, Value);
            ShowResetButton(ref Value, DefaultValue);

            EditorGUILayout.EndHorizontal();
        }
        void DisplayFloat(string Name, ref float Value, float DefaultValue)
        {
            EditorGUILayout.BeginHorizontal();

            Value = EditorGUILayout.FloatField(Name, Value);
            AutoResetButton(ref Value, DefaultValue);

            EditorGUILayout.EndHorizontal();
        }
        void DisplayPositiveFloat(string Name, ref float Value, float DefaultValue)
        {
            EditorGUILayout.BeginHorizontal();

            Value = EditorGUILayout.FloatField(Name, Mathf.Max(Value, 0));
            AutoResetButton(ref Value, DefaultValue);

            EditorGUILayout.EndHorizontal();
        }

        void AutoResetButton<T>(ref T Value, T Default)
        {
            if (!Value.Equals(Default))
            {
                ShowResetButton(ref Value, Default);
            }
        }
        void ShowResetButton<T>(ref T Value, T Default)
        {
            if (GUILayout.Button("Reset", GUILayout.ExpandWidth(false)))
            {
                Value = Default;
            }
        }
    }
}