import { useState } from "react";


interface Currency {
    CharCode: string
    Name: string
    Nominal: number
    Value: number
    Previous: number
}
interface DailyRates {
    Date: string
    Valute: Record<string, Currency>
}


function FetchRates() {
    const [rates, setRates] = useState<Currency[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [date, setDate] = useState<string | null>(null);

    async function handlerClick()
    {
        setLoading(true);
        setError(null);
        setRates([]);
        setDate(null);

        try {
            const response = await fetch("https://www.cbr-xml-daily.ru/daily_json.js");

            if (!response.ok) {
                setError(`Ошибка HTTP: ${response.status}`);
                return;
            }

            const data = await response.json() as DailyRates;
            setDate(data.Date);
            setRates(Object.values(data.Valute));
        }
        catch (error) {
            setError("Не удалось загрузить курсы валют");
        }
        finally {
            setLoading(false);
        }
    }
    

    return (
        <div>
            <h1>Курсы валют</h1>
            <button onClick={handlerClick}>Загрузить курсы</button>
            {loading && <p>Загрузка...</p>}
            {error && <p>Ошибка: {error}</p>}
            {date && <p>Дата: {date}</p>}
            {rates.length > 0 && <ul>
                {rates.map((rate) => (
                    <li key={rate.CharCode}>{rate.CharCode} - {rate.Value}</li>
                ))}
            </ul>}
        </div>
    )
}

export default FetchRates;