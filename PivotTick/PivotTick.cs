// ----------------------------------------------------------------------------
// Copyright (c) 2025 https://github.com/qtx-project
// Licensed under the **[GPL-3.0 License](./license.txt)**.
// See LICENSE file in the project root for full license information.
// ----------------------------------------------------------------------------

namespace PivotTick;

using System;
using System.Drawing;
using System.Linq;
using TradingPlatform.BusinessLayer;
using TradingPlatform.BusinessLayer.Chart;

/// <summary>
/// Custom indicator for calculating and displaying pivot points on a tick chart.
/// The PivotTick indicator supports various calculation methods for pivot points 
/// (Classic, Camarilla, Fibonacci, etc.) and allows displaying pivot lines for 
/// different time periods (hour, day, week, month) on chart tick. (Temporary solution)
/// </summary>
public class PivotTick : Indicator
{
    #region QtPivotPointConstants

    /// <summary>
    /// Constant for the input parameter specifying that only the current period should be used
    /// </summary>
    private const string CurrentPeriodInputParameter = "Only current period";

    /// <summary>
    /// Constant for the input parameter specifying the base period to be used in the calculation of pivot points.
    /// </summary>
    private const string BasePeriodInputParameter = "Base period";

    /// <summary>
    /// Constant for the input parameter specifying the range of periods to be considered in the pivot point calculation.
    /// </summary>
    private const string RangeInputParameter = "Range";

    /// <summary>
    /// Constant for the input parameter specifying the calculation method used to determine pivot points.
    /// </summary>
    private const string CalculationMethodInputParameter = "Calculation method";

    /// <summary>
    /// Constant for the index of the Pivot Point (PP) line in the indicator's value array.
    /// </summary>
    private const int PpLineIndex = 0;

    /// <summary>
    /// Constant for the index of the first resistance (R1) line in the indicator's value array.
    /// </summary>
    private const int R1LineIndex = 1;

    /// <summary>
    /// Constant for the index of the second resistance (R2) line in the indicator's value array.
    /// </summary>
    private const int R2LineIndex = 2;

    /// <summary>
    /// Constant for the index of the third resistance (R3) line in the indicator's value array.
    /// </summary>
    private const int R3LineIndex = 3;

    /// <summary>
    /// Constant for the index of the fourth resistance (R4) line in the indicator's value array.
    /// </summary>
    private const int R4LineIndex = 4;

    /// <summary>
    /// Constant for the index of the fifth resistance (R5) line in the indicator's value array.
    /// </summary>
    private const int R5LineIndex = 5;

    /// <summary>
    /// Constant for the index of the sixth resistance (R6) line in the indicator's value array.
    /// </summary>
    private const int R6LineIndex = 6;

    /// <summary>
    /// Constant for the index of the first support (S1) line in the indicator's value array.
    /// </summary>
    private const int S1LineIndex = 7;

    /// <summary>
    /// Constant for the index of the second support (S2) line in the indicator's value array.
    /// </summary>
    private const int S2LineIndex = 8;

    /// <summary>
    /// Constant for the index of the third support (S3) line in the indicator's value array.
    /// </summary>
    private const int S3LineIndex = 9;

    /// <summary>
    /// Constant for the index of the fourth support (S4) line in the indicator's value array.
    /// </summary>
    private const int S4LineIndex = 10;

    /// <summary>
    /// Constant for the index of the fifth support (S5) line in the indicator's value array.
    /// </summary>
    private const int S5LineIndex = 11;

    /// <summary>
    /// Constant for the index of the sixth support (S6) line in the indicator's value array.
    /// </summary>
    private const int S6LineIndex = 12;

    #endregion

    #region params

    /// <summary>
    /// Color for the support line in the chart. 
    /// Used to customize the appearance of the support lines representing price levels of support.
    /// </summary>
    [InputParameter("Color for Support Line")]
    public Color ColorSupport;

    /// <summary>
    /// Color for the resistance line in the chart.
    /// Used to customize the appearance of the resistance lines representing price levels of resistance.
    /// </summary>
    [InputParameter("Color for Resistance Line")]
    public Color ColorResistance;

    /// <summary>
    /// Color for the pivot point line in the chart.
    /// Used to customize the appearance of the pivot point line that represents the central price level.
    /// </summary>
    [InputParameter("Color for Pivot Point Line")]
    public Color ColorPivotPoint;

    /// <summary>
    /// Boolean to enable or disable pivot point calculation for the hourly period.
    /// If true, the indicator will calculate and display the pivot points for each hour.
    /// </summary>
    [InputParameter("enable pivot point hour")]
    public bool IsEnabledHour;

    /// <summary>
    /// Boolean to enable or disable pivot point calculation for the daily period.
    /// If true, the indicator will calculate and display the pivot points for each day.
    /// </summary>
    [InputParameter("enable pivot point Day")]
    public bool IsEnabledDay;

    /// <summary>
    /// Boolean to enable or disable pivot point calculation for the weekly period.
    /// If true, the indicator will calculate and display the pivot points for each week.
    /// </summary>
    [InputParameter("enable pivot point Week")]
    public bool IsEnabledWeek;

    /// <summary>
    /// Boolean to enable or disable pivot point calculation for the monthly period.
    /// If true, the indicator will calculate and display the pivot points for each month.
    /// </summary>
    [InputParameter("enable pivot point Month")]
    public bool IsEnabledMonth;

    /// <summary>
    /// The calculation method used to compute the pivot points.
    /// Available options:
    /// - Classic: Standard pivot point calculation method.
    /// - Camarilla: A method based on the theory that price will usually fall within certain ranges.
    /// - Fibonacci: Uses Fibonacci retracement levels for support and resistance calculations.
    /// - Woodie: A method similar to Classic but with a slightly different formula.
    /// - DeMark: A method popularized by Tom DeMark, with a different approach to support/resistance.
    /// </summary>
    [InputParameter(CalculationMethodInputParameter, 1, variants:
        [
            "Classic", PivotPointType.Classic,
            "Camarilla", PivotPointType.Camarilla,
            "Fibonacci", PivotPointType.Fibonacci,
            "Woodie", PivotPointType.Woodie,
            "DeMark", PivotPointType.DeMark
        ]
    )]
    public PivotPointType CalculationMethod;

    /// <summary>
    /// The range value used in the pivot point calculations, typically the number of periods for calculation.
    /// Values range from 1 to 60 (with 1 being the default), determining how far back the historical data should be considered.
    /// </summary>
    [InputParameter(RangeInputParameter, 2, 1, 60, 1, 0)]
    public int PeriodValue = 1;

    #endregion

    /// <summary>
    /// Private fields used in the PivotTick indicator.
    /// </summary>

    #region PrivateFields

    private HistoricalData _history; // Holds the historical data for the symbol, used for calculations of pivot points.

    private Pen
        _penColorP, _penColorR, _penColorS; // Pens used to draw the pivot, resistance, and support lines on the chart.

    private Brush
        _brushP, _brushR, _brushS; // Brushes used to fill the pivot, resistance, and support labels on the chart.

    private Indicator
        _ppHour,
        _ppDaily,
        _ppWeek,
        _ppMonth; // Indicators for pivot points at different time periods (hour, day, week, month).

    private readonly PivotLevelsDto
        _ppHourDto,
        _ppDailyDto,
        _ppWeekDto,
        _ppMonthDto; // Data transfer objects (DTOs) to hold pivot point values for different periods.

    private Font _font; // Font used to display labels for the pivot points on the chart.

    #endregion

    /// <summary>
    /// Constructor for the PivotTick class.
    /// Initializes the default values for the PivotTick indicator:
    /// - Sets the default name and description.
    /// - Initializes the colors for the support, resistance, and pivot point lines.
    /// - Sets the default calculation method to Classic.
    /// - Enables daily pivot point calculations while disabling others (hour, week, month).
    /// - Initializes DTO objects for the pivot points at different time periods (hour, day, week, month).
    /// - Initializes the font used for displaying labels on the chart.
    /// </summary>
    public PivotTick()
    {
        Name = "PivotTick";
        Description = "Display Pivot On Tick Chart";
        SeparateWindow = false;
        CalculationMethod = PivotPointType.Classic;
        ColorSupport = Color.LawnGreen;
        ColorResistance = Color.Red;
        ColorPivotPoint = Color.Gray;
        IsEnabledHour = false;
        IsEnabledDay = true;
        IsEnabledWeek = false;
        IsEnabledMonth = false;
        _ppHourDto = PivotLevelsDto.Empty();
        _ppDailyDto = PivotLevelsDto.Empty();
        _ppWeekDto = PivotLevelsDto.Empty();
        _ppMonthDto = PivotLevelsDto.Empty();
        _font = new Font("Arial", 7);
        UpdateColor();
    }

    /// <summary>
    /// Initializes the necessary data and indicators for the Pivot Points.
    /// It retrieves historical data for the last 3 days and adds the appropriate Pivot Point indicators 
    /// based on the enabled flags for hour, day, week, and month periods.
    /// </summary>
    protected override void OnInit()
    {
        base.OnInit();

        // Get History For Pivot Point
        _history = this.Symbol.GetHistory(new HistoryRequestParameters()
        {
            Symbol = Symbol,
            FromTime = DateTime.Now.AddDays(-3),
            ToTime = DateTime.Now,
            Aggregation = new HistoryAggregationTime(
                Period.HOUR1,
                Symbol.HistoryType
            ),
        });

        // Get Quantower Pivot Point
        var quantowerPivotPoint = Core
            .Instance
            .Indicators
            .All
            .FirstOrDefault(info => info.Name == "Pivot Point");

        if (IsEnabledHour)
        {
            _ppHour = CreatePivotPoint(quantowerPivotPoint, BasePeriod.Hour);
            _history.AddIndicator(_ppHour);
        }

        if (IsEnabledDay)
        {
            _ppDaily = CreatePivotPoint(quantowerPivotPoint, BasePeriod.Day);
            _history.AddIndicator(_ppDaily);
        }

        if (IsEnabledWeek)
        {
            _ppWeek = CreatePivotPoint(quantowerPivotPoint, BasePeriod.Week);
            _history.AddIndicator(_ppWeek);
        }

        if (IsEnabledMonth)
        {
            _ppMonth = CreatePivotPoint(quantowerPivotPoint, BasePeriod.Month);
            _history.AddIndicator(_ppMonth);
        }
    }

    #region OnUpdateValue

    /// <summary>
    /// Updates the Pivot Point levels for different time periods (hour, day, week, month) based on the enabled flags.
    /// It calls the method `OnUpdatePivotDTO` to update the Pivot Points for each time period (hour, day, week, month).
    /// </summary>
    /// <param name="args">The event arguments for the update.</param>
    protected override void OnUpdate(UpdateArgs args)
    {
        base.OnUpdate(args);
        if (IsEnabledHour)
        {
            OnUpdatePivotDTO(_ppHour, _ppHourDto);
        }

        if (IsEnabledDay)
        {
            OnUpdatePivotDTO(_ppDaily, _ppDailyDto);
        }

        if (IsEnabledWeek)
        {
            OnUpdatePivotDTO(_ppWeek, _ppWeekDto);
        }

        if (IsEnabledMonth)
        {
            OnUpdatePivotDTO(_ppMonth, _ppMonthDto);
        }
    }

    /// <summary>
    /// Updates the Pivot Point values in the provided `PivotLevelsDto` for a specific time period.
    /// The method retrieves the Pivot Points (PP), resistance levels (R1, R2, R3), and support levels (S1, S2, S3, etc.)
    /// based on the selected calculation method (DeMark, Camarilla, etc.).
    /// </summary>
    /// <param name="pp">The Pivot Point indicator to fetch the values from.</param>
    /// <param name="dto">The PivotLevelsDto object that stores the updated Pivot Point values.</param>
    private void OnUpdatePivotDTO(Indicator pp, PivotLevelsDto dto)
    {
        if (double.IsNaN(pp.GetValue()))
        {
            return; // wait official Pivot Point 
        }

        dto.PP = pp.GetValue(0, PpLineIndex);
        dto.R1 = pp.GetValue(0, R1LineIndex);
        dto.S1 = pp.GetValue(0, S1LineIndex);

        if (CalculationMethod != PivotPointType.DeMark)
        {
            dto.R2 = pp.GetValue(0, R2LineIndex);
            dto.R3 = pp.GetValue(0, R3LineIndex);
            dto.S2 = pp.GetValue(0, S2LineIndex);
            dto.S3 = pp.GetValue(0, S3LineIndex);
        }

        if (CalculationMethod == PivotPointType.Camarilla)
        {
            dto.R4 = pp.GetValue(0, R4LineIndex);
            dto.R5 = pp.GetValue(0, R5LineIndex);
            dto.R6 = pp.GetValue(0, R6LineIndex);
            dto.S4 = pp.GetValue(0, S4LineIndex);
            dto.S5 = pp.GetValue(0, S5LineIndex);
            dto.S6 = pp.GetValue(0, S6LineIndex);
        }
    }

    #endregion

    #region OnDraw

    /// <summary>
    /// Paints the Pivot Points and related levels on the chart for different time periods (hour, day, week, month).
    /// This method checks the enabled flags and calls the method to draw each Pivot Point level (PP, R1, S1, etc.) for the specified time period.
    /// </summary>
    /// <param name="args">The event arguments for painting the chart.</param>
    public override void OnPaintChart(PaintChartEventArgs args)
    {
        base.OnPaintChart(args);
        if (IsEnabledHour)
        {
            OnPaintPivotDTO(args, _ppHourDto, "H");
        }

        if (IsEnabledDay)
        {
            OnPaintPivotDTO(args, _ppDailyDto, "D");
        }

        if (IsEnabledWeek)
        {
            OnPaintPivotDTO(args, _ppWeekDto, "W");
        }

        if (IsEnabledMonth)
        {
            OnPaintPivotDTO(args, _ppMonthDto, "M");
        }
    }

    /// <summary>
    /// Draws the Pivot Point levels (PP, R1, S1, etc.) on the chart for a specific time period (hour, day, week, month).
    /// This method calls the method to draw lines for each Pivot Point level, depending on the calculation method (DeMark, Camarilla, etc.).
    /// </summary>
    /// <param name="args">The event arguments for painting the chart.</param>
    /// <param name="dto">The PivotLevelsDto containing the values for the Pivot Points and resistance/support levels.</param>
    /// <param name="label">The label to indicate the time period (e.g., "H" for hour, "D" for day).</param>
    private void OnPaintPivotDTO(PaintChartEventArgs args, PivotLevelsDto dto, String label)
    {
        var coordinateConverter = CurrentChart.Windows[args.WindowIndex].CoordinatesConverter;
        OnDrawPivotLines(args, coordinateConverter, dto.PP, _penColorP, _brushP, $"{label}:PP");
        OnDrawPivotLines(args, coordinateConverter, dto.R1, _penColorR, _brushR, $"{label}:R1");
        OnDrawPivotLines(args, coordinateConverter, dto.S1, _penColorS, _brushS, $"{label}:S1");

        if (CalculationMethod != PivotPointType.DeMark)
        {
            OnDrawPivotLines(args, coordinateConverter, dto.R2, _penColorR, _brushR, $"{label}:R2");
            OnDrawPivotLines(args, coordinateConverter, dto.R3, _penColorR, _brushR, $"{label}:R3");
            OnDrawPivotLines(args, coordinateConverter, dto.S2, _penColorS, _brushS, $"{label}:S2");
            OnDrawPivotLines(args, coordinateConverter, dto.S3, _penColorS, _brushS, $"{label}:S3");
        }

        if (CalculationMethod == PivotPointType.Camarilla)
        {
            OnDrawPivotLines(args, coordinateConverter, dto.R4, _penColorR, _brushR, $"{label}:R4");
            OnDrawPivotLines(args, coordinateConverter, dto.R5, _penColorR, _brushR, $"{label}:R5");
            OnDrawPivotLines(args, coordinateConverter, dto.R6, _penColorR, _brushR, $"{label}:R6");
            OnDrawPivotLines(args, coordinateConverter, dto.S4, _penColorS, _brushS, $"{label}:S4");
            OnDrawPivotLines(args, coordinateConverter, dto.S5, _penColorS, _brushS, $"{label}:S5");
            OnDrawPivotLines(args, coordinateConverter, dto.S6, _penColorS, _brushS, $"{label}:S6");
        }
    }

    /// <summary>
    /// Draws a line and a label for the Pivot Point (or other levels) on the chart.
    /// The method calculates the Y-coordinate of the value, draws the label, and then draws the line at the calculated position.
    /// </summary>
    /// <param name="args">The event arguments for painting the chart.</param>
    /// <param name="coord">The coordinates converter for charting values to pixel positions.</param>
    /// <param name="value">The value of the Pivot Point or other level to be drawn.</param>
    /// <param name="pen">The pen used to draw the line.</param>
    /// <param name="brush">The brush used to draw the label.</param>
    /// <param name="label">The label text to be displayed next to the line.</param>
    private void OnDrawPivotLines(
        PaintChartEventArgs args,
        IChartWindowCoordinatesConverter coord,
        double value,
        Pen pen,
        Brush brush,
        String label
    )
    {
        var y = (int)coord.GetChartY(value);
        args.Graphics.DrawString(label, _font, brush, 2, y + 3);
        args.Graphics.DrawLine(pen, 0, y, args.Rectangle.Width, y);
    }

    #endregion

    #region OnUpdateSettings

    /// <summary>
    /// Called automatically when the indicator's settings are modified
    /// This override ensures that any visual-related changes, such as colors, are immediately applied.
    /// </summary>
    protected override void OnSettingsUpdated()
    {
        base.OnSettingsUpdated();
        UpdateColor();
    }

    /// <summary>
    /// Updates the colors for the Pivot Point, Resistance, and Support lines and brushes.
    /// Initializes the pens and brushes with the current colors for each element.
    /// </summary>
    private void UpdateColor()
    {
        _penColorP = new Pen(ColorPivotPoint, 1);
        _penColorR = new Pen(ColorResistance, 1);
        _penColorS = new Pen(ColorSupport, 1);
        _brushP = new SolidBrush(ColorPivotPoint);
        _brushR = new SolidBrush(ColorResistance);
        _brushS = new SolidBrush(ColorSupport);
    }

    #endregion

    /// <summary>
    /// Creates a new Pivot Point indicator using the provided indicator information and base period.
    /// Sets the indicator's settings based on the current period, base period, range, and calculation method.
    /// </summary>
    /// <param name="indicatorInfo">The information for the indicator to be created.</param>
    /// <param name="basePeriod">The base period for the pivot point calculation.</param>
    /// <returns>A new Pivot Point indicator with the configured settings.</returns>
    private Indicator CreatePivotPoint(IndicatorInfo indicatorInfo, BasePeriod basePeriod)
    {
        var pp = Core.Instance.Indicators.CreateIndicator(indicatorInfo);
        pp.Settings =
        [
            new SettingItemBoolean(CurrentPeriodInputParameter, true),
            new SettingItemObject(BasePeriodInputParameter, basePeriod),
            new SettingItemObject(RangeInputParameter, PeriodValue),
            new SettingItemObject(CalculationMethodInputParameter, CalculationMethod)
        ];

        return pp;
    }

    /// <summary>
    /// Releases resources used by the object and its fields. Calls base class Dispose and disposes of 
    /// the associated resources like fonts, pens, brushes, and pivot point data objects.
    /// </summary>
    public override void Dispose()
    {
        base.Dispose();
        _font?.Dispose();
        _penColorP?.Dispose();
        _penColorR?.Dispose();
        _penColorS?.Dispose();
        _brushP?.Dispose();
        _brushR?.Dispose();
        _brushS?.Dispose();
        _history?.Dispose();
        _ppHour?.Dispose();
        _ppDaily?.Dispose();
        _ppWeek?.Dispose();
        _ppMonth?.Dispose();
    }
}