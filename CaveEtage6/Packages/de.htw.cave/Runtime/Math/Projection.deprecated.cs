using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;
using UnityEngine;

namespace Htw.Cave.Math
{
    public static partial class Projection
    {
		[Obsolete("Holographic is deprecated, please use Holographic (out Matrix4x4 worldToCamera) instead.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Matrix4x4 Holographic(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 vr, Vector3 vu, Vector3 vn, Vector3 target, float near, float far)
		{
			Vector3 va = v1 - target;
			Vector3 vb = v2 - target;
			Vector3 vc = v3 - target;

			float d = Vector3.Dot(va, vn);
			float nd = near / d;

			float l = Vector3.Dot(vr, va) * nd;
			float r = Vector3.Dot(vr, vb) * nd;
			float b = Vector3.Dot(vu, va) * nd;
			float t = Vector3.Dot(vu, vc) * nd;

			Matrix4x4 mat = Matrix4x4.zero;

			mat[0] = (2f * near) / (r - l);
			mat[5] = (2f * near) / (t - b);
			mat[8] = (r + l) / (r - l);
			mat[9] = (t + b) / (t - b);
			mat[10] = -(far + near) / (far - near);
			mat[11] = -1f;
			mat[14] = (-2f * far * near) / (far - near);

			return mat;
		}

		[Obsolete("Holographic is deprecated, please use Holographic (out Matrix4x4 worldToCamera) instead.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Matrix4x4 Holographic(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 target, float near, float far)
		{
			Vector3 vr = (v2 - v1).normalized;
			Vector3 vu = (v3 - v1).normalized;
			Vector3 vn = Vector3.Cross(vr, vu).normalized;

			Vector3 va = v1 - target;
			Vector3 vb = v2 - target;
			Vector3 vc = v3 - target;

			float d = Vector3.Dot(va, vn);
			float nd = near / d;

			float l = Vector3.Dot(vr, va) * nd;
			float r = Vector3.Dot(vr, vb) * nd;
			float b = Vector3.Dot(vu, va) * nd;
			float t = Vector3.Dot(vu, vc) * nd;

			Matrix4x4 mat = Matrix4x4.zero;

			mat[0] = (2f * near) / (r - l);
			mat[5] = (2f * near) / (t - b);
			mat[8] = (r + l) / (r - l);
			mat[9] = (t + b) / (t - b);
			mat[10] = -(far + near) / (far - near);
			mat[11] = -1f;
			mat[14] = (-2f * far * near) / (far - near);

			return mat;
		}

		[Obsolete("NearPlane is deprecated, please use Holographic instead.")]
		public static Vector2[] NearPlane(Transform target, Vector3 rectMin, Vector3 rectMax, float near)
		{
			//  rectMin
			//    +-----+     describes the actual surface that
			//    |     |     the image will be projected on.
			//    +-----+     rectMax - rectMin = center of rect
			//        rectMax

			Vector3 nearCenter = target.position + target.forward * near;
			Plane plane = new Plane(-target.forward, nearCenter);

			//  nearOrigin
			//       x-------------+                           |
			//       |             |                           |
			//       |      x      |                x       <--x nearCenter
			//       |  nearCenter |              target       |
			//       +------------ x                           |
			//                   nearEnd                     plane
			float distance = 0f;
			Vector3 direction = (rectMin - target.position).normalized;
			Ray ray = new Ray(target.position, direction);
			bool hit = plane.Raycast(ray, out distance);

			if(!hit)
				return null;

			Vector3 nearOrigin = -(target.InverseTransformPoint(nearCenter) - target.InverseTransformPoint(target.position + direction * distance));
			float left = nearOrigin.x;
			float top = nearOrigin.y;

			direction = (rectMax - target.position).normalized;
			ray = new Ray(target.position, direction);
			hit = plane.Raycast(ray, out distance);

			if(!hit)
				return null;

			Vector3 nearEnd = -(target.InverseTransformPoint(nearCenter) - target.InverseTransformPoint((target.position + direction * distance)));
			float right = nearEnd.x;
			float bottom = nearEnd.y;

			return new Vector2[]{
				new Vector2(left, top),
				new Vector2(right, bottom)
			};
		}

		[Obsolete("HolographicNearPlane is deprecated, please use Holographic instead.")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Matrix4x4 HolographicNearPlane(Vector2[] nearPlane, float near, float far)
		{
		    //       plane matrix P 3x2      holographic matrix H 4x4
		    //       column = vertical            row = vertical
		    //            |  0   1              |  0   1   2   3
		    //          --+--------           --+----------------
		    //   row    0 | -x   x    column  0 | H00 H10  0  H20
		    //    =     1 | -y   y      =     1 | H01 H11  0  H21
		    // horizont 2 |  n   f   horizont 2 |  0   0   d   0
		    //                                3 | H02 H12  0   1

		    float left = nearPlane[0].x;
		    float right = nearPlane[1].x;
		    float top = nearPlane[0].y;
			float bottom = nearPlane[1].y;

		    Matrix4x4 mat = Matrix4x4.zero;

		    mat[0] = (2f * near) / (right - left);
		    mat[5] = (2f * near) / (top - bottom);
		    mat[8] = (right + left) / (right - left);
		    mat[9] = (top + bottom) / (top - bottom);
		    mat[10] = -(far + near) / (far - near);
		    mat[11] = -1f;
		    mat[14] = -(2f * far * near) / (far - near);

		    return mat;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Matrix4x4 Bimber(Vector3[] homography)
		{
			//          bimber matrix B 4x4
			//             row = vertical
			//            |  0   1   2   3
			//          --+----------------
			//  column  0 | H0x H1x  0  H2x
			//    =     1 | H0y H1y  0  H2y
			// horizont 2 |  0   0   d   0
			//          3 | H0z H1z  0   1
			//
			// with H = homography vectors
			// approximation of depth buffer with d = 1-|H20|-|H21|

			Matrix4x4 mat = Matrix4x4.zero;

			mat[0] = homography[0].x;
			mat[1] = homography[1].x;
			mat[3] = homography[2].x;
			mat[4] = homography[0].y;
			mat[5] = homography[1].y;
			mat[7] = homography[2].y;
			mat[10] = 1f - (mat[3] < 0f ? -mat[3] : mat[3]) - (mat[7] < 0f ? -mat[7] : mat[7]);
			mat[12] = homography[0].z;
			mat[13] = homography[1].z;
			mat[15] = 1f;

			return mat;
		}
    }
}
