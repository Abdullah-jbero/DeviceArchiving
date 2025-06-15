//using System;
//using System.Data.SqlClient;
//using System.IO;
//using Microsoft.Extensions.Configuration;

//namespace YourWindowsFormsApp
//{
//    public class DatabaseInitializer
//    {
//        private readonly string _masterConnectionString;
//        private readonly string _connectionString;
//        private readonly string _dbPath;
//        private readonly string _dbName;

//        public DatabaseInitializer()
//        {
//            // إعداد التكوين
//            IConfiguration configuration = new ConfigurationBuilder()
//                .SetBasePath(Directory.GetCurrentDirectory())
//                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
//                .Build();

//            // قراءة الإعدادات من appsettings.json
//            _masterConnectionString = configuration.GetSection("ConnectionStrings:MasterConnectionString").Value;
//            _connectionString = configuration.GetSection("ConnectionStrings:ConnectionString").Value;
//            _dbName = configuration.GetSection("DatabaseSettings:DbName").Value;
//            _dbPath = configuration.GetSection("DatabaseSettings:DbPath").Value;

//            // التحقق من أن القيم ليست فارغة
//            if (string.IsNullOrEmpty(_masterConnectionString) || string.IsNullOrEmpty(_connectionString) ||
//                string.IsNullOrEmpty(_dbName) || string.IsNullOrEmpty(_dbPath))
//            {
//                throw new InvalidOperationException("إعدادات قاعدة البيانات مفقودة في ملف appsettings.json.");
//            }
//        }

//        public void InitializeDatabase()
//        {
//            try
//            {
//                // التأكد من وجود المجلد
//                string directory = Path.GetDirectoryName(_dbPath);
//                if (!Directory.Exists(directory))
//                {
//                    Directory.CreateDirectory(directory);
//                    Console.WriteLine($"تم إنشاء المجلد {directory} بنجاح.");
//                }

//                // الاتصال بخادم SQL Server (باستخدام قاعدة master)
//                using (var connection = new SqlConnection(_masterConnectionString))
//                {
//                    connection.Open();

//                    // التحقق مما إذا كانت القاعدة موجودة
//                    string checkDbExistsQuery = $"SELECT COUNT(*) FROM sys.databases WHERE name = @DbName";
//                    using (var command = new SqlCommand(checkDbExistsQuery, connection))
//                    {
//                        command.Parameters.AddWithValue("@DbName", _dbName);
//                        int dbCount = (int)command.ExecuteScalar();
//                        if (dbCount == 0)
//                        {
//                            // تنفيذ السكربت لإنشاء قاعدة البيانات والجداول والإجراءات المخزنة
//                            string createDbScript = $@"
//                                USE [master]
//                                GO
//                                CREATE DATABASE [{_dbName}]
//                                 CONTAINMENT = NONE
//                                 ON  PRIMARY 
//                                ( NAME = N'{_dbName}', FILENAME = N'{_dbPath}' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
//                                 LOG ON 
//                                ( NAME = N'{_dbName}_log', FILENAME = N'{_dbPath.Replace(".mdf", "_log.ldf")}' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
//                                 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
//                                GO
//                                ALTER DATABASE [{_dbName}] SET COMPATIBILITY_LEVEL = 150
//                                GO
//                                IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
//                                begin
//                                EXEC [{_dbName}].[dbo].[sp_fulltext_database] @action = 'enable'
//                                end
//                                GO
//                                ALTER DATABASE [{_dbName}] SET ANSI_NULL_DEFAULT OFF 
//                                GO
//                                ALTER DATABASE [{_dbName}] SET ANSI_NULLS OFF 
//                                GO
//                                ALTER DATABASE [{_dbName}] SET ANSI_PADDING OFF 
//                                GO
//                                ALTER DATABASE [{_dbName}] SET ANSI_WARNINGS OFF 
//                                GO
//                                ALTER DATABASE [{_dbName}] SET ARITHABORT OFF 
//                                GO
//                                ALTER DATABASE [{_dbName}] SET AUTO_CLOSE OFF 
//                                GO
//                                ALTER DATABASE [{_dbName}] SET AUTO_SHRINK OFF 
//                                GO
//                                ALTER DATABASE [{_dbName}] SET AUTO_UPDATE_STATISTICS ON 
//                                GO
//                                ALTER DATABASE [{_dbName}] SET CURSOR_CLOSE_ON_COMMIT OFF 
//                                GO
//                                ALTER DATABASE [{_dbName}] SET CURSOR_DEFAULT  GLOBAL 
//                                GO
//                                ALTER DATABASE [{_dbName}] SET CONCAT_NULL_YIELDS_NULL OFF 
//                                GO
//                                ALTER DATABASE [{_dbName}] SET NUMERIC_ROUNDABORT OFF 
//                                GO
//                                ALTER DATABASE [{_dbName}] SET QUOTED_IDENTIFIER OFF 
//                                GO
//                                ALTER DATABASE [{_dbName}] SET RECURSIVE_TRIGGERS OFF 
//                                GO
//                                ALTER DATABASE [{_dbName}] SET  DISABLE_BROKER 
//                                GO
//                                ALTER DATABASE [{_dbName}] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
//                                GO
//                                ALTER DATABASE [{_dbName}] SET DATE_CORRELATION_OPTIMIZATION OFF 
//                                GO
//                                ALTER DATABASE [{_dbName}] SET TRUSTWORTHY OFF 
//                                GO
//                                ALTER DATABASE [{_dbName}] SET ALLOW_SNAPSHOT_ISOLATION OFF 
//                                GO
//                                ALTER DATABASE [{_dbName}] SET PARAMETERIZATION SIMPLE 
//                                GO
//                                ALTER DATABASE [{_dbName}] SET READ_COMMITTED_SNAPSHOT ON 
//                                GO
//                                ALTER DATABASE [{_dbName}] SET HONOR_BROKER_PRIORITY OFF 
//                                GO
//                                ALTER DATABASE [{_dbName}] SET RECOVERY SIMPLE 
//                                GO
//                                ALTER DATABASE [{_dbName}] SET  MULTI_USER 
//                                GO
//                                ALTER DATABASE [{_dbName}] SET PAGE_VERIFY CHECKSUM  
//                                GO
//                                ALTER DATABASE [{_dbName}] SET DB_CHAINING OFF 
//                                GO
//                                ALTER DATABASE [{_dbName}] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
//                                GO
//                                ALTER DATABASE [{_dbName}] SET TARGET_RECOVERY_TIME = 60 SECONDS 
//                                GO
//                                ALTER DATABASE [{_dbName}] SET DELAYED_DURABILITY = DISABLED 
//                                GO
//                                ALTER DATABASE [{_dbName}] SET ACCELERATED_DATABASE_RECOVERY = OFF  
//                                GO
//                                EXEC sys.sp_db_vardecimal_storage_format N'{_dbName}', N'ON'
//                                GO
//                                ALTER DATABASE [{_dbName}] SET QUERY_STORE = OFF
//                                GO
//                                USE [{_dbName}]
//                                GO
//                                SET ANSI_NULLS ON
//                                GO
//                                SET QUOTED_IDENTIFIER ON
//                                GO
//                                CREATE TABLE [dbo].[Devices](
//	                                [Id] [int] IDENTITY(1,1) NOT NULL,
//	                                [Source] [nvarchar](max) NOT NULL,
//	                                [BrotherName] [nvarchar](max) NOT NULL,
//	                                [LaptopName] [nvarchar](max) NOT NULL,
//	                                [SystemPassword] [nvarchar](max) NOT NULL,
//	                                [WindowsPassword] [nvarchar](max) NOT NULL,
//	                                [HardDrivePassword] [nvarchar](max) NOT NULL,
//	                                [FreezePassword] [nvarchar](max) NOT NULL,
//	                                [Code] [nvarchar](max) NOT NULL,
//	                                [Type] [nvarchar](max) NOT NULL,
//	                                [SerialNumber] [nvarchar](450) NOT NULL,
//	                                [Comment] [nvarchar](max) NULL,
//	                                [ContactNumber] [nvarchar](max) NULL,
//	                                [Card] [nvarchar](max) NOT NULL,
//	                                [IsActive] [bit] NOT NULL,
//	                                [CreatedAt] [datetime2](7) NOT NULL,
//	                                [UpdatedAt] [datetime2](7) NULL,
//	                                [UserId] [int] NOT NULL,
//                                 CONSTRAINT [PK_Devices] PRIMARY KEY CLUSTERED 
//                                (
//	                                [Id] ASC
//                                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
//                                ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
//                                GO
//                                CREATE TABLE [dbo].[Operations](
//	                                [Id] [int] IDENTITY(1,1) NOT NULL,
//	                                [DeviceId] [int] NOT NULL,
//	                                [OperationName] [nvarchar](max) NOT NULL,
//	                                [OldValue] [nvarchar](max) NULL,
//	                                [NewValue] [nvarchar](max) NULL,
//	                                [Comment] [nvarchar](max) NULL,
//	                                [CreatedAt] [datetime2](7) NOT NULL,
//	                                [UserId] [int] NOT NULL,
//                                 CONSTRAINT [PK_Operations] PRIMARY KEY CLUSTERED 
//                                (
//	                                [Id] ASC
//                                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
//                                ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
//                                GO
//                                CREATE TABLE [dbo].[OperationsTypes](
//	                                [Id] [int] IDENTITY(1,1) NOT NULL,
//	                                [Name] [nvarchar](max) NULL,
//	                                [Description] [nvarchar](max) NULL,
//                                 CONSTRAINT [PK_OperationsTypes] PRIMARY KEY CLUSTERED 
//                                (
//	                                [Id] ASC
//                                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
//                                ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
//                                GO
//                                CREATE TABLE [dbo].[Users](
//	                                [Id] [int] IDENTITY(1,1) NOT NULL,
//	                                [UserName] [nvarchar](450) NOT NULL,
//	                                [Password] [nvarchar](max) NOT NULL,
//	                                [Picture] [varbinary](max) NOT NULL,
//                                 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
//                                (
//	                                [Id] ASC
//                                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
//                                ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
//                                GO
//                                SET ANSI_PADDING ON
//                                GO
//                                CREATE UNIQUE NONCLUSTERED INDEX [IX_Devices_SerialNumber] ON [dbo].[Devices]
//                                (
//	                                [SerialNumber] ASC
//                                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
//                                GO
//                                CREATE NONCLUSTERED INDEX [IX_Devices_UserId] ON [dbo].[Devices]
//                                (
//	                                [UserId] ASC
//                                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
//                                GO
//                                CREATE NONCLUSTERED INDEX [IX_Operations_DeviceId] ON [dbo].[Operations]
//                                (
//	                                [DeviceId] ASC
//                                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
//                                GO
//                                CREATE NONCLUSTERED INDEX [IX_Operations_UserId] ON [dbo].[Operations]
//                                (
//	                                [UserId] ASC
//                                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
//                                GO
//                                SET ANSI_PADDING ON
//                                GO
//                                CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_UserName] ON [dbo].[Users]
//                                (
//	                                [UserName] ASC
//                                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
//                                GO
//                                ALTER TABLE [dbo].[Devices]  WITH CHECK ADD  CONSTRAINT [FK_Devices_Users_UserId] FOREIGN KEY([UserId])
//                                REFERENCES [dbo].[Users] ([Id])
//                                ON DELETE CASCADE
//                                GO
//                                ALTER TABLE [dbo].[Devices] CHECK CONSTRAINT [FK_Devices_Users_UserId]
//                                GO
//                                ALTER TABLE [dbo].[Operations]  WITH CHECK ADD  CONSTRAINT [FK_Operations_Devices_DeviceId] FOREIGN KEY([DeviceId])
//                                REFERENCES [dbo].[Devices] ([Id])
//                                ON DELETE CASCADE
//                                GO
//                                ALTER TABLE [dbo].[Operations] CHECK CONSTRAINT [FK_Operations_Devices_DeviceId]
//                                GO
//                                ALTER TABLE [dbo].[Operations]  WITH CHECK ADD  CONSTRAINT [FK_Operations_Users_UserId] FOREIGN KEY([UserId])
//                                REFERENCES [dbo].[Users] ([Id])
//                                GO
//                                ALTER TABLE [dbo].[Operations] CHECK CONSTRAINT [FK_Operations_Users_UserId]
//                                GO
//                                SET ANSI_NULLS ON
//                                GO
//                                SET QUOTED_IDENTIFIER ON
//                                GO
//                                CREATE PROCEDURE [dbo].[sp_addAppUser]
//                                    @UserName NVARCHAR(255),
//                                    @Password NVARCHAR(255)
//                                AS
//                                BEGIN
//                                    SET NOCOUNT ON;

//                                    IF EXISTS (SELECT 1 FROM Users WHERE UserName = @UserName)
//                                        THROW 50000, 'اسم المستخدم موجود بالفعل', 1;

//                                    INSERT INTO Users (UserName, Password, Picture)
//                                    VALUES (@UserName, @Password, 0x);
//                                END;
//                                GO
//                                SET ANSI_NULLS ON
//                                GO
//                                SET QUOTED_IDENTIFIER ON
//                                GO
//                                CREATE PROCEDURE [dbo].[sp_AddDevice]
//                                    @Source NVARCHAR(100),
//                                    @BrotherName NVARCHAR(100),
//                                    @LaptopName NVARCHAR(100),
//                                    @SystemPassword NVARCHAR(100),
//                                    @WindowsPassword NVARCHAR(100),
//                                    @HardDrivePassword NVARCHAR(100),
//                                    @FreezePassword NVARCHAR(100),
//                                    @Code NVARCHAR(50),
//                                    @Type NVARCHAR(50),
//                                    @SerialNumber NVARCHAR(50),
//                                    @Card NVARCHAR(50),
//                                    @Comment NVARCHAR(500),
//                                    @ContactNumber NVARCHAR(50),
//                                    @IsActive BIT,
//                                    @CreatedAt DATETIME,
//                                    @UserId INT
//                                AS
//                                BEGIN
//                                    SET NOCOUNT ON;

//                                    IF EXISTS (SELECT 1 FROM Devices WHERE SerialNumber = @SerialNumber OR LaptopName = @LaptopName)
//                                        THROW 50000, 'الجهاز موجود بالفعل في النظام', 1;

//                                    INSERT INTO Devices (Source, BrotherName, LaptopName, SystemPassword, WindowsPassword, HardDrivePassword, FreezePassword, Code, Type, SerialNumber, Card, Comment, ContactNumber, IsActive, CreatedAt, UserId)
//                                    VALUES (@Source, @BrotherName, @LaptopName, @SystemPassword, @WindowsPassword, @HardDrivePassword, @FreezePassword, @Code, @Type, @SerialNumber, @Card, @Comment, @ContactNumber, @IsActive, @CreatedAt, @UserId);
//                                END;
//                                GO
//                                SET ANSI_NULLS ON
//                                GO
//                                SET QUOTED_IDENTIFIER ON
//                                GO
//                                CREATE PROCEDURE [dbo].[sp_AddOperation]
//                                    @Comment NVARCHAR(500),
//                                    @NewValue NVARCHAR(255),
//                                    @OldValue NVARCHAR(255),
//                                    @OperationName NVARCHAR(100),
//                                    @DeviceId INT,
//                                    @CreatedAt DATETIME,
//	                                @UserId INT
//                                AS
//                                BEGIN
//                                    INSERT INTO Operations (Comment, NewValue, OldValue, OperationName, DeviceId, CreatedAt, UserId)
//                                    VALUES (@Comment, @NewValue, @OldValue, @OperationName, @DeviceId, @CreatedAt, @UserId);
//                                END;
//                                GO
//                                SET ANSI_NULLS ON
//                                GO
//                                SET QUOTED_IDENTIFIER ON
//                                GO
//                                CREATE PROCEDURE [dbo].[sp_AddOperationType]
//                                    @Name NVARCHAR(100),
//                                    @Description NVARCHAR(500)
//                                AS
//                                BEGIN
//                                    INSERT INTO OperationsTypes (Name, Description)
//                                    VALUES (@Name, @Description);
//                                END;
//                                GO
//                                SET ANSI_NULLS ON
//                                GO
//                                SET QUOTED_IDENTIFIER ON
//                                GO
//                                CREATE PROCEDURE [dbo].[sp_adduser]
//                                    @loginName NVARCHAR(255),
//                                    @UserName NVARCHAR(255),
//                                    @Password NVARCHAR(255)
//                                AS
//                                BEGIN
//                                    SET NOCOUNT ON;

//                                    IF EXISTS (SELECT 1 FROM Users WHERE UserName = @UserName)
//                                        THROW 50000, 'اسم المستخدم موجود بالفعل', 1;

//                                    INSERT INTO Users (UserName, Password, Picture)
//                                    VALUES (@UserName, @Password, 0x);
//                                END;
//                                GO
//                                SET ANSI_NULLS ON
//                                GO
//                                SET QUOTED_IDENTIFIER ON
//                                GO
//                                CREATE PROCEDURE [dbo].[sp_AuthenticateUser]
//                                    @UserName NVARCHAR(255)
//                                AS
//                                BEGIN
//                                    SET NOCOUNT ON;

//                                    SELECT Id, UserName, Password
//                                    FROM Users
//                                    WHERE UserName = @UserName;
//                                END;
//                                GO
//                                SET ANSI_NULLS ON
//                                GO
//                                SET QUOTED_IDENTIFIER ON
//                                GO
//                                CREATE PROCEDURE [dbo].[sp_BackupDatabase]
//                                    @BackupPath NVARCHAR(512)
//                                AS
//                                BEGIN
//                                    SET NOCOUNT ON;

//                                    DECLARE @BackupFile NVARCHAR(512);
//                                    DECLARE @DatabaseName NVARCHAR(128) = '{_dbName}';
//                                    DECLARE @Timestamp NVARCHAR(20) = REPLACE(CONVERT(NVARCHAR, GETDATE(), 120), ':', '-');

//                                    SET @BackupFile = @BackupPath + '\\' + @DatabaseName + '_Backup_' + @Timestamp + '.bak';

//                                    BACKUP DATABASE @DatabaseName
//                                    TO DISK = @BackupFile
//                                    WITH INIT,
//                                         NAME = @DatabaseName,
//                                         DESCRIPTION = 'DeviceArchive';
//                                END;
//                                GO
//                                SET ANSI_NULLS ON
//                                GO
//                                SET QUOTED_IDENTIFIER ON
//                                GO
//                                CREATE PROCEDURE [dbo].[sp_CheckDuplicates]
//                                    @SerialNumbers NVARCHAR(MAX),
//                                    @LaptopNames NVARCHAR(MAX)
//                                AS
//                                BEGIN
//                                    WITH SplitSerials AS (
//                                        SELECT value AS SerialNumber
//                                        FROM STRING_SPLIT(@SerialNumbers, ',')
//                                        WHERE value <> ''
//                                    ),
//                                    SplitLaptopNames AS (
//                                        SELECT value AS LaptopName
//                                        FROM STRING_SPLIT(@LaptopNames, ',')
//                                        WHERE value <> ''
//                                    )
//                                    SELECT SerialNumber
//                                    FROM Devices
//                                    WHERE SerialNumber IN (SELECT SerialNumber FROM SplitSerials)
//                                    UNION
//                                    SELECT LaptopName
//                                    FROM Devices
//                                    WHERE LaptopName IN (SELECT LaptopName FROM SplitLaptopNames);
//                                END;
//                                GO
//                                SET ANSI_NULLS ON
//                                GO
//                                SET QUOTED_IDENTIFIER ON
//                                GO
//                                CREATE PROCEDURE [dbo].[sp_DeleteDevice]
//                                    @Id INT,
//                                    @UpdatedAt DATETIME,
//	                                @UserId INT
//                                AS
//                                BEGIN
//                                    IF NOT EXISTS (SELECT 1 FROM Devices WHERE Id = @Id)
//                                        THROW 50000, 'الجهاز غير موجود في النظام', 1;

//                                    UPDATE Devices
//                                    SET IsActive = 0, UpdatedAt = @UpdatedAt
//                                    WHERE Id = @Id;

//                                    INSERT INTO Operations (DeviceId, OperationName, CreatedAt, UserId)
//                                    VALUES (@Id, 'تم حذف الجهاز', @UpdatedAt, @UserId);
//                                END;
//                                GO
//                                Set ANSI_NULLS ON
//                                GO
//                                SET QUOTED_IDENTIFIER ON
//                                GO
//                                CREATE PROCEDURE [dbo].[sp_DeleteOperationType]
//                                    @Id INT
//                                AS
//                                BEGIN
//                                    DELETE FROM OperationsTypes WHERE Id = @Id;
//                                END;
//                                GO
//                                SET ANSI_NULLS ON
//                                GO
//                                SET QUOTED_IDENTIFIER ON
//                                GO
//                                CREATE PROCEDURE [dbo].[sp_GetAllDevices]
//                                AS
//                                BEGIN
//                                    SELECT d.*, u.UserName
//                                    FROM Devices d
//                                    LEFT JOIN Users u ON d.UserId = u.Id
//                                    WHERE d.IsActive = 1;
//                                END;
//                                GO
//                                SET ANSI_NULLS ON
//                                GO
//                                SET QUOTED_IDENTIFIER ON
//                                GO
//                                CREATE PROCEDURE [dbo].[sp_GetAllOperations]
//                                    @DeviceId INT
//                                AS
//                                BEGIN
//                                    SELECT o.*, u.*
//                                    FROM Operations o
//                                    LEFT JOIN Users u ON o.UserId = u.Id
//                                    WHERE o.DeviceId = @DeviceId;
//                                END;
//                                GO
//                                SET ANSI_NULLS ON
//                                GO
//                                SET QUOTED_IDENTIFIER ON
//                                GO
//                                CREATE PROCEDURE [dbo].[sp_GetAllOperationTypes]
//                                    @SearchTerm NVARCHAR(100) = NULL
//                                AS
//                                BEGIN
//                                    IF @SearchTerm IS NOT NULL
//                                        SELECT * FROM OperationsTypes
//                                        WHERE Name LIKE '%' + @SearchTerm + '%' OR Description LIKE '%' + @SearchTerm + '%';
//                                    ELSE
//                                        SELECT * FROM OperationsTypes;
//                                END;
//                                GO
//                                SET ANSI_NULLS ON
//                                GO
//                                SET QUOTED_IDENTIFIER ON
//                                GO
//                                CREATE PROCEDURE [dbo].[sp_GetDeviceById_Device]
//                                    @Id INT
//                                AS
//                                BEGIN
//                                    SET NOCOUNT ON;

//                                    SELECT 
//                                        d.Id,
//                                        d.Source,
//                                        d.BrotherName,
//                                        d.LaptopName,
//                                        d.SystemPassword,
//                                        d.WindowsPassword,
//                                        d.HardDrivePassword,
//                                        d.FreezePassword,
//                                        d.Code,
//                                        d.Type,
//                                        d.SerialNumber,
//                                        d.Card,
//                                        d.Comment,
//                                        d.ContactNumber,
//                                        u.UserName
//                                    FROM 
//                                        Devices d
//                                    LEFT JOIN 
//                                        Users u ON d.UserId = u.Id
//                                    WHERE 
//                                        d.Id = @Id;
//                                END;
//                                GO
//                                SET ANSI_NULLS ON
//                                GO
//                                SET QUOTED_IDENTIFIER ON
//                                GO
//                                CREATE PROCEDURE [dbo].[sp_GetDeviceById_Operations]
//                                    @Id INT
//                                AS
//                                BEGIN
//                                    SET NOCOUNT ON;

//                                    SELECT 
//                                        o.OperationName,
//                                        o.OldValue,
//                                        o.NewValue,
//                                        o.Comment,
//                                        o.CreatedAt,
//                                        u.UserName
//                                    FROM 
//                                        Operations o
//                                    INNER JOIN 
//                                        Users u ON o.UserId = u.Id
//                                    WHERE 
//                                        o.DeviceId = @Id
//                                    ORDER BY 
//                                        o.CreatedAt DESC;
//                                END;
//                                GO
//                                SET ANSI_NULLS ON
//                                GO
//                                SET QUOTED_IDENTIFIER ON
//                                GO
//                                CREATE PROCEDURE [dbo].[sp_GetDeviceBySerialOrLaptop]
//                                    @SerialNumber NVARCHAR(50),
//                                    @LaptopName NVARCHAR(100)
//                                AS
//                                BEGIN
//                                    SELECT TOP 1 *
//                                    FROM Devices
//                                    WHERE SerialNumber = @SerialNumber OR LaptopName = @LaptopName;
//                                END;
//                                GO
//                                SET ANSI_NULLS ON
//                                GO
//                                SET QUOTED_IDENTIFIER ON
//                                GO
//                                CREATE PROCEDURE [dbo].[sp_UpdateDevice]
//                                    @Id INT,
//                                    @Source NVARCHAR(100),
//                                    @BrotherName NVARCHAR(100),
//                                    @LaptopName NVARCHAR(100),
//                                    @SystemPassword NVARCHAR(100),
//                                    @WindowsPassword NVARCHAR(100),
//                                    @HardDrivePassword NVARCHAR(100),
//                                    @FreezePassword NVARCHAR(100),
//                                    @Code NVARCHAR(50),
//                                    @Type NVARCHAR(50),
//                                    @SerialNumber NVARCHAR(50),
//                                    @Card NVARCHAR(50),
//                                    @Comment NVARCHAR(500),
//                                    @ContactNumber NVARCHAR(50),
//                                    @UpdatedAt DATETIME,
//                                    @UserId INT
//                                AS
//                                BEGIN
//                                    SET NOCOUNT ON;

//                                    IF NOT EXISTS (SELECT 1 FROM Devices WHERE Id = @Id)
//                                        THROW 50000, 'الجهاز غير موجود في النظام', 1;

//                                    IF EXISTS (SELECT 1 FROM Devices WHERE (SerialNumber = @SerialNumber OR LaptopName = @LaptopName) AND Id != @Id)
//                                        THROW 50000, 'الجهاز موجود بالفعل في النظام', 1;

//                                    UPDATE Devices
//                                    SET Source = @Source,
//                                        BrotherName = @BrotherName,
//                                        LaptopName = @LaptopName,
//                                        SystemPassword = @SystemPassword,
//                                        WindowsPassword = @WindowsPassword,
//                                        HardDrivePassword = @HardDrivePassword,
//                                        FreezePassword = @FreezePassword,
//                                        Code = @Code,
//                                        Type = @Type,
//                                        SerialNumber = @SerialNumber,
//                                        Card = @Card,
//                                        Comment = @Comment,
//                                        ContactNumber = @ContactNumber,
//                                        UpdatedAt = @UpdatedAt,
//                                        UserId = @UserId
//                                    WHERE Id = @Id;
//                                END;
//                                GO
//                                SET ANSI_NULLS ON
//                                GO
//                                SET QUOTED_IDENTIFIER ON
//                                GO
//                                CREATE PROCEDURE [dbo].[sp_UpdateOperationType]
//                                    @Id INT,
//                                    @Name NVARCHAR(100),
//                                    @Description NVARCHAR(500)
//                                AS
//                                BEGIN
//                                    IF NOT EXISTS (SELECT 1 FROM OperationsTypes WHERE Id = @Id)
//                                        THROW 50000, 'نوع العملية غير موجود في النظام', 1;

//                                    UPDATE OperationsTypes
//                                    SET Name = @Name, Description = @Description
//                                    WHERE Id = @Id;
//                                END;
//                                GO
//                                USE [master]
//                                GO
//                                ALTER DATABASE [{_dbName}] SET READ_WRITE 
//                                GO";

//                            // تنفيذ السكربت
//                            using (var createCommand = new SqlCommand(createDbScript, connection))
//                            {
//                                createCommand.ExecuteNonQuery();
//                                Console.WriteLine($"تم إنشاء قاعدة البيانات '{_dbName}' وجميع الكائنات بنجاح.");
//                            }
//                        }
//                        else
//                        {
//                            Console.WriteLine($"قاعدة البيانات '{_dbName}' موجودة بالفعل.");
//                        }
//                    }

//                    connection.Close();
//                }
//            }
//            catch (SqlException ex)
//            {
//                MessageBox.Show($"خطأ أثناء إنشاء قاعدة البيانات: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"خطأ عام: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }
//    }
//}