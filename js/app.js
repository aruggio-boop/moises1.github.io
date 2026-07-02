const airports = [
  { code: "CNF", city: "Belo Horizonte", country: "Brasil", label: "Belo Horizonte (CNF)", region: "brasil" },
  { code: "GRU", city: "São Paulo", country: "Brasil", label: "São Paulo (GRU)", region: "brasil" },
  { code: "GIG", city: "Rio de Janeiro", country: "Brasil", label: "Rio de Janeiro (GIG)", region: "brasil" },
  { code: "VCP", city: "Campinas", country: "Brasil", label: "Campinas (VCP)", region: "brasil" },
  { code: "POA", city: "Porto Alegre", country: "Brasil", label: "Porto Alegre (POA)", region: "brasil" },
  { code: "REC", city: "Recife", country: "Brasil", label: "Recife (REC)", region: "brasil" },
  { code: "LIS", city: "Lisboa", country: "Portugal", label: "Lisboa (LIS)", region: "europa" },
  { code: "OPO", city: "Porto", country: "Portugal", label: "Porto (OPO)", region: "europa" },
  { code: "MAD", city: "Madri", country: "Espanha", label: "Madri (MAD)", region: "europa" },
  { code: "BCN", city: "Barcelona", country: "Espanha", label: "Barcelona (BCN)", region: "europa" },
  { code: "FCO", city: "Roma", country: "Itália", label: "Roma (FCO)", region: "europa" },
  { code: "MXP", city: "Milão", country: "Itália", label: "Milão (MXP)", region: "europa" },
  { code: "FLR", city: "Florença", country: "Itália", label: "Florença (FLR)", region: "europa" },
  { code: "VCE", city: "Veneza", country: "Itália", label: "Veneza (VCE)", region: "europa" },
  { code: "MUC", city: "Munique", country: "Alemanha", label: "Munique (MUC)", region: "europa" },
  { code: "CDG", city: "Paris", country: "França", label: "Paris (CDG)", region: "europa" },
  { code: "LHR", city: "Londres", country: "Reino Unido", label: "Londres (LHR)", region: "europa" },
];

const airlines = [
  "LATAM",
  "Azul",
  "Gol",
  "TAP Air Portugal",
  "ITA Airways",
  "Air France",
  "Lufthansa",
  "Iberia",
];

const recentSearchesStorageKey = "de-mala-e-cuia-flight-searches";
const resultsState = {
  query: null,
  flights: [],
};

const form = document.querySelector("#flight-search-form");
const originInput = document.querySelector("#origin");
const destinationInput = document.querySelector("#destination");
const departureDateInput = document.querySelector("#departureDate");
const returnDateInput = document.querySelector("#returnDate");
const budgetInput = document.querySelector("#budget");
const budgetValue = document.querySelector("#budget-value");
const formMessage = document.querySelector("#form-message");
const airportOptions = document.querySelector("#airport-options");
const resultsList = document.querySelector("#results-list");
const resultsSummary = document.querySelector("#results-summary");
const resultsToolbar = document.querySelector("#results-toolbar");
const sortResults = document.querySelector("#sortResults");
const recentSearchesSection = document.querySelector("#recent-searches-section");
const recentSearchesList = document.querySelector("#recent-searches-list");

populateAirports();
setDefaultDates();
syncBudgetValue();
toggleReturnDateField();
renderRecentSearches();

budgetInput.addEventListener("input", syncBudgetValue);
sortResults.addEventListener("change", () => renderResults(resultsState.query, [...resultsState.flights]));
[departureDateInput, returnDateInput].forEach((input) => {
  input.addEventListener("change", syncDateBoundaries);
});

[...form.tripType].forEach((radio) => {
  radio.addEventListener("change", toggleReturnDateField);
});

form.addEventListener("submit", (event) => {
  event.preventDefault();
  formMessage.textContent = "";

  const query = collectFormData();
  const validationError = validateQuery(query);

  if (validationError) {
    formMessage.textContent = validationError;
    return;
  }

  const flights = searchFlights(query);
  resultsState.query = query;
  resultsState.flights = flights;

  saveRecentSearch(query);
  renderRecentSearches();
  renderResults(query, flights);
});

recentSearchesList.addEventListener("click", (event) => {
  const trigger = event.target.closest("[data-search-index]");
  if (!trigger) {
    return;
  }

  const searches = getRecentSearches();
  const selected = searches[Number(trigger.dataset.searchIndex)];

  if (!selected) {
    return;
  }

  applySearchToForm(selected);
  form.requestSubmit();
});

function populateAirports() {
  airportOptions.innerHTML = airports
    .map((airport) => `<option value="${airport.label}">${airport.city}, ${airport.country}</option>`)
    .join("");
}

function setDefaultDates() {
  const today = new Date();
  const departure = addDays(today, 35);
  const returning = addDays(today, 52);
  const minDate = formatDateForInput(today);

  departureDateInput.min = minDate;
  returnDateInput.min = minDate;

  departureDateInput.value = formatDateForInput(departure);
  returnDateInput.value = formatDateForInput(returning);
  syncDateBoundaries();
}

function syncBudgetValue() {
  budgetValue.textContent = `Até ${formatCurrency(Number(budgetInput.value))}`;
}

function toggleReturnDateField() {
  const isOneWay = form.tripType.value === "oneway";
  returnDateInput.disabled = isOneWay;
  returnDateInput.required = !isOneWay;

  if (isOneWay) {
    returnDateInput.dataset.previousValue = returnDateInput.value;
    returnDateInput.value = "";
  } else if (!returnDateInput.value) {
    const previous = returnDateInput.dataset.previousValue;
    returnDateInput.value = previous || formatDateForInput(addDays(new Date(departureDateInput.value), 10));
  }

  syncDateBoundaries();
}

function collectFormData() {
  return {
    tripType: form.tripType.value,
    origin: originInput.value.trim(),
    destination: destinationInput.value.trim(),
    departureDate: departureDateInput.value,
    returnDate: returnDateInput.value,
    passengers: Number(form.passengers.value),
    cabinClass: form.cabinClass.value,
    budget: Number(form.budget.value),
    directOnly: form.directOnly.checked,
  };
}

function validateQuery(query) {
  const origin = resolveAirport(query.origin);
  const destination = resolveAirport(query.destination);

  if (!origin || !destination) {
    return "Selecione origem e destino usando uma das opções sugeridas pela lista.";
  }

  if (origin.code === destination.code) {
    return "Origem e destino precisam ser diferentes para gerar resultados.";
  }

  if (!query.departureDate) {
    return "Informe a data de ida.";
  }

  if (query.tripType === "roundtrip" && !query.returnDate) {
    return "Informe a data de volta para viagens de ida e volta.";
  }

  const departureDate = new Date(`${query.departureDate}T00:00:00`);
  const today = new Date();
  today.setHours(0, 0, 0, 0);

  if (departureDate < today) {
    return "A data de ida precisa ser hoje ou uma data futura.";
  }

  if (query.tripType === "roundtrip") {
    const returnDate = new Date(`${query.returnDate}T00:00:00`);

    if (returnDate <= departureDate) {
      return "A data de volta deve ser posterior à data de ida.";
    }
  }

  if (!Number.isInteger(query.passengers) || query.passengers < 1 || query.passengers > 9) {
    return "Escolha entre 1 e 9 passageiros.";
  }

  return "";
}

function searchFlights(query) {
  const origin = resolveAirport(query.origin);
  const destination = resolveAirport(query.destination);
  const routeBand = getRouteBand(origin, destination);
  const seed = `${origin.code}${destination.code}${query.departureDate}${query.returnDate}${query.cabinClass}${query.tripType}`;
  const random = createSeededRandom(seed);
  const candidates = [];

  for (let index = 0; index < 12; index += 1) {
    const stops = query.directOnly ? 0 : Math.min(2, Math.floor(random() * (routeBand === "long" ? 3 : 2)));
    const airline = airlines[Math.floor(random() * airlines.length)];
    const departureTime = generateDepartureTime(random);
    const durationMinutes = calculateDurationMinutes(routeBand, stops, random);
    const price = calculatePrice(query, routeBand, stops, random);
    const baggageIncluded = random() > 0.25;
    const flexibleChange = random() > 0.48;
    const comfortScore = Math.floor(76 + random() * 22);
    const loyalty = Math.floor(350 + random() * 900);

    candidates.push({
      id: `${origin.code}-${destination.code}-${index}`,
      airline,
      origin,
      destination,
      stops,
      departureTime,
      durationMinutes,
      price,
      baggageIncluded,
      flexibleChange,
      comfortScore,
      loyalty,
      routeBand,
      returnTime: query.tripType === "roundtrip" ? generateDepartureTime(random) : null,
      totalLabel: query.tripType === "roundtrip" ? "ida e volta" : "somente ida",
    });
  }

  const filtered = candidates
    .filter((flight) => flight.price <= query.budget)
    .sort((flightA, flightB) => flightA.price - flightB.price);

  return filtered.slice(0, 6);
}

function renderResults(query, flights) {
  if (!query) {
    resultsToolbar.hidden = true;
    return;
  }

  const sortedFlights = sortFlights(flights, sortResults.value);
  resultsToolbar.hidden = false;

  const origin = resolveAirport(query.origin);
  const destination = resolveAirport(query.destination);
  const formattedDeparture = formatDateLong(query.departureDate);
  const baseSummary = `${sortedFlights.length} ${
    sortedFlights.length === 1 ? "opção encontrada" : "opções encontradas"
  } para ${query.passengers} ${query.passengers === 1 ? "passageiro" : "passageiros"} ${
    query.tripType === "roundtrip" ? "em ida e volta" : "em somente ida"
  } de ${origin.city} para ${destination.city}`;

  resultsSummary.textContent =
    query.tripType === "roundtrip"
      ? `${baseSummary} entre ${formattedDeparture} e ${formatDateLong(query.returnDate)}.`
      : `${baseSummary} com embarque em ${formattedDeparture}.`;

  if (!sortedFlights.length) {
    resultsList.innerHTML = `
      <article class="empty-state">
        <h3>Nenhum voo encontrado dentro do orçamento</h3>
        <p>Tente aumentar o limite máximo ou desmarcar a opção de voos diretos.</p>
      </article>
    `;
    return;
  }

  resultsList.innerHTML = sortedFlights
    .map((flight, index) => {
      const bestValueLabel = index === 0 ? '<span class="pill pill--accent">Melhor custo-benefício</span>' : "";
      const stopsLabel = flight.stops === 0 ? "Voo direto" : `${flight.stops} ${flight.stops === 1 ? "escala" : "escalas"}`;
      const baggageLabel = flight.baggageIncluded ? "1 bagagem despachada" : "Somente bagagem de mão";
      const flexibleLabel = flight.flexibleChange ? "Remarcação flexível" : "Tarifa promocional";

      return `
        <article class="flight-card">
          <div class="flight-card__top">
            <div class="flight-card__route">
              <div class="flight-card__meta">
                <span class="pill">${flight.airline}</span>
                ${bestValueLabel}
                <span class="pill ${flight.stops === 0 ? "pill--success" : ""}">${stopsLabel}</span>
              </div>
              <h3>${flight.origin.city} (${flight.origin.code}) → ${flight.destination.city} (${flight.destination.code})</h3>
              <p>
                Saída às <strong>${flight.departureTime}</strong> · Duração total
                <strong>${formatDuration(flight.durationMinutes)}</strong> · ${flight.totalLabel}
              </p>
            </div>

            <div class="flight-card__price">
              <p>Preço por passageiro</p>
              <strong>${formatCurrency(flight.price)}</strong>
              <p>${query.cabinClass === "business" ? "classe executiva" : query.cabinClass === "premium" ? "premium economy" : "classe econômica"}</p>
            </div>
          </div>

          <div class="flight-card__bottom">
            <div class="flight-card__meta">
              <span class="pill">${baggageLabel}</span>
              <span class="pill">${flexibleLabel}</span>
              <span class="pill">Conforto ${flight.comfortScore}/100</span>
              <span class="pill">+${flight.loyalty} milhas</span>
            </div>
            <p class="pill">Retorno ${flight.returnTime || "não aplicável"}</p>
          </div>
        </article>
      `;
    })
    .join("");
}

function sortFlights(flights, criterion) {
  const copy = [...flights];

  if (criterion === "duration") {
    return copy.sort((flightA, flightB) => flightA.durationMinutes - flightB.durationMinutes);
  }

  if (criterion === "departure") {
    return copy.sort((flightA, flightB) => timeToMinutes(flightA.departureTime) - timeToMinutes(flightB.departureTime));
  }

  return copy.sort((flightA, flightB) => flightA.price - flightB.price);
}

function saveRecentSearch(query) {
  const nextSearches = [query, ...getRecentSearches().filter((item) => !isSameSearch(item, query))].slice(0, 4);

  try {
    localStorage.setItem(recentSearchesStorageKey, JSON.stringify(nextSearches));
  } catch (error) {
    // Ignora falhas de armazenamento local para não interromper a busca.
  }
}

function renderRecentSearches() {
  const searches = getRecentSearches();
  recentSearchesSection.hidden = searches.length === 0;

  recentSearchesList.innerHTML = searches
    .map((search, index) => {
      const origin = resolveAirport(search.origin);
      const destination = resolveAirport(search.destination);
      const tripLabel = search.tripType === "roundtrip" ? "Ida e volta" : "Somente ida";

      return `
        <button type="button" class="recent-searches__item" data-search-index="${index}">
          <strong>${origin.city} → ${destination.city}</strong>
          <small>${tripLabel} · ${search.passengers} ${search.passengers === 1 ? "passageiro" : "passageiros"}</small>
          <small>${formatDateShort(search.departureDate)}${search.returnDate ? ` · ${formatDateShort(search.returnDate)}` : ""}</small>
        </button>
      `;
    })
    .join("");
}

function getRecentSearches() {
  try {
    return JSON.parse(localStorage.getItem(recentSearchesStorageKey)) || [];
  } catch (error) {
    return [];
  }
}

function applySearchToForm(search) {
  form.tripType.value = search.tripType;
  [...form.tripType].forEach((radio) => {
    radio.checked = radio.value === search.tripType;
  });

  originInput.value = search.origin;
  destinationInput.value = search.destination;
  departureDateInput.value = search.departureDate;
  returnDateInput.value = search.returnDate || "";
  form.passengers.value = String(search.passengers);
  form.cabinClass.value = search.cabinClass;
  form.directOnly.checked = search.directOnly;
  budgetInput.value = String(search.budget);

  syncBudgetValue();
  toggleReturnDateField();
}

function resolveAirport(value) {
  const normalized = value.trim().toLowerCase();

  return (
    airports.find((airport) => airport.label.toLowerCase() === normalized) ||
    airports.find((airport) => normalized.includes(airport.code.toLowerCase())) ||
    airports.find((airport) => normalized.includes(airport.city.toLowerCase()))
  );
}

function getRouteBand(origin, destination) {
  if (origin.region === destination.region) {
    return origin.region === "brasil" ? "medium" : "short";
  }

  if (
    (origin.region === "brasil" && destination.region === "europa") ||
    (origin.region === "europa" && destination.region === "brasil")
  ) {
    return "long";
  }

  return "medium";
}

function generateDepartureTime(random) {
  const hour = 4 + Math.floor(random() * 18);
  const minuteBlock = [0, 10, 20, 30, 40, 50][Math.floor(random() * 6)];
  return `${String(hour).padStart(2, "0")}:${String(minuteBlock).padStart(2, "0")}`;
}

function calculateDurationMinutes(routeBand, stops, random) {
  const baseByBand = {
    short: 105,
    medium: 215,
    long: 700,
  };

  const varianceByBand = {
    short: 40,
    medium: 120,
    long: 180,
  };

  const stopPenalty = stops * (routeBand === "long" ? 110 : 55);
  return baseByBand[routeBand] + Math.floor(random() * varianceByBand[routeBand]) + stopPenalty;
}

function calculatePrice(query, routeBand, stops, random) {
  const baseByBand = {
    short: 850,
    medium: 1650,
    long: 4200,
  };

  const cabinMultiplier = {
    economy: 1,
    premium: 1.45,
    business: 2.35,
  };

  const tripMultiplier = query.tripType === "roundtrip" ? 1.72 : 1;
  const stopDiscount = stops === 0 ? 1.1 : 0.96 - stops * 0.04;
  const monthFactor = getMonthFactor(query.departureDate);
  const randomness = 0.9 + random() * 0.22;

  return Math.round(baseByBand[routeBand] * cabinMultiplier[query.cabinClass] * tripMultiplier * stopDiscount * monthFactor * randomness);
}

function getMonthFactor(dateString) {
  const month = new Date(`${dateString}T00:00:00`).getMonth();
  const seasonalFactors = [1.08, 1.04, 1.01, 0.96, 0.94, 0.98, 1.12, 1.18, 1.07, 1.02, 1.06, 1.14];
  return seasonalFactors[month];
}

function isSameSearch(searchA, searchB) {
  return [
    searchA.tripType === searchB.tripType,
    searchA.origin === searchB.origin,
    searchA.destination === searchB.destination,
    searchA.departureDate === searchB.departureDate,
    searchA.returnDate === searchB.returnDate,
    searchA.passengers === searchB.passengers,
    searchA.cabinClass === searchB.cabinClass,
    searchA.budget === searchB.budget,
    searchA.directOnly === searchB.directOnly,
  ].every(Boolean);
}

function createSeededRandom(seedText) {
  let seed = 0;

  for (let index = 0; index < seedText.length; index += 1) {
    seed = (seed << 5) - seed + seedText.charCodeAt(index);
    seed |= 0;
  }

  return function seededRandom() {
    seed += 0x6d2b79f5;
    let value = seed;
    value = Math.imul(value ^ (value >>> 15), value | 1);
    value ^= value + Math.imul(value ^ (value >>> 7), value | 61);
    return ((value ^ (value >>> 14)) >>> 0) / 4294967296;
  };
}

function addDays(date, days) {
  const copy = new Date(date);
  copy.setDate(copy.getDate() + days);
  return copy;
}

function formatDateForInput(date) {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  return `${year}-${month}-${day}`;
}

function formatDateLong(dateString) {
  return new Intl.DateTimeFormat("pt-BR", {
    day: "2-digit",
    month: "long",
    year: "numeric",
  }).format(new Date(`${dateString}T12:00:00`));
}

function formatDateShort(dateString) {
  return new Intl.DateTimeFormat("pt-BR", {
    day: "2-digit",
    month: "2-digit",
  }).format(new Date(`${dateString}T12:00:00`));
}

function formatCurrency(value) {
  return new Intl.NumberFormat("pt-BR", {
    style: "currency",
    currency: "BRL",
    maximumFractionDigits: 0,
  }).format(value);
}

function formatDuration(totalMinutes) {
  const hours = Math.floor(totalMinutes / 60);
  const minutes = totalMinutes % 60;
  return `${hours}h ${String(minutes).padStart(2, "0")}min`;
}

function timeToMinutes(time) {
  const [hours, minutes] = time.split(":").map(Number);
  return hours * 60 + minutes;
}

function syncDateBoundaries() {
  if (!departureDateInput.value) {
    return;
  }

  returnDateInput.min = departureDateInput.value;

  if (returnDateInput.value && returnDateInput.value <= departureDateInput.value) {
    returnDateInput.value = formatDateForInput(addDays(new Date(`${departureDateInput.value}T00:00:00`), 10));
  }
}
