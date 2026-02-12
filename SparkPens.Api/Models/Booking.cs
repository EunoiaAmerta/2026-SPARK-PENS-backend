namespace SparkPens.Api.Models;

public class Booking
{
    public Guid Id { get; set; }
    public string NIM { get; set; } = string.Empty;
    public string NamaMahasiswa { get; set; } = string.Empty;
    public string NamaRuangan { get; set; } = string.Empty;
    public DateTime WaktuMulai { get; set; }
    public DateTime WaktuSelesai { get; set; }
    public string Keperluan { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
}