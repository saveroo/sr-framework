using System;

namespace SRUL;


public class SRStepper
{
    // Step Hexadecimal with decimal step
    public static string StepHexadecimal(string hex, int step)
    {
        int dec = Convert.ToInt32(hex, 16);
        dec += step;
        return dec.ToString("X");
    }
    
    // Step Decimal with hexadecimal step
    public static string StepDecimal(string dec, int step)
    {
        int deci = Convert.ToInt32(dec);
        deci += step;
        return deci.ToString("X");
    }
    
    // Hexadecimal addition
    public static string AddHexadecimal(string hex1, string hex2)
    {
        int dec1 = Convert.ToInt32(hex1, 16);
        int dec2 = Convert.ToInt32(hex2, 16);
        int dec = dec1 + dec2;
        return dec.ToString("X");
    }
    
    // Hexadecimal subtraction
    public static string SubtractHexadecimal(string hex1, string hex2)
    {
        int dec1 = Convert.ToInt32(hex1, 16);
        int dec2 = Convert.ToInt32(hex2, 16);
        int dec = dec1 - dec2;
        return dec.ToString("X");
    }
    
    // Hexadecimal stepping
    public static string StepHexadecimal(string hex, int step, int stepSize)
    {
        int dec = Convert.ToInt32(hex, 16);
        dec += step * stepSize;
        return dec.ToString("X");
    }
    
    // Top - 720
    // Top left - 23A0
    // Bottom left - 390
    // Bottom - 1AB8
    // Bottom Right - 1E48
    // Top Right - 1398
    // HexStepper

    public static void OneDayBuildAllHex(string staticAddress, int complexCount)
    {
        for (int i = 0; i < complexCount; i++)
        {
            SubtractHexadecimal(staticAddress, "720");
            SubtractHexadecimal(staticAddress, "23A0");
            SubtractHexadecimal(staticAddress, "390");
            
            SubtractHexadecimal(staticAddress, "1AB8");
            SubtractHexadecimal(staticAddress, "1E48");
            SubtractHexadecimal(staticAddress, "1398");
        }
        SRLoaderForm._srLoader.rw.SRWrite("ADayBuild", "0.99", "float");
    }

}