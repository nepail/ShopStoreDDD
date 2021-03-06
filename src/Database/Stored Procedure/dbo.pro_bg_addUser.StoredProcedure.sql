USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_bg_addUser]    Script Date: 2022/1/28 下午 01:57:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
    描述: 新增後台用戶
    日期: 2021-12-23

	描述: 調整程式碼風格
	日期: 2022-01-22
*/
CREATE PROCEDURE [dbo].[pro_bg_addUser]
				 @account VARCHAR(20) ,
				 @pcode VARCHAR(20) ,
				 @name NVARCHAR(20) ,
				 @groupId INT ,
				 @createTime DATETIME ,
				 @updateTime DATETIME
AS
	BEGIN
		INSERT INTO t_manager_users (f_account , f_pcode , f_name , f_groupId , f_createTime , f_updateTime)
		VALUES (@account , @pcode , @name , @groupId , @createTime , @updateTime)
	END
GO
