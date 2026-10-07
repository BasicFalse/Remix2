using UnityEngine;

public static class OklabConverter
{
    // Oklab constants and conversion matrices for D65 whitepoint
    private static readonly float[][] M1 = {
        new float[] { 0.8189330101f, 0.3618667424f, -0.1288597138f },
        new float[] { 0.0329845436f, 0.9293118955f, 0.0361456387f },
        new float[] { 0.0482054884f, 0.2643662691f, 0.6338517070f }
    };

    private static readonly float[][] M2 = {
        new float[] { 0.2104542553f, 1.9779984951f, 0.0259040371f },
        new float[] { 0.7936177046f, -2.4285922050f, 0.1403610967f },
        new float[] { -0.0040681296f, 0.4506596155f, -0.6860252913f }
    };

    /// <summary>
    /// Converts a color from Linear RGB to OKLab.
    /// Assumes Unity is in Linear color space.
    /// </summary>
    public static Color OklabFromLinearRgb(Color c)
    {
        // 1. Convert Linear RGB to LMS
        float l = M1[0][0] * c.r + M1[0][1] * c.g + M1[0][2] * c.b;
        float m = M1[1][0] * c.r + M1[1][1] * c.g + M1[1][2] * c.b;
        float s = M1[2][0] * c.r + M1[2][1] * c.g + M1[2][2] * c.b;

        // 2. Apply cubic root to LMS
        float l_p = Mathf.Pow(l, 1.0f / 3.0f);
        float m_p = Mathf.Pow(m, 1.0f / 3.0f);
        float s_p = Mathf.Pow(s, 1.0f / 3.0f);

        // 3. Convert L'M'S' to OKLab
        float oklabL = 0.5f * (l_p + m_p + s_p);
        float oklabA = 0.5f * (l_p + m_p - 2.0f * s_p);
        float oklabB = 0.5f * (l_p - m_p);

        return new Color(oklabL, oklabA, oklabB, c.a);
    }

    /// <summary>
    /// Converts a color from OKLab to Linear RGB.
    /// Returns a Color struct with linear RGB values.
    /// </summary>
    public static Color LinearRgbFromOklab(Color c)
    {
        // 1. Convert OKLab to L'M'S'
        float l_p = c.r + c.g + c.b;
        float m_p = c.r + c.g - c.b;
        float s_p = c.r - 2.0f * c.g;

        // 2. Apply cubic power to L'M'S'
        float l = l_p * l_p * l_p;
        float m = m_p * m_p * m_p;
        float s = s_p * s_p * s_p;

        // 3. Convert LMS to Linear RGB
        float r = M2[0][0] * l + M2[0][1] * m + M2[0][2] * s;
        float g = M2[1][0] * l + M2[1][1] * m + M2[1][2] * s;
        float b = M2[2][0] * l + M2[2][1] * m + M2[2][2] * s;

        return new Color(r, g, b, c.a);
    }
}
