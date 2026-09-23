using Microsoft.EntityFrameworkCore;
using MiniB2B.Data;
using MiniB2B.Models;

namespace MiniB2B.Services
{
    public class GridConfigService
    {
        private readonly AppDbContext _context;

        public GridConfigService(AppDbContext context)
        {
            _context = context;
        }

     
        public async Task<List<GridColumnConfig>> GetVisibleColumnsAsync(bool isAdmin)
        {
            var query = _context.GridColumnConfigs
                .AsNoTracking()
                .Where(c => c.IsVisible);

            if (!isAdmin)
                query = query.Where(c => !c.AdminOnly);

            return await query
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
        }
    }
}