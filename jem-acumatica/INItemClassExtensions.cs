using PX.Data.BQL.Fluent;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.DR;
using PX.Objects.IN;
using PX.Objects.TX;
using PX.Objects;
using PX.TM;
using SelectParentItemClass = PX.Data.BQL.Fluent.SelectFrom<PX.Objects.IN.INItemClass>.Where<PX.Objects.IN.INItemClass.itemClassID.IsEqual<PX.Objects.IN.INItemClass.parentItemClassID.FromCurrent>>;
using System.Collections.Generic;
using System;

namespace PX.Objects.IN
{
  public class INItemClassExt : PXCacheExtension<PX.Objects.IN.INItemClass>
  {
    #region UsrJEMPhase
    [PXDBString(2)] // Limita el campo a 2 caracteres para almacenar solo los códigos
[PXStringList(
    new string[] { "01", "02", "03", "04", "05", "06", "07", "08", "09" },
    new string[] { 
        "01 - Vents", 
        "02 - Ducts", 
        "03 - Copper", 
        "04 - Hang AHU", 
        "05 - Condenser Unit", 
        "06 - Ventilation", 
        "07 - Wall caps", 
        "08 - Trim", 
        "09 - Complementaries" 
    }
)]
    [PXUIField(DisplayName = "Phase")]
    public virtual string UsrJEMPhase { get; set; }
    public abstract class usrJEMPhase : PX.Data.BQL.BqlString.Field<usrJEMPhase> { }
    #endregion
  }
}