﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.237
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QAUtil.Controllers
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="ChmContribution")]
	public partial class PaymentTypesDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertPP_CHURCH_TYPE_PROCESSOR(PP_CHURCH_TYPE_PROCESSOR instance);
    partial void UpdatePP_CHURCH_TYPE_PROCESSOR(PP_CHURCH_TYPE_PROCESSOR instance);
    partial void DeletePP_CHURCH_TYPE_PROCESSOR(PP_CHURCH_TYPE_PROCESSOR instance);
    partial void InsertPP_TYPE(PP_TYPE instance);
    partial void UpdatePP_TYPE(PP_TYPE instance);
    partial void DeletePP_TYPE(PP_TYPE instance);
    #endregion
		
		public PaymentTypesDataContext() : 
				base(global::System.Configuration.ConfigurationManager.ConnectionStrings["ChmContributionConnectionString"].ConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public PaymentTypesDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public PaymentTypesDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public PaymentTypesDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public PaymentTypesDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<PP_CHURCH_TYPE_PROCESSOR> PP_CHURCH_TYPE_PROCESSORs
		{
			get
			{
				return this.GetTable<PP_CHURCH_TYPE_PROCESSOR>();
			}
		}
		
		public System.Data.Linq.Table<PP_TYPE> PP_TYPEs
		{
			get
			{
				return this.GetTable<PP_TYPE>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.PP_CHURCH_TYPE_PROCESSOR")]
	public partial class PP_CHURCH_TYPE_PROCESSOR : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _PP_TYPE_ID;
		
		private int _CHURCH_ID;
		
		private System.Nullable<System.DateTime> _CREATED_DATE;
		
		private string _CREATED_BY_LOGIN;
		
		private System.Nullable<int> _CREATED_BY_USER_ID;
		
		private System.Nullable<System.DateTime> _LAST_UPDATED_DATE;
		
		private string _LAST_UPDATED_BY_LOGIN;
		
		private System.Nullable<int> _LAST_UPDATED_BY_USER_ID;
		
		private int _FEATURE_ID;
		
		private System.Nullable<int> _PP_Merchant_Provider_ID;
		
		private EntityRef<PP_TYPE> _PP_TYPE;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnPP_TYPE_IDChanging(int value);
    partial void OnPP_TYPE_IDChanged();
    partial void OnCHURCH_IDChanging(int value);
    partial void OnCHURCH_IDChanged();
    partial void OnCREATED_DATEChanging(System.Nullable<System.DateTime> value);
    partial void OnCREATED_DATEChanged();
    partial void OnCREATED_BY_LOGINChanging(string value);
    partial void OnCREATED_BY_LOGINChanged();
    partial void OnCREATED_BY_USER_IDChanging(System.Nullable<int> value);
    partial void OnCREATED_BY_USER_IDChanged();
    partial void OnLAST_UPDATED_DATEChanging(System.Nullable<System.DateTime> value);
    partial void OnLAST_UPDATED_DATEChanged();
    partial void OnLAST_UPDATED_BY_LOGINChanging(string value);
    partial void OnLAST_UPDATED_BY_LOGINChanged();
    partial void OnLAST_UPDATED_BY_USER_IDChanging(System.Nullable<int> value);
    partial void OnLAST_UPDATED_BY_USER_IDChanged();
    partial void OnFEATURE_IDChanging(int value);
    partial void OnFEATURE_IDChanged();
    partial void OnPP_Merchant_Provider_IDChanging(System.Nullable<int> value);
    partial void OnPP_Merchant_Provider_IDChanged();
    #endregion
		
		public PP_CHURCH_TYPE_PROCESSOR()
		{
			this._PP_TYPE = default(EntityRef<PP_TYPE>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PP_TYPE_ID", DbType="Int NOT NULL", IsPrimaryKey=true)]
		public int PP_TYPE_ID
		{
			get
			{
				return this._PP_TYPE_ID;
			}
			set
			{
				if ((this._PP_TYPE_ID != value))
				{
					if (this._PP_TYPE.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnPP_TYPE_IDChanging(value);
					this.SendPropertyChanging();
					this._PP_TYPE_ID = value;
					this.SendPropertyChanged("PP_TYPE_ID");
					this.OnPP_TYPE_IDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CHURCH_ID", DbType="Int NOT NULL", IsPrimaryKey=true)]
		public int CHURCH_ID
		{
			get
			{
				return this._CHURCH_ID;
			}
			set
			{
				if ((this._CHURCH_ID != value))
				{
					this.OnCHURCH_IDChanging(value);
					this.SendPropertyChanging();
					this._CHURCH_ID = value;
					this.SendPropertyChanged("CHURCH_ID");
					this.OnCHURCH_IDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CREATED_DATE", DbType="DateTime")]
		public System.Nullable<System.DateTime> CREATED_DATE
		{
			get
			{
				return this._CREATED_DATE;
			}
			set
			{
				if ((this._CREATED_DATE != value))
				{
					this.OnCREATED_DATEChanging(value);
					this.SendPropertyChanging();
					this._CREATED_DATE = value;
					this.SendPropertyChanged("CREATED_DATE");
					this.OnCREATED_DATEChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CREATED_BY_LOGIN", DbType="VarChar(20)")]
		public string CREATED_BY_LOGIN
		{
			get
			{
				return this._CREATED_BY_LOGIN;
			}
			set
			{
				if ((this._CREATED_BY_LOGIN != value))
				{
					this.OnCREATED_BY_LOGINChanging(value);
					this.SendPropertyChanging();
					this._CREATED_BY_LOGIN = value;
					this.SendPropertyChanged("CREATED_BY_LOGIN");
					this.OnCREATED_BY_LOGINChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CREATED_BY_USER_ID", DbType="Int")]
		public System.Nullable<int> CREATED_BY_USER_ID
		{
			get
			{
				return this._CREATED_BY_USER_ID;
			}
			set
			{
				if ((this._CREATED_BY_USER_ID != value))
				{
					this.OnCREATED_BY_USER_IDChanging(value);
					this.SendPropertyChanging();
					this._CREATED_BY_USER_ID = value;
					this.SendPropertyChanged("CREATED_BY_USER_ID");
					this.OnCREATED_BY_USER_IDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LAST_UPDATED_DATE", DbType="DateTime")]
		public System.Nullable<System.DateTime> LAST_UPDATED_DATE
		{
			get
			{
				return this._LAST_UPDATED_DATE;
			}
			set
			{
				if ((this._LAST_UPDATED_DATE != value))
				{
					this.OnLAST_UPDATED_DATEChanging(value);
					this.SendPropertyChanging();
					this._LAST_UPDATED_DATE = value;
					this.SendPropertyChanged("LAST_UPDATED_DATE");
					this.OnLAST_UPDATED_DATEChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LAST_UPDATED_BY_LOGIN", DbType="VarChar(20)")]
		public string LAST_UPDATED_BY_LOGIN
		{
			get
			{
				return this._LAST_UPDATED_BY_LOGIN;
			}
			set
			{
				if ((this._LAST_UPDATED_BY_LOGIN != value))
				{
					this.OnLAST_UPDATED_BY_LOGINChanging(value);
					this.SendPropertyChanging();
					this._LAST_UPDATED_BY_LOGIN = value;
					this.SendPropertyChanged("LAST_UPDATED_BY_LOGIN");
					this.OnLAST_UPDATED_BY_LOGINChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LAST_UPDATED_BY_USER_ID", DbType="Int")]
		public System.Nullable<int> LAST_UPDATED_BY_USER_ID
		{
			get
			{
				return this._LAST_UPDATED_BY_USER_ID;
			}
			set
			{
				if ((this._LAST_UPDATED_BY_USER_ID != value))
				{
					this.OnLAST_UPDATED_BY_USER_IDChanging(value);
					this.SendPropertyChanging();
					this._LAST_UPDATED_BY_USER_ID = value;
					this.SendPropertyChanged("LAST_UPDATED_BY_USER_ID");
					this.OnLAST_UPDATED_BY_USER_IDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FEATURE_ID", DbType="Int NOT NULL", IsPrimaryKey=true)]
		public int FEATURE_ID
		{
			get
			{
				return this._FEATURE_ID;
			}
			set
			{
				if ((this._FEATURE_ID != value))
				{
					this.OnFEATURE_IDChanging(value);
					this.SendPropertyChanging();
					this._FEATURE_ID = value;
					this.SendPropertyChanged("FEATURE_ID");
					this.OnFEATURE_IDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PP_Merchant_Provider_ID", DbType="Int")]
		public System.Nullable<int> PP_Merchant_Provider_ID
		{
			get
			{
				return this._PP_Merchant_Provider_ID;
			}
			set
			{
				if ((this._PP_Merchant_Provider_ID != value))
				{
					this.OnPP_Merchant_Provider_IDChanging(value);
					this.SendPropertyChanging();
					this._PP_Merchant_Provider_ID = value;
					this.SendPropertyChanged("PP_Merchant_Provider_ID");
					this.OnPP_Merchant_Provider_IDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="PP_TYPE_PP_CHURCH_TYPE_PROCESSOR", Storage="_PP_TYPE", ThisKey="PP_TYPE_ID", OtherKey="PP_TYPE_ID", IsForeignKey=true)]
		public PP_TYPE PP_TYPE
		{
			get
			{
				return this._PP_TYPE.Entity;
			}
			set
			{
				PP_TYPE previousValue = this._PP_TYPE.Entity;
				if (((previousValue != value) 
							|| (this._PP_TYPE.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._PP_TYPE.Entity = null;
						previousValue.PP_CHURCH_TYPE_PROCESSORs.Remove(this);
					}
					this._PP_TYPE.Entity = value;
					if ((value != null))
					{
						value.PP_CHURCH_TYPE_PROCESSORs.Add(this);
						this._PP_TYPE_ID = value.PP_TYPE_ID;
					}
					else
					{
						this._PP_TYPE_ID = default(int);
					}
					this.SendPropertyChanged("PP_TYPE");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.PP_TYPE")]
	public partial class PP_TYPE : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _PP_TYPE_ID;
		
		private string _PP_TYPE_NAME;
		
		private string _TYPE_CODE;
		
		private System.DateTime _DATE_ENTERED;
		
		private System.Nullable<int> _PP_PROCESSOR_ID;
		
		private System.Nullable<System.DateTime> _CREATED_DATE;
		
		private string _CREATED_BY_LOGIN;
		
		private System.Nullable<int> _CREATED_BY_USER_ID;
		
		private System.Nullable<System.DateTime> _LAST_UPDATED_DATE;
		
		private string _LAST_UPDATED_BY_LOGIN;
		
		private System.Nullable<int> _LAST_UPDATED_BY_USER_ID;
		
		private EntitySet<PP_CHURCH_TYPE_PROCESSOR> _PP_CHURCH_TYPE_PROCESSORs;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnPP_TYPE_IDChanging(int value);
    partial void OnPP_TYPE_IDChanged();
    partial void OnPP_TYPE_NAMEChanging(string value);
    partial void OnPP_TYPE_NAMEChanged();
    partial void OnTYPE_CODEChanging(string value);
    partial void OnTYPE_CODEChanged();
    partial void OnDATE_ENTEREDChanging(System.DateTime value);
    partial void OnDATE_ENTEREDChanged();
    partial void OnPP_PROCESSOR_IDChanging(System.Nullable<int> value);
    partial void OnPP_PROCESSOR_IDChanged();
    partial void OnCREATED_DATEChanging(System.Nullable<System.DateTime> value);
    partial void OnCREATED_DATEChanged();
    partial void OnCREATED_BY_LOGINChanging(string value);
    partial void OnCREATED_BY_LOGINChanged();
    partial void OnCREATED_BY_USER_IDChanging(System.Nullable<int> value);
    partial void OnCREATED_BY_USER_IDChanged();
    partial void OnLAST_UPDATED_DATEChanging(System.Nullable<System.DateTime> value);
    partial void OnLAST_UPDATED_DATEChanged();
    partial void OnLAST_UPDATED_BY_LOGINChanging(string value);
    partial void OnLAST_UPDATED_BY_LOGINChanged();
    partial void OnLAST_UPDATED_BY_USER_IDChanging(System.Nullable<int> value);
    partial void OnLAST_UPDATED_BY_USER_IDChanged();
    #endregion
		
		public PP_TYPE()
		{
			this._PP_CHURCH_TYPE_PROCESSORs = new EntitySet<PP_CHURCH_TYPE_PROCESSOR>(new Action<PP_CHURCH_TYPE_PROCESSOR>(this.attach_PP_CHURCH_TYPE_PROCESSORs), new Action<PP_CHURCH_TYPE_PROCESSOR>(this.detach_PP_CHURCH_TYPE_PROCESSORs));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PP_TYPE_ID", DbType="Int NOT NULL", IsPrimaryKey=true)]
		public int PP_TYPE_ID
		{
			get
			{
				return this._PP_TYPE_ID;
			}
			set
			{
				if ((this._PP_TYPE_ID != value))
				{
					this.OnPP_TYPE_IDChanging(value);
					this.SendPropertyChanging();
					this._PP_TYPE_ID = value;
					this.SendPropertyChanged("PP_TYPE_ID");
					this.OnPP_TYPE_IDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PP_TYPE_NAME", DbType="VarChar(30) NOT NULL", CanBeNull=false)]
		public string PP_TYPE_NAME
		{
			get
			{
				return this._PP_TYPE_NAME;
			}
			set
			{
				if ((this._PP_TYPE_NAME != value))
				{
					this.OnPP_TYPE_NAMEChanging(value);
					this.SendPropertyChanging();
					this._PP_TYPE_NAME = value;
					this.SendPropertyChanged("PP_TYPE_NAME");
					this.OnPP_TYPE_NAMEChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TYPE_CODE", DbType="Char(3) NOT NULL", CanBeNull=false)]
		public string TYPE_CODE
		{
			get
			{
				return this._TYPE_CODE;
			}
			set
			{
				if ((this._TYPE_CODE != value))
				{
					this.OnTYPE_CODEChanging(value);
					this.SendPropertyChanging();
					this._TYPE_CODE = value;
					this.SendPropertyChanged("TYPE_CODE");
					this.OnTYPE_CODEChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DATE_ENTERED", DbType="DateTime NOT NULL")]
		public System.DateTime DATE_ENTERED
		{
			get
			{
				return this._DATE_ENTERED;
			}
			set
			{
				if ((this._DATE_ENTERED != value))
				{
					this.OnDATE_ENTEREDChanging(value);
					this.SendPropertyChanging();
					this._DATE_ENTERED = value;
					this.SendPropertyChanged("DATE_ENTERED");
					this.OnDATE_ENTEREDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PP_PROCESSOR_ID", DbType="Int")]
		public System.Nullable<int> PP_PROCESSOR_ID
		{
			get
			{
				return this._PP_PROCESSOR_ID;
			}
			set
			{
				if ((this._PP_PROCESSOR_ID != value))
				{
					this.OnPP_PROCESSOR_IDChanging(value);
					this.SendPropertyChanging();
					this._PP_PROCESSOR_ID = value;
					this.SendPropertyChanged("PP_PROCESSOR_ID");
					this.OnPP_PROCESSOR_IDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CREATED_DATE", DbType="DateTime")]
		public System.Nullable<System.DateTime> CREATED_DATE
		{
			get
			{
				return this._CREATED_DATE;
			}
			set
			{
				if ((this._CREATED_DATE != value))
				{
					this.OnCREATED_DATEChanging(value);
					this.SendPropertyChanging();
					this._CREATED_DATE = value;
					this.SendPropertyChanged("CREATED_DATE");
					this.OnCREATED_DATEChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CREATED_BY_LOGIN", DbType="VarChar(20)")]
		public string CREATED_BY_LOGIN
		{
			get
			{
				return this._CREATED_BY_LOGIN;
			}
			set
			{
				if ((this._CREATED_BY_LOGIN != value))
				{
					this.OnCREATED_BY_LOGINChanging(value);
					this.SendPropertyChanging();
					this._CREATED_BY_LOGIN = value;
					this.SendPropertyChanged("CREATED_BY_LOGIN");
					this.OnCREATED_BY_LOGINChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CREATED_BY_USER_ID", DbType="Int")]
		public System.Nullable<int> CREATED_BY_USER_ID
		{
			get
			{
				return this._CREATED_BY_USER_ID;
			}
			set
			{
				if ((this._CREATED_BY_USER_ID != value))
				{
					this.OnCREATED_BY_USER_IDChanging(value);
					this.SendPropertyChanging();
					this._CREATED_BY_USER_ID = value;
					this.SendPropertyChanged("CREATED_BY_USER_ID");
					this.OnCREATED_BY_USER_IDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LAST_UPDATED_DATE", DbType="DateTime")]
		public System.Nullable<System.DateTime> LAST_UPDATED_DATE
		{
			get
			{
				return this._LAST_UPDATED_DATE;
			}
			set
			{
				if ((this._LAST_UPDATED_DATE != value))
				{
					this.OnLAST_UPDATED_DATEChanging(value);
					this.SendPropertyChanging();
					this._LAST_UPDATED_DATE = value;
					this.SendPropertyChanged("LAST_UPDATED_DATE");
					this.OnLAST_UPDATED_DATEChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LAST_UPDATED_BY_LOGIN", DbType="VarChar(20)")]
		public string LAST_UPDATED_BY_LOGIN
		{
			get
			{
				return this._LAST_UPDATED_BY_LOGIN;
			}
			set
			{
				if ((this._LAST_UPDATED_BY_LOGIN != value))
				{
					this.OnLAST_UPDATED_BY_LOGINChanging(value);
					this.SendPropertyChanging();
					this._LAST_UPDATED_BY_LOGIN = value;
					this.SendPropertyChanged("LAST_UPDATED_BY_LOGIN");
					this.OnLAST_UPDATED_BY_LOGINChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LAST_UPDATED_BY_USER_ID", DbType="Int")]
		public System.Nullable<int> LAST_UPDATED_BY_USER_ID
		{
			get
			{
				return this._LAST_UPDATED_BY_USER_ID;
			}
			set
			{
				if ((this._LAST_UPDATED_BY_USER_ID != value))
				{
					this.OnLAST_UPDATED_BY_USER_IDChanging(value);
					this.SendPropertyChanging();
					this._LAST_UPDATED_BY_USER_ID = value;
					this.SendPropertyChanged("LAST_UPDATED_BY_USER_ID");
					this.OnLAST_UPDATED_BY_USER_IDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="PP_TYPE_PP_CHURCH_TYPE_PROCESSOR", Storage="_PP_CHURCH_TYPE_PROCESSORs", ThisKey="PP_TYPE_ID", OtherKey="PP_TYPE_ID")]
		public EntitySet<PP_CHURCH_TYPE_PROCESSOR> PP_CHURCH_TYPE_PROCESSORs
		{
			get
			{
				return this._PP_CHURCH_TYPE_PROCESSORs;
			}
			set
			{
				this._PP_CHURCH_TYPE_PROCESSORs.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_PP_CHURCH_TYPE_PROCESSORs(PP_CHURCH_TYPE_PROCESSOR entity)
		{
			this.SendPropertyChanging();
			entity.PP_TYPE = this;
		}
		
		private void detach_PP_CHURCH_TYPE_PROCESSORs(PP_CHURCH_TYPE_PROCESSOR entity)
		{
			this.SendPropertyChanging();
			entity.PP_TYPE = null;
		}
	}
}
#pragma warning restore 1591
