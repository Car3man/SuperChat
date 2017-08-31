using UnityEngine;

namespace KBCakeMath {
	public class Math {
		public static Vector3 MultiplyVectors (Vector3 a, Vector3 b) {
			return new Vector3 (a.x * b.x, a.y * b.y, a.z * b.z);
		}

		public static Vector3 GetBezier2Point (Vector3 start, Vector3 control, Vector3 end, float t) {
			return (((1 - t) * (1 - t)) * start) + (2 * t * (1 - t) * control) + ((t * t) * end);
		}
	}
}