using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "GetTotalContributionsDonorFund")]
    public partial class GetTotalContributionsDonorFund
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs => new PropertyChangingEventArgs("");

        private int? _CreditGiverId;

        private string _HeadName;

        private string _SpouseName;

        private int? _Count;

        private decimal? _Amount;

        private decimal? _PledgeAmount;

        private int _FundId;

        private string _FundName;

        private string _MainFellowship;

        private string _MemberStatus;

        private DateTime? _JoinDate;

        private int _ContributionStatusId;
        private int _ContributionTypeId;

        public GetTotalContributionsDonorFund()
        {
        }

        [Column(Name = "CreditGiverId", Storage = "_CreditGiverId", DbType = "int")]
        public int? CreditGiverId
        {
            get => _CreditGiverId;

            set
            {
                if (_CreditGiverId != value)
                {
                    _CreditGiverId = value;
                }
            }
        }

        [Column(Name = "HeadName", Storage = "_HeadName", DbType = "nvarchar(140)")]
        public string HeadName
        {
            get => _HeadName;

            set
            {
                if (_HeadName != value)
                {
                    _HeadName = value;
                }
            }
        }

        [Column(Name = "SpouseName", Storage = "_SpouseName", DbType = "nvarchar(140)")]
        public string SpouseName
        {
            get => _SpouseName;

            set
            {
                if (_SpouseName != value)
                {
                    _SpouseName = value;
                }
            }
        }

        [Column(Name = "Count", Storage = "_Count", DbType = "int")]
        public int? Count
        {
            get => _Count;

            set
            {
                if (_Count != value)
                {
                    _Count = value;
                }
            }
        }

        [Column(Name = "Amount", Storage = "_Amount", DbType = "Decimal(38,2)")]
        public decimal? Amount
        {
            get => _Amount;

            set
            {
                if (_Amount != value)
                {
                    _Amount = value;
                }
            }
        }

        [Column(Name = "PledgeAmount", Storage = "_PledgeAmount", DbType = "Decimal(38,2)")]
        public decimal? PledgeAmount
        {
            get => _PledgeAmount;

            set
            {
                if (_PledgeAmount != value)
                {
                    _PledgeAmount = value;
                }
            }
        }

        [Column(Name = "FundId", Storage = "_FundId", DbType = "int NOT NULL")]
        public int FundId
        {
            get => _FundId;

            set
            {
                if (_FundId != value)
                {
                    _FundId = value;
                }
            }
        }

        [Column(Name = "FundName", Storage = "_FundName", DbType = "nvarchar(256) NOT NULL")]
        public string FundName
        {
            get => _FundName;

            set
            {
                if (_FundName != value)
                {
                    _FundName = value;
                }
            }
        }

        [Column(Name = "ContributionStatusId", Storage = "_ContributionStatusId", DbType = "int NOT NULL")]
        public int ContributionStatusId
        {
            get => _ContributionStatusId;

            set
            {
                if (_ContributionStatusId != value)
                {
                    _ContributionStatusId = value;
                }
            }
        }

        [Column(Name = "ContributionTypeId", Storage = "_ContributionTypeId", DbType = "int NOT NULL")]
        public int ContributionTypeId
        {
            get => _ContributionTypeId;

            set
            {
                if (_ContributionTypeId != value)
                {
                    _ContributionTypeId = value;
                }
            }
        }

        [Column(Name = "MainFellowship", Storage = "_MainFellowship", DbType = "nvarchar(100)")]
        public string MainFellowship
        {
            get => _MainFellowship;

            set
            {
                if (_MainFellowship != value)
                {
                    _MainFellowship = value;
                }
            }
        }

        [Column(Name = "MemberStatus", Storage = "_MemberStatus", DbType = "nvarchar(50)")]
        public string MemberStatus
        {
            get => _MemberStatus;

            set
            {
                if (_MemberStatus != value)
                {
                    _MemberStatus = value;
                }
            }
        }

        [Column(Name = "JoinDate", Storage = "_JoinDate", DbType = "datetime")]
        public DateTime? JoinDate
        {
            get => _JoinDate;

            set
            {
                if (_JoinDate != value)
                {
                    _JoinDate = value;
                }
            }
        }
    }
}
