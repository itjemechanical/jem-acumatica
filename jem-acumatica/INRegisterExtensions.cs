using PX.Data.BQL.Fluent;
using PX.Data.BQL;
using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data.WorkflowAPI;
using PX.Data;
using PX.Objects.CM;
using PX.Objects.Common.Attributes;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.SO;
using PX.Objects;
using System.Collections.Generic;
using System;

namespace PX.Objects.IN
{
  public class INRegisterExt : PXCacheExtension<PX.Objects.IN.INRegister>
  {
    #region UsrRFP
    [PXDBString(11)]
    [PXUIField(DisplayName="R.F.P.")]

    public virtual string UsrRFP { get; set; }
    public abstract class usrRFP : PX.Data.BQL.BqlString.Field<usrRFP> { }
    #endregion
  }
}