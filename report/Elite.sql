-- *********************************************
-- * SQL MySQL generation                      
-- *--------------------------------------------
-- * DB-MAIN version: 11.0.2              
-- * Generator date: Sep 14 2021              
-- * Generation date: Sun Aug 13 18:36:06 2023 
-- * LUN file: \\vmware-host\Shared Folders\Scrivania\ElegantMotors.lun 
-- * Schema: Relazionale/2 
-- ********************************************* 


-- Database Section
-- ________________ 

create database Elite;
use Elite;


-- Tables Section
-- _____________ 

create table CLIENTE (
     Nome char(100) not null,
     Cognome char(100) not null,
     CF char(16) not null unique check(length(CF) = 16),
     CellularePersonale int not null unique check(length(CellularePersonale) = 10),
     MailPersonale char(100) unique check (MailPersonale like '%@%'),
     IDBadge int not null auto_increment,
     DataScadenza date not null,
     constraint ID_CLIENTE_ID primary key (IDBadge));

create table CONTO_VENDITA (
     LivMotore int not null check(LivMotore >= 0 and LivMotore <= 10),
     LivCarrozzeria int not null check(LivCarrozzeria >= 0 and LivCarrozzeria <= 10),
     LivInterni int not null check(LivInterni >= 0 and LivInterni <= 10),
     CodContratto int not null auto_increment,
     NrTelaio int not null,
     Prezzo float(10) not null,
     Commissione float(10) not null,
     IDBadge int not null,
     constraint ID_CONTO_VENDITA_ID primary key (CodContratto),
     constraint FKrelazione_ID unique (NrTelaio));

create table DIPENDENTE (
     Nome char(100) not null,
     Cognome char(100) not null,
     CF char(16) not null check(length(CF) = 16),
     CellularePersonale int not null unique check(length(CellularePersonale) = 10),
     MailPersonale char(100) unique check (MailPersonale like '%@%'),
     CellulareAziendale int not null unique check(length(CellulareAziendale) = 10),
     EmailAziendale char(100) not null unique check (EmailAziendale like '%@elite.it%'),
     constraint ID_DIPENDENTE_ID primary key (EmailAziendale));

create table MANUTENZIONE (
     NrTelaio int not null,
     DataManutenzione date not null,
     constraint ID_MANUTENZIONE_ID primary key (NrTelaio, DataManutenzione));

create table OPTIONAL_AUTO (
     CodOptional int not null auto_increment,
     Prezzo float(20) not null,
     NomeOptional char(250) not null,
     NomeProduttore char(100) not null,
     NrTelaio int not null,
     Descrizione varchar(400) not null,
     LivelloQualita int not null check(LivelloQualita >= 0 and LivelloQualita <= 10),
     constraint ID_OPTIONAL_AUTO_ID primary key (CodOptional),
     constraint SID_OPTIONAL_AUTO_1_ID unique (NomeProduttore, NomeOptional),
     constraint SID_OPTIONAL_AUTO_ID unique (NrTelaio, NomeOptional));

create table ORDINE (
     Importo float(20) not null,
     CodOrdine int not null auto_increment,
     DataOrdine date not null,
     EmailAziendale char(100) not null,
     IDBadge int not null,
     constraint ID_ORDINE_ID primary key (CodOrdine));

create table PRODUTTORE (
     Indirizzo varchar(400) not null,
     Automibilistico char not null,
     OptionalMarket char not null,
     NomeProduttore char(100) not null,
     constraint ID_PRODUTTORE_ID primary key (NomeProduttore));

create table SEGMENTO (
     NomeSegemento char(250) not null,
     Descrizione varchar(400) not null,
     constraint ID_SEGMENTO_ID primary key (NomeSegemento));

create table STIPENDIO (
     EmailAziendale char(100) not null,
     Importo float(10) not null,
     Anno int not null,
     Mese int not null,
     Bonus float(10),
     constraint ID_STIPENDIO_ID primary key (EmailAziendale, Anno, Mese));

create table SUPERCAR (
     NomeProduttore char(100) not null,
     NomeModello char(200) not null,
     CavalliPotenza int not null,
     Alimentazione char(200) not null,
     NomeSegemento char(250) not null,
     constraint ID_SUPERCAR_ID primary key (NomeProduttore, NomeModello));

create table supporto (
     CodOptional int not null,
     NomeSegemento char(250) not null,
     constraint ID_supporto_ID primary key (NomeSegemento, CodOptional));

create table VERSIONE (
     NomeVersione char(250) not null,
     NrTelaio int not null,
     Colore char(250) not null,
     Prezzo float(10) not null,
     CodOrdine int,
     NomeProduttore char(100) not null,
     NomeModello char(200) not null,
     constraint ID_VERSIONE_ID primary key (NrTelaio));


-- Constraints Section
-- ___________________ 

alter table CONTO_VENDITA add constraint FKrelazione_FK
     foreign key (NrTelaio)
     references VERSIONE (NrTelaio);

alter table CONTO_VENDITA add constraint FKpiazzamento_FK
     foreign key (IDBadge)
     references CLIENTE (IDBadge);

alter table MANUTENZIONE add constraint FKcontrollo
     foreign key (NrTelaio)
     references VERSIONE (NrTelaio);

-- Not implemented
-- alter table OPTIONAL_AUTO add constraint ID_OPTIONAL_AUTO_CHK
--     check(exists(select * from supporto
--                  where supporto.CodOptional = CodOptional)); 

alter table OPTIONAL_AUTO add constraint FKproduzioneOptional
     foreign key (NomeProduttore)
     references PRODUTTORE (NomeProduttore);

alter table OPTIONAL_AUTO add constraint FKequipaggiamento
     foreign key (NrTelaio)
     references VERSIONE (NrTelaio);

-- Not implemented
-- alter table ORDINE add constraint ID_ORDINE_CHK
--     check(exists(select * from VERSIONE
--                  where VERSIONE.CodOrdine = CodOrdine)); 

alter table ORDINE add constraint FKvendita_FK
     foreign key (EmailAziendale)
     references DIPENDENTE (EmailAziendale);

alter table ORDINE add constraint FKacquisto_FK
     foreign key (IDBadge)
     references CLIENTE (IDBadge);

alter table STIPENDIO add constraint FKpagamento
     foreign key (EmailAziendale)
     references DIPENDENTE (EmailAziendale);

-- Not implemented
-- alter table SUPERCAR add constraint ID_SUPERCAR_CHK
--     check(exists(select * from VERSIONE
--                  where VERSIONE.NomeProduttore = NomeProduttore and VERSIONE.NomeModello = NomeModello)); 

alter table SUPERCAR add constraint FKproduzioneSupercar
     foreign key (NomeProduttore)
     references PRODUTTORE (NomeProduttore);

alter table SUPERCAR add constraint FKcategorizzazione_FK
     foreign key (NomeSegemento)
     references SEGMENTO (NomeSegemento);

alter table supporto add constraint FKsup_SEG
     foreign key (NomeSegemento)
     references SEGMENTO (NomeSegemento);

alter table supporto add constraint FKsup_OPT_FK
     foreign key (CodOptional)
     references OPTIONAL_AUTO (CodOptional);

alter table VERSIONE add constraint FKcontenimento_FK
     foreign key (CodOrdine)
     references ORDINE (CodOrdine);

alter table VERSIONE add constraint FKrestyling_FK
     foreign key (NomeProduttore, NomeModello)
     references SUPERCAR (NomeProduttore, NomeModello);


-- Index Section
-- _____________ 

create unique index ID_CLIENTE_IND
     on CLIENTE (IDBadge);

create unique index ID_CONTO_VENDITA_IND
     on CONTO_VENDITA (CodContratto);

create unique index FKrelazione_IND
     on CONTO_VENDITA (NrTelaio);

create index FKpiazzamento_IND
     on CONTO_VENDITA (IDBadge);

create unique index ID_DIPENDENTE_IND
     on DIPENDENTE (EmailAziendale);

create unique index ID_MANUTENZIONE_IND
     on MANUTENZIONE (NrTelaio, DataManutenzione);

create unique index ID_OPTIONAL_AUTO_IND
     on OPTIONAL_AUTO (CodOptional);

create unique index SID_OPTIONAL_AUTO_1_IND
     on OPTIONAL_AUTO (NomeProduttore, NomeOptional);

create unique index SID_OPTIONAL_AUTO_IND
     on OPTIONAL_AUTO (NrTelaio, NomeOptional);

create unique index ID_ORDINE_IND
     on ORDINE (CodOrdine);

create index FKvendita_IND
     on ORDINE (EmailAziendale);

create index FKacquisto_IND
     on ORDINE (IDBadge);

create unique index ID_PRODUTTORE_IND
     on PRODUTTORE (NomeProduttore);

create unique index ID_SEGMENTO_IND
     on SEGMENTO (NomeSegemento);

create unique index ID_STIPENDIO_IND
     on STIPENDIO (EmailAziendale, Anno, Mese);

create unique index ID_SUPERCAR_IND
     on SUPERCAR (NomeProduttore, NomeModello);

create index FKcategorizzazione_IND
     on SUPERCAR (NomeSegemento);

create unique index ID_supporto_IND
     on supporto (NomeSegemento, CodOptional);

create index FKsup_OPT_IND
     on supporto (CodOptional);

create unique index ID_VERSIONE_IND
     on VERSIONE (NrTelaio);

create index FKcontenimento_IND
     on VERSIONE (CodOrdine);

create index FKrestyling_IND
     on VERSIONE (NomeProduttore, NomeModello);

