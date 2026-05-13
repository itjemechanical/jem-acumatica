using PX.Data.EP;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data.WorkflowAPI;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CM.Extensions;
using PX.Objects.Common.Discount;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.CT;
using PX.Objects.GL;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects;
using PX.TM;
using System.Collections.Generic;
using System.Collections;
using System;

namespace PX.Objects.CT
{
  public class ContractExt : PXCacheExtension<PX.Objects.CT.Contract>
  {
    #region UsrTotalUnits
    [PXDBInt]
    [PXUIField(DisplayName="Total Units")]

    public virtual int? UsrTotalUnits { get; set; }
    public abstract class usrTotalUnits : PX.Data.BQL.BqlInt.Field<usrTotalUnits> { }
    #endregion
  }
}