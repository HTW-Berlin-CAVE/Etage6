using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;
using UnityEngine;

namespace Htw.Cave.Math
{
	/// <summary>
	/// Provides common calculations for specialized projection matrices.
	/// </summary>
	public static partial class Projection
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void HolographicPrecompute(Vector3 pa, Vector3 pb, Vector3 pc, ref Vector3 vr, ref Vector3 vu, ref Vector3 vn)
		{
			vr = (pb - pa).normalized;
			vu = (pc - pa).normalized;
			vn = Vector3.Cross(vr, vu).normalized;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Matrix4x4 Holographic(Vector3 pa, Vector3 pb, Vector3 pc, Vector3 vr, Vector3 vu, Vector3 vn, Vector3 eye, float near, float far, out Matrix4x4 worldToCamera)
		{
			Vector3 va = pa - eye;
			Vector3 vb = pb - eye;
			Vector3 vc = pc - eye;

			// Credit to Johnathon Selstad (github.com/zalo).
			// Are we looking at the backface of the plane object?
			if (Vector3.Dot(-Vector3.Cross(va, vc), vb) < 0f) {
				// Mirror points along the z axis (most users
				// probably expect the x axis to stay fixed).
				vu = -vu;
				pa = pc;
				pb = pa + vr;
				pc = pa + vu;
				va = pa - eye;
				vb = pb - eye;
				vc = pc - eye;
			}

			float d = Vector3.Dot(va, vn);
			float nd = near / d;

			float l = Vector3.Dot(vr, va) * nd;
			float r = Vector3.Dot(vr, vb) * nd;
			float b = Vector3.Dot(vu, va) * nd;
			float t = Vector3.Dot(vu, vc) * nd;

			Matrix4x4 pm = Matrix4x4.zero;
			pm[0, 0] = 2f * near / (r - l);
			pm[0, 2] = (r + l) / (r - l);
			pm[1, 1] = 2f * near / (t - b);
			pm[1, 2] = (t + b) / (t - b);
			pm[2, 2] = (far + near) / (near - far);
			pm[2, 3] = 2f * far * near / (near - far);
			pm[3, 2] = -1f;

			Matrix4x4 rm = Matrix4x4.zero;
			rm[0, 0] = vr.x;
			rm[0, 1] = vr.y;
			rm[0, 2] = vr.z;
			rm[1, 0] = vu.x;
			rm[1, 1] = vu.y;
			rm[1, 2] = vu.z;
			rm[2, 0] = vn.x;
			rm[2, 1] = vn.y;
			rm[2, 2] = vn.z;
			rm[3, 3] = 1f;

			Matrix4x4 tm = Matrix4x4.zero;
			tm[0, 0] = 1f;
			tm[0, 3] = -eye.x;
			tm[1, 1] = 1f;
			tm[1, 3] = -eye.y;
			tm[2, 2] = 1f;
			tm[2, 3] = -eye.z;
			tm[3, 3] = 1f;

			worldToCamera = rm * tm;

			return pm;
		}
	}
}
