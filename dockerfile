# Bước 1: Sử dụng Docker image cho .NET SDK 8.0 để build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Thiết lập thư mục làm việc
WORKDIR /app

# Sao chép solution file và các project file vào container
COPY EasyUiBackend.sln ./ 
COPY EasyUiBackend.Api/*.csproj ./EasyUiBackend.Api/
COPY EasyUiBackend.Application/*.csproj ./EasyUiBackend.Application/
COPY EasyUiBackend.Domain/*.csproj ./EasyUiBackend.Domain/
COPY EasyUiBackend.Infrastructure/*.csproj ./EasyUiBackend.Infrastructure/

# Restore dependencies
RUN dotnet restore EasyUiBackend.sln

# Sao chép toàn bộ mã nguồn vào container
COPY . ./

# Build ứng dụng
RUN dotnet publish EasyUiBackend.Api -c Release -o /app/publish

# Bước 2: Sử dụng image runtime để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Expose port 44319
EXPOSE 44319

# Sao chép ứng dụng đã build từ bước 1 vào container
COPY --from=build /app/publish . 

# Lệnh để chạy ứng dụng .NET
ENTRYPOINT ["dotnet", "EasyUiBackend.Api.dll"]
# Tạo chứng chỉ HTTPS phát triển (bỏ comment nếu cần)
# RUN dotnet dev-certs https --trust
