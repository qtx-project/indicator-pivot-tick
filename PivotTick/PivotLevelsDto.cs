// ----------------------------------------------------------------------------
// Copyright (c) 2025 https://github.com/qtx-project
// Licensed under the **[GPL-3.0 License](./license.txt)**.
// See LICENSE file in the project root for full license information.
// ----------------------------------------------------------------------------

namespace PivotTick;

/// <summary>
/// This class is used to store and represent the various pivot points and their associated support and resistance levels.
/// It includes values for the primary pivot point (PP) and six levels each for resistance (R1 to R6) and support (S1 to S6).
/// These levels are calculated based on the selected pivot point method.
/// </summary>
public class PivotLevelsDto
{
    /// <summary>
    /// Gets or sets the central pivot point (PP), calculated based on the selected pivot method.
    /// </summary>
    public double PP { get; set; }

    /// <summary>
    /// Gets or sets the first resistance level (R1), calculated based on the selected pivot method.
    /// </summary>
    public double R1 { get; set; }

    /// <summary>
    /// Gets or sets the second resistance level (R2), calculated based on the selected pivot method.
    /// </summary>
    public double R2 { get; set; }

    /// <summary>
    /// Gets or sets the third resistance level (R3), calculated based on the selected pivot method.
    /// </summary>
    public double R3 { get; set; }

    /// <summary>
    /// Gets or sets the fourth resistance level (R4), used for methods like Camarilla that calculate more levels.
    /// </summary>
    public double R4 { get; set; }

    /// <summary>
    /// Gets or sets the fifth resistance level (R5), used for methods like Camarilla that calculate more levels.
    /// </summary>
    public double R5 { get; set; }

    /// <summary>
    /// Gets or sets the sixth resistance level (R6), used for methods like Camarilla that calculate more levels.
    /// </summary>
    public double R6 { get; set; }

    /// <summary>
    /// Gets or sets the first support level (S1), calculated based on the selected pivot method.
    /// </summary>
    public double S1 { get; set; }

    /// <summary>
    /// Gets or sets the second support level (S2), calculated based on the selected pivot method.
    /// </summary>
    public double S2 { get; set; }

    /// <summary>
    /// Gets or sets the third support level (S3), calculated based on the selected pivot method.
    /// </summary>
    public double S3 { get; set; }

    /// <summary>
    /// Gets or sets the fourth support level (S4), used for methods like Camarilla that calculate more levels.
    /// </summary>
    public double S4 { get; set; }

    /// <summary>
    /// Gets or sets the fifth support level (S5), used for methods like Camarilla that calculate more levels.
    /// </summary>
    public double S5 { get; set; }

    /// <summary>
    /// Gets or sets the sixth support level (S6), used for methods like Camarilla that calculate more levels.
    /// </summary>
    public double S6 { get; set; }

    /// <summary>
    /// Returns a string representation of the pivot levels, including the pivot point (PP),
    /// resistance levels (R1-R6), and support levels (S1-S6).
    /// </summary>
    /// <returns>A string that represents the pivot levels in the format:
    /// "PP: {PP}, R1-R6: [{R1}, {R2}, {R3}, {R4}, {R5}, {R6}], S1-S6: [{S1}, {S2}, {S3}, {S4}, {S5}, {S6}]"
    /// </returns>
    public override string ToString()
    {
        return $"PP: {PP}, R1-R6: [{R1}, {R2}, {R3}, {R4}, {R5}, {R6}], S1-S6: [{S1}, {S2}, {S3}, {S4}, {S5}, {S6}]";
    }

    /// <summary>
    /// Static method to create an instance of PivotLevelsDto with all levels set to zero.
    /// This is useful for initializing the object when no pivot points have been calculated yet.
    /// </summary>
    /// <returns>A new instance of PivotLevelsDto with all levels set to zero.</returns>
    public static PivotLevelsDto Empty()
    {
        return new PivotLevelsDto
        {
            PP = 0,
            R1 = 0, R2 = 0, R3 = 0, R4 = 0, R5 = 0, R6 = 0,
            S1 = 0, S2 = 0, S3 = 0, S4 = 0, S5 = 0, S6 = 0
        };
    }
}