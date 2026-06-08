async function loadWeather() {
    const city = document.getElementById("city").value;

    const response = await fetch(`/weather?city=${encodeURIComponent(city)}`);
    const data = await response.json();
    const current = data?.current ?? {};
    const temperature = current.temperature_2m ?? "-";
    const rain = current.rain ?? "-";

    document.getElementById("temperature").textContent = temperature === "-" ? "-" : `${temperature} °C`;
    document.getElementById("rain").textContent = rain === "-" ? "-" : `${rain} mm`;
}
