USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_bg_getOrderSetting]    Script Date: 2022/1/28 下午 01:57:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
    描述: 取得訂單設定
    日期: 2022-01-03

	描述: 調整程式碼風格
	日期: 2022-01-22
*/
CREATE PROCEDURE [dbo].[pro_bg_getOrderSetting]
AS
	BEGIN
		SELECT '030600' + CAST(f_id AS VARCHAR) AS Code , f_name AS Name , 'bg-' + f_badge AS Style
		FROM t_orderStatus WITH (NOLOCK)

		SELECT '030610' + CAST(f_id AS VARCHAR) AS Code , f_name AS Name , f_badge AS Style
		FROM t_orderShipping WITH (NOLOCK)
	END
GO
