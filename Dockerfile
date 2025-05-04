FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
COPY ./EasyUiBackend.Api/EasyUiBackend.Api.csproj Api/
COPY ./EasyUiBackend.Infrastructure/EasyUiBackend.Infrastructure.csproj Infrastructure/
COPY ./EasyUiBackend.Application/EasyUiBackend.Application.csproj Application/
COPY ./EasyUiBackend.Domain/EasyUiBackend.Domain.csproj Domain/

RUN dotnet restore "EasyUiBackend.sln"

COPY ./EasyUiBackend.Api/ Api/
COPY ./EasyUiBackend.Infrastructure/ Infrastructure/
COPY ./EasyUiBackend.Application/ Application/
COPY ./EasyUiBackend.Domain/ Domain/ 

WORKDIR /src/Api
RUN dotnet build "EasyUiBackend.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EasyUiBackend.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
RUN addgroup --system --gid 1001 dotnet
RUN adduser --system --uid 1001 csharp
COPY --from=publish /app/publish .
RUN chown -R csharp:dotnet /app && chmod -R 750 /app
USER csharp
EXPOSE 8080
# RUN apt-get update && apt-get install -y openssl \
#     && mkdir -p /root/.dotnet/corefx/cryptography/x509stores/my \
#     && openssl req -x509 -newkey rsa:4096 -keyout /root/.dotnet/corefx/cryptography/x509stores/my/localhost.key -out /root/.dotnet/corefx/cryptography/x509stores/my/localhost.crt -days 3650 -nodes -subj "/CN=localhost" \
#     && cat /root/.dotnet/corefx/cryptography/x509stores/my/localhost.crt >> /usr/local/share/ca-certificates/localhost.crt \
#     && update-ca-certificates
ENTRYPOINT ["dotnet", "EasyUiBackend.Api.dll"]
# RUN dotnet dev-certs https --trust
# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# WORKDIR /src

# # Copy project files
# COPY ./EasyUiBackend.Api/EasyUiBackend.Api.csproj Api/
# COPY ./EasyUiBackend.Infrastructure/EasyUiBackend.Infrastructure.csproj Infrastructure/
# COPY ./EasyUiBackend.Application/EasyUiBackend.Application.csproj Application/
# COPY ./EasyUiBackend.Domain/EasyUiBackend.Domain.csproj Domain/

# # Restore nuget
# RUN dotnet restore Api/EasyUiBackend.Api.csproj

# # Copy source code
# # COPY . .
# COPY ./EasyUiBackend.Api/ Api/
# COPY ./EasyUiBackend.Infrastructure/ Infrastructure/
# COPY ./EasyUiBackend.Application/ Application/
# COPY ./EasyUiBackend.Domain/ Domain/ 

# WORKDIR /src/Api

# # Không cần build/publish nếu dùng `dotnet run`
# EXPOSE 8080

# # Cấu hình cổng chạy HTTP
# # ENV ASPNETCORE_URLS=http://+:8080

# # Dùng dotnet run để chạy
# # CMD ["dotnet", "run", "--no-launch-profile", "--project", "EasyUiBackend.Api.csproj", "--configuration", "Release"]
# CMD ["dotnet", "run", "--project", "EasyUiBackend.Api.csproj"]
