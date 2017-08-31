using System;
using System.Collections;
using UnityEngine;

namespace HGL {
	[Serializable]
	public abstract class HGL_BaseWindow : MonoBehaviour {
		private float time;

		protected float Time {
			get {
				return time;
			}
			set {
				time = value;
				if (value >= 0) {
					UpdateWindow ();
				}
			}
		}

		protected float MaxTime;
		public HGL_WindowStates State;
		public string NameWindow;
		[SerializeField] protected HGL_AnimationClip OpenClip;
		[SerializeField] protected HGL_AnimationClip CloseClip;

		protected HGL_AnimationClip CurrentClip { get; set; }

		public abstract void Open (bool forceOpen, bool modal, bool invertingClip, Action onStartOpen, Action onFinishOpen);

		public abstract void Close (bool forceClose, bool invertingClip, Action onStartClose, Action onFinishClose);

		public abstract void UpdateWindow ();

		public abstract IEnumerator PlayAnimation (bool force, bool showShadow, bool invertingClip, Action onStart, Action onFinish);

		public abstract void SetAnimation (bool open);

		public abstract void SetState (HGL_WindowStates state);

		public abstract void ShowShadow ();

		public abstract void HideShadow ();
	}
}
