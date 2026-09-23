using MiniB2B.Models;

namespace MiniB2B.Helpers
{
    public static class GridColumnRenderHelper
    {
        public static string ResponsiveClass(GridColumnConfig c)
        {
            if (!c.VisibleOnMobile) return "d-none d-md-table-cell";
            if (!c.VisibleOnTablet) return "d-none d-lg-table-cell";
            if (!c.VisibleOnDesktop) return "d-lg-none";
            return "";
        }

        public static string AlignClass(GridColumnConfig c) => c.Alignment switch
        {
            "Center" => "text-center",
            "Right" => "text-end",
            _ => "text-start"
        };

        public static string WidthStyle(GridColumnConfig c) =>
            !string.IsNullOrEmpty(c.ColumnWidth) ? $"width:{c.ColumnWidth};" : "";

        public static string CellClass(GridColumnConfig c) =>
            $"{ResponsiveClass(c)} {AlignClass(c)}".Trim();
    }
}