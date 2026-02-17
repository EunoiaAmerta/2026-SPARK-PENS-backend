# 🖊️ SparkPens Backend System

**Integrated Room Booking & Customer Management API**

## 📖 Description

SparkPens API adalah sistem backend yang dirancang untuk mengelola data peminjaman ruangan dan manajemen peminjaman (Booking) di lingkungan kampus PENS. Proyek ini bertujuan untuk menggantikan pencatatan manual menjadi sistem digital yang terpusat, efisien, dan minim kesalahan data.

## ✨ Features

- **Room Management (CRUD)**: Pengelolaan data ruangan lengkap (Create, Read, Update, Delete).
- **Soft Delete**: Mekanisme penghapusan data aman tanpa menghilangkan record dari database.
- **Booking System**:
  - Peminjaman ruangan oleh mahasiswa/tamu
  - Approve/Reject booking oleh admin
  - Pengecekan overlapping booking otomatis
  - Filter booking berdasarkan ruangan dan tanggal
- **User Authentication**:
  - Login dengan email/password
  - Google OAuth integration
  - JWT Token-based authentication
  - Role-based access (Admin/User)
- **Password Management**:
  - Lupa password dengan reset token
  - Set password untuk pengguna Google
- **API Documentation**: Dokumentasi interaktif menggunakan Swagger UI.

## 🛠️ Tech Stack

- **Framework**: ASP.NET Core 10 (C#)
- **Database**: PostgreSQL (Neon.tech)
- **ORM**: Entity Framework Core
- **Authentication**: JWT Bearer + Google OAuth
- **Password Hashing**: BCrypt
- **Documentation**: Swagger / OpenAPI
- **Deployment**: Docker + Render.com

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

## 🌐 API Endpoints

### Authentication (`/api/auth`)

| Method | Endpoint                    | Description                       |
| ------ | --------------------------- | --------------------------------- |
| POST   | `/api/auth/login`           | Login dengan email/password       |
| POST   | `/api/auth/google`          | Login dengan Google OAuth         |
| POST   | `/api/auth/set-password`    | Set password untuk user Google    |
| POST   | `/api/auth/forgot-password` | Request reset password            |
| POST   | `/api/auth/reset-password`  | Reset password dengan token       |
| GET    | `/api/auth/me`              | Get current user info (Protected) |

### Rooms (`/api/rooms`)

| Method | Endpoint          | Description                      |
| ------ | ----------------- | -------------------------------- |
| GET    | `/api/rooms`      | Get all rooms                    |
| POST   | `/api/rooms`      | Create new room (Admin)          |
| PUT    | `/api/rooms/{id}` | Update room (Admin)              |
| DELETE | `/api/rooms/{id}` | Delete room (Soft delete, Admin) |

### Bookings (`/api/bookings`)

| Method | Endpoint                                  | Description                            |
| ------ | ----------------------------------------- | -------------------------------------- |
| GET    | `/api/bookings`                           | Get all bookings                       |
| GET    | `/api/bookings/room/{roomId}?date={date}` | Get bookings by room and date          |
| POST   | `/api/bookings`                           | Create new booking                     |
| PATCH  | `/api/bookings/{id}/status`               | Update booking status (Approve/Reject) |
| DELETE | `/api/bookings/{id}`                      | Delete booking                         |

## 🔐 Default Credentials

- **Username**: admin
- **Password**: admin

Untuk pertama kali login, sistem akan membuatkan user Admin secara otomatis.

## 🌐 Environment Variables

| Variable                               | Description                           | Required |
| -------------------------------------- | ------------------------------------- | -------- |
| `ASPNETCORE_ENVIRONMENT`               | Set to "Development" for Swagger      | No       |
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string          | Yes      |
| `Jwt__Key`                             | JWT secret key                        | Yes      |
| `Jwt__Issuer`                          | JWT issuer                            | Yes      |
| `Jwt__Audience`                        | JWT audience                          | Yes      |
| `Google__ClientId`                     | Google OAuth Client ID                | Yes      |
| `FrontendUrl`                          | Frontend URL for password reset links | Yes      |

Aplikasi ini menggunakan pengaturan standar .NET. Pastikan **_ASPNETCORE_ENVIRONMENT_** diatur ke **Development** untuk melihat dokumentasi Swagger secara lengkap.

## 📂 Project Structure

```
SparkPens.Api/
├── Controllers/
│   ├── AuthController.cs      # Authentication endpoints
│   ├── BookingsController.cs # Booking management
│   └── RoomsController.cs    # Room management
├── Models/
│   ├── Booking.cs
│   ├── Room.cs
│   └── User.cs
├── DTOs/
│   ├── AuthResponseDto.cs
│   ├── BookingDto.cs
│   ├── CreateBookingDto.cs
│   ├── CustomerDto.cs
│   ├── GoogleLoginDto.cs
│   ├── LoginDto.cs
│   ├── UpdateStatusDto.cs
│   └── SetPasswordDto.cs
├── Data/
│   └── AppDbContext.cs       # EF Core DbContext
├── Migrations/                # Database migrations
├── Program.cs                 # Application entry point
└── appsettings.json          # Configuration
```

## 🐳 Docker

Build dan run dengan Docker:

```bash
docker build -t sparkpens-api .
docker run -p 8080:8080 sparkpens-api
```

## 📝 License

Distributed under the MIT License.

## 👤 Author

- Kratos Spartan - Backend Developer - [GitHub Profile](https://github.com/EunoiaAmerta)
