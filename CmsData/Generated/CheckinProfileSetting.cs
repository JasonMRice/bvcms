using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;
using CmsData.Infrastructure;

namespace CmsData
{
    [Table(Name = "dbo.CheckinProfileSettings")]
    public partial class CheckinProfileSetting : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        #region Private Fields

        private int _CheckinProfileId;

        private int? _CampusId;

        private bool _Testing;

        private int? _TestDay;

        private string _AdminPIN;

        private int? _PINTimeout;

        private bool _DisableJoin;

        private bool _DisableTimer;

        private int? _BackgroundImage;

        private string _BackgroundImageName;

        private string _BackgroundImageURL;

        private int _CutoffAge;

        private string _Logout;

        private bool _Guest;

        private bool _Location;

        private int _SecurityType;

        private int _ShowCheckinConfirmation;



        private EntityRef<CheckinProfile> _CheckinProfile;

        #endregion

        #region Extensibility Method Definitions
        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnCheckinProfileIdChanging(int value);
        partial void OnCheckinProfileIdChanged();

        partial void OnCampusIdChanging(int? value);
        partial void OnCampusIdChanged();

        partial void OnTestingChanging(bool value);
        partial void OnTestingChanged();

        partial void OnTestDayChanging(int? value);
        partial void OnTestDayChanged();

        partial void OnAdminPINChanging(string value);
        partial void OnAdminPINChanged();

        partial void OnPINTimeoutChanging(int? value);
        partial void OnPINTimeoutChanged();

        partial void OnDisableJoinChanging(bool value);
        partial void OnDisableJoinChanged();

        partial void OnDisableTimerChanging(bool value);
        partial void OnDisableTimerChanged();

        partial void OnBackgroundImageChanging(int? value);
        partial void OnBackgroundImageChanged();

        partial void OnBackgroundImageNameChanging(string value);
        partial void OnBackgroundImageNameChanged();

        partial void OnBackgroundImageURLChanging(string value);
        partial void OnBackgroundImageURLChanged();

        partial void OnCutoffAgeChanging(int value);
        partial void OnCutoffAgeChanged();

        partial void OnLogoutChanging(string value);
        partial void OnLogoutChanged();

        partial void OnGuestChanging(bool value);
        partial void OnGuestChanged();

        partial void OnLocationChanging(bool value);
        partial void OnLocationChanged();

        partial void OnSecurityTypeChanging(int value);
        partial void OnSecurityTypeChanged();

        partial void OnShowCheckinConfirmationChanging(int value);
        partial void OnShowCheckinConfirmationChanged();

        #endregion
        public CheckinProfileSetting()
        {


            this._CheckinProfile = default(EntityRef<CheckinProfile>);

            OnCreated();
        }


        #region Columns
        
        [Column(Name = "CheckinProfileId", UpdateCheck = UpdateCheck.Never, Storage = "_CheckinProfileId", DbType = "int NOT NULL UNIQUE", IsPrimaryKey = true)]
        [IsForeignKey]
        public int CheckinProfileId
        {
            get { return this._CheckinProfileId; }

            set
            {
                if (this._CheckinProfileId != value)
                {

                    if (this._CheckinProfile.HasLoadedOrAssignedValue)
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();

                    this.OnCheckinProfileIdChanging(value);
                    this.SendPropertyChanging();
                    this._CheckinProfileId = value;
                    this.SendPropertyChanged("CheckinProfileId");
                    this.OnCheckinProfileIdChanged();
                }

            }

        }


        [Column(Name = "CampusId", UpdateCheck = UpdateCheck.Never, Storage = "_CampusId", DbType = "int")]
        public int? CampusId
        {
            get { return this._CampusId; }

            set
            {
                if (this._CampusId != value)
                {

                    this.OnCampusIdChanging(value);
                    this.SendPropertyChanging();
                    this._CampusId = value;
                    this.SendPropertyChanged("CampusId");
                    this.OnCampusIdChanged();
                }

            }

        }


        [Column(Name = "Testing", UpdateCheck = UpdateCheck.Never, Storage = "_Testing", DbType = "bit NOT NULL")]
        public bool Testing
        {
            get { return this._Testing; }

            set
            {
                if (this._Testing != value)
                {

                    this.OnTestingChanging(value);
                    this.SendPropertyChanging();
                    this._Testing = value;
                    this.SendPropertyChanged("Testing");
                    this.OnTestingChanged();
                }

            }

        }


        [Column(Name = "TestDay", UpdateCheck = UpdateCheck.Never, Storage = "_TestDay", DbType = "int")]
        public int? TestDay
        {
            get { return this._TestDay; }

            set
            {
                if (this._TestDay != value)
                {

                    this.OnTestDayChanging(value);
                    this.SendPropertyChanging();
                    this._TestDay = value;
                    this.SendPropertyChanged("TestDay");
                    this.OnTestDayChanged();
                }

            }

        }


        [Column(Name = "AdminPIN", UpdateCheck = UpdateCheck.Never, Storage = "_AdminPIN", DbType = "nvarchar(10)")]
        public string AdminPIN
        {
            get { return this._AdminPIN; }

            set
            {
                if (this._AdminPIN != value)
                {

                    this.OnAdminPINChanging(value);
                    this.SendPropertyChanging();
                    this._AdminPIN = value;
                    this.SendPropertyChanged("AdminPIN");
                    this.OnAdminPINChanged();
                }

            }

        }


        [Column(Name = "PINTimeout", UpdateCheck = UpdateCheck.Never, Storage = "_PINTimeout", DbType = "int")]
        public int? PINTimeout
        {
            get { return this._PINTimeout; }

            set
            {
                if (this._PINTimeout != value)
                {

                    this.OnPINTimeoutChanging(value);
                    this.SendPropertyChanging();
                    this._PINTimeout = value;
                    this.SendPropertyChanged("PINTimeout");
                    this.OnPINTimeoutChanged();
                }

            }

        }


        [Column(Name = "DisableJoin", UpdateCheck = UpdateCheck.Never, Storage = "_DisableJoin", DbType = "bit NOT NULL")]
        public bool DisableJoin
        {
            get { return this._DisableJoin; }

            set
            {
                if (this._DisableJoin != value)
                {

                    this.OnDisableJoinChanging(value);
                    this.SendPropertyChanging();
                    this._DisableJoin = value;
                    this.SendPropertyChanged("DisableJoin");
                    this.OnDisableJoinChanged();
                }

            }

        }


        [Column(Name = "DisableTimer", UpdateCheck = UpdateCheck.Never, Storage = "_DisableTimer", DbType = "bit NOT NULL")]
        public bool DisableTimer
        {
            get { return this._DisableTimer; }

            set
            {
                if (this._DisableTimer != value)
                {

                    this.OnDisableTimerChanging(value);
                    this.SendPropertyChanging();
                    this._DisableTimer = value;
                    this.SendPropertyChanged("DisableTimer");
                    this.OnDisableTimerChanged();
                }

            }

        }


        [Column(Name = "BackgroundImage", UpdateCheck = UpdateCheck.Never, Storage = "_BackgroundImage", DbType = "int")]
        public int? BackgroundImage
        {
            get { return this._BackgroundImage; }

            set
            {
                if (this._BackgroundImage != value)
                {

                    this.OnBackgroundImageChanging(value);
                    this.SendPropertyChanging();
                    this._BackgroundImage = value;
                    this.SendPropertyChanged("BackgroundImage");
                    this.OnBackgroundImageChanged();
                }

            }

        }


        [Column(Name = "BackgroundImageName", UpdateCheck = UpdateCheck.Never, Storage = "_BackgroundImageName", DbType = "nvarchar")]
        public string BackgroundImageName
        {
            get { return this._BackgroundImageName; }

            set
            {
                if (this._BackgroundImageName != value)
                {

                    this.OnBackgroundImageNameChanging(value);
                    this.SendPropertyChanging();
                    this._BackgroundImageName = value;
                    this.SendPropertyChanged("BackgroundImageName");
                    this.OnBackgroundImageNameChanged();
                }

            }

        }


        [Column(Name = "BackgroundImageURL", UpdateCheck = UpdateCheck.Never, Storage = "_BackgroundImageURL", DbType = "nvarchar")]
        public string BackgroundImageURL
        {
            get { return this._BackgroundImageURL; }

            set
            {
                if (this._BackgroundImageURL != value)
                {

                    this.OnBackgroundImageURLChanging(value);
                    this.SendPropertyChanging();
                    this._BackgroundImageURL = value;
                    this.SendPropertyChanged("BackgroundImageURL");
                    this.OnBackgroundImageURLChanged();
                }

            }

        }


        [Column(Name = "CutoffAge", UpdateCheck = UpdateCheck.Never, Storage = "_CutoffAge", DbType = "int NOT NULL")]
        public int CutoffAge
        {
            get { return this._CutoffAge; }

            set
            {
                if (this._CutoffAge != value)
                {

                    this.OnCutoffAgeChanging(value);
                    this.SendPropertyChanging();
                    this._CutoffAge = value;
                    this.SendPropertyChanged("CutoffAge");
                    this.OnCutoffAgeChanged();
                }

            }

        }


        [Column(Name = "Logout", UpdateCheck = UpdateCheck.Never, Storage = "_Logout", DbType = "nvarchar(10)")]
        public string Logout
        {
            get { return this._Logout; }

            set
            {
                if (this._Logout != value)
                {

                    this.OnLogoutChanging(value);
                    this.SendPropertyChanging();
                    this._Logout = value;
                    this.SendPropertyChanged("Logout");
                    this.OnLogoutChanged();
                }

            }

        }


        [Column(Name = "Guest", UpdateCheck = UpdateCheck.Never, Storage = "_Guest", DbType = "bit NOT NULL")]
        public bool Guest
        {
            get { return this._Guest; }

            set
            {
                if (this._Guest != value)
                {

                    this.OnGuestChanging(value);
                    this.SendPropertyChanging();
                    this._Guest = value;
                    this.SendPropertyChanged("Guest");
                    this.OnGuestChanged();
                }

            }

        }


        [Column(Name = "Location", UpdateCheck = UpdateCheck.Never, Storage = "_Location", DbType = "bit NOT NULL")]
        public bool Location
        {
            get { return this._Location; }

            set
            {
                if (this._Location != value)
                {

                    this.OnLocationChanging(value);
                    this.SendPropertyChanging();
                    this._Location = value;
                    this.SendPropertyChanged("Location");
                    this.OnLocationChanged();
                }

            }

        }


        [Column(Name = "SecurityType", UpdateCheck = UpdateCheck.Never, Storage = "_SecurityType", DbType = "int NOT NULL")]
        public int SecurityType
        {
            get { return this._SecurityType; }

            set
            {
                if (this._SecurityType != value)
                {

                    this.OnSecurityTypeChanging(value);
                    this.SendPropertyChanging();
                    this._SecurityType = value;
                    this.SendPropertyChanged("SecurityType");
                    this.OnSecurityTypeChanged();
                }

            }

        }


        [Column(Name = "ShowCheckinConfirmation", UpdateCheck = UpdateCheck.Never, Storage = "_ShowCheckinConfirmation", DbType = "int NOT NULL")]
        public int ShowCheckinConfirmation
        {
            get { return this._ShowCheckinConfirmation; }

            set
            {
                if (this._ShowCheckinConfirmation != value)
                {

                    this.OnShowCheckinConfirmationChanging(value);
                    this.SendPropertyChanging();
                    this._ShowCheckinConfirmation = value;
                    this.SendPropertyChanged("ShowCheckinConfirmation");
                    this.OnShowCheckinConfirmationChanged();
                }

            }

        }


        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK__CheckinPr__Check__37DB1BFD", Storage = "_CheckinProfile", ThisKey = "CheckinProfileId", IsForeignKey = true)]
        public CheckinProfile CheckinProfile
        {
            get { return this._CheckinProfile.Entity; }

            set
            {
                CheckinProfile previousValue = this._CheckinProfile.Entity;
                if (((previousValue != value)
                            || (this._CheckinProfile.HasLoadedOrAssignedValue == false)))
                {
                    this.SendPropertyChanging();
                    if (previousValue != null)
                    {
                        this._CheckinProfile.Entity = null;
                        previousValue.CheckinProfileSettings.Remove(this);
                    }

                    this._CheckinProfile.Entity = value;
                    if (value != null)
                    {
                        value.CheckinProfileSettings.Add(this);

                        this._CheckinProfileId = value.CheckinProfileId;

                    }

                    else
                    {

                        this._CheckinProfileId = default(int);

                    }

                    this.SendPropertyChanged("CheckinProfile");
                }

            }

        }


        #endregion

        public event PropertyChangingEventHandler PropertyChanging;
        protected virtual void SendPropertyChanging()
        {
            if ((this.PropertyChanging != null))
                this.PropertyChanging(this, emptyChangingEventArgs);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(String propertyName)
        {
            if ((this.PropertyChanged != null))
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


    }

}

