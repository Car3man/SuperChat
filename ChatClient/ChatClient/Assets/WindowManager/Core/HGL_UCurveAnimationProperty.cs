using UnityEngine;
using System.Collections;
using System;

namespace HGL {
    [Serializable]
    public class HGL_UCurveAnimationProperty {
        public AnimationCurve Curve;
        public HGL_ClipProperty Property;

        public HGL_UCurveAnimationProperty(HGL_ClipProperty property) {
            Curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
            Property = property;
        }

        public HGL_UCurveAnimationProperty(AnimationCurve curve, HGL_ClipProperty property) {
            Curve = curve;
            Property = property;
        }

        public float GetMaxTime() {
            return Curve.keys[Curve.length - 1].time;
        }

        public float Evaluate(float time) {
            return Curve.Evaluate(time);
        }

        public void Init() {
            Curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        }
    }
}
