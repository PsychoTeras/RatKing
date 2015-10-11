using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GMechanics.FlowchartControl.ItemControls;

namespace GMechanics.FlowchartControl.ItemPainters.Painters
{
    internal class ItemPainter_Base : IItemPainter
    {
        private Padding _padding = new Padding(0, 0, 0, 0);
        private readonly Font _headerFont = new Font("Verdana", 8.6f, FontStyle.Bold);
        private readonly Font _groupFont = new Font("Verdana", 7.4f, FontStyle.Bold);
        private readonly Font _groupElementFont = new Font("Verdana", 7.3f, FontStyle.Regular);
        private readonly StringFormat _headerFormat = new StringFormat(StringFormatFlags.NoWrap)
                                                          {
                                                              Alignment = StringAlignment.Center,
                                                              LineAlignment = StringAlignment.Center,
                                                              Trimming = StringTrimming.EllipsisCharacter
                                                          };
        public Padding Padding
        {
            get { return _padding; }
            set { _padding = value; }
        }

        public virtual string Name { get { return null; } }

        protected virtual Font HeaderFont { get { return _headerFont; } }

        protected virtual Font GroupFont { get { return _groupFont; } }

        protected virtual Font GroupElementFont { get { return _groupElementFont; } }

        protected virtual StringFormat HeaderFormat { get { return _headerFormat; } }

        protected virtual int HeaderHeight { get { return 20; } }

        protected virtual int CornerRadius { get { return 15; } }

        protected virtual int ShadowDistance { get { return 4; } }

        protected virtual int GroupAreaHeight { get { return 18; } }

        protected virtual int GroupElementAreaHeight { get { return 16; } }

        protected virtual int GroupSpaceHeight { get { return 2; } }

        protected virtual int GroupPlusMinusSize { get { return 10; } }

        protected virtual int GroupElementSpaceHeight { get { return 2; } }

        protected virtual int GroupElementLeftPadding { get { return 22; } }

        protected virtual int WorkplaceMargin { get { return 9; } }

        protected virtual int ScrollBarPadding { get { return 2; } }

        protected virtual Color ShadowColor { get { return Color.Black; } }

        public virtual void UpdateLinkPoints(FlowchartItem item, ItemLinkPoint[] linkPoints) { }

        public virtual void Paint(FlowchartItem item, ItemScrollBar scrollBar,
            ItemLinkPoint[] linkPoints) { }

        public int CompareTo(IItemPainter other)
        {
            return String.CompareOrdinal(Name, other.Name);
        }

        public override string ToString()
        {
            return Name;
        }

        public void RemoveHiddenItemControls(FlowchartItem item)
        {
            //Create hidden controls list
            List<IItemControl> forRemove = new List<IItemControl>();
            foreach (IItemControl control in item.ControlsOnScreen.Values)
            {
                if (control.Destroyed)
                {
                    forRemove.Add(control);
                }
            }

            //Remove hidden controls and update ControlUnderCursor item member
            int forRemoveCnt = forRemove.Count;
            for (int i = 0; i < forRemoveCnt; i++)
            {
                IItemControl control = forRemove[i];
                item.ControlsOnScreen.Remove(control.UserObject);
                if (item.ControlUnderCursor != null &&
                    item.ControlUnderCursor == control)
                {
                    item.ControlUnderCursor = null;
                }
                control.Dispose();
            }
        }

        public ItemButton CreateItemButton(FlowchartItem item, Rectangle rectangle,
            Color color, Color captionColor, Font font, int leftMargin, 
            Image icon, bool iconVisible, string caption, object userObject, 
            OnItemPrePaint prePaint, OnItemPostPaint postPaint)
        {
            ItemButton button;
            if (!item.ControlsOnScreen.ContainsKey(userObject))
            {
                button = new ItemButton(rectangle, userObject);
                button.Caption = caption;
                button.Font = font;
                button.TextMargin = leftMargin;
                button.Color = color;
                button.CaptionColor = captionColor;
                button.ItemPrePaint += prePaint;
                button.ItemPostPaint += postPaint;
                item.ControlsOnScreen.Add(userObject, button);
            }
            else
            {
                button = (ItemButton) item.ControlsOnScreen[userObject];
                button.ClientRectangle = rectangle;
            }
            if (iconVisible)
            {
                button.Icon = icon;
            }
            button.Caption = caption;
            button.Destroyed = false;
            return button;
        }

        public void CreateItemImage(FlowchartItem item, Rectangle rectangle,
            Image image, bool imageVisible, Cursor cursor, string hint,
            object userObject,  OnItemPrePaint prePaint, OnItemPostPaint postPaint)
        {
            ItemImage itemImage = null;
            if (!item.ControlsOnScreen.ContainsKey(userObject))
            {
                if (image != null && imageVisible)
                {
                    itemImage = new ItemImage(rectangle, userObject);
                    itemImage.ItemPrePaint += prePaint;
                    itemImage.ItemPostPaint += postPaint;
                    item.ControlsOnScreen.Add(userObject, itemImage);
                }
            }
            else
            {
                itemImage = (ItemImage) item.ControlsOnScreen[userObject];
                if (image == null || !imageVisible)
                {
                    itemImage.Destroyed = true;
                    itemImage = null;
                }
                else
                {
                    itemImage.ClientRectangle = rectangle;
                }
            }

            if (itemImage != null)
            {
                itemImage.Image = image;
                itemImage.Cursor = cursor;
                itemImage.Hint = hint;
                itemImage.Destroyed = false;
            }
        }

        public void DrawControls(FlowchartItem item)
        {
            foreach (IItemControl control in item.ControlsOnScreen.Values)
            {
                control.Draw(item.Buffer);
            }
        }

        public virtual void Dispose()
        {
            _headerFont.Dispose();
            _headerFormat.Dispose();
            _groupFont.Dispose();
            _groupElementFont.Dispose();
        }
    }
}
