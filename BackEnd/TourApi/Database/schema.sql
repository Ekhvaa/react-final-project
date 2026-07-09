/*
    Tour Booking Database Schema
    ----------------------------
    Based on the original script, with these fixes applied:
      - Cities.Name was typed as `int` (bug) -> changed to nvarchar(50)
      - Country flags are stored as file URLs instead of varbinary/base64 data.
    Tours.TourDetailId has no enforced foreign key (kept unenforced
    on purpose to avoid a circular FK between Tours <-> TourDetails).
*/

create table Countries
(
    Id         int identity
        constraint PK_Countries
            primary key,
    Name       nvarchar(30)                        not null,
    IsoName    nvarchar(2)                         not null,
    FlagUrl    nvarchar(500),
    CreateDate datetime2 default getdate()         not null,
    IsDeleted  bit       default CONVERT([bit], 0) not null,
    UpdateDate datetime2
)
go

create table Cities
(
    Id         int identity
        constraint PK_Cities
            primary key,
    CountryId  int                                 not null
        constraint FK_Cities_Countries_CountryId
            references Countries
            on delete cascade,
    Name       nvarchar(50)                        not null,
    CreateDate datetime2 default getdate()         not null,
    IsDeleted  bit       default CONVERT([bit], 0) not null,
    UpdateDate datetime2
)
go

create index IX_Cities_CountryId
    on Cities (CountryId)
go

create unique index IX_Countries_IsoName
    on Countries (IsoName)
go

create unique index IX_Countries_Name
    on Countries (Name)
go

create table HotelServices
(
    Id   int identity
        constraint PK_HotelServices
            primary key,
    Name nvarchar(30) not null
)
go

create table Hotels
(
    Id         int identity
        constraint PK_Hotels
            primary key,
    CityId     int                                 not null
        constraint FK_Hotels_Cities_CityId
            references Cities
            on delete cascade,
    Name       nvarchar(50)                        not null,
    StarRating tinyint                             not null,
    CreateDate datetime2 default getdate()         not null,
    IsDeleted  bit       default CONVERT([bit], 0) not null,
    UpdateDate datetime2
)
go

create table HotelServiceMappings
(
    HotelId        int not null
        constraint FK_HotelServiceMappings_Hotels_HotelId
            references Hotels
            on delete cascade,
    HotelServiceId int not null
        constraint FK_HotelServiceMappings_HotelServices_HotelServiceId
            references HotelServices
            on delete cascade,
    constraint PK_HotelServiceMappings
        primary key (HotelId, HotelServiceId)
)
go

create index IX_HotelServiceMappings_HotelServiceId
    on HotelServiceMappings (HotelServiceId)
go

create index IX_Hotels_CityId
    on Hotels (CityId)
go

create table Tours
(
    Id           int identity
        constraint PK_Tours
            primary key,
    Code         nvarchar(50)                        not null,
    Name         nvarchar(max)                       not null,
    TourDetailId int                                 not null,
    CurrentPrice money                               not null,
    CreateDate   datetime2 default getdate()         not null,
    IsDeleted    bit       default CONVERT([bit], 0) not null,
    UpdateDate   datetime2
)
go

create table TourDetails
(
    Id                     int identity
        constraint PK_TourDetails
            primary key,
    TourId                 int                                 not null
        constraint FK_TourDetails_Tours_TourId
            references Tours
            on delete cascade,
    CityId                 int                                 not null
        constraint FK_TourDetails_Cities_CityId
            references Cities
            on delete cascade,
    HotelId                int
        constraint FK_TourDetails_Hotels_HotelId
            references Hotels,
    Sequence               tinyint                             not null,
    EstimatedArrivalDate   datetime2(0)                        not null,
    EstimatedDepartureDate datetime2(0)                        not null,
    CreateDate             datetime2 default getdate()         not null,
    IsDeleted              bit       default CONVERT([bit], 0) not null,
    UpdateDate             datetime2
)
go

create index IX_TourDetails_CityId
    on TourDetails (CityId)
go

create index IX_TourDetails_HotelId
    on TourDetails (HotelId)
go

create index IX_TourDetails_TourId
    on TourDetails (TourId)
go

create unique index IX_Tours_Code
    on Tours (Code)
go

create table [User]
(
    Id           int identity
        constraint PK_User
            primary key,
    Username     nvarchar(30)                        not null,
    PasswordHash binary(64)                          not null,
    CreateDate   datetime2 default getdate()         not null,
    IsDeleted    bit       default CONVERT([bit], 0) not null,
    UpdateDate   datetime2
)
go

create table Employees
(
    Id            int identity
        constraint PK_Employees
            primary key,
    UserId        int                                 not null
        constraint FK_Employees_User_UserId
            references [User],
    Discriminator nvarchar(13)                        not null,
    ContactPhone  nvarchar(20)                        not null,
    Email         nvarchar(50)                        not null,
    DateOfBirth   date                                not null,
    FirstName     nvarchar(50)                        not null,
    Gender        nvarchar(1)                         not null
        constraint CK_PersonalInfo_Gender
            check ([Gender] = 'F' OR [Gender] = 'M'),
    LastName      nvarchar(50)                        not null,
    NationalId    nvarchar(20)                        not null,
    CreateDate    datetime2 default getdate()         not null,
    IsDeleted     bit       default CONVERT([bit], 0) not null,
    UpdateDate    datetime2,
    Experience    nvarchar(200)
)
go

create unique index IX_Employees_UserId
    on Employees (UserId)
go

create table Tourists
(
    Id           int identity
        constraint PK_Tourists
            primary key,
    UserId       int                                 not null
        constraint FK_Tourists_User_UserId
            references [User],
    ContactPhone nvarchar(20)                        not null,
    Email        nvarchar(50)                        not null,
    CreateDate   datetime2 default getdate()         not null,
    IsDeleted    bit       default CONVERT([bit], 0) not null,
    UpdateDate   datetime2,
    DateOfBirth  date                                not null,
    FirstName    nvarchar(50)                        not null,
    Gender       nvarchar(1)                         not null
        constraint CK_PersonalInfo_Gender1
            check ([Gender] = 'F' OR [Gender] = 'M'),
    LastName     nvarchar(50)                        not null,
    NationalId   nvarchar(20)                        not null
)
go

create table Bookings
(
    Id            int identity
        constraint PK_Bookings
            primary key,
    TourId        int       not null
        constraint FK_Bookings_Tours_TourId
            references Tours
            on delete cascade,
    TouristId     int       not null
        constraint FK_Bookings_Tourists_TouristId
            references Tourists
            on delete cascade,
    TravelAgentId int       not null
        constraint FK_Bookings_Employees_TravelAgentId
            references Employees
            on delete cascade,
    DateOfBooking datetime2 not null,
    PricePaid     money     not null,
    Status        int       not null
)
go

create index IX_Bookings_TourId
    on Bookings (TourId)
go

create index IX_Bookings_TouristId
    on Bookings (TouristId)
go

create index IX_Bookings_TravelAgentId
    on Bookings (TravelAgentId)
go

create table TouringHistories
(
    TouristId     int          not null
        constraint FK_TouringHistories_Tourists_TouristId
            references Tourists
            on delete cascade,
    TourId        int          not null
        constraint FK_TouringHistories_Tours_TourId
            references Tours
            on delete cascade,
    DepartureDate datetime2(0) not null,
    ReturnDate    datetime2(0) not null,
    constraint PK_TouringHistories
        primary key (TouristId, TourId)
)
go

create index IX_TouringHistories_TourId
    on TouringHistories (TourId)
go

create unique index IX_Tourists_UserId
    on Tourists (UserId)
go

create unique index IX_User_Username
    on [User] (Username)
go

-- External social login links (Google/Facebook)
IF OBJECT_ID(N'[ExternalLogins]', N'U') IS NULL
BEGIN
    CREATE TABLE [ExternalLogins](
        [Id] INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ExternalLogins] PRIMARY KEY,
        [UserId] INT NOT NULL,
        [Provider] NVARCHAR(32) NOT NULL,
        [ProviderUserId] NVARCHAR(256) NOT NULL,
        [Email] NVARCHAR(256) NULL,
        [CreateDate] DATETIME2 NOT NULL CONSTRAINT [DF_ExternalLogins_CreateDate] DEFAULT GETDATE(),
        [IsDeleted] BIT NOT NULL CONSTRAINT [DF_ExternalLogins_IsDeleted] DEFAULT 0,
        [UpdateDate] DATETIME2 NULL,
        CONSTRAINT [FK_ExternalLogins_User_UserId]
            FOREIGN KEY ([UserId]) REFERENCES [User]([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_ExternalLogins_UserId]
        ON [ExternalLogins]([UserId]);

    CREATE UNIQUE INDEX [IX_ExternalLogins_Provider_ProviderUserId]
        ON [ExternalLogins]([Provider], [ProviderUserId]);
END

-- Added by production backend completion refactor
-- These changes are also represented in EF migration AddProductionFeatures.
IF COL_LENGTH('[User]', 'EmailConfirmed') IS NULL
BEGIN
    ALTER TABLE [User] ADD [EmailConfirmed] bit NOT NULL CONSTRAINT DF_User_EmailConfirmed DEFAULT 0;
END

IF COL_LENGTH('Tours', 'CreatedByEmployeeId') IS NULL
BEGIN
    ALTER TABLE Tours ADD CreatedByEmployeeId int NULL;
    CREATE INDEX IX_Tours_CreatedByEmployeeId ON Tours(CreatedByEmployeeId);
    ALTER TABLE Tours ADD CONSTRAINT FK_Tours_Employees_CreatedByEmployeeId
        FOREIGN KEY (CreatedByEmployeeId) REFERENCES Employees(Id);
END

IF COL_LENGTH('Tours', 'AssignedTourGuideId') IS NULL
BEGIN
    ALTER TABLE Tours ADD AssignedTourGuideId int NULL;
    CREATE INDEX IX_Tours_AssignedTourGuideId ON Tours(AssignedTourGuideId);
    ALTER TABLE Tours ADD CONSTRAINT FK_Tours_Employees_AssignedTourGuideId
        FOREIGN KEY (AssignedTourGuideId) REFERENCES Employees(Id);
END
