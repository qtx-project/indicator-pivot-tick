namespace PivotTick;

public class PivotLevelsDto
{
    public double PP { get; set; }

    public double R1 { get; set; }
    
    public double R2 { get; set; }
    
    public double R3 { get; set; }
    
    public double R4 { get; set; }
    
    public double R5 { get; set; }
    
    public double R6 { get; set; }

    public double S1 { get; set; }
    
    public double S2 { get; set; }
    
    public double S3 { get; set; }
    
    public double S4 { get; set; }
    
    public double S5 { get; set; }
    
    public double S6 { get; set; }

    public override string ToString()
    {
        return $"PP: {PP}, R1-R6: [{R1}, {R2}, {R3}, {R4}, {R5}, {R6}], S1-S6: [{S1}, {S2}, {S3}, {S4}, {S5}, {S6}]";
    }
     
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
