using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Common;
using PX.Data;
using PX.Objects.GL;
using PX.Objects.CM.Extensions;
using PX.Objects.CS;
using PX.Objects.CR;
using PX.Objects.TX;
using PX.Objects.IN;
using PX.Objects.EP;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.SO;
using SOOrder = PX.Objects.SO.SOOrder;
using SOLine = PX.Objects.SO.SOLine;
using PX.Data.DependencyInjection;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.LicensePolicy;
using PX.Objects.PM;
using CRLocation = PX.Objects.CR.Standalone.Location;
using PX.Objects.AP.MigrationMode;
using PX.Objects.Common;
using PX.Objects.Common.Discount;
using PX.Data.BQL.Fluent;
using PX.Data.BQL;
using PX.Objects.Common.Bql;
using PX.Objects.Extensions.CostAccrual;
using PX.Objects.DR;
using PX.Data.WorkflowAPI;
using PX.Objects.Common.Scopes;
using PX.Objects.IN.Services;
using PX.Objects.Extensions.MultiCurrency;
using PX.Data.Description;
using PX.Objects.PO.GraphExtensions.POOrderEntryExt;
using PX.Objects.IN.InventoryRelease;
using PX.Objects.Common.Interfaces;
using PX.Objects.PO.DAC.Projections;
using PX.Objects;
using PX.Objects.PO;
using PX.Objects.JE;

namespace PX.Objects.PO
{

  public class POOrderEntry_Extension : PXGraphExtension<PX.Objects.PO.POOrderEntry>
  {
    private int MAINWARE_ID = 57;
    private int RETURN_SITE_ID = 529;

    #region actions
    public PXAction<POOrder> PrintPOWithDeviceHub;
    
    [PXButton(CommitChanges = true)]
    [PXUIField(DisplayName = "Print with DeviceHub", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
    protected virtual IEnumerable printPOWithDeviceHub(PXAdapter adapter)
    {
        POOrder order = Base.Document.Current;
        if (order == null)
            return adapter.Get();
    
        PXLongOperation.StartOperation(Base, () =>
        {
            var reportID = "PO641000";
            var parameters = new Dictionary<string, string>
            {
                ["OrderType"] = order.OrderType,
                ["OrderNbr"] = order.OrderNbr
            };
        
            DeviceHubUtility.PrintReportToDeviceHub(Base, reportID, parameters);
        });
    
        return adapter.Get();
    }
    #endregion
 
    #region Block InItem Description
    protected void POLine_TranDesc_FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
    {
        POLine row = (POLine)e.Row;
        if (row == null) return;
    
        if (IsNonStockItem(row))
        {
          PXUIFieldAttribute.SetEnabled<POLine.tranDesc>(sender, row, true);
        }
        else
        {
            PXUIFieldAttribute.SetEnabled<POLine.tranDesc>(sender, row, false);
        }
    }
    
    private bool IsNonStockItem(POLine line)
    {
        InventoryItem item = PXSelectorAttribute.Select<POLine.inventoryID>(Base.Transactions.Cache, line) as InventoryItem;
        return item != null && item.StkItem == false;
    }
    #endregion
      
    protected void POLine_RowPersisting(PXCache sender, PXRowPersistingEventArgs e)
    {
    if (e.Row is POLine line)
    {
        if (line.OrderQty <= 0)
        {
            sender.RaiseExceptionHandling<POLine.orderQty>(
                line,
                line.OrderQty,
                new PXSetPropertyException("Order quantity cannot be 0.00.", PXErrorLevel.Error)
            );
        }

        if (line.CuryUnitCost <= 0)
        {
            sender.RaiseExceptionHandling<POLine.curyUnitCost>(
                line,
                line.CuryUnitCost,
                new PXSetPropertyException("Unit cost cannot be 0.00.", PXErrorLevel.Error)
            );
        }

        // Solo validamos el siteID si el OrderType es "RO"
        /*POOrder order = Base.Document.Current;
        if (order != null && order.OrderType == "RO")
        {
          // Validación del siteID
          if (!isValidSite(line))
          {
              // Borra el SiteID si no es válido
              line.SiteID = null; // Establece el SiteID a null (vacío)
        
              // Lanza una excepción para indicar que el site es inválido
              sender.RaiseExceptionHandling<POLine.siteID>(
                  line,
                  line.SiteID,
                  new PXSetPropertyException("Invalid site. Please select 'MAINWARE' or 'RETURN'.", PXErrorLevel.Error)
              );
          }
        }*/
      }
    }
    
   // Función para validar si el siteID es válido
    protected bool isValidSite(POLine line)
    {
        // Verifica si el siteID de la POLine es uno de los dos sitios válidos
        if (line.SiteID == MAINWARE_ID || line.SiteID == RETURN_SITE_ID)
        {
            return true;
        }
        return false;
    }
      
    #region JEPOOrderLogLine
    // Data Views
    public PXSelect<JEPOOrderLogLine,
        Where<JEPOOrderLogLine.orderType, Equal<Current<POOrder.orderType>>,
            And<JEPOOrderLogLine.orderNbr, Equal<Current<POOrder.orderNbr>>>>>
        LogLines;

    public PXFilter<JEAddCommentFilter> CommentDialog;

    public PXAction<POOrder> AddUserComment;
    [PXButton(CommitChanges = true)]
    [PXUIField(DisplayName = "Add Comment", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
    protected virtual IEnumerable addUserComment(PXAdapter adapter)
    {
        var order = Base.Document.Current;
        if (order == null)
            return adapter.Get();

        if (string.IsNullOrWhiteSpace(order.OrderNbr))
            throw new PXException("Save the PO before adding a comment.");

        var result = CommentDialog.AskExt();
        if (result != WebDialogResult.OK)
            return adapter.Get();

        var filter = CommentDialog.Current;
        if (filter == null || string.IsNullOrWhiteSpace(filter.Comment))
            throw new PXException("Comment cannot be empty.");

        AddLogEntry(
            eventType: JEPOOrderLogLineEventType.UserMessage,
            description: filter.Comment,
            oldStatus: order.Status,
            newStatus: order.Status
        );

        Base.Actions.PressSave();

        // limpiar popup para el próximo uso
        filter.Comment = null;
        CommentDialog.Update(filter);

        return adapter.Get();
    }

    // Events
    protected virtual void _(Events.RowInserting<JEPOOrderLogLine> e)
    {
        if (e.Row == null) return;

        var row = e.Row;
        var order = Base.Document.Current;

        if (order == null)
        {
            // Si no hay orden actual, cancelar la inserción
            e.Cancel = true;
            return;
        }

        // Auto-asignar OrderType y OrderNbr desde el documento principal
        row.OrderType = order.OrderType;
        row.OrderNbr = order.OrderNbr;

        // Auto-incrementar LineNbr
        var maxLineNbr = LogLines.Select()
            .RowCast<JEPOOrderLogLine>()
            .Where(line => line.LineNbr != null)
            .Select(line => line.LineNbr.Value)
            .DefaultIfEmpty(0)
            .Max();

        row.LineNbr = maxLineNbr + 1;
    }

    protected virtual void _(Events.RowUpdated<POOrder> e)
    {
        try
        {
            if (e.Row == null) return;

            var newRow = e.Row;
            var oldRow = e.OldRow;

            if (
                oldRow.Status == newRow.Status
                || oldRow.Status == null
                || newRow.Status == null
                || oldRow.OrderNbr == null
                || oldRow.OrderType == null
               )
            {
                //Not Changes
                return;
            }

            // State Machine
            if (
                (oldRow.Status == POOrderStatus.PendingApproval) &&
                (newRow.Status == POOrderStatus.Open)
            )
            {
                // Registro aprobacion de PO
                AddLogEntry(
                  eventType: JEPOOrderLogLineEventType.Approved,
                  description: $"The PO was successfully approved.",
                  oldStatus: oldRow.Status,
                  newStatus: newRow.Status
                );
            }
            else if (
                (oldRow.Status != POOrderStatus.PendingApproval) &&
                (newRow.Status == POOrderStatus.PendingApproval)
            )
            {
                // registro rechazo de PO
                AddLogEntry(
                  eventType: JEPOOrderLogLineEventType.ApprovalRequest,
                  description: $"PO approval is requested.",
                  oldStatus: oldRow.Status,
                  newStatus: newRow.Status
                );
            }
            else if (
                (oldRow.Status == POOrderStatus.PendingApproval) &&
                (newRow.Status == POOrderStatus.Rejected)
            )
            {
                // registro rechazo de PO
                AddLogEntry(
                  eventType: JEPOOrderLogLineEventType.Rejected,
                  description: $"The PO was rejected.",
                  oldStatus: oldRow.Status,
                  newStatus: newRow.Status
                );
            }
            else if (oldRow.Status != newRow.Status)
            {
                // registro cambio de status
                AddLogEntry(
                    eventType: JEPOOrderLogLineEventType.Modified,
                    description: $"Order status changed.",
                    oldStatus: oldRow.Status,
                    newStatus: newRow.Status
                );
            }
        }
        catch (Exception ex)
        {
            PXTrace.WriteError(ex.ToString());
            // throw; // no se va a propagar el error, esta funcionalidad solo es de registro
        }
    }

    // Método alternativo para registrar eventos en el log
    public void AddLogEntry(string eventType, string description, string oldStatus = null, string newStatus = null)
    {
        try
        {
            var order = Base.Document.Current;
            if (order == null) return;

            if (!IsRealOrderNbr(order.OrderNbr)) return;

            var logLine = new JEPOOrderLogLine();
            logLine.OrderType = order.OrderType;
            logLine.OrderNbr = order.OrderNbr;
            logLine.EventType = eventType;
            logLine.Descr = description;
            logLine.OldStatus = oldStatus;
            logLine.NewStatus = newStatus;

            LogLines.Insert(logLine);
        }
        catch (Exception ex)
        {
            PXTrace.WriteVerbose(ex.ToString());
        }
    }
    
    private static bool IsRealOrderNbr(string orderNbr)
    {
        return !string.IsNullOrWhiteSpace(orderNbr)
            && !string.Equals(orderNbr.Trim(), "<NEW>", StringComparison.OrdinalIgnoreCase);
    }
    #endregion
 
  }
}