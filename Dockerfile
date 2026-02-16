# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy file csproj dan restore (agar cache layer Docker efisien)
COPY ["SparkPens.Api/SparkPens.Api.csproj", "SparkPens.Api/"]
RUN dotnet restore "SparkPens.Api/SparkPens.Api.csproj"

# Copy seluruh source code
COPY . .

# Build project
WORKDIR "/src/SparkPens.Api"
RUN dotnet build "SparkPens.Api.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "SparkPens.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Final Image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copy hasil publish dari stage sebelumnya
COPY --from=publish /app/publish .

# Command untuk menjalankan aplikasi
ENTRYPOINT ["dotnet", "SparkPens.Api.dll"]