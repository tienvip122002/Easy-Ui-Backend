xóa migration
 
 dotnet ef database drop --project EasyUiBackend.Infrastructure\Migrations --startup-project EasyUiBackend.Api

tạo migration
 dotnet ef migrations add InitialCreate --project EasyUiBackend.Infrastructure\Persistence\ --startup-project AppDbContext.cs

 cập nhập 
  dotnet ef database update --project EasyUiBackend.Infrastructure --startup-project EasyUiBackend.Api
