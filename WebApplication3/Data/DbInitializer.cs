using WebApplication3.Controllers;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Models;
using System;
using System.Collections.Generic;

namespace WebApplication3.Data;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        context.Database.EnsureCreated();

        // Delete and recreate database on each startup
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Locations
        var locations = new List<Location>
        {
            new Location
            {
                Name = "Gestolen Bakfiets Centrale", Address = "Stationsplein 14, Utrecht", Latitude = 52.089,
                Longitude = 5.110
            },
            new Location
            {
                Name = "Foute Snorrenclub", Address = "Snorrenstraat 23, Gent", Latitude = 51.054, Longitude = 3.720
            },
            new Location
            {
                Name = "Verdachte Frikandellenkraam", Address = "Marktplein 7, Rotterdam", Latitude = 51.922,
                Longitude = 4.481
            },
            new Location
            {
                Name = "Koekjesdiefstal Museum", Address = "Speculooslaan 42, Hasselt", Latitude = 50.931,
                Longitude = 5.337
            },
            new Location
            {
                Name = "Winkelcentrum Woensel", Address = "Woenselse Markt 7, Eindhoven", Latitude = 51.465,
                Longitude = 5.481
            },
            new Location
            {
                Name = "Het Omgekeerde Fietsenhok", Address = "Kettingstraat 18, Amsterdam", Latitude = 52.376,
                Longitude = 4.904
            },
            new Location
            {
                Name = "Illegale Stroopwafelbrouwerij", Address = "Siroopsteeg 9, Gouda", Latitude = 52.011,
                Longitude = 4.711
            },
            new Location
            {
                Name = "Ondergrondse Hagelslag Bunker", Address = "Chocoladeweg 77, Breda", Latitude = 51.585,
                Longitude = 4.775
            },
            new Location
            {
                Name = "Nachtelijke Kaasrollersbaan", Address = "Edammer Helling 34, Alkmaar", Latitude = 52.631,
                Longitude = 4.751
            },
            new Location
            {
                Name = "Het Gefluister Café", Address = "Stiltelaan 3, Maastricht", Latitude = 50.848, Longitude = 5.690
            },
            new Location
            {
                Name = "De Bizarre Kapsalon", Address = "Schaarweg 66, Den Haag", Latitude = 52.077, Longitude = 4.313
            },
            new Location
            {
                Name = "Verdwijnende Parapluwinkel", Address = "Regenstraat 101, Groningen", Latitude = 53.219,
                Longitude = 6.568
            }
        };

        context.Locations.AddRange(locations);
        context.SaveChanges();

        // Users
        var users = new List<User>
        {
            new User
            {
                FirstName = "Vin", LastName = "Diesel", Auth0Id = "auth0|thiefmaster123",
                Email = "bakfietsbende@example.com"
            },
            new User
            {
                FirstName = "Baltzaar", LastName = "Boma", Auth0Id = "auth0|snorman456",
                Email = "snorclubpresident@example.com"
            },
            new User
            {
                FirstName = "Tommeke", LastName = "Gijsbers", Auth0Id = "auth0|frikandelspy789",
                Email = "worstlover@example.com"
            },
            new User
            {
                FirstName = "Koen", LastName = "Kruimel", Auth0Id = "auth0|vlindercrime911", Email = "Testeroni@test.nl"
            },
            new User
            {
                FirstName = "Fleur", LastName = "Fietsdief", Auth0Id = "auth0|wheeldealer42",
                Email = "fietsenfans@example.com"
            },
            new User
            {
                FirstName = "Henk", LastName = "Hagelslag", Auth0Id = "auth0|sprinklemonster",
                Email = "broodbeleg@example.com"
            },
            new User
            {
                FirstName = "Sophie", LastName = "Stroopwafel", Auth0Id = "auth0|syrupninja",
                Email = "wafelfanaat@example.com"
            },
            new User
            {
                FirstName = "Jeroen", LastName = "Jokkebrok", Auth0Id = "auth0|prankmaster99",
                Email = "grappenmaker@example.com"
            },
            new User
            {
                FirstName = "Lotte", LastName = "Lampkap", Auth0Id = "auth0|shaderaider22",
                Email = "donkerehoekjes@example.com"
            },
            new User
            {
                FirstName = "Pieter", LastName = "Pannekoek", Auth0Id = "auth0|flippingpro",
                Email = "pannenkoekvriend@example.com"
            },
            new User
            {
                FirstName = "Tanja", LastName = "Tompoes", Auth0Id = "auth0|creamqueen",
                Email = "gebakliefhebber@example.com"
            },
            new User
            {
                FirstName = "Willem", LastName = "Waanzin", Auth0Id = "auth0|crazyideas666",
                Email = "gekkeplannen@example.com"
            }
        };

        context.Users.AddRange(users);
        context.SaveChanges();

        // Events - past, present and future with varying themes
        var now = DateTime.Now;
        var events = new List<Event>
        {
            new Event
            {
                Name = "Middernachtelijke Fietsenroof",
                Description =
                    "Samen gezellig fietsen stelen onder het maanlicht. Verkleed je in het zwart en neem je eigen kniptang mee. Winnaar is wie de duurste fiets bemachtigt.",
                Location = locations[0], EventDate = now.AddDays(7)
            },
            new Event
            {
                Name = "Workshop Snorrenvermomming",
                Description =
                    "Leer hoe je onherkenbaar wordt met een perfecte snor. Diverse stijlen komen aan bod: de Poirot, de Handlebar en de beruchte Walrus.",
                Location = locations[1], EventDate = now.AddDays(14)
            },
            new Event
            {
                Name = "Verdachte Worsten BBQ",
                Description =
                    "Frikandellen eten van dubieuze herkomst. Niemand weet wat erin zit, maar het smaakt verrassend goed!",
                Location = locations[2], EventDate = now.AddDays(21)
            },
            new Event
            {
                Name = "Koekjesroof Speurtocht",
                Description =
                    "Vind de geheime stash speculaas. Aanwijzingen zijn verstopt in de hele stad. Breng je eigen zak mee om je buit in te verzamelen.",
                Location = locations[3], EventDate = now.AddMonths(1)
            },
            new Event
            {
                Name = "Mysterieus winkelmandje vullen",
                Description =
                    "Wie vult het meest bizarre winkelmandje? De jury beoordeelt op creativiteit, shock-factor en absurditeit.",
                Location = locations[4], EventDate = now.AddMonths(2)
            },
            new Event
            {
                Name = "Omgekeerde Fietsrace",
                Description =
                    "Fiets zo langzaam mogelijk zonder om te vallen. Laatste over de finish wint. Strikte regels tegen stilstaan!",
                Location = locations[5], EventDate = now.AddDays(-14)
            },
            new Event
            {
                Name = "Stiekeme Stroopwafel Expeditie",
                Description =
                    "Infiltreer de stroopwafelfabriek en ontdek het geheime recept. Kom verkleed als meelzak om niet op te vallen.",
                Location = locations[6], EventDate = now.AddDays(5)
            },
            new Event
            {
                Name = "Hagelslag Proeverij XL",
                Description =
                    "Blind-tasting van 42 soorten hagelslag. Kun jij de melkchocolade van de pure onderscheiden met een blinddoek om?",
                Location = locations[7], EventDate = now.AddDays(-5)
            },
            new Event
            {
                Name = "Nachtelijke Kaasrol Marathon",
                Description =
                    "Een hele nacht kazen van de helling rollen. Helm verplicht! Winnaar krijgt een gouden kaasschaaf.",
                Location = locations[8], EventDate = now.AddDays(18)
            },
            new Event
            {
                Name = "Fluisterfeest",
                Description =
                    "Een hele avond feesten waarbij alleen gefluisterd mag worden. Schreeuwen = onmiddellijk verwijderd worden!",
                Location = locations[9], EventDate = now.AddDays(-20)
            },
            new Event
            {
                Name = "Gekke Kapseldag",
                Description =
                    "Laat je haar transformeren tot een kunstwerk. Wie het meest bizarre kapsel durft te dragen, wint een jaar gratis haarproducten.",
                Location = locations[10], EventDate = now.AddDays(10)
            },
            new Event
            {
                Name = "Paraplu Verstoppertje",
                Description =
                    "Verstop je paraplu ergens in de stad. Wie de meeste paraplu's van anderen vindt, is winnaar. Let op: paraplu's moeten aan het eind geretourneerd worden!",
                Location = locations[11], EventDate = now.AddDays(3)
            },
            new Event
            {
                Name = "Nep-Buitenaards Bezoek Organiseren",
                Description =
                    "Help mee met het creëren van een nep UFO-landing. Inclusief workshop aluminiumhoedjes vouwen.",
                Location = locations[5], EventDate = now.AddDays(25)
            },
            new Event
            {
                Name = "Onderwater Schaaktoernooi",
                Description =
                    "Schaak terwijl je in het zwembad bent. Waterdichte schaakstukken worden voorzien. Zuurstoftank zelf meenemen.",
                Location = locations[3], EventDate = now.AddDays(30)
            },
            new Event
            {
                Name = "Geheimschrift Ontcijferen",
                Description =
                    "Kraak de code en vind de verborgen schat. Voorkennis van hiërogliefen is een pre maar geen vereiste.",
                Location = locations[8], EventDate = now.AddDays(-30)
            }
        };

        context.Events.AddRange(events);
        context.SaveChanges();

        // Event Registrations - showing active participation patterns
        var registrations = new List<EventRegistration>
        {
            
            new EventRegistration { UserId = users[0].Id, EventId = events[5].Id, RegisteredAt = now.AddDays(-30) },
            new EventRegistration { UserId = users[1].Id, EventId = events[5].Id, RegisteredAt = now.AddDays(-29) },
            new EventRegistration { UserId = users[2].Id, EventId = events[5].Id, RegisteredAt = now.AddDays(-28) },
            new EventRegistration { UserId = users[4].Id, EventId = events[5].Id, RegisteredAt = now.AddDays(-27) },
            new EventRegistration { UserId = users[7].Id, EventId = events[5].Id, RegisteredAt = now.AddDays(-25) },
            new EventRegistration { UserId = users[3].Id, EventId = events[7].Id, RegisteredAt = now.AddDays(-15) },
            new EventRegistration { UserId = users[5].Id, EventId = events[7].Id, RegisteredAt = now.AddDays(-14) },
            new EventRegistration { UserId = users[9].Id, EventId = events[7].Id, RegisteredAt = now.AddDays(-13) },
            new EventRegistration { UserId = users[2].Id, EventId = events[9].Id, RegisteredAt = now.AddDays(-40) },
            new EventRegistration { UserId = users[6].Id, EventId = events[9].Id, RegisteredAt = now.AddDays(-38) },
            new EventRegistration { UserId = users[8].Id, EventId = events[9].Id, RegisteredAt = now.AddDays(-35) },
            new EventRegistration { UserId = users[10].Id, EventId = events[9].Id, RegisteredAt = now.AddDays(-32) },
            new EventRegistration { UserId = users[0].Id, EventId = events[0].Id, RegisteredAt = now.AddHours(-72) },
            new EventRegistration { UserId = users[2].Id, EventId = events[0].Id, RegisteredAt = now.AddHours(-65) },
            new EventRegistration { UserId = users[4].Id, EventId = events[0].Id, RegisteredAt = now.AddHours(-60) },
            new EventRegistration { UserId = users[6].Id, EventId = events[0].Id, RegisteredAt = now.AddHours(-48) },
            new EventRegistration { UserId = users[8].Id, EventId = events[0].Id, RegisteredAt = now.AddHours(-24) },
            new EventRegistration { UserId = users[10].Id, EventId = events[0].Id, RegisteredAt = now.AddHours(-12) },
            new EventRegistration { UserId = users[1].Id, EventId = events[1].Id, RegisteredAt = now.AddHours(-100) },
            new EventRegistration { UserId = users[3].Id, EventId = events[1].Id, RegisteredAt = now.AddHours(-90) },
            new EventRegistration { UserId = users[5].Id, EventId = events[1].Id, RegisteredAt = now.AddHours(-80) },
            new EventRegistration { UserId = users[11].Id, EventId = events[1].Id, RegisteredAt = now.AddHours(-70) },

            new EventRegistration { UserId = users[2].Id, EventId = events[2].Id, RegisteredAt = now.AddMinutes(-45) },
            new EventRegistration { UserId = users[4].Id, EventId = events[2].Id, RegisteredAt = now.AddMinutes(-40) },
            new EventRegistration { UserId = users[7].Id, EventId = events[2].Id, RegisteredAt = now.AddMinutes(-30) },

            new EventRegistration { UserId = users[0].Id, EventId = events[3].Id, RegisteredAt = now.AddMinutes(-30) },
            new EventRegistration { UserId = users[3].Id, EventId = events[3].Id, RegisteredAt = now.AddMinutes(-25) },
            new EventRegistration { UserId = users[9].Id, EventId = events[3].Id, RegisteredAt = now.AddMinutes(-20) },

            
            new EventRegistration { UserId = users[1].Id, EventId = events[4].Id, RegisteredAt = now.AddMinutes(-15) },
            new EventRegistration { UserId = users[5].Id, EventId = events[4].Id, RegisteredAt = now.AddMinutes(-10) },

            new EventRegistration { UserId = users[6].Id, EventId = events[6].Id, RegisteredAt = now.AddHours(-5) },
            new EventRegistration { UserId = users[8].Id, EventId = events[6].Id, RegisteredAt = now.AddHours(-4) },

            new EventRegistration { UserId = users[7].Id, EventId = events[8].Id, RegisteredAt = now.AddDays(-1) },
            new EventRegistration { UserId = users[11].Id, EventId = events[8].Id, RegisteredAt = now.AddHours(-20) },

            
            new EventRegistration { UserId = users[0].Id, EventId = events[11].Id, RegisteredAt = now.AddMinutes(-5) },
            new EventRegistration { UserId = users[10].Id, EventId = events[11].Id, RegisteredAt = now.AddMinutes(-3) },
            new EventRegistration { UserId = users[5].Id, EventId = events[11].Id, RegisteredAt = now.AddMinutes(-1) }
        };
        context.EventRegistrations.AddRange(registrations);

        // Event Feedback - with varied opinions and reactions
        var feedback = new List<EventFeedback>
        {
            // Feedback for past events
            new EventFeedback
            {
                EventId = events[5].Id, UserId = users[0].Id,
                Message = "Geweldige avond! Mijn fiets ging zo langzaam dat hij bijna achteruit reed!"
            },
            new EventFeedback
            {
                EventId = events[5].Id, UserId = users[2].Id,
                Message = "Volgende keer breng ik mijn oma mee, zij kan nog langzamer fietsen."
            },
            new EventFeedback
            {
                EventId = events[5].Id, UserId = users[4].Id,
                Message = "Ik viel na 5 meter al om. 10/10 zou weer deelnemen!"
            },

            new EventFeedback
            {
                EventId = events[7].Id, UserId = users[3].Id,
                Message = "Ik kon de pure en melk perfect onderscheiden, maar hoe zit het met mijn hond?"
            },
            new EventFeedback
            {
                EventId = events[7].Id, UserId = users[5].Id,
                Message = "Mijn tong is nu zwart van alle chocolade. Wanneer is de volgende proeverij?"
            },

            new EventFeedback
            {
                EventId = events[9].Id, UserId = users[6].Id,
                Message = "IEMAND SCHREEUWDE BIJ DE BAR! Het was hilarisch toen security hem in slowmotion wegdroeg."
            },
            new EventFeedback
            {
                EventId = events[9].Id, UserId = users[8].Id,
                Message = "Ik heb gefluisterd tot ik schor was. Perfect feestje voor introverten!"
            },

            // Feedback for upcoming events
            new EventFeedback
            {
                EventId = events[0].Id, UserId = users[0].Id,
                Message = "Kan niet wachten! Mijn kniptang is al gesmeerd."
            },
            new EventFeedback
            {
                EventId = events[0].Id, UserId = users[4].Id,
                Message = "Is een bakfiets stelen extra punten waard? Vraag voor een vriend..."
            },
            new EventFeedback
            {
                EventId = events[0].Id, UserId = users[6].Id,
                Message = "Ik heb zwarte kleding en een bivakmuts klaarliggen. Dit wordt episch!"
            },

            new EventFeedback
            {
                EventId = events[1].Id, UserId = users[1].Id,
                Message = "Ik oefen al weken voor de perfecte Poirot. Mijn snorwax is ingeslagen!"
            },
            new EventFeedback
            {
                EventId = events[1].Id, UserId = users[3].Id,
                Message = "Werkt dit ook voor vrouwen? Ik wil graag onherkenbaar naar mijn schoonmoeder."
            },

            new EventFeedback
            {
                EventId = events[2].Id, UserId = users[2].Id,
                Message = "Lijkt me leuk, maar ik ben lichtelijk bezorgd over de herkomst van het vlees..."
            },
            new EventFeedback
            {
                EventId = events[2].Id, UserId = users[7].Id,
                Message = "Hoe dubieuzer, hoe beter! Neem extra saus mee, mensen!"
            },

            // Most recent feedback
            new EventFeedback
            {
                EventId = events[11].Id, UserId = users[0].Id,
                Message =
                    "Waar kan ik het best mijn paraplu verstoppen? In een paraplustandaard is te voor de hand liggend."
            },
            new EventFeedback
            {
                EventId = events[11].Id, UserId = users[10].Id,
                Message = "Wat gebeurt er als het regent tijdens dit evenement? Extra uitdaging?"
            },
            new EventFeedback
            {
                EventId = events[2].Id, // Verdachte Worsten BBQ
                UserId = users[9].Id, // Pieter Pannekoek
                Message = "Heb m'n eigen brandblusser bij voor als de worst vlam vat. Safety first, dysenterie second!"
            },
            new EventFeedback
            {
                EventId = events[3].Id, // Koekjesroof Speurtocht
                UserId = users[3].Id, // Koen Kruimel
                Message = "Ik neem een kruimeldief mee. Niet om schoon te maken, maar om de buit sneller op te zuigen."
            },
            new EventFeedback
            {
                EventId = events[4].Id, // Mysterieus Winkelmandje vullen
                UserId = users[2].Id, // Tommeke Gijsbers
                Message =
                    "Mijn mandje bevat nu een opblaaskrokodil, glitter-tandpasta en één avocado. Kunst of pure waanzin?"
            },
            new EventFeedback
            {
                EventId = events[6].Id, // Stiekeme Stroopwafel Expeditie
                UserId = users[10].Id, // Tanja Tompoes
                Message = "Camouflageplan: mezelf oprollen als stroopwafel. Als ik verdwijn, zoek me in de siroopketel."
            },
            new EventFeedback
            {
                EventId = events[7].Id, // Hagelslag Proeverij XL
                UserId = users[1].Id, // Baltzaar Boma
                Message = "Heb de hagelslag alfabetisch gerangschikt. OCD of MVP? U beslist!"
            },
            new EventFeedback
            {
                EventId = events[8].Id, // Nachtelijke Kaasrol Marathon
                UserId = users[5].Id, // Henk Hagelslag
                Message = "Kaas + zwaartekracht = kunst. Ik roep 'GOUDSE POWER' bij elke rol."
            },
            new EventFeedback
            {
                EventId = events[10].Id, // Gekke Kapseldag
                UserId = users[4].Id, // Fleur Fietsdief
                Message = "Mijn haar is nu een bakfiets-parade. Hoop dat het door de deur past!"
            },
            new EventFeedback
            {
                EventId = events[12].Id, // Nep-Buitenaards Bezoek
                UserId = users[11].Id, // Willem Waanzin
                Message = "Heeft iemand nog zilverfolie? Mijn helm moet de zichtbare wifi-signalen blokkeren."
            },
            new EventFeedback
            {
                EventId = events[13].Id, // Onderwater Schaaktoernooi
                UserId = users[8].Id, // Lotte Lampkap
                Message = "Verlies ik, dan doe ik de ‘verdrinkende pion’-dans. Entertainment gegarandeerd."
            },
            new EventFeedback
            {
                EventId = events[14].Id, // Geheimschrift Ontcijferen
                UserId = users[6].Id, // Sophie Stroopwafel
                Message = "🦝🔑🍪 — als je dit begrijpt, ben je aangenomen als co-codekraker!"
            }
        };


        context.EventFeedbacks.AddRange(feedback);
        context.SaveChanges();
    }
}