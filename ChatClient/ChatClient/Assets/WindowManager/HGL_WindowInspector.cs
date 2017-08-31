using UnityEngine;
using System.Collections;
using System;

namespace HGL {
    [Serializable]
    public class HGL_WindowInspector {
        public HGL_BaseWindow Window;
        public bool Force;
        public bool Modal;
        public bool InvertingClip;
        public bool AnimationState;
        public float DelayBefore;
        public float DelayAfter;

        public HGL_WindowInspector(HGL_BaseWindow window, bool force, bool modal, bool invertingClip, bool animationState, float delayBefore, float delayAfter) {
            Force = force;
            Window = window;
            Modal = modal;
            InvertingClip = invertingClip;
            AnimationState = animationState;
            DelayBefore = delayBefore;
            DelayAfter = delayAfter;
        }
    }
}