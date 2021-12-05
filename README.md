# Portál člena studentského klubu U Kachničky (Kachna Online)

Je Kachna otevřená? To je oč tu běží.

Kachna Online je aplikace vytvořena s cílem centralizovat informace týkající se studentského klubu U Kachničky na jedno lehce dostupné a přehledné místo. Nejvýznamnějším řešeným problémem je informování studentů fakulty o otvírání klubu, protože nemá smysl jej otvírat, pokud se o tom cílová skupina nedozví. Dále se však studenti mohou v portálu dozvědět o nadcházejících akcích Studentské unie, nebo si vypůjčit z klubu některou z velké nabídky deskových her.

Produkčně je Kachna Online nasazena pod [Studentskou unií](https://www.su.fit.vutbr.cz/kachna/) a dokumentace k API je dostupná [tamtéž](https://www.su.fit.vutbr.cz/kachna/api/swagger/index.html). Přihlášení probíhá pomocí interního systému Studentské unie (tedy pomocí federace identit eduID.cz).

## Struktura projektu

Aplikace se skládá z backendu postaveného na platformě **.NET 6** s databází **PostgreSQL** a frontendu postaveného na frameworku **Angular 12**. Vzájemná komunikace těchto dvou částí probíhá pomocí HTTP REST API.

Implementace backendu se skládá z pěti projektů:

- **KachnaOnline.App**: Hlavní projekt, ve kterém jsou implementovány kontroléry, které jsou prostředníkem mezi uživatelem a business vrstvou.
- **KachnaOnline.Dto**: Deklarace datových struktur, které jsou využívány pro komunikaci s klientskou aplikací
- **KachnaOnline.Business**: Business vrstva imlementující aplikační logiku.
- **KachnaOnline.Business.Data a KachnaOnline.Data**: Vrstvy zajišťující přístup business vrstvy k datům

Frontend je možné najít v adresáři **ClientApp/KachnaOnline**, je členěn na menší Angular moduly, které se skládají z jednotlivých komponent. Za zmínku zde stojí moduly implementující hlavní klientskou část:

- **board-games**: Implementuje uživatelské rozhraní pro deskové hry, jejich správu a také rezervace.
- **events**: Implementuje rozhraní pro nahlížení na akce Studentské unie a jejich správu.
- **home**: Domovská hlavní stránka aplikace.
- **navigation-bar**: Navigace po stránce.
- **states**: Implementuje rozhraní pro správu stavů klubu.
- **users**: Rozhraní pro správu uživatelských profilů a rolí.
- **shared**: Obsahuje implementaci služeb komunikujících s backendem a dále také některé komponenty, funkce a třídy, které jsou používány napříč aplikací.

- **models**: Obsahuje deklarace datových struktur používaných pro komunikaci s backendem. 

## Závislosti

Pro lokální spuštění aplikace je nutné mít nainstalovaný a nastavený SŘBD [PostgreSQL](https://www.postgresql.org/download/), platformu [.NET 6](https://dotnet.microsoft.com/download) a framework [Angular](https://angular.io/guide/setup-local) (pro jeho funkčnost je vyžadován správce balíků npm a prostředí Node.js). Kromě těchto technologií využívá aplikace několik knihoven, které ovšem budou automaticky nainstalovány při prvním spuštění projektu, viz sekce Spuštění. Tyto knihovny ovšem jistě stojí za zmínku:

- [**CSS knihovna Bootstrap**](https://getbootstrap.com/) usnadňující stylování aplikace a zajištění její responzivity na různá zařízení.
- [**ng-bootstrap**](https://ng-bootstrap.github.io/#/home) poskytující některé nativní Angular komponenty vytvořené pomocí knihovny Bootstrap pro urychlení vývoje.

- [**Angular service worker**](https://angular.io/guide/service-worker-intro) umožňující přijímat push notifikace.
- [**fullcalendar**](https://fullcalendar.io/) pro implementaci kalendáře na domovské stránce.
- [**angular-jwt**](https://www.npmjs.com/package/@auth0/angular-jwt) pro práci s JWT tokeny, pomocí kterých probíhá komunikace s interním systémem SU a také backendem Kachna Online.
- [**ngx-toastr**](https://www.npmjs.com/package/ngx-toastr) pro zobrazování zpráv uživateli.

## Spuštění

Pro spuštění je nutné provést konfiguraci backendu v souboru *KachnaOnline.App/appsettings.json*, význám jednotlivých nastavení je možné najít v odpovídajících modelech v adresáři *KachnaOnline.Business/Configuration*. Primárně je nutné nastavit připojení k databázi v sekci *ConnectionStrings:AppDb*, příklad takového nastavení je:

```
Host=127.0.0.1;Database=kachna-online;Username=[username];Password=[password]
```

Před spuštěním v běžném režimu je nutné provést migraci databáze a také je možné naplnit ji ukázkovými daty z adresáře **KachnaOnline.App**.

```
dotnet run --migrate-db --bootstrap-db
```

Poté je možné aplikaci spustit již v běžném režimu.

```
dotnet run
```

Frontend je možné spusti pomocí

```
npm run start
```

a následně je dostupný v prohlížeči na https://localhost:4200/kachna/ . V takovémto nastavení ovšem nebudou fungovat push notifikace, Angular service worker totiž musí běžet přes plnohodnotný HTTP server. K tomu je možné využít například *http-server* dostupný pomocí správce balíčků *npm*, instrukce pro spuštění je možné nalézt v [Angular dokumentaci](https://angular.io/guide/service-worker-getting-started#service-worker-in-action-a-tour).

## Autoři
**Pracovní skupina Sluníčka**

František Nečas (xnecas27)
David Chocholatý (xchoch08)
Ondřej Ondryáš (xondry02)

