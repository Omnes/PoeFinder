drop database poeitems;
create database poeitems;
use poeitems;

create table items(
	itemName varchar (30),
    itemType varchar (30),
    framteType int(1),
    #ilvl int (2),
    id char(64),
    stashID char(64),
    isIdentified bool,
    isCorrupted bool,
    links int(1),
    #implicitMods varchar(156),
    #explicitMods varchar(156),
    chaosPrice float(5),
    priceString varchar(32),
    addedDate datetime default null,
    primary key (id)
);

create table soldItems(
	itemName varchar (30),
    itemType varchar (30),
    framteType int(1),
    #ilvl int (2),
    id char(64),
    isIdentified bool,
    isCorrupted bool,
    links int(1),
    #implicitMods varchar(156),
    #explicitMods varchar(156),
    chaosPrice float(5),
    priceString varchar(32),
    addedDate datetime default null,
    primary key (id)
);