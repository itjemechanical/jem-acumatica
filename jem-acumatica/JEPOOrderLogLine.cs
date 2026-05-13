using System;
using System.Text;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.PO;

namespace PX.Objects.JE
{
  [Serializable]
  [PXCacheName("JEPOOrderLogLine")]
  [PXPrimaryGraph(
    new Type[] { typeof(POOrderEntry) },
    new Type[] {
        typeof(Select<JEPOOrderLogLine,
            Where<JEPOOrderLogLine.orderType, Equal<Current<JEPOOrderLogLine.orderType>>,
                And<JEPOOrderLogLine.orderNbr, Equal<Current<JEPOOrderLogLine.orderNbr>>>>>)
    })]
  public class JEPOOrderLogLine : PXBqlTable, IBqlTable
  {
    #region Keys
    public class PK : PrimaryKeyOf<JEPOOrderLogLine>.By<orderType, orderNbr, lineNbr>
    {
      public static JEPOOrderLogLine Find(
      PXGraph graph, string orderType, string orderNbr, int lineNbr)
      => FindBy(graph, orderType, orderNbr, lineNbr);
    }

    public class POOrderFK : POOrder.PK.ForeignKeyOf<POOrder>.By<orderType, orderNbr> { }

    #endregion

    #region OrderNbr
    [PXDBString(15, IsKey = true, IsUnicode = true, InputMask = "")]
    [PXParent(typeof(Select<POOrder,
    Where<POOrder.orderType, Equal<Current<orderType>>,
        And<POOrder.orderNbr, Equal<Current<orderNbr>>>>>))]
    public virtual string OrderNbr { get; set; }
    public abstract class orderNbr : PX.Data.BQL.BqlString.Field<orderNbr> { }
    #endregion

    #region OrderType
    [PXDBString(2, IsKey = true, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Order Type")]
    public virtual string OrderType { get; set; }
    public abstract class orderType : PX.Data.BQL.BqlString.Field<orderType> { }
    #endregion

    #region LineNbr
    [PXDBInt(IsKey = true)]
    [PXUIField(DisplayName = "Line Nbr")]
    public virtual int? LineNbr { get; set; }
    public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
    #endregion

    #region Descr
    [PXDBString(256, IsUnicode = true, InputMask = "")]
    [PXUIField(DisplayName = "Descr")]
    public virtual string Descr { get; set; }
    public abstract class descr : PX.Data.BQL.BqlString.Field<descr> { }
    #endregion

    #region EventType
    [PXDBString(4, InputMask = "")]
    [PXUIField(DisplayName = "Event Type")]
    [JEPOOrderLogLineEventType.List()]
    public virtual string EventType { get; set; }
    public abstract class eventType : PX.Data.BQL.BqlString.Field<eventType> { }
    #endregion

    #region OldStatus
    [PXDBString(1, InputMask = "")]
    [PXUIField(DisplayName = "Old Status Code")]
    public virtual string OldStatus { get; set; }
    public abstract class oldStatus : PX.Data.BQL.BqlString.Field<oldStatus> { }
    #endregion

    #region NewStatus
    [PXDBString(1, InputMask = "")]
    [PXUIField(DisplayName = "New Status Code")]
    public virtual string NewStatus { get; set; }
    public abstract class newStatus : PX.Data.BQL.BqlString.Field<newStatus> { }
    #endregion

    #region CreatedByID
    [PXDBCreatedByID()]
    public virtual Guid? CreatedByID { get; set; }
    public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
    #endregion

    #region CreatedByScreenID
    [PXDBCreatedByScreenID()]
    public virtual string CreatedByScreenID { get; set; }
    public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
    #endregion

    #region CreatedDateTime
    public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
    protected DateTime? _CreatedDateTime;
    [PXDBCreatedDateTime(DisplayMask = "g", InputMask = "g")]
    [PXUIField(DisplayName = "Created Datetime", Enabled = false, IsReadOnly = true)]
    public virtual DateTime? CreatedDateTime
    {
      get
      {
        return this._CreatedDateTime;
      }
      set
      {
        this._CreatedDateTime = value;
      }
    }
    #endregion

    #region LastModifiedByID
    [PXDBLastModifiedByID()]
    public virtual Guid? LastModifiedByID { get; set; }
    public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
    #endregion

    #region LastModifiedByScreenID
    [PXDBLastModifiedByScreenID()]
    public virtual string LastModifiedByScreenID { get; set; }
    public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
    #endregion

    #region LastModifiedDateTime
    [PXDBLastModifiedDateTime]
    [PXUIField(DisplayName = "Last Modified Datetime")]
    public virtual DateTime? LastModifiedDateTime { get; set; }
    public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
    #endregion

    #region Tstamp
    [PXDBTimestamp()]
    [PXUIField(DisplayName = "Tstamp")]
    public virtual byte[] Tstamp { get; set; }
    public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
    #endregion

    #region Noteid
    [PXNote()]
    public virtual Guid? Noteid { get; set; }
    public abstract class noteid : PX.Data.BQL.BqlGuid.Field<noteid> { }
    #endregion

    //------------------------------- Campos Virtuales -----------------------------------------
    #region OldStatusDescr
    [PXString(60, IsUnicode = true)]
    [PXUIField(DisplayName = "Old Status", Enabled = false)]
    public virtual string OldStatusDescr
    {
      get => JEPOOrderStatusHelper.GetDescriptionOrNull(OldStatus);
      set { }
    }
    public abstract class oldStatusDescr : PX.Data.BQL.BqlString.Field<oldStatusDescr> { }
    #endregion

    #region NewStatusDescr
    [PXString(60, IsUnicode = true)]
    [PXUIField(DisplayName = "New Status", Enabled = false)]
    public virtual string NewStatusDescr
    {
      get => JEPOOrderStatusHelper.GetDescriptionOrNull(NewStatus);
      set { }
    }
    public abstract class newStatusDescr : PX.Data.BQL.BqlString.Field<newStatusDescr> { }
    #endregion
  }

  public class JEPOOrderLogLineEventType
  {
    // Definición de constantes
    public const string Created = "CRTD";
    public const string Modified = "MODF";
    public const string UserMessage = "UMSG";
    public const string ApprovalRequest = "RFAP";
    public const string Approved = "APPR";
    public const string Rejected = "RJCT";

    // Clases BQL para cada constante
    public class created : PX.Data.BQL.BqlString.Constant<created>
    {
      public created() : base(Created) { }
    }

    public class modified : PX.Data.BQL.BqlString.Constant<modified>
    {
      public modified() : base(Modified) { }
    }

    public class userMessage : PX.Data.BQL.BqlString.Constant<userMessage>
    {
      public userMessage() : base(UserMessage) { }
    }

    public class approvalRequest : PX.Data.BQL.BqlString.Constant<approvalRequest>
    {
      public approvalRequest() : base(ApprovalRequest) { }
    }

    public class approved : PX.Data.BQL.BqlString.Constant<approved>
    {
      public approved() : base(Approved) { }
    }

    public class rejected : PX.Data.BQL.BqlString.Constant<rejected>
    {
      public rejected() : base(Rejected) { }
    }

    // Atributo para el dropdown
    public class ListAttribute : PXStringListAttribute
    {
      public ListAttribute()
        : base(
          new string[]
          {
            Created,
            Modified,
            UserMessage,
            ApprovalRequest,
            Approved,
            Rejected
          },
          new string[]
          {
            "Created",
            "Modified",
            "User Message",
            "Approval Request",
            "Approved",
            "Rejected"
          })
      { }
    }
  }

  [Serializable]
  [PXCacheName("Add Comment")]
  public class JEAddCommentFilter : PXBqlTable, IBqlTable
  {
    #region Comment
    [PXString(4000, IsUnicode = true)]
    [PXUIField(DisplayName = "Comment")]
    public virtual string Comment { get; set; }
    public abstract class comment : PX.Data.BQL.BqlString.Field<comment> { }
    #endregion
  }

  //---------------- Helpers -------------------
  public static class JEPOOrderStatusHelper
  {
    private static readonly Lazy<POOrderStatusListAccessor> StatusList =
        new Lazy<POOrderStatusListAccessor>(() => new POOrderStatusListAccessor());

    public static string GetDescriptionOrNull(string status)
    {
      if (string.IsNullOrEmpty(status))
        return null;

      return StatusList.Value.GetDescriptionOrNull(status);
    }

    private sealed class POOrderStatusListAccessor : POOrderStatus.ListAttribute
    {
      public string GetDescriptionOrNull(string status)
      {
        int index = Array.IndexOf(_AllowedValues, status);

        if (index < 0 || index >= _AllowedLabels.Length)
          return null;

        return _AllowedLabels[index];
      }
    }
  }
}