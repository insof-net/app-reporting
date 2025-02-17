delimiter //

create or replace procedure InsertEvent
(
    in p_Timestamp DATETIME,
    in p_Severity INT,
    in p_SystemName NVARCHAR(50),
    in p_ApplicationName NVARCHAR(50),
    in p_ApplicationVersion NVARCHAR(50),
    in p_HostName NVARCHAR(128),
    in p_FilePath NVARCHAR(255),
    in p_MethodName NVARCHAR(255),
    in p_ClassName NVARCHAR(255),
    in p_LineNumber INT,
    in p_Message NVARCHAR(4000),
    in p_ExceptionType NVARCHAR(128),
    in p_ExceptionStackTrace NVARCHAR(4000)
) begin
    insert into 
        Events (EventTimestamp, Severity, SystemName, ApplicationName, ApplicationVersion, HostName, 
                MethodName, Message, ExceptionType, ExceptionStackTrace, LineNumber, ClassName, FilePath)
    values (p_Timestamp, p_Severity, p_SystemName, p_ApplicationName,
            p_ApplicationVersion, p_HostName, p_MethodName, p_Message,
            p_ExceptionType, p_ExceptionStackTrace, p_LineNumber,
            p_ClassName, p_FilePath);
    
    select last_insert_id();

end //

delimiter ;
