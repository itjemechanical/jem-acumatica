using System;
using System.Collections;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.PM;
using PX.Objects.PO;

namespace PX.Objects.JE
{
    [Serializable]
    [PXHidden]
    public class JEPOPlannerViewerFilter : PXBqlTable, IBqlTable
    {
        #region StartDate
        public abstract class startDate : BqlDateTime.Field<startDate> { }

        [PXDate]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "Start Date", Required = true)]
        public virtual DateTime? StartDate { get; set; }
        #endregion

        #region EndDate
        public abstract class endDate : BqlDateTime.Field<endDate> { }

        [PXDate]
        [PXDefault(typeof(AccessInfo.businessDate))]
        [PXUIField(DisplayName = "End Date", Required = true)]
        public virtual DateTime? EndDate { get; set; }
        #endregion
    }

    [Serializable]
    [PXHidden]
    public class JEPOPlannerViewerRow : PXBqlTable, IBqlTable
    {
        #region RowKey
        public abstract class rowKey : BqlInt.Field<rowKey> { }

        [PXInt(IsKey = true)]
        [PXUIField(DisplayName = "Row Key", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual int? RowKey { get; set; }
        #endregion

        #region Level
        public abstract class level : BqlInt.Field<level> { }

        [PXInt]
        [PXUIField(Visible = false)]
        public virtual int? Level { get; set; }
        #endregion

        #region DisplayText
        public abstract class displayText : BqlString.Field<displayText> { }

        [PXString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Promised On")]
        public virtual string DisplayText { get; set; }
        #endregion

        #region ExpectedDate
        public abstract class expectedDate : BqlDateTime.Field<expectedDate> { }

        [PXDate]
        [PXUIField(DisplayName = "Expected Date")]
        public virtual DateTime? ExpectedDate { get; set; }
        #endregion

        #region ProjectID
        public abstract class projectID : BqlInt.Field<projectID> { }

        [PXInt]
        [PXUIField(Visible = false)]
        public virtual int? ProjectID { get; set; }
        #endregion

        #region ProjectCD
        public abstract class projectCD : BqlString.Field<projectCD> { }

        [PXString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "Project")]
        public virtual string ProjectCD { get; set; }
        #endregion

        #region ProjectDescription
        public abstract class projectDescription : BqlString.Field<projectDescription> { }

        [PXString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Project Name")]
        public virtual string ProjectDescription { get; set; }
        #endregion

        #region OrderType
        public abstract class orderType : BqlString.Field<orderType> { }

        [PXString(2, IsFixed = true)]
        [PXUIField(DisplayName = "Order Type")]
        public virtual string OrderType { get; set; }
        #endregion

        #region OrderNbr
        public abstract class orderNbr : BqlString.Field<orderNbr> { }

        [PXString(15, IsUnicode = true)]
        [PXSelector(
            typeof(Search<POOrder.orderNbr,
                Where<POOrder.orderType, Equal<Current<orderType>>>>),
            typeof(POOrder.orderType),
            typeof(POOrder.orderNbr),
            typeof(POOrder.vendorID),
            typeof(POOrder.orderDate),
            typeof(POOrder.status),
            SubstituteKey = typeof(POOrder.orderNbr))]
        [PXUIField(DisplayName = "PO Nbr.")]
        public virtual string OrderNbr { get; set; }
        #endregion

        #region CreatedDateTime
        public abstract class createdDateTime : BqlDateTime.Field<createdDateTime> { }

        [PXDate]
        [PXUIField(DisplayName = "Created Date")]
        public virtual DateTime? CreatedDateTime { get; set; }
        #endregion

        #region LastJEPOOrderLogLine_Description
        public abstract class lastJEPOOrderLogLine_Description : BqlString.Field<lastJEPOOrderLogLine_Description> { }

        [PXString(256, IsUnicode = true)]
        [PXUIField(DisplayName = "Last Comment")]
        public virtual string LastJEPOOrderLogLine_Description { get; set; }
        #endregion

        #region AddressLine1
        public abstract class addressLine1 : BqlString.Field<addressLine1> { }

        [PXString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Address Line 1")]
        public virtual string AddressLine1 { get; set; }
        #endregion
    }

}