USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_fr_addMember]    Script Date: 2022/1/28 下午 01:57:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
    描述: 新增會員資料
    日期: 2022-01-24
	
*/
CREATE PROCEDURE [dbo].[pro_fr_addMember] (
				 @name NVARCHAR(20) ,
				 @nickName NVARCHAR(20) ,
				 @account VARCHAR(20) ,
				 @pwd VARCHAR(20) ,
				 @phone CHAR(10) ,
				 @mail VARCHAR(30) ,
				 @createTime DATETIME ,
				 @address NVARCHAR(50) ,
				 @groupId INT ,
				 @cash INT ,
				 @isSuspend BIT
)
AS
	BEGIN
		INSERT INTO [dbo].[t_members] ([f_name] , [f_nickName] , [f_account] , [f_pcode] , [f_phone] , [f_mail] , [f_createTime] , [f_address] , [f_groupId] , [f_updateTime] , [f_cash] , [f_isSuspend])
		VALUES (@name , @nickName , @account , @pwd , @phone , @mail , @createTime , @address , @groupId , @createTime , @cash , @isSuspend)
	END
GO
