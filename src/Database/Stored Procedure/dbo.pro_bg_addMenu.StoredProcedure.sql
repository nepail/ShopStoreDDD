USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_bg_addMenu]    Script Date: 2022/1/28 下午 01:57:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
    描述: 新增菜單
    日期: 2021-12-03

	描述: 調整程式碼風格
	日期: 2022-01-24
*/
CREATE PROCEDURE [dbo].[pro_bg_addMenu]
				 @f_name NVARCHAR(10)

AS
	BEGIN
		INSERT INTO t_manager_menu (f_name , f_isDel)
		VALUES (@f_name , 0)
	END
GO
