# 🖊️ SparkPens Backend System

**Integrated Room Booking & Customer Management API**

## 📖 Description

SparkPens API adalah sistem backend yang dirancang untuk mengelola data peminjaman ruangan dan manajemen informasi pelanggan (Customer) di lingkungan kampus PENS. Proyek ini bertujuan untuk menggantikan pencatatan manual menjadi sistem digital yang terpusat, efisien, dan minim kesalahan data.

## ✨ Features

- **Master Customer CRUD**: Pengelolaan data pelanggan lengkap (Create, Read, Update, Delete).
- **Soft Delete**: Mekanisme penghapusan data aman tanpa menghilangkan record dari database.
- **Advanced Search & Filter**: Pencarian pelanggan berdasarkan Nama atau Email.
- **Pagination**: Pengaturan tampilan data dalam jumlah besar agar aplikasi tetap ringan.
- **Room Booking System**: Pencatatan jadwal peminjaman ruangan kampus.
- **API Documentation**: Dokumentasi interaktif menggunakan Swagger UI.

## 🛠️ Tech Stack

- **Framework**: ASP.NET Core 10 (C#)
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **Documentation**: Swagger / OpenAPI

## ⚙️ Installation

1. Clone repositori ini ke mesin lokal Anda.
2. Pastikan PostgreSQL sudah terinstal dan berjalan.
3. Konfigurasi koneksi database di file `SparkPens.Api/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost; Database=SparkPensDb; Username=postgres; Password=yourpassword"
   }
   ```
4. Jalankan perintah restore untuk mengunduh dependencies: `dotnet restore`
5. Terapkan migrasi database: `dotnet ef database update`

## 🚀 Usage

Jalankan aplikasi dengan perintah: `dotnet run`
Akses dokumentasi API dan uji coba endpoint melalui browser di: http://localhost:5151/swagger

## 🌐 Environment Variables

Aplikasi ini menggunakan pengaturan standar .NET. Pastikan **_ASPNETCORE_ENVIRONMENT_** diatur ke **Development** untuk melihat dokumentasi Swagger secara lengkap.4

## 📝 License

Distributed under the MIT License.

## 👤 Author

- Kratos Spartan - Backend Developer - [GitHub Profile](https://github.com/EunoiaAmerta)
