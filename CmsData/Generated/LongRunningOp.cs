using System; 
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;

namespace CmsData
{
	[Table(Name="dbo.LongRunningOp")]
	public partial class LongRunningOp : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private string _Operation;
		
		private DateTime? _Started;
		
		private int? _Count;
		
		private int? _Processed;
		
		private DateTime? _Completed;
		
		private string _ElapsedTime;
		
   		
    	
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnOperationChanging(string value);
		partial void OnOperationChanged();
		
		partial void OnStartedChanging(DateTime? value);
		partial void OnStartedChanged();
		
		partial void OnCountChanging(int? value);
		partial void OnCountChanged();
		
		partial void OnProcessedChanging(int? value);
		partial void OnProcessedChanged();
		
		partial void OnCompletedChanging(DateTime? value);
		partial void OnCompletedChanged();
		
		partial void OnElapsedTimeChanging(string value);
		partial void OnElapsedTimeChanged();
		
    #endregion
		public LongRunningOp()
		{
			
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="id", UpdateCheck=UpdateCheck.Never, Storage="_Id", DbType="int NOT NULL", IsPrimaryKey=true)]
		public int Id
		{
			get { return this._Id; }

			set
			{
				if (this._Id != value)
				{
				
                    this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}

			}

		}

		
		[Column(Name="operation", UpdateCheck=UpdateCheck.Never, Storage="_Operation", DbType="nvarchar(25) NOT NULL", IsPrimaryKey=true)]
		public string Operation
		{
			get { return this._Operation; }

			set
			{
				if (this._Operation != value)
				{
				
                    this.OnOperationChanging(value);
					this.SendPropertyChanging();
					this._Operation = value;
					this.SendPropertyChanged("Operation");
					this.OnOperationChanged();
				}

			}

		}

		
		[Column(Name="started", UpdateCheck=UpdateCheck.Never, Storage="_Started", DbType="datetime")]
		public DateTime? Started
		{
			get { return this._Started; }

			set
			{
				if (this._Started != value)
				{
				
                    this.OnStartedChanging(value);
					this.SendPropertyChanging();
					this._Started = value;
					this.SendPropertyChanged("Started");
					this.OnStartedChanged();
				}

			}

		}

		
		[Column(Name="count", UpdateCheck=UpdateCheck.Never, Storage="_Count", DbType="int")]
		public int? Count
		{
			get { return this._Count; }

			set
			{
				if (this._Count != value)
				{
				
                    this.OnCountChanging(value);
					this.SendPropertyChanging();
					this._Count = value;
					this.SendPropertyChanged("Count");
					this.OnCountChanged();
				}

			}

		}

		
		[Column(Name="processed", UpdateCheck=UpdateCheck.Never, Storage="_Processed", DbType="int")]
		public int? Processed
		{
			get { return this._Processed; }

			set
			{
				if (this._Processed != value)
				{
				
                    this.OnProcessedChanging(value);
					this.SendPropertyChanging();
					this._Processed = value;
					this.SendPropertyChanged("Processed");
					this.OnProcessedChanged();
				}

			}

		}

		
		[Column(Name="completed", UpdateCheck=UpdateCheck.Never, Storage="_Completed", DbType="datetime")]
		public DateTime? Completed
		{
			get { return this._Completed; }

			set
			{
				if (this._Completed != value)
				{
				
                    this.OnCompletedChanging(value);
					this.SendPropertyChanging();
					this._Completed = value;
					this.SendPropertyChanged("Completed");
					this.OnCompletedChanged();
				}

			}

		}

		
		[Column(Name="ElapsedTime", UpdateCheck=UpdateCheck.Never, Storage="_ElapsedTime", DbType="varchar(20)", IsDbGenerated=true)]
		public string ElapsedTime
		{
			get { return this._ElapsedTime; }

			set
			{
				if (this._ElapsedTime != value)
				{
				
                    this.OnElapsedTimeChanging(value);
					this.SendPropertyChanging();
					this._ElapsedTime = value;
					this.SendPropertyChanged("ElapsedTime");
					this.OnElapsedTimeChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
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

