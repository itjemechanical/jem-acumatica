using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using PX.Common;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CA;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.DR;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PO;
using PX.Objects.TX;
using POLine = PX.Objects.PO.POLine;
using POOrder = PX.Objects.PO.POOrder;
using PX.CarrierService;
using CRLocation = PX.Objects.CR.Standalone.Location;
using ARRegisterAlias = PX.Objects.AR.Standalone.ARRegisterAlias;
using PX.Objects.AR.MigrationMode;
using PX.Objects.Common;
using PX.Objects.Common.Discount;
using PX.Objects.Common.Extensions;
using PX.CS.Contracts.Interfaces;
using Message = PX.CarrierService.Message;
using PX.Data.DependencyInjection;
using PX.Data.WorkflowAPI;
using PX.LicensePolicy;
using PX.Objects.SO.GraphExtensions.CarrierRates;
using PX.Objects.SO.GraphExtensions.SOOrderEntryExt;
using PX.Objects.SO.Attributes;
using PX.Objects.Common.Attributes;
using PX.Objects.Common.Bql;
using OrderActions = PX.Objects.SO.SOOrderEntryActionsAttribute;
using PX.Data.BQL.Fluent;
using PX.Objects.IN.InventoryRelease;
using PX.Data.BQL;
using PX.Objects.IN.InventoryRelease.Utility;
using PX.Objects.SO.Standalone;
using PX.Objects.IN.InventoryRelease.Accumulators.QtyAllocated;
using PX.Objects.Common.Interfaces;
using PX.Objects;
using PX.Objects.SO;

namespace PX.Objects.SO
{
  public class SOOrderEntry_Extension : PXGraphExtension<PX.Objects.SO.SOOrderEntry>
  {
    #region Overwrite ReleaseFromHold
    public delegate IEnumerable ReleaseFromHoldDelegate(PXAdapter adapter);
    
    [PXOverride]
    public IEnumerable ReleaseFromHold(PXAdapter adapter, ReleaseFromHoldDelegate baseMethod)
    {
        bool error = false;

        // Obtener la orden actual
        SOOrder currentOrder = Base.Document.Current;

        if (currentOrder == null)
        {
            throw new PXException("No order is currently selected.");
        }

        // Verificar si el tipo de orden es "PT" Project Transfer
        if (currentOrder.OrderType == "PT")
        {
            // Validamos solo las líneas de la orden actual
            foreach (SOLine line in PXSelect<SOLine,
                     Where<SOLine.orderNbr, Equal<Required<SOLine.orderNbr>>,
                     And<SOLine.orderType, Equal<Required<SOLine.orderType>>>>>.Select(Base, currentOrder.OrderNbr, currentOrder.OrderType))
            {
                  // Verificamos que la cantidad de la orden sea válida
                if (line.OrderQty > 0)
                {
                    decimal qtyAvail = GetAvailableQty(line); // Método para obtener la cantidad disponible
                    if (qtyAvail < line.OrderQty)
                    {
                        PXUIFieldAttribute.SetError<SOLine.orderQty>(Base.Caches[typeof(SOLine)], line,
                            $"The order quantity ({line.OrderQty}) exceeds the available quantity ({qtyAvail}).");
                        error = true;
                    }
                }
            }
    
            if (error)
            {
              throw new PXException("Cannot release hold because order quantity exceeds available quantity.");
            }
        }

        

        // Si todo está correcto, proceder con el ReleaseFromHold original
        return baseMethod(adapter);
    }
    // Método adicional para obtener la cantidad disponible
    private decimal GetAvailableQty(SOLine line)
    {
        // Obtener el SubItemID de la línea, si no existe, asignarle un valor predeterminado (puedes definir un valor por defecto)
        int? subItemID = line.SubItemID;
        if (subItemID == null)
        {
                    return 0m;
        }

        // Obtener la cantidad disponible para el artículo en el sitio y subitem especificado
        INSiteStatus siteStatus = PXSelect<INSiteStatus,
            Where<INSiteStatus.inventoryID, Equal<Required<INSiteStatus.inventoryID>>,
            And<INSiteStatus.siteID, Equal<Required<INSiteStatus.siteID>>,
            And<INSiteStatus.subItemID, Equal<Required<INSiteStatus.subItemID>>>>>>.Select(Base, line.InventoryID, line.SiteID, subItemID);

        // Si encontramos la información de disponibilidad, devolver la cantidad disponible
        if (siteStatus != null)
        {
            return siteStatus.QtyAvail.GetValueOrDefault();
        }

        // Si no encontramos datos, devolvemos 0
        return 0m;
    }
    #endregion
     
    #region Event Handlers

    protected void SOLine_OrderQty_FieldVerifying(PXCache cache, PXFieldVerifyingEventArgs e, PXFieldVerifying InvokeBaseHandler)
    {
      
      var row = (SOLine)e.Row;
      if (row == null)
        return;

      decimal? NewOrderQty = (decimal?)e.NewValue;
      // Si SubItemID es nulo y se requiere, puedes asignarle un valor por defecto o manejarlo según corresponda.
      int? subItemID = row.SubItemID;

      //Bring Available QTY
      INSiteStatus siteStatus = PXSelect<INSiteStatus,
        Where<INSiteStatus.inventoryID, Equal<Required<INSiteStatus.inventoryID>>,
          And<INSiteStatus.siteID, Equal<Required<INSiteStatus.siteID>>,
          And<INSiteStatus.subItemID, Equal<Required<INSiteStatus.subItemID>>>>>>.Select(Base, row.InventoryID, row.SiteID, subItemID);
      
      if (siteStatus != null && row.SiteID != null)
      {
        decimal qtyAvail = siteStatus.QtyAvail.GetValueOrDefault();
        if (NewOrderQty > qtyAvail)
        {
          throw new PXSetPropertyException($"The order quantity ({NewOrderQty}) exceeds the available quantity ({qtyAvail}).");
        }
      }
      else if (row.SiteID != null)
      {
        throw new PXSetPropertyException("Not in stock");
      }

      if(InvokeBaseHandler != null)
        InvokeBaseHandler(cache, e);
      
      
    }
    
    protected void SOOrder_RowPersisting(PXCache cache, PXRowPersistingEventArgs e, PXRowPersisting InvokeBaseHandler)
    {
        SOOrder order = e.Row as SOOrder;
        if (order == null)
            return;

        foreach (SOLine line in Base.Transactions.Select())
        {
            // Obtiene el estado extendido del campo (incluye posibles errores)
            PXFieldState state = Base.Transactions.Cache.GetValueExt<SOLine.orderQty>(line) as PXFieldState;
            if (state != null && !string.IsNullOrEmpty(state.Error))
            {
                // Se lanza excepción para bloquear la persistencia de la orden completa
                throw new PXRowPersistingException(typeof(SOLine.orderQty).Name, line.OrderQty, state.Error);
            }
        }

        if(InvokeBaseHandler != null)
            InvokeBaseHandler(cache, e);
    }
          
    protected void SOOrder_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
    {
      var order = (SOOrder)e.Row;
      if (order == null || order.RequestDate == null || order.OrderDate == null)
          return;

      // Comparar solo fechas, ignorando horas
      if (order.Status == SOOrderStatus.Hold && order.RequestDate.Value.Date < order.OrderDate.Value.Date)
      {
        sender.RaiseExceptionHandling<SOOrder.requestDate>(
            order,
            order.RequestDate,
            new PXSetPropertyException("Request date cannot be earlier than Order date. This date determines the shipment date of the lines."));

        throw new PXRowPersistingException(nameof(SOOrder.requestDate), order.RequestDate,
            "Request date cannot be earlier than Order date. This date determines the shipment date of the lines.");
      }
    }

    #endregion
          
    #region actions
    public PXAction<SOOrder> PrintSOWithDeviceHub;
    [PXButton(CommitChanges = true)]
    [PXUIField(DisplayName = "Print with DeviceHub", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
    protected virtual IEnumerable printSOWithDeviceHub(PXAdapter adapter)
    {
        SOOrder order = Base.Document.Current;
        if (order == null)
            return adapter.Get();
    
        PXLongOperation.StartOperation(Base, () =>
        {
            var reportID = "SO641010";
            var parameters = new Dictionary<string, string>
            {
                ["OrderType"] = order.OrderType,
                ["OrderNbr"] = order.OrderNbr
            };
        
            DeviceHubUtility.PrintReportToDeviceHub(Base, reportID, parameters);
        });
    
        return adapter.Get();
    }
          
    protected void SOLine_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
    {
      if (e.Row is SOLine line)
      {
        if (line.OrderQty <= 0)
        {
            sender.RaiseExceptionHandling<SOLine.orderQty>(
                line,
                line.OrderQty,
                new PXSetPropertyException("Order quantity must be greater than 0.", PXErrorLevel.Error)
            );
         }
        }
    }
    #endregion
  }
}