using System;
using System.Drawing;
using System.Linq;
using TradingPlatform.BusinessLayer;
using TradingPlatform.BusinessLayer.Chart;

namespace PivotTick;

public class PivotTick : Indicator
{
    #region QtPivotPointConstants

    private const string CurrentPeriodInputParameter = "Only current period"; // true because tick chart.
    private const string BasePeriodInputParameter = "Base period";
    private const string RangeInputParameter = "Range";
    private const string CalculationMethodInputParameter = "Calculation method";
    private const int PpLineIndex = 0;
    private const int R1LineIndex = 1;
    private const int R2LineIndex = 2;
    private const int R3LineIndex = 3;
    private const int R4LineIndex = 4;
    private const int R5LineIndex = 5;
    private const int R6LineIndex = 6;
    private const int S1LineIndex = 7;
    private const int S2LineIndex = 8;
    private const int S3LineIndex = 9;
    private const int S4LineIndex = 10;
    private const int S5LineIndex = 11;
    private const int S6LineIndex = 12;

    #endregion

    #region params

    [InputParameter("Color for Support Line")]
    public Color ColorSupport;

    [InputParameter("Color for Resistance Line")]
    public Color ColorResistance;

    [InputParameter("Color for Pivot Point Line")]
    public Color ColorPivotPoint;

    [InputParameter("enable pivot point hour")]
    public bool IsEnabledHour;

    [InputParameter("enable pivot point Day")]
    public bool IsEnabledDay;

    [InputParameter("enable pivot point Week")]
    public bool IsEnabledWeek;

    [InputParameter("enable pivot point Month")]
    public bool IsEnabledMonth;

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

    [InputParameter(RangeInputParameter, 2, 1, 60, 1, 0)]
    public int PeriodValue = 1;

    #endregion

    private HistoricalData _history;
    private Pen _penColorP, _penColorR, _penColorS;
    private Brush _brushP, _brushR, _brushS;
    private Indicator _ppHour, _ppDaily, _ppWeek, _ppMonth;
    private readonly PivotLevelsDto _ppHourDto, _ppDailyDto, _ppWeekDto, _ppMonthDto;
    private Font _font;

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

    protected override void OnSettingsUpdated()
    {
        base.OnSettingsUpdated();
        UpdateColor();
    }

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