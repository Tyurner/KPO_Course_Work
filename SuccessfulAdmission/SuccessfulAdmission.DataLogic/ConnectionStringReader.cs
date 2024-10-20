namespace SuccessfulAdmission.DataLogic;

public class ConnectionStringReader
{
    private readonly string _fileName = "connectionString.txt";
    public string GetConnectionString()
    {
        try
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _fileName);
            return File.Exists(path) ? File.ReadAllText(path).Trim() : string.Empty;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
            return string.Empty;
        }
    }
}