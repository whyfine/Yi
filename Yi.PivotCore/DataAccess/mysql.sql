create database pivot DEFAULT CHARACTER SET gbk COLLATE gbk_chinese_ci;

CREATE TABLE `overview` (
`Id`  bigint ZEROFILL NOT NULL AUTO_INCREMENT ,
`PV`  int ZEROFILL NULL ,
`UV`  int ZEROFILL NULL ,
`RequestCount`  int ZEROFILL NULL ,
`RegisterCount`  int ZEROFILL NULL ,
`ChannelViews`  int ZEROFILL NULL ,
`ContentViews`  int ZEROFILL NULL ,
`ContentSales`  int ZEROFILL NULL ,
`CommentCount`  int ZEROFILL NULL ,
`OrderCount`  int ZEROFILL NULL ,
`PayCount`  int ZEROFILL NULL ,
`TotalOrderAmount`  decimal ZEROFILL NULL ,
`TotalPayAmount`  decimal ZEROFILL NULL ,
`PivotDate`  date NULL ,
PRIMARY KEY (`Id`)
)
;

CREATE TABLE `member` (
`Id`  bigint ZEROFILL NOT NULL AUTO_INCREMENT ,
`UserId`  varchar(32) NULL ,
`UserName` varchar(32) NULL ,
`ViewCount`  int ZEROFILL NULL ,
`ViewTimes`  bigint ZEROFILL NULL ,
`CommentCount`  int ZEROFILL NULL ,
`OrderCount`  int ZEROFILL NULL ,
`PayCount`  int ZEROFILL NULL ,
`TotalOrderAmount`  decimal ZEROFILL NULL ,
`TotalPayAmount`  decimal ZEROFILL NULL ,
`PivotDate`  date NULL ,
PRIMARY KEY (`Id`)
)
;

CREATE TABLE `address` (
`Id`  bigint ZEROFILL NOT NULL AUTO_INCREMENT ,
`ParentName`  varchar(64) NULL , 
`Name`  varchar(32) NULL ,
`Count`  int ZEROFILL NULL ,
`PivotDate`  date NULL ,
PRIMARY KEY (`Id`)
)
;


CREATE TABLE `channel` (
`Id`  bigint ZEROFILL NOT NULL AUTO_INCREMENT ,
`ChannelId`  int ZEROFILL NULL ,
`Title`  varchar(32) NULL ,
`ViewCount`  int ZEROFILL NULL ,
`ViewTimes`  bigint ZEROFILL NULL ,
`ScrollCount`  int ZEROFILL NULL ,
`PivotDate`  date NULL ,
PRIMARY KEY (`Id`)
)
;


CREATE TABLE `content` (
`Id`  bigint ZEROFILL NOT NULL AUTO_INCREMENT ,
`ContentId`  bigint ZEROFILL NULL ,
`Title`  varchar(64) NULL ,
`ViewCount`  int ZEROFILL NULL ,
`ViewTimes`  bigint ZEROFILL NULL ,
`ScrollCount`  int ZEROFILL NULL ,
`Sales`  int ZEROFILL NULL ,
`CommentCount`  int ZEROFILL NULL ,
`PivotDate`  date NULL ,
PRIMARY KEY (`Id`)
)
;


CREATE TABLE `apiinfo` (
`Id`  bigint ZEROFILL NOT NULL AUTO_INCREMENT ,
`Type`  smallint ZEROFILL NULL ,
`Title`  varchar(72) NULL ,
`RequestCount`  int ZEROFILL NULL ,
`RequestTimeSum`  bigint ZEROFILL NULL ,
`MinRequestTime`  bigint ZEROFILL NULL ,
`MaxRequestTime`  bigint ZEROFILL NULL ,
`SpeedPercentage`  int ZEROFILL NULL ,
`PivotDate`  date NULL ,
PRIMARY KEY (`Id`)
)
;

CREATE TABLE `related` (
`Id`  bigint ZEROFILL NOT NULL AUTO_INCREMENT ,
`ContentId`  bigint ZEROFILL NULL ,
`RelevanceId`  bigint ZEROFILL NULL ,
`TitleRelevance`  int ZEROFILL NULL ,
`CollRelevance`  int ZEROFILL NULL ,
`OrderRelevance`  int ZEROFILL NULL ,
`PivotDate`  date NULL ,
PRIMARY KEY (`Id`)
)
;

----相关数据sql原型
--CREATE TABLE IF NOT EXISTS `docs` (
--  `Id`  bigint ZEROFILL NOT NULL AUTO_INCREMENT ,
--  `uid`  varchar(32) NULL ,
--  `cid`  bigint NULL ,
--  PRIMARY KEY (`Id`)
--) DEFAULT CHARSET=utf8;
--INSERT INTO `docs` (`uid`, `cid`) VALUES
--  ('ljr', 1),
--  ('ljr', 2),
--  ('ljr', 3),
--  ('ljr', 4),
--  ('ljr1', 1),
--  ('ljr1', 3),
--  ('ljr2', 1),
--  ('ljr2', 2),
--  ('ljr3', 3);

--  select a.cid,c.cid,count(1)-1
--from (
--  select cid 
--  from docs 
--  group by cid) as a 
--left join docs as b
--on a.cid=b.cid 
--left join docs as c 
--on b.uid=c.uid group by a.cid,c.cid having a.cid<>c.cid