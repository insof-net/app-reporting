delimiter //

create or replace procedure InsertEventAttribute
(
    in p_Id bigint,
    in p_Name nvarchar(128),
    in p_Value nvarchar(4000)
) begin 
    
    insert into EventAttributes
        (EventId, Name, Value) 
    values (p_Id, p_Name, p_Value);
    
end //

delimiter ;