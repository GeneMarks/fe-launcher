namespace FELauncher.Host.Tray
{
    public class TrayMenuRenderer : ToolStripRenderer
    {
        private static readonly Color _textDefaultColor = Color.Black;
        private static readonly Color _textHoverColor = Color.Black;
        private static readonly Color _itemDefaultColor = Color.FromArgb(242, 242, 242);
        private static readonly Color _itemHoverColor = Color.FromArgb(145, 201, 247);
        private static readonly Color _borderColor = Color.FromArgb(204, 204, 204);

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            var g = e.Graphics;

            var brushColor = e.Item.Selected
                ? _itemHoverColor
                : _itemDefaultColor;

            using var brush = new SolidBrush(brushColor);

            var rect = new Rectangle(Point.Empty, e.Item.Size);

            g.FillRectangle(brush, rect);

            base.OnRenderMenuItemBackground(e);
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            ControlPaint.DrawBorder(
                e.Graphics,
                e.AffectedBounds,
                _borderColor,
                ButtonBorderStyle.Solid);

            base.OnRenderToolStripBorder(e);
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = e.Item.Selected
                ? _textHoverColor
                : _textDefaultColor;

            base.OnRenderItemText(e);
        }
    }
}
