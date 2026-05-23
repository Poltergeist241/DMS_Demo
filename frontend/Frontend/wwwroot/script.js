async function loadWeather() {
    const city = document.getElementById("city").value;

    const response = await fetch(`/weather?city=${encodeURIComponent(city)}`);
    const data = await response.json();
    const current = data?.current ?? {};
    const temperature = current.temperature_2m ?? "-";
    const rain = current.rain ?? "-";

    document.getElementById("result").textContent = `Temperatur: ${temperature} °C\nRegen: ${rain} mm`;
}
