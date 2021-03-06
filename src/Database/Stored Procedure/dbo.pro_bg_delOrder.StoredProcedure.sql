USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_bg_delOrder]    Script Date: 2022/1/28 下午 01:57:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
    描述: 將訂單退貨
    日期: 2021-12-15

	描述: 調整程式碼風格
	日期: 2022-01-22
*/
CREATE PROCEDURE [dbo].[pro_bg_delOrder]
				 @ordernum CHAR(14)
AS
	BEGIN
		UPDATE t_orders WITH(ROWLOCK) SET f_isDel = 1, f_status = 4 WHERE f_id = @ordernum		   				 		
	END
GO
