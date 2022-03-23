# Portál člena studentského klubu U Kachničky (Kachna Online)

Je Kachna otevřená? To je, oč tu běží.

Kachna Online je aplikace vytvořena s cílem centralizovat informace týkající se studentského klubu U Kachničky na jedno lehce dostupné a přehledné místo. Nejvýznamnějším řešeným problémem je informování studentů fakulty o otvírání klubu, protože nemá smysl jej otvírat, pokud se o tom cílová skupina nedozví. Dále se však studenti mohou v portálu dozvědět o nadcházejících akcích Studentské unie, nebo si vypůjčit z klubu některou z velké nabídky deskových her.

Produkčně je portál Kachna Online nasazen na serveru [Studentské unie FIT VUT v Brně](https://www.su.fit.vutbr.cz/kachna/). Dokumentace k API je dostupná ve formě OpenAPI dokumentu a Swagger UI [tamtéž](https://www.su.fit.vutbr.cz/kachna/api/swagger/index.html). Přihlášení probíhá pomocí interního systému Studentské unie *KIS*, který využívá federaci identit eduID.cz.

## Struktura projektu

Aplikace se skládá z backendu postaveného na platformě **.NET 6** s databází **PostgreSQL** a frontendu (uživatelské aplikace) postaveného na frameworku **Angular 12**. Vzájemná komunikace těchto dvou částí probíhá pomocí HTTP REST API.

Implementace backendu se skládá z pěti projektů:

- **KachnaOnline.App**: Hlavní projekt, ve kterém jsou implementovány kontroléry, které jsou prostředníkem mezi uživatelem a business vrstvou.
- **KachnaOnline.Dto**: Deklarace datových struktur, které jsou využívány pro komunikaci s klientskou aplikací.
- **KachnaOnline.Business**: Business vrstva imlementující aplikační logiku.
- **KachnaOnline.Business.Data a KachnaOnline.Data**: Vrstvy zajišťující přístup business vrstvy k datům.

Frontend je možné najít v adresáři `ClientApp/KachnaOnline`.

## Závislosti

Pro lokální spuštění aplikace je nutné mít nainstalovaný a nastavený [PostgreSQL](https://www.postgresql.org/download/), běhové prostředí [.NET 6](https://dotnet.microsoft.com/download) a framework [Angular](https://angular.io/guide/setup-local) (pro jeho funkčnost je vyžadováno prostředí [Node.js](https://nodejs.org/en/download/) a jeho správce balíčků npm).

Úplný seznam knihoven frontendu je k dispozici v souboru `ClientApp/KachnaOnline/package.json`. Knihovny využívané backendem budou automaticky staženy při spuštění. Jejich seznam je možné najít v jednotlivých projektových souborech `.csproj`.

## Spuštění

### Backend

Pro spuštění je nutné provést konfiguraci backendu v souboru `KachnaOnline.App/appsettings.json`, význam jednotlivých nastavení je možné najít v odpovídajících modelech v adresáři `KachnaOnline.Business/Configuration`. Primárně je nutné nastavit připojení k databázi v sekci `ConnectionStrings:AppDb`, příklad takového nastavení je:

```
Host=127.0.0.1;Database=kachna-online;Username=[username];Password=[password]
```

Spuštění aplikace je nutné provést v adresáři hlavního projektu, `KachnaOnline.App`.

Před spuštěním v běžném režimu je nutné provést migraci databáze. Také je možné naplnit ji ukázkovými daty:

```
dotnet run - --migrate-db --bootstrap-db
```

Poté je možné aplikaci spustit v běžném režimu:

```
dotnet run
```

### Frontend

Frontend se spouští z adresáře `ClientApp/KachnaOnline`.


Před prvním spuštěním frontendu je nutné stáhnout knihovny pomocí nástroje npm:
```
npm install
```

Ve výchozím stavu je spouštěcí skript nastaven tak, aby spustil vývojový server s podporou SSL a s užitím vývojového certifikátu z ASP.NET backendu. Ten může být nutné vyexportovat přiloženým skriptem:

```
node aspnetcore-https
```

Vývojový server je poté možné spustit pomocí:
```
npm run start
```

Alternativou je spustit vývojový server v nezabezpečeném režimu:
```
npm run start-http
```

Následně je aplikace dostupná v prohlížeči na https://localhost:4200/kachna/. V takovémto nastavení ovšem nebudou fungovat push notifikace, Angular service worker totiž musí běžet přes plnohodnotný HTTP server. K tomu je možné využít například *http-server* dostupný pomocí správce balíčků *npm*, instrukce pro spuštění je možné nalézt v [Angular dokumentaci](https://angular.io/guide/service-worker-getting-started#service-worker-in-action-a-tour).
