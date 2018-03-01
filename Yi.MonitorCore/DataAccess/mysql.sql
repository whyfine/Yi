create database monitor DEFAULT CHARACTER SET gbk COLLATE gbk_chinese_ci;

CREATE TABLE `page` (
`Id`  bigint ZEROFILL NOT NULL AUTO_INCREMENT ,
`CookieId`  varchar(32) NULL ,
`UserId`  varchar(32) NULL ,
`PageId`  varchar(32) NULL ,
`EventType`  smallint ZEROFILL NULL ,
`MonitorTimestamp`  bigint ZEROFILL NULL ,
`ChannelId`  int ZEROFILL NULL ,
`ChannelTitle`  varchar(32) NULL ,
`ContentId`  bigint NULL ,
`ContentTitle`  varchar(64) NULL ,
`ScrollHeight`  smallint ZEROFILL NULL ,
PRIMARY KEY (`Id`)
)
;

CREATE TABLE `comment` (
`Id`  bigint ZEROFILL NOT NULL AUTO_INCREMENT ,
`CookieId`  varchar(32) NULL ,
`UserId`  varchar(32) NULL ,
`PageId`  varchar(32) NULL ,
`EventType`  smallint ZEROFILL NULL ,
`MonitorTimestamp`  bigint ZEROFILL NULL ,
`ContentId`  bigint NULL ,
`Level`  smallint ZEROFILL NULL ,
`Content`  varchar(256) NULL ,
`HasImg`  tinyint ZEROFILL NULL ,
`Result`  tinyint ZEROFILL NULL ,
`RequestId`  varchar(32) NULL ,
PRIMARY KEY (`Id`)
)
;

CREATE TABLE `userinfo` (
`Id`  bigint ZEROFILL NOT NULL AUTO_INCREMENT ,
`CookieId`  varchar(32) NULL ,
`UserId`  varchar(32) NULL ,
`PageId`  varchar(32) NULL ,
`EventType`  smallint ZEROFILL NULL ,
`MonitorTimestamp`  bigint ZEROFILL NULL ,
`UserName` varchar(32) NULL ,
`Mobile`  varchar(32) NULL ,
`Result`  tinyint ZEROFILL NULL ,
`RequestId`  varchar(32) NULL ,
PRIMARY KEY (`Id`)
)
;


CREATE TABLE `usercoll` (
`Id`  bigint ZEROFILL NOT NULL AUTO_INCREMENT ,
`CookieId`  varchar(32) NULL ,
`UserId`  varchar(32) NULL ,
`PageId`  varchar(32) NULL ,
`EventType`  smallint ZEROFILL NULL ,
`MonitorTimestamp`  bigint ZEROFILL NULL ,
`ContentId`  bigint NULL ,
`ContentTitle`  varchar(64) NULL ,
`Result`  tinyint ZEROFILL NULL ,
`RequestId`  varchar(32) NULL ,
PRIMARY KEY (`Id`)
)
;


CREATE TABLE `ordermain` (
`Id`  bigint ZEROFILL NOT NULL AUTO_INCREMENT ,
`CookieId`  varchar(32) NULL ,
`UserId`  varchar(32) NULL ,
`PageId`  varchar(32) NULL ,
`EventType`  smallint ZEROFILL NULL ,
`MonitorTimestamp`  bigint ZEROFILL NULL ,
`OrderId`  varchar(64) NULL ,
`OrderValue`  decimal ZEROFILL NULL ,
`PayValue`  decimal ZEROFILL NULL ,
`PayWay`  smallint ZEROFILL NULL ,
`Province`  varchar(16) NULL ,
`City`  varchar(16) NULL ,
`Area`  varchar(32) NULL ,
`Result`  tinyint ZEROFILL NULL ,
`RequestId`  varchar(32) NULL ,
PRIMARY KEY (`Id`)
)
;


CREATE TABLE `orderdetail` (
`Id`  bigint ZEROFILL NOT NULL AUTO_INCREMENT ,
`CookieId`  varchar(32) NULL ,
`UserId`  varchar(32) NULL ,
`PageId`  varchar(32) NULL ,
`EventType`  smallint ZEROFILL NULL ,
`MonitorTimestamp`  bigint ZEROFILL NULL ,
`OrderId`  varchar(64) NULL ,
`ContentId`  bigint ZEROFILL NULL ,
`Num`  int ZEROFILL NULL ,
`Result`  tinyint ZEROFILL NULL ,
`RequestId`  varchar(32) NULL ,
PRIMARY KEY (`Id`)
)
;