create or replace table EventAttributes
(
    EventId bigint                        not null,
    Name    varchar(128) charset utf8mb3  not null,
    Value   varchar(4000) charset utf8mb3 not null,
    constraint EventAttributes_Events_EventId_fk
        foreign key (EventId) references reporting.Events (EventId)
            on update cascade on delete cascade
);

