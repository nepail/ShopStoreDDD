USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_bg_getUserById]    Script Date: 2022/1/28 下午 01:57:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
    描述: 取得用戶
    日期: 2022-01-05

	描述: 調整程式碼風格
	日期: 2022-01-22
*/
CREATE PROCEDURE [dbo].[pro_bg_getUserById]
				 @Account VARCHAR(20) ,
				 @Pcode VARCHAR(20)

AS
	BEGIN
		SELECT f_id AS ID , f_account AS Account , f_name AS Name , f_groupId AS GroupId
		FROM t_manager_users WITH (NOLOCK)
		WHERE f_account = @Account
			  AND
			  f_pcode = @Pcode
	END
GO
