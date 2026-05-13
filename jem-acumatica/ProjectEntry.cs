using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CommonServiceLocator;
using PX.Api;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.DependencyInjection;
using PX.LicensePolicy;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.CA;
using PX.Objects.CM.Extensions;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.Objects.CR.Extensions;
using PX.Objects.CS;
using PX.Objects.CT;
using PX.Objects.EP;
using PX.Objects.Extensions.MultiCurrency;
using PX.Objects.GL;
using PX.Objects.GL.FinPeriods;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.SO;
using PX.SM;
using System.Diagnostics;
using PX.Objects.CN.ProjectAccounting;
using PX.Objects;
using PX.Objects.PM;

namespace PX.Objects.PM
{
  public class ProjectEntry_Extension : PXGraphExtension<PX.Objects.PM.ProjectEntry>
  {
    #region Event Handlers

    protected void PMProject_RowInserting(PXCache cache, PXRowInsertingEventArgs e)
    {
      
      var row = (PMProject)e.Row;
      
      // Asign a value to custom field UsrTotalUnits
      row.GetExtension<ContractExt>().UsrTotalUnits = 1; // Example: Fixed Value 100
    }

    

    #endregion
  }
}