xóa migration
dotnet ef database drop --project EasyUiBackend.Infrastructure --startup-project EasyUiBackend.Api --context AppDbContext

tạo migration
dotnet ef database update --project EasyUiBackend.Infrastructure --startup-project EasyUiBackend.Api --context AppDbContext

cập nhập 
dotnet ef database update --project EasyUiBackend.Infrastructure --startup-project EasyUiBackend.Api --context AppDbContext
