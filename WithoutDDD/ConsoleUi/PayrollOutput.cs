using Models;

namespace ConsoleUi;

public static class PayrollOutput
{
    public static void Print(Payroll payroll)
    {
        string status = payroll.IsApproved ? "утверждена" : "черновик";
        Console.WriteLine($"Ведомость #{payroll.Id}, водитель #{payroll.DriverId}: {status}");

        foreach (var entry in payroll.Entries)
            Console.WriteLine($"  Рейс #{entry.TripId}: {entry.Amount:N2} руб.");

        Console.WriteLine($"ИТОГО: {payroll.Entries.Sum(entry => entry.Amount):N2} руб.");
    }
}
