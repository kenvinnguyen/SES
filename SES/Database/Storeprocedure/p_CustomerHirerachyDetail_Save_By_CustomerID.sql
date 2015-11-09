
CREATE PROCEDURE [dbo].[p_CustomerHirerachyDetail_Save_By_CustomerID]
	@CustomerID nvarchar(15) = ''
	,@UserID varchar(256) = 'system'
	,@CustomerHirerachyIDs nvarchar(4000) = ''
AS
	IF OBJECT_ID('tempdb..#SelectedCustomerHireachy') IS NOT NULL
		DROP TABLE #SelectedCustomerHireachy
		
	SELECT	ID
			,Data
	INTO	#SelectedCustomerHireachy
	FROM	[dbo].[fn_DelimitedStringToTable](@CustomerHirerachyIDs, ',')
	
	-- exec in transaction
	BEGIN TRAN

	BEGIN TRY
		DELETE	a
		FROM	CustomerHirerachyDetail a
		WHERE	CustomerID = @CustomerID
					
		INSERT INTO CustomerHirerachyDetail(CustomerID, CustomerHirerachyID,  [Status], CreatedAt, CreatedBy)
		SELECT	@CustomerID
				,a.Data
				,1 AS 'Status'
				,GETDATE()
				,@UserID
		FROM	#SelectedCustomerHireachy a
	END TRY
	BEGIN CATCH
		SELECT ERROR_MESSAGE() AS 'ErrorMessage'

		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;
	END CATCH;

	IF @@TRANCOUNT > 0
		COMMIT TRANSACTION;
			
	IF OBJECT_ID('tempdb..#SelectedCustomerHireachy') IS NOT NULL
		DROP TABLE #SelectedCustomerHireachy
	
	-- [p_CustomerHirerachyDetail_SaveAction_By_CustomerID]
