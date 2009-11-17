SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[TaggedPeople](@tagid INT) 
RETURNS @t TABLE ( PeopleId INT)
AS
BEGIN
	INSERT INTO @t (PeopleId)
	SELECT p.PeopleId FROM dbo.People p
	WHERE EXISTS(
    SELECT NULL
    FROM dbo.TagPerson t
    WHERE (t.Id = @tagid) AND (t.PeopleId = p.PeopleId))
    RETURN
END
GO
