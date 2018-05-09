
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 05/09/2018 14:48:11
-- Generated from EDMX file: C:\Users\Olga\Source\Repos\CAMS\CAMS\CAMS\Models\CAMS_DatabaseModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [CAMS_Database];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_Activity_ToComputer]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Activities] DROP CONSTRAINT [FK_Activity_ToComputer];
GO
IF OBJECT_ID(N'[dbo].[FK_Computer_Lab]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Computers] DROP CONSTRAINT [FK_Computer_Lab];
GO
IF OBJECT_ID(N'[dbo].[FK_ComputerLabs_Computer]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ComputerLabs] DROP CONSTRAINT [FK_ComputerLabs_Computer];
GO
IF OBJECT_ID(N'[dbo].[FK_ComputerLabs_Lab]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ComputerLabs] DROP CONSTRAINT [FK_ComputerLabs_Lab];
GO
IF OBJECT_ID(N'[dbo].[FK_Lab_Department]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Labs] DROP CONSTRAINT [FK_Lab_Department];
GO
IF OBJECT_ID(N'[dbo].[FK_UserDepartments_Department]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserDepartments] DROP CONSTRAINT [FK_UserDepartments_Department];
GO
IF OBJECT_ID(N'[dbo].[FK_UserDepartments_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserDepartments] DROP CONSTRAINT [FK_UserDepartments_User];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Activities]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Activities];
GO
IF OBJECT_ID(N'[dbo].[Computers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Computers];
GO
IF OBJECT_ID(N'[dbo].[ComputerLabs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ComputerLabs];
GO
IF OBJECT_ID(N'[dbo].[Departments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Departments];
GO
IF OBJECT_ID(N'[dbo].[Labs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Labs];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[UserDepartments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserDepartments];
GO
IF OBJECT_ID(N'[dbo].[database_firewall_rules]', 'U') IS NOT NULL
    DROP TABLE [dbo].[database_firewall_rules];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Activities'
CREATE TABLE [dbo].[Activities] (
    [Login] datetime  NOT NULL,
    [UserName] nvarchar(50)  NULL,
    [Logout] datetime  NULL,
    [Mode] tinyint  NOT NULL,
    [ComputerId] int  NOT NULL,
    [Weekend] bit  NOT NULL
);
GO

-- Creating table 'Computers'
CREATE TABLE [dbo].[Computers] (
    [ComputerId] int  NOT NULL,
    [MAC] char(12)  NULL,
    [ComputerName] nvarchar(50)  NULL,
    [CurrentLab] int  NULL,
    [LocationInLab] nvarchar(50)  NULL
);
GO

-- Creating table 'ComputerLabs'
CREATE TABLE [dbo].[ComputerLabs] (
    [LabId] int  NOT NULL,
    [ComputerId] int  NOT NULL,
    [Entrance] datetime  NOT NULL,
    [Exit] datetime  NULL
);
GO

-- Creating table 'Departments'
CREATE TABLE [dbo].[Departments] (
    [DepartmentId] int  NOT NULL,
    [DepartmentName] nvarchar(50)  NULL,
    [Domain] nvarchar(50)  NULL
);
GO

-- Creating table 'Labs'
CREATE TABLE [dbo].[Labs] (
    [LabId] int  NOT NULL,
    [Building] nvarchar(150)  NULL,
    [RoomNumber] char(15)  NULL,
    [DepartmentId] int  NOT NULL,
    [TodaysClasses] nvarchar(150)  NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [UserId] int  NOT NULL,
    [Email] nvarchar(50)  NULL,
    [DisconnectedPeriod] int  NULL,
    [NotActivePeriod] int  NULL,
    [NotificationFrequency] tinyint  NOT NULL
);
GO

-- Creating table 'UserDepartments'
CREATE TABLE [dbo].[UserDepartments] (
    [UserId] int  NOT NULL,
    [DepartmentId] int  NOT NULL,
    [AccessType] tinyint  NOT NULL
);
GO

-- Creating table 'database_firewall_rules'
CREATE TABLE [dbo].[database_firewall_rules] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(128)  NOT NULL,
    [start_ip_address] varchar(45)  NOT NULL,
    [end_ip_address] varchar(45)  NOT NULL,
    [create_date] datetime  NOT NULL,
    [modify_date] datetime  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Login], [Mode], [ComputerId] in table 'Activities'
ALTER TABLE [dbo].[Activities]
ADD CONSTRAINT [PK_Activities]
    PRIMARY KEY CLUSTERED ([Login], [Mode], [ComputerId] ASC);
GO

-- Creating primary key on [ComputerId] in table 'Computers'
ALTER TABLE [dbo].[Computers]
ADD CONSTRAINT [PK_Computers]
    PRIMARY KEY CLUSTERED ([ComputerId] ASC);
GO

-- Creating primary key on [LabId], [ComputerId], [Entrance] in table 'ComputerLabs'
ALTER TABLE [dbo].[ComputerLabs]
ADD CONSTRAINT [PK_ComputerLabs]
    PRIMARY KEY CLUSTERED ([LabId], [ComputerId], [Entrance] ASC);
GO

-- Creating primary key on [DepartmentId] in table 'Departments'
ALTER TABLE [dbo].[Departments]
ADD CONSTRAINT [PK_Departments]
    PRIMARY KEY CLUSTERED ([DepartmentId] ASC);
GO

-- Creating primary key on [LabId] in table 'Labs'
ALTER TABLE [dbo].[Labs]
ADD CONSTRAINT [PK_Labs]
    PRIMARY KEY CLUSTERED ([LabId] ASC);
GO

-- Creating primary key on [UserId] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([UserId] ASC);
GO

-- Creating primary key on [UserId], [DepartmentId] in table 'UserDepartments'
ALTER TABLE [dbo].[UserDepartments]
ADD CONSTRAINT [PK_UserDepartments]
    PRIMARY KEY CLUSTERED ([UserId], [DepartmentId] ASC);
GO

-- Creating primary key on [id], [name], [start_ip_address], [end_ip_address], [create_date], [modify_date] in table 'database_firewall_rules'
ALTER TABLE [dbo].[database_firewall_rules]
ADD CONSTRAINT [PK_database_firewall_rules]
    PRIMARY KEY CLUSTERED ([id], [name], [start_ip_address], [end_ip_address], [create_date], [modify_date] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [ComputerId] in table 'Activities'
ALTER TABLE [dbo].[Activities]
ADD CONSTRAINT [FK_Activity_ToComputer]
    FOREIGN KEY ([ComputerId])
    REFERENCES [dbo].[Computers]
        ([ComputerId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Activity_ToComputer'
CREATE INDEX [IX_FK_Activity_ToComputer]
ON [dbo].[Activities]
    ([ComputerId]);
GO

-- Creating foreign key on [CurrentLab] in table 'Computers'
ALTER TABLE [dbo].[Computers]
ADD CONSTRAINT [FK_Computer_Lab]
    FOREIGN KEY ([CurrentLab])
    REFERENCES [dbo].[Labs]
        ([LabId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Computer_Lab'
CREATE INDEX [IX_FK_Computer_Lab]
ON [dbo].[Computers]
    ([CurrentLab]);
GO

-- Creating foreign key on [ComputerId] in table 'ComputerLabs'
ALTER TABLE [dbo].[ComputerLabs]
ADD CONSTRAINT [FK_ComputerLabs_Computer]
    FOREIGN KEY ([ComputerId])
    REFERENCES [dbo].[Computers]
        ([ComputerId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ComputerLabs_Computer'
CREATE INDEX [IX_FK_ComputerLabs_Computer]
ON [dbo].[ComputerLabs]
    ([ComputerId]);
GO

-- Creating foreign key on [LabId] in table 'ComputerLabs'
ALTER TABLE [dbo].[ComputerLabs]
ADD CONSTRAINT [FK_ComputerLabs_Lab]
    FOREIGN KEY ([LabId])
    REFERENCES [dbo].[Labs]
        ([LabId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [DepartmentId] in table 'Labs'
ALTER TABLE [dbo].[Labs]
ADD CONSTRAINT [FK_Lab_Department]
    FOREIGN KEY ([DepartmentId])
    REFERENCES [dbo].[Departments]
        ([DepartmentId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Lab_Department'
CREATE INDEX [IX_FK_Lab_Department]
ON [dbo].[Labs]
    ([DepartmentId]);
GO

-- Creating foreign key on [DepartmentId] in table 'UserDepartments'
ALTER TABLE [dbo].[UserDepartments]
ADD CONSTRAINT [FK_UserDepartments_Department]
    FOREIGN KEY ([DepartmentId])
    REFERENCES [dbo].[Departments]
        ([DepartmentId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserDepartments_Department'
CREATE INDEX [IX_FK_UserDepartments_Department]
ON [dbo].[UserDepartments]
    ([DepartmentId]);
GO

-- Creating foreign key on [UserId] in table 'UserDepartments'
ALTER TABLE [dbo].[UserDepartments]
ADD CONSTRAINT [FK_UserDepartments_User]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users]
        ([UserId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------