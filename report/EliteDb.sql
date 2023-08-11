-- *********************************************
-- * SQL MySQL generation                      
-- *--------------------------------------------
-- * DB-MAIN version: 11.0.2              
-- * Generator date: Sep 14 2021              
-- * Generation date: Wed Aug  9 15:10:32 2023 
-- * LUN file: \\vmware-host\Shared Folders\Scrivania\ElegantMotors.lun 
-- * Schema: Relazionale/1 
-- ********************************************* 


-- Database Section
-- ________________ 

create database Elite;
use Elite;


-- Tables Section
-- _____________ 

create table CLIENTE (
     Nome char(30) not null,
     Cognome char(30) not null,
     CF char(16) not null unique check(length(CF) = 16),
     Cellulare_Personale bigint(10) not null unique check(length(Cellulare_Personale) = 10),
     Mail_Personale char(40) unique check (Mail_Personale like '%@%'),
     ID_Badge bigint not null AUTO_INCREMENT,
     Data_Scadenza date not null,
     constraint ID_CLIENTE_ID primary key (ID_Badge));

create table CONTO_VENDITA (
     Liv_Motore int not null check(Liv_Motore >= 0 and Liv_Motore <= 10),
     Liv_Carrozzeria int not null check(Liv_Carrozzeria >= 0 and Liv_Carrozzeria <= 10),
     Liv_Interni int not null check(Liv_Interni >= 0 and Liv_Interni <= 10),
     Cod_Contratto int not null auto_increment,
     Nr_Telaio bigint(11) not null,
     Prezzo decimal(10,2) not null,
     Commissione decimal(10,2) not null,
     ID_Badge bigint not null,
     constraint ID_CONTO_VENDITA_ID primary key (Cod_Contratto),
     constraint FKrelazione_ID unique (Nr_Telaio));

create table DIPENDENTE (
     Nome char(30) not null,
     Cognome char(30) not null,
     CF char(16) not null check(length(CF) = 16),
     Cellulare_Personale int not null unique check(length(Cellulare_Personale) = 10),
     Mail_Personale char(40) unique check (Mail_Personale like '%@%'),
     Cellulare_Aziendale int not null unique check(length(Cellulare_Aziendale) = 10),
     Email_Aziendale char(40) not null unique check (Email_Aziendale like '%@%'),
     constraint ID_DIPENDENTE_ID primary key (Email_Aziendale));

create table MANUTENZIONE (
     Nr_Telaio bigint not null,
     Data_Manutenzione date not null,
     constraint ID_MANUTENZIONE_ID primary key (Nr_Telaio, Data_Manutenzione));

create table OPTIONAL_AUTO (
     P_IVA char(11) not null, 
     Prezzo bigint not null,
     Nome_Optional char(20) not null,
     Descrizione varchar(400) not null,
     Livello_Qualita int not null check(Livello_Qualita >= 0 and Livello_Qualita <= 10),
     Nome_Modello char(30) not null,
     constraint ID_OPTIONAL_AUTO_ID primary key (P_IVA, Nome_Optional));

create table ORDINE (
     Importo bigint not null,
     Cod_Ordine int not null auto_increment,
     DataOrdine date not null,
     Ora time not null,
     Email_Aziendale char(40) not null,
     ID_Badge bigint not null,
     constraint ID_ORDINE_ID primary key (Cod_Ordine));

create table PRODUTTORE (
     Automibilistico boolean not null,
     OptionalMarket boolean not null,
     Stato char(40) not null,
     Citta char(40) not null,
     Via char(50) not null,
     Civico int not null,
     Nome_Produttore char(50) not null unique,
     P_IVA char(11) not null,
     constraint ID_PRODUTTORE_ID primary key (P_IVA));

create table SEGMENTO (
     Nome char(50) not null,
     Descrizione varchar(300) not null,
     constraint ID_SEGMENTO_ID primary key (Nome));

create table STIPENDIO (
     Email_Aziendale char(40) not null,
     Importo int not null,
     Anno int not null,
     Mese int not null,
     Bonus int,
     constraint ID_STIPENDIO_ID primary key (Email_Aziendale, Anno, Mese));

create table SUPERCAR (
     Nome_Modello char(50) not null,
     Cavalli_Potenza int not null,
     Alimentazione char(50) not null,
     P_IVA char(11) not null,
     Nome char(50) not null,
     constraint ID_SUPERCAR_ID primary key (Nome_Modello));

create table supporto (
     P_IVA char(11) not null,
     Nome_Optional char(20) not null,
     Nome char(50) not null,
     constraint ID_supporto_ID primary key (Nome, P_IVA, Nome_Optional));

create table VERSIONE (
     Nr_Telaio bigint not null,
     Colore char(100) not null,
     Prezzo bigint not null,
     Nome_Modello char(50) not null,
     Cod_Ordine int,
     constraint ID_VERSIONE_ID primary key (Nr_Telaio));


-- Constraints Section
-- ___________________ 

alter table CONTO_VENDITA add constraint FKrelazione_FK
     foreign key (Nr_Telaio)
     references VERSIONE (Nr_Telaio);

alter table CONTO_VENDITA add constraint FKpiazzamento_FK
     foreign key (ID_Badge)
     references CLIENTE (ID_Badge);

alter table MANUTENZIONE add constraint FKcontrollo
     foreign key (Nr_Telaio)
     references VERSIONE (Nr_Telaio);

-- Not implemented
-- alter table OPTIONAL_AUTO add constraint ID_OPTIONAL_AUTO_CHK
--     check(exists(select * from supporto
--                  where supporto.P_IVA = P_IVA and supporto.Nome_Optional = Nome_Optional)); 

alter table OPTIONAL_AUTO add constraint FKproduzioneOptional
     foreign key (P_IVA)
     references PRODUTTORE (P_IVA);

alter table OPTIONAL_AUTO add constraint FKequipaggiamento_FK
     foreign key (Nome_Modello)
     references SUPERCAR (Nome_Modello);

-- Not implemented
-- alter table ORDINE add constraint ID_ORDINE_CHK
--     check(exists(select * from VERSIONE
--                  where VERSIONE.Cod_Ordine = Cod_Ordine)); 

alter table ORDINE add constraint FKvendita_FK
     foreign key (Email_Aziendale)
     references DIPENDENTE (Email_Aziendale);

alter table ORDINE add constraint FKacquisto_FK
     foreign key (ID_Badge)
     references CLIENTE (ID_Badge);

alter table STIPENDIO add constraint FKpagamento
     foreign key (Email_Aziendale)
     references DIPENDENTE (Email_Aziendale);

-- Not implemented
-- alter table SUPERCAR add constraint ID_SUPERCAR_CHK
--     check(exists(select * from OPTIONAL_AUTO
--                  where OPTIONAL_AUTO.Nome_Modello = Nome_Modello)); 

-- Not implemented
-- alter table SUPERCAR add constraint ID_SUPERCAR_CHK
--     check(exists(select * from VERSIONE
--                  where VERSIONE.Nome_Modello = Nome_Modello)); 

alter table SUPERCAR add constraint FKproduzioneSupercar_FK
     foreign key (P_IVA)
     references PRODUTTORE (P_IVA);

alter table SUPERCAR add constraint FKcategorizzazione_FK
     foreign key (Nome)
     references SEGMENTO (Nome);

alter table supporto add constraint FKsup_SEG
     foreign key (Nome)
     references SEGMENTO (Nome);

alter table supporto add constraint FKsup_OPT_FK
     foreign key (P_IVA, Nome_Optional)
     references OPTIONAL_AUTO (P_IVA, Nome_Optional);

alter table VERSIONE add constraint FKrestyling_FK
     foreign key (Nome_Modello)
     references SUPERCAR (Nome_Modello);

alter table VERSIONE add constraint FKcontenimento_FK
     foreign key (Cod_Ordine)
     references ORDINE (Cod_Ordine);


-- Index Section
-- _____________ 

create unique index ID_CLIENTE_IND
     on CLIENTE (ID_Badge);

create unique index ID_CONTO_VENDITA_IND
     on CONTO_VENDITA (Cod_Contratto);

create unique index FKrelazione_IND
     on CONTO_VENDITA (Nr_Telaio);

create index FKpiazzamento_IND
     on CONTO_VENDITA (ID_Badge);

create unique index ID_DIPENDENTE_IND
     on DIPENDENTE (Email_Aziendale);

create unique index ID_MANUTENZIONE_IND
     on MANUTENZIONE (Nr_Telaio, Data_Manutenzione);

create unique index ID_OPTIONAL_AUTO_IND
     on OPTIONAL_AUTO (P_IVA, Nome_Optional);

create index FKequipaggiamento_IND
     on OPTIONAL_AUTO (Nome_Modello);

create unique index ID_ORDINE_IND
     on ORDINE (Cod_Ordine);

create index FKvendita_IND
     on ORDINE (Email_Aziendale);

create index FKacquisto_IND
     on ORDINE (ID_Badge);

create unique index ID_PRODUTTORE_IND
     on PRODUTTORE (P_IVA);

create unique index ID_SEGMENTO_IND
     on SEGMENTO (Nome);

create unique index ID_STIPENDIO_IND
     on STIPENDIO (Email_Aziendale, Anno, Mese);

create unique index ID_SUPERCAR_IND
     on SUPERCAR (Nome_Modello);

create index FKproduzioneSupercar_IND
     on SUPERCAR (P_IVA);

create index FKcategorizzazione_IND
     on SUPERCAR (Nome);

create unique index ID_supporto_IND
     on supporto (Nome, P_IVA, Nome_Optional);

create index FKsup_OPT_IND
     on supporto (P_IVA, Nome_Optional);

create unique index ID_VERSIONE_IND
     on VERSIONE (Nr_Telaio);

create index FKrestyling_IND
     on VERSIONE (Nome_Modello);

create index FKcontenimento_IND
     on VERSIONE (Cod_Ordine);

