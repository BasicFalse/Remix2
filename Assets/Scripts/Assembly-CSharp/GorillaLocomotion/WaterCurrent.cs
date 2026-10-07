using System;
using System.Collections.Generic;
using SplineTools;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace GorillaLocomotion.Swimming
{
	public class WaterCurrent : MonoBehaviour
	{
		[SerializeField]
		private List<SplineContainer> splines = new();

		[SerializeField]
		private float fullEffectDistance = 1f;

		[SerializeField]
		private float fadeDistance = 0.5f;

		[SerializeField]
		private float currentSpeed = 1f;

		[SerializeField]
		private float currentAccel = 10f;

		[SerializeField]
		private float velocityAnticipationAdjustment = 0.05f;

		[SerializeField]
		private float inwardCurrentFullEffectRadius = 1f;

		[SerializeField]
		private float inwardCurrentNoEffectRadius = 0.25f;

		[SerializeField]
		private float inwardCurrentSpeed = 1f;

		[SerializeField]
		private float inwardCurrentAccel = 10f;

		[SerializeField]
		private float dampingHalfLife = 0.25f;

		private Vector3 debugCurrentVelocity = Vector3.zero;

		private Vector3 debugSplinePoint = Vector3.zero;

		public float Speed => currentSpeed;

		public float Accel => currentAccel;

		public float InwardSpeed => inwardCurrentSpeed;

		public float InwardAccel => inwardCurrentAccel;

		public bool GetCurrentAtPoint(Vector3 worldPoint, Vector3 startingVelocity, float dt, out Vector3 currentVelocity, out Vector3 velocityChange)
		{
			float num = (fullEffectDistance + fadeDistance) * (fullEffectDistance + fadeDistance);
			bool result = false;
			velocityChange = Vector3.zero;
			currentVelocity = Vector3.zero;
			float num2 = 0.0001f;
			float magnitude = startingVelocity.magnitude;
			if (magnitude > num2)
			{
				Vector3 vector = startingVelocity / magnitude;
				float num3 = DamperDecayExact(magnitude, dampingHalfLife, dt);
				Vector3 vector2 = vector * num3;
				velocityChange += vector2 - startingVelocity;
			}
			for (int i = 0; i < splines.Count; i++)
			{
				SplineContainer catmullRomSpline = splines[i];
				Vector3 linePoint;
				float closestEvaluationOnSpline = SplineEvaluation.GetClosestFloatPoint(catmullRomSpline, worldPoint);
				catmullRomSpline.Evaluate(closestEvaluationOnSpline, out float3 x2, out _, out _);
				Vector3 vector3 = new(x2.x, x2.y, x2.z);
				Vector3 vector4 = vector3 - worldPoint;
				if (!(vector4.sqrMagnitude < num))
				{
					continue;
				}
				result = true;
				float magnitude2 = vector4.magnitude;
				float num4 = (magnitude2 > fullEffectDistance) ? (1f - Mathf.Clamp01((magnitude2 - fullEffectDistance) / fadeDistance)) : 1f;
				float t = Mathf.Clamp01(closestEvaluationOnSpline + velocityAnticipationAdjustment);
				Vector3 forwardTangent = SplineEvaluation.GetForwardTangent(catmullRomSpline.Spline, t);
				if (currentSpeed > num2 && Vector3.Dot(startingVelocity, forwardTangent) < num4 * currentSpeed)
				{
					velocityChange += forwardTangent * (currentAccel * dt);
				}
				else if (currentSpeed < num2 && Vector3.Dot(startingVelocity, forwardTangent) > num4 * currentSpeed)
				{
					velocityChange -= forwardTangent * (currentAccel * dt);
				}
				currentVelocity += forwardTangent * num4 * currentSpeed;
				float num5 = Mathf.InverseLerp(inwardCurrentNoEffectRadius, inwardCurrentFullEffectRadius, magnitude2);
				if (num5 > num2)
				{
					linePoint = Vector3.ProjectOnPlane(vector4, forwardTangent);
					Vector3 normalized = linePoint.normalized;
					if (inwardCurrentSpeed > num2 && Vector3.Dot(startingVelocity, normalized) < num5 * inwardCurrentSpeed)
					{
						velocityChange += normalized * (InwardAccel * dt);
					}
					else if (inwardCurrentSpeed < num2 && Vector3.Dot(startingVelocity, normalized) > num5 * inwardCurrentSpeed)
					{
						velocityChange -= normalized * (InwardAccel * dt);
					}
				}
				debugSplinePoint = vector3;
			}
			debugCurrentVelocity = velocityChange.normalized;
			return result;
		}

		private void OnDrawGizmosSelected()
		{
			int num = 16;
			for (int i = 0; i < splines.Count; i++)
			{
				Spline catmullRomSpline = splines[i].Spline;
				catmullRomSpline.Evaluate(0f, out float3 f2 , out _, out _);
                Vector3 vector = new(f2.x, f2.y, f2.z);
				for (int j = 1; j <= num; j++)
				{
					float t = (float)j / (float)num;
					catmullRomSpline.Evaluate(t, out float3 f3 , out _, out _);
                    Vector3 vector2 = new(f3.x, f3.y, f3.z);
					vector2 += splines[i].transform.position;
					_ = vector2 - vector;
					Quaternion rotation = Quaternion.LookRotation(SplineEvaluation.GetForwardTangent(catmullRomSpline, t), Vector3.up);
					Gizmos.color = new Color(0f, 0.5f, 0.75f);
					DrawGizmoCircle(vector2, rotation, fullEffectDistance);
					Gizmos.color = new Color(0f, 0.25f, 0.5f);
					DrawGizmoCircle(vector2, rotation, fullEffectDistance + fadeDistance);
				}
			}
		}

		private void DrawGizmoCircle(Vector3 center, Quaternion rotation, float radius)
		{
			Vector3 vector = Vector3.right * radius;
			int num = 16;
			for (int i = 1; i <= num; i++)
			{
				float f = (float)i / (float)num * 2f * (float)Math.PI;
				Vector3 vector2 = new Vector3(Mathf.Cos(f), Mathf.Sin(f), 0f) * radius;
				Gizmos.DrawLine(center + rotation * vector, center + rotation * vector2);
				vector = vector2;
			}
		}

		public static float DamperDecayExact(float x, float halflife, float dt, float eps = 1E-05f)
		{
			return x * FastNegExp(0.6931472f * dt / (halflife + eps));
		}

		public static float FastNegExp(float x)
		{
			return 1f / (1f + x + 0.48f * x * x + 0.235f * x * x * x);
		}
	}
}
