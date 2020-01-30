IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON (t.schema_id = s.schema_id)
WHERE s.name = 'dbo' AND t.name = 'AttendanceStatsUpdate')
BEGIN
    CREATE TABLE dbo.AttendanceStatsUpdate
	(
        id INT IDENTITY NOT NULL PRIMARY KEY,
        CreatedDate datetime2(7) NOT NULL DEFAULT (GETDATE()),
		MeetingId INT NOT NULL,
		OrganizationId INT NOT NULL,
		PeopleId INT NOT NULL,
		OtherMeetings varchar(max) NULL
	)
END
GO

ALTER PROCEDURE [dbo].[RecordAttend]
( @MeetingId INT, @PeopleId INT, @Present BIT)
AS
BEGIN
	DECLARE @ret nvarchar(100)

	--BEGIN TRANSACTION
	
	DECLARE @orgid INT
			,@meetingdate DATETIME
			,@meetdt DATE
			,@dt DATETIME
			,@regularhour BIT
			,@membertypeid INT
			,@allowoverlap BIT
			,@bfcattend INT
			,@bfcid INT
			,@name nvarchar(50)
			,@bfclassid INT
			,@ResetNewVisitorDays INT

	SELECT
		@orgid = o.OrganizationId,
		@meetingdate = m.MeetingDate,
		@meetdt = CONVERT(DATE, m.MeetingDate),
		@allowoverlap = o.AllowAttendOverlap,
		@membertypeid = dbo.MemberTypeAsOf(o.OrganizationId, @PeopleId, m.MeetingDate)
	FROM dbo.Meetings m
	JOIN dbo.Organizations o ON m.OrganizationId = o.OrganizationId
	WHERE m.MeetingId = @MeetingId
	
	SELECT @ResetNewVisitorDays = ISNULL((SELECT Setting FROM dbo.Setting WHERE Id = 'ResetNewVisitorDays'), 180)
	SELECT @dt = DATEADD(DAY, -@ResetNewVisitorDays, @meetdt)
	
	SELECT @regularhour = CASE WHEN EXISTS(
		SELECT null
			FROM dbo.Meetings m
			JOIN dbo.Organizations o ON m.OrganizationId = o.OrganizationId
			WHERE m.MeetingId = @MeetingId
				AND EXISTS(SELECT NULL FROM dbo.OrgSchedule
							WHERE OrganizationId = o.OrganizationId
							AND SchedDay IN (DATEPART(weekday, m.MeetingDate)-1, 10)
							AND CONVERT(TIME, m.MeetingDate) = CONVERT(TIME, MeetingTime)
							AND (o.FirstMeetingDate IS NULL OR o.FirstMeetingDate < @meetingdate) 
							AND (o.LastMeetingDate IS NULL OR o.LastMeetingDate > @meetingdate)
							)
		)
		THEN 1 ELSE 0 END

	IF @dt IS NULL
		SELECT @dt = DATEADD(DAY, 3 * -7, @meetdt)

	SELECT @name = p.[Name], @bfclassid = BibleFellowshipClassId
	FROM dbo.People p
	WHERE PeopleId = @PeopleId
	
	--Check Attended Elsewhere
	IF EXISTS(SELECT NULL
			FROM Attend ae
			JOIN dbo.Meetings m ON ae.MeetingId = m.MeetingId
			JOIN dbo.Organizations o ON m.OrganizationId = o.OrganizationId
			WHERE ae.PeopleId = @PeopleId
			AND ae.AttendanceFlag = 1
			AND ae.MeetingDate = @meetingdate
			AND ae.OrganizationId <> @orgid
			AND o.AllowAttendOverlap <> 1
			AND @allowoverlap <> 1)
	BEGIN
		SELECT @ret = @name + '(' + CONVERT(nvarchar(15), @PeopleId) + ') already attended elsewhere'
		--ROLLBACK TRANSACTION
		SELECT @ret AS ret
		RETURN
	END
	
	-- BFC class membership at same hour
	SELECT TOP 1 @bfcid = om.OrganizationId 
	FROM dbo.OrganizationMembers om
	JOIN dbo.Organizations o ON om.OrganizationId = o.OrganizationId
	WHERE om.PeopleId = @PeopleId 
	AND om.OrganizationId <> @orgid
	AND om.MemberTypeId <> 230
	AND om.MemberTypeId <> 311
	AND ISNULL(om.Pending, 0) = 0
	AND @regularhour = 1
	AND EXISTS(SELECT NULL FROM dbo.OrgSchedule os
				JOIN lookup.AttendCredit at ON os.AttendCreditId = at.Id
				WHERE os.OrganizationId = o.OrganizationId
				AND os.SchedDay IN (DATEPART(weekday, @meetingdate)-1, 10)
				AND CONVERT(TIME, @meetingdate) = CONVERT(TIME, os.MeetingTime)
				AND at.Code = 'E' -- AttendCredit = Every Meeting vs Once a Week
				)

	-- BFC Attendance at same time
	SELECT TOP 1 @bfcattend = a.AttendId FROM dbo.Attend a
	JOIN dbo.OrganizationMembers om ON a.OrganizationId = om.OrganizationId AND a.PeopleId = om.PeopleId
	JOIN dbo.Organizations o ON om.OrganizationId = o.OrganizationId
	WHERE a.MeetingDate = @meetingdate
	AND @regularhour = 1
	AND EXISTS(SELECT NULL FROM dbo.OrgSchedule
				WHERE OrganizationId = o.OrganizationId
				AND SchedDay IN (DATEPART(weekday, @meetingdate)-1, 10)
				AND CONVERT(TIME, @meetingdate) = CONVERT(TIME, MeetingTime))
	AND om.OrganizationId <> @orgid
	AND om.OrganizationId = @bfcid
	AND a.PeopleId = @PeopleId

----------------------------------

	DECLARE @AttendId INT 
	SELECT @AttendId = AttendId 
	FROM dbo.Attend 
	WHERE MeetingId = @MeetingId AND PeopleId = @PeopleId
	
	IF @AttendId IS NULL
	BEGIN
		INSERT dbo.Attend
		        ( PeopleId ,
		          MeetingId ,
		          OrganizationId ,
		          MeetingDate ,
		          AttendanceFlag ,
		          CreatedDate,
		          MemberTypeId
		        )
		VALUES  ( @PeopleId,
		          @MeetingId,
		          @orgid,
		          @meetingdate,
		          @Present,
		          GETDATE(),
		          220
		        )
		SELECT @AttendId = SCOPE_IDENTITY()
	END
	ELSE
		UPDATE dbo.Attend
		SET AttendanceFlag = @Present, SeqNo = NULL
		WHERE AttendId = @AttendId
	
	DECLARE @OtherMeetings TABLE ( id INT )
	
	DECLARE @GroupMeetingFlag BIT
	SELECT @GroupMeetingFlag = GroupMeetingFlag 
	FROM dbo.Meetings 
	WHERE MeetingId = @MeetingId
	
	DECLARE @VIPAttendance TABLE ( id INT )
	
	INSERT INTO @VIPAttendance ( id ) SELECT AttendId
				FROM Attend v
				JOIN dbo.OrganizationMembers om ON v.OrganizationId = om.OrganizationId AND v.PeopleId = om.PeopleId
				WHERE v.PeopleId = @PeopleId
				AND v.MeetingDate = @meetingdate
				AND v.AttendanceFlag = 1
				AND v.OrganizationId <> @orgid
				AND om.MemberTypeId = 700 	-- vip
				AND @orgid = @bfclassid
	
	IF @GroupMeetingFlag = 1 AND @membertypeid IS NOT NULL	
	BEGIN
		IF EXISTS(SELECT NULL FROM @VIPAttendance)
			UPDATE dbo.Attend
			SET AttendanceTypeId = 20, -- volunteer
				MemberTypeId = @membertypeid
			WHERE AttendId = @AttendId
		ELSE
			UPDATE dbo.Attend			
			SET AttendanceTypeId = 90, -- group
				MemberTypeId = @membertypeid
			WHERE AttendId = @AttendId
	END
	ELSE IF @membertypeid IS NOT NULL and @membertypeid <> 311 -- is a member of this class and not a prospect
	BEGIN
		UPDATE dbo.Attend
		SET MemberTypeId = @membertypeid,
			AttendanceTypeId = dbo.GetAttendType(@Present, @membertypeid, @GroupMeetingFlag),
			BFCAttendance = (CASE WHEN @bfclassid = @orgid THEN 1 ELSE 0 END)
		WHERE AttendId = @AttendId
		
		IF @bfcid IS NOT NULL AND (@Present = 1 OR @bfcattend IS NOT NULL)
		BEGIN
            /* At this point I am recording attendance for a vip class                     
             * or for a class where I am doing InService (long term) teaching
             * And now I am looking at the BFClass where I am a regular member or an InService Member
             * I don't need to be here if I am reversing my attendance and there is no BFCAttendance to fix
             */
			IF @bfcattend IS NULL
				EXECUTE @bfcattend = dbo.CreateOtherAttend @MeetingId, @bfcid, @PeopleId
				
			UPDATE dbo.Attend
			SET OtherAttends = @Present
			WHERE AttendId = @bfcattend
			
			UPDATE dbo.Attend
			SET OtherAttends = ISNULL((SELECT AttendanceFlag FROM dbo.Attend WHERE AttendId = @bfcattend), 0)
			WHERE AttendId = @AttendId
			
			DECLARE @BFCOtherAttends BIT
			IF EXISTS(SELECT NULL FROM dbo.Attend WHERE AttendId = @bfcattend AND OtherAttends > 0)
				SET @BFCOtherAttends = 1
							
			IF @membertypeid = 700 -- VIP
				IF @BFCOtherAttends = 1
					UPDATE dbo.Attend
					SET AttendanceTypeId = 20 -- Volunteer
					WHERE AttendId = @bfcattend
				ELSE
					UPDATE dbo.Attend
					SET AttendanceTypeId = 30 -- Member
					WHERE AttendId = @bfcattend
			ELSE IF @membertypeid = 500 -- InService Member
				IF @BFCOtherAttends = 1
					UPDATE dbo.Attend
					SET AttendanceTypeId = 70 -- InService
					WHERE AttendId = @bfcattend
				ELSE					
					UPDATE dbo.Attend
					SET AttendanceTypeId = 30 -- Member
					WHERE AttendId = @bfcattend
					
			INSERT @OtherMeetings ( id ) VALUES  ( @bfcattend )
		END
		ELSE IF EXISTS(SELECT NULL FROM @VIPAttendance) -- need to indicate BFCAttendance or not
		BEGIN
		/* At this point I am recording attendance for a BFClass
         * And now I am looking at the one or more VIP classes where I a sometimes volunteer
         */
			UPDATE dbo.Attend
			SET OtherAttends = @Present
			WHERE AttendId IN (SELECT Id FROM @VIPAttendance)
			
			UPDATE dbo.Attend
			SET AttendanceTypeId = 20,-- Volunteer
				OtherAttends = 1
			WHERE AttendId = @AttendId
			
			INSERT @OtherMeetings (id) SELECT Id FROM @VIPAttendance
		END
	END
	ELSE -- not a member of this class
	BEGIN
		IF NOT EXISTS(SELECT NULL FROM dbo.OrganizationMembers
				WHERE OrganizationId = @bfcid
				AND PeopleId = @PeopleId)
		BEGIN
			IF EXISTS(SELECT NULL FROM Attend -- RecentVisitor?
					WHERE PeopleId = @PeopleId
					AND AttendanceFlag = 1
					AND MeetingDate >= @dt
					AND MeetingDate <= @meetdt
					AND OrganizationId = @orgid
					AND AttendanceTypeId IN (50, 60)) -- new and recent
				UPDATE dbo.Attend
				SET AttendanceTypeId = 50, -- RecentVisitor
					MemberTypeId = isnull(@membertypeid, 310) -- prospect or Visitor
				WHERE AttendId = @AttendId
			ELSE
				UPDATE dbo.Attend
				SET AttendanceTypeId = 60, -- NewVisitor
					MemberTypeId = isnull(@membertypeid, 310) -- prospect or Visitor
				WHERE AttendId = @AttendId
		END
		ELSE -- member of another class (visiting member)
		BEGIN
			IF @Present = 1
				UPDATE dbo.Attend
				SET AttendanceTypeId = 40, -- Visiting Member
					MemberTypeId = 310 -- Visiting Member
				WHERE AttendId = @AttendId
				
			IF @bfcattend IS NULL
				EXECUTE @bfcattend = dbo.CreateOtherAttend @MeetingId, @bfcid, @PeopleId
				
			UPDATE dbo.Attend
			SET OtherAttends = @Present
			WHERE AttendId = @bfcattend
			
			IF @Present = 1
				UPDATE dbo.Attend
				SET AttendanceTypeId = 110 -- Other Class
				WHERE AttendId = @bfcattend
			ELSE
			BEGIN
				DECLARE @mt INT, @bfcoid INT, @group BIT
				SELECT @bfcoid = om.OrganizationId, @mt = om.MemberTypeId, @group = m.GroupMeetingFlag
				FROM dbo.Attend a 
				JOIN dbo.OrganizationMembers om ON a.PeopleId = om.PeopleId AND a.OrganizationId = om.OrganizationId
				JOIN dbo.Meetings m ON a.MeetingId = m.MeetingId
				WHERE a.AttendId = @bfcattend
				
				UPDATE dbo.Attend
				SET AttendanceTypeId = dbo.GetAttendType(AttendanceFlag, @mt, @group)
				WHERE AttendId = @bfcattend
			END	
			INSERT @OtherMeetings (id) VALUES(@bfcattend)
		END
	END
	IF EXISTS(SELECT 1 FROM dbo.Setting s WHERE s.Id='AttendCountUpdatesOffline' and s.Setting = 'true')
	BEGIN
		EXEC dbo.UpdateMeetingCounters @MeetingId
		INSERT INTO dbo.AttendanceStatsUpdate
		(MeetingId, OrganizationId, PeopleId, OtherMeetings)
		VALUES (@MeetingId, @orgid, @PeopleId, STUFF((
												   SELECT ',' + CAST(id as varchar)
												   FROM @OtherMeetings 
												   FOR XML PATH('')
											   ), 1, 1, ''))
	END
	ELSE 
	BEGIN
		EXEC dbo.UpdateAttendStr @orgid, @PeopleId
		EXEC dbo.UpdateMeetingCounters @MeetingId
		EXEC dbo.AttendUpdateN @PeopleId, 10
	
		DECLARE othermeetingscursor CURSOR FOR SELECT DISTINCT id FROM @OtherMeetings
		OPEN othermeetingscursor
		DECLARE @oaid INT
		FETCH NEXT FROM othermeetingscursor INTO @oaid
		WHILE @@Fetch_Status=0
		BEGIN
			DECLARE @oid INT, @mid INT
			SELECT @oid = OrganizationId, @mid = MeetingId FROM dbo.Attend WHERE AttendId = @oaid
			EXEC dbo.UpdateAttendStr @oid, @PeopleId
			EXEC dbo.UpdateMeetingCounters @mid
			FETCH NEXT FROM othermeetingscursor INTO @oaid
		END
		CLOSE othermeetingscursor
		DEALLOCATE othermeetingscursor
	END
	--COMMIT TRANSACTION
	SELECT 'ok' AS ret
END
GO

ALTER PROCEDURE [dbo].[UpdateAllAttendStr] (@orgid INT)
AS
BEGIN
	SET NOCOUNT ON;
	
	IF EXISTS(SELECT 1 FROM dbo.Setting s WHERE s.Id='AttendCountUpdatesOffline' and s.Setting = 'true')
	BEGIN
		INSERT INTO dbo.AttendanceStatsUpdate
		(MeetingId, OrganizationId, PeopleId, OtherMeetings)
		VALUES (0, @orgid, 0, '')
	END
	ELSE
	BEGIN
		DECLARE @start DATETIME = CURRENT_TIMESTAMP;

		DECLARE @yearago DATETIME,
				@lastmeet DATETIME,
				@maxfuturemeeting DATETIME,
				@tzoffset INT,
				@earlycheckinhours INT = 10; -- to include future meetings
				
		SELECT @tzoffset = CONVERT(INT, Setting) FROM dbo.Setting WHERE Id = 'TZOffset';
		SELECT @maxfuturemeeting = DATEADD(hh ,ISNULL(@tzoffset,0) + @earlycheckinhours, GETDATE());
			
		SELECT @lastmeet = MAX(MeetingDate) 
		FROM dbo.Meetings
		WHERE OrganizationId = @orgid
		AND MeetingDate <= @maxfuturemeeting;
		
		SELECT @yearago = DATEADD(YEAR, -1, @lastmeet);
		
		WITH om as (
			SELECT o.PeopleId, o.OrganizationId 
			FROM dbo.OrganizationMembers o
			WHERE o.OrganizationId = @orgid
		),
		v as (
			SELECT	a.PeopleId,
					CONVERT(INT, a.EffAttendFlag) Attended, 
					DATEPART(yy, m.MeetingDate) [Year], 
					DATEPART(ww, m.MeetingDate) [Week], 
					s.ScheduleId,
					a.AttendanceTypeId,
					CASE WHEN ISNULL(m.AttendCreditId, 1) = 1 
						THEN a.AttendId + 20 -- make every meeting count, 20 gets it out of the way of AttendCredit codes
						ELSE m.AttendCreditId
					END AttendCredit
			FROM dbo.Attend a
			JOIN dbo.Meetings m ON a.MeetingId = m.MeetingId
			LEFT JOIN dbo.OrgSchedule s 
				ON m.OrganizationId = s.OrganizationId 
				AND s.ScheduleId = dbo.ScheduleId(NULL, a.MeetingDate)
			WHERE m.OrganizationId = @orgid
				AND m.MeetingDate >= dbo.MinMeetingDate(m.OrganizationId, a.PeopleId, @yearago)
				AND m.MeetingDate <= @maxfuturemeeting
		),
		t as (	
			SELECT 
				PeopleId,
				CONVERT(BIT, MAX(Attended)) Attended,
				[Year],
				[Week],
				AttendCredit,
				MAX(AttendanceTypeId) AS AttendanceTypeId
			FROM v
			GROUP BY PeopleId, [Year], [Week], AttendCredit
		),
		t2 as (
		SELECT PeopleId
			, (SELECT MAX(MeetingDate) 
				FROM dbo.Attend
				WHERE PeopleId = om.PeopleId
				AND OrganizationId = om.OrganizationId
				AND AttendanceFlag = 1) LastAttended
			, CONVERT(FLOAT, ISNULL((SELECT COUNT(PeopleId)
				FROM (
						SELECT TOP 52 t.PeopleId, t.Attended
						FROM t 
						WHERE t.PeopleId = om.PeopleId
						ORDER BY t.[Year] DESC, t.[Week] DESC
					) tt 
				WHERE Attended = 1),0))
				/
				NULLIF((SELECT COUNT(PeopleId) 
				FROM (
						SELECT TOP 52 t.PeopleId, t.Attended
						FROM t 
						WHERE t.PeopleId = om.PeopleId
						ORDER BY t.[Year] DESC, t.[Week] DESC
					) tt 
				WHERE Attended IS NOT NULL),0) * 100.0
			AttPct
			,(SELECT (SELECT 
				CASE 
				WHEN Attended IS NULL THEN
					CASE AttendanceTypeId
					WHEN 20 THEN 'V'
					WHEN 70 THEN 'I'
					WHEN 80 THEN 'O'
					WHEN 90 THEN 'G'
					WHEN 110 THEN '*'
					ELSE '*'
					END
				WHEN Attended = 1 THEN 'P'
				ELSE '-'
				END AS [text()]
					FROM t
					WHERE t.PeopleId = om.PeopleId
					FOR XML PATH(''))
			) AttStr 	 
			FROM om
		)
		UPDATE dbo.OrganizationMembers
		SET LastAttended = t2.LastAttended,
			AttendStr = t2.AttStr,
			AttendPct = t2.AttPct
		FROM om m
		JOIN t2 ON t2.PeopleId = m.PeopleId
		WHERE m.OrganizationId = @orgid;

		INSERT INTO dbo.ActivityLog
				( ActivityDate , UserId , Activity , Machine )
		VALUES  ( GETDATE(), NULL , 'UpdateAllAttendStr (' + CONVERT(nvarchar, @orgid) + ',' + CONVERT(nvarchar, DATEDIFF(ms, @start, CURRENT_TIMESTAMP) / 1000) + ')', 'DB' )
	END
END
GO

ALTER PROC [dbo].[AttendUpdateN] (@pid INT, @max INT)
AS
BEGIN

	IF (SELECT COUNT(*) FROM dbo.Attend WHERE PeopleId = @pid AND AttendanceFlag = 1) > @max
		RETURN 0;

	UPDATE dbo.Attend
	SET SeqNo = NULL
	WHERE PeopleId = @pid;
	
	WITH tt as (
			SELECT DISTINCT TOP(5) CAST(MeetingDate AS DATE) MeetingDate
			FROM dbo.Attend
	        WHERE PeopleId = @pid AND AttendanceFlag = 1
			ORDER BY MeetingDate
	)
	,yy as (
		SELECT MeetingDate, ROW_NUMBER() OVER(ORDER BY MeetingDate) AS Rnk
	    FROM tt
	)
	,vv AS (
		SELECT a.AttendId, yy.Rnk FROM dbo.Attend a
		JOIN yy ON CAST(a.MeetingDate AS DATE) = yy.MeetingDate
		WHERE a.PeopleId = @pid
	) 
	UPDATE attend
	SET SeqNo = vv.Rnk
	FROM dbo.Attend attend 
	join vv ON vv.AttendId = attend.AttendId
END
GO

IF NOT EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.Attend')
AND name='IX_Attend_MeetingId_PeopleId_MeetingDate_AttendanceTypeId_AttendId_EffAttendFlag')
CREATE NONCLUSTERED INDEX [IX_Attend_MeetingId_PeopleId_MeetingDate_AttendanceTypeId_AttendId_EffAttendFlag] ON [dbo].[Attend]
(
	[MeetingId] ASC
)
INCLUDE ( 	[PeopleId],
	[MeetingDate],
	[AttendanceTypeId],
	[AttendId],
	[EffAttendFlag]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

IF NOT EXISTS( SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.Meetings')
AND name='IX_Meetings_OrganizationId_MeetingDate_MeetingId')
CREATE NONCLUSTERED INDEX [IX_Meetings_OrganizationId_MeetingDate_MeetingId] ON [dbo].[Meetings]
(
	[OrganizationId] ASC,
	[MeetingDate] ASC,
	[MeetingId] ASC
)
INCLUDE ([AttendCreditId]) WITH (SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

IF NOT EXISTS( SELECT 1 FROM sys.stats WHERE object_id = OBJECT_ID('dbo.Meetings')
AND name='ST_Meetings_MeetingId_OrganizationId_MeetingDate')
CREATE STATISTICS [ST_Meetings_MeetingId_OrganizationId_MeetingDate] ON [dbo].[Meetings]([MeetingId], [OrganizationId], [MeetingDate])
GO

ALTER PROC [dbo].[AddAbsentsToMeeting](@mid INT)
AS
BEGIN

	DECLARE @oid INT,
			@mdt DATETIME,
			@mbr INT,
			@atyp INT,
			@grp INT,
			@pid INT,
			@offsite INT,
			@acr INT,
			@NoAutoAbsents BIT,
			@firstdt DATETIME,
			@lastdt DATETIME

	SELECT 
		@oid = m.OrganizationId,
		@mid = m.MeetingId,
		@mdt = m.MeetingDate,
		@grp = m.GroupMeetingFlag,
		@NoAutoAbsents = m.NoAutoAbsents,
		@firstdt = o.FirstMeetingDate,
		@lastdt = DATEADD(d, 1, o.LastMeetingDate)
	FROM dbo.Meetings m
	JOIN dbo.Organizations o ON o.OrganizationId = m.OrganizationId
	WHERE m.MeetingId = @mid

	SELECT @acr = AttendCreditId
	FROM dbo.OrgSchedule
	WHERE OrganizationId = @oid
	AND CAST(SchedTime AS TIME) = CAST(@mdt AS TIME)
	AND SchedDay = (DATEPART(dw, @mdt) - 1)

	DECLARE c CURSOR FOR
	SELECT m.PeopleId, m.MemberTypeId, mt.AttendanceTypeId 
	FROM dbo.OrganizationMembers m
	JOIN lookup.MemberType mt ON m.MemberTypeId = mt.Id
	LEFT JOIN dbo.Attend a ON a.PeopleId = m.PeopleId AND a.MeetingId = @mid
	WHERE m.OrganizationId = @oid
	AND EnrollmentDate < @mdt
	AND @acr IS NOT NULL
	AND m.MemberTypeId <> 230 -- Inactive
	AND m.MemberTypeId <> 311 -- Prospect
	AND ISNULL(m.Pending, 0) = 0
	AND a.PeopleId IS NULL

	OPEN c;
	FETCH NEXT FROM c INTO @pid, @mbr, @atyp;
	
	WHILE @@FETCH_STATUS = 0
	BEGIN
		
		IF (@firstdt IS NULL OR @mdt > @firstdt) AND (@lastdt IS NULL OR @mdt < @lastdt)
			SELECT @offsite = CASE WHEN EXISTS(	
					SELECT NULL FROM dbo.OrganizationMembers om
					JOIN dbo.Organizations o ON om.OrganizationId = o.OrganizationId
					WHERE om.PeopleId = @pid
					AND om.OrganizationId <> @oid
					AND o.Offsite = 1
					AND o.FirstMeetingDate <= @mdt
					AND @mdt <= DATEADD(d, 1, o.LastMeetingDate))
				THEN 1 ELSE 0 END
		ELSE
			SET @offsite = 0
			
		INSERT INTO dbo.Attend
		        ( PeopleId ,
		          MeetingId ,
		          OrganizationId ,
		          MeetingDate ,
		          MemberTypeId ,
		          AttendanceTypeId ,
		          CreatedDate ,
		          AttendanceFlag ,
		          OtherAttends 
		        )
		VALUES  ( @pid , -- PeopleId - int
		          @mid , -- MeetingId - int
		          @oid , -- OrganizationId - int
		          @mdt , -- MeetingDate - datetime
		          @mbr , -- MemberTypeId - int
		          CASE WHEN @grp = 1 THEN 90 WHEN @offsite = 1 THEN 80 ELSE @atyp END , -- AttendanceTypeId - int
		          GETDATE() , -- CreatedDate - datetime
		          0 , -- AttendanceFlag - bit
		          @offsite
		        )
		FETCH NEXT FROM c INTO @pid, @mbr, @atyp;
	END;
	CLOSE c;
	DEALLOCATE c;
	IF @acr IS NOT NULL
	BEGIN
		DECLARE @usebroker BIT = (SELECT is_broker_enabled FROM sys.databases WHERE name = DB_NAME())
		IF @usebroker = 1
		BEGIN
			DECLARE @dialog UNIQUEIDENTIFIER
			BEGIN DIALOG CONVERSATION @dialog
			  FROM SERVICE UpdateAttendStrService
			  TO SERVICE 'UpdateAttendStrService'
			  ON CONTRACT UpdateAttendStrContract
			  WITH ENCRYPTION = OFF;
			SEND ON CONVERSATION @dialog MESSAGE TYPE UpdateAttendStrMessage (@oid)
		END
		ELSE
			EXEC dbo.UpdateAllAttendStr @oid
		END		
	END
GO

IF NOT EXISTS(SELECT OBJECT_ID('AddAbsentsToMeetingMessage', 'message'))
CREATE MESSAGE TYPE [AddAbsentsToMeetingMessage]
AUTHORIZATION [dbo]
VALIDATION=NONE
GO

if NOT EXISTS(SELECT OBJECT_ID('AddAbsentsToMeetingContract', 'contract'))
CREATE CONTRACT [AddAbsentsToMeetingContract]
AUTHORIZATION [dbo] ( 
[AddAbsentsToMeetingMessage] SENT BY INITIATOR
)
GO

DROP PROCEDURE IF EXISTS [dbo].[AddAbsentsToMeetingProc]
GO
CREATE PROCEDURE [dbo].[AddAbsentsToMeetingProc]
AS
BEGIN
	DECLARE @mid INT, @dlgId UNIQUEIDENTIFIER	

	WHILE(1=1)
	BEGIN
		RECEIVE top(1) @mid = CONVERT(INT, message_body), @dlgId = conversation_handle FROM dbo.AddAbsentsToMeetingQueue
		IF @@ROWCOUNT = 0		
			BREAK;
		IF @mid IS NOT NULL
			EXEC dbo.AddAbsentsToMeeting @mid
			
		END CONVERSATION @dlgId WITH CLEANUP
	END	
END
GO

if NOT EXISTS(SELECT OBJECT_ID('AddAbsentsToMeetingQueue', 'queue'))
CREATE QUEUE [dbo].[AddAbsentsToMeetingQueue] 
WITH STATUS=ON, 
RETENTION=OFF,
POISON_MESSAGE_HANDLING (STATUS=ON), 
ACTIVATION (
STATUS=ON, 
PROCEDURE_NAME=[dbo].[AddAbsentsToMeetingProc], 
MAX_QUEUE_READERS=3, 
EXECUTE AS OWNER
)
ON [PRIMARY]
GO

if NOT EXISTS(SELECT OBJECT_ID('AddAbsentsToMeetingService', 'service'))
CREATE SERVICE [AddAbsentsToMeetingService]
AUTHORIZATION [dbo]
ON QUEUE [dbo].[AddAbsentsToMeetingQueue]
(
[AddAbsentsToMeetingContract]
)
GO

ALTER TRIGGER [dbo].[insMeeting] 
   ON  [dbo].[Meetings]
   AFTER INSERT
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @oid INT,
			@mid INT,
			@mdt DATETIME,
			@mbr INT,
			@atyp INT,
			@grp INT,
			@pid INT,
			@offsite INT,
			@acr INT,
			@NoAutoAbsents BIT,
			@firstdt DATETIME,
			@lastdt DATETIME
	
	SELECT 
		@oid = OrganizationId,
		@mid = MeetingId,
		@mdt = MeetingDate,
		@grp = GroupMeetingFlag,
		@acr = AttendCreditId,
		@NoAutoAbsents = NoAutoAbsents
	FROM INSERTED
	
	IF (@NoAutoAbsents = 1)
		RETURN
	
	SELECT @NoAutoAbsents = NoAutoAbsents, @firstdt = FirstMeetingDate, @lastdt = DATEADD(d, 1, LastMeetingDate)
	FROM dbo.Organizations WHERE OrganizationId = @oid
	IF (@NoAutoAbsents <> 1)
	BEGIN
		IF EXISTS(SELECT 1 FROM dbo.Setting s WHERE s.Id='AttendCountUpdatesOffline' and s.Setting = 'true')
		BEGIN
			INSERT INTO dbo.AttendanceStatsUpdate
			(MeetingId, OrganizationId, PeopleId, OtherMeetings)
			VALUES (@mid, @oid, 0, '')
		END
		ELSE 
			DECLARE @usebroker BIT = (SELECT is_broker_enabled FROM sys.databases WHERE name = DB_NAME())
			IF @usebroker = 1
			BEGIN
				DECLARE @dialog UNIQUEIDENTIFIER
				BEGIN DIALOG CONVERSATION @dialog
					FROM SERVICE AddAbsentsToMeetingService
					TO SERVICE 'AddAbsentsToMeetingService'
					ON CONTRACT AddAbsentsToMeetingContract
					WITH ENCRYPTION = OFF;
				SEND ON CONVERSATION @dialog MESSAGE TYPE AddAbsentsToMeetingMessage (@oid)
			END
			ELSE
				EXEC dbo.AddAbsentsToMeeting @mid	
		END
	END
GO
