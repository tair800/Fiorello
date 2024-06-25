using Fiorella.Data;
using Microsoft.AspNetCore.Mvc;

namespace Fiorella.ViewComponents
{
    public class SettingHeaderViewComponent : ViewComponent
    {
        readonly FiorelloDbContext _context;

        public SettingHeaderViewComponent(FiorelloDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            var setting = _context.Settings.ToDictionary(k => k.Key, v => v.Value);
            return View(await Task.FromResult(setting));
        }
    }
}
