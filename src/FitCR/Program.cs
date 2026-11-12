using System;
using System.IO;
using System.Windows.Forms;
using FitCR.Views;

namespace FitCR;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        try
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new FormLogin());
        }
        catch (Exception ex)
        {
            File.WriteAllText("error.txt", ex.ToString());
            Console.WriteLine(ex.ToString());
        }
    }
}
