create database ASMFinal
go
use ASMFinal
create table CourseCate(
	CateID int primary key,
	CateName nvarchar(100),
	Description nvarchar(100)
)
go
create table Course(
	CourseID int primary key,
	CourseName nvarchar(100),
	CateID int references CourseCate(CateID),
	Description nvarchar(100)
)
go
create table Trainer(
	TrainerID int primary key,
	TrainerName nvarchar(100),
	Type nvarchar(100),
	WorkPlace nvarchar(100),
	Phone int,
	CourseID int references Course(CourseID)
)
go
create table Topic(
	TopicID int primary key,
	TopicName nvarchar(100),
	CourseID int references Course(CourseID),
	TrainerID int references Trainer(TrainerID),
	Description nvarchar(100)
)
go
create table Trainee(
	TraineeID int primary key,
	TraineeName nvarchar(100),
	Account nvarchar(100),
	TraineeAge int,
	DOB nvarchar(100),
	Education nvarchar(100),
	MainLaguage nvarchar(100),
	TOEC nvarchar(100),
	ExperienceDetail nvarchar(100),
	Department nvarchar(100),
	CourseID int references Course(CourseID),
	Location nvarchar(100)
)
go
create table Staff(
	StaffID int primary key,
	StaffName nvarchar(100),
	StaffAge int,
	StaffEmail nvarchar(100),
	[Address] nvarchar(100)
)
go
