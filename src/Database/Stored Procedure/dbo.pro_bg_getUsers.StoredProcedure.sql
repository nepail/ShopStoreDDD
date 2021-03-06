USE [ShoppingDB]
GO
/****** Object:  StoredProcedure [dbo].[pro_bg_getUsers]    Script Date: 2022/1/28 下午 01:57:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
    描述: 取得後台用戶列表
    日期: 2021-12-23

	描述: 調整程式碼風格
	日期: 2022-01-22
*/
CREATE PROCEDURE [dbo].[pro_bg_getUsers]
AS
	BEGIN
		SELECT a.f_id AS ID , a.f_account AS Account , a.f_name AS Name , b.f_id AS GroupId , b.f_name AS GroupName , a.f_createTime , a.f_updateTime
		FROM t_manager_users AS a WITH (NOLOCK) 
		JOIN t_manager_group AS b WITH (NOLOCK) ON a.f_groupId = b.f_id
	END
GO
