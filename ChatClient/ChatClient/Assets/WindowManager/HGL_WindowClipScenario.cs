using UnityEngine;
using System.Collections;

namespace HGL {
    public class HGL_WindowClipScenario : MonoBehaviour {
        public HGL_WindowInspector[] Windows;
        IEnumerator scenarioPlaying;

        public void PlayScenario() {
            if (scenarioPlaying != null) {
                return;
            }
            scenarioPlaying = ScenarioPlaying();
            StartCoroutine(scenarioPlaying);
        }

        void Update() {
            if (Input.GetKeyDown(KeyCode.S)) {
                PlayScenario();
            }
        }

        IEnumerator ScenarioPlaying() {
            for (int i = 0; i < Windows.Length; i++) {
                yield return new WaitForSeconds(Windows[i].DelayBefore); //Deley before starting animation

                if (!Windows[i].Window.gameObject.activeSelf) {
                    Windows[i].Window.gameObject.SetActive(true);
                }
                Windows[i].Window.SetAnimation(Windows[i].AnimationState);
                if (Windows[i].AnimationState) {
                    Windows[i].Window.SetState(HGL_WindowStates.Opening);
                } else {
                    Windows[i].Window.SetState(HGL_WindowStates.Closing);
                }
                yield return Windows[i].Window.PlayAnimation(Windows[i].Force, Windows[i].Modal, Windows[i].InvertingClip, null, null);

                yield return new WaitForSeconds(Windows[i].DelayAfter); //Deley after ending animation
            }
            scenarioPlaying = null;
        }
    }
}