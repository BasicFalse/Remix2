using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GravNew
{
    /// <summary>
    /// Planet acts like a planet. Inverted acts like an inverted planet.
    ///  V3 & V3 Inverted are used for custom gravity like platforms and splines.
    /// </summary>
    public enum GravSetting {Planet, Inverted, V3, InvertedV3}

    /// <summary>
    /// Mono uses a spline and the target to determine the direction of gravity.
    /// Dual uses two splines to determine the direction of gravity.
    /// </summary>
    public enum GravSplineSetting {MonoSpline, DualSpline}
}
