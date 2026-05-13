using CRLocation = PX.Objects.CR.Standalone.Location;
using PX.Common;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Data.WorkflowAPI;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CA;
using PX.Objects.CM;
using PX.Objects.Common.Attributes;
using PX.Objects.Common.Extensions;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CS.Attributes;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN.RelatedItems;
using PX.Objects.IN;
using PX.Objects.PM;
using PX.Objects.SO.Attributes;
using PX.Objects.SO.Interfaces;
using PX.Objects.SO;
using PX.Objects.TX;
using PX.Objects;
using PX.TM;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace PX.Objects.SO
{
      [PXNonInstantiatedExtension]
  public class SO_SOOrder_ExistingColumn : PXCacheExtension<PX.Objects.SO.SOOrder>
  {
      #region ProjectID  
        [PXMergeAttributes(Method = MergeMethod.Append)]
[PXRestrictor( typeof(Where<PMProject.customerID, Equal<Current<SOOrder.customerID>>>), "El proyecto no pertenece al cliente de la orden", typeof(PMProject.contractCD) )]
      public int? ProjectID { get; set; }
      #endregion

      #region OrderDate  
      [PXDBDateAndTime()]
    [PXDefault(typeof(AccessInfo.businessDate))]
    [PXUIField(DisplayName = "Date", Visibility = PXUIVisibility.SelectorVisible)]
      public DateTime? OrderDate { get; set; }
      #endregion
  }
}