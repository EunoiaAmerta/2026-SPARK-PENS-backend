namespace SparkPens.Api.DTOs;

public class BookingDto
{
    public string NIM { get; set; } = string.Empty;
    public string NamaMahasiswa { get; set; } = string.Empty;
    public string NamaRuangan { get; set; } = string.Empty;
    public DateTime WaktuMulai { get; set; }
    public DateTime WaktuSelesai { get; set; }
    public string Keperluan { get; set; } = string.Empty;
}