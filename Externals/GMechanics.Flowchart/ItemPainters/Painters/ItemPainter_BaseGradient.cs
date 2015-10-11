using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using GMechanics.FlowchartControl.ItemControls;
using GMechanics.FlowchartControl.ItemElements;

namespace GMechanics.FlowchartControl.ItemPainters.Painters
{
    internal class ItemPainter_BaseGradient : ItemPainter_Base
    {

#region Private members

        protected virtual Color FrameColor { get { return Color.Black; } }
        protected virtual Color FrameColorSelected { get { return Color.Black; } }
        protected virtual Color TopColor { get { return Color.Black; } }
        protected virtual Color BottomColor { get { return Color.Black; } }
        protected virtual Color HeaderTextColor { get { return Color.Black; } }

        protected virtual Color WorkplaceBrushColor { get { return Color.Black; } }
        protected virtual Color WorkplacePenColor { get { return Color.Black; } }

        protected virtual Color ScrollBodyColor { get { return Color.Black; } }
        protected virtual Color ScrollFrontColor { get { return Color.Black; } }
        protected virtual Color ScrollMarksColor { get { return Color.Black; } }

        protected virtual Color ButtonColor { get { return Color.Black; } }
        protected virtual Color ButtonCaptionColor { get { return Color.Black; } }

        protected virtual Color GroupPlusMinusFrameColor { get { return Color.Black; } }
        protected virtual Color GroupPlusMinusFillColor { get { return Color.Black; } }
        protected virtual Color GroupPlusMinusMarkColor { get { return Color.Black; } }

        protected virtual Color LeftLinkPointColor { get { return Color.Black; } }
        protected virtual Color LeftLinkPointColorMouseOn { get { return Color.Black; } }
        protected virtual Color LeftLinkPointColorSelected { get { return Color.Black; } }

        protected virtual Color RightLinkPointColor { get { return Color.Black; } }
        protected virtual Color RightLinkPointColorMouseOn { get { return Color.Black; } }
        protected virtual Color RightLinkPointColorSelected { get { return Color.Black; } }

        private readonly Brush _solidBrush;
        private readonly Pen _outerFramePen;
        private readonly Pen _outerFrameSelectedPen;
        private readonly Pen _innerFramePen;
        private readonly Pen _headerLinePen;
        private readonly Brush _headerTextBrush;
        private readonly Brush _workplaceBrush;
        private readonly Pen _workplacePen;

        private readonly Pen _groupPlusMinusFramePen;
        private readonly Brush _groupPlusMinusFillBrush;
        private readonly Pen _groupPlusMinusMarkPen;

#endregion

#region Class functions

        public ItemPainter_BaseGradient()
        {
            _headerTextBrush = new SolidBrush(HeaderTextColor);
            _headerLinePen = new Pen(Color.FromArgb(150, FrameColor), 1);
            _innerFramePen = new Pen(Color.FromArgb(100, TopColor), 1);
            _outerFramePen = new Pen(FrameColor, 1);

            _outerFrameSelectedPen = new Pen(FrameColorSelected, 1.5f);
            _outerFrameSelectedPen.Alignment = PenAlignment.Inset;

            _solidBrush = new SolidBrush(TopColor);
            _workplaceBrush = new SolidBrush(WorkplaceBrushColor);
            _workplacePen = new Pen(WorkplacePenColor);

            _groupPlusMinusFillBrush = new SolidBrush(GroupPlusMinusFillColor);
            _groupPlusMinusFramePen = new Pen(GroupPlusMinusFrameColor);
            _groupPlusMinusMarkPen = new Pen(GroupPlusMinusMarkColor, 2);

            Padding = new Padding(WorkplaceMargin, 
                                  HeaderHeight + WorkplaceMargin - 2,
                                  ShadowDistance + WorkplaceMargin,
                                  ShadowDistance + WorkplaceMargin);
        }

        public override void Dispose()
        {
            _solidBrush.Dispose();
            _outerFramePen.Dispose();
            _innerFramePen.Dispose();
            _outerFrameSelectedPen.Dispose();
            _headerLinePen.Dispose();
            _headerTextBrush.Dispose();
            _workplaceBrush.Dispose();
            _workplacePen.Dispose();
            _groupPlusMinusFramePen.Dispose();
            _groupPlusMinusFillBrush.Dispose();
            _groupPlusMinusMarkPen.Dispose();
            base.Dispose();
        }

#endregion

#region Paint functions

        public override void Paint(FlowchartItem item, ItemScrollBar scrollBar,
            ItemLinkPoint[] linkPoints)
        {
            if (item.PreviousItemSize == item.Size)
            {
                DrawWorkplaceFromBuffer(item);
            }
            else
            {
                item.PreviousItemSize = item.Size;
                CalculateWorkplaceRectangle(item);
                item.Buffer.Clear(Color.Transparent);

                DrawBody(item);
                DrawHeader(item);
                DrawWorkplace(item);
                DrawSelectionBodyFrame(item);
            }

            bool scrollBarVisible = scrollBar.Visible;
            Rectangle clippedWorkplaceRectangle = Rectangle.Inflate(
                item.WorkplaceRectangle, -1, -1);

            item.Buffer.SetClip(clippedWorkplaceRectangle);
            DrawItemElements(item, scrollBar);

            //Repaint if scroll bar visible was changed
            if (scrollBarVisible != scrollBar.Visible)
            {
                item.Buffer.SetClip(item.ClientRectangle);
                Paint(item, scrollBar, linkPoints);
                return;
            }

            //Correct scroll bar position
            if (scrollBar.Position > scrollBar.MaxValue)
            {
                item.Buffer.SetClip(item.ClientRectangle);
                scrollBar.Position = scrollBar.MaxValue;
                return;
            }

            DrawControls(item);
            DrawScrollBar(item, scrollBar);

            int shift = item.Selected ? 1 : 0;
            Rectangle rectangle = new Rectangle(
                item.ClientRectangle.Left + shift,
                item.ClientRectangle.Top,
                item.ClientRectangle.Width - shift * 2 - ShadowDistance,
                item.ClientRectangle.Height);
            item.Buffer.SetClip(rectangle);
            DrawLinkPoint(item, linkPoints);
            item.Buffer.SetClip(item.ClientRectangle);
        }

        private void CalculateWorkplaceRectangle(FlowchartItem item)
        {
            item.WorkplaceRectangle = new Rectangle(
                Padding.Left, 
                Padding.Top, 
                item.Width - Padding.Left - Padding.Right, 
                item.Height - Padding.Top - Padding.Bottom);
        }

        private void DrawBody(FlowchartItem item)
        {
            //Calculate body rectangle
            Rectangle rectangle = item.ClientRectangle;

            ItemPainterHelper.DrawRoundedRectangleShadow(item.Buffer,
                ref rectangle, CornerRadius, ShadowDistance, ShadowColor);

            //Calculate body graphics path
            item.ItemRegion = ItemPainterHelper.CalculateRoundedRectangleGraphicsPath(
                rectangle, CornerRadius);

            //Draw body gradient fill
            using (Brush gradientBrush = new LinearGradientBrush(rectangle,
                TopColor, BottomColor, 90, false))
            {
                Rectangle bodyRectangle = new Rectangle(
                    rectangle.Left,
                    rectangle.Top,
                    rectangle.Width - 1,
                    rectangle.Height - 1);
                ItemPainterHelper.DrawRoundedRectangle(item.Buffer,
                    bodyRectangle, CornerRadius, null, gradientBrush);
            }

            //Draw unselected body frame
            if (!item.Selected)
            {
                ItemPainterHelper.DrawRoundedRectangle(item.Buffer, rectangle,
                    CornerRadius, _outerFramePen, null);
            }

            //Draw internal frame
            ItemPainterHelper.DrawRoundedRectangle(item.Buffer, rectangle,
                CornerRadius, _innerFramePen, null);
        }

        private void DrawHeader(FlowchartItem item)
        {
            //Calculate header rectangle
            RectangleF bounds = item.ItemRegion.GetBounds();
            RectangleF headerRectangle = new RectangleF(bounds.Left + 1, 
                bounds.Top, bounds.Left + bounds.Width - 2, bounds.Top + 
                HeaderHeight);

            //Draw header frame
            item.Buffer.DrawLine(_headerLinePen, headerRectangle.X, 
                headerRectangle.Y + headerRectangle.Height - 1, headerRectangle.X +
                headerRectangle.Width, headerRectangle.Y + headerRectangle.Height - 1);

            //Draw header icon
            if (item.Icon != null)
            {
                Point iconPosition = new Point(
                    (int) (headerRectangle.Left + headerRectangle.Width - item.Icon.Width - 1),
                    (int) (headerRectangle.Top + (headerRectangle.Height/2 - (float)item.Icon.Height/2)) + 1);
                item.Buffer.DrawImageUnscaled(item.Icon, iconPosition);
            }

            //Draw header caption
            if (!string.IsNullOrEmpty(item.Caption))
            {
                item.Buffer.DrawString(item.Caption, HeaderFont, _headerTextBrush, 
                    headerRectangle, HeaderFormat);
            }
        }

        private void DrawWorkplace(FlowchartItem item)
        {
            //Calculate workplace width and height
            int workplaceWidth = item.WorkplaceRectangle.Width;
            int workplaceHeight = item.WorkplaceRectangle.Height;

            //Store workplace buffer
            item.WorkplaceBuffer.SetBuffer(workplaceWidth, workplaceHeight);

            //Paint workplace onto buffer
            item.WorkplaceBuffer.Buffer.DrawImage(item.BufferBitmap, 0, 0,
                item.WorkplaceRectangle, GraphicsUnit.Pixel);
            item.WorkplaceBuffer.Buffer.FillRectangle(_workplaceBrush, 0, 0,
                workplaceWidth - 1, workplaceHeight - 1);
            item.WorkplaceBuffer.Buffer.DrawRectangle(_workplacePen, 0, 0,
                workplaceWidth - 1, workplaceHeight - 1);

            //Draw workplace buffer
            item.Buffer.DrawImageUnscaledAndClipped(item.WorkplaceBuffer.BufferBitmap,
                item.WorkplaceRectangle);
        }

        private void DrawWorkplaceFromBuffer(FlowchartItem item)
        {
            //Draw workplace buffer
            item.Buffer.DrawImageUnscaledAndClipped(item.WorkplaceBuffer.BufferBitmap,
                item.WorkplaceRectangle);
        }

        private void DrawItemElements(FlowchartItem item, ItemScrollBar scrollBar)
        {
            //Mark all visibled item control as destroyed
            foreach (IItemControl control in item.ControlsOnScreen.Values)
            {
                control.Destroyed = true;
            }

            //Get scroll bar padding and calculate control rectangle
            int scrollBarPadding = scrollBar.Visible ? ScrollBarPadding : 1;
            Rectangle controlsRectangle = new Rectangle(item.WorkplaceRectangle.Left,
                item.WorkplaceRectangle.Top, item.WorkplaceRectangle.Width -
                scrollBar.Width - scrollBarPadding, item.WorkplaceRectangle.Height);

            //Fix bad situation, when scroll position equal to max. value and
            //item has increase own vertical size
            int areaSizeDiff = controlsRectangle.Height - scrollBar.AreaSize;
            if (scrollBar.AreaSize > 0 && areaSizeDiff > 0 &&
                scrollBar.Position + areaSizeDiff >= scrollBar.MaxValue)
            {
                scrollBar.SetPositionSilent(Math.Max(scrollBar.MaxValue - areaSizeDiff, 0));
            }

            //Calculate current screen height (with positioning)
            int curHeight = -scrollBar.Position + GroupSpaceHeight;

            List<ItemGroup> groups = item.Groups.GetVisibleGroups();
            foreach (ItemGroup group in groups)
            {
                List<ItemGroupElement> elements = group.GetVisibleElements();

                // +1 - for the divider line
                bool gDrawing = curHeight + GroupAreaHeight + 1 >= 0 &&
                                curHeight <= controlsRectangle.Height;
                if (gDrawing)
                {
                    DrawGroup(item, group, elements.Count, controlsRectangle,
                              curHeight + controlsRectangle.Top);
                }

                curHeight += GroupAreaHeight + GroupSpaceHeight + 1;

                if (!group.Collapsed && elements.Count > 0)
                {
                    curHeight -= 1;
                    foreach (ItemGroupElement element in elements)
                    {
                        bool eDrawing = curHeight + GroupElementAreaHeight >= 0 &&
                                        curHeight <= controlsRectangle.Height;
                        if (eDrawing)
                        {
                            DrawGroupElement(item, element, controlsRectangle,
                                curHeight + controlsRectangle.Top);
                        }
                        curHeight += GroupElementAreaHeight;
                    }
                }
                else
                {
                    curHeight -= GroupSpaceHeight + 1;
                }
            }

            curHeight += GroupElementSpaceHeight + 1;

            //Update scrollbar rectangle
            scrollBar.ClientRectangle = new Rectangle(Padding.Left + (item.Width -
                Padding.Left - Padding.Right) - scrollBar.Width - scrollBarPadding,
                controlsRectangle.Top + ScrollBarPadding, scrollBar.Width,
                controlsRectangle.Height - ScrollBarPadding * 2);

            //Set scrollbar area size and max value
            scrollBar.AreaSize = controlsRectangle.Height;
            scrollBar.MaxValue = Math.Max(curHeight + scrollBar.Position -
                controlsRectangle.Height, 0);

            //Remove hidden item controls
            RemoveHiddenItemControls(item);
        }

        private void GroupPostPaint(Graphics graphics, IItemControl control)
        {
            //Calculate plus/minus button rectangle
            int buttonPadding = (GroupAreaHeight - GroupPlusMinusSize)/2;
            Rectangle buttonRectangle = new Rectangle(control.ClientRectangle.Left +
                control.ClientRectangle.Width - buttonPadding - GroupPlusMinusSize, 
                control.ClientRectangle.Top + buttonPadding, GroupPlusMinusSize, 
                GroupPlusMinusSize);

            //Draw button
            graphics.FillRectangle(_groupPlusMinusFillBrush, buttonRectangle);
            graphics.DrawRectangle(_groupPlusMinusFramePen, buttonRectangle);

            //Draw minus mark
            const int markPadding = 2;
            int midY = buttonRectangle.Top + buttonRectangle.Height / 2;
            graphics.DrawLine(_groupPlusMinusMarkPen, buttonRectangle.Left +
                markPadding, midY, buttonRectangle.Left + buttonRectangle.Width - 
                markPadding, midY);

            //Draw plus mark
            ItemGroup group = (ItemGroup) control.UserObject;
            if (group.Collapsed)
            {
                int midX = buttonRectangle.Left + buttonRectangle.Width/2;
                graphics.DrawLine(_groupPlusMinusMarkPen, midX, buttonRectangle.Top +
                    markPadding, midX, buttonRectangle.Top + buttonRectangle.Height -
                    markPadding);
            }
        }

        private void DrawGroup(FlowchartItem item, ItemGroup group, 
            int elementsCount, Rectangle workplaceRectangle, int curHeight)
        {
            //Calculate group rectangle
            Rectangle groupRectangle = new Rectangle(workplaceRectangle.X +
                GroupSpaceHeight, curHeight, workplaceRectangle.Width -
                (GroupSpaceHeight * 2), GroupAreaHeight);

            //Draw group icon
            int leftMargin = 0;
            if (group.Icon != null && group.IconVisible)
            {
                leftMargin = (groupRectangle.Height / 2 - group.Icon.Height / 2) *
                    3 + group.Icon.Width;
            }

            //Draw group underline
            item.Buffer.DrawLine(_headerLinePen, groupRectangle.X, groupRectangle.Y +
                groupRectangle.Height, groupRectangle.X + groupRectangle.Width,
                groupRectangle.Y + groupRectangle.Height);

            //Create item control for group
            CreateItemButton(item, groupRectangle, ButtonColor, ButtonCaptionColor,
                GroupFont, leftMargin, group.Icon, group.IconVisible, 
                string.Format("{0} ({1})", group.Caption, elementsCount), 
                group, null, GroupPostPaint);
        }

        private void DrawGroupElement(FlowchartItem item, ItemGroupElement element,
            Rectangle workplaceRectangle, int curHeight)
        {
            //Calculate group element rectangle
            Rectangle elementRectangle = new Rectangle(workplaceRectangle.X +
                GroupElementLeftPadding, curHeight, workplaceRectangle.Width -
                GroupElementSpaceHeight - GroupElementLeftPadding,
                GroupElementAreaHeight);

            //Get element text according to nesting level
            string text = string.Empty;
            if (element.NestingLevel > 0)
            {
                for (int i = 0; i < element.NestingLevel; i++)
                {
                    text += i < element.NestingLevel - 1 ? "  " : " ";
                }
                text = string.Format("{0}• {1}", text, element.Text);
            }
            else
            {
                text = element.Text;
            }

            //Create item control for element
            ItemButton button = CreateItemButton(item, elementRectangle, 
                ButtonColor, ButtonCaptionColor, GroupElementFont, 0, null, 
                false, text, element, null, null);

            Rectangle iconStateRectangle = new Rectangle(workplaceRectangle.X +
                GroupSpaceHeight, curHeight, 16, 16);
            CreateItemImage(item, iconStateRectangle, element.Icon, element.IconVisible,
                element.IconCursor, element.IconHint, button, null, null);
        }

        private void DrawScrollBar(FlowchartItem item, ItemScrollBar scrollBar)
        {
            scrollBar.Draw(item.Buffer, ScrollBodyColor, ScrollFrontColor, 
                ScrollMarksColor);
        }

        public override void UpdateLinkPoints(FlowchartItem item, ItemLinkPoint[] linkPoints)
        {
            //Update left link point
            ItemLinkPoint lpLeft = linkPoints[0];
            lpLeft.Size = WorkplaceMargin + 1;
            int leftX = item.ClientRectangle.Left;
            int leftY = item.ClientRectangle.Top + (item.ClientRectangle.Height / 2);
            Point leftLinkPointLocation = new Point(leftX, leftY);
            lpLeft.ClientRectangle = new Rectangle(leftLinkPointLocation.X,
                leftLinkPointLocation.Y - lpLeft.Size / 2, lpLeft.Size, lpLeft.Size);

            //Update right link point
            ItemLinkPoint lpRight = linkPoints[1];
            lpRight.Size = WorkplaceMargin + 1;
            int rightX = item.ClientRectangle.Left + item.ClientRectangle.Width -
                ShadowDistance;
            int rightY = item.ClientRectangle.Top + (item.ClientRectangle.Height / 2);
            Point rightLinkPointLocation = new Point(rightX, rightY);
            lpRight.ClientRectangle = new Rectangle(rightLinkPointLocation.X - lpRight.Size,
                rightLinkPointLocation.Y - lpRight.Size / 2, lpRight.Size, lpRight.Size);
        }

        private void DrawLinkPoint(FlowchartItem item, ItemLinkPoint[] linkPoints)
        {
            UpdateLinkPoints(item, linkPoints);
            if (!item.ReadOnly)
            {
                if (item.LeftLinkPointVisible)
                {
                    linkPoints[0].Draw(item.Buffer, LeftLinkPointColor, LeftLinkPointColorMouseOn,
                                       LeftLinkPointColorSelected);
                }
                if (item.RightLinkPointVisible)
                {
                    linkPoints[1].Draw(item.Buffer, RightLinkPointColor, RightLinkPointColorMouseOn,
                                       RightLinkPointColorSelected);
                }
            }
        }

        private void DrawSelectionBodyFrame(FlowchartItem item)
        {
            if (item.Selected)
            {
                Rectangle rectangle = new Rectangle(
                    item.ClientRectangle.X,
                    item.ClientRectangle.Y, 
                    item.ClientRectangle.Width - ShadowDistance,
                    item.ClientRectangle.Height - ShadowDistance);
                ItemPainterHelper.DrawRoundedRectangle(item.Buffer, rectangle,
                    CornerRadius, _outerFrameSelectedPen, null);
            }
        }

#endregion

    }
}
