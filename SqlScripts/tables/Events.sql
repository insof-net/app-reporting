create or replace table Events
(
    EventId             bigint auto_increment
        primary key,
    EventTimestamp      datetime       not null,
    Severity            int            not null,
    SystemName          nvarchar(50)   not null,
    ApplicationName     nvarchar(50)   not null,
    ApplicationVersion  nvarchar(50)   not null,
    HostName            nvarchar(128)  not null,
    MethodName          nvarchar(255)  not null,
    Message             nvarchar(4000) not null,
    ExceptionType       nvarchar(128)  not null,
    ExceptionStackTrace nvarchar(4000) not null,
    LineNumber          int            not null,
    ClassName           nvarchar(255)  not null,
    FilePath            nvarchar(255)  not null
);

create index Events_ApplicationName_index
    on Events (ApplicationName);

create index Events_EventTimestamp_index
    on Events (EventTimestamp);

create index Events_Severity_index
    on Events (Severity);

create index Events_SystemName_index
    on Events (SystemName);

