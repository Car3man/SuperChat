using UnityEngine;
using System.Collections;
using System;

namespace HGL {
    [Serializable]
    internal class HGL_WindowClipMakerEditorController {
        internal string savePath;
        internal string SavePath {
            get { return savePath; }
            set { savePath = value; }
        }
        internal HGL_AnimationClip clip;
        internal HGL_AnimationClip Clip
        {
            get {
                return clip;
            }
            set {
                clip = value;
                ReloadProperties();
            }
        }
        internal GameObject windowObject;
        internal GameObject WindowObject
        {
            get
            {
                return windowObject;
            }
            set
            {
                windowObject = value;
            }
        }
        internal float timeDuration;
        internal float TimeDuration
        {
            get
            {
                return timeDuration;
            }
            set
            {
                timeDuration = value;
                Clip.TimeDuration = value;
            }
        }

        void ReloadProperties()  {
            TimeDuration = Clip.TimeDuration;
        }
    }
}
