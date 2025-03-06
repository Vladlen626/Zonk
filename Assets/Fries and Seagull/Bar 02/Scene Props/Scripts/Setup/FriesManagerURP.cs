﻿using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
# if UNITY_EDITOR
using UnityEditor;
using Seagull.Bar_02.Inspector;
# endif

namespace Seagull.Bar_02.SceneProps.Setup {
    public class FriesManagerURP : MonoBehaviour {
# if UNITY_EDITOR
        public VolumeProfile volumeProfile;
        
        [Tooltip("Post Process effect including glowing will only show to these cameras")]
        public List<Camera> gameCameras = new();
        
        [AButton("Initialize")] [IgnoreInInspector]
        public Action initialize;

        private void Reset() {
            initialize = init;
        }

        private void init() {
            if (gameCameras == null || gameCameras.Count == 0) {
                Debug.LogError("Please provide at least 1 valid camera to Game Cameras field.");
                return;
            }
            
            foreach (var camera in gameCameras) {
                camera.GetComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;
                Volume volume = camera.GetComponent<Volume>();
                if (volume) volume.sharedProfile = volumeProfile;
                else {
                    volume = camera.gameObject.AddComponent<Volume>();
                    volume.sharedProfile = volumeProfile;
                }
            }
            Debug.Log($"Init post-processor settings for Universal Rendering Pipeline successfully.");
        }
# endif
        
        public static void setupLight() {
# if UNITY_EDITOR
            UniversalRendererData universalRendererData = AssetDatabase.LoadAssetAtPath<UniversalRendererData>("Assets/Fries and Seagull/Bar 02/Scene Props/Lighting Sample (URP-HighFidelity-Renderer).asset");
            
            UniversalRenderPipelineAsset urpAsset =
                GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (urpAsset == null) {
                Debug.LogError("No active Universal Render Pipeline is found!");
                return;
            }
            
            FieldInfo rendererDataListField = typeof(UniversalRenderPipelineAsset).GetField(
                "m_RendererDataList",
                BindingFlags.Instance | BindingFlags.NonPublic
            );

            ScriptableRendererData[] rendererDataList =
                rendererDataListField.GetValue(urpAsset) as ScriptableRendererData[];
            if (rendererDataList == null || rendererDataList.Length == 0) 
                rendererDataList = new ScriptableRendererData[1];
            
            string previousDataGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath((UniversalRendererData)rendererDataList[0]));
            EditorPrefs.SetString("Fries.Previous_Data_Guid", previousDataGuid);
            
            rendererDataList[0] = universalRendererData;
            rendererDataListField.SetValue(urpAsset, rendererDataList);

            EditorUtility.SetDirty(urpAsset);
            AssetDatabase.SaveAssets();
            
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(48f/255f, 50f/255f, 152f/255f);
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogColor = new Color(53f/255f, 17f/255f, 58f/255f);
            RenderSettings.fogStartDistance = 0f;
            RenderSettings.fogEndDistance = 316.7f;
            
            AssetDatabase.Refresh();
            EditorUtility.RequestScriptReload();
# endif
        }

        public static void unsetLight() {
# if UNITY_EDITOR
            UniversalRenderPipelineAsset urpAsset =
                GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (urpAsset == null) {
                Debug.LogError("No active Universal Render Pipeline is found!");
                return;
            }
            
            FieldInfo rendererDataListField = typeof(UniversalRenderPipelineAsset).GetField(
                "m_RendererDataList",
                BindingFlags.Instance | BindingFlags.NonPublic
            );

            ScriptableRendererData[] rendererDataList =
                rendererDataListField.GetValue(urpAsset) as ScriptableRendererData[];
            if (rendererDataList == null || rendererDataList.Length == 0) 
                rendererDataList = new ScriptableRendererData[1];
            
            string previousDataGuid = EditorPrefs.GetString("Fries.Previous_Data_Guid");
            UniversalRendererData previousRenderData = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(AssetDatabase.GUIDToAssetPath(previousDataGuid));
            
            rendererDataList[0] = previousRenderData;
            rendererDataListField.SetValue(urpAsset, rendererDataList);

            EditorUtility.SetDirty(urpAsset);
            AssetDatabase.SaveAssets();

            RenderSettings.ambientMode = AmbientMode.Skybox;
            RenderSettings.ambientLight = new Color(54f/255f, 58f/255f, 66f/255f);
            RenderSettings.fog = false;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = new Color(128f/255f, 128f/255f, 128f/255f);
            RenderSettings.fogStartDistance = 0f;
            RenderSettings.fogEndDistance = 300f;
            
            AssetDatabase.Refresh();
            EditorUtility.RequestScriptReload();
# endif
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(FriesManagerURP))]
    public class FriesInitializerInspector : AnInspector { }
#endif
}