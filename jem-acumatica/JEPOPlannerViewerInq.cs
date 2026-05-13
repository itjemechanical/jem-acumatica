using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.PM;
using PX.Objects.PO;

namespace PX.Objects.JE
{
    public class JEPOPlannerViewerInq : PXGraph<JEPOPlannerViewerInq>
    {
        public PXCancel<JEPOPlannerViewerFilter> Cancel;
        public PXFilter<JEPOPlannerViewerFilter> Filter;

        [PXFilterable]
        public SelectFrom<JEPOPlannerViewerRow>.View.ReadOnly Results;

        public override bool IsDirty => false;

        protected virtual IEnumerable results()
        {
            int counterRowKey = 0;
            PXDelegateResult result = new PXDelegateResult
            {
                IsResultFiltered = true,
                IsResultSorted = true,
                IsResultTruncated = false
            };

            JEPOPlannerViewerFilter filter = Filter.Current;

            if (filter?.StartDate == null || filter.EndDate == null)
                return result;

            // Pre-cargar el último log de cada orden en un solo query (evita N+1)
            var lastLogByOrder = new Dictionary<(string, string), JEPOOrderLogLine>();

            foreach (PXResult<JEPOOrderLogLine, POOrder> logItem in
                SelectFrom<JEPOOrderLogLine>
                    .InnerJoin<POOrder>
                        .On<POOrder.orderType.IsEqual<JEPOOrderLogLine.orderType>
                            .And<POOrder.orderNbr.IsEqual<JEPOOrderLogLine.orderNbr>>>
                    .Where<
                        POOrder.expectedDate.IsNotNull
                        .And<POOrder.status.IsNotEqual<POOrderStatus.hold>>
                        .And<POOrder.expectedDate.IsGreaterEqual<JEPOPlannerViewerFilter.startDate.FromCurrent>>
                        .And<POOrder.expectedDate.IsLessEqual<JEPOPlannerViewerFilter.endDate.FromCurrent>>
                        .And<POOrder.orderType.IsIn<POOrderType.regularOrder, POOrderType.projectDropShip>>>
                    .View.ReadOnly
                    .Select(this))
            {
                JEPOOrderLogLine log = logItem;
                POOrder o = logItem;
                var key = (o.OrderType, o.OrderNbr);

                if (!lastLogByOrder.TryGetValue(key, out JEPOOrderLogLine existing) || log.LineNbr > existing.LineNbr)
                    lastLogByOrder[key] = log;
            }

            DateTime? lastDate = null;

            foreach (PXResult<POOrder, PMProject, POShipAddress> item in
                SelectFrom<POOrder>
                    .LeftJoin<PMProject>
                        .On<PMProject.contractID.IsEqual<POOrder.projectID>
                            .And<PMProject.baseType.IsEqual<CT.CTPRType.project>>>
                    .LeftJoin<POShipAddress>
                        .On<POShipAddress.addressID.IsEqual<POOrder.shipAddressID>>
                    .Where<
                        POOrder.expectedDate.IsNotNull
                        .And<POOrder.status.IsNotEqual<POOrderStatus.hold>>
                        .And<POOrder.expectedDate.IsGreaterEqual<JEPOPlannerViewerFilter.startDate.FromCurrent>>
                        .And<POOrder.expectedDate.IsLessEqual<JEPOPlannerViewerFilter.endDate.FromCurrent>>
                        .And<POOrder.orderType.IsIn<POOrderType.regularOrder, POOrderType.projectDropShip>>>
                    .OrderBy<
                        POOrder.expectedDate.Asc,
                        POOrder.orderNbr.Asc>
                    .View.ReadOnly
                    .Select(this))
            {
                POOrder order = item;
                PMProject project = item;
                POShipAddress shipAddress = item;

                DateTime expectedDate = order.ExpectedDate.Value.Date;

                if (lastDate != expectedDate)
                {
                    result.Add(new JEPOPlannerViewerRow
                    {
                        RowKey = counterRowKey,
                        Level = 0,
                        DisplayText = expectedDate.ToString("M/d/yyyy"),
                        ExpectedDate = expectedDate
                    });

                    lastDate = expectedDate;
                    counterRowKey++;
                }

                lastLogByOrder.TryGetValue((order.OrderType, order.OrderNbr), out JEPOOrderLogLine lastLog);

                result.Add(new JEPOPlannerViewerRow
                {
                    RowKey = counterRowKey,
                    //RowKey = $"O|{expectedDate:yyyyMMdd}|{order.OrderType}|{order.OrderNbr}",
                    Level = 1,
                    DisplayText = "",
                    ExpectedDate = expectedDate,
                    ProjectID = order.ProjectID,
                    ProjectCD = project?.ContractCD,
                    ProjectDescription = project?.Description,
                    OrderType = order.OrderType,
                    OrderNbr = order.OrderNbr,
                    CreatedDateTime = order.CreatedDateTime,
                    LastJEPOOrderLogLine_Description = lastLog?.Descr,
                    AddressLine1 = shipAddress?.AddressLine1
                });
                counterRowKey++;
            }

            return result;
        }

        #region Log Lines
        [PXFilterable]
        public SelectFrom<JEPOOrderLogLine>
            .Where<
                JEPOOrderLogLine.orderType.IsEqual<JEPOPlannerViewerRow.orderType.FromCurrent>
                .And<JEPOOrderLogLine.orderNbr.IsEqual<JEPOPlannerViewerRow.orderNbr.FromCurrent>>>
            .OrderBy<JEPOOrderLogLine.lineNbr.Asc>
            .View.ReadOnly LogLines;

        public PXFilter<JEAddCommentFilter> CommentDialog;

        #endregion

        #region POLines
        [PXFilterable]
        public SelectFrom<POLine>
            .Where<
                POLine.orderType.IsEqual<JEPOPlannerViewerRow.orderType.FromCurrent>
                .And<POLine.orderNbr.IsEqual<JEPOPlannerViewerRow.orderNbr.FromCurrent>>>
            .OrderBy<POLine.lineNbr.Asc>
            .View.ReadOnly POLines;
        #endregion

        #region Actions
        public PXAction<JEPOPlannerViewerRow> AddUserComment;
        [PXButton(CommitChanges = true, DisplayOnMainToolbar = false)]
        [PXUIField(DisplayName = "Add Comment")]
        protected virtual IEnumerable addUserComment(PXAdapter adapter)
        {

            if (Results.Current?.OrderType == null || Results.Current.OrderNbr == null)
                return adapter.Get();

            if (CommentDialog.AskExt() != WebDialogResult.OK)
                return adapter.Get();

            JEAddCommentFilter filter = CommentDialog.Current;
            if (filter == null || string.IsNullOrWhiteSpace(filter.Comment))
                return adapter.Get();

            JEPOOrderLogLine maxLineRec = PXSelectGroupBy<JEPOOrderLogLine,
                Where<JEPOOrderLogLine.orderType, Equal<Required<JEPOOrderLogLine.orderType>>,
                    And<JEPOOrderLogLine.orderNbr, Equal<Required<JEPOOrderLogLine.orderNbr>>>>,
                Aggregate<Max<JEPOOrderLogLine.lineNbr>>>
                .Select(this, Results.Current.OrderType, Results.Current.OrderNbr);

            int nextLine = (maxLineRec?.LineNbr ?? 0) + 1;

            JEPOOrderLogLine logLine = new JEPOOrderLogLine
            {
                OrderType = Results.Current.OrderType,
                OrderNbr = Results.Current.OrderNbr,
                LineNbr = nextLine,
                EventType = JEPOOrderLogLineEventType.UserMessage,
                Descr = filter.Comment
            };

            Caches[typeof(JEPOOrderLogLine)].Insert(logLine);
            Persist();

            filter.Comment = null;
            CommentDialog.Update(filter);
            LogLines.View.Clear();

            return adapter.Get();
        }
        #endregion
    }
}