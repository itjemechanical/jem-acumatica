using System;
using PX.Data;
using PX.Objects.PO;

namespace PX.SM
{
    public class WikiFileMaintenance_Extension : PXGraphExtension<WikiFileMaintenance>
    {
        protected void _(Events.RowSelected<UploadFile> e)
        {
            if (e.Row == null) return;

            bool poNotInHold = IsFileLinkedToPOOrderNotInHold(e.Row.FileID);
            bool poReceiptNotInHold = IsFileLinkedToReceiptNotInHold(e.Row.FileID);

            if (poNotInHold || poReceiptNotInHold)
            {
                PXUIFieldAttribute.SetEnabled<UploadFile.fileID>(e.Cache, e.Row, false);

                Base.Delete.SetEnabled(false);
                Base.Actions["uploadNewVersion"]?.SetEnabled(false);
                
                // También bloquear el borrado de versiones desde el grid Revisions
                Base.Revisions.AllowDelete = false;
            }
            else
            {
                // Habilitar si no está bloqueado
                Base.Revisions.AllowDelete = true;
            }
        }

        protected void _(Events.RowSelected<UploadFileRevisionNoData> e)
        {
            if (e.Row == null) return;

            if (IsFileLinkedToPOOrderNotInHold(e.Row.FileID))
            {
                PXUIFieldAttribute.SetEnabled<UploadFileRevisionNoData.fileID>(e.Cache, e.Row, false);
            }
        }

        private bool IsFileLinkedToPOOrderNotInHold(Guid? fileID)
        {
            if (fileID == null)
                return false;

            // Buscar si el archivo tiene una relación con un NoteDoc
            NoteDoc noteDoc = PXSelect<NoteDoc,
                Where<NoteDoc.fileID, Equal<Required<NoteDoc.fileID>>>>
                .Select(Base, fileID)
                ?.TopFirst;

            if (noteDoc == null)
                return false;

            // Buscar si ese NoteDoc está asociado a un POOrder
            POOrder poOrder = PXSelect<POOrder,
                Where<POOrder.noteID, Equal<Required<POOrder.noteID>>>>
                .Select(Base, noteDoc.NoteID)
                ?.TopFirst;

            if (poOrder == null)
                return false;

            // El archivo debe bloquearse si el estado del PO no es Hold
            return poOrder.Status != POOrderStatus.Hold;
        }
          
        private bool IsFileLinkedToReceiptNotInHold(Guid? fileID)
        {
            if (fileID == null)
                return false;

            // Buscar NoteDoc relacionado al archivo
            NoteDoc noteDoc = PXSelect<NoteDoc,
                Where<NoteDoc.fileID, Equal<Required<NoteDoc.fileID>>>>
                .Select(Base, fileID)
                ?.TopFirst;

            if (noteDoc == null)
                return false;

            // Buscar si ese NoteDoc está vinculado a un POReceipt
            POReceipt receipt = PXSelect<POReceipt,
                Where<POReceipt.noteID, Equal<Required<POReceipt.noteID>>>>
                .Select(Base, noteDoc.NoteID)
                ?.TopFirst;

            if (receipt == null)
                return false;

            // Bloquear si el Receipt no está en estado Hold
            return receipt.Status != POReceiptStatus.Hold;
        }
    }
}