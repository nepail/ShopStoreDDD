USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_bg_editMemberAuth]    Script Date: 2022/1/28 下午 01:57:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
    描述: 會員停權
    日期: 2022-01-03

	描述: 調整程式碼風格
	日期: 2022-01-22
*/
CREATE PROCEDURE [dbo].[pro_bg_editMemberAuth]
				 @memberId INT ,
				 @isSuspend BIT
AS
	BEGIN
		UPDATE t_members WITH(ROWLOCK) SET f_isSuspend = @isSuspend WHERE f_id = @memberId			   		
	END
GO
