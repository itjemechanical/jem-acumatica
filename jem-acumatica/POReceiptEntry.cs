using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.DependencyInjection;
using PX.Data.WorkflowAPI;
using PX.LicensePolicy;
using PX.Objects.AP;
using PX.Objects.AP.MigrationMode;
using PX.Objects.CM.Extensions;
using PX.Objects.Common;
using PX.Objects.Common.Bql;
using PX.Objects.Common.Extensions;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.Extensions.CostAccrual;
using PX.Objects.Extensions.MultiCurrency;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.IN.InventoryRelease;
using PX.Objects.IN.InventoryRelease.Accumulators.QtyAllocated;
using PX.Objects.IN.Services;
using PX.Objects.PM;
using PX.Objects.PO.GraphExtensions.POReceiptEntryExt;
using PX.Objects.PO.LandedCosts;
using PX.Objects.PO.Scopes;
using PX.Objects.SO;
using PX.Objects.TX;
using CRLocation = PX.Objects.CR.Standalone.Location;
using ItemLotSerial = PX.Objects.IN.InventoryRelease.Accumulators.QtyAllocated.ItemLotSerial;
using SiteLotSerial = PX.Objects.IN.InventoryRelease.Accumulators.QtyAllocated.SiteLotSerial;
using SOLine4 = PX.Objects.SO.SOLine4;
using SOOrder = PX.Objects.SO.SOOrder;
using PX.Objects;
using PX.Objects.PO;

namespace PX.Objects.PO
{
  public class POReceiptEntry_Extension : PXGraphExtension<PX.Objects.PO.POReceiptEntry>
  {
    #region Event Handlers

    protected void POReceiptLine_RowSelected(PXCache cache, PXRowSelectedEventArgs e)
    {
        var row = e.Row as POReceiptLine;
        if (row == null) return;

        string displayName;

        switch (row.ReceiptType)
        {
            case "RN":
                displayName = "Return Qty.";
                break;
            case "RT":
                displayName = "Receipt Qty.";
                break;
            case "RX":
                displayName = "Transfer Qty.";
                break;
            default:
                displayName = "Qty.";
                break;
        }

        PXUIFieldAttribute.SetDisplayName<POReceiptLine.receiptQty>(cache, displayName);
      
    }

    
      
    protected void POReceiptLine_ReceiptQty_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e, PXFieldVerifying InvokeBaseHandler)
    {
    var row = (POReceiptLine)e.Row;
    if (row == null || e.NewValue == null || row.ReceiptType == "RN" || row.ReceiptType == "RX")
        return;

    decimal newReceiptQty = (decimal)e.NewValue;

    // Obtener la línea de orden de compra usando PXSelect
    POLine poLine = PXSelect<POLine,
        Where<POLine.orderNbr, Equal<Required<POLine.orderNbr>>,
            And<POLine.lineNbr, Equal<Required<POLine.lineNbr>>>>>
        .Select(Base, row.PONbr, row.POLineNbr);

    if (poLine == null)
    {
        throw new PXSetPropertyException("The corresponding purchase order line was not found.");
    }

    decimal openOrderQty = poLine.OpenQty ?? 0;

    if (newReceiptQty > openOrderQty)
    {
        throw new PXSetPropertyException($"The receipt quantity ({newReceiptQty}) cannot be greater than the remaining quantity on the purchase order ({openOrderQty}).");
    }

    InvokeBaseHandler?.Invoke(cache, e);
    }
  
    #endregion
  }
}