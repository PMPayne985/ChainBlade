using System.IO;
using System.Collections;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

using GameTexturesToolkit;
using LitJson;

namespace EditorCoroutines {
    public class gt_toolkit : EditorWindow {
        private readonly string[] mat_toggle_array = new string[] { "Metallic", "Specular", "Metallic + Specular" };
        private readonly string[] pipeline_toggle_array = new string[] { "Standard", "HDRP", "LWRP" };

        [SerializeField] private int mat_index = 0;
        [SerializeField] private int pipeline_index = 0;
        [SerializeField] private bool create_mat;

        private bool is_metallic = false;
        private bool is_specular = false;

        private static bool[] QueryWorkflowMethod(string process_method) {
            int option = EditorUtility.DisplayDialogComplex(process_method, "Please choose the PBR Workflow:", "Metallic", "Specular", "Cancel");
            bool[] workflow = new bool[2];

            switch (option) {
                case 0:
                    workflow[0] = true;
                    workflow[1] = false;
                    break;
                case 1:
                    workflow[0] = false;
                    workflow[1] = true;
                    break;
                case 2:
                    workflow[0] = false;
                    workflow[1] = false;
                    break;
            }
            return workflow;
        }

        private static int QueryPipelineType(string pipeline_type) {
            return EditorUtility.DisplayDialogComplex(pipeline_type, "Please choose the Render Pipeline:", "Standard", "HDRP", "LWRP");
        }

        private static Toolkit.PipelineType GetPipeline(int index) {
            switch(index) {
                case 1:
                    return Toolkit.PipelineType.HDRP;
                case 2:
                    return Toolkit.PipelineType.LWRP;
                default:
                    return Toolkit.PipelineType.DEFAULT;
            }
        }
        
        // - INTERFACE - GT TOOLKIT
        [MenuItem("GameTextures/GameTextures Toolkit", priority = -1000)]

        public static void ShowWindow() {
            // Show existing window instance. If one does not exist, create one.
            gt_toolkit gt = (gt_toolkit)EditorWindow.GetWindow(typeof(gt_toolkit), true, "GT Toolkit");
            gt.maxSize = new Vector2(270, 480);
            gt.minSize = gt.maxSize;
        }

        private Texture2D gt_logo;

        private void OnGUI() {
            gt_logo = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/GT_Toolkit/Editor/resources/gt_toolkit_logo.png");
            GUILayout.Space(20);
            GUILayout.Label(gt_logo);

            // TOOLKIT
            GUILayout.Space(20);
            GUILayout.Label("Import Material", EditorStyles.boldLabel);

            // Create Material
            GUILayout.BeginHorizontal();
            GUILayout.Label("Pipeline:");
            this.pipeline_index = (EditorGUILayout.Popup(this.pipeline_index, pipeline_toggle_array, GUILayout.MaxWidth(250)));
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Workflow:");
            this.mat_index = (EditorGUILayout.Popup(this.mat_index, mat_toggle_array, GUILayout.MaxWidth(250)));
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            
            GUILayout.BeginHorizontal();
            this.create_mat = EditorGUILayout.Toggle("Create Material", this.create_mat);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            // Import Button
            if (GUILayout.Button("Import Material", GUILayout.MaxWidth(300), GUILayout.MinHeight(30))) {
                if (mat_index == 0) {
                    is_metallic = true;
                    is_specular = false;
                }
                else if (mat_index == 1) {
                    is_metallic = false;
                    is_specular = true;
                }
                else if (mat_index == 2) {
                    is_metallic = true;
                    is_specular = true;
                }
                else {
                    is_metallic = false;
                    is_specular = false;
                }
                Debug.Log(create_mat);
                this.StartCoroutine(Toolkit.CreateMaterialAsync(Toolkit.GetFilePath("Import GameTextures Material", "gtex"), GetPipeline(this.pipeline_index), is_metallic, is_specular, create_mat));
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            // Import Button
            if (GUILayout.Button("Batch Import Materials", GUILayout.MaxWidth(300), GUILayout.MinHeight(30))) {
                if (mat_index == 0) {
                    is_metallic = true;
                    is_specular = false;
                }
                else if (mat_index == 1) {
                    is_metallic = false;
                    is_specular = true;
                }
                else if (mat_index == 2) {
                    is_metallic = true;
                    is_specular = true;
                }
                else {
                    is_metallic = false;
                    is_specular = false;
                }
                this.StartCoroutine(Toolkit.BatchCreateMaterialAsync(Toolkit.GetFolderPath("Import GameTextures Material - BATCH"), GetPipeline(this.pipeline_index), is_metallic, is_specular, create_mat));
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        // - INTERFACE - IMPORT MATERIAL
        [MenuItem("GameTextures/Import Material", priority = 10)]
        static void ImportSingle() {
            int pipeline = QueryPipelineType("Query Pipeline Type");
            bool[] workflow = QueryWorkflowMethod("Import Material");
            if(!(workflow[0] == false && workflow[1] == false)) {
                Toolkit.CreateMaterial(Toolkit.GetFilePath("Import GameTextures Material", "gtex"), GetPipeline(pipeline), workflow[0], workflow[1], true);
            }
        }
        // - INTERFACE - IMPORT MATERIAL BATCH
        [MenuItem("GameTextures/Batch Import Material", priority = 10)]
        static void ImportBatch() {
            int pipeline = QueryPipelineType("Query Pipeline Type");
            bool[] workflow = QueryWorkflowMethod("Batch Import Material");
            if(!(workflow[0] == false && workflow[1] == false)) {
                Toolkit.BatchCreateMaterial(Toolkit.GetFolderPath("Import GameTextures Material - BATCH"), GetPipeline(pipeline), workflow[0], workflow[1], true);
            }
        }
    }
    // ------------------------------------------------------------------------------------
    // ASSET POST PROCESSOR ---------------------------------------------------------------
    // ------------------------------------------------------------------------------------
    class GTTextureProcessor : AssetPostprocessor {
        void OnPreprocessTexture() {
            // Only postprocess textures if they are in or child of GameTextures/Textures
            // or a sub folder of it.
            string localAssetPath = assetPath.ToLower();
            if (localAssetPath.IndexOf("/GameTextures/Textures") == -1) {
                PreProcessTexture();
            }
        }
        void PreProcessTexture() {
            string[] texture_conversion = { "specular", "gloss", "metallic", "roughness", "height", "emissive", "opacity", "ambientocclusion" };
            string ap = assetPath.ToLower();
            TextureImporter importer = (TextureImporter)assetImporter;
            if (ap.Contains("normal")) {
                importer.textureType = TextureImporterType.NormalMap;
            }
            else {
                foreach (string texture_type in texture_conversion) {
                    if (ap.Contains(texture_type)) {
                        importer.sRGBTexture = false;
                        break;
                    }
                }
            }
        }
    }
}
