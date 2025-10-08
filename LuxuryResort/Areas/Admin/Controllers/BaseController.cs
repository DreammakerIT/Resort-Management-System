using Microsoft.AspNetCore.Authorization; // Cần using này
using Microsoft.AspNetCore.Mvc;

namespace LuxuryResort.Areas.Admin.Controllers
{
    [Area("Admin")]
[Authorize(Roles = "Quản lý, Nhân viên")]
    public class BaseController : Controller
    {
        // Controller này rỗng, nó chỉ dùng để kế thừa
    }
}