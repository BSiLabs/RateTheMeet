SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/***Answer***/
ALTER TABLE [dbo].[Answer] ADD  DEFAULT (sysutcdatetime()) FOR [CreatedAt]
Go
ALTER TABLE [dbo].[Answer] ADD  DEFAULT (sysutcdatetime()) FOR [UpdatedAt]
GO
CREATE TRIGGER [dbo].[TR_dbo_Answer_InsertUpdateDelete] ON [dbo].[Answer]
AFTER INSERT, UPDATE, DELETE AS BEGIN UPDATE [dbo].[Answer] 
SET [dbo].[Answer].[UpdatedAt] = CONVERT(DATETIMEOFFSET, SYSUTCDATETIME()) 
FROM INSERTED WHERE inserted.[Id] = [dbo].[Answer].[Id] END
GO
ALTER TABLE [dbo].[Answer] ENABLE TRIGGER [TR_dbo_Answer_InsertUpdateDelete]
GO

/***Group***/
ALTER TABLE [dbo].[Group] ADD  DEFAULT (sysutcdatetime()) FOR [CreatedAt]
Go
ALTER TABLE [dbo].[Group] ADD  DEFAULT (sysutcdatetime()) FOR [UpdatedAt]
GO
CREATE TRIGGER [dbo].[TR_dbo_Group_InsertUpdateDelete] ON [dbo].[Group] 
AFTER INSERT, UPDATE, DELETE AS BEGIN UPDATE [dbo].[Group] 
SET [dbo].[Group].[UpdatedAt] = CONVERT(DATETIMEOFFSET, SYSUTCDATETIME()) 
FROM INSERTED WHERE inserted.[Id] = [dbo].[Group].[Id] END
GO
ALTER TABLE [dbo].[Group] ENABLE TRIGGER [TR_dbo_Group_InsertUpdateDelete]
GO

/***Answer***/
ALTER TABLE [dbo].[GroupUser] ADD  DEFAULT (sysutcdatetime()) FOR [CreatedAt]
Go
ALTER TABLE [dbo].[GroupUser] ADD  DEFAULT (sysutcdatetime()) FOR [UpdatedAt]
GO
CREATE TRIGGER [dbo].[TR_dbo_GroupUser_InsertUpdateDelete] ON [dbo].[GroupUser] 
AFTER INSERT, UPDATE, DELETE AS BEGIN UPDATE [dbo].[GroupUser] 
SET [dbo].[GroupUser].[UpdatedAt] = CONVERT(DATETIMEOFFSET, SYSUTCDATETIME()) 
FROM INSERTED WHERE inserted.[Id] = [dbo].[GroupUser].[Id] END
GO
ALTER TABLE [dbo].[GroupUser] ENABLE TRIGGER [TR_dbo_GroupUser_InsertUpdateDelete]
GO

/***Question***/
ALTER TABLE [dbo].[Question] ADD  DEFAULT (sysutcdatetime()) FOR [CreatedAt]
Go
ALTER TABLE [dbo].[Question] ADD  DEFAULT (sysutcdatetime()) FOR [UpdatedAt]
GO
CREATE TRIGGER [dbo].[TR_dbo_Question_InsertUpdateDelete] ON [dbo].[Question] 
AFTER INSERT, UPDATE, DELETE AS BEGIN UPDATE [dbo].[Question] 
SET [dbo].[Question].[UpdatedAt] = CONVERT(DATETIMEOFFSET, SYSUTCDATETIME()) 
FROM INSERTED WHERE inserted.[Id] = [dbo].[Question].[Id] END
GO
ALTER TABLE [dbo].[Question] ENABLE TRIGGER [TR_dbo_Question_InsertUpdateDelete]
GO

/***Survey***/
ALTER TABLE [dbo].[Survey] ADD  DEFAULT (sysutcdatetime()) FOR [CreatedAt]
Go
ALTER TABLE [dbo].[Survey] ADD  DEFAULT (sysutcdatetime()) FOR [UpdatedAt]
GO
CREATE TRIGGER [dbo].[TR_dbo_Survey_InsertUpdateDelete] ON [dbo].[Survey] 
AFTER INSERT, UPDATE, DELETE AS BEGIN UPDATE [dbo].[Survey] 
SET [dbo].[Survey].[UpdatedAt] = CONVERT(DATETIMEOFFSET, SYSUTCDATETIME()) 
FROM INSERTED WHERE inserted.[Id] = [dbo].[Survey].[Id] END
GO
ALTER TABLE [dbo].[Survey] ENABLE TRIGGER [TR_dbo_Survey_InsertUpdateDelete]
GO

/***UserAccount***/
ALTER TABLE [dbo].[UserAccount] ADD  DEFAULT (sysutcdatetime()) FOR [CreatedAt]
Go
ALTER TABLE [dbo].[UserAccount] ADD  DEFAULT (sysutcdatetime()) FOR [UpdatedAt]
GO
CREATE TRIGGER [dbo].[TR_dbo_UserAccount_InsertUpdateDelete] ON [dbo].[UserAccount] 
AFTER INSERT, UPDATE, DELETE AS BEGIN UPDATE [dbo].[UserAccount] 
SET [dbo].[UserAccount].[UpdatedAt] = CONVERT(DATETIMEOFFSET, SYSUTCDATETIME()) 
FROM INSERTED WHERE inserted.[Id] = [dbo].[UserAccount].[Id] END
GO
ALTER TABLE [dbo].[UserAccount] ENABLE TRIGGER [TR_dbo_UserAccount_InsertUpdateDelete]
GO

/***GroupSurvey***/
ALTER TABLE [dbo].[GroupSurvey] ADD  DEFAULT (sysutcdatetime()) FOR [CreatedAt]
Go
ALTER TABLE [dbo].[GroupSurvey] ADD  DEFAULT (sysutcdatetime()) FOR [UpdatedAt]
GO
CREATE TRIGGER [dbo].[TR_dbo_GroupSurvey_InsertUpdateDelete] ON [dbo].[GroupSurvey] 
AFTER INSERT, UPDATE, DELETE AS BEGIN UPDATE [dbo].[GroupSurvey] 
SET [dbo].[GroupSurvey].[UpdatedAt] = CONVERT(DATETIMEOFFSET, SYSUTCDATETIME()) 
FROM INSERTED WHERE inserted.[Id] = [dbo].[GroupSurvey].[Id] END
GO
ALTER TABLE [dbo].[GroupSurvey] ENABLE TRIGGER [TR_dbo_GroupSurvey_InsertUpdateDelete]
GO