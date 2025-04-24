// ----------------------------------------------------------------------------
// Copyright (c) 2025 https://github.com/qtx-project
// Licensed under the **[GPL-3.0 License](./license.txt)**.
// See LICENSE file in the project root for full license information.
// ----------------------------------------------------------------------------

namespace PivotTick;

/// <summary>
/// Enum representing different types of pivot point calculation methods.
/// </summary>
public enum PivotPointType
{
    /// <summary>
    /// Classic pivot point calculation method.
    /// </summary>
    Classic,
    
    /// <summary>
    /// Camarilla pivot point calculation method, with more levels than the classic.
    /// </summary>
    Camarilla,
    
    /// <summary>
    /// Fibonacci pivot point calculation method, based on Fibonacci levels.
    /// </summary>
    Fibonacci,
    
    /// <summary>
    /// Woodie pivot point calculation method, which gives more weight to the close price.
    /// </summary>
    Woodie,
    
    /// <summary>
    /// DeMark pivot point calculation method, focused on market psychology and price action.
    /// </summary>
    DeMark
}
