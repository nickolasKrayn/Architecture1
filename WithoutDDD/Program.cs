using System.Globalization;
using System.Text;
using System.Text.Json;
using ConsoleUi;
using Input;

Console.OutputEncoding = Encoding.UTF8;
CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");

try
{
    var data = ScenarioFile.Load(ScenarioFile.DefaultPath);
    var payroll = ScenarioRunner.Calculate(data);

    PayrollOutput.Print(payroll);

    Services.PayrollService.Approve(payroll);
    Console.WriteLine("Ведомость утверждена.");
}
catch (Exception error) when (error is ArgumentException or InvalidOperationException or
    IOException or UnauthorizedAccessException or JsonException or OverflowException)
{
    Console.Error.WriteLine($"Ошибка: {error.Message}");
    Environment.ExitCode = 1;
}
