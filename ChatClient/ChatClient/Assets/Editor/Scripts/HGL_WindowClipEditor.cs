using UnityEngine;
using UnityEditor;

namespace HGL {
    [CustomEditor(typeof(HGL_AnimationClip))]
    public class HGL_WindowClipEditor : UnityEditor.Editor {
        private HGL_AnimationClip m_target;

        public void OnEnable() {
            m_target = (HGL_AnimationClip)target;
        }

        public override void OnInspectorGUI() {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("Open in window clip maker")) {
                HGL_WindowClipMakerEditor.Open(m_target);
            }
            if (m_target.AnimationProperty == null) {
                GUILayout.Label("Properties doesn't exist");
            } else { 
                GUILayout.Label(string.Format("Count properties: {0}", m_target.AnimationProperty.Count));
            }
            GUILayout.Label(string.Format("Time duration : {0}", m_target.TimeDuration));
            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }

}
