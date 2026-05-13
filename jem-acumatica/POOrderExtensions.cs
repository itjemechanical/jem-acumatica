using CRLocation = PX.Objects.CR.Standalone.Location;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data.WorkflowAPI;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.CM.Extensions;
using PX.Objects.CN.Subcontracts.SC.Graphs;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS.Attributes;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.PO;
using PX.Objects.TX;
using PX.Objects;
using PX.TM;
using System.Collections.Generic;
using System;

namespace PX.Objects.PO
{
  public class POOrderExt : PXCacheExtension<PX.Objects.PO.POOrder>
  {
    #region UsrRFP
    [PXDBString(11)]
    [PXUIField(DisplayName="R.F.P.")]

    public virtual string UsrRFP { get; set; }
    public abstract class usrRFP : PX.Data.BQL.BqlString.Field<usrRFP> { }
    #endregion

    #region UsrJobAccount
    [PXDBString]
    [PXUIField(DisplayName="Job Account")]

    public virtual string UsrJobAccount { get; set; }
    public abstract class usrJobAccount : PX.Data.BQL.BqlString.Field<usrJobAccount> { }
    #endregion

    #region UsrNonPrintableNote
    [PXDBString(255)]
    [PXUIField(DisplayName="Non Printable Note")]

    public virtual string UsrNonPrintableNote { get; set; }
    public abstract class usrNonPrintableNote : PX.Data.BQL.BqlString.Field<usrNonPrintableNote> { }
    #endregion

    #region UsrReceiver
    [PXDBString]
    [PXUIField(DisplayName="Receiver")]

    public virtual string UsrReceiver { get; set; }
    public abstract class usrReceiver : PX.Data.BQL.BqlString.Field<usrReceiver> { }
    #endregion

    #region UsrReceiverPhone
    [PXDBString(50)]
    [PXUIField(DisplayName="Receiver Phone")]

    public virtual string UsrReceiverPhone { get; set; }
    public abstract class usrReceiverPhone : PX.Data.BQL.BqlString.Field<usrReceiverPhone> { }
    #endregion

    #region UsrIntReceivingStartTime
    [PXTimeList]
    [PXDBInt]
    [PXUIField(DisplayName="Receiving Start Time")]
    public virtual int? UsrIntReceivingStartTime { get; set; }
    public abstract class usrIntReceivingStartTime : PX.Data.BQL.BqlInt.Field<usrIntReceivingStartTime> { }
    #endregion

    #region UsrIntReceivingEndTime
    [PXTimeList]
    [PXDBInt]
    [PXUIField(DisplayName="Receiving End Time")]
    public virtual int? UsrIntReceivingEndTime { get; set; }
    public abstract class usrIntReceivingEndTime : PX.Data.BQL.BqlInt.Field<usrIntReceivingEndTime> { }
    #endregion
  }

      [PXNonInstantiatedExtension]
  public class PO_POOrder_ExistingColumn : PXCacheExtension<PX.Objects.PO.POOrder>
  {
      #region BranchID  
        [PXMergeAttributes(Method = MergeMethod.Append)]

      public int? BranchID { get; set; }
      #endregion
  }
}