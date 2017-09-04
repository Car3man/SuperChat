using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace HGL {
	public class HGL_UGUIWindow : HGL_BaseWindow {
		[SerializeField] GameObject shadow;
		GameObject instanceShadow = null;
		IEnumerator playAnimation;

		void Awake () {
			Time = -1;
		}

		public override void Open (bool force, bool modal, bool invertingClip, Action onStartOpen, Action onFinishOpen) {
			gameObject.SetActive (true);
			State = HGL_WindowStates.Opening;
			if (playAnimation != null) {
				StopCoroutine (playAnimation);
			}
			SetAnimation (true);
			MaxTime = GetMaxTimeAnim ();
			playAnimation = PlayAnimation (force, modal, invertingClip, onStartOpen, onFinishOpen);
			StartCoroutine (Opening ());
		}

		public override void Close (bool force, bool invertingClip, Action onStartClose, Action onFinishClose) {
			State = HGL_WindowStates.Closing;
			if (playAnimation != null) {
				StopCoroutine (playAnimation);
			}
			SetAnimation (false);
			MaxTime = GetMaxTimeAnim ();
			playAnimation = PlayAnimation (force, false, invertingClip, onStartClose, onFinishClose);

			if (gameObject.activeSelf)
				StartCoroutine (Closing ());
		}

		public void Close (int data) {
			bool force = data.Equals (2) || data.Equals (3) ? true : false;
			bool invertingClip = data.Equals (1) || data.Equals (3) ? true : false;
			Close (force, invertingClip, null, null);
		}

		IEnumerator Opening () {
			yield return StartCoroutine (playAnimation);
			State = HGL_WindowStates.Open;
		}

		IEnumerator Closing () {
			yield return StartCoroutine (playAnimation);
			State = HGL_WindowStates.Close;
			gameObject.SetActive (false);
		}

		public override void ShowShadow () {
			if (instanceShadow == null && shadow != null) {
				instanceShadow = Instantiate (shadow) as GameObject;
				instanceShadow.transform.SetParent (transform.parent);
				instanceShadow.transform.localScale = new Vector3 (1, 1, 1);
				instanceShadow.GetComponent<RectTransform> ().anchorMax = new Vector2 (1, 1);
				instanceShadow.GetComponent<RectTransform> ().anchorMin = new Vector2 (0, 0);
				instanceShadow.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, 0);
				instanceShadow.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0, 0);
				instanceShadow.GetComponent<RectTransform> ().localRotation = Quaternion.Euler (Vector3.zero);
				instanceShadow.transform.SetSiblingIndex (transform.GetSiblingIndex ());
			} else if (shadow != null && instanceShadow != null) {
				instanceShadow.transform.SetSiblingIndex (transform.GetSiblingIndex () - 1);
				instanceShadow.SetActive (true);
			}
		}

		public override void HideShadow () {
			if (instanceShadow != null) {
				instanceShadow.SetActive (false);
			}
		}

		float GetMaxTimeAnim () {
			float maxTime = 0f;

			if (CurrentClip != null) {
				int countProps = CurrentClip.AnimationProperty.Count;
				for (int i = 0; i < countProps; i++) {
					if (CurrentClip.AnimationProperty [i].GetMaxTime () >= maxTime) {
						maxTime = CurrentClip.AnimationProperty [i].GetMaxTime ();
					}
				}
			}

			return maxTime;
		}

		public override IEnumerator PlayAnimation (bool force, bool showShadow, bool invertingClip, Action onStart, Action onFinish) {
			MaxTime = GetMaxTimeAnim ();

			if (showShadow) {
				ShowShadow ();
			} else {
				HideShadow ();
			}

			if (onStart != null) {
				onStart ();
			}

			if (!force) {
				if (Time < 0) {
					if (invertingClip) {
						Time = MaxTime;
					} else {
						Time = 0f;
					}
				}
				bool animFlag = true;
				while (animFlag) {
					if (invertingClip) {
						animFlag = Time > 0f ? true : false;
						Time -= UnityEngine.Time.unscaledDeltaTime;
					} else {
						animFlag = Time < MaxTime ? true : false;
						Time += UnityEngine.Time.unscaledDeltaTime;
					}
					yield return null;
				}
			} else {
				if (invertingClip) {
					Time = 0f;
				} else {
					Time = MaxTime;
				}
			}

			CurrentClip = null;
			Time = -1;


			if (onFinish != null) {
				onFinish ();
			}

			playAnimation = null;
		}

		public override void UpdateWindow () {
			if (CurrentClip == null)
				return;
			int countProps = CurrentClip.AnimationProperty.Count;
			List<HGL_UCurveAnimationProperty> props = CurrentClip.AnimationProperty;

			Vector3 pos;
			Vector3 localPos;
			Vector3 eAngles;
			Vector3 localEAngles;
			Vector3 localScale;

			Vector2 anchPosition;

			for (int i = 0; i < countProps; i++) {
				pos = transform.position;
				localPos = transform.localPosition;
				eAngles = transform.eulerAngles;
				localEAngles = transform.localEulerAngles;
				localScale = transform.localScale;

				anchPosition = transform.GetComponent<RectTransform> ().anchoredPosition;

				switch (props [i].Property) {
					case HGL_ClipProperty.Opasity:
						GetComponent<CanvasGroup> ().alpha = props [i].Evaluate (Time);
						break;
					case HGL_ClipProperty.PositionX:
						transform.position = new Vector3 (props [i].Evaluate (Time), pos.y, pos.z);
						break;
					case HGL_ClipProperty.PositionY:
						transform.position = new Vector3 (pos.x, props [i].Evaluate (Time), pos.z);
						break;
					case HGL_ClipProperty.PositionZ:
						transform.position = new Vector3 (pos.x, pos.y, props [i].Evaluate (Time));
						break;
					case HGL_ClipProperty.localPositionX:
						transform.localPosition = new Vector3 (props [i].Evaluate (Time), localPos.y, localPos.z);
						break;
					case HGL_ClipProperty.localPositionY:
						transform.localPosition = new Vector3 (localPos.x, props [i].Evaluate (Time), localPos.z);
						break;
					case HGL_ClipProperty.localPositionZ:
						transform.localPosition = new Vector3 (localPos.x, localPos.y, props [i].Evaluate (Time));
						break;
					case HGL_ClipProperty.EulerAnglesX:
						transform.eulerAngles = new Vector3 (props [i].Evaluate (Time), eAngles.y, eAngles.z);
						break;
					case HGL_ClipProperty.EulerAnglesY:
						transform.eulerAngles = new Vector3 (eAngles.x, props [i].Evaluate (Time), eAngles.z);
						break;
					case HGL_ClipProperty.EulerAnglesZ:
						transform.eulerAngles = new Vector3 (eAngles.x, eAngles.y, props [i].Evaluate (Time));
						break;
					case HGL_ClipProperty.localEulerAnglesX:
						transform.localEulerAngles = new Vector3 (props [i].Evaluate (Time), localEAngles.y, localEAngles.z);
						break;
					case HGL_ClipProperty.localEulerAnglesY:
						transform.localEulerAngles = new Vector3 (localEAngles.x, props [i].Evaluate (Time), localEAngles.z);
						break;
					case HGL_ClipProperty.localEulerAnglesZ:
						transform.localEulerAngles = new Vector3 (localEAngles.x, localEAngles.y, props [i].Evaluate (Time));
						break;
					case HGL_ClipProperty.Scale:
						transform.localScale = new Vector3 (props [i].Evaluate (Time), props [i].Evaluate (Time), localScale.z);
						break;
					case HGL_ClipProperty.AnchoredPositionX:
						transform.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (props [i].Evaluate (Time), anchPosition.y);
						break;
					case HGL_ClipProperty.AnchoredPositionY:
						transform.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (anchPosition.x, props [i].Evaluate (Time));
						break;
				}
			}
		}

		public override void SetAnimation (bool open) {
			if (open) {
				CurrentClip = OpenClip;
			} else {
				CurrentClip = CloseClip;
			}
		}

		public override void SetState (HGL_WindowStates state) {
			State = state;
		}
	}
}
