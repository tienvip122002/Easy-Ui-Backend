# Sử dụng .NET SDK 8.0 để xây dựng ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Đặt thư mục làm việc
WORKDIR /src

# Copy giải pháp và các file .csproj vào container
COPY ["EasyUiBackend.sln", "./"]
COPY ["EasyUiBackend.Api/EasyUiBackend.Api.csproj", "EasyUiBackend.Api/"]
COPY ["EasyUiBackend.Application/EasyUiBackend.Application.csproj", "EasyUiBackend.Application/"]
COPY ["EasyUiBackend.Domain/EasyUiBackend.Domain.csproj", "EasyUiBackend.Domain/"]
COPY ["EasyUiBackend.Infrastructure/EasyUiBackend.Infrastructure.csproj", "EasyUiBackend.Infrastructure/"]

# Khôi phục các dependency của ứng dụng
RUN dotnet restore

# Copy toàn bộ mã nguồn của dự án vào container
COPY . .

# Publish ứng dụng
RUN dotnet publish "EasyUiBackend.Api/EasyUiBackend.Api.csproj" -c Release -o /app/publish

# Image cuối cùng sử dụng .NET 8.0 Runtime để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Đặt thư mục làm việc trong image cuối cùng
WORKDIR /app

# Copy output đã được publish từ image build
COPY --from=build /app/publish .

# Mở cổng cho ứng dụng
EXPOSE 80

# Đặt entry point để chạy ứng dụng
ENTRYPOINT ["dotnet", "EasyUiBackend.Api.dll"]
