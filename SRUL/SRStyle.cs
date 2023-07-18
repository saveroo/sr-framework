using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using SRUL.Types;
using TB.ComponentModel;

namespace SRUL;

public class SRStyle : IDisposable
{
    private GridView _view = null;
    static Feature? GetCurrentFieldRow(GridView view, int rowHandle)
    {
        var field = view.GetRow(rowHandle) as Feature;
        if (field == null) return null;
        // var fieldValue = field.GetType().GetProperty(fieldName).GetValue(field);
        return field;
    }
    
    private void ResourceRowStyle(object o, RowStyleEventArgs args)
    {
        var view = o as GridView;
        if (view == null) return;
        if (args.RowHandle == GridControl.InvalidRowHandle) return;
        var field = GetCurrentFieldRow(view, args.RowHandle);
        if (field == null) return;
        switch (field.name)
        {
            case string a when a.Contains("Production") && a.Contains("Annually"):
                string demand = field.name.Replace("Production", "Demand").GetFeature(SREnum.CategoryName.Resources)
                    ?.value;
                if (String.IsNullOrEmpty(demand)) return;
                // var production = name.Replace("Demand", "Production").GetFeature(SREnum.CategoryName.Resources)?.value;
                if (demand?.To<double>() < field.value?.To<double>())
                    args.Appearance.ForeColor = Color.LimeGreen;
                else
                    args.Appearance.ForeColor = Color.PaleVioletRed;
                break;
        }
    }

    private void EditableStyle(object o, RowStyleEventArgs args)
    {
        var view = o as GridView;
        if (args.RowHandle == GridControl.InvalidRowHandle) return;
        var feat = (view.GetRow(args.RowHandle) as Feature);
        var editable = view.GetRowCellValue(args.RowHandle, "editable") is bool &&
                       (bool)view.GetRowCellValue(args.RowHandle, "editable");
        if(feat == null) return;
        if(!editable) 
            args.Appearance.BackColor = Color.Black;
        else if (!feat.IsReadable)
            args.Appearance.BackColor = Color.FromArgb(255, 29 ,2, 5);
        else
            args.Appearance.BackColor = default;
    }

    private void GridView_ShowingEditor(object sender, CancelEventArgs e)
    {
        var view = sender as GridView;
        var feat = view?.GetRow(view.FocusedRowHandle) as Feature;
        if(feat == null) return;
        if(!feat.editable)
            e.Cancel = true;
        if (!feat.IsReadable)
            e.Cancel = true;
    }   

    private void warfareOriginalComparatorHelper<T>(T original, T currentValue, 
        Color ForeA,
        Color BackA, 
        Color ForeB,
        Color BackB,
        ref RowStyleEventArgs args) 
        where T : IComparable<T>
    {
        if (original.CompareTo(currentValue) > 0)
        {
            args.Appearance.ForeColor = ForeA;
            args.Appearance.BackColor = BackA;
        }

        if (original.CompareTo(currentValue) < 0)
        {
            args.Appearance.ForeColor = ForeB;
            args.Appearance.BackColor = BackB;
        }
    }
    private void WarfareOriginalComparatorStyle(object o, RowStyleEventArgs args)
    {
        var view = o as GridView;
        if (view == null) return;
        if(args.RowHandle == GridControl.InvalidRowHandle) return;
        if (args.RowHandle < 0) return;
        var feature = GetCurrentFieldRow(view, args.RowHandle);
        if (feature == null) return;
        if (!feature.editable) return;
        if (!GetCurrentFieldRow(view, args.RowHandle)!.category.Equals("Warfare")) return;
        
        var original = GetCurrentFieldRow(view, args.RowHandle)?.original;
        var current = GetCurrentFieldRow(view, args.RowHandle)?.value;
        var type = GetCurrentFieldRow(view, args.RowHandle)!.type;

        var backColor = Color.Bisque;
        
        if(original == null) return;
        switch (type)
        {
            case "byte": 
                warfareOriginalComparatorHelper<byte>(
                    original.As<byte>().OrDefault(), 
                    current.As<byte>().OrDefault(),
                    Color.WhiteSmoke,
                    Color.Brown,
                    Color.WhiteSmoke,
                    Color.DarkGreen,
                    ref args);
                // if (original.To<byte>() > current.To<byte>())
                //     args.Appearance.BackColor = Color.Brown;
                // if (original.To<byte>() < current.To<byte>())
                //     args.Appearance.BackColor = backColor;
                break;
            case "2bytes":
                warfareOriginalComparatorHelper<ushort>(
                    original.As<ushort>().OrDefault(), 
                    current.As<ushort>().OrDefault(),
                    Color.WhiteSmoke,
                    Color.Brown,
                    Color.WhiteSmoke,
                    Color.DarkGreen,
                    ref args);
                break;
            case "2byte":
                warfareOriginalComparatorHelper<ushort>(
                    original.As<ushort>().OrDefault(), 
                    current.As<ushort>().OrDefault(), 
                    Color.WhiteSmoke,
                    Color.Brown,
                    Color.WhiteSmoke,
                    Color.DarkGreen,
                    ref args);
                break;
            case "int":
                warfareOriginalComparatorHelper<decimal>(
                    original.As<decimal>().OrDefault(), 
                    current.As<decimal>().OrDefault(), 
                    Color.WhiteSmoke,
                    Color.Brown,
                    Color.WhiteSmoke,
                    Color.DarkGreen,
                    ref args);
                break;
            case "float":
                warfareOriginalComparatorHelper<float>(
                    original.As<float>().OrDefault(), 
                    current.As<float>().OrDefault(), 
                    Color.WhiteSmoke,
                    Color.Brown,
                    Color.WhiteSmoke,
                    Color.DarkGreen,
                    ref args);
                break;
        }
        
    }
    
    // Highlight GRIDVIEW when value increased or decreased
    public void GvHighlight(GridView gv)
    {
        GridFormatRule Highlight(string ruleName,
            string iconName,
            string colorFill,
            FormatConditionDataUpdateTrigger trigger)
        {
            GridFormatRule gridFormatRule = new GridFormatRule();
            FormatConditionRuleDataUpdate dataUpdate = new FormatConditionRuleDataUpdate();
            
            dataUpdate.HighlightTime = 1000;
            dataUpdate.Icon.PredefinedName = iconName;
            dataUpdate.PredefinedName = colorFill;
            dataUpdate.Trigger = trigger;
            dataUpdate.AllowAnimation = DefaultBoolean.True;
            
            gridFormatRule.Name = ruleName;
            gridFormatRule.Rule = dataUpdate;
            gridFormatRule.ApplyToRow = true;
            gridFormatRule.Column = gridFormatRule.ColumnApplyTo = gv.Columns["value"];

            return gridFormatRule;
        }
        
        gv.FormatRules.Add(Highlight("FormatIncreased",
            "Arrows3_1.png",
            "Green Fill",
            FormatConditionDataUpdateTrigger.ValueIncreased));
        gv.FormatRules.Add(Highlight("FormatDecreased",
            "Arrows3_3.png",
            "Red Fill",
            FormatConditionDataUpdateTrigger.ValueDecreased));
    }

    public void gvRowStyle(GridView dgv)
    {
        _view = dgv;
        dgv.RowStyle += (sender, args) =>
        {
            EditableStyle(sender, args);
            WarfareOriginalComparatorStyle(sender, args);
            ResourceRowStyle(sender, args);
        };

        dgv.ShowingEditor += GridView_ShowingEditor;
    }

    public void Dispose()
    {
        _view = null;
        // throw new NotImplementedException();
    }
}