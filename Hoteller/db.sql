create table Hotels
(
    Id           int auto_increment
        primary key,
    Name         varchar(255) not null,
    PhoneNumber  varchar(20)  not null,
    EmailAddress varchar(255) not null,
    Username     varchar(255) not null,
    Password     varchar(255) not null,
    img          varchar(250) null,
    detail       text         null,
    size         varchar(255) null,
    room_number  varchar(255) null,
    price        varchar(255) null
);

create table rezervasyonlar
(
    id               int auto_increment
        primary key,
    oda_id           int          not null,
    giris_tarihi     date         not null,
    cikis_tarihi     date         not null,
    isim_soyisim     varchar(100) not null,
    telefon_numarasi varchar(20)  not null,
    constraint rezervasyonlar_ibfk_1
        foreign key (oda_id) references hotels (Id)
);

create index oda_id
    on rezervasyonlar (oda_id);

create table slider
(
    id        int auto_increment
        primary key,
    title     varchar(255) not null,
    image_url varchar(255) not null
);