using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace HGL {
    public class HGL_WindowClipMakerEditor : EditorWindow {
        static HGL_WindowClipMakerEditorController controller = new HGL_WindowClipMakerEditorController();

        internal static HGL_WindowClipMakerEditorController Controller {
            get { return controller; }
            private set { controller = value; }
        }

        static GenericMenu addActionsMenuProperties;

        internal static void Open(HGL_AnimationClip clip) {
            if (clip != null) {
                addActionsMenuProperties = null;
                GetWindow<HGL_WindowClipMakerEditor>("Clip Maker");
                Controller.Clip = clip;
                Controller.SavePath = AssetDatabase.GetAssetPath(clip); 
                Debug.Log(string.Format("Window clip was opened; TimeDuration: {0}", clip.TimeDuration));
            }
        }

        [MenuItem("HGL/ClipMaker")]
        static void Init() {
            addActionsMenuProperties = null;
            HGL_WindowClipMakerEditor wnd = GetWindow<HGL_WindowClipMakerEditor>("Clip Maker");
            wnd.Show();
        }

        void OnGUI() {
            DrawHead();
            if (Controller.Clip == null) return;
            EditorGUILayout.BeginHorizontal();
            DrawNavigation();
            DrawWorkspace();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawHead() {
            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal("Box"); {
                if (GUILayout.Button("New", GUILayout.Height(30))) {
                    NewClip();
                }

                if (Controller.Clip != null) {
                    if (GUILayout.Button("Save", GUILayout.Height(30))) {
                        Save();
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawNavigation() {
            EditorGUILayout.BeginVertical("Box", GUILayout.Width(350));
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.BeginVertical();
                    {
                        DrawTools();
                        DrawProperties();
                    }
                    GUILayout.Space(1);
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawTools() {
            EditorGUILayout.BeginVertical("Box"); {
                Controller.WindowObject = EditorGUILayout.ObjectField("WindowObject", Controller.WindowObject, typeof(GameObject), true) as GameObject;
                Controller.TimeDuration = EditorGUILayout.FloatField("Time duration", Controller.TimeDuration);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical("Box");
            {
                EditorGUILayout.BeginHorizontal("Box");
                {
                    if (GUILayout.Button("Record", GUILayout.Height(25))) {
                       
                    }
                    if (GUILayout.Button("Play", GUILayout.Height(25))) {
                        
                    }
                    if (GUILayout.Button("Pause", GUILayout.Height(25))) {
                       
                    }
                    if (GUILayout.Button("ToStart", GUILayout.Height(25))) {
                        
                    }
                    if (GUILayout.Button("ToEnd", GUILayout.Height(25))) {
                        
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
        }

        private void DrawProperties() {
            if (Controller.Clip == null) return;
            if (Controller.Clip.AnimationProperty == null) return;
            EditorGUILayout.BeginVertical("Box"); { 
                for(int i = 0; i < Controller.Clip.AnimationProperty.Count; i++) {
                    if (GUILayout.Button(Controller.Clip.AnimationProperty[i].Property.ToString(), GUILayout.Height(30))) {
                        Controller.Clip.AnimationProperty.RemoveAt(i);
                    }
                }
                GUILayout.Space(5);
                if (GUILayout.Button("Add property", GUILayout.Height(25))) {
                    if (addActionsMenuProperties == null) {
                        addActionsMenuProperties = new GenericMenu();

                        List<HGL_ClipProperty> props = new List<HGL_ClipProperty>((HGL_ClipProperty[])Enum.GetValues((typeof(HGL_ClipProperty))));
                        List<HGL_ClipProperty> availableProps = Controller.Clip.GetAvailableProperty(props);

                        foreach (HGL_ClipProperty p in availableProps) {
                            addActionsMenuProperties.AddItem(new GUIContent(p.ToString()), false, CreateContextItem, p);
                        }
                    }
                    addActionsMenuProperties.ShowAsContext();
                    Event.current.Use();
                }
                GUILayout.Space(2);
            }
            EditorGUILayout.EndVertical();
        }

        private void CreateContextItem(object obj) {
            HGL_UCurveAnimationProperty animProp = new HGL_UCurveAnimationProperty((HGL_ClipProperty)obj);
            Controller.Clip.AnimationProperty.Add(animProp);
            addActionsMenuProperties = null;
            Repaint();
        }

        private void DrawWorkspace() {
            EditorGUILayout.BeginVertical("Box", GUILayout.Width(350)); {
                int countProperties = Controller.Clip.AnimationProperty.Count;
                for (int i = 0; i < countProperties; i++) {
                    HGL_UCurveAnimationProperty curveAnimProperty = (Controller.Clip.AnimationProperty[i] as HGL_UCurveAnimationProperty);
                    curveAnimProperty.Curve = EditorGUILayout.CurveField(Controller.Clip.AnimationProperty[i].Property.ToString(), curveAnimProperty.Curve);
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void NewClip() {
            var newClip = (HGL_AnimationClip)ScriptableObject.CreateInstance(typeof(HGL_AnimationClip));
            AssetDatabase.CreateAsset(newClip, "Assets/NewWindowClip.asset");
            newClip.TimeDuration = 0;
            newClip.AnimationProperty = new List<HGL_UCurveAnimationProperty>();
            AssetDatabase.SaveAssets();
            Controller.Clip = newClip;
        }
 
        private void Save() {
            AssetDatabase.SaveAssets();
            if (Controller.Clip != null) {
                EditorUtility.SetDirty(Controller.Clip);
            }
        }

        void OnDestroy() {
            Save(); 
        }
    }
}
