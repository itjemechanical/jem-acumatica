using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Data.WorkflowAPI;
using PX.Objects.CN.Common.Descriptor;
using PX.Objects.CN.Common.Extensions;
using PX.Objects.CN.Common.Helpers;
using PX.Objects.Common.Extensions;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.PJ.Common.CacheExtensions;
using PX.Objects.PJ.Common.Descriptor;
using PX.Objects.PJ.ProjectManagement.Descriptor;
using PX.Objects.PJ.ProjectManagement.PJ.DAC;
using PX.Objects.PJ.ProjectManagement.PJ.Graphs;
using PX.Objects.PJ.ProjectManagement.PJ.Services;
using PX.Objects.PJ.ProjectManagement.PM.CacheExtensions;
using PX.Objects.PJ.ProjectsIssue.PJ.DAC;
using PX.Objects.PJ.ProjectsIssue.PJ.Descriptor.Attributes;
using PX.Objects.PJ.RequestsForInformation.Descriptor;
using PX.Objects.PJ.RequestsForInformation.PJ.DAC;
using PX.Objects.PJ.RequestsForInformation.PJ.Descriptor.Attributes;
using PX.Objects.PJ.RequestsForInformation.PJ.Descriptor.Attributes.DocumentSelectorProviders;
using PX.Objects.PJ.RequestsForInformation.PJ.Descriptor.Lists;
using PX.Objects.PJ.RequestsForInformation.PJ.Extensions;
using PX.Objects.PJ.RequestsForInformation.PJ.Services;
using PX.Objects.PJ.RequestsForInformation.PM.DAC;
using PX.Objects.PM;
using PX.Objects.PM.ChangeRequest;
using Constants = PX.Objects.PJ.RequestsForInformation.PM.Descriptor.Constants;
using Messages = PX.Objects.CR.Messages;
using PmConstants = PX.Objects.PJ.ProjectManagement.PJ.Descriptor.Constants;
using PX.Objects;
using PX.Objects.PJ.RequestsForInformation.PJ.Graphs;

namespace PX.Objects.PJ.RequestsForInformation.PJ.Graphs
{
  public class RequestForInformationMaint_Extension : PXGraphExtension<PX.Objects.PJ.RequestsForInformation.PJ.Graphs.RequestForInformationMaint>
  {
    #region Event Handlers

    #endregion
  }
}