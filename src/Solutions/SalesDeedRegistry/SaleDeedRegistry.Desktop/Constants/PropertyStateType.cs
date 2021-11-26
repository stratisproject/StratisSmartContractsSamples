using System.ComponentModel;

namespace SaleDeedRegistry.Desktop.Constants
{
    public enum PropertyStateType : uint
    {
        [Description("NotStarted")]
        NotStarted = 0,
        [Description("InProgress")]
        InProgress = 1,
        [Description("UnderReview")]
        UnderReview = 2,
        [Description("ReviewComplete")]
        ReviewComplete = 3,
        [Description("PaidTransferFee")]
        PaidTransferFee = 4,
        [Description("Approved")]
        Approved = 5,
        [Description("Rejected")]
        Rejected = 6
    }
}
