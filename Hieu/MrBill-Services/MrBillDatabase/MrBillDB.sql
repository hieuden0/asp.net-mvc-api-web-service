use master
drop database company

create database company on primary
(
	name ='company',
	filename ='E:\data\company.mdf',
	size =3072kb,
	maxsize= unlimited,
	filegrowth= 1024kb	
)
log on
(
	name ='company_log',
	filename ='E:\data\company_log.ldf',
	size =1024kb,
	maxsize = 2024kb,
	filegrowth=10%
)
go

use MrBillDB

create table UserProfile 
(
	 UserID int identity(1,1) not null,
	 UserName nvarchar(50) unique not null,
	 Password nvarchar(50) not null,
	 FirstName nvarchar(50) not null,
	 LastName nvarchar(50) not null,
	 Address nvarchar(100) not null,
	 City nvarchar(50) not null,
	 PostCode nvarchar(50) not null,
	 Phone nvarchar(50) not null,
	 Status int not null,
	 CompanyID int not null,
	 UserRoleID int not null
)
go




create table Email 
(
	EmailID int identity(1,1) not null,
	Address nvarchar(100) not null,
	Name nvarchar(50) not null,
	UserID int not null
)
go

create table TransactionGroup
(
	TransactionGroupID int identity(1,1) not null,
	Name nvarchar(50) not null
)
go

create table PaymentType
(
	PaymentID int identity(1,1) not null,
	Name nvarchar(50) not null
)
go

create table CompanyInfo 
(
	CompanyID int identity(1,1) not null,
	Name nvarchar(50) not null,
	Phone nvarchar(50) not null,
	Adress nvarchar(100) not null,
	Country nvarchar(50) not null,
	Email nvarchar(50) null,
	VatCode nvarchar(50) null,
	RootCompanyID int null

)
go

--create table FinancialInfo 
--(
--	FinancialID int identity(1,1) not null,
--	Costcenter nvarchar(50) null,
--	ProjectNo nvarchar(50) null,
--	EmployeeID nvarchar(50) null

--)
--go

create table TripCategory 
(
	CategoryID int identity(1,1) not null,
	Name nvarchar(50) not null
)
go

create table SupplierInfo 
(
	SupplierID int identity(1,1) not null,
	Name nvarchar(50) not null,
	URL nvarchar(100) not null,
	SignUpUrl nvarchar(100) null,
	ResetPasswordUrl nvarchar(100) null
)
go

create table UserSupplierInfo 
(
	SupplierID int not null,
	UserID int not null,
	Username nvarchar(50) not null,
	Password nvarchar(50) not null
)
go

create table UserRole 
(
	UserRoleID int identity(1,1) not null,
	Name nvarchar(50) unique not null
)
go

create table [Transaction]
(
	TransactionID int identity(1,1) not null,
	BookingRef nvarchar(50) not null,
	Description nvarchar(200) not null,
	Destination nvarchar(50) not null,
	Traveller nvarchar(50) not null,
	HtlTime1 datetime null,
	HtlTime2 datetime null,
	AirDepTime1 datetime null,
	AirDepTime2 datetime null,
	AirRetTime1 datetime null,
	AirRetTime2 datetime null,
	CarTime1 datetime null,
	CarTime2 datetime null,
	Country nvarchar(50) null,
	CityDep1 nvarchar(50) null,
	CityDep2 nvarchar(50) null,
	CityRet1 nvarchar(50) null,
	CityRet2 nvarchar(50) null,
	Product nvarchar(50) null,
	Price real null,
	PriceUserCurrency real null,
	Total real null,
	Vat1 real null,
	Vat2 real null,
	Vat3 real null,
	Units int null,
	PriceCurrency real null,
	CardNumber nvarchar(50) null,
	CardHolder nvarchar(50) null,
	Status int not null,
	BookingDate datetime null,
	AddedDate datetime null,
	RemoveDate datetime null,
	ExtraInfo ntext null,
	Attendees nvarchar(50) null,
	ApproveDate datetime null,
	ExportStatus int not null,
	UnlockedDate datetime null,
	

	UnlockedBy int null,
	CategoryID  int not null,
	UserID  int not null,
	PaymentID int not null,
	TransactionGroupID int null,
	SupplierID int not null,
	CostCenter int null,
	EmployeeID int null,
	ProjectNO int null
)
go

create table Project 
(
	ProjectID int identity(1,1) not null,
	No nvarchar(50) not null,
	UserID int not null
)
go

create table CostCenter 
(
	CostCenterID int identity(1,1) not null,
	Name nvarchar(50) not null,
	UserID int not null
)
go

create table Employee 
(
	EmployeeID int identity(1,1) not null,
	EmployeeIdentity nvarchar(50) not null,
	UserID int not null
)
go

--Primary key
         
alter table UserProfile add constraint pk_UserProfile primary key (UserID);
go

alter table Email add constraint pk_Email primary key (EmailID);
go

alter table TransactionGroup add constraint pk_TransactionGroup primary key (TransactionGroupID);
go

alter table PaymentType add constraint pk_PaymentType primary key (PaymentID);
go

alter table CompanyInfo add constraint pk_CompanyInfo primary key (CompanyID);
go

--alter table FinancialInfo add constraint pk_FinancialInfo primary key (FinancialID);
--go

alter table TripCategory add constraint pk_TripCategory primary key (CategoryID);
go

alter table SupplierInfo add constraint pk_SupplierInfo primary key (SupplierID);
go

alter table UserSupplierInfo add constraint pk_User_Supplier primary key (UserID,SupplierID);
go


alter table UserRole add constraint pk_UserRole primary key (UserRoleID);
go

alter table  [Transaction] add constraint pk_Transaction primary key (TransactionID);
go

alter table  Project add constraint pk_Project primary key (ProjectID);
go

alter table  CostCenter add constraint pk_CostCenter primary key (CostCenterID);
go

alter table  Employee add constraint pk_Employee primary key (EmployeeID);
go
-- Foreign key

alter table UserProfile add constraint fk_UserProfile_Company foreign key(CompanyID) references CompanyInfo(CompanyID)
go

--alter table UserProfile add constraint fk_UserProfile_Financial foreign key(FinancialID) references FinancialInfo(FinancialID)
--go

alter table UserProfile add constraint fk_UserProfile_Role foreign key(UserRoleID) references UserRole(UserRoleID)
go

alter table Email add constraint fk_Email_UserProfile foreign key(UserID) references UserProfile(UserID)
go

alter table CompanyInfo add constraint fk_Company_RootCompany foreign key(RootCompanyID) references CompanyInfo(CompanyID)
go

alter table UserSupplierInfo add constraint fk_UserSupplier_Supplier foreign key(SupplierID) references SupplierInfo(SupplierID)
go

alter table UserSupplierInfo add constraint fk_UserSupplier_UserProfile foreign key(UserID) references UserProfile(UserID)
go

alter table [Transaction] add constraint fk_Transaction_Category foreign key(CategoryID) references TripCategory(CategoryID)
go

alter table [Transaction] add constraint fk_Transaction_UserProfile foreign key(UserID) references UserProfile(UserID)
go

alter table [Transaction] add constraint fk_Transaction_PaymentType foreign key(PaymentID) references PaymentType(PaymentID)
go

alter table [Transaction] add constraint fk_Transaction_Supplier foreign key(SupplierID) references SupplierInfo(SupplierID)
go

alter table [Transaction] add constraint fk_Transaction_TransactionGroup foreign key(TransactionGroupID) references TransactionGroup(TransactionGroupID)
go

alter table [Transaction] add constraint fk_Transaction_Employee foreign key(EmployeeID) references Employee(EmployeeID)
go

alter table [Transaction] add constraint fk_Transaction_CostCenter foreign key(CostCenter) references CostCenter(CostCenterID)
go
  
alter table [Transaction] add constraint fk_Transaction_Project foreign key(ProjectNO) references Project(ProjectID)
go  

alter table [Transaction] add constraint fk_Transaction_UserProfile_Unlock foreign key(UserID) references UserProfile(UserID)
go  

alter table Employee add constraint fk_Employee_UserProfile foreign key(UserID) references UserProfile(UserID)
go

alter table CostCenter add constraint fk_CostCenter_UserProfile foreign key(UserID) references UserProfile(UserID)
go

alter table Project add constraint fk_Project_UserProfile foreign key(UserID) references UserProfile(UserID)
go

--insert data

--CompanyInfo
insert into CompanyInfo values ('MrBill company','0909090909','Nygränd 10 Stockholm, Sverige','Sweden','info@mrbill.se','vatcode',null);
insert into CompanyInfo values ('MrBill sub 1','0908070605','9 Rue Comte Félix Gastaldi,98000 Monaco','Sweden','info2@mrbill.se','vatcode sub1',1);
insert into CompanyInfo values ('MrBill sub 2','0909080807','22/33 ly van kiet p10 q10 hcm city','VietNam','infovn@mrbill.se','vatcode sub2',1);

insert into CompanyInfo values ('ABC company','0101010101','1534 14th St NW,Washington, DC 20005','United States','recieve@abc.com','A123',null);
insert into CompanyInfo values ('ABC sub company','0102030405','1110 Vermont Ave NW,Washington, DC 20005','United States','subcom@abc.com','A22',4);

insert into CompanyInfo values ('Google Group','08888888888','600 Amphitheatre Parkway Mountain View, CA 94043','United States','maingoogle@gmail.com','2545',null);
insert into CompanyInfo values ('Germany Google','0777777777','Unter den Linden 14 10117 Berlin, Germany','Germany','germanygoogle@gmail.com','8788',6);
insert into CompanyInfo values ('Colombia Google','0666666666','Carrera 11A #94-45, Floor 8th Bogotá, Colombia','Colombia','colombiagoogle@gmail.com','2222',6);
insert into CompanyInfo values ('Google Budapest','0555555555','Árpád Fejedelem útja 26-28. Budapest, Hungary','Hungary','budapestgoogle@gmail.com','1234',6);

insert into CompanyInfo values ('Test company','1234567890','abc ddw, dds','Sweden','dsdsd@dsds.se','rrrr',null);
insert into CompanyInfo values ('MrBill test company','2345456786','gggg ffff www, fff','Sweden','aaaa@gfdgfg.sb','aaaa',null);
insert into CompanyInfo values ('sweden company','4567876543','time abc, second w','Sweden','bbbb@dsaad.ab','1111',null);
insert into CompanyInfo values ('www company','1234325678','rrrr rrrr rrrr,rr','Sweden','jukhk@kljk.jj','dddd',null);
insert into CompanyInfo values ('mnm company','8768567890','dffs fd fs, fdfs','Sweden','hjghj@jhgj.tt','hjhjg',null);


--UserRole
insert into UserRole values ('MrBillAdmin');
insert into UserRole values ('CompanyAdmin');
insert into UserRole values ('OrdinaryUser');

--UserProfile
create table UserProfile 
(
	 UserID int identity(1,1) not null,
	 UserName nvarchar(50) unique not null,
	 Password nvarchar(50) not null,
	 FirstName nvarchar(50) not null,
	 LastName nvarchar(50) not null,
	 Address nvarchar(100) not null,
	 City nvarchar(50) not null,
	 PostCode nvarchar(50) not null,
	 Phone nvarchar(50) not null,
	 Status int not null,
	 CompanyID int not null,
	 UserRoleID int not null
)
go
insert into UserProfile values ('test@mrbill.se','123456','abc','bcd','67/55 abc, ddd ee','sweden','2222','0102030406',1,1,3);
insert into UserProfile values ('admin@mrbill.se','123456','ddd','eee','12 gfgd,fdfs gg','sweden','3333','0123456789',1,1,1);
insert into UserProfile values ('test1@mrbill.se','123456','ggg','ttt','88 lll, ooo jjj','sweden','1111','0222222222',1,2,3);

insert into UserProfile values ('test2@mrbill.se','123456','abc company','ccc','ffd wew, llll','sweden','1212','1234545678',1,3,2);
insert into UserProfile values ('test3@mrbill.se','123456','test','ttt','88 lll, ooo jjj','sweden','1111','0222272222',1,3,3);
insert into UserProfile values ('test4@mrbill.se','123456','ggg','aaa','88 lll, ooo jjj','sweden','1111','0222229222',1,3,3);

insert into UserProfile values ('test5@mrbill.se','123456','Google Group','main','88 lll, ooo jjj','sweden','1111','0227222222',1,6,2);
insert into UserProfile values ('test6@mrbill.se','123456','Colombia Google','sub','88 lll, ooo jjj','sweden','1111','0222822222',1,8,3);
insert into UserProfile values ('test7@mrbill.se','123456','Google Budapest','sub','88 lll, ooo jjj','sweden','1111','0222292222',1,9,3);

insert into UserProfile values ('test8@mrbill.se','123456','test8','abc','90 hghg,ghgh gfgf','sweden','1111','0222222322',1,10,2);
insert into UserProfile values ('test9@mrbill.se','123456','test9','bde','65 frf, fdfdf fdf','sweden','1111','0222222422',1,11,2);
insert into UserProfile values ('test10@mrbill.se','123456','test10','mmm','12 fdffg,gfg gfgf','sweden','1111','0222252222',1,12,2);
insert into UserProfile values ('test11@mrbill.se','123456','test11','kkk','rr rrrr,rrrr','sweden','1111','0222222262',1,14,2);
insert into UserProfile values ('test12@mrbill.se','123456','test12','iii','xxx xx, xxxx x','sweden','1111','0222227222',1,13,2);

