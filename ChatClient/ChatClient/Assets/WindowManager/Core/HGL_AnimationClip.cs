using UnityEngine;
using System;
using System.Collections.Generic;

namespace HGL {
    [Serializable]
    public class HGL_AnimationClip : ScriptableObject {
        public float TimeDuration;
        public List<HGL_UCurveAnimationProperty> AnimationProperty;

        public List<HGL_ClipProperty> GetAvailableProperty(List<HGL_ClipProperty> input) {
            List<HGL_ClipProperty> result = new List<HGL_ClipProperty>();
            int countProperies = input.Count;
            for(int i = 0; i < countProperies; i++) {
                if (!PropertyExist(input[i])) {
                    result.Add(input[i]);
                }
            }
            return result;
        }

        bool PropertyExist(HGL_ClipProperty prop) {
            for (int i = 0; i < AnimationProperty.Count; i++) {
                if (AnimationProperty[i].Property.Equals(prop)) {
                    return true;
                }
            }
            return false;
        }
    }
}
