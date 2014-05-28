﻿using System;
using System.Windows.Forms;
using PowerPointLabs.Models;
using Office = Microsoft.Office.Core;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace PowerPointLabs
{
    class ConvertToPicture
    {
        private const string ErrorTypeNotSupported = "Convert to Picture only supports Shapes and Charts.";
        private const string ErrorWindowTitle = "Unable to Convert to Picture";

        public static void ConvertSelectionToPicture()
        {
            var selection = PowerPointPresentation.CurrentSelection;
            if (IsSelectionShape(selection))
            {
                var shape = GetShapeFromSelection(selection);
                shape = CutPasteShape(shape);
                ConvertToPictureForShape(ref shape);
            }
            else
            {
                MessageBox.Show(ErrorTypeNotSupported, ErrorWindowTitle);
            }
        }

        public static PowerPoint.Shape ConvertToPictureForShape(ref PowerPoint.Shape shape)
        {
            float rotation = 0;
            try
            {
                rotation = shape.Rotation;
                shape.Rotation = 0;
            }
            catch (Exception e)
            {
                PowerPointLabsGlobals.LogException(e, "Chart cannot be rotated.");
            }
            shape.Copy();
            float x = shape.Left;
            float y = shape.Top;
            float width = shape.Width;
            float height = shape.Height;
            shape.Delete();
            var pic = PowerPointLabsGlobals.GetCurrentSlide().Shapes.PasteSpecial(PowerPoint.PpPasteDataType.ppPastePNG)[1];
            pic.Left = x + (width - pic.Width) / 2;
            pic.Top = y + (height - pic.Height) / 2;
            pic.Rotation = rotation;
            pic.Select();
            return pic;
        }

        public static System.Drawing.Bitmap GetConvertToPicMenuImage(Office.IRibbonControl control)
        {
            try
            {
                return new System.Drawing.Bitmap(Properties.Resources.ConvertToPicture);
            }
            catch (Exception e)
            {
                PowerPointLabsGlobals.LogException(e, "GetConvertToPicMenuImage");
                throw;
            }
        }

        /// <summary>
        /// To avoid corrupted shape.
        /// Corrupted shape is produced when delete or cut a shape programmatically, but then users undo it.
        /// After that, most of operations on corrupted shapes will throw an exception.
        /// One solution for this is to re-allocate its memory: simply cut/copy and paste.
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        private static PowerPoint.Shape CutPasteShape(PowerPoint.Shape shape)
        {
            shape.Cut();
            shape = PowerPointLabsGlobals.GetCurrentSlide().Shapes.Paste()[1];
            return shape;
        }

        private static PowerPoint.Shape GetShapeFromSelection(PowerPoint.Selection selection)
        {
            PowerPoint.Shape shape = 
                selection.ShapeRange.Count > 1 ? selection.ShapeRange.Group() : selection.ShapeRange[1];
            return shape;
        }

        private static bool IsSelectionShape(PowerPoint.Selection selection)
        {
            return selection.Type == PowerPoint.PpSelectionType.ppSelectionShapes;
        }
    }
}
